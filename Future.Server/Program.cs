using Easel;
using Future.Common.csharp.scenes;
using Future.Server.csharp;

GameSettings settings = new GameSettings();
settings.TargetFps = 60;
settings.Server = true;

using FutureServer server = new FutureServer(settings, new Earth());
server.StartServer();