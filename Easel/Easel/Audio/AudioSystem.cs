using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.SDL;
using static Easel.Audio.MixrNative;

namespace Easel.Audio;

public unsafe class AudioDevice : IDisposable
{
    private IntPtr _system;
    
    public event OnBufferFinished BufferFinished;
    
    // Use SDL for audio.
    private Sdl _sdl;
    private uint _device;
    private AudioSpec _spec;

    public readonly ushort NumChannels;

    private BufferFinishedCallback _callback;
    
    public AudioDevice(int sampleRate, ushort channels)
    {
        NumChannels = channels;
        
        _system = mxCreateSystem(sampleRate, channels);

        _callback = BufferFinishedCB;
        mxSetBufferFinishedCallback(_system, _callback);
        
        _sdl = Sdl.GetApi();
        if (_sdl.Init(Sdl.InitAudio) != 0)
            throw new Exception("SDL could not initialize: " + Marshal.PtrToStringAnsi((IntPtr) _sdl.GetError()));
            
        _spec.Freq = sampleRate;
        _spec.Format = Sdl.AudioF32;
        _spec.Channels = 2;
        _spec.Samples = 512;

        _spec.Callback = new PfnAudioCallback(AudioCallback);

        fixed (AudioSpec* spec = &_spec)
            _device = _sdl.OpenAudioDevice((byte*) null, 0, spec, null, 0);
        _sdl.PauseAudioDevice(_device, 0);
    }

    public int CreateBuffer() => mxCreateBuffer(_system);

    public void DeleteBuffer(int buffer) => mxDeleteBuffer(_system, buffer);

    public void UpdateBuffer<T>(int buffer, T[] data, AudioFormat format) where T : unmanaged
    {
        fixed (void* buf = data)
            mxUpdateBuffer(_system, buffer, buf, (nuint) (data.Length * sizeof(T)), format);
    }

    public void PlayBuffer(int buffer, ushort channel, ChannelProperties properties) =>
        mxPlayBuffer(_system, buffer, channel, properties);

    public void QueueBuffer(int buffer, ushort channel) => mxQueueBuffer(_system, buffer, channel);

    public void SetChannelProperties(ushort channel, ChannelProperties properties) =>
        mxSetChannelProperties(_system, channel, properties);

    public void Play(ushort channel) => mxPlay(_system, channel);

    public void Pause(ushort channel) => mxPause(_system, channel);

    public void Stop(ushort channel) => mxStop(_system, channel);

    public bool IsPlaying(ushort channel) => mxIsPlaying(_system, channel);

    public ushort GetAvailableChannel()
    {
        for (ushort i = 0; i < NumChannels; i++)
        {
            if (!mxIsPlaying(_system, i))
                return i;
        }

        return 0;
    }

    private void AudioCallback(void* arg0, byte* bData, int len)
    {
        for (int i = 0; i < len; i += 4)
        {
            float advance = mxAdvance(_system);
            int iAdvance = *(int*) &advance;
            bData[i + 0] = (byte) (iAdvance & 0xFF);
            bData[i + 1] = (byte) ((iAdvance >> 8) & 0xFF);
            bData[i + 2] = (byte) ((iAdvance >> 16) & 0xFF);
            bData[i + 3] = (byte) (iAdvance >> 24);
        }
    }

    public void Dispose()
    {
        _sdl.CloseAudioDevice(_device);
        _sdl.Quit();
        mxDeleteSystem(_system);
    }

    private void BufferFinishedCB(ushort channel, int buffer)
    {
        BufferFinished?.Invoke(this, channel, buffer);
    }

    public delegate void OnBufferFinished(AudioDevice system, ushort channel, int buffer);
}