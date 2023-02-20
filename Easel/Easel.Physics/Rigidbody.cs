using System.Numerics;
using BulletSharp;
using Easel.Entities;
using Easel.Entities.Components;

namespace Easel.Physics;

public class Rigidbody : Component
{
    private float _iMass;
    private CollisionShape _iShape;
    
    private RigidBody _rb;

    private Transform _lastTransform;

    public bool LockX, LockY, LockZ;

    public Vector3 Velocity
    {
        get => _rb.LinearVelocity;
        set => _rb.LinearVelocity = value;
    }

    public float Friction
    {
        get => _rb.Friction;
        set => _rb.Friction = value;
    }

    public Rigidbody(float mass, CollisionShape shape)
    {
        _iMass = mass;
        _iShape = shape;
    }

    protected override void Initialize()
    {
        base.Initialize();

        Matrix4x4 transform = Matrix4x4.CreateFromQuaternion(Transform.Rotation) *
                              Matrix4x4.CreateTranslation(Transform.Position);
        
        _iShape.LocalScaling = Transform.Scale;

        if (_iMass == 0)
            _rb = Physics.AddStaticBody(_iShape, transform);
        else
            _rb = Physics.AddRigidBody(_iMass, _iShape, transform);

        //_rb.Restitution = 0.9f;
        //_rb.CcdMotionThreshold = 0.00005f;
        //_rb.CcdSweptSphereRadius = 0.5f;
    }

    protected override void Update()
    {
        base.Update();
        
        _rb.WorldTransform = Matrix4x4.CreateFromQuaternion(Transform.Rotation) *
                             Matrix4x4.CreateTranslation(Transform.Position);
        //_rb.AngularFactor = new Vector3(LockX ? 0 : 1, LockY ? 0 : 1, LockZ ? 0 : 1);

        //Transform.Position = _rb.WorldTransform.Translation;
        //Transform.Rotation = _rb.Orientation;
    }

    public override void Dispose()
    {
        base.Dispose();
        
        Physics.World.RemoveRigidBody(_rb);
        _rb.Dispose();
    }
}