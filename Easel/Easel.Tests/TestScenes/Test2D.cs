using System;
using System.IO;
using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Formats;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;
using Easel.Scenes;
using Sprite = Easel.Entities.Components.Sprite;

namespace Easel.Tests.TestScenes;

public class Test2D : Scene
{
    private SpriteRenderer.SpriteVertex[] _vertices;
    private uint[] _indices;
    private Texture2D _texture;

    protected override void Initialize()
    {
        base.Initialize();
        
        //File.WriteAllBytes("/home/ollie/Pictures/ETF/test.etf", ETF.CreateEtf(new Bitmap("/home/ollie/Pictures/24bitcolor.png"), customData: "(C) SPACEBOX 2023"));

        ETF tex = new ETF(File.ReadAllBytes("/home/ollie/Pictures/ETF/test.etf"));
        
        //Console.WriteLine("loading dds");
        //DDS tex = new DDS(File.ReadAllBytes("/home/ollie/Downloads/RubberCuboidFloor/RubberCuboidFloor_4K_BaseColor.dds"));
        
        Camera.Main.UseOrtho2D();
        Camera.Main.ClearColor = Color.CornflowerBlue;
        
        _vertices = new SpriteRenderer.SpriteVertex[]
        {
            new(new Vector2<float>(0, 0), new Vector2<float>(0, 0), Color.White),
            new(new Vector2<float>(1024, 0), new Vector2<float>(1, 0), Color.White),
            new(new Vector2<float>(1024, 1024), new Vector2<float>(1, 1), Color.White),
            new(new Vector2<float>(0, 1024), new Vector2<float>(0, 1), Color.White),
        };
        
        _indices = new uint[]
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };

        /*_font = Content.Load<Font>("Abel-Regular");

        const uint size = 128;
        const string text = "docs.piegfx.com";
        
        _rt = new RenderTarget(_font.MeasureString(size, text) + new Size<int>(0, 10));
        
        Graphics.SetRenderTarget(_rt);
        Graphics.Clear(Color.Transparent);
        Graphics.SpriteRenderer.Begin();
        _font.Draw(Graphics.SpriteRenderer, size, text, Vector2<int>.Zero, Color.White);
        Graphics.SpriteRenderer.End();
        Graphics.SetRenderTarget(null);

        */

        _texture = new Texture2D(tex.Bitmaps[0][0], SamplerState.LinearClamp);
        
        /*Entity sprite = new Entity();
        sprite.AddComponent(new Sprite(texture));
        AddEntity(sprite);
        
        UI.Add("test", new Label(new Position(Anchor.CenterCenter), "Stuff", 100, Color.Red));*/
    }

    private float _f;
    
    protected override void Draw()
    {
        base.Draw();

        Graphics.SpriteRenderer.Begin();
        Graphics.SpriteRenderer.DrawVertices(_texture, _vertices, _indices);
        Graphics.SpriteRenderer.End();

        //Camera.Main.ClearColor = Color.FromHsv(200, 0.5f, 0.75f);
        //Camera.Main.Transform.Position.X += 1;
    }
}