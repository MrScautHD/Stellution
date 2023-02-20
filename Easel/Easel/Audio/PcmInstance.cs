using System;

namespace Easel.Audio;

public struct PcmInstance : ISoundInstance
{
    private ushort _channel;
    private AudioDevice _device;

    private ChannelProperties _properties;

    public PcmInstance(AudioDevice device, ushort channel, in ChannelProperties properties)
    {
        _device = device;
        _channel = channel;

        _properties = properties;
    }

    public double Volume
    {
        get => _properties.Volume;
        set
        {
            _properties.Volume = value;
            _device.SetChannelProperties(_channel, _properties);
        }
    }
    
    public double Speed
    {
        get => _properties.Speed;
        set
        {
            _properties.Speed = value;
            _device.SetChannelProperties(_channel, _properties);
        }
    }

    public double Panning
    {
        get => _properties.Panning;
        set
        {
            _properties.Panning = value;
            _device.SetChannelProperties(_channel, _properties);
        }
    }

    public bool Loop {
        get => _properties.Loop;
        set
        {
            _properties.Loop = value;
            _device.SetChannelProperties(_channel, _properties);
        }
    }

    public void Stop()
    {
        _device.Stop(_channel);
        _channel = ushort.MaxValue;
    }

    public void Pause()
    {
        _device.Pause(_channel);
    }

    public void Resume()
    {
        _device.Play(_channel);
    }

    public void Restart()
    {
        throw new System.NotImplementedException();
    }
}