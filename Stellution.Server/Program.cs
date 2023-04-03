using Easel;
using Stellution.Common.csharp.scenes;
using Stellution.Server.csharp;

GameSettings settings = new GameSettings() {
    AutoGenerateContentDirectory = null,
    TargetFps = 60,
    Server = true
};

using StellutionServer server = new StellutionServer(settings, new Earth());
server.Run();