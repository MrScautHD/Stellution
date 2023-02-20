
using System;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Easel.Scenes;

namespace Easel.Entities;

/// <summary>
/// A perspective camera used for 3D scenes.
/// </summary>
public class Camera : Entity
{
    public Vector4 Viewport;
    
    /// <summary>
    /// The projection matrix of this camera.
    /// </summary>
    public Matrix4x4 ProjectionMatrix { get; private set; }

    /// <summary>
    /// Calculates and returns the view matrix of this camera.
    /// </summary>
    public Matrix4x4 ViewMatrix
    {
        get
        {
            Matrix4x4 parent = Matrix4x4.Identity;
            if (Parent != null)
                Matrix4x4.Invert(Parent.Transform.TransformMatrix, out parent);
            return parent * Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);
        }
    }

    public Color ClearColor;

    public Skybox Skybox;

    public CameraInfo CameraInfo => new CameraInfo()
    {
        Projection = ProjectionMatrix,
        View = ViewMatrix,
        Position = Transform.Position,
        ClearColor = ClearColor,
        Skybox = Skybox
    };

    private float _fov;
    private float _aspectRatio;
    private float _near;
    private float _far;
    private ProjectionType _projectionType;

    /// <summary>
    /// The type of camera this is, a projection (typically for 3D), or an orthographic (typically for 2D) camera.
    /// </summary>
    /// <remarks>Certain camera properties will have no effect depending on which mode is selected.</remarks>
    public ProjectionType ProjectionType
    {
        get => _projectionType;
        set
        {
            _projectionType = value;
            GenerateProjectionMatrix();
        }
    }

    public CameraType CameraType;
    
    #region Perspective

    /// <summary>
    /// Get or set the field of view (FOV), in radians, of this camera.
    /// </summary>
    public float FieldOfView
    {
        get => _fov;
        set
        {
            _fov = value;
            GenerateProjectionMatrix();
        }
    }

    /// <summary>
    /// Get or set the aspect ratio (typically width / height) of this camera. You won't normally need to set this value.
    /// </summary>
    public float AspectRatio
    {
        get => _aspectRatio;
        set
        {
            _aspectRatio = value;
            GenerateProjectionMatrix();
        }
    }

    /// <summary>
    /// The near plane distance of this camera.
    /// </summary>
    public float NearPlane
    {
        get => _near;
        set
        {
            _near = value;
            GenerateProjectionMatrix();
        }
    }

    /// <summary>
    /// The far plane distance of this camera.
    /// </summary>
    public float FarPlane
    {
        get => _far;
        set
        {
            _far = value;
            GenerateProjectionMatrix();
        }
    }
    
    #endregion

    #region Orthographic

    private Vector2<float> _orthoSize;
    
    /// <summary>
    /// The size of the orthographic matrix, in normalized 0-1 coordinates. (1, 1) will be the size of the current
    /// viewport. (0.5, 0.5) will be half the size of the current viewport, and all objects will be twice the size.
    /// Use this for camera zoom functionality.
    /// </summary>
    /// <returns></returns>
    public Vector2<float> OrthoSize
    {
        get => _orthoSize;
        set
        {
            _orthoSize = value;
            GenerateProjectionMatrix();
        }
    }

    #endregion

    public Frustum<float> Frustum => new Frustum<float>((Matrix<float>) (ProjectionMatrix * ViewMatrix));

    /// <summary>
    /// Create a new perspective camera for use in 3D scenes.
    /// </summary>
    /// <param name="fov">The field of view, in radians, of this camera.</param>
    /// <param name="aspectRatio">The aspect ratio of this camera (typically width / height).</param>
    /// <param name="near">The near plane distance of this camera.</param>
    /// <param name="far">The far plane distance of this camera.</param>
    public Camera(float fov, float aspectRatio, float near = 0.1f, float far = 1000f, CameraType type = CameraType.Camera3D)
    {
        _projectionType = ProjectionType.Perspective;
        _fov = fov;
        _aspectRatio = aspectRatio;
        _near = near;
        _far = far;
        _orthoSize = Vector2<float>.One;
        ClearColor = Color.Black;
        CameraType = type;

        Viewport = new Vector4(0, 0, 1, 1);
        
        GenerateProjectionMatrix();
    }

    public Camera(Vector2<float>? orthoSize = null, CameraType type = CameraType.Camera3D)
    {
        _projectionType = ProjectionType.Orthographic;
        CameraType = type;
        _orthoSize = orthoSize ?? Vector2<float>.One;
        ClearColor = Color.Black;

        Viewport = new Vector4(0, 0, 1, 1);
        
        GenerateProjectionMatrix();
    }

    /// <summary>
    /// Quickly switch this camera to an orthographic 2D camera.
    /// </summary>
    public void UseOrtho2D()
    {
        CameraType = CameraType.Camera2D;
        ProjectionType = ProjectionType.Orthographic;
    }

    /// <summary>
    /// Quickly switch this camera to a perspective 3D camera.
    /// </summary>
    public void UsePerspective3D()
    {
        CameraType = CameraType.Camera3D;
        ProjectionType = ProjectionType.Perspective;
    }

    protected internal override void Initialize()
    {
        base.Initialize();
        
        Graphics.ViewportResized += GraphicsOnViewportResized;
    }

    private void GraphicsOnViewportResized(Rectangle<int> viewport)
    {
        _aspectRatio = viewport.Width / (float) viewport.Height;
        GenerateProjectionMatrix();
    }

    private void GenerateProjectionMatrix()
    {
        ProjectionMatrix = ProjectionType switch
        {
            ProjectionType.Perspective => Matrix4x4.CreatePerspectiveFieldOfView(_fov, _aspectRatio, _near, _far),
            ProjectionType.Orthographic => Matrix4x4.CreateOrthographicOffCenter(0, Graphics.Viewport.Width * _orthoSize.X, Graphics.Viewport.Height * _orthoSize.Y, 0, -1, 1),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override void Dispose()
    {
        base.Dispose();

        Graphics.ViewportResized -= GraphicsOnViewportResized;
    }

    /// <summary>
    /// Get the main camera for the current scene. This is the first camera in the scene with the
    /// <see cref="Tags.MainCamera"/> tag.
    /// </summary>
    public static Camera Main => (Camera) SceneManager.ActiveScene.GetEntitiesWithTag(Tags.MainCamera)[0];
}

public enum ProjectionType
{
    Perspective,
    Orthographic
}

[Flags]
public enum CameraType
{
    Camera2D = 1 << 0,
    Camera3D = 1 << 1
}