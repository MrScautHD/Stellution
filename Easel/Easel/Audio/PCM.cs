using System;
using System.IO;
using static Easel.Audio.MixrNative;

namespace Easel.Audio;

public class PCM
{
    public byte[] Data;
    public AudioFormat Format;

    public PCM(byte[] data, AudioFormat format)
    {
        Data = data;
        Format = format;
    }

    public static PCM LoadWav(string path)
    {
        return LoadWav(File.ReadAllBytes(path));
    }

    public static unsafe PCM LoadWav(byte[] data)
    {
        MixrNative.PCM* pcm;
        fixed (byte* ptr = data)
            pcm = mxPCMLoadWav(ptr, (nuint) data.Length);
        ReadOnlySpan<byte> pcmData = new ReadOnlySpan<byte>(pcm->Data, (int) pcm->DataLength);
        byte[] dataArr = pcmData.ToArray();
        AudioFormat format = pcm->Format;
        mxPCMFree(pcm);
        return new PCM(dataArr, format);
    }
}