using System;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Mewlist.MassiveClouds
{
#if UNITY_POST_PROCESSING_STACK_V2
    [Serializable]
    public sealed class MassiveCloudsRendererParameter : ParameterOverride<AbstractMassiveClouds>
    {
    }

#endif
}