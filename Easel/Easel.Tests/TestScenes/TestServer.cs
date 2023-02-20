using System;
using Easel.Entities;
using Easel.Scenes;

namespace Easel.Tests.TestScenes;

public class TestServer : Scene
{
    protected override void Initialize()
    {
        base.Initialize();

        Entity entity = new Entity();
        AddEntity("test", entity);
    }

    protected override void Update()
    {
        base.Update();

        Entity test = GetEntity("test");
        test.Transform.Position.X -= 1 * Time.DeltaTime;
        Console.WriteLine(Time.DeltaTime);
    }
}