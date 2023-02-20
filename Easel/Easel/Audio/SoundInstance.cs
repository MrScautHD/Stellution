using System;

namespace Easel.Audio;

public interface ISoundInstance
{
    public double Volume { get; set; }
    
    public double Speed { get; set; }
    
    public double Panning { get; set; }
    
    public bool Loop { get; set; }

    public void Stop();

    public void Pause();

    public void Resume();

    public void Restart();
}