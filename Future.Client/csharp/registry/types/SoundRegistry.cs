using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.registry.types; 

public class SoundRegistry : IClientRegistry {
    
    public static SoundEffectInstance Future { get; private set; }

    public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        Future = this.Register(content, "future");
    }
    
    private SoundEffectInstance Register(ContentManager content, string name) {
        SoundEffect soundEffect = content.Load<SoundEffect>("sounds/" + name);

        RegistryTypes.Sounds.Add(name, soundEffect);
        return soundEffect.CreateInstance();
    }
}