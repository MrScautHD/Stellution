#if ENVIRO_HDRP
using System;

namespace UnityEngine.Rendering.HighDefinition
{
    [VolumeComponentMenu("Sky/Enviro Skybox")]
    [SkyUniqueID(999)]
    public class EnviroSkybox : SkySettings
    {
  
        /* public override SkyRenderer CreateRenderer()
         {
             return new EnviroSkyboxRenderer(this);
         }
         */

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();

            unchecked
            {
               // hash = hash * 23 + GetHashCode();
            }

            return hash;
        }

        public override Type GetSkyRendererType() { return typeof(EnviroSkyboxRenderer); }
    }
}
#endif