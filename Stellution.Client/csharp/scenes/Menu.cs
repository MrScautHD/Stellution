using Easel.GUI;
using Easel.Math;
using Stellution.Client.csharp.registry;
using Stellution.Client.csharp.ui.elements;
using Stellution.Common.csharp.scenes;

namespace Stellution.Client.csharp.scenes;

public class Menu : ModifiedScene {

    public Menu() : base("menu") {
        
    }

    protected override void Initialize() {
        base.Initialize();

        UI.DefaultStyle.Font = FontRegistry.Fontoe;

        UI.Add(new Panel("test", new Position(Anchor.CenterCenter), new Size<int>(90)));
        UI.Add(new ImageElement("image", TextureRegistry.Female, new Position(Anchor.CenterCenter), new Size<int>(500)));
        UI.Add(new LabelElement("label", "IT WORKS!", new Position(Anchor.BottomCenter), 50, true, Color.Aqua));
        
        //UI.Add("logo", new Image(new Position(Anchor.TopRight), ClientTextureRegistry.Get(ClientTextureRegistry.LogoTexture), new Size<int>(250)));
        //UI.Add("label", new Label(new Position(Anchor.CenterCenter), "label", 20));

        //UI.Add("button", new Button(new Position(Anchor.CenterCenter), new Size<int>(200, 40), Translation.Lang.Get("gui.button.singleplayer")));
    }
}