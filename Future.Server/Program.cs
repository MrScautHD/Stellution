using Easel;
using Future.Server.csharp;
using Future.Server.csharp.scenes;

GameSettings settings = new GameSettings();
settings.Server = true;

using FutureServer server = new FutureServer(settings, new TestScene());
server.StartServer();