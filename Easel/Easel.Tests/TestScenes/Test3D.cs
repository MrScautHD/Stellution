using System;
using System.IO;
using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Formats;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Easel.Graphics.Primitives;
using Easel.GUI;
using Easel.Math;
using Easel.Scenes;
using Pie.Windowing;

namespace Easel.Tests.TestScenes;

public class Test3D : Scene
{
    protected override void Initialize()
    {
        base.Initialize();

        //Input.MouseState = MouseState.Locked;
        
        //Graphics.ResizeGraphics(new Size<int>(1280, 720));

        DDS dds = new DDS(File.ReadAllBytes("/home/ollie/Pictures/RubberFloor.dds"));

        Texture2D texture = Content.Load<Texture2D>("awesomeface");
        texture.SamplerState = SamplerState.AnisotropicRepeat;
        
        Camera.Main.ClearColor = Color.CornflowerBlue;
        Bitmap right = Content.Load<Bitmap>("right");
        Bitmap left = Content.Load<Bitmap>("left");
        Bitmap top = Content.Load<Bitmap>("top");
        Bitmap bottom = Content.Load<Bitmap>("bottom");
        Bitmap front = Content.Load<Bitmap>("front");
        Bitmap back = Content.Load<Bitmap>("back");
        Camera.Main.Skybox = new Skybox(right, left, top, bottom, front, back);
        Camera.Main.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(EaselMath.ToRadians(20), 0, 0);
        Camera.Main.Viewport = new Vector4T<float>(0, 0, 0.5f, 1f);
        Camera.Main.AddComponent(new NoClipCamera()
        {
            MoveSpeed = 10
        });
        Material mat = new StandardMaterial(texture, Texture2D.EmptyNormal, Texture2D.Black, Texture2D.Black, Texture2D.White);
        //Camera.Main.AddComponent(new MeshRenderer(new MaterialMesh(Mesh.FromPrimitive(new Cube()), mat)));

        Camera second = new Camera(EaselMath.ToRadians(75), 640 / 360f);
        second.Transform.Position = new Vector3(0, 0, -5);
        second.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(EaselMath.ToRadians(180), 0, 0);
        second.ClearColor = Color.RebeccaPurple;
        //Bitmap awesomeface = Content.Load<Bitmap>("awesomeface");
        // lol
        //second.Skybox = new Skybox(awesomeface, awesomeface, awesomeface, awesomeface, awesomeface, awesomeface);
        second.Skybox = Camera.Main.Skybox;
        second.Tag = Tags.MainCamera;
        second.Viewport = new Vector4T<float>(0.5f, 0, 1.0f, 0.5f);
        AddEntity("second", second);

        Camera third = new Camera(EaselMath.ToRadians(75), 640 / 360f);
        third.Transform.Position = new Vector3(-3.5f, 1f, 0f);
        third.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(EaselMath.ToRadians(-20), 0, 0);
        third.ClearColor = Color.Orange;
        // TODO: Better camera solution than forcing every camera to use main camera tag.
        third.Tag = Tags.MainCamera;
        third.Skybox = Camera.Main.Skybox;
        third.Viewport = new Vector4T<float>(0.5f, 0.5f, 1.0f, 1.0f);
        AddEntity("third", third);

        Entity entity = new Entity(new Transform()
        {
            Position = new Vector3(0, 0, -3)
        });

        //entity.AddComponent(new MeshRenderer(new MaterialMesh(Mesh.FromPrimitive(new Cube()), mat)));
        AddEntity("cube", entity);

        Entity thingy = new Entity();
        thingy.AddComponent(new Sprite(texture));
        AddEntity(thingy);
        
        UI.Add("test", new Label(new Position(Anchor.BottomLeft), "Hello NativeAOT!", 24));
        
        //UI.Add("test2", new GaussianBlur(new Position(Anchor.CenterCenter, new Vector2<int>(0, 0)), new Size<int>(300), 0.9f, 12));
    }

    protected override void Update()
    {
        base.Update();
        
        if (Input.KeyPressed(Key.Escape))
            Game.Close();
        
        //Camera.Main.Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, 1 * Time.DeltaTime);
        //GetEntity<Camera>("second").Transform.Rotation *= Quaternion.CreateFromAxisAngle(-Vector3.UnitY, 1 * Time.DeltaTime)

        GetEntity("cube").Transform.Rotation *=
            Quaternion.CreateFromAxisAngle(Vector3.UnitX, 1 * Time.DeltaTime) *
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.75f * Time.DeltaTime) *
            Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0.34f * Time.DeltaTime);
    }
}