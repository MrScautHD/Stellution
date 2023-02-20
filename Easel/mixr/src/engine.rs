use std::io;

use crate::system::AudioSystem;

pub struct AudioEngine {
    system: AudioSystem
}

impl AudioEngine {
    pub fn system(&self) -> &AudioSystem {
        &self.system
    }
}

pub struct Sound<'a> {
    engine: &'a AudioEngine
}

impl<'a> Sound<'a> {
    //pub fn from_file(engine: &AudioEngine) -> Result<Self, io::Error> {
    //    Ok(()))
    //}


}

pub trait AudioStreamer {
    
}