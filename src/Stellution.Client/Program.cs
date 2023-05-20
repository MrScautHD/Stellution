using Easel;
using Easel.Graphics;
using Easel.Math;
using Pie.Windowing;
using Stellution.Client.csharp;
using Stellution.Common.csharp.scenes;

GameSettings settings = new GameSettings {
    Title = "Stellution (WIP)",
    Icon = new Bitmap("content/bitmaps/logo/logo.bmp"),
    AutoGenerateContentDirectory = null,
    TitleBarFlags = TitleBarFlags.ShowGraphicsApi,
    VSync = true,
    Border = WindowBorder.Resizable,
    
    //Size = new Size<int>(videoMode.Size.Width, videoMode.Size.Height),
    Size = new Size<int>(1920, 1080),
    Fullscreen = true
};

using StellutionClient game = new StellutionClient(settings, new Earth());
game.Run();