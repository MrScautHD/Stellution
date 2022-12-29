using Future.Client.csharp.renderer;
using Future.Client.csharp.renderer.objects;
using Future.Client.csharp.renderer.overlay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.registry.types; 

public class DrawRegistry : IClientRegistry {

    // OBJECTS
    public readonly StreetLightRenderer StreetLightRenderer = Register("street_light", new StreetLightRenderer());
    
    // OVERLAY
    public readonly CrosshairOverlay CrosshairOverlay = Register("crosshair", new CrosshairOverlay());

    private static T Register<T>(string name, T renderer) where T : IRenderer {
        RegistryTypes.Renderers.Add(name, renderer);
        
        return renderer;
    }
    
    public void Initialize(GraphicsDevice graphicsDevice, GameWindow window) {
        foreach (IRenderer renderer in RegistryTypes.Renderers.Values) {
            renderer.Initialize(graphicsDevice, window);
        }
    }

    public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        foreach (IRenderer renderer in RegistryTypes.Renderers.Values) {
            renderer.LoadContent(graphicsDevice, content);
        }
    }

    public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime time) {
        foreach (IRenderer renderer in RegistryTypes.Renderers.Values) {
            renderer.Draw(graphicsDevice, spriteBatch, time);
        }
    }
}