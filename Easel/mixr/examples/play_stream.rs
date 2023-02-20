// mixr, piegfx 2023.
// Play a wave file.
// Simply input your path, and the program will run until the sound has finished.

use clap::Parser;
use mixr::{self, ChannelProperties, system::{AudioSystem}, loaders::{Stream, StreamManager}, AudioFormat};
use sdl2::audio::{AudioSpecDesired, AudioCallback};

#[derive(Parser)]
struct CommandArgs {
    path: String,

    #[arg(short, long, default_value_t = 1.0)]
    volume: f64,

    #[arg(short, long, default_value_t = 1.0)]
    speed: f64,

    #[arg(short, long, default_value_t = 0.5)]
    panning: f64,

    #[arg(long, default_value_t = false)]
    looping: bool
}

fn main() {
    // Parse the command line arguments.
    let args = CommandArgs::parse();
    let path = args.path.as_str();
    let volume = args.volume;
    let speed = args.speed;
    let panning = args.panning;
    let looping = args.looping;

    const SAMPLE_RATE: i32 = 48000;
    let mut system = AudioSystem::new(SAMPLE_RATE, 1);

    let mut manager = StreamManager::new();
    let mut stream = manager.load_stream_path(path).unwrap();
    let format = stream.format();

    const NUM_BUFFERS: usize = 2;

    let mut buffers = Vec::with_capacity(NUM_BUFFERS);
    for _ in 0..NUM_BUFFERS {
        let buffer = system.create_buffer();
        system.update_buffer(buffer, stream.buffer().unwrap(), format).unwrap();
        buffers.push(buffer);
    }

    system.play_buffer(buffers[0], 0, ChannelProperties {
        volume,
        speed,
        panning,
        looping,
        interpolation_type: mixr::InterpolationType::Linear,
        loop_start: 0,
        loop_end: -1,
    }).unwrap();

    for i in 1..buffers.len() {
        system.queue_buffer(buffers[i], 0).unwrap();
    }

    // Initialize the SDL audio subsystem.
    let sdl = sdl2::init().unwrap();
    let audio = sdl.audio().unwrap();

    // The SDL audio should match the format of mixr.
    // Mixr's audio format has been designed to be compatible with SDL's audio spec.
    let desired_spec = AudioSpecDesired {
        freq: Some(SAMPLE_RATE),
        channels: Some(2),
        samples: Some(8192)
    };

    // Create our audio device, with our audio struct (defined later) as our callback.
    let device = audio.open_playback(None, &desired_spec, |_| {
        Audio {
            system: &mut system,
            stream: &mut stream,
            format
        }
    }).unwrap();

    // Start playback of the device (this is important.)
    device.resume();

    // This just handles the ctrl+c keypress, since rust does not seem to handle it itself.
    ctrlc::set_handler(move || std::process::exit(0)).unwrap();

    // Sleep for 1 second, then check if the channel is playing.
    // If it's not, stop the program.
    loop {
        std::thread::sleep(std::time::Duration::from_secs(1));
        if !system.is_playing(0) {
            std::process::exit(0);
        }
    }
}

// This struct holds the audio callback for SDK.
// We need to keep a reference to our audio system in the audio struct.
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
                if let Some(buf) = self.stream.buffer() {
                    self.system.update_buffer(buffer, buf, self.format).unwrap();
                    self.system.queue_buffer(buffer, channel).unwrap();
                } else {
                    std::process::exit(0);
                }
            }
        }
    }
}