using Easel.GUI;
using Easel.Math;
using Easel.Scenes;
using Future.Client.csharp.registry;
using Future.Client.csharp.translation;

namespace Future.Client.csharp.scenes;

public class Menu : Scene {
    
    protected override void Initialize() {
        base.Initialize();
        
        UI.Theme.Font = ClientFontRegistry.Fontoe;
        
        UI.Add("hello", new Label(new Position(Anchor.CenterCenter), Translation.Lang.Get("gui.idiot"), 18, Color.White));
        UI.Add("test", new Button(new Position(Anchor.BottomCenter), new Size<int>(200, 50), Translation.Lang.Get("gui.fuck"), 10));
        //UI.Add("testModel", new );
    }
    
    protected override void Draw() {
        base.Draw();
    }
}