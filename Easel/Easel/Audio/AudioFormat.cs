using System;
using System.Runtime.InteropServices;

namespace Easel.Audio;

[StructLayout(LayoutKind.Sequential)]
public struct AudioFormat
{
    public byte Channels;
    public int SampleRate;
    public byte BitsPerSample;

    public AudioFormat(byte channels, int sampleRate, byte bitsPerSample)
    {
        Channels = channels;
        SampleRate = sampleRate;
        BitsPerSample = bitsPerSample;
    }

    public static readonly AudioFormat Stereo48khz = new AudioFormat(2, 48000, 16);
}