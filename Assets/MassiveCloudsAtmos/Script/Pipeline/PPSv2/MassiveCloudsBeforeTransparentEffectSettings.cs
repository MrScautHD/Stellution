using System;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

#if UNITY_POST_PROCESSING_STACK_V2
namespace Mewlist.MassiveClouds
{
    [Serializable]
    [PostProcess(typeof(MassiveCloudsBeforeTransparentEffectRenderer),
        PostProcessEvent.BeforeTransparent,
        "Mewlist/MassiveCloudsBeforeTransparent")]
    public sealed class MassiveCloudsBeforeTransparentEffectSettings : AbstractMassiveCloudsEffectSettings
    {
    }
}
#endif