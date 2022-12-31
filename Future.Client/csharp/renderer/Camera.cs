using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.renderer;

public class Camera {
    
    private GraphicsDevice _graphicsDevice;

    public float FieldOfViewDegrees = 80f;

    public Matrix View { get; private set; }
    public Matrix Projection { get; private set; }
    public Matrix World;

    public Camera(GraphicsDevice graphicsDevice) {
        this._graphicsDevice = graphicsDevice;
        this.ReCreateWorldAndView();
        this.ReCreateThePerspectiveProjectionMatrix(this.FieldOfViewDegrees);
    }

    public Vector3 Position {
        get => this.World.Translation;

        set {
            this.World.Translation = value;
            this.ReCreateWorldAndView();
        }
    }

    public Vector3 Forward {
        get => this.World.Forward;

        set {
            this.SetWorldAndView(Matrix.CreateWorld(this.World.Translation, value, Vector3.Up));
            this.ReCreateWorldAndView();
        }
    }
    
    public void SetWorldAndView(Matrix world) {
        this.World = world;
        this.View = Matrix.CreateLookAt(this.World.Translation, this.World.Forward + this.World.Translation, this.World.Up);
    }
    
    public void TargetPositionToLookAt(Vector3 targetPosition) {
        this.SetWorldAndView(Matrix.CreateWorld(this.World.Translation, Vector3.Normalize(targetPosition - this.World.Translation), Vector3.Up));
        this.ReCreateWorldAndView();
    }

    public void LookAtTheTargetMatrix(Matrix targetMatrix) {
        this.SetWorldAndView(Matrix.CreateWorld(this.World.Translation, Vector3.Normalize(targetMatrix.Translation - this.World.Translation), Vector3.Up));
        this.ReCreateWorldAndView();
    }

    private void ReCreateWorldAndView() {
        this.SetWorldAndView(Matrix.CreateWorld(World.Translation, World.Forward, Vector3.Up));
        this.View = Matrix.CreateLookAt(World.Translation, World.Forward + World.Translation, World.Up);
    }
    
    public void ReCreateThePerspectiveProjectionMatrix(float fovInDegrees) {
        float aspectRatio = this._graphicsDevice.Viewport.Width / (float) this._graphicsDevice.Viewport.Height;
        this.Projection = Matrix.CreatePerspectiveFieldOfView(fovInDegrees * (3.14159265358f / 180f), aspectRatio, .05f, 1000f);
    }

    public float GetGimbleLockDangerValue() {
        var c0 = Vector3.Dot(this.World.Forward, this.World.Up);
        if (c0 < 0f) c0 = -c0;
        return c0;
    }
}