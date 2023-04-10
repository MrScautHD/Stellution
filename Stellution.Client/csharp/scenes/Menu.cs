using Easel.Core;
using Easel.Graphics;
using Easel.GUI;
using Easel.Math;
using Stellution.Client.csharp.registry;
using Stellution.Client.csharp.translation;
using Stellution.Client.csharp.ui.elements;
using Stellution.Common.csharp.scenes;

namespace Stellution.Client.csharp.scenes;

public class Menu : ModifiedScene {

    public Menu() : base("menu") {
        
    }

    protected override void Initialize() {
        base.Initialize();

        UI.DefaultStyle.Font = FontRegistry.Fontoe;

        UI.Add(new ImageElement("image", TextureRegistry.Female, new Position(Anchor.CenterCenter), new Size<int>(500)));
        UI.Add(new ButtonElement("button", Texture2D.Missing, "BUTTON", 160, new Position(Anchor.CenterLeft), new Size<int>(400), true, null, Color.Aqua, 
            () => {
                Logger.Error("CLICKED");
                return true;
            }));
        
        UI.Add(new LabelElement("label", Translation.Lang.Get("gui.button.singleplayer"), new Position(Anchor.CenterRight), 60, true, Color.Aqua));
    }
}