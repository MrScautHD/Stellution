#if ENVIRO_HDRP
using System;

namespace UnityEngine.Rendering.HighDefinition
{
    [VolumeComponentMenu("Sky/Enviro Lite Skybox")]
    [SkyUniqueID(998)] 
    public class EnviroSkyboxLite : SkySettings
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
                //hash = hash * 23 + top.GetHashCode();
            }

            return hash;
        }

        public override Type GetSkyRendererType() { return typeof(EnviroSkyboxLiteRenderer); }
    }
}
#endif