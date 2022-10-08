using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace Mewlist.MassiveClouds
{
    [ExecuteInEditMode]
    public class MassiveCloudsCameraEffect : MonoBehaviour, IFullScreenDrawable
    {
        [SerializeField] private AbstractMassiveClouds massiveClouds = null;
        [SerializeField] private CameraEvent cameraEvent = CameraEvent.AfterSkybox;
        [SerializeField] private Light sun = null;
        [SerializeField] private Transform moon = null;

        private CommandBuffer        commandBuffer;
        private CameraEvent          currentCameraEvent = CameraEvent.AfterSkybox;

        private Camera TargetCamera { get { return GetComponent<Camera>(); } }
        private AbstractMassiveClouds MassiveClouds { get { return massiveClouds; } }

        private void Start()
        {
            if (!Application.isPlaying)
                DynamicGI.UpdateEnvironment();
        }

        private void SetupCamera()
        {
            currentCameraEvent = cameraEvent;
            TargetCamera.forceIntoRenderTexture = true;
            if ((TargetCamera.depthTextureMode & DepthTextureMode.Depth) == 0)
            {
                TargetCamera.depthTextureMode |= DepthTextureMode.Depth;
            }
        }

        private void Create()
        {
            Clear();
            SetupCamera();

            if (commandBuffer == null)
                commandBuffer = new CommandBuffer {name = "MassiveClouds"};
            TargetCamera.AddCommandBuffer(currentCameraEvent, commandBuffer);
        }

        private void OnPreRender()
        {
            if (commandBuffer == null) return;
            commandBuffer.Clear();
            if (!MassiveClouds) return;

            var ctx = CreateCameraEffectContext(commandBuffer, TargetCamera);

            MassiveClouds.UpdateClouds(sun, moon);
            commandBuffer.SetGlobalFloat("_MassiveCloudsProbeScale", 1f);
            commandBuffer.SetGlobalFloat("_SkyIntensity", 1.0f);
            MassiveClouds.BuildCommandBuffer(ctx, this);
        }

        private void Clear()
        {
            if (commandBuffer != null)
            {
                TargetCamera.RemoveCommandBuffer(currentCameraEvent, commandBuffer);
            }
            
            if (MassiveClouds) MassiveClouds.Clear();

            commandBuffer = null;
        }

        private void Update()
        {
            if (!MassiveClouds)
            {
                Clear();
                return;
            }
//            DynamicGI.UpdateEnvironment();

            if (commandBuffer == null) Create();
        }

        private void OnDisable()
        {
            Clear();
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

        private MassiveCloudsPassContext CreateCameraEffectContext(CommandBuffer cmd, Camera camera)
        {
            return new MassiveCloudsPassContext(
                cmd, camera, CreateRenderTextureDesc(camera), BuiltinRenderTextureType.CameraTarget);
        }

        public void Draw(CommandBuffer commandBuffer, RenderTargetIdentifier source)
        {
            CommandBufferUtility.BlitProcedural(commandBuffer, source, BuiltinRenderTextureType.CameraTarget);
        }
    }
}