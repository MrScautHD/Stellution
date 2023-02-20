using System;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Graphics;
using Easel.Math;
using Easel.Scenes;

namespace Easel.Tests.TestScenes;

public class TestCanvas : Scene
{
    private Texture2D _texture;
    private Canvas _canvas;

    private Bitmap _bitmap1;
    private Bitmap _bitmap2;

    protected override void Initialize()
    {
        base.Initialize();

        _bitmap1 = new Bitmap("/home/ollie/Pictures/ball.png");
        _bitmap2 = new Bitmap("/home/ollie/Pictures/awesomeface.png");
        
        Camera.Main.UseOrtho2D();

        _canvas = new Canvas(new Size<int>(800, 600));
        _canvas.Clear(Color.CornflowerBlue);

        _texture = new Texture2D(_canvas.ToBitmap());

        Entity drawEntity = new Entity();
        drawEntity.AddComponent(new Sprite(_texture));
        AddEntity(drawEntity);
    }
    
    protected override void Draw()
    {
        _canvas.Clear(Color.CornflowerBlue);

        //_canvas.DrawBitmap(0, 0, (int) EaselMath.Lerp(100, 800, (MathF.Sin(Time.TotalSeconds) + 1) / 2f), 600, _bitmap2);
        _canvas.DrawBitmap(0, 0, 800, 600, _bitmap2);
        
        _texture.SetData(0, 0, _canvas.Size.Width, _canvas.Size.Height, _canvas.ToBitmap().Data);
        
        base.Draw();
    }
}