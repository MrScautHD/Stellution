using Easel;
using Easel.Graphics;
using Stellution.Common.csharp.scenes;
using Pie.Windowing;
using Stellution.Client.csharp;

GameSettings settings = new GameSettings {
    Title = "Future (WIP)",
    Icon = new Bitmap("content/bitmaps/logo/logo.bmp"),
    AutoGenerateContentDirectory = null,
    TitleBarFlags = TitleBarFlags.ShowGraphicsApi,
    VSync = false,
    Border = WindowBorder.Resizable
};

using StellutionClient game = new StellutionClient(settings, new Earth());
game.Run();