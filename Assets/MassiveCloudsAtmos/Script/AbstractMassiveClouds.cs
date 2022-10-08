using UnityEngine;
using UnityEngine.Rendering;

namespace Mewlist.MassiveClouds
{
    public interface IFullScreenDrawable
    {
        void Draw(CommandBuffer commandBuffer, RenderTargetIdentifier source);
    }

    public abstract class AbstractMassiveClouds : ScriptableObject
    {
        [SerializeField] protected float sunIntensityScale = 1f;
        [Range(-1f, 1f), SerializeField] protected float cloudIntensityAdjustment = 1f;
        [SerializeField] protected Sun sun = new Sun();
        [SerializeField] protected Moon moon = new Moon();
        [SerializeField] protected MassiveCloudsAmbient ambient = new MassiveCloudsAmbient();
        [SerializeField] protected SkyPass skyPass = new SkyPass();

        public MassiveCloudsLight Sun { get { return sun; } }
        public MassiveCloudsLight Moon { get { return moon; } }
        public float SunIntensityScale { get { return sunIntensityScale; } }
        public float CloudIntensityAdjustment { get { return cloudIntensityAdjustment; } }
        public MassiveCloudsAmbient Ambient { get { return ambient; } }
        public SkyPass SkyPass { get { return skyPass; } }

        public abstract void BuildCommandBuffer(MassiveCloudsPassContext ctx, IFullScreenDrawable fullScreenDrawer);

        protected void UpdateLightSources(Light light, Transform moonTransform)
        {
            if (sun.Mode == MassiveCloudsLight.LightSourceMode.Auto)
            {
                sun.SetReference(light);
                sun.Synchronize(sun.Reference);
            }

            if (moon.Mode != MassiveCloudsLight.LightSourceMode.Manual)
            {
                moon.SetReference(moonTransform);
                moon.Synchronize(moon.Reference);
            }

            if (ambient.Mode == MassiveCloudsAmbient.AmbientMode.Auto)
            {
                ambient.Collect(sun, moon);
            }
        }

        public void SetMoon(float intensity, Color color)
        {
            moon.SetIntensity(intensity);
            moon.SetColor(color);
        }

        public abstract void UpdateClouds(Light sun, Transform moon);
        public abstract void Clear();
    }
}