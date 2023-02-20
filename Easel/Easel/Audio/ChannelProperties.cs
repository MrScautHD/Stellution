namespace Easel.Audio;

public struct ChannelProperties
{
    public double Volume;
    public double Speed;
    public double Panning;
    public bool Loop;
    public InterpolationType InterpolationType;
    public int BeginLoopPoint;
    public int EndLoopPoint;

    public ChannelProperties()
    {
        Volume = 1;
        Speed = 1;
        Panning = 0.5;
        Loop = false;
        InterpolationType = InterpolationType.Linear;
        BeginLoopPoint = 0;
        EndLoopPoint = -1;
    }
}

public enum InterpolationType
{
    None,
    Linear
}