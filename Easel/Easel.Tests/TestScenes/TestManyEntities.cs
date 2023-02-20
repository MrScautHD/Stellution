using System;
using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Graphics;
using Easel.Math;
using Easel.Scenes;

namespace Easel.Tests.TestScenes;

public class TestManyEntities : Scene
{
    protected override void Initialize()
    {
        base.Initialize();
        
        Camera.Main.UseOrtho2D();

        Texture2D texture = Content.Load<Texture2D>("awesomeface");
        Random random = new Random();
        
        for (int i = 0; i < 13457; i++)
        {
            Entity entity = new Entity(new Transform()
            {
                Position = new Vector3(random.Next(0, 1280), random.Next(0, 720), 0),
                SpriteRotation = random.NextSingle() * MathF.PI * 2,
                Scale = new Vector3(random.NextSingle() * 0.5f, random.NextSingle() * 0.5f, 1.0f)
            });
            entity.AddComponent(new Sprite(texture));
            AddEntity(entity);
        }
    }
}