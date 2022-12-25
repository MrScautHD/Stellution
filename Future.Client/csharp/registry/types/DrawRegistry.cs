using Future.Client.csharp.renderer;
using Future.Client.csharp.renderer.objects;
using Future.Client.csharp.renderer.overlay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.registry.types; 

public class DrawRegistry : ClientRegistry {

    // OBJECTS
    public static readonly StreetLightRenderer StreetLightRenderer = Register<StreetLightRenderer>("street_light", new StreetLightRenderer());
    
    // OVERLAY
    public static readonly CrosshairOverlay CrosshairOverlay = Register<CrosshairOverlay>("crosshair", new CrosshairOverlay());
    
    private static T Register<T>(string name, object renderer) {
        RegistryTypes.Renderers.Add(name, (DefaultRenderer) renderer);

        return (T) renderer;
    }
    
    public override void Initialize(GraphicsDevice graphicsDevice, GameWindow window) {
        foreach (var renderer in RegistryTypes.Renderers.Values) {
            renderer.Initialize(graphicsDevice, window);
        }
    }

    public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        foreach (var renderer in RegistryTypes.Renderers.Values) {
            renderer.LoadContent(graphicsDevice, content);
        }
    }

    public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime time) {
        foreach (var renderer in RegistryTypes.Renderers.Values) {
            renderer.Draw(graphicsDevice, spriteBatch, time);
        }
    }
}