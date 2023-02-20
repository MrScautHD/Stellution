use mixr;

#[test]
pub fn load_wave() {
    let mut reader = mixr::binary_reader::BinaryReader::new(&std::fs::read("/home/ollie/Music/Always There.wav").unwrap());
    println!("\"RIFF\": {}", reader.read_string(4));
    println!("File size: {}", reader.read_i32());
    println!("\"WAVE\": {}", reader.read_string(4));
    println!("\"fmt \": {}", reader.read_string(4));
    println!("FMT data length: {}", reader.read_i32());
    println!("Sample type (1 is PCM): {}", reader.read_i16());
    println!("Channels: {}", reader.read_i16());
    println!("Sample rate: {}", reader.read_i32());

    // ignore these, they serve no purpose
    reader.read_i32();
    reader.read_i16();

    println!("Bits per sample: {}", reader.read_i16());

    println!("Data size: {}", reader.read_i32());
}