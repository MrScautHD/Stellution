pub struct BinaryReader {
    pub data: Vec<u8>,
    pub position: usize
}

impl BinaryReader {
    pub fn new(data: &[u8]) -> BinaryReader {
        BinaryReader {
            data: data.to_vec(),
            position: 0
        }
    }

    pub fn read_i8(&mut self) -> i8 {
        let data = self.data[self.position] as i8;
        self.position += 1;
        data
    }

    pub fn read_u8(&mut self) -> u8 {
        let data = self.data[self.position];
        self.position += 1;
        data
    }

    pub fn read_i16(&mut self) -> i16 {
        let b1 = self.read_u8() as i16;
        let b2 = self.read_u8() as i16;

        b1 | (b2 << 8)
    }

    pub fn read_u16(&mut self) -> u16 {
        let b1 = self.read_u8() as u16;
        let b2 = self.read_u8() as u16;

        b1 | (b2 << 8)
    }

    pub fn read_i32(&mut self) -> i32 {
        let b1 = self.read_u8() as i32;
        let b2 = self.read_u8() as i32;
        let b3 = self.read_u8() as i32;
        let b4 = self.read_u8() as i32;

        b1 | (b2 << 8) | (b3 << 16) | (b4 << 24)
    }

    pub fn read_u32(&mut self) -> u32 {
        let b1 = self.read_u8() as u32;
        let b2 = self.read_u8() as u32;
        let b3 = self.read_u8() as u32;
        let b4 = self.read_u8() as u32;

        b1 | (b2 << 8) | (b3 << 16) | (b4 << 24)
    }

    pub fn read_i64(&mut self) -> i64 {
        let b1 = self.read_u8() as i64;
        let b2 = self.read_u8() as i64;
        let b3 = self.read_u8() as i64;
        let b4 = self.read_u8() as i64;
        let b5 = self.read_u8() as i64;
        let b6 = self.read_u8() as i64;
        let b7 = self.read_u8() as i64;
        let b8 = self.read_u8() as i64;

        b1 | (b2 << 8) | (b3 << 16) | (b4 << 24) | (b5 << 32) | (b6 << 40) | (b7 << 48) | (b8 << 56)
    }

    pub fn read_u64(&mut self) -> u64 {
        let b1 = self.read_u8() as u64;
        let b2 = self.read_u8() as u64;
        let b3 = self.read_u8() as u64;
        let b4 = self.read_u8() as u64;
        let b5 = self.read_u8() as u64;
        let b6 = self.read_u8() as u64;
        let b7 = self.read_u8() as u64;
        let b8 = self.read_u8() as u64;

        b1 | (b2 << 8) | (b3 << 16) | (b4 << 24) | (b5 << 32) | (b6 << 40) | (b7 << 48) | (b8 << 56)
    }

    pub fn read_string(&mut self, num_chars: i32) -> String {
        let mut text = String::new();

        for _ in 0..num_chars {
            text.push(self.data[self.position] as char);
            self.position += 1;
        }

        text
    }

    pub fn read_bytes(&mut self, num_bytes: usize) -> &[u8] {
        let data = &self.data[self.position..(self.position + num_bytes)];
        self.position += num_bytes;
        data
    }
}