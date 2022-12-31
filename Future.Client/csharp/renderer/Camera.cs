using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.renderer;

public class Camera {
    
    private GraphicsDevice _graphicsDevice;

    public float FieldOfViewDegrees = 80f;
    public float NearClipPlane = 0.05f;
    public float FarClipPlane = 2000f;
    
    public Matrix View;
    public Matrix Projection;
    public Matrix CameraWorld;

    public Camera(GraphicsDevice graphicsDevice) {
        this._graphicsDevice = graphicsDevice;
        this.ReCreateWorldAndView();
        this.ReCreateThePerspectiveProjectionMatrix(FieldOfViewDegrees);
    }
    
    public Matrix World {
        get => this.CameraWorld;
        
        set {
            this.CameraWorld = value;
            this.View = Matrix.CreateLookAt(this.CameraWorld.Translation, this.CameraWorld.Forward + this.CameraWorld.Translation, this.CameraWorld.Up);
        }
    }
    
    public Vector3 Position {
        get => this.World.Translation;

        set {
            this.CameraWorld.Translation = value;
            this.ReCreateWorldAndView();
        }
    }

    public Vector3 Forward {
        get => this.World.Forward;

        set {
            this.World = Matrix.CreateWorld(this.World.Translation, value, Vector3.Up);
            this.ReCreateWorldAndView();
        }
    }

    public Vector3 LookAtDirection {
        get => this.World.Forward;

        set {
            this.World = Matrix.CreateWorld(this.World.Translation, value, Vector3.Up);
            this.ReCreateWorldAndView();
        }
    }

    public void TargetPositionToLookAt(Vector3 targetPosition) {
        this.World = Matrix.CreateWorld(this.World.Translation, Vector3.Normalize(targetPosition - this.World.Translation), Vector3.Up);
        this.ReCreateWorldAndView();
    }

    public void LookAtTheTargetMatrix(Matrix targetMatrix) {
        this.World = Matrix.CreateWorld(this.World.Translation, Vector3.Normalize(targetMatrix.Translation - this.World.Translation), Vector3.Up);
        this.ReCreateWorldAndView();
    }

    private void ReCreateWorldAndView() {
        this.World = Matrix.CreateWorld(World.Translation, World.Forward, Vector3.Up);
        this.View = Matrix.CreateLookAt(World.Translation, World.Forward + World.Translation, World.Up);
    }
    
    public void ReCreateThePerspectiveProjectionMatrix(float fovInDegrees) {
        float aspectRatio = this._graphicsDevice.Viewport.Width / (float) this._graphicsDevice.Viewport.Height;
        this.Projection = Matrix.CreatePerspectiveFieldOfView(fovInDegrees * (3.14159265358f / 180f), aspectRatio, .05f, 1000f);
    }
    
    public void ReCreateThePerspectiveProjectionMatrix(float fieldOfViewInDegrees, float nearPlane, float farPlane) {
        this.FieldOfViewDegrees = MathHelper.ToRadians(fieldOfViewInDegrees);
        this.NearClipPlane = nearPlane;
        this.FarClipPlane = farPlane;
        
        float aspectRatio = this._graphicsDevice.Viewport.Width / (float) this._graphicsDevice.Viewport.Height;
        this.Projection = Matrix.CreatePerspectiveFieldOfView(this.FieldOfViewDegrees, aspectRatio, this.NearClipPlane, this.FarClipPlane);
    }
    
    public float GetGimbleLockDangerValue() {
        var c0 = Vector3.Dot(this.World.Forward, this.World.Up);
        if (c0 < 0f) c0 = -c0;
        return c0;
    }
}