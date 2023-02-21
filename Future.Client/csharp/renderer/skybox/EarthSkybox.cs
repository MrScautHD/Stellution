using Easel.Graphics;

namespace Future.Client.csharp.renderer.skybox; 

public class EarthSkybox : Skybox {
    
    public EarthSkybox(Bitmap right, Bitmap left, Bitmap top, Bitmap bottom, Bitmap front, Bitmap back, SamplerState samplerState = null) : base(right, left, top, bottom, front, back, samplerState) {
        
    }
}