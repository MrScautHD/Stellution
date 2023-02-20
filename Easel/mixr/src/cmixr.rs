use std::ffi::CStr;

use crate::{system::{AudioSystem, AudioErrorType, AudioError}, AudioFormat, ChannelProperties, AudioResult, loaders::PCM};

#[repr(C)]
pub struct CPCM {
    pub data: *mut u8,
    pub data_length: usize,
    pub format: AudioFormat
}

#[no_mangle]
pub extern "C" fn mxCreateSystem(sample_rate: i32, channels: u16) -> *mut AudioSystem {
    Box::into_raw(Box::new(AudioSystem::new(sample_rate, channels)))
}

#[no_mangle]
pub unsafe extern "C" fn mxDeleteSystem(system: &mut AudioSystem) {
    std::mem::drop(system);
}

#[no_mangle]
pub extern "C" fn mxSetBufferFinishedCallback(system: &mut AudioSystem, callback: extern "C" fn(u16, i32)) {
    system.set_buffer_finished_callback(unsafe { std::mem::transmute(callback) });
}

#[no_mangle]
pub extern "C" fn mxCreateBuffer(system: &mut AudioSystem) -> i32 {
    system.create_buffer()
}

#[no_mangle]
pub extern "C" fn mxDeleteBuffer(system: &mut AudioSystem, buffer: i32) -> AudioResult {
    result_to_result(system.delete_buffer(buffer))
}
    
#[no_mangle]
pub extern "C" fn mxUpdateBuffer(system: &mut AudioSystem, buffer: i32, data: *const std::ffi::c_void, data_length: usize, format: AudioFormat) -> AudioResult {
    let data = unsafe { std::slice::from_raw_parts(data, data_length) };

    assert_eq!(data_length, data.len(), "data_length, {}, does not equal the converted data length, {}", data_length, data.len());

    result_to_result(system.update_buffer(buffer, &data, format))
}

#[no_mangle]
pub extern "C" fn mxPlayBuffer(system: &mut AudioSystem, buffer: i32, channel: u16, properties: ChannelProperties) -> AudioResult {
    result_to_result(system.play_buffer(buffer, channel, properties))
}

#[no_mangle]
pub extern "C" fn mxQueueBuffer(system: &mut AudioSystem, buffer: i32, channel: u16) -> AudioResult {
    result_to_result(system.queue_buffer(buffer, channel))
}

#[no_mangle]
pub extern "C" fn mxSetChannelProperties(system: &mut AudioSystem, channel: u16, properties: ChannelProperties) -> AudioResult {
    result_to_result(system.set_channel_properties(channel, properties))
}

#[no_mangle]
pub extern "C" fn mxPlay(system: &mut AudioSystem, channel: u16) -> AudioResult {
    result_to_result(system.play(channel))
}

#[no_mangle]
pub extern "C" fn mxPause(system: &mut AudioSystem, channel: u16) -> AudioResult {
    result_to_result(system.pause(channel))
}

#[no_mangle]
pub extern "C" fn mxStop(system: &mut AudioSystem, channel: u16) -> AudioResult {
    result_to_result(system.stop(channel))
}

#[no_mangle]
pub extern "C" fn mxAdvance(system: &mut AudioSystem) -> f32 {
    system.advance()
}

#[no_mangle]
pub extern "C" fn mxAdvanceBuffer(system: &mut AudioSystem, buffer: *mut f32, buffer_len: usize) {
    unsafe { system.advance_buffer(std::slice::from_raw_parts_mut(buffer, buffer_len)); }
}

#[no_mangle]
pub extern "C" fn mxGetNumChannels(system: &mut AudioSystem) -> u16 {
    system.num_channels()
}

#[no_mangle]
pub extern "C" fn mxIsPlaying(system: &mut AudioSystem, channel: u16) -> bool {
    system.is_playing(channel)
}

fn result_to_result(result: Result<(), AudioError>) -> AudioResult {
    match result {
        Ok(_) => AudioResult::Ok,
        Err(err) => match err.error_type {
            AudioErrorType::InvalidBuffer => AudioResult::InvalidBuffer,
            AudioErrorType::InvalidChannel => AudioResult::InvalidChannel,
            AudioErrorType::OutOfRange => AudioResult::OutOfRange
        }
    }
}

#[no_mangle]
pub unsafe extern "C" fn mxPCMLoadWav(data: *const u8, data_length: usize) -> *mut CPCM {
    let mut pcm = PCM::load_wav(std::slice::from_raw_parts(data, data_length)).unwrap();
    let data = pcm.data.as_mut_ptr();
    let len = pcm.data.len();
    std::mem::forget(pcm.data);

    Box::into_raw(Box::new(CPCM {
        data,
        data_length: len,
        format: pcm.format
    }))
}

#[no_mangle]
pub unsafe extern "C" fn mxPCMFree(pcm: &mut CPCM) {
    std::mem::drop(Vec::from_raw_parts(pcm.data, pcm.data_length, pcm.data_length));
    std::mem::drop(Box::from_raw(pcm));
}