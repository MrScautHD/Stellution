using System.Collections.Generic;
using FontStashSharp;
using Future.Client.csharp.renderer;
using Future.Client.csharp.ticker;
using Future.Common.csharp.file;
using Microsoft.Xna.Framework.Audio;

namespace Future.Client.csharp.registry; 

public class ClientRegistryTypes {
    public static readonly Dictionary<string, IRenderer> Renderers = new();
    public static readonly Dictionary<string, DynamicSpriteFont> Fonts = new();
    public static readonly Dictionary<string, SoundEffectInstance> Sounds = new();
    public static readonly Dictionary<string, IClientTicker> Ticker = new();
    public static readonly Dictionary<string, AbstractConfig> Configs = new();
}