# Easel Texture Format (.etf)
Each ETF file consists of the following layout:

```rs
struct EtfFile {
    file_id: u32,
    version: u32,
    header:  EtfHeader,
    data:    Vec<MipmappedBitmap>
}
```

### file_id
ETF file identifier. Contains the string 'ETF ' (0x20465445)

### version
The version number of the ETF file.

### header
The EtfHeader itself.

### data
Contains the texture data. The length of this will be equal to the `array_size`.

## EtfHeader

```rs
struct EtfHeader {
    width: u32,
    height: u32,
    flags: u8,
    reserved_len: Option<u32>,
    reserved: Vec<u8>,
    format: PieFormat,
    mip_levels: Option<u8>,
    array_size: Option<u32>,
    data_size: u32
}
```

### width
The width of the texture in pixels.

### height
The height of the texture in pixels.

### flags
Various flags.

| Name     | Value | Description                                 |
|----------|-------|---------------------------------------------|
| Mipmaps  | 0x1   | This texture contains mipmaps.              |
| Array    | 0x2   | This texture is an array texture.           |
| Reserved | 0x4   | This texture contains reserved/custom data. |

### reserved_len
The length, in bytes, of the reserved data. **Only read this value if the reserved flag is set.**

### reserved
The reserved/custom data itself. Custom data may be contained here, but can be ignored by most implementations. **Only read this value if the reserved flag is set.**

### format
The pie format.

### mip_levels
The number of mipmaps in this texture. **Only read this value if the mipmaps flag is set.**

### array_size
The size of the array. **Only read this value if the array flag is set.**

### data_size
The size, in bytes, of the most detailed image.

## MipmappedBitmap
```rs
struct MipmappedBitmap {
    data: Vec<Vec<u8>>
}
```

### data
The mipmapped data itself. The length of this Vec is equal to `mip_levels`.