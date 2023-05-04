using Easel.Graphics;
using Easel.GUI;
using Easel.Math;
using Stellution.Client.csharp.registry.types;
using Stellution.Client.csharp.translation;
using Stellution.Client.csharp.ui.elements;
using Stellution.Common.csharp.scenes;

namespace Stellution.Client.csharp.scenes;

public class Menu : ModifiedScene {

    public Menu() : base("menu") {
        
    }

    protected override void Initialize() {
        base.Initialize();

        UI.DefaultStyle.Font = FontRegistry.Fontoe.Value;
        UI.DefaultStyle.BackgroundTexture = TextureRegistry.CarBanner.Value;
        
        UI.Add(new ImageElement("car_banner", TextureRegistry.CarBanner.Value, new Position(Anchor.CenterCenter)));
        UI.Add(new ImageElement("black_shadow", Texture2D.Black, new Position(Anchor.CenterCenter), new Size<int>(1920, 1080), new Color(Color.Black, 200)));

        UI.Add(new ImageElement("banner", TextureRegistry.LogoBanner.Value, new Position(Anchor.CenterCenter)));
        
        
        UI.Add(new ButtonElement("button", Texture2D.Missing, "BUTTON", 160, new Position(Anchor.CenterLeft), new Size<int>(200, 50), true, null, Color.Aqua, 
            () => {
                StellutionClient.NetworkManager.Connect("127.0.0.1:7777");
                return true;
            }));
        
        UI.Add(new LabelElement("label", Translation.Lang.Get("gui.button.singleplayer"), new Position(Anchor.CenterRight), 60, true, Color.Aqua));
    }
}