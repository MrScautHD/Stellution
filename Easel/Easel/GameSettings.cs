using System.Reflection;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Pie;
using Pie.Windowing;

namespace Easel;

/// <summary>
/// Initial game settings used on game startup.
/// </summary>
public struct GameSettings
{
    /// <summary>
    /// The starting size (resolution) of the game view, in pixels. (Default: 1280x720)
    /// </summary>
    public Size<int> Size;

    /// <summary>
    /// If enabled, the window will start in fullscreen mode at the given <see cref="Size"/>. (Default: false)
    /// </summary>
    public bool Fullscreen;

    /// <summary>
    /// The starting title of the game view. (Default: The starting assembly name)
    /// </summary>
    public string Title;

    /// <summary>
    /// The initial border of the view. (Default: <see cref="WindowBorder.Fixed"/>)
    /// </summary>
    public WindowBorder Border;

    /// <summary>
    /// Whether or not the game will synchronize to the vertical refresh rate. (Default: <see langword="true" />)
    /// </summary>
    public bool VSync;

    /// <summary>
    /// The target frames per second of the game. Set to 0 to have unlimited FPS. (Default: 0, if <see cref="VSync"/> is
    /// enabled the game will run at the monitor's native refresh rate (typically 60, 144, etc.)
    /// </summary>
    public int TargetFps;

    /// <summary>
    /// The graphics API the game will use - leave null to let Easel decide which API to use. (Default: <see langword="null"/>)
    ///
    /// Default API:
    /// <list type="bullet">
    ///     <item><b>Windows -</b> <see cref="GraphicsApi.D3D11"/></item>
    ///     <item><b>Linux -</b> <see cref="GraphicsApi.OpenGl33"/></item>
    ///     <item><b>macOS -</b> <see cref="GraphicsApi.OpenGl33"/> - Warning: macOS is not officially supported</item>
    /// </list>
    /// </summary>
    public GraphicsApi? Api;

    /// <summary>
    /// If enabled, Easel will not error if it tries to load items that do not exist, such as textures, instead
    /// displaying a default "missing" object. (Default: <see langword="false" />
    /// </summary>
    public bool AllowMissing;

    /// <summary>
    /// The view icon, if any. (Default: <see langword="null" />)
    /// </summary>
    public Bitmap Icon;

    /// <summary>
    /// The title bar flags, if any (Default: <see cref="Easel.TitleBarFlags.ShowEasel"/>)
    /// </summary>
    public TitleBarFlags TitleBarFlags;

    /// <summary>
    /// If disabled, the game window will not be visible until you tell it to become visible.
    /// </summary>
    public bool StartVisible;

    /// <summary>
    /// The render options Easel will use for your application.
    /// </summary>
    public RenderOptions RenderOptions;

    /// <summary>
    /// Run the game in a configuration where no graphics device or window is created.
    /// </summary>
    public bool Server;

    public GameSettings(Size<int> size, string title, bool fullscreen, WindowBorder border, bool vSync, int targetFps, 
        GraphicsApi? api, bool allowMissing, Bitmap icon, TitleBarFlags titleBarFlags, bool startVisible,
        RenderOptions renderOptions, bool server)
    {
        Size = size;
        Fullscreen = fullscreen;
        Title = title;
        Border = border;
        VSync = vSync;
        TargetFps = targetFps;
        Api = api;
        AllowMissing = allowMissing;
        Icon = icon;
        TitleBarFlags = titleBarFlags;
        StartVisible = startVisible;
        RenderOptions = renderOptions;
        Server = server;
    }

    /// <summary>
    /// Create the default game settings.
    /// </summary>
    public GameSettings()
    {
        Size = new Size<int>(1280, 720);
        Fullscreen = false;

        Title = Assembly.GetEntryAssembly()?.GetName().Name ?? "Easel Window";
        Border = WindowBorder.Fixed;
        VSync = true;
        TargetFps = 0;
        Api = null;
        AllowMissing = false;
        Icon = null;
        TitleBarFlags = TitleBarFlags.ShowEasel;
        StartVisible = true;
        RenderOptions = RenderOptions.Default;
        Server = false;
    }

    public static GameSettings StartFullscreen => new GameSettings()
    {
        Size = new Size<int>(-1, -1),
        Fullscreen = true
    };
}