namespace Easel.Animations;

public interface IKeyframe
{
    public IKeyframeInterpolationResult Lerp(IKeyframe previous, float amount);
}