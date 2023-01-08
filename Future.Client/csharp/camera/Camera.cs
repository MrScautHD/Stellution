using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.camera;

public class Camera {
    
    private GraphicsDevice _graphicsDevice;

    public Matrix View { get; private set; }
    public Matrix Projection { get; private set; }
    private Matrix _world;

    private readonly float _nearPlaneDistance = 0.05f;
    private readonly float _farPlaneDistance = 1000f;
    
    private float _fov;

    public float Yaw { get; private set; }
    public float Pitch { get; private set; }
    public float Roll { get; private set; }

    public Camera(GraphicsDevice graphicsDevice, float fov, Vector3 pos, Vector3 lookAt) {
        this._graphicsDevice = graphicsDevice;
        this.FieldOfViewDegrees = fov;
        this.Position = pos;
        this.Forward = lookAt;
    }
    
    /**
     * Get / Set FOV
     */
    public float FieldOfViewDegrees {
        get => this._fov;

        set {
            this._fov = value;
            this.UpdatePerspective(this.FieldOfViewDegrees);
            this.SetWorldAndView(this.Forward);
        }
    }

    /**
     * Get / Set Position
     */
    public Vector3 Position {
        get => this._world.Translation;

        set {
            this._world.Translation = value;
            this.SetWorldAndView(this.Forward);
        }
    }

    /**
     * Get / Set Forward Vector
     */
    public Vector3 Forward {
        get => this._world.Forward;
        set => this.SetWorldAndView(value);
    }

    /**
     * Set World and View Matrix
     */
    private void SetWorldAndView(Vector3 forward) {
        this._world = Matrix.CreateWorld(this._world.Translation, forward, Vector3.Up);
        this.View = Matrix.CreateLookAt(this._world.Translation, this._world.Forward + this._world.Translation, this._world.Up);
    }
    
    /**
     * Update FOV
     */
    private void UpdatePerspective(float fovInDegrees) {
        float aspectRatio = this._graphicsDevice.Viewport.Width / (float) this._graphicsDevice.Viewport.Height;
        this.Projection = Matrix.CreatePerspectiveFieldOfView(fovInDegrees * (3.14159265358F / 180F), aspectRatio, this._nearPlaneDistance, this._farPlaneDistance);
    }

    /**
     * Tag a Pos on that the Camera should look
     */
    public void TargetPositionToLookAt(Vector3 targetPosition) {
        this.SetWorldAndView(Vector3.Normalize(targetPosition - this._world.Translation));
    }
    
    /**
     * To stop Rendering things that not watchable
     */
    public BoundingFrustum GetBoundingFrustum() {
        return new BoundingFrustum(this.View * this.Projection);
    }
    
    /**
     * Move Camera (Normally Tagged to the Player with Position)
     */
    public void Move(GameTime time, int speed = 30) {
        this.Position += (this._world.Forward * speed) * (float) time.ElapsedGameTime.TotalSeconds;
    }

    /**
     * Rotate Camera
     */
    public void Rotate(float yaw, float pitch, float roll) {
        this.Yaw = yaw % 360;
        this.Pitch = Math.Clamp(pitch, -90, 90);
        this.Roll = roll % 360;

        Matrix rotation = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(this.Yaw), MathHelper.ToRadians(this.Pitch), 0);
        this.SetWorldAndView(rotation.Forward);
        
        this.View *= Matrix.CreateRotationZ(MathHelper.ToRadians(this.Roll));
    }
}