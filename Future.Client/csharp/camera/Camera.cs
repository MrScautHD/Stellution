using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.camera;

public class Camera {
    
    private GraphicsDevice _graphicsDevice;

    private float _fov;

    public Matrix View { get; private set; }
    public Matrix Projection { get; private set; }
    private Matrix _world;

    public Camera(GraphicsDevice graphicsDevice, float fov) {
        this._graphicsDevice = graphicsDevice;
        this.FieldOfViewDegrees = fov;
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

        set {
            this.SetWorldAndView(value);
        }
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
        this.Projection = Matrix.CreatePerspectiveFieldOfView(fovInDegrees * (3.14159265358f / 180f), aspectRatio, .05f, 1000f);
    }

    /**
     * Tag a Pos on that the Camera should look
     */
    public void TargetPositionToLookAt(Vector3 targetPosition) {
        this.SetWorldAndView(Vector3.Normalize(targetPosition - this._world.Translation));
    }
    
    /**
     * Move Camera (Normally Tagged to the Player)
     */
    public void Move(GameTime time, int speed = 30) {
        this.Position += (this._world.Forward * speed) * (float) time.ElapsedGameTime.TotalSeconds;
    }

    /**
     * Rotate Camera (Normally Tagged to the Player)
     */
    public void Rotate(float yaw, float pitch) {
        Matrix matrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
        this.Forward = Vector3.TransformNormal(this.Forward, matrix);
        this.SetWorldAndView(this.Forward);
    }
}