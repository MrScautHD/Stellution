use std::collections::HashMap;
use std::collections::VecDeque;
use crate::{AudioFormat, ChannelProperties, ByteConvert};

// Chunk size denotes how many samples a chunk is. In this case, 48000 samples.
const CHUNK_SIZE: f64 = 48000.0;

#[derive(Debug)]
pub enum AudioErrorType {
    InvalidBuffer,
    InvalidChannel,
    OutOfRange
}

#[derive(Debug)]
pub struct AudioError<'a> {
    pub error_type: AudioErrorType,
    pub description: &'a str
}

impl<'a> AudioError<'a> {
    pub fn new(error_type: AudioErrorType) -> Self {
        let description = match error_type {
            AudioErrorType::InvalidBuffer => "An invalid buffer was provided.",
            AudioErrorType::InvalidChannel => "An invalid channel was provided.",
            AudioErrorType::OutOfRange => "The given sample was out of range."
        };

        Self {
            error_type,
            description
        }
    }

    pub fn with_description(error_type: AudioErrorType, description: &'a str) -> Self {
        Self { error_type, description }
    }
}

struct Buffer {
    data: Vec<u8>,
    format: AudioFormat,
    bytes_per_sample: u8,
    length_in_samples: usize
}

struct Channel {
    sample_rate: i32,
    speed: f64,

    buffer: i32,
    prev_buffer: i32,

    chunk: u64,
    position: f64,

    prev_curr_sample: usize,
    prev_sample: usize,

    playing: bool,
    properties: ChannelProperties,
    queued: VecDeque<i32>,

    in_use: bool
}

pub struct AudioSystem {
    //pub format: AudioFormat,
    pub master_volume: f64,

    sample_rate: i32,

    buffers: HashMap<i32, Buffer>,
    channels: Vec<Channel>,
    current_handle: i32,
    current_sample: u8,

    buffers_finished: VecDeque<(u16, i32)>,

    callback: Option<fn(u16, i32)>
}

impl AudioSystem {
    pub fn new(sample_rate: i32, channels: u16) -> AudioSystem {

        let mut v_channels = Vec::with_capacity(channels as usize);
        for _ in 0..channels {
            v_channels.push(Channel {
                sample_rate: 0,
                speed: 0.0,

                buffer: -1,
                prev_buffer: -1,

                chunk: 0,
                position: 0.0,

                prev_curr_sample: 0,
                prev_sample: 0,

                playing: false,
                properties: ChannelProperties::default(),
                queued: VecDeque::new(),

                in_use: false
            });
        }

        AudioSystem { 
            sample_rate: sample_rate,

            buffers: HashMap::new(), 
            channels: v_channels, 
            current_handle: 0, 
            current_sample: 0,
            buffers_finished: VecDeque::new(),

            master_volume: 1.0,

            callback: None
        }
    }

    pub fn create_buffer(&mut self) -> i32 {
        let buffer = Buffer { 
            data: Vec::new(), 
            format: AudioFormat { channels: 0, sample_rate: 0, bits_per_sample: 0, floating_point: false }, 
            length_in_samples: 0,
            bytes_per_sample: 0 
        };

        self.buffers.insert(self.current_handle, buffer);
        
        let p_buffer = self.current_handle;
        self.current_handle += 1;

        p_buffer
    }

    pub fn delete_buffer(&mut self, buffer: i32) -> Result<(), AudioError> {
        for channel in self.channels.iter_mut() {
            if channel.buffer == buffer {
                channel.playing = false;
            }
        }

        self.buffers.remove(&buffer).ok_or(AudioError::new(AudioErrorType::InvalidBuffer))?;

        Ok(())
    }

    pub fn update_buffer<T: Sized>(&mut self, buffer: i32, data: &[T], format: AudioFormat) -> Result<(), AudioError> {
        let mut i_buffer = self.buffers.get_mut(&buffer).ok_or(AudioError::new(AudioErrorType::InvalidBuffer))?;

        i_buffer.data = unsafe { std::slice::from_raw_parts(data.as_ptr() as *const u8, data.len() * std::mem::size_of::<T>()).to_vec() };
        
        // As the data is stored as bytes, we need to convert this to a byte value.
        // A bits per sample of 8 = 1 byte. 16 bits = 2 bytes, etc.
        let bytes_per_sample = format.bits_per_sample / 8;

        i_buffer.length_in_samples = data.len() / bytes_per_sample as usize / format.channels as usize;
        i_buffer.bytes_per_sample = bytes_per_sample;

        i_buffer.format = format;

        Ok(())
    }

    pub fn play_buffer(&mut self, buffer: i32, channel: u16, properties: ChannelProperties) -> Result<(), AudioError> {
        let i_buffer = self.buffers.get(&buffer).ok_or(AudioError::new(AudioErrorType::InvalidBuffer))?;
        let i_channel = self.channels.get_mut(channel as usize).ok_or(AudioError::new(AudioErrorType::InvalidChannel))?;

        i_channel.queued.clear();

        i_channel.chunk = 0;
        i_channel.position = 0.0;
        i_channel.prev_sample = 0;
        i_channel.prev_curr_sample = 0;

        i_channel.properties = properties;
        if i_channel.properties.loop_end == -1 {
            i_channel.properties.loop_end = self.buffers[&buffer].length_in_samples as i32;
        }

        i_channel.sample_rate = i_buffer.format.sample_rate;
        i_channel.speed = i_buffer.format.sample_rate as f64 / self.sample_rate as f64;
        i_channel.speed *= i_channel.properties.speed;
        i_channel.buffer = buffer;
        i_channel.prev_buffer = buffer;
        i_channel.playing = true;
        i_channel.in_use = true;

        Ok(())
    }

    pub fn set_channel_properties(&mut self, channel: u16, properties: ChannelProperties) -> Result<(), AudioError> {
        let mut channel = self.channels.get_mut(channel as usize).ok_or(AudioError::new(AudioErrorType::InvalidChannel))?;
        if !channel.in_use {
            return Ok(());
        }

        let buffer = self.buffers.get(&channel.buffer).ok_or(AudioError::new(AudioErrorType::InvalidBuffer))?;
        
        channel.properties = properties;
        channel.speed = buffer.format.sample_rate as f64 / self.sample_rate as f64;
        channel.speed *= channel.properties.speed;

        if channel.properties.loop_end == -1 {
            channel.properties.loop_end = buffer.length_in_samples as i32;
        }

        Ok(())
    }

    pub fn play(&mut self, channel: u16) -> Result<(), AudioError> {
        let mut channel = self.channels.get_mut(channel as usize).ok_or(AudioError::new(AudioErrorType::InvalidChannel))?;
        if !channel.in_use {
            return Ok(());
        }

        channel.playing = true;

        Ok(())
    }

    pub fn pause(&mut self, channel: u16) -> Result<(), AudioError> {
        let mut channel = self.channels.get_mut(channel as usize).ok_or(AudioError::new(AudioErrorType::InvalidChannel))?;
        if !channel.in_use {
            return Ok(());
        }

        channel.playing = false;

        Ok(())
    }

    pub fn stop(&mut self, channel: u16) -> Result<(), AudioError> {
        let mut channel = self.channels.get_mut(channel as usize).ok_or(AudioError::new(AudioErrorType::InvalidChannel))?;
        if !channel.in_use {
            return Ok(());
        }

        channel.playing = false;
        channel.position = 0.0;
        channel.chunk = 0;
        channel.queued.clear();

        Ok(())
    }

    pub fn queue_buffer(&mut self, buffer: i32, channel: u16) -> Result<(), AudioError> {
        // todo: Check if channel is in use, if it isn't, return NotInUse error.
        let channel = self.channels.get_mut(channel as usize).ok_or(AudioError::new(AudioErrorType::InvalidChannel))?;
        if !self.buffers.contains_key(&buffer) { 
            return Err(AudioError::new(AudioErrorType::InvalidBuffer));
        }
        channel.queued.push_back(buffer);

        Ok(())
    }

    pub fn pop_finished_buffer(&mut self) -> Option<(u16, i32)> {
        self.buffers_finished.pop_back()
    }

    pub fn set_buffer_finished_callback(&mut self, callback: fn(u16, i32)) {
        self.callback = Some(callback);
    }

    pub fn advance(&mut self) -> f32 {
        let mut result: f64 = 0.0;

        let mut current_channel = 0;
        for channel in self.channels.iter_mut() {
            // Do not attempt to mix channels that are not playing.
            if !channel.playing {
                continue;
            }

            let mut curr_buffer = &self.buffers[&channel.buffer];
            let prev_buffer = &self.buffers[&channel.prev_buffer];

            let properties = &channel.properties;
            let curr_format = &curr_buffer.format;
            let prev_format = &prev_buffer.format;

            // Calculate the current and previous sample.
            // Sample != position in array
            let mut curr_sample_f64 = channel.chunk as f64 * CHUNK_SIZE + channel.position;
            let mut curr_sample = curr_sample_f64 as usize;

            if curr_sample >= properties.loop_end as usize {
                if properties.looping {
                    // Looping just returns the channel back to the loop point..
                    let loop_pos = (properties.loop_start + (curr_sample as i32 - properties.loop_end)) as f64;
                    channel.chunk = (loop_pos / CHUNK_SIZE) as u64;
                    channel.position = if channel.chunk == 0 { loop_pos } else { loop_pos / channel.chunk as f64 - CHUNK_SIZE };
                    curr_sample_f64 = channel.chunk as f64 * CHUNK_SIZE + channel.position;
                    curr_sample = curr_sample_f64 as usize;

                } else if channel.queued.len() > 0 {
                    channel.chunk = 0;
                    channel.position = if CHUNK_SIZE > channel.position { 0.0 } else { channel.position - CHUNK_SIZE };
                    curr_sample_f64 = channel.chunk as f64 * CHUNK_SIZE + channel.position;
                    curr_sample = curr_sample_f64 as usize;

                    channel.buffer = channel.queued.pop_front().unwrap();
                    curr_buffer = &self.buffers[&channel.buffer];

                } else {
                    self.buffers_finished.push_back((current_channel, channel.buffer));
                    if let Some(cb) = self.callback {
                        cb(current_channel, channel.buffer);
                    }

                    channel.playing = false;
                    channel.in_use = false;
                    continue;
                }
            }

            if channel.prev_curr_sample != curr_sample {
                channel.prev_sample = channel.prev_curr_sample;
                channel.prev_curr_sample = curr_sample;
            }

            let prev_sample = channel.prev_sample;

            let curr_data = &curr_buffer.data;
            let prev_data = &prev_buffer.data;
            
            let curr_bps = curr_buffer.bytes_per_sample as usize;
            let prev_bps = prev_buffer.bytes_per_sample as usize;

            let mut curr_pos = curr_sample * curr_bps * curr_format.channels as usize;
            curr_pos += self.current_sample as usize * (curr_format.channels - 1) as usize * curr_bps;
            // curr_pos % curr_bps
            curr_pos -= (curr_pos & (curr_bps - 1)) * curr_format.channels as usize;

            let mut curr_value = unsafe { Self::get_sample(curr_data, curr_pos, curr_format.bits_per_sample, curr_format.floating_point) };

            match properties.interpolation_type {
                crate::InterpolationType::None => {},
                crate::InterpolationType::Linear => {
                    let mut prev_pos = prev_sample * prev_bps * prev_format.channels as usize;
                    prev_pos += self.current_sample as usize * (prev_format.channels - 1) as usize * prev_bps;
                    // prev_pos % prev_bps
                    prev_pos -= (prev_pos & (prev_bps - 1)) * prev_format.channels as usize;

                    let prev_value = unsafe { Self::get_sample(prev_data, prev_pos, prev_format.bits_per_sample, prev_format.floating_point) };

                    curr_value = Self::lerp(prev_value, curr_value, curr_sample_f64 - curr_sample as f64);
                }
            }

            let pan = f64::clamp(if self.current_sample == 0 { (1.0 - properties.panning) * 2.0 } else { 1.0 - ((0.5 - properties.panning)) * 2.0 }, 0.0, 1.0);

            result += curr_value * pan * properties.volume;

            // Advance by the channel's speed, but only when both stereo channels have been mixed.
            if self.current_sample == 0 {
                if channel.prev_buffer != channel.buffer {
                    self.buffers_finished.push_back((current_channel, channel.prev_buffer));
                    if let Some(cb) = self.callback {
                        cb(current_channel, channel.buffer);
                    }
                }
                channel.prev_buffer = channel.buffer;

                channel.position += channel.speed;
                if channel.position >= CHUNK_SIZE {
                    channel.chunk += 1;
                    channel.position -= CHUNK_SIZE;
                }
            }

            current_channel += 1;
        }

        self.current_sample += 1;
        // self.current_sample % 2
        self.current_sample = self.current_sample & 1;

        let result = f64::clamp(result * self.master_volume, -1.0, 1.0) as f32;

        result
    }

    pub fn advance_buffer(&mut self, buffer: &mut [f32]) {
        for element in buffer.iter_mut() {
            *element = self.advance();
        }
    }

    pub fn num_channels(&self) -> u16 {
        self.channels.len() as u16
    }

    pub fn is_playing(&self, channel: u16) -> bool {
        if let Some(channel) = self.channels.get(channel as usize) {
            channel.playing
        } else {
            false
        }
    }

    pub fn seek_to_sample(&mut self, channel: u16, sample: usize) -> Result<(), AudioError> {
        let mut channel = self.channels.get_mut(channel as usize).ok_or(AudioError::new(AudioErrorType::InvalidChannel))?;
        
        let sample = sample as f64;

        let buffer = self.buffers.get(&channel.buffer).ok_or(AudioError::new(AudioErrorType::InvalidBuffer))?;
        if sample < 0.0 || sample >= (buffer.data.len() / buffer.bytes_per_sample as usize) as f64 {
            return Err(AudioError::new(AudioErrorType::OutOfRange));
        }

        channel.chunk = (sample / CHUNK_SIZE) as u64;
        channel.position = if channel.chunk == 0 { sample } else { sample / channel.chunk as f64 - CHUNK_SIZE };

        Ok(())
    }

    pub fn seek_seconds(&mut self, channel: u16, seconds: f64) -> Result<(), AudioError> {
        let mut channel = self.channels.get_mut(channel as usize).ok_or(AudioError::new(AudioErrorType::InvalidChannel))?;
        // I have no idea why I stored the sample rate in the channel but it's coming in useful now I guess..
        let sample = channel.sample_rate as f64 * seconds;

        channel.chunk = (sample / CHUNK_SIZE) as u64;
        channel.position = if channel.chunk == 0 { sample } else { sample / channel.chunk as f64 - CHUNK_SIZE };

        Ok(())
    }

    //#[inline(always)]
    unsafe fn get_sample(data: &[u8], pos: usize, fmt_bps: u8, floating_point: bool) -> f64 {
        match fmt_bps {
            32 => {
                if floating_point {
                    f32::from_bytes_le(&data[pos..pos + 4]) as f64
                } else {
                    i32::from_bytes_le(&data[pos..pos + 4]) as f64 / i32::MAX as f64
                }
            },
            /*24 => {
                let mut value = ((data[pos] as u32)) | ((data[pos + 1] as u32) << 8) | ((data[pos + 2] as u32) << 16);
                //let value = value as i32;

                let value = ((value << 8) & 0xFFFFFF00);

                value as f64 / 0x7FFFFFFF as f64
            },*/
            16 => (data[pos] as i16 | ((data[pos + 1] as i16) << 8) as i16) as f64 / i16::MAX as f64,
            8 => ((((data[pos] as i32) << 8) as i32) - i16::MAX as i32) as f64 / i16::MAX as f64,
            _ => panic!("Invalid bits per sample.")
        }
    }

    #[inline(always)]
    fn lerp(value: f64, next: f64, amount: f64) -> f64 {
        amount * (next - value) + value
    }
}