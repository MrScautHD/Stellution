use std::{time::Duration, cell::RefCell};

use mixr::{self, system::AudioSystem, AudioFormat, ChannelProperties, loaders::{Stream, PcmStream, StreamManager}};
use sdl2::audio::{AudioSpecDesired, AudioCallback};

struct Audio<'a> {
    system: &'a mut AudioSystem,
    stream: &'a mut Box<dyn Stream + Send>,
    format: AudioFormat
}

impl<'a> AudioCallback for Audio<'a> {
    type Channel = f32;

    fn callback(&mut self, out: &mut [Self::Channel]) {
        for x in out.iter_mut() {
            *x = self.system.advance();

            while let Some((channel, buffer)) = self.system.pop_finished_buffer() {
                self.system.update_buffer(buffer, self.stream.buffer(), self.format).unwrap();
                self.system.queue_buffer(buffer, channel).unwrap();
            }
        }
    }
}

/*#[test]
fn test_wav() {
    const SAMPLE_RATE: i32 = 48000;

    let mut system = mixr::system::AudioSystem::new(SAMPLE_RATE, 2);
    system.master_volume = 1.0;

    let pcm1 = mixr::loaders::PCM::load_wav_path("/home/ollie/Music/r-59.wav").unwrap();
    let buffer = system.create_buffer();
    system.update_buffer(buffer, &pcm1.data, pcm1.format).unwrap();
    system.play_buffer(buffer, 0, ChannelProperties::default()).unwrap();
    
    const NUM_BUFFERS: i32 = 30;
    const SIZE: usize = 96000;

    /*let mut buffers = Vec::with_capacity(NUM_BUFFERS as usize);

    
    for i in 0..NUM_BUFFERS {
        let i = i as usize;

        let buffer = system.create_buffer();
        system.update_buffer(buffer, &pcm1.data[(i * SIZE)..((i + 1) * SIZE)], pcm1.format).unwrap();
        buffers.push(buffer);
    }

    let mut props = ChannelProperties::default();
    props.speed = 1.0;
    system.play_buffer(buffers[0], 0, props).unwrap();

    for i in 1..NUM_BUFFERS {
        system.queue_buffer(buffers[i as usize], 0).unwrap();
    }*/

    let sdl = sdl2::init().unwrap();
    let audio = sdl.audio().unwrap();

    let desired_spec = AudioSpecDesired {
        freq: Some(SAMPLE_RATE),
        channels: Some(2),
        samples: Some(8192)
    };

    let device = audio.open_playback(None, &desired_spec, |_| {
        Audio {
            system: &mut system,
        }
    }).unwrap();

    device.resume();

    //std::thread::sleep(Duration::from_secs((((length as i32) / 4 / rate) - 1) as u64));
    loop {
        std::thread::sleep(Duration::from_secs(5));
    }
}*/

#[test]
fn test_stream() {
    const SAMPLE_RATE: i32 = 48000;

    let mut system = mixr::system::AudioSystem::new(SAMPLE_RATE, 2);
    system.master_volume = 1.0;

    let mut manager = StreamManager::new();
    let mut stream = manager.load_stream_path("/home/ollie/Music/Always There.wav").unwrap();
    let format = stream.format();
    
    const NUM_BUFFERS: usize = 2;

    let mut buffers = Vec::with_capacity(NUM_BUFFERS);
    for _ in 0..NUM_BUFFERS {
        let buffer = system.create_buffer();
        system.update_buffer(buffer, stream.buffer(), format).unwrap();
        buffers.push(buffer);
    }

    system.play_buffer(buffers[0], 0, ChannelProperties::default()).unwrap();
    for i in 1..buffers.len() {
        system.queue_buffer(buffers[i], 0).unwrap();
    }

    let sdl = sdl2::init().unwrap();
    let audio = sdl.audio().unwrap();

    let desired_spec = AudioSpecDesired {
        freq: Some(SAMPLE_RATE),
        channels: Some(2),
        samples: Some(8192)
    };

    let device = audio.open_playback(None, &desired_spec, |_| {
        Audio {
            system: &mut system,
            stream: &mut stream,
            format: format
        }
    }).unwrap();

    device.resume();

    //std::thread::sleep(Duration::from_secs((((length as i32) / 4 / rate) - 1) as u64));
    loop {
        std::thread::sleep(Duration::from_secs(5));
    }
}