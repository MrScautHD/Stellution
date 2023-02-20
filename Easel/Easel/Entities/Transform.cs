using System;
using System.Numerics;
using Easel.Core;

namespace Easel.Entities;

/// <summary>
/// Describes position, rotation, and scale, and provides some helper functions and properties for determining things such
/// as the forward vector and model matrix. Typically used in entities, however can be used anywhere.
/// </summary>
public sealed class Transform : IEquatable<Transform>, ICloneable
{
    /// <summary>
    /// The position of this transform. (Default: Zero)
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// The rotation of this transform. (Default: Identity)
    /// </summary>
    public Quaternion Rotation;

    /// <summary>
    /// The scale of this transform. (Default: One)
    /// </summary>
    public Vector3 Scale;

    public Vector3 Origin;

    /// <summary>
    /// The Sprite (Z) rotation of this transform. This is helpful when dealing with 2D objects.
    /// </summary>
    public float SpriteRotation
    {
        get => Rotation.ToEulerAngles().Z;
        set => Rotation = Quaternion.CreateFromYawPitchRoll(0, 0, value);
    }

    /// <summary>
    /// Create a new default transform.
    /// </summary>
    public Transform()
    {
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
        Origin = Vector3.Zero;
    }

    /// <summary>
    /// The forward vector of this transform.
    /// </summary>
    public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);

    /// <summary>
    /// The backward vector of this transform.
    /// </summary>
    public Vector3 Backward => Vector3.Transform(Vector3.UnitZ, Rotation);

    /// <summary>
    /// The right vector of this transform.
    /// </summary>
    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);

    /// <summary>
    /// The left vector of this transform.
    /// </summary>
    public Vector3 Left => Vector3.Transform(-Vector3.UnitX, Rotation);

    /// <summary>
    /// The up vector of this transform.
    /// </summary>
    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

    /// <summary>
    /// The down vector of this transform.
    /// </summary>
    public Vector3 Down => Vector3.Transform(-Vector3.UnitY, Rotation);

    /// <summary>
    /// Calculates and returns the matrix for this transform.
    /// </summary>
    public Matrix4x4 TransformMatrix => Matrix4x4.CreateTranslation(-Origin) *
                                        Matrix4x4.CreateScale(Scale) *
                                        Matrix4x4.CreateFromQuaternion(Quaternion.Normalize(Rotation)) *
                                        Matrix4x4.CreateTranslation(Position);

    public void RotateAroundLocalPoint(Vector3 point, Vector3 axis, float angle)
    {
        Quaternion rotation = Quaternion.CreateFromAxisAngle(axis, angle);
        Rotation *= rotation;
        Position = Vector3.Transform(Position, Rotation) + point;
    }

    public bool Equals(Transform other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Position.Equals(other.Position) && Rotation.Equals(other.Rotation) && Scale.Equals(other.Scale);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is Transform other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, Rotation, Scale);
    }

    public object Clone()
    {
        return new Transform()
        {
            Position = Position,
            Rotation = Rotation,
            Scale = Scale
        };
    }

    public static bool operator ==(Transform left, Transform right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Transform left, Transform right)
    {
        return !Equals(left, right);
    }

    public static Transform Lerp(Transform a, Transform b, float amount)
    {
        return new Transform()
        {
            Position = Vector3.Lerp(a.Position, b.Position, amount),
            Rotation = Quaternion.Lerp(a.Rotation, b.Rotation, amount),
            Scale = Vector3.Lerp(a.Scale, b.Scale, amount),
            Origin = Vector3.Lerp(a.Origin, b.Origin, amount)
        };
    }
}