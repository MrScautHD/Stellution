using System;
using System.IO;
using System.Text;
using Easel.Graphics;
using Easel.Math;
using Pie;

namespace Easel.Formats;

public class ETF
{
    public readonly int MipLevels;

    public readonly Size<int> Size;

    public readonly Bitmap[][] Bitmaps;

    public ETF(byte[] data)
    {
        using MemoryStream stream = new MemoryStream(data);
        using BinaryReader reader = new BinaryReader(stream);

        if (reader.ReadUInt32() != 0x20465445)
            throw new InvalidDataException("Given file is not an ETF file (ETF identifier missing).");

        reader.ReadUInt32(); // Version

        #region Header

        uint width = reader.ReadUInt32();

        uint height = reader.ReadUInt32();

        byte flags = reader.ReadByte();

        bool mipMaps = (flags & 0x1) == 0x1;
        bool arrayTexture = (flags & 0x2) == 0x2;

        if ((flags & 0x4) == 0x4)
        {
            uint increaseBy = reader.ReadUInt32();
            reader.BaseStream.Position += increaseBy;
        }

        Format format = (Format) reader.ReadByte();

        int mipLevels = 1;
        int arraySize = 1;

        if (mipMaps)
            mipLevels = reader.ReadByte();
        if (arrayTexture)
            arraySize = (int) reader.ReadUInt32();

        uint dataSize = reader.ReadUInt32();

        #endregion

        MipLevels = mipLevels;
        Size = new Size<int>((int) width, (int) height);
        Bitmaps = new Bitmap[arraySize][];

        for (int i = 0; i < arraySize; i++)
        {
            Bitmaps[i] = new Bitmap[mipLevels];

            int w = (int) width;
            int h = (int) height;
            int dS = (int) dataSize;
            
            for (int m = 0; m < mipLevels; m++)
            {
                Bitmaps[i][m] = new Bitmap(w, h, format, reader.ReadBytes(dS));

                w /= 2;
                h /= 2;
                dS /= 4;
            }
        }
    }
    
    public static byte[] CreateEtf(Bitmap bitmap, Format? desiredFormat = null, string customData = null)
    {
        int mipLevels = 1;
        int arraySize = 1;

        bool containsCustomData = customData != null;

        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(0x20465445);
        
        writer.Write(1);

        #region EtfHeader

        writer.Write(bitmap.Size.Width);
        
        writer.Write(bitmap.Size.Height);

        byte flags = 0;
        if (mipLevels > 1)
            flags |= 0x1;
        if (arraySize > 1)
            flags |= 0x2;
        if (containsCustomData)
            flags |= 0x4;
        
        writer.Write(flags);

        if (containsCustomData)
        {
            byte[] reservedBytes = Encoding.UTF8.GetBytes(customData);
            writer.Write(reservedBytes.Length);
            writer.Write(reservedBytes);
        }

        Format format = desiredFormat ?? bitmap.Format;
        writer.Write((byte) format);
        
        if (mipLevels > 1)
            writer.Write((byte) mipLevels);
        if (arraySize > 1)
            writer.Write(arraySize);
        
        writer.Write(bitmap.Size.Width * bitmap.Size.Height * PieUtils.CalculateBitsPerPixel(format));

        #endregion

        
        for (int m = 0; m < mipLevels; m++)
        {
            writer.Write(bitmap.Data);
        }

        return stream.ToArray();
    }
}