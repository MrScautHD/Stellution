using System.Numerics;
using Easel;
using Easel.Entities.Components;
using Easel.GUI;
using Easel.Math;
using Easel.Scenes;
using Future.Client.csharp.registry;
using Future.Client.csharp.translation;
using Future.Common.csharp.entity;
using Pie.Windowing;

namespace Future.Client.csharp.scenes;

public class Menu : Scene {
    
    protected override void Initialize() {
        base.Initialize();
        
        UI.Theme.Font = ClientFontRegistry.Fontoe;
        
        UI.Add("hello", new Label(new Position(Anchor.CenterCenter), Translation.Lang.Get("gui.idiot"), 18, Color.White));
        UI.Add("test", new Button(new Position(Anchor.BottomCenter), new Size<int>(200, 50), Translation.Lang.Get("gui.fuck"), 10));
        //UI.Add("testModel", new );
        
        //car.AddComponent(ClientRendererRegistry.CyberCarRenderer);
        
        
        CyberCar car = new CyberCar();
        this.AddEntity("cyber_car", car);
        this.GetEntity("cyber_car").AddComponent(new ModelRenderer(ClientModelRegistry.CyberCarModel));

        CyberCar car2 = new CyberCar();
        this.AddEntity("test", car2);
        this.GetEntity("test").AddComponent(new ModelRenderer(ClientModelRegistry.CyberCarModel));
    }

    protected override void Update() {
        base.Update();
        
        if (Input.KeyDown(Key.A)) {
            this.GetEntity("Main Camera").Transform.Position += new Vector3(+0.01F, 0, 0);
        }
        
        if (Input.KeyDown(Key.D)) {
            this.GetEntity("Main Camera").Transform.Position += new Vector3(-0.01F, 0, 0);
        }

        if (Input.KeyDown(Key.W)) {
            this.GetEntity("Main Camera").Transform.Position += new Vector3(0, 0, -0.01F);
        }
        
        if (Input.KeyDown(Key.S)) {
            this.GetEntity("Main Camera").Transform.Position += new Vector3(0, 0, +0.01F);
        }
    }
}