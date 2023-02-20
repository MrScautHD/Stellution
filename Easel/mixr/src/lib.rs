pub mod system;
pub mod binary_reader;

//#[cfg(feature = "cmixr")]
mod cmixr;

//#[cfg(feature = "loaders")]
pub mod loaders;

//#[cfg(feature = "engine")]
//pub mod engine;

#[derive(Clone, Debug, Copy)]
#[repr(C)]
pub struct AudioFormat {
    pub channels: u8,
    pub sample_rate: i32,
    pub bits_per_sample: u8,
    pub floating_point: bool
}

impl Default for AudioFormat {
    fn default() -> Self {
        Self {
            channels: 2,
            sample_rate: 48000,
            bits_per_sample: 16,
            floating_point: false
        }
    }
}

#[derive(Clone, Debug, Copy)]
#[repr(C)]
pub enum InterpolationType {
    None,
    Linear
}

#[derive(Clone, Debug, Copy)]
#[repr(C)]
pub struct ChannelProperties {
    pub volume: f64,
    pub speed: f64,
    pub panning: f64,
    pub looping: bool,
    pub interpolation_type: InterpolationType,

    pub loop_start: i32,
    pub loop_end: i32,
}

impl Default for ChannelProperties {
    fn default() -> Self {
        Self { volume: 1.0, speed: 1.0, panning: 0.5, looping: false, interpolation_type: InterpolationType::Linear, loop_start: 0, loop_end: -1 }
    }
}

#[repr(C)]
pub enum ChannelState {
    Playing,
    Stopped
}

#[repr(C)]
pub enum AudioResult {
    Ok,

    InvalidBuffer,
    InvalidChannel,
    OutOfRange
}

pub trait ByteConvert<T> {
    fn from_bytes_le(bytes: &[u8]) -> T;
}

impl ByteConvert<f32> for f32 {
    #[inline(always)]
    fn from_bytes_le(bytes: &[u8]) -> f32 {
        unsafe { std::mem::transmute::<i32, f32>(i32::from_bytes_le(bytes)) }
    }
}

impl ByteConvert<i32> for i32 {
    #[inline(always)]
    fn from_bytes_le(bytes: &[u8]) -> i32 {
        bytes[0] as i32 | ((bytes[1] as i32) << 8) | ((bytes[2] as i32) << 16)| ((bytes[3] as i32) << 24)
    }
}