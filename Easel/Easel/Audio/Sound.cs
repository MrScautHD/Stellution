using System;
using System.IO;

namespace Easel.Audio;

public class Sound : IDisposable
{
    public IAudioPlayer AudioPlayer;
    public SoundType SoundType;
    
    public Sound(string path)
    {
        using Stream stream = File.OpenRead(path);
        using BinaryReader reader = new BinaryReader(stream);

        AudioDevice device = EaselGame.Instance.AudioInternal;

        SoundType = GetSoundType(reader);
        switch (SoundType)
        {
            case SoundType.Wav:
                AudioPlayer = new WavPlayer(device, reader.ReadBytes((int) reader.BaseStream.Length));
                break;
            case SoundType.Vorbis:
                AudioPlayer = new VorbisPlayer(device, reader.ReadBytes((int) reader.BaseStream.Length));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public ISoundInstance Play(double volume = 1, double speed = 1, double panning = 0.5f, bool loop = false)
    {
        ChannelProperties properties = new ChannelProperties()
        {
            Volume = volume,
            Speed = speed,
            Panning = panning,
            Loop = SoundType != SoundType.Vorbis && loop,
            InterpolationType = InterpolationType.Linear,
            BeginLoopPoint = 0,
            EndLoopPoint = -1
        };

        AudioDevice device = EaselGame.Instance.AudioInternal;
        ushort channel = device.GetAvailableChannel();
        return AudioPlayer.Play(channel, properties);
    }

    public void Dispose()
    {
        AudioPlayer.Dispose();
    }

    private bool CheckWav(BinaryReader reader)
    {
        reader.BaseStream.Position = 0;
        bool value = new string(reader.ReadChars(4)) == "RIFF";
        reader.BaseStream.Position = 0;
        return value;
    }

    private bool CheckOggVorbis(BinaryReader reader)
    {
        reader.BaseStream.Position = 0;
        if (new string(reader.ReadChars(4)) != "OggS")
        {
            reader.BaseStream.Position = 0;
            return false;
        }

        reader.ReadBytes(25);
        if (new string(reader.ReadChars(6)) != "vorbis")
        {
            reader.BaseStream.Position = 0;
            return false;
        }

        reader.BaseStream.Position = 0;
        return true;
    }

    private SoundType GetSoundType(BinaryReader reader)
    {
        if (CheckWav(reader))
            return SoundType.Wav;
        if (CheckOggVorbis(reader))
            return SoundType.Vorbis;
        return SoundType.Unknown;
    }
}