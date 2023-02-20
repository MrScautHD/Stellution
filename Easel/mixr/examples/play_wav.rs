// mixr, piegfx 2023.
// Play a wave file.
// Simply input your path, and the program will run until the sound has finished.

use clap::Parser;
use mixr::{self, ChannelProperties, system::{AudioSystem}};
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

    // Create our audio system. We must give it a sample rate, in this case 48khz.
    // We also only create one channel, as that's all we need to play a single sound.
    // In mixr, a single channel can play both mono and stereo sounds.
    const SAMPLE_RATE: i32 = 48000;
    let mut system = AudioSystem::new(SAMPLE_RATE, 1);

    // Load our wav file from the given path.
    let pcm = mixr::loaders::PCM::load_wav_path(path).expect("A valid path is required. Make sure if it contains spaces, you surround it with quotes.");

    // In mixr, buffer IDs are stored as integers. Creating a buffer always creates a unique ID.
    // Once the buffer has been created, we give it our PCM data and its format.
    let buffer = system.create_buffer();
    system.update_buffer(buffer, &pcm.data, pcm.format).unwrap();

    // Play the buffer!
    // First, we provide it with the buffer itself. Then, we provide the channel.
    // The channel can be any value between 0 and the maximum number of channels, however be wary of playing a sound on
    // an existing channel. Doing so will overwrite the currently playing sound!
    // Then, we give it the channel properties.
    // The channel properties do pretty much what they say on the tin. You tell it what volume, speed, panning, etc, to run at.
    // You can change this later as well, by using set_channel_properties.
    // A few things to note:
    //     - These values are normalized. Therefore, a volume of 1.0 is full volume.
    //       While you can go out of this range, it is not advisable, as mixr, by design, does not have a volume limiter.
    //     - Panning is also normalized. A value of 0.0 = fully left, and 1.0 = fully right.
    //       Therefore, the default panning value is 0.5.
    //     - Setting loop_end to -1 will set the loop point to the end of the sample.
    //     - Currently, even if looping is disabled, the sample will stop at the loop point. This is not final, and may change later.
    system.play_buffer(buffer, 0, ChannelProperties {
        volume,
        speed,
        panning,
        looping,
        interpolation_type: mixr::InterpolationType::Linear,
        loop_start: 0,
        loop_end: -1,
    }).unwrap();

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
            system: &mut system
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
    system: &'a mut AudioSystem
}

impl<'a> AudioCallback for Audio<'a> {
    type Channel = f32;

    fn callback(&mut self, out: &mut [Self::Channel]) {
        /*for x in out.iter_mut() {
            // Advance mixr.
            // Mixr does not return an audio buffer, instead you call advance for each sample you need.
            // Make sure this doesn't get out of sync, otherwise many problems can occur.
            // Advance currently returns a u16, this is subject to change.
            *x = self.system.advance();
        }*/

        self.system.advance_buffer(out);
    }
}