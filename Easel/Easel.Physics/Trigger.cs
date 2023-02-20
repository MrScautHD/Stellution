using System.Numerics;
using BulletSharp;
using Easel.Entities.Components;

namespace Easel.Physics;

public class Trigger : Component
{
    public event OnCollision Collision;
    
    private CollisionShape _iShape;
    
    private GhostObject _ghost;

    public bool IsCollision;
    
    public Trigger(CollisionShape shape)
    {
        _iShape = shape;
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        Matrix4x4 transform = Matrix4x4.CreateFromQuaternion(Transform.Rotation) *
                              Matrix4x4.CreateTranslation(Transform.Position);

        _iShape.LocalScaling = Transform.Scale;
        _ghost = Physics.AddTrigger(_iShape, transform);
    }

    protected override void Update()
    {
        base.Update();

        _ghost.WorldTransform = Transform.TransformMatrix;

        IsCollision = _ghost.NumOverlappingObjects > 0;

        for (int i = 0; i < _ghost.NumOverlappingObjects; i++)
        {
            Collision?.Invoke(_ghost.OverlappingPairs[i]);
        }
    }

    public delegate void OnCollision(CollisionObject obj);
}