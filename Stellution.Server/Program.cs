using Easel;
using Stellution.Common.csharp.scenes;
using Stellution.Server.csharp;

GameSettings settings = new GameSettings() {
    TargetFps = 60
};

using StellutionServer server = new StellutionServer(settings, new Earth());
server.Run();