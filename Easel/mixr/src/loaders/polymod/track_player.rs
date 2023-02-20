use crate::ChannelProperties;

use super::{track::Track, PianoKey, Effect, sample::Sample, Note};

pub const SAMPLE_RATE: i32 = 48000;

struct TrackChannel {
    properties: ChannelProperties,
    enabled: bool,

    current_sample: Option<u8>,
    note_volume: u8,

    vol_memory: u8,
    pitch_memory: u8,

    offset_memory: u8,
    high_offset: usize
}

pub struct TrackPlayer {
    track: Track,
    system: crate::system::AudioSystem,
    buffers: Vec<i32>,

    current_half_sample: u32,
    half_samples_per_tick: u32,
    current_tick: u8,
    current_speed: u8,

    current_order: usize,
    current_row: usize,

    next_row: usize,
    next_order: usize,

    should_jump: bool,

    channels: Vec<TrackChannel>,

    pub tuning: f64
}

impl TrackPlayer {
    pub fn new(track: Track) -> Self {
        let mut system = crate::system::AudioSystem::new(SAMPLE_RATE,64);
        
        let mut buffers = Vec::with_capacity(track.samples.len());
        for i in 0..track.samples.len() {
            let buffer = system.create_buffer();
            let sample = &track.samples[i];
            system.update_buffer(buffer, &sample.data, sample.format).unwrap();
            buffers.push(buffer);
        }

        let mut channels = Vec::with_capacity(system.num_channels() as usize);
        for i in 0..system.num_channels() {
            let mut properties = ChannelProperties::default();
            properties.interpolation_type = crate::InterpolationType::Linear;

            let pan = track.pans[i as usize];
            properties.panning = pan as f64 / 64.0;
            // A pan value of >= 128 means the channel is disabled and will not be played.
            channels.push(TrackChannel {
                properties,
                enabled: pan < 128,
                current_sample: None,
                note_volume: 0,

                vol_memory: 0,
                pitch_memory: 0,

                offset_memory: 0,
                high_offset: 0
            });
        }

        let half_samples_per_tick = calculate_half_samples_per_tick(track.tempo);
        let speed = track.speed;

        Self { 
            track, 
            system,
            buffers,

            current_half_sample: 0,
            half_samples_per_tick,
            current_tick: 0,
            current_speed: speed,

            current_order: 0,
            current_row: 0,
            
            next_row: 0,
            next_order: 0,

            should_jump: false,

            channels,

            tuning: 1.0
        }
    }

    pub fn advance(&mut self) -> f32 {
        let pattern = &self.track.patterns[self.track.orders[self.current_order] as usize];

        if self.current_half_sample == 0 {
            for c in 0..pattern.channels {
                let mut channel = &mut self.channels[c as usize];

                if !channel.enabled {
                    continue;
                }

                let note = pattern.notes.get(c as usize, self.current_row);
                
                if !note.initialized {
                    continue;
                }

                if self.current_tick == 0 {
                    if note.key == PianoKey::NoteCut || note.key == PianoKey::NoteOff || note.key == PianoKey::NoteFade {
                        channel.current_sample = None;
                        channel.note_volume = 0;
                        self.system.stop(c).unwrap();
                        continue;
                    }

                    let mut sample_id = note.sample;
                    if sample_id.is_none() {
                        sample_id = channel.current_sample;
                    }

                    if let Some(sample_id) = sample_id {
                        if note.key != PianoKey::None && sample_id < self.buffers.len() as u8 {
                            let sample = &self.track.samples[sample_id as usize];
                            let properties = &mut channel.properties;
                            let volume = note.volume.unwrap_or(sample.default_volume);
                            properties.volume = ((volume as u32 * sample.global_volume as u32 * 64 * self.track.global_volume as u32) >> 18) as f64 / 128.0 * (self.track.mix_volume as f64 / u8::MAX as f64);
                            properties.speed = calculate_speed(note.key, note.octave, sample.multiplier) * self.tuning;
                            properties.looping = sample.looping;
                            properties.loop_start = sample.loop_start;
                            properties.loop_end = sample.loop_end;

                            self.system.play_buffer(self.buffers[sample_id as usize], c, channel.properties).unwrap();
                            
                            channel.current_sample = note.sample;
                            channel.note_volume = volume;
                        }
                    }

                    if let (Some(volume), Some(sample)) = (note.volume, channel.current_sample) {
                        let sample = &self.track.samples[sample as usize];
                        channel.properties.volume = ((volume as u32 * sample.global_volume as u32 * 64 * self.track.global_volume as u32) >> 18) as f64 / 128.0 * (self.track.mix_volume as f64 / u8::MAX as f64);
                        self.system.set_channel_properties(c, channel.properties).unwrap();
                        channel.note_volume = volume;
                    }
                }

                match note.effect {
                    Effect::None => {},
                    Effect::SetSpeed => if self.current_tick == 0 { self.current_speed = note.effect_param },
                    Effect::PositionJump => {
                        self.next_row = 0;
                        self.next_order = note.effect_param as usize;
                        self.should_jump = true;
                    },
                    Effect::PatternBreak => {
                        self.next_order = self.current_order + 1;
                        self.next_row = note.effect_param as usize;
                        self.should_jump = true;
                    },
                    Effect::VolumeSlide => {
                        // If the note parameter is 0, we just fetch the last one stored in memory.
                        // If the last parameter is also 0 then nothing happens.
                        let mut vol_param = if note.effect_param == 0 { channel.vol_memory } else { note.effect_param };
                        channel.vol_memory = vol_param;

                        // Handle DFy and DxF, if 'F' is set then the volume slide only occurs on the first tick.
                        // However, if value is D0F, then ignore, as this is not a fine volume slide.
                        // Volume slide occurs on every tick except the first, **unless** it is D0F.
                        if channel.current_sample.is_none() || (self.current_tick == 0 && ((vol_param & 0xF0) != 0xF0 && (vol_param & 0xF) != 0xF)) ||
                            (((vol_param & 0xF0) == 0xF0 || ((vol_param & 0xF) == 0xF && (vol_param & 0xF0) != 0)) && self.current_tick != 0) {
                            continue;
                        }

                        let sample_id = channel.current_sample.unwrap();

                        let mut volume = channel.note_volume as i32;

                        // If the volume parameter is DFx then we need to remove the F so that the volume slide
                        // works as usual, otherwise it would think it's a value of 240 + x
                        if (vol_param & 0xF0) == 0xF0 {
                            vol_param = vol_param & 0x0F
                        }

                        // D0y decreases volume by y units.
                        // Dx0 increases volume by x units.
                        if vol_param < 16 {
                            volume -= vol_param as i32;
                        } else {
                            volume += vol_param as i32 / 16;
                        }

                        // Volume cannot exceed 64.
                        channel.note_volume = volume.clamp(0, 64) as u8;

                        let sample = &self.track.samples[sample_id as usize];
                        channel.properties.volume = ((channel.note_volume as u32 * sample.global_volume as u32 * 64 * self.track.global_volume as u32) >> 18) as f64 / 128.0 * (self.track.mix_volume as f64 / u8::MAX as f64);
                        self.system.set_channel_properties(c, channel.properties).unwrap();
                    },
                    Effect::PortamentoDown => {
                        let mut pitch_param = if note.effect_param == 0 { channel.pitch_memory } else { note.effect_param };
                        channel.pitch_memory = pitch_param;

                        if ((pitch_param & 0xF0) >= 0xE0 && self.current_tick != 0) || self.current_tick == 0 && (pitch_param & 0xF0) < 0xE0 {
                            continue;
                        }

                        let multiplier = if (pitch_param & 0xF0) == 0xE0 { 1.0 / 4.0 } else { 1.0 };

                        if (pitch_param & 0xF0) == 0xF0 {
                            pitch_param &= 0xF;
                        } else if (pitch_param & 0xF0) == 0xE0 {
                            pitch_param &= 0xF;
                        }

                        channel.properties.speed *= f64::powf(2.0, -4.0 * (pitch_param as f64 * multiplier) / 768.0);
                        self.system.set_channel_properties(c, channel.properties).unwrap();
                    },
                    Effect::PortamentoUp => {
                        let mut pitch_param = if note.effect_param == 0 { channel.pitch_memory } else { note.effect_param };
                        channel.pitch_memory = pitch_param;

                        if ((pitch_param & 0xF0) >= 0xE0 && self.current_tick != 0) || self.current_tick == 0 && (pitch_param & 0xF0) < 0xE0 {
                            continue;
                        }

                        let multiplier = if (pitch_param & 0xF0) == 0xE0 { 1.0 / 4.0 } else { 1.0 };

                        if (pitch_param & 0xF0) == 0xF0 {
                            pitch_param &= 0xF;
                        } else if (pitch_param & 0xF0) == 0xE0 {
                            pitch_param &= 0xF;
                        }

                        channel.properties.speed *= f64::powf(2.0, 4.0 * (pitch_param as f64 * multiplier) / 768.0);
                        self.system.set_channel_properties(c, channel.properties).unwrap();
                    },
                    /*Effect::TonePortamento => todo!(),
                    Effect::Vibrato => todo!(),
                    Effect::Tremor => todo!(),
                    Effect::Arpeggio => todo!(),
                    Effect::VolumeSlideVibrato => todo!(),
                    Effect::VolumeSlideTonePortamento => todo!(),
                    Effect::SetChannelVolume => todo!(),
                    Effect::ChannelVolumeSlide => todo!(),*/
                    Effect::SampleOffset => {
                        if self.current_tick == 0 {
                            let offset = if note.effect_param == 0 { channel.offset_memory } else { note.effect_param };
                            channel.offset_memory = offset;

                            if note.key != PianoKey::None {
                                let _ = self.system.seek_to_sample(c, offset as usize * 256 + channel.high_offset);
                            }
                        }
                    },
                    /*Effect::PanningSlide => todo!(),
                    Effect::Retrigger => todo!(),
                    Effect::Tremolo => todo!(),*/
                    Effect::Special => {
                        let param = note.effect_param;

                        if param >= 0x80 && param <= 0x8F {
                            channel.properties.panning = (param & 0xF) as f64 / 15.0;
                            self.system.set_channel_properties(c, channel.properties).unwrap();
                        }

                        if param >= 0xA0 && param <= 0xAF {
                            channel.high_offset = (param & 0xF) as usize * 65536;
                        }
                    },
                    Effect::Tempo => {
                        if note.effect_param > 0x20 && self.current_tick == 0 {
                            self.half_samples_per_tick = calculate_half_samples_per_tick(note.effect_param);
                        }
                    },
                    /*Effect::FineVibrato => todo!(),
                    Effect::SetGlobalVolume => todo!(),
                    Effect::GlobalVolumeSlide => todo!(),*/
                    Effect::SetPanning => {
                        channel.properties.panning = note.effect_param as f64 / 255.0;
                        self.system.set_channel_properties(c, channel.properties).unwrap();
                    },
                    /*Effect::Panbrello => todo!(),
                    Effect::MidiMacro => todo!(),*/
                    _ => {}
                }
            }
        }

        self.current_half_sample += 1;

        if self.current_half_sample >= self.half_samples_per_tick {
            self.current_tick += 1;
            self.current_half_sample = 0;

            if self.current_tick >= self.current_speed {
                self.current_tick = 0;
                self.current_row += 1;     
                
                if self.should_jump {
                    self.should_jump = false;
                    self.current_row = self.next_row;
                    self.current_order = self.next_order;
                }

                if self.current_row >= pattern.rows as usize {
                    self.current_row = 0;
                    self.current_order += 1;

                    if self.current_order >= self.track.orders.len() || self.track.orders[self.current_order] == 255 {
                        self.current_order = 0;
                    }
                }

                println!("Ord {}/{} Row {}/{} Spd {}, HSPT {} (Tmp {}, SR {})", self.current_order + 1, self.track.orders.len(), self.current_row, pattern.rows, self.current_speed, self.half_samples_per_tick, calculate_tempo_from_hspt(self.half_samples_per_tick), SAMPLE_RATE);
            }
        }

        self.system.advance()
    }

    pub fn set_interpolation(&mut self, interp_type: crate::InterpolationType) {
        for channel in self.channels.iter_mut() {
            channel.properties.interpolation_type = interp_type;
        }
    }
}

pub fn calculate_half_samples_per_tick(tempo: u8) -> u32 {
    ((2.5 / tempo as f64) * 2.0 * SAMPLE_RATE as f64) as u32
}

pub fn calculate_tempo_from_hspt(hspt: u32) -> u8 {
    (2.5 / (hspt as f64 / SAMPLE_RATE as f64 / 2.0)) as u8
}

pub fn calculate_speed(key: PianoKey, octave: u8, multiplier: f64) -> f64 {
    if key == PianoKey::NoteCut {
        return 0.0;
    }

    // 40 is middle C. Therefore, to work out which note corresponds to the given piano key + octace, we first
    // convert the key to int, subtract the value of middle C (as it is not 0 in the enum), and then add on our octave,
    // multiplied by 12, as that is how many keys are in one octave. We subtract it by 5 as our "middle c" octave is 5.
    let note = 40 + (key as i32 - PianoKey::C as i32) + ((octave as i32 - 5) * 12);

    let pow_note = f64::powf(2.0, (note as f64 - 49.0) / 12.0);

    pow_note * multiplier
}