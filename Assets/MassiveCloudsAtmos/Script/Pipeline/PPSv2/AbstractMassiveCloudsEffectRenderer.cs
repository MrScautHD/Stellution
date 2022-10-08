using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
using UnityEngine.Rendering;
using UnityEngine.XR;


#if UNITY_POST_PROCESSING_STACK_V2
namespace Mewlist.MassiveClouds
{
    public class AbstractMassiveCloudsEffectRenderer<T> : PostProcessEffectRenderer<T>, IFullScreenDrawable
        where T : AbstractMassiveCloudsEffectSettings
    {
        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }

        private DynamicRenderTexture screenTexture;
        private PostProcessRenderContext currentContext;

        private void OnPreRender()
        {
        }

        public override void Release()
        {
            var massiveCloudsRenderer = settings.rendererParameter.value;
            if (massiveCloudsRenderer)
                massiveCloudsRenderer.Clear();

            if (screenTexture != null)
                screenTexture.Dispose();
            screenTexture = null;

            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var massiveCloudsRenderer = settings.rendererParameter.value;
            if (!massiveCloudsRenderer) return;

            var commandBuffer = context.command;
            if (commandBuffer == null) return;

            if (screenTexture == null)
                screenTexture = new DynamicRenderTexture(context.sourceFormat);
           
            currentContext = context;
            var rtDesc = new RenderTextureDesc();
            screenTexture.Update(context.camera, CreateRenderTextureDesc(context.camera));

            var sunLightSource = Object.FindObjectOfType<MassiveCloudsSunLightSource>();
            massiveCloudsRenderer.UpdateClouds(sunLightSource ? sunLightSource.Light : null, null);
            commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, screenTexture.GetRenderTexture(context.camera));
            commandBuffer.SetGlobalFloat("_MassiveCloudsProbeScale", 1f);
            commandBuffer.SetGlobalFloat("_SkyIntensity", 1.0f);

            var ctx = CreateCameraEffectContext(commandBuffer, context.camera, screenTexture.GetRenderTexture(context.camera));
            massiveCloudsRenderer.BuildCommandBuffer(ctx, this);
            currentContext = null;
        }

        private RenderTextureDesc CreateRenderTextureDesc(Camera camera)
        {
            if (XRSettings.enabled)
            {
                // Single Pass : double wide width
                // Multi Pass : single width
                var w = XRSettings.eyeTextureDesc.width;
                var h = XRSettings.eyeTextureDesc.height;
                return new RenderTextureDesc(w, h, XRSettings.eyeTextureDesc.vrUsage);
            }
            else
            {
                return new RenderTextureDesc(camera.pixelWidth, camera.pixelHeight);
            }
        }

        private MassiveCloudsPassContext CreateCameraEffectContext(CommandBuffer cmd, Camera camera,
            RenderTexture destination)
        {
            return new MassiveCloudsPassContext(
                cmd, camera, CreateRenderTextureDesc(camera), destination);
        }

        public void Draw(CommandBuffer commandBuffer, RenderTargetIdentifier source)
        {
            CommandBufferUtility.BlitProcedural(commandBuffer, source, currentContext.destination);
        }
    }
}
#endif