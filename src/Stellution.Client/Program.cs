using Easel;
using Easel.Graphics;
using Pie.Windowing;
using Stellution.Client.csharp;
using Stellution.Common.csharp.scenes;

GameSettings settings = new GameSettings {
    Title = "Stellution (WIP)",
    Icon = new Bitmap("content/bitmaps/logo/logo.bmp"),
    AutoGenerateContentDirectory = null,
    TitleBarFlags = TitleBarFlags.ShowGraphicsApi,
    VSync = false,
    Border = WindowBorder.Resizable,
    /*
    Size = new Size<int>(videoMode.Size.Width, videoMode.Size.Height),
    Fullscreen = false*/
};

using StellutionClient game = new StellutionClient(settings, new Earth());
game.Run();