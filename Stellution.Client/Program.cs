using Easel;
using Easel.Graphics;
using Pie.Windowing;
using Stellution.Client.csharp;
using Stellution.Client.csharp.scenes;

GameSettings settings = new GameSettings {
    Title = "Stellution (WIP)",
    Icon = new Bitmap("content/bitmaps/logo/logo.bmp"),
    AutoGenerateContentDirectory = null,
    TitleBarFlags = TitleBarFlags.ShowGraphicsApi,
    VSync = false,
    Border = WindowBorder.Resizable
};

using StellutionClient game = new StellutionClient(settings, new Menu());
game.Run();