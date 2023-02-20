using Easel;
using Future.Client.csharp;
using Future.Client.csharp.scenes;
using Pie.Windowing;

GameSettings settings = new GameSettings();
settings.Title = "Future (WIP)";
settings.TitleBarFlags = TitleBarFlags.ShowGraphicsApi;
settings.VSync = false;
settings.Border = WindowBorder.Resizable;

using EaselGame game = new FutureClient(settings, new Menu());
game.Run();