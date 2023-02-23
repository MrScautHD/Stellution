using Easel.GUI;
using Easel.Math;
using Future.Client.csharp.registry;
using Future.Client.csharp.translation;
using Future.Common.csharp.scenes;

namespace Future.Client.csharp.scenes;

public class Menu : ModifiedScene {

    public override string SceneKey() {
        return "menu";
    }

    protected override void Initialize() {
        base.Initialize();

        UI.Theme.Font = ClientFontRegistry.Fontoe;
        UI.Theme.Blur = new GaussianBlur(100, 100);

        //Size windowSize = EaselGame.Instance.Window.Size;
        //UI.Add("background", new Image(new Position(Anchor.CenterCenter), ClientTextureRegistry.MenuBackgroundTexture, new Size<int>(windowSize.Width, windowSize.Height)));

        UI.Add("logo", new Image(new Position(Anchor.TopRight), ClientTextureRegistry.LogoTexture, new Size<int>(250)));
        UI.Add("label", new Label(new Position(Anchor.CenterCenter), "label", 20));
        UI.Add("button", new Button(new Position(Anchor.CenterLeft), new Size<int>(200, 40), Translation.Lang.Get("gui.test")));
    }
}