using Easel;
using Easel.Content;
using Easel.Content.Builder;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Easel.GUI;
using Future.Client.csharp.config;
using Future.Client.csharp.events;
using Future.Client.csharp.translation;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry;

public class ClientRegistry : IRegistry {
    
    public static ContentManager Content => EaselGame.Instance.Content;

    // FONTS
    public static Font FontoeFont => Content.Load<Font>("font/fontoe.ttf");

    // BITMAPS
    public static Bitmap SkyEarthTop => Content.Load<Bitmap>("textures/sky/skybox/earth/earth_top.bmp");
    public static Bitmap SkyEarthSide => Content.Load<Bitmap>("textures/sky/earth/skybox/earth_side.bmp");
    public static Bitmap SkyEarthBottom => Content.Load<Bitmap>("textures/sky/earth/skybox/earth_bottom.bmp");
    
    // TEXTURES
    public static Texture2D CyberCarTexture => TextureGetter("textures/entity/vehicle/cyber_car.png");
    
    // MODELS
    public static Model CyberCarModel => ModelGetter("models/entity/vehicle/cyber_car.glb", CyberCarMaterial);
    
    // MATERIALS
    public static TranslucentStandardMaterial CyberCarMaterial { get; private set; }

    // SKYBOXES
    public static Skybox EarthSkybox { get; private set; }
    
    // CONFIGS
    public static GraphicConfig GraphicConfig { get; private set; }
    
    // TRANSLATIONS
    public static Translation EnglishTranslation { get; private set; }

    public void Initialize(ContentManager content) {
        ContentDefinition definition = new ContentBuilder("content")
            
            // FONTS
            .Add(new FontContent("font/fontoe.ttf"))
            
            // MODELS
            .Add(new ModelContent("models/entity/vehicle/cyber_car.glb", false))
            
            // BITMAPS
            .Add(new ImageContent("textures/sky/skybox/earth/earth_top.bmp"))
            .Add(new ImageContent("textures/sky/skybox/earth/earth_side.bmp"))
            .Add(new ImageContent("textures/sky/skybox/earth/earth_bottom.bmp"))
            
            // TEXTURES
            .Add(new ImageContent("textures/entity/vehicle/cyber_car.png"))
            
            .Build();

        content.AddContent(definition);

        // MATERIALS & TEXTURE STATES
        CyberCarTexture.SamplerState = SamplerState.PointClamp;
        CyberCarMaterial = new TranslucentStandardMaterial(CyberCarTexture);

        // SKYBOXES
        EarthSkybox = new Skybox(SkyEarthSide, SkyEarthSide, SkyEarthTop, SkyEarthBottom, SkyEarthSide, SkyEarthSide);

        // CONFIGS
        GraphicConfig = new GraphicConfig("config", "graphic-config");
        
        // TRANSLATIONS
        EnglishTranslation = new Translation("content", "english");
        
        // EVENTS
        EntityConstructorEvent entityConstructorEvent = new EntityConstructorEvent();
        SceneEvent sceneEvent = new SceneEvent();
    }

    /**
     * Use this to get the "TEXTURES" with the right "Sample-State".
     */
    public static Texture2D TextureGetter(string path, SamplerState state = null) {
        Texture2D texture = Content.Load<Texture2D>(path);
        texture.SamplerState = state ?? SamplerState.PointClamp;

        return texture;
    }

    /**
     * Use this to get the "MODEL" with the right Material.
     */
    protected static Model ModelGetter(string path, Material material) {
        Model model = Content.Load<Model>(path);
        
        for (int i = 0; i < model.Materials.Length; i++) {
            ref Material modelMaterial = ref model.Materials[i];

            modelMaterial = material;
        }
        
        return model;
    }
}