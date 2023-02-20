using System.Numerics;
using Easel.Entities;

namespace Easel.Animations;

public struct TransformKeyframe : IKeyframe
{
    public Transform Transform;

    public TransformKeyframe(Transform transform)
    {
        Transform = transform;
    }

    public IKeyframeInterpolationResult Lerp(IKeyframe previous, float amount)
    {
        TransformKeyframe tkf = (TransformKeyframe) previous;
        
        return new TransformResult()
        {
            Transform = Transform.Lerp(tkf.Transform, Transform, amount)
        };
    }
}