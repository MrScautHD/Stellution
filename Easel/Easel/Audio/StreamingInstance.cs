namespace Easel.Audio;

public class StreamingInstance : ISoundInstance
{
    public double Volume { get; set; }
    public double Speed { get; set; }
    public double Panning { get; set; }
    public bool Loop { get; set; }
    public void Stop()
    {
        throw new System.NotImplementedException();
    }

    public void Pause()
    {
        throw new System.NotImplementedException();
    }

    public void Resume()
    {
        throw new System.NotImplementedException();
    }

    public void Restart()
    {
        throw new System.NotImplementedException();
    }
}