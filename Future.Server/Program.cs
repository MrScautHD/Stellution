using Easel;
using Future.Common.csharp.scenes;
using Future.Server.csharp;

GameSettings settings = new GameSettings() {
    AutoGenerateContentDirectory = null,
    TargetFps = 60,
    Server = true
};

using FutureServer server = new FutureServer(settings, new Earth());
server.Run();