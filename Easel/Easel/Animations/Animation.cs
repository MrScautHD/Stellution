using System.Linq;

namespace Easel.Animations;

public struct Animation
{
    public readonly (IKeyframe keyframe, float time)[] Keyframes;

    private float _startTime;
    private bool _loop;

    public Animation(params (IKeyframe keyframe, float time)[] keyframes)
    {
        Keyframes = keyframes;
        _startTime = 0;
        _loop = false;
    }

    public void Play(bool loop = false)
    {
        _startTime = Time.TotalSeconds;
        _loop = loop;
    }

    public IKeyframeInterpolationResult Result
    {
        get
        {
            float currentTime = Time.TotalSeconds - _startTime;

            if (currentTime >= Keyframes[^1].time)
            {
                if (_loop)
                {
                    _startTime = Time.TotalSeconds;
                    currentTime = float.Epsilon;
                }
                else
                    return Keyframes[^1].keyframe.Lerp(Keyframes[^1].keyframe, 0);
            }

            (IKeyframe frame, float time) prevFrame = Keyframes.Last((keyframe) => keyframe.time < currentTime);
            (IKeyframe frame, float time) currFrame = Keyframes.First((keyframe) => keyframe.time >= currentTime);

            return currFrame.frame.Lerp(prevFrame.frame,
                (currentTime - prevFrame.time) / (currFrame.time - prevFrame.time));
        }
    }
}