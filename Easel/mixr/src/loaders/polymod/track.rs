use crate::AudioFormat;

use super::{PianoKey, ModuleType};

use super::{Arr2D, Note, sample::Sample};
use std::io;

pub struct Pattern {
    pub notes: Arr2D<Note>,
    pub channels: u16,
    pub rows: u16
}

impl Pattern {
    pub fn new(channels: u16, rows: u16) -> Self {
        Self { notes: Arr2D::new(channels as usize, rows as usize), channels, rows }
    }

    pub fn set_note(&mut self, channel: u16, row: u16, note: Note) {
        self.notes.set(channel as usize, row as usize, note);
    }
}

pub struct Track {
    pub mod_type: ModuleType,

    pub patterns: Vec<Pattern>,
    pub orders: Vec<u8>,
    pub samples: Vec<Sample>,

    pub tempo: u8,
    pub speed: u8,

    pub global_volume: u8,
    pub pans: Vec<u8>,
    pub mix_volume: u8
}

impl Track {
    /// Load the given Impulse Tracker file (.IT)
    pub fn from_it(data: &[u8]) -> Result<Track, io::Error> {
        let mut reader = crate::binary_reader::BinaryReader::new(data);
        if reader.read_string(4) != String::from("IMPM") {
            return Err(io::Error::new(io::ErrorKind::InvalidData, "Expected \"IMPM\", not found."));
        }

        let title = reader.read_string(26);
        super::log(format!("Loading \"{}\"...", title));

        reader.read_bytes(2); // pattern highlight
        
        let num_orders = reader.read_u16();
        let num_instruments = reader.read_u16();
        let num_samples = reader.read_u16();
        let num_patterns = reader.read_u16();

        reader.read_bytes(4); // created with tracker, not needed here.

        let flags = reader.read_u16();
        if (flags & 4) == 4 {
            return Err(io::Error::new(io::ErrorKind::Unsupported, "Instruments are not currently supported."));
        }

        reader.read_bytes(2); // special, not needed.

        let global_volume = reader.read_u8();
        let mix_volume = reader.read_u8();
        let initial_speed = reader.read_u8();
        let initial_tempo = reader.read_u8();

        super::log(format!("gv: {global_volume}, mv: {mix_volume}, spd: {initial_speed}, tmp: {initial_tempo}"));

        reader.read_bytes(12); // stuff we don't need.

        let pans = reader.read_bytes(64).to_vec();
        let vols = reader.read_bytes(64).to_vec();

        assert_eq!(reader.position, 0xC0);

        let orders = reader.read_bytes(num_orders as usize).to_vec();

        reader.position = (0xC0 + num_orders + num_instruments * 4) as usize;
        
        let mut samples = Vec::with_capacity(num_samples as usize);

        for _ in 0..num_samples {
            let offset = reader.read_u32();
            let curr_pos = reader.position;

            reader.position = offset as usize;

            if reader.read_string(4) != String::from("IMPS") {
                return Err(io::Error::new(io::ErrorKind::InvalidData, "Expected \"IMPS\", not found."));
            }

            let dos_name = reader.read_string(12);
            reader.read_u8(); // seemingly unused byte.

            let s_global = reader.read_u8();
            let s_flags = reader.read_u8();

            let mut format = AudioFormat::default();
            format.bits_per_sample = if (s_flags & 2) == 2 { 16 } else { 8 };
            format.channels = if (s_flags & 4) == 4 { 2 } else { 1 };
            // todo, loops and stuff

            let s_def_vol = reader.read_u8(); // default volume

            let s_name = reader.read_string(26);
            super::log(format!("Loading {s_name} ({dos_name})..."));

            let s_cvt = reader.read_u8(); // convert, unused *yet* but will be later.
            reader.read_u8(); // default pan, don't think it needs to be used.

            let s_length = reader.read_u32() * format.channels as u32 * (format.bits_per_sample / 8) as u32;
            let s_loop_start = reader.read_u32();
            let s_loop_end = reader.read_u32();
            format.sample_rate = reader.read_i32();

            reader.read_bytes(8); // ignoring sustain stuff for now

            let pointer = reader.read_u32();

            reader.position = pointer as usize;
            let s_data = reader.read_bytes(s_length as usize);

            let s_loop = (s_flags & 16) == 16;
            samples.push(Sample::new(s_data, format, s_loop, s_loop_start as i32, if !s_loop { -1 } else { s_loop_end as i32 }, s_global, s_def_vol));

            reader.position = curr_pos;
        }

        reader.position = (0xC0 + num_orders + num_instruments * 4 + num_samples * 4) as usize;

        let mut p_cache = Vec::with_capacity(64);
        for _ in 0..64 {
            p_cache.push(PatternCache { mask: 0, note: 0, instrument: 0, volume: 0, effect: 0, eff_param: 0  });
        }

        let mut patterns = Vec::with_capacity(num_patterns as usize);

        for i in 0..num_patterns {
            let offset = reader.read_u32();
            if offset == 0 {
                patterns.push(Pattern::new(64, 64));
                continue;
            }
            let curr_pos = reader.position;

            reader.position = offset as usize;

            reader.read_bytes(2); // length
            let rows = reader.read_u16();

            reader.read_bytes(4); // empty data

            let mut pattern = Pattern::new(64, rows);

            for r in 0..rows {
                let mut c_var = reader.read_u8();

                while c_var != 0 {
                    let channel = (c_var - 1) & 63;
                    let mut prev_var = &mut p_cache[channel as usize];

                    let mask_variable = if (c_var & 128) == 128 { reader.read_u8() } else { prev_var.mask };
                    prev_var.mask = mask_variable;

                    let mut note: u8 = 253;
                    let mut instrument: Option<u8> = None;
                    let mut volume: Option<u8> = None;
                    let mut effect: u8 = 0;
                    let mut effect_param: u8 = 0;

                    if (mask_variable & 1) == 1 {
                        note = reader.read_u8();
                        prev_var.note = note;
                    }

                    if (mask_variable & 2) == 2 {
                        let inst = reader.read_u8() - 1;
                        instrument = Some(inst);
                        prev_var.instrument = inst;
                    }

                    if (mask_variable & 4) == 4 {
                        let vol = reader.read_u8();
                        volume = Some(vol);
                        prev_var.volume = vol;
                    }

                    if (mask_variable & 8) == 8 {
                        effect = reader.read_u8();
                        effect_param = reader.read_u8();

                        prev_var.effect = effect;
                        prev_var.eff_param = effect_param;
                    }

                    if (mask_variable & 16) == 16 {
                        note = prev_var.note;
                    }

                    if (mask_variable & 32) == 32 {
                        instrument = Some(prev_var.instrument);
                    }

                    if (mask_variable & 64) == 64 {
                        volume = Some(prev_var.volume);
                    }

                    if (mask_variable & 128) == 128 {
                        effect = prev_var.effect;
                        effect_param = prev_var.eff_param;
                    }

                    let mut key = PianoKey::None;
                    let mut octave = 0;
                    let effect = unsafe { std::mem::transmute(effect) };

                    match note {
                        255 => key = PianoKey::NoteOff,
                        254 => key = PianoKey::NoteCut,
                        253 => {},
                        _ => {
                            key = unsafe { std::mem::transmute(note % 12 + PianoKey::C as u8) };
                            octave = note / 12;
                        }
                    }

                    let note = Note::new(key, octave, instrument, volume, effect, effect_param);
                    super::log(format!("Row: {r}, Channel: {channel}, Pattern: {i}, Note: {:?}", note));
                    pattern.set_note(channel as u16, r, note);

                    c_var = reader.read_u8();
                }
            }

            patterns.push(pattern);
            reader.position = curr_pos;
        }

        Ok(Track { 
            mod_type: ModuleType::IT,
            
            patterns,
            orders,
            samples,

            tempo: initial_tempo,
            speed: initial_speed,

            global_volume,
            pans,
            mix_volume
        })
    }
}

struct PatternCache {
    pub mask: u8,
    pub note: u8,
    pub instrument: u8,
    pub volume: u8,
    pub effect: u8,
    pub eff_param: u8
}