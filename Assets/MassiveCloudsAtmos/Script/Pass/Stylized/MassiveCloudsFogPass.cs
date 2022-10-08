using System;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class MassiveCloudsFogPass : IMassiveCloudsPass<MassiveCloudsStylizedCloud>
    {
        [SerializeField] private MassiveCloudsColorMode fogColorMode = MassiveCloudsColorMode.Ambient;
        [SerializeField] private Color fogColor = new Color32(200, 200, 230, 255);
        [SerializeField] private float fogLuminanceFix = 0f;

        // Height Fog
        [SerializeField] private bool heightFog = false;
        [Range(-10000f, 10000f), SerializeField] private float groundHeight = 0f;
        [Range(0f, 10000f), SerializeField] private float heightFogFromDistance = 10000f;
        [Range(0.001f, 10000f), SerializeField] private float heightFogRange = 500f;
        [Range(0.001f, 10000f), SerializeField] private float farHeightFogRange = 1000f;
        [Range(0f, 1f), SerializeField] private float heightFogDensity = 0.2f;

        private Material heightFogMaterial;
        private Material HeightFogMaterial
        {
            get
            {
                if (heightFogMaterial == null) heightFogMaterial = new Material(Shader.Find("MassiveCloudsHeightFog"));
                return heightFogMaterial;
            }
        }

        public MassiveCloudsColorMode FogColorMode { get { return fogColorMode; } }

        public void Update(MassiveCloudsStylizedCloud context)
        {
            HeightFogMaterial.SetFloat("_GroundHeight", groundHeight);
            HeightFogMaterial.SetFloat("_HeightFogFromDistance", heightFogFromDistance);
            HeightFogMaterial.SetFloat("_HeightFogRange", heightFogRange);
            HeightFogMaterial.SetFloat("_FarHeightFogRange", farHeightFogRange);
            HeightFogMaterial.SetFloat("_HeightFogDensity", heightFogDensity);

            if (heightFog) HeightFogMaterial.EnableKeyword("_HEIGHTFOG_ON");
            else HeightFogMaterial.DisableKeyword("_HEIGHTFOG_ON");
            HeightFogMaterial.SetFloat("_HEIGHTFOG", heightFog ? 1f : 0f);

            HeightFogMaterial.SetColor("_MCFogColor", GetFogColor(context));
            HeightFogMaterial.SetColor("_FogColorTop", GetFogColor(context));

            context.Sun.ApplySunParameters(HeightFogMaterial, context.SunIntensityScale);
        }

        public void BuildCommandBuffer(MassiveCloudsStylizedCloud context, MassiveCloudsPassContext ctx,
            FlippingRenderTextures renderTextures)
        {
            if (!heightFog) return;
            ctx.cmd.Blit(renderTextures.From, renderTextures.To, HeightFogMaterial);
            renderTextures.Flip();
        }

        public void Clear()
        {
            if (Application.isPlaying)
                Object.Destroy(heightFogMaterial);
            else
                Object.DestroyImmediate(heightFogMaterial);
            heightFogMaterial = null;
        }

        private Color GetFogColor(MassiveCloudsStylizedCloud context)
        {
            switch (fogColorMode)
            {
                case MassiveCloudsColorMode.FogColor: return RenderSettings.fogColor;
                case MassiveCloudsColorMode.Ambient:
                {
                    var factor = Mathf.Pow(2, -fogLuminanceFix);
                    return context.Ambient.EquatorColor / factor;
                }
                case MassiveCloudsColorMode.Constant:
                default: return fogColor;
            }
        }
    }
}