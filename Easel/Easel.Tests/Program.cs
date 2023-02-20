using System;
using System.Numerics;
using Easel;
using Easel.Core;
using Easel.Math;
using Easel.Tests;
using Easel.Tests.TestScenes;
using Pie.Windowing;

GameSettings settings = new GameSettings()
{
    Border = WindowBorder.Resizable,
    Size = new Size<int>(1024, 1024),
    Server = true,
    TargetFps = 60
};

Logger.UseConsoleLogs();

using TestGame game = new TestGame(settings, new TestServer());
game.Run();

/*QuaternionT<float> quat = QuaternionT.FromEuler(1f, 0.5f, 0.25f);
Console.WriteLine(quat);

Quaternion quat2 = Quaternion.CreateFromYawPitchRoll(1f, 0.5f, 0.25f);
Console.WriteLine(quat2);

Console.WriteLine(MatrixT.FromQuaternion(quat));

Console.WriteLine(Matrix4x4.CreateFromQuaternion(quat2));*/