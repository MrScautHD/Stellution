using System;
using System.Numerics;
using Easel.Math;
using Pie.Windowing;

namespace Easel.Entities.Components;

/// <summary>
/// Provides a basic built-in no-clip FPS camera.
/// </summary>
public class NoClipCamera : Component
{
    /// <summary>
    /// If enabled, the camera will behave exactly like an FPS camera.
    /// Y-rotation (pitch) is limited to -90 to +90 degrees, and Z-rotation (roll) is not allowed.
    /// If disabled, the camera can be rotated in every direction, and does not behave like an FPS camera.
    /// </summary>
    public bool FpsConstraints;

    public Key Forward;
    public Key Backward;
    public Key StrafeRight;
    public Key StrafeLeft;
    public Key Up;
    public Key Down;
    public Key Run;

    public float MouseSensitivity;
    public float MoveSpeed;
    public float RunSpeed;

    private Vector2<float> _rotation;

    public NoClipCamera()
    {
        FpsConstraints = true;

        Forward = Key.W;
        Backward = Key.S;
        StrafeLeft = Key.A;
        StrafeRight = Key.D;
        Up = Key.Space;
        Down = Key.LeftControl;
        Run = Key.LeftShift;

        MouseSensitivity = 0.01f;
        MoveSpeed = 50;
        RunSpeed = 100;
        
        _rotation = Vector2<float>.Zero;
    }

    protected internal override void Update()
    {
        base.Update();
        
        // TODO: Input contexts & configurations, no hardcoded keys

        float speed = MoveSpeed;
        if (Input.KeyDown(Run))
            speed = RunSpeed;
        speed *= Time.DeltaTime;

        if (Input.KeyDown(Forward))
            Transform.Position += Transform.Forward * speed;
        if (Input.KeyDown(Backward))
            Transform.Position += Transform.Backward * speed;
        if (Input.KeyDown(StrafeLeft))
            Transform.Position += Transform.Left * speed;
        if (Input.KeyDown(StrafeRight))
            Transform.Position += Transform.Right * speed;
        if (Input.KeyDown(Up))
            Transform.Position += Transform.Up * speed;
        if (Input.KeyDown(Down))
            Transform.Position += Transform.Down * speed;

        Vector2<float> mouseDelta = Input.DeltaMousePosition * MouseSensitivity;
        if (FpsConstraints)
        {
            _rotation.X -= mouseDelta.X;
            _rotation.Y -= mouseDelta.Y;
            _rotation.Y = EaselMath.Clamp(_rotation.Y, -MathF.PI / 2, MathF.PI / 2);
            Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, 0);
        }
        else
        {
            Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, -mouseDelta.X);
            Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, -mouseDelta.Y);
        }
    }
}