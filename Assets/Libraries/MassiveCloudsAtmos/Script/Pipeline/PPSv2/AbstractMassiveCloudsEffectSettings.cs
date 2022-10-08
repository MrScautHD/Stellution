using System;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif


#if UNITY_POST_PROCESSING_STACK_V2
namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class AbstractMassiveCloudsEffectSettings : PostProcessEffectSettings
    {
        public MassiveCloudsRendererParameter rendererParameter = new MassiveCloudsRendererParameter();

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value;
        }
    }
}
#endif