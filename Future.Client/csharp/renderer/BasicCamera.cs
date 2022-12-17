using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Future.Client.csharp.renderer;

public class BasicCamera {
    
    private GraphicsDevice graphicsDevice = null;
    private GameWindow gameWindow = null;

    private MouseState mState = default(MouseState);
    private KeyboardState kbState = default(KeyboardState);

    public float MovementUnitsPerSecond { get; set; } = 30f;
    public float RotationRadiansPerSecond { get; set; } = 60f;

    public float fieldOfViewDegrees = 80f;
    public float nearClipPlane = .05f;
    public float farClipPlane = 2000f;

    private float yMouseAngle = 0f;
    private float xMouseAngle = 0f;
    private bool mouseLookIsUsed = true;

    private int fpsKeyboardLayout = 1;
    private int cameraTypeOption = 1;

    /// <summary>
    /// operates pretty much like a fps camera.
    /// </summary>
    public const int CAM_UI_OPTION_FPS_LAYOUT = 0;
    /// <summary>
    /// I put this one on by default.
    /// free cam i use this for editing its more like a air plane or space camera.
    /// the horizon is not corrected for in this one so use the z and c keys to roll 
    /// hold the right mouse to look with it.
    /// </summary>
    public const int CAM_UI_OPTION_EDIT_LAYOUT = 1;
    /// <summary>
    /// Determines how the camera behaves fixed 0  free 1
    /// </summary>

    /// <summary>
    /// A fixed camera is typically used in fps games. It is called a fixed camera because the up is stabalized to the system vectors up.
    /// However be aware that this means if the forward vector or were you are looking is directly up or down you will gimble lock.
    /// Typically this is not allowed in many fps or rather it is faked so you can never truely look directly up or down.
    /// </summary>
    public const int CAM_TYPE_OPTION_FIXED = 0;
    /// <summary>
    /// A free camera has its up vector unlocked good for a space sim, air fighter game or editing. 
    /// It won't gimble lock. Provided the up is found from the cross of the right forward it can't gimble lock.
    /// The draw back is the horizon stabilization needs to be handled for some types of games.
    /// </summary>
    public const int CAM_TYPE_OPTION_FREE = 1;


    /// <summary>
    /// Constructs the camera.
    /// </summary>
    public BasicCamera(GraphicsDevice gfxDevice, GameWindow window)
    {
        graphicsDevice = gfxDevice;
        gameWindow = window;
        ReCreateWorldAndView();
        ReCreateThePerspectiveProjectionMatrix(gfxDevice, fieldOfViewDegrees);
    }

    /// <summary>
    /// Select how you want the ui to feel or how to control the camera by passing in Basic3dExampleCamera. CAM_UI_ options
    /// </summary>
    /// <param name="UiOption"></param>
    public void CameraUi(int UiOption)
    {
        fpsKeyboardLayout = UiOption;
    }
    /// <summary>
    /// Select a camera type fixed free or other by passing in ( Basic3dExampleCamera. CAM_TYPE_ options )
    /// </summary>
    /// <param name="cameraOption"></param>
    public void CameraType(int cameraOption)
    {
        cameraTypeOption = cameraOption;
    }

    /// <summary>
    /// This serves as the cameras up. For fixed cameras this might not change at all ever. For free cameras it changes constantly.
    /// A fixed camera keeps a fixed horizon but can gimble lock under normal rotation when looking straight up or down.
    /// A free camera has no fixed horizon but can't gimble lock under normal rotation as the up changes as the camera moves.
    /// Most hybrid cameras are a blend of the two but all are based on one or both of the above.
    /// </summary>
    private Vector3 up = Vector3.Up;
    /// <summary>
    /// This serves as the cameras world orientation like almost all 3d game objects they have a world matrix. 
    /// It holds all orientational values and is used to move the camera properly thru the world space.
    /// </summary>
    private Matrix camerasWorld = Matrix.Identity;
    /// <summary>
    /// The view matrice is created from the cameras world matrice but it has special properties.
    /// Using create look at to create this matrix you move from the world space into the view space.
    /// If you are working on world objects you should not take individual elements from this to directly operate on world matrix components.
    /// As well the multiplication of a view matrix by a world matrix moves the resulting matrix into view space itself.
    /// </summary>
    private Matrix viewMatrix = Matrix.Identity;
    /// <summary>
    /// The projection matrix. This matrice creates a vanishing point and skews all objects drawn to create the illusion of depth and a perspective parallax view at distance.
    /// </summary>
    private Matrix projectionMatrix = Matrix.Identity;

    /// <summary>
    /// Gets or sets the the camera's position in the world.
    /// </summary>
    public Vector3 Position
    {
        set
        {
            camerasWorld.Translation = value;
            // since we know here that a change has occured to the cameras world orientations we can update the view matrix.
            ReCreateWorldAndView();
        }
        get { return camerasWorld.Translation; }
    }
    /// <summary>
    /// Gets or Sets the direction the camera is looking at in the world.
    /// The forward is the same as the look at direction it i a directional vector not a position.
    /// </summary>
    public Vector3 Forward
    {
        set
        {
            camerasWorld = Matrix.CreateWorld(camerasWorld.Translation, value, up);
            // since we know here that a change has occured to the cameras world orientations we can update the view matrix.
            ReCreateWorldAndView();
        }
        get { return camerasWorld.Forward; }
    }
    /// <summary>
    /// Get the cameras up vector. You shouldn't need to set the up you shouldn't at all if you are using the free camera type.
    /// </summary>
    public Vector3 Up
    {
        set
        {
            up = value;
            camerasWorld = Matrix.CreateWorld(camerasWorld.Translation, camerasWorld.Forward, value);
            // since we know here that a change has occured to the cameras world orientations we can update the view matrix.
            ReCreateWorldAndView();
        }
        get { return up; }
    }

    /// <summary>
    /// Gets or Sets the direction the camera is looking at in the world as a directional vector.
    /// </summary>
    public Vector3 LookAtDirection
    {
        set
        {
            camerasWorld = Matrix.CreateWorld(camerasWorld.Translation, value, up);
            // since we know here that a change has occured to the cameras world orientations we can update the view matrix.
            ReCreateWorldAndView();
        }
        get { return camerasWorld.Forward; }
    }
    /// <summary>
    /// Sets a positional target in the world to look at.
    /// </summary>
    public Vector3 TargetPositionToLookAt
    {
        set
        {
            camerasWorld = Matrix.CreateWorld(camerasWorld.Translation, Vector3.Normalize(value - camerasWorld.Translation), up);
            // since we know here that a change has occured to the cameras world orientations we can update the view matrix.
            ReCreateWorldAndView();
        }
    }
    /// <summary>
    /// Turns the camera to face the target this method just takes in the targets matrix for convienience.
    /// </summary>
    public Matrix LookAtTheTargetMatrix
    {
        set
        {
            camerasWorld = Matrix.CreateWorld(camerasWorld.Translation, Vector3.Normalize(value.Translation - camerasWorld.Translation), up);
            // since we know here that a change has occured to the cameras world orientations we can update the view matrix.
            ReCreateWorldAndView();
        }
    }

    /// <summary>
    /// Directly set or get the world matrix this also updates the view matrix
    /// </summary>
    public Matrix World
    {
        get
        {
            return camerasWorld;
        }
        set
        {
            camerasWorld = value;
            viewMatrix = Matrix.CreateLookAt(camerasWorld.Translation, camerasWorld.Forward + camerasWorld.Translation, camerasWorld.Up);
        }
    }

    /// <summary>
    /// Gets the view matrix we never really set the view matrix ourselves outside this method just get it.
    /// The view matrix is remade internally when we know the world matrix forward or position is altered.
    /// </summary>
    public Matrix View
    {
        get
        {
            return viewMatrix;
        }
    }

    /// <summary>
    /// Gets the projection matrix.
    /// </summary>
    public Matrix Projection
    {
        get
        {
            return projectionMatrix;
        }
    }

    /// <summary>
    /// When the cameras position or orientation changes, we call this to ensure that the cameras world matrix is orthanormal.
    /// We also set the up depending on our choices of is fix or free camera and we then update the view matrix.
    /// </summary>
    private void ReCreateWorldAndView()
    {
        if (cameraTypeOption == CAM_TYPE_OPTION_FIXED)
            up = Vector3.Up;
        if (cameraTypeOption == CAM_UI_OPTION_EDIT_LAYOUT )
            up = camerasWorld.Up;

        camerasWorld = Matrix.CreateWorld(camerasWorld.Translation, camerasWorld.Forward, up);
        viewMatrix = Matrix.CreateLookAt(camerasWorld.Translation, camerasWorld.Forward + camerasWorld.Translation, camerasWorld.Up);
    }

    /// <summary>
    /// Changes the perspective matrix to a new near far and field of view.
    /// </summary>
    public void ReCreateThePerspectiveProjectionMatrix(GraphicsDevice gd, float fovInDegrees)
    {
        float aspectRatio = graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;
        projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fovInDegrees * (float)((3.14159265358f) / 180f), aspectRatio, .05f, 1000f);
    }
    /// <summary>
    /// Changes the perspective matrix to a new near far and field of view.
    /// The projection matrix is typically only set up once at the start of the app.
    /// </summary>
    public void ReCreateThePerspectiveProjectionMatrix(float fieldOfViewInDegrees, float nearPlane, float farPlane)
    {
        // create the projection matrix.
        this.fieldOfViewDegrees = MathHelper.ToRadians(fieldOfViewInDegrees);
        nearClipPlane = nearPlane;
        farClipPlane = farPlane;
        float aspectRatio = graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;
        projectionMatrix = Matrix.CreatePerspectiveFieldOfView(this.fieldOfViewDegrees, aspectRatio, nearClipPlane, farClipPlane);
    }

    /// <summary>
    /// update the camera.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        if (fpsKeyboardLayout == CAM_UI_OPTION_FPS_LAYOUT)
            FpsUiControlsLayout(gameTime);
        if (fpsKeyboardLayout == CAM_UI_OPTION_EDIT_LAYOUT)
            EditingUiControlsLayout(gameTime);
    }

    /// <summary>
    /// like a fps games camera right clicking turns mouse look on or off same for the edit mode.
    /// </summary>
    /// <param name="gameTime"></param>
    private void FpsUiControlsLayout(GameTime gameTime)
    {
        MouseState state = Mouse.GetState(gameWindow);
        KeyboardState kstate = Keyboard.GetState();
        if (kstate.IsKeyDown(Keys.W))
        {
            MoveForward(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.S) == true)
        {
            MoveBackward(gameTime);
        }
        // strafe. 
        if (kstate.IsKeyDown(Keys.A) == true)
        {
            MoveLeft(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.D) == true)
        {
            MoveRight(gameTime);
        }

        // rotate 
        if (kstate.IsKeyDown(Keys.Left) == true)
        {
            RotateLeft(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.Right) == true)
        {
            RotateRight(gameTime);
        }
        // rotate 
        if (kstate.IsKeyDown(Keys.Up) == true)
        {
            RotateUp(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.Down) == true)
        {
            RotateDown(gameTime);
        }

        if (kstate.IsKeyDown(Keys.Q) == true)
        {
            if (cameraTypeOption == CAM_TYPE_OPTION_FIXED)
                MoveUpInNonLocalSystemCoordinates(gameTime);
            if (cameraTypeOption == CAM_TYPE_OPTION_FREE)
                MoveUp(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.E) == true)
        {
            if (cameraTypeOption == CAM_TYPE_OPTION_FIXED)
                MoveDownInNonLocalSystemCoordinates(gameTime);
            if (cameraTypeOption == CAM_TYPE_OPTION_FREE)
                MoveDown(gameTime);
        }


        if (state.LeftButton == ButtonState.Pressed)
        {
            if (mouseLookIsUsed == false)
                mouseLookIsUsed = true;
            else
                mouseLookIsUsed = false;
        }
        if (mouseLookIsUsed)
        {
            Vector2 diff = state.Position.ToVector2() - mState.Position.ToVector2();
            if (diff.X != 0f)
                RotateLeftOrRight(gameTime, diff.X);
            if (diff.Y != 0f)
                RotateUpOrDown(gameTime, diff.Y);
        }
        mState = state;
        kbState = kstate;
    }

    /// <summary>
    /// when working like programing editing and stuff.
    /// </summary>
    /// <param name="gameTime"></param>
    private void EditingUiControlsLayout(GameTime gameTime)
    {
        MouseState state = Mouse.GetState(gameWindow);
        KeyboardState kstate = Keyboard.GetState();
        if (kstate.IsKeyDown(Keys.E))
        {
            MoveForward(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.Q) == true)
        {
            MoveBackward(gameTime);
        }
        if (kstate.IsKeyDown(Keys.W))
        {
            RotateUp(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.S) == true)
        {
            RotateDown(gameTime);
        }
        if (kstate.IsKeyDown(Keys.A) == true)
        {
            RotateLeft(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.D) == true)
        {
            RotateRight(gameTime);
        }

        if (kstate.IsKeyDown(Keys.Left) == true)
        {
            MoveLeft(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.Right) == true)
        {
            MoveRight(gameTime);
        }
        // rotate 
        if (kstate.IsKeyDown(Keys.Up) == true)
        {
            MoveUp(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.Down) == true)
        {
            MoveDown(gameTime);
        }

        // roll counter clockwise
        if (kstate.IsKeyDown(Keys.Z) == true)
        {
            if (cameraTypeOption == CAM_TYPE_OPTION_FREE)
                RotateRollCounterClockwise(gameTime);
        }
        // roll clockwise
        else if (kstate.IsKeyDown(Keys.C) == true)
        {
            if (cameraTypeOption == CAM_TYPE_OPTION_FREE)
                RotateRollClockwise(gameTime);
        }

        if (state.RightButton == ButtonState.Pressed)
                mouseLookIsUsed = true;
        else
            mouseLookIsUsed = false;
        if (mouseLookIsUsed)
        {
            Vector2 diff = state.Position.ToVector2() - mState.Position.ToVector2();
            if (diff.X != 0f)
                RotateLeftOrRight(gameTime, diff.X);
            if (diff.Y != 0f)
                RotateUpOrDown(gameTime, diff.Y);
        }
        mState = state;
        kbState = kstate;
    }

    /// <summary>
    /// This function can be used to check if gimble is about to occur in a fixed camera.
    /// If this value returns 1.0f you are in a state of gimble lock, However even as it gets near to 1.0f you are in danger of problems.
    /// In this case you should interpolate towards a free camera. Or begin to handle it.
    /// Earlier then .9 in some manner you deem to appear fitting otherwise you will get a hard spin effect. Though you may want that.
    /// </summary>
    public float GetGimbleLockDangerValue()
    {
        var c0 = Vector3.Dot(World.Forward, World.Up);
        if (c0 < 0f) c0 = -c0;
        return c0;
    }
    
    #region Local Translations and Rotations.

    public void MoveForward(GameTime gameTime)
    {
        Position += (camerasWorld.Forward * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void MoveBackward(GameTime gameTime)
    {
        Position += (camerasWorld.Backward * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void MoveLeft(GameTime gameTime)
    {
        Position += (camerasWorld.Left * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void MoveRight(GameTime gameTime)
    {
        Position += (camerasWorld.Right * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void MoveUp(GameTime gameTime)
    {
        Position += (camerasWorld.Up * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void MoveDown(GameTime gameTime)
    {
        Position += (camerasWorld.Down * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void RotateUp(GameTime gameTime)
    {
        var radians = RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(camerasWorld.Right, MathHelper.ToRadians(radians));
        LookAtDirection = Vector3.TransformNormal(LookAtDirection, matrix);
        ReCreateWorldAndView();
    }
    public void RotateDown(GameTime gameTime)
    {
        var radians = -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(camerasWorld.Right, MathHelper.ToRadians(radians));
        LookAtDirection = Vector3.TransformNormal(LookAtDirection, matrix);
        ReCreateWorldAndView();
    }
    public void RotateLeft(GameTime gameTime)
    {
        var radians = RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(camerasWorld.Up, MathHelper.ToRadians(radians));
        LookAtDirection = Vector3.TransformNormal(LookAtDirection, matrix);
        ReCreateWorldAndView();
    }
    public void RotateRight(GameTime gameTime)
    {
        var radians = -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(camerasWorld.Up, MathHelper.ToRadians(radians));
        LookAtDirection = Vector3.TransformNormal(LookAtDirection, matrix);
        ReCreateWorldAndView();
    }
    public void RotateRollClockwise(GameTime gameTime)
    {
        var radians = RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        var pos = camerasWorld.Translation;
        camerasWorld *= Matrix.CreateFromAxisAngle(camerasWorld.Forward, MathHelper.ToRadians(radians));
        camerasWorld.Translation = pos;
        ReCreateWorldAndView();
    }
    public void RotateRollCounterClockwise(GameTime gameTime)
    {
        var radians = -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        var pos = camerasWorld.Translation;
        camerasWorld *= Matrix.CreateFromAxisAngle(camerasWorld.Forward, MathHelper.ToRadians(radians));
        camerasWorld.Translation = pos;
        ReCreateWorldAndView();
    }

    // just for example this is the same as the above rotate left or right.
    public void RotateLeftOrRight(GameTime gameTime, float amount)
    {
        var radians = amount * -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(camerasWorld.Up, MathHelper.ToRadians(radians));
        LookAtDirection = Vector3.TransformNormal(LookAtDirection, matrix);
        ReCreateWorldAndView();
    }
    public void RotateUpOrDown(GameTime gameTime, float amount)
    {
        var radians = amount * -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(camerasWorld.Right, MathHelper.ToRadians(radians));
        LookAtDirection = Vector3.TransformNormal(LookAtDirection, matrix);
        ReCreateWorldAndView();
    } 

    #endregion

    #region Non Local System Translations and Rotations.

    public void MoveForwardInNonLocalSystemCoordinates(GameTime gameTime)
    {
        Position += (Vector3.Forward * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void MoveBackwardsInNonLocalSystemCoordinates(GameTime gameTime)
    {
        Position += (Vector3.Backward * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void MoveUpInNonLocalSystemCoordinates(GameTime gameTime)
    {
        Position += (Vector3.Up * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void MoveDownInNonLocalSystemCoordinates(GameTime gameTime)
    {
        Position += (Vector3.Down * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void MoveLeftInNonLocalSystemCoordinates(GameTime gameTime)
    {
        Position += (Vector3.Left * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void MoveRightInNonLocalSystemCoordinates(GameTime gameTime)
    {
        Position += (Vector3.Right * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    /// <summary>
    /// These aren't typically useful and you would just use create world for a camera snap to a new view. I leave them for completeness.
    /// </summary>
    public void NonLocalRotateLeftOrRight(GameTime gameTime, float amount)
    {
        var radians = amount * -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(radians));
        LookAtDirection = Vector3.TransformNormal(LookAtDirection, matrix);
        ReCreateWorldAndView();
    }
    /// <summary>
    /// These aren't typically useful and you would just use create world for a camera snap to a new view.  I leave them for completeness.
    /// </summary>
    public void NonLocalRotateUpOrDown(GameTime gameTime, float amount)
    {
        var radians = amount * -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(radians));
        LookAtDirection = Vector3.TransformNormal(LookAtDirection, matrix);
        ReCreateWorldAndView();
    }

    #endregion
}