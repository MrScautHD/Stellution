using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System;


public class EnviroHaltonSequence
{
    public int radix = 3;
    private int storedIndex = 0;
    public float Get()
    {
        float result = 0f;
        float fraction = 1f / (float)radix;
        int index = storedIndex;
        while (index > 0)
        {
            result += (float)(index % radix) * fraction;

            index /= radix;
            fraction /= (float)radix;
        }
        storedIndex++;
        return result;
    }
}


[Serializable]
public class EnviroCustomRenderingSettings
{
    [Header("Feature Control")]
    public bool useVolumeClouds = true;
    public bool useVolumeLighting = true;
    public bool useDistanceBlur = true;
    public bool useFog = true;

    public EnviroVolumeCloudsQuality customCloudsQuality;
}

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[RequireComponent(typeof(Camera))]
public class EnviroSkyRendering : MonoBehaviour
{
#if ENVIRO_HD
    #region General Var
    [HideInInspector]
    public bool aboveClouds = false;
    [HideInInspector]
    public bool isAddionalCamera = false;
    private Camera myCam;
    private RenderTexture spSatTex;
    private Camera spSatCam;
    #endregion 
    ////////////////////////
    #region CustomRenderSettings
    public bool useGlobalRenderingSettings = true;
    public EnviroCustomRenderingSettings customRenderingSettings = new EnviroCustomRenderingSettings();
    private bool useVolumeClouds = true;
    private bool useVolumeLighting = true;
    private bool useDistanceBlur = true;
    private bool useFog = true;
    #endregion
    ////////////////////////
    #region Fog Var
    public enum FogType
    {
        Disabled,
        Simple,
        Standard
    }
    [HideInInspector]
    public FogType currentFogType;

    #endregion
    ////////////////////////
    #region Volume Clouds Var
    ////////////////////// Clouds //////////////////////
    private EnviroHaltonSequence sequence = new EnviroHaltonSequence() { radix = 3 };
    //
    private Material cloudsMat;
    private Material blitMat;
    private Material compose;
    private Material downsample;
    private RenderTexture subFrameTex;
    private RenderTexture prevFrameTex;
    private Matrix4x4 projection;
    private Matrix4x4 projectionSPVR;
    private Matrix4x4 inverseRotation;
    private Matrix4x4 inverseRotationSPVR;
    private Matrix4x4 rotation;
    private Matrix4x4 rotationSPVR;
    private Matrix4x4 previousRotation;
    private Matrix4x4 previousRotationSPVR;
    [HideInInspector]
    public EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize currentReprojectionPixelSize;
    private int reprojectionPixelSize;
    private bool isFirstFrame;
    private int subFrameNumber;
    private int[] frameList;
    private int renderingCounter;
    private int subFrameWidth;
    private int subFrameHeight;
    private int frameWidth;
    private int frameHeight;
    private bool textureDimensionChanged;
    private EnviroVolumeCloudsQualitySettings usedCloudsQuality;
    #endregion
    ////////////////////////
    #region Volume Fog Var
    ///////////////// Volume Lighting //////////////////////
    public enum VolumtericResolution
    {
        Full,
        Half,
        Quarter
    };
    public static event Action<EnviroSkyRendering, Matrix4x4, Matrix4x4> PreRenderEvent;
    private static Mesh _pointLightMesh;
    private static Mesh _spotLightMesh;
    private static Material _lightMaterial;
    private CommandBuffer _preLightPass;
    public CommandBuffer _afterLightPass;
    private Matrix4x4 _viewProj;
    private Matrix4x4 _viewProjSP;
    [HideInInspector]
    public Material fogMat;
    private Material _bilateralBlurMaterial;
    private RenderTexture _volumeLightTexture;
    private RenderTexture _halfVolumeLightTexture;
    private RenderTexture _quarterVolumeLightTexture;
    private static Texture _defaultSpotCookie;
    private RenderTexture _halfDepthBuffer;
    private RenderTexture _quarterDepthBuffer;
    private VolumtericResolution currentVolumeRes;
    [HideInInspector]
    public Texture2D _ditheringTexture;
    private Texture2D blackTexture;
    [HideInInspector]
    public Texture DefaultSpotCookie;
    [HideInInspector]
    public Material volumeLightMat;
    public CommandBuffer GlobalCommandBuffer { get { return _preLightPass; } }
    public CommandBuffer GlobalCommandBufferForward { get { return _afterLightPass; } }
    public static Material GetLightMaterial()
    {
        return _lightMaterial;
    }
    public static Mesh GetPointLightMesh()
    {
        return _pointLightMesh;
    }
    public static Mesh GetSpotLightMesh()
    {
        return _spotLightMesh;
    }
    public RenderTexture GetVolumeLightBuffer()
    {
#if ENVIRO_HD
        if (EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Quarter)
            return _quarterVolumeLightTexture;
        else if (EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Half)
            return _halfVolumeLightTexture;
        else
            return _volumeLightTexture;
#else
        return null;
#endif
    }
    public RenderTexture GetVolumeLightDepthBuffer()
    {
#if ENVIRO_HD
        if (EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Quarter)
            return _quarterDepthBuffer;
        else if (EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Half)
            return _halfDepthBuffer;
        else
            return null;
#else
        return null;
#endif
    }
    public static Texture GetDefaultSpotCookie()
    {
        return _defaultSpotCookie;
    }
    #endregion
    ////////////////////////
    #region Blur Var
    /////////////////// Blur //////////////////////
    private Material postProcessMat;
    private const int kMaxIterations = 16;
    private RenderTexture[] _blurBuffer1 = new RenderTexture[kMaxIterations];
    private RenderTexture[] _blurBuffer2 = new RenderTexture[kMaxIterations];
    // private float _threshold = 0f;
    public float thresholdGamma
    {
        get { return Mathf.Max(0f, 0); }
        // set { _threshold = value; }
    }
    public float thresholdLinear
    {
        get { return Mathf.GammaToLinearSpace(thresholdGamma); }
        //   set { _threshold = Mathf.LinearToGammaSpace(value); }
    }
    #endregion
    ////////////////////////
    #region Enable and Disable
    void OnEnable()
    {

#if ENVIRO_LWRP || ENVIRO_HDRP
        this.enabled = false;
        return;
#else
        if (myCam == null)
            myCam = GetComponent<Camera>();

        CreateMaterialsAndTextures();
        SetupVolumeFog();
        CreateCommandBuffer();
        CreateFogMaterial();

        if (EnviroSky.instance == null)
        {
            return;
        }

        UpdateQualitySettings();

        if (EnviroSky.instance != null)
            SetReprojectionPixelSize(usedCloudsQuality.reprojectionPixelSize);

        //Workaround to disble volume lighting in sky baking...
        if (isAddionalCamera && !useGlobalRenderingSettings)
        {
            if (!customRenderingSettings.useVolumeLighting && fogMat != null)
            {
                fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
                fogMat.SetTexture("_EnviroVolumeLightingTex", blackTexture);
            }

        }
#endif
    }

    void OnDisable()
    {

#if ENVIRO_LWRP || ENVIRO_HDRP
        CleanupMaterials();
        return;
#else
        RemoveCommandBuffer();
        CleanupMaterials();
#endif
    }
    #endregion
    ////////////////////////
    #region Setup
    private void CleanupMaterials()
    {
        if (postProcessMat != null)
            DestroyImmediate(postProcessMat);

        if (volumeLightMat != null)
            DestroyImmediate(volumeLightMat);

        if (_bilateralBlurMaterial != null)
            DestroyImmediate(_bilateralBlurMaterial);

        if (cloudsMat != null)
            DestroyImmediate(cloudsMat);

        if (fogMat != null)
            DestroyImmediate(fogMat);

        if (blitMat != null)
            DestroyImmediate(blitMat);

        if (compose != null)
            DestroyImmediate(compose);

        if (downsample != null)
            DestroyImmediate(downsample);
    }

    private void CreateCommandBuffer()
    {
        _preLightPass = new CommandBuffer();
        _preLightPass.name = "PreLight";

        _afterLightPass = new CommandBuffer();
        _afterLightPass.name = "AfterLight";

        if (myCam.actualRenderingPath == RenderingPath.Forward)
        {
            myCam.AddCommandBuffer(CameraEvent.AfterDepthTexture, _preLightPass);
            myCam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _afterLightPass);
        }
        else
        {
            myCam.AddCommandBuffer(CameraEvent.BeforeLighting, _preLightPass);
        }
    }

    private void RemoveCommandBuffer()
    {
        if (myCam.actualRenderingPath == RenderingPath.Forward)
        {
            if (_preLightPass != null)
                myCam.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, _preLightPass);
            if (_afterLightPass != null)
                myCam.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, _afterLightPass);
        }
        else
        {
            if (_preLightPass != null)
                myCam.RemoveCommandBuffer(CameraEvent.BeforeLighting, _preLightPass);
        }
    }

    private void SetupVolumeFog()
    {
        if (EnviroSky.instance == null)
            return;

        currentVolumeRes = EnviroSky.instance.volumeLightSettings.Resolution;
        //ChangeResolution();

        if (_pointLightMesh == null)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _pointLightMesh = go.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(go);
        }

        if (_spotLightMesh == null)
        {
            _spotLightMesh = CreateSpotLightMesh();
        }

        if (_lightMaterial == null)
        {
            Shader shaderLight = Shader.Find("Enviro/Standard/VolumeLight");
            if (shaderLight == null)
                throw new Exception("Critical Error: \"Enviro/VolumeLight\" shader is missing.");
            _lightMaterial = new Material(shaderLight);
        }

        if (_defaultSpotCookie == null)
        {
            _defaultSpotCookie = DefaultSpotCookie;
        }

        GenerateDitherTexture();
    }

    private void ChangeResolution(RenderTextureDescriptor descriptor)
    {
        int width = myCam.pixelWidth;
        int height = myCam.pixelHeight;

        if (width <= 0 || height <= 0)
            return;
        

        if (_volumeLightTexture != null)
        {
            _volumeLightTexture.Release();
            DestroyImmediate(_volumeLightTexture);
        }

        _volumeLightTexture = new RenderTexture(descriptor);
        _volumeLightTexture.name = "VolumeLightBuffer";

        //Half Res
        if (_halfDepthBuffer != null)
        {
            _halfDepthBuffer.Release();
            DestroyImmediate(_halfDepthBuffer);
        }
        if (_halfVolumeLightTexture != null)
        {
            _halfVolumeLightTexture.Release();
            DestroyImmediate(_halfVolumeLightTexture);
        }

        if (EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Half || EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Quarter)
        {
            RenderTextureDescriptor halfResDescriptor = descriptor;
            halfResDescriptor.width = halfResDescriptor.width / 2;
            halfResDescriptor.height = halfResDescriptor.height / 2;

            _halfVolumeLightTexture = new RenderTexture(halfResDescriptor);
            _halfVolumeLightTexture.name = "VolumeLightBufferHalf";

            RenderTextureDescriptor halfResDepthDescriptor = halfResDescriptor;
            halfResDepthDescriptor.colorFormat = RenderTextureFormat.RFloat;

            _halfDepthBuffer = new RenderTexture(halfResDepthDescriptor);
            _halfDepthBuffer.name = "VolumeLightHalfDepth";
            _halfDepthBuffer.Create();
            _halfDepthBuffer.filterMode = FilterMode.Point;

        }

        //Quarter Res
        if (_quarterVolumeLightTexture != null)
        {
            _quarterVolumeLightTexture.Release();
            DestroyImmediate(_quarterVolumeLightTexture);
        }
        if (_quarterDepthBuffer != null)
        {
            _quarterDepthBuffer.Release();
            DestroyImmediate(_quarterDepthBuffer);
        }

        if (EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Quarter)
        {
            RenderTextureDescriptor quarterResDescriptor = descriptor;
            quarterResDescriptor.width = quarterResDescriptor.width / 4;
            quarterResDescriptor.height = quarterResDescriptor.height / 4;
            _quarterVolumeLightTexture = new RenderTexture(quarterResDescriptor);
            _quarterVolumeLightTexture.name = "VolumeLightBufferQuarter";

            RenderTextureDescriptor quarterResDepthDescriptor = quarterResDescriptor;
            quarterResDepthDescriptor.colorFormat = RenderTextureFormat.RFloat;

            _quarterDepthBuffer = new RenderTexture(quarterResDepthDescriptor);
            _quarterDepthBuffer.name = "VolumeLightQuarterDepth";
            _quarterDepthBuffer.Create();
            _quarterDepthBuffer.filterMode = FilterMode.Point;
        }
    }

    private void CreateFogMaterial()
    {
        if (EnviroSky.instance == null)
            return;

        //Cleanup
        if (fogMat != null)
            DestroyImmediate(fogMat);

        if (!useFog)
        {
            Shader shader = Shader.Find("Enviro/Standard/EnviroFogRenderingDisabled");
            if (shader == null)
                throw new Exception("Critical Error: \"Enviro/EnviroFogRenderingDisabled\" shader is missing.");
            fogMat = new Material(shader);

            currentFogType = FogType.Disabled;
        }
        else if (!EnviroSky.instance.fogSettings.useSimpleFog)
        {
            Shader shader = Shader.Find("Enviro/Standard/EnviroFogRendering");
            if (shader == null)
                throw new Exception("Critical Error: \"Enviro/EnviroFogRendering\" shader is missing.");
            fogMat = new Material(shader);

            currentFogType = FogType.Standard;
        }
        else
        {
            Shader shader = Shader.Find("Enviro/Standard/EnviroFogRenderingSimple");
            if (shader == null)
                throw new Exception("Critical Error: \"Enviro/EnviroFogRenderingSimple\" shader is missing.");
            fogMat = new Material(shader);

            currentFogType = FogType.Simple;
        }
    }

    private void CreateMaterialsAndTextures()
    {
        if (cloudsMat == null)
            cloudsMat = new Material(Shader.Find("Enviro/Standard/RaymarchClouds"));

        if (blitMat == null)
            blitMat = new Material(Shader.Find("Enviro/Standard/Blit"));

        if (compose == null)
            compose = new Material(Shader.Find("Hidden/Enviro/Upsample"));

        if (downsample == null)
            downsample = new Material(Shader.Find("Hidden/Enviro/DepthDownsample"));

        if (volumeLightMat != null)
            volumeLightMat = new Material(Shader.Find("Enviro/Standard/VolumeLight"));

        if (postProcessMat != null)
            postProcessMat = new Material(Shader.Find("Hidden/EnviroDistanceBlur"));

        if (_bilateralBlurMaterial != null)
            _bilateralBlurMaterial = new Material(Shader.Find("Hidden/EnviroBilateralBlur"));

        if (blackTexture == null)
            blackTexture = Resources.Load("tex_enviro_black") as Texture2D;
    }
    #endregion
    ////////////////////////
    #region Rendering
    void OnPreRender()
    {
        if (EnviroSky.instance == null)
            return;

        //Volume Lighting
        if (useVolumeLighting)
        {
            if (_bilateralBlurMaterial == null)
                _bilateralBlurMaterial = new Material(Shader.Find("Hidden/EnviroBilateralBlur"));

            Matrix4x4 projLeft = Matrix4x4.Perspective(myCam.fieldOfView, myCam.aspect, 0.01f, myCam.farClipPlane);
            Matrix4x4 projRight = Matrix4x4.Perspective(myCam.fieldOfView, myCam.aspect, 0.01f, myCam.farClipPlane);

            if (myCam.stereoEnabled)
            {
                projLeft = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                projLeft = GL.GetGPUProjectionMatrix(projLeft, true);
                projRight = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                projRight = GL.GetGPUProjectionMatrix(projRight, true);
            }
            else
            {
                projLeft = Matrix4x4.Perspective(myCam.fieldOfView, myCam.aspect, 0.01f, myCam.farClipPlane);
                projLeft = GL.GetGPUProjectionMatrix(projLeft, true);
            }

            // use very low value for near clip plane to simplify cone/frustum intersection 
            if (myCam.stereoEnabled)
            {
                _viewProj = projLeft * myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
                _viewProjSP = projRight * myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
            }
            else
            {
                _viewProj = projLeft * myCam.worldToCameraMatrix;
                _viewProjSP = projRight * myCam.worldToCameraMatrix;
            }

            if (PreRenderEvent != null)
                PreRenderEvent(this, _viewProj, _viewProjSP);
        }


        ///////////////////Matrix Information
        if (myCam.stereoEnabled)
        {
            // Both stereo eye inverse view matrices
            Matrix4x4 left_world_from_view = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse;
            Matrix4x4 right_world_from_view = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;

            // Both stereo eye inverse projection matrices, plumbed through GetGPUProjectionMatrix to compensate for render texture
            Matrix4x4 left_screen_from_view = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            Matrix4x4 right_screen_from_view = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
            Matrix4x4 left_view_from_screen = GL.GetGPUProjectionMatrix(left_screen_from_view, true).inverse;
            Matrix4x4 right_view_from_screen = GL.GetGPUProjectionMatrix(right_screen_from_view, true).inverse;

            // Negate [1,1] to reflect Unity's CBuffer state
            if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
            {
                left_view_from_screen[1, 1] *= -1;
                right_view_from_screen[1, 1] *= -1;
            }

            Shader.SetGlobalMatrix("_LeftWorldFromView", left_world_from_view);
            Shader.SetGlobalMatrix("_RightWorldFromView", right_world_from_view);
            Shader.SetGlobalMatrix("_LeftViewFromScreen", left_view_from_screen);
            Shader.SetGlobalMatrix("_RightViewFromScreen", right_view_from_screen);
        }
        else
        {
            // Main eye inverse view matrix
            Matrix4x4 left_world_from_view = myCam.cameraToWorldMatrix;

            // Inverse projection matrices, plumbed through GetGPUProjectionMatrix to compensate for render texture
            Matrix4x4 screen_from_view = myCam.projectionMatrix;
            Matrix4x4 left_view_from_screen = GL.GetGPUProjectionMatrix(screen_from_view, true).inverse;

            // Negate [1,1] to reflect Unity's CBuffer state
            if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
                left_view_from_screen[1, 1] *= -1;

            // Store matrices
            Shader.SetGlobalMatrix("_LeftWorldFromView", left_world_from_view);
            Shader.SetGlobalMatrix("_LeftViewFromScreen", left_view_from_screen);
        }
        //////////////////////////////
    }

    private void UpdateQualitySettings()
    {
        if (useGlobalRenderingSettings)
        {
            useVolumeClouds = EnviroSky.instance.useVolumeClouds;
            useVolumeLighting = EnviroSky.instance.useVolumeLighting;
            useDistanceBlur = EnviroSky.instance.useDistanceBlur;
            useFog = EnviroSky.instance.useFog;
            usedCloudsQuality = EnviroSky.instance.cloudsSettings.cloudsQualitySettings;
        }
        else
        {
            useVolumeClouds = customRenderingSettings.useVolumeClouds;
            useVolumeLighting = customRenderingSettings.useVolumeLighting;
            useDistanceBlur = customRenderingSettings.useDistanceBlur;
            useFog = customRenderingSettings.useFog;
            if (customRenderingSettings.customCloudsQuality != null)
                usedCloudsQuality = customRenderingSettings.customCloudsQuality.qualitySettings;
            else
                usedCloudsQuality = EnviroSky.instance.cloudsSettings.cloudsQualitySettings;

        }
    }

    void Update()
    {
        if (EnviroSky.instance == null || myCam == null)
            return;

        UpdateQualitySettings();

        if (currentReprojectionPixelSize != usedCloudsQuality.reprojectionPixelSize)
        {
            currentReprojectionPixelSize = usedCloudsQuality.reprojectionPixelSize;
            SetReprojectionPixelSize(usedCloudsQuality.reprojectionPixelSize);
        }

        if (!useFog && currentFogType != FogType.Disabled || useFog && EnviroSky.instance.fogSettings.useSimpleFog && currentFogType != FogType.Simple || useFog && !EnviroSky.instance.fogSettings.useSimpleFog && currentFogType != FogType.Standard)
            CreateFogMaterial();
    }

    [ImageEffectOpaque]
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (EnviroSky.instance == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        /*       
        #if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
               if(UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
                {
                    Graphics.Blit(source, destination);
                    return;
                }
        #endif     
        */

        if (fogMat == null)
            CreateFogMaterial();

        // Set Camera to render depth in forward
        if (myCam.actualRenderingPath == RenderingPath.Forward)
            myCam.depthTextureMode |= DepthTextureMode.Depth;

        #region Renderlogic

        RenderTextureDescriptor desc = source.descriptor;
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore || SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
            desc.depthBufferBits = 0;

        if (useVolumeClouds && useVolumeLighting && useDistanceBlur)
        {
            RenderTexture tempTexture = RenderTexture.GetTemporary(desc);
            RenderTexture tempTexture2 = RenderTexture.GetTemporary(desc);

            //Clouds
            if (!Application.isPlaying && EnviroSky.instance.showVolumeCloudsInEditor || Application.isPlaying)
            {
                RenderVolumeClouds(source, tempTexture);
            }
            else
            {
                Graphics.Blit(source, tempTexture);
            }

            //Volume Lighting
            if (!Application.isPlaying && EnviroSky.instance.showVolumeLightingInEditor || Application.isPlaying)
            {
                RenderVolumeFog(tempTexture, tempTexture2);
            }
            else
            {
                RenderFog(tempTexture, tempTexture2);
                if (!isAddionalCamera)
                {
                    Shader.DisableKeyword("ENVIROVOLUMELIGHT");
                    Shader.SetGlobalTexture("_EnviroVolumeLightingTex", blackTexture);
                }
                else
                {
                    fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
                    fogMat.SetTexture("_EnviroVolumeLightingTex", blackTexture);
                }
            }

            //Distance Blur
            if (!Application.isPlaying && EnviroSky.instance.showDistanceBlurInEditor || Application.isPlaying)
            {
                RenderDistanceBlur(tempTexture2, destination);
            }
            else
            {
                Graphics.Blit(tempTexture2, destination);
            }

            RenderTexture.ReleaseTemporary(tempTexture);
            RenderTexture.ReleaseTemporary(tempTexture2);

        }
        else if (useVolumeClouds && !useVolumeLighting && useDistanceBlur)
        {
            RenderTexture tempTexture = RenderTexture.GetTemporary(desc);
            RenderTexture tempTexture2 = RenderTexture.GetTemporary(desc);

            //Clouds
            if (!Application.isPlaying && EnviroSky.instance.showVolumeCloudsInEditor || Application.isPlaying)
            {
                RenderVolumeClouds(source, tempTexture);
            }
            else
            {
                Graphics.Blit(source, tempTexture);
            }

            //Fog
            RenderFog(tempTexture, tempTexture2);
            if (!isAddionalCamera)
            {
                Shader.DisableKeyword("ENVIROVOLUMELIGHT");
                Shader.SetGlobalTexture("_EnviroVolumeLightingTex", blackTexture);
            }
            else
            {
                fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
                fogMat.SetTexture("_EnviroVolumeLightingTex", blackTexture);
            }

            //Distance Blur
            if (!Application.isPlaying && EnviroSky.instance.showDistanceBlurInEditor || Application.isPlaying)
            {
                RenderDistanceBlur(tempTexture2, destination);
            }
            else
            {
                Graphics.Blit(tempTexture2, destination);
            }

            RenderTexture.ReleaseTemporary(tempTexture);
            RenderTexture.ReleaseTemporary(tempTexture2);

        }
        else if (useVolumeClouds && useVolumeLighting && !useDistanceBlur)
        {
            RenderTexture tempTexture = RenderTexture.GetTemporary(desc);
            //RenderTexture tempTexture2 = RenderTexture.GetTemporary(desc);

            //Clouds
            if (!Application.isPlaying && EnviroSky.instance.showVolumeCloudsInEditor || Application.isPlaying)
            {
                RenderVolumeClouds(source, tempTexture);
            }
            else
            {
                Graphics.Blit(source, tempTexture);
            }

            //Volume Lighting
            if (!Application.isPlaying && EnviroSky.instance.showVolumeLightingInEditor || Application.isPlaying)
            {
                RenderVolumeFog(tempTexture, destination);
            }
            else
            {
                RenderFog(tempTexture, destination);
                if (!isAddionalCamera)
                {
                    Shader.DisableKeyword("ENVIROVOLUMELIGHT");
                    Shader.SetGlobalTexture("_EnviroVolumeLightingTex", blackTexture);
                }
                else
                {
                    fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
                    fogMat.SetTexture("_EnviroVolumeLightingTex", blackTexture);
                }
            }

            RenderTexture.ReleaseTemporary(tempTexture);

        }
        else if (useVolumeClouds && !useVolumeLighting && !useDistanceBlur)
        {
            if (!useFog)
            {
                if (!Application.isPlaying && EnviroSky.instance.showVolumeCloudsInEditor || Application.isPlaying)
                {
                    RenderVolumeClouds(source, destination);
                }
                else
                {
                    Graphics.Blit(source, destination);
                }
            }
            else
            {
                RenderTexture tempTexture = RenderTexture.GetTemporary(desc);

                if (!Application.isPlaying && EnviroSky.instance.showVolumeCloudsInEditor || Application.isPlaying)
                {
                    RenderVolumeClouds(source, tempTexture);
                }
                else
                {
                    Graphics.Blit(source, tempTexture);
                }

                RenderFog(tempTexture, destination);

                if (!isAddionalCamera)
                {
                    Shader.DisableKeyword("ENVIROVOLUMELIGHT");
                    Shader.SetGlobalTexture("_EnviroVolumeLightingTex", blackTexture);
                }
                else
                {
                    fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
                    fogMat.SetTexture("_EnviroVolumeLightingTex", blackTexture);
                }
                RenderTexture.ReleaseTemporary(tempTexture);
            }
        }
        else if (!useVolumeClouds && useVolumeLighting && useDistanceBlur)
        {
            RenderTexture tempTexture = RenderTexture.GetTemporary(desc);

            //Volume Lighting
            if (!Application.isPlaying && EnviroSky.instance.showVolumeLightingInEditor || Application.isPlaying)
            {
                RenderVolumeFog(source, tempTexture);
            }
            else
            {
                RenderFog(source, tempTexture);
                if (!isAddionalCamera)
                {
                    Shader.DisableKeyword("ENVIROVOLUMELIGHT");
                    Shader.SetGlobalTexture("_EnviroVolumeLightingTex", blackTexture);
                }
                else
                {
                    fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
                    fogMat.SetTexture("_EnviroVolumeLightingTex", blackTexture);
                }
            }

            //Distance Blur
            if (!Application.isPlaying && EnviroSky.instance.showDistanceBlurInEditor || Application.isPlaying)
            {
                RenderDistanceBlur(tempTexture, destination);
            }
            else
            {
                Graphics.Blit(tempTexture, destination);
            }

            RenderTexture.ReleaseTemporary(tempTexture);
        }
        else if (!useVolumeClouds && !useVolumeLighting && useDistanceBlur)
        {
            //Distance Blur
            if (!Application.isPlaying && EnviroSky.instance.showDistanceBlurInEditor || Application.isPlaying)
            {
                RenderDistanceBlur(source, destination);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
        else if (!useVolumeClouds && useVolumeLighting && !useDistanceBlur)
        {
            //Volume Lighting
            if (!Application.isPlaying && EnviroSky.instance.showVolumeLightingInEditor || Application.isPlaying)
            {
                RenderVolumeFog(source, destination);
            }
            else
            {
                RenderFog(source, destination);
                if (!isAddionalCamera)
                {
                    Shader.DisableKeyword("ENVIROVOLUMELIGHT");
                    Shader.SetGlobalTexture("_EnviroVolumeLightingTex", blackTexture);
                }
                else
                {
                    fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
                    fogMat.SetTexture("_EnviroVolumeLightingTex", blackTexture);
                }
            }
        }
        else
        {
            if (useFog)
            {
                RenderFog(source, destination);
                if (!isAddionalCamera)
                {
                    Shader.DisableKeyword("ENVIROVOLUMELIGHT");
                    Shader.SetGlobalTexture("_EnviroVolumeLightingTex", blackTexture);
                }
                else
                {
                    fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
                    fogMat.SetTexture("_EnviroVolumeLightingTex", blackTexture);
                }
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
        #endregion
    }


    private void RenderVolumeClouds(RenderTexture src, RenderTexture dst)
    {
        #region Clouds

        if (blitMat == null)
            blitMat = new Material(Shader.Find("Enviro/Standard/Blit"));

        StartFrame();

        if (subFrameTex == null || prevFrameTex == null || textureDimensionChanged)
            CreateCloudsRenderTextures(src);

        if (!isAddionalCamera)
            EnviroSky.instance.cloudsRenderTarget = subFrameTex;


        //Rendering Clouds
          RenderClouds(src, subFrameTex);

      
        if (isFirstFrame)
        {
            Graphics.Blit(subFrameTex, prevFrameTex);
            isFirstFrame = false;
        }

        //Set blending type:
        if (EnviroSky.instance.cloudsSettings.depthBlending)
            Shader.EnableKeyword("ENVIRO_DEPTHBLENDING");
        else
            Shader.DisableKeyword("ENVIRO_DEPTHBLENDING");

        int downsampling = EnviroSky.instance.cloudsSettings.bilateralUpsampling ? reprojectionPixelSize * usedCloudsQuality.cloudsRenderResolution : 1;

        if (downsampling > 1)
        {
            if (compose == null)
                compose = new Material(Shader.Find("Hidden/Enviro/Upsample"));

            if (downsample == null)
                downsample = new Material(Shader.Find("Hidden/Enviro/DepthDownsample"));


            RenderTexture lowDepth = DownsampleDepth(Screen.width, Screen.height, src, downsample, downsampling);

            compose.SetTexture("_CameraDepthLowRes", lowDepth);

            RenderTexture upsampledTex = RenderTexture.GetTemporary(myCam.pixelWidth / downsampling * 2, myCam.pixelHeight / downsampling * 2, 0, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Default);
            upsampledTex.filterMode = FilterMode.Bilinear;

            // composite to screen
            Vector2 pixelSize = new Vector2(1.0f / lowDepth.width, 1.0f / lowDepth.height);
            compose.SetVector("_LowResPixelSize", pixelSize);
            compose.SetVector("_LowResTextureSize", new Vector2(lowDepth.width, lowDepth.height));
            compose.SetFloat("_DepthMult", 32.0f);
            compose.SetFloat("_Threshold", 0.0005f);

            compose.SetTexture("_LowResTexture", subFrameTex);
            Graphics.Blit(subFrameTex, upsampledTex, compose);
            RenderTexture.ReleaseTemporary(lowDepth);

            //Blit clouds to final image
            blitMat.SetTexture("_MainTex", src);
            blitMat.SetTexture("_SubFrame", upsampledTex);
            blitMat.SetTexture("_PrevFrame", prevFrameTex);
            SetBlitmaterialProperties();

            Graphics.Blit(src, dst, blitMat);
            Graphics.Blit(upsampledTex, prevFrameTex);
            Graphics.SetRenderTarget(dst);
            RenderTexture.ReleaseTemporary(upsampledTex);
        }
        else
        {
            //Blit clouds to final image
            blitMat.SetTexture("_MainTex", src);
            blitMat.SetTexture("_SubFrame", subFrameTex);
            blitMat.SetTexture("_PrevFrame", prevFrameTex);
            SetBlitmaterialProperties();
            Graphics.Blit(src, dst, blitMat);
            Graphics.Blit(subFrameTex, prevFrameTex);
            Graphics.SetRenderTarget(dst);
        }
        
        FinalizeFrame();
  
        #endregion
    }

    private void RenderFog(RenderTexture src, RenderTexture dst)
    {
        //////////////// FOG
        float FdotC = myCam.transform.position.y - EnviroSky.instance.fogSettings.height;
        float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
        var sceneMode = RenderSettings.fogMode;
        var sceneDensity = RenderSettings.fogDensity;
        var sceneStart = RenderSettings.fogStartDistance;
        var sceneEnd = RenderSettings.fogEndDistance;
        Vector4 sceneParams;
        bool linear = (sceneMode == FogMode.Linear);
        float diff = linear ? sceneEnd - sceneStart : 0.0f;
        float invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;
        sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
        sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
        sceneParams.z = linear ? -invDiff : 0.0f;
        sceneParams.w = linear ? sceneEnd * invDiff : 0.0f;
        //////////////////

        if (!EnviroSky.instance.fogSettings.useSimpleFog)
        {
            Shader.SetGlobalVector("_FogNoiseVelocity", new Vector4(-EnviroSky.instance.Components.windZone.transform.forward.x * EnviroSky.instance.windIntensity * 5, -EnviroSky.instance.Components.windZone.transform.forward.z * EnviroSky.instance.windIntensity * 5) * EnviroSky.instance.fogSettings.noiseScale);
            Shader.SetGlobalVector("_FogNoiseData", new Vector4(EnviroSky.instance.fogSettings.noiseScale, EnviroSky.instance.fogSettings.noiseIntensity, EnviroSky.instance.fogSettings.noiseIntensityOffset));
            Shader.SetGlobalTexture("_FogNoiseTexture", EnviroSky.instance.ressources.detailNoiseTexture);
        }

        Shader.SetGlobalFloat("_EnviroVolumeDensity", EnviroSky.instance.globalVolumeLightIntensity);
        Shader.SetGlobalVector("_SceneFogParams", sceneParams);
        Shader.SetGlobalVector("_SceneFogMode", new Vector4((int)sceneMode, EnviroSky.instance.fogSettings.useRadialDistance ? 1 : 0, 0, Application.isPlaying ? 1f : 0f));
        Shader.SetGlobalVector("_HeightParams", new Vector4(EnviroSky.instance.fogSettings.height, FdotC, paramK, EnviroSky.instance.fogSettings.heightDensity * 0.5f));
        Shader.SetGlobalVector("_DistanceParams", new Vector4(-Mathf.Max(EnviroSky.instance.fogSettings.startDistance, 0.0f), 0, 0, 0));

        fogMat.SetFloat("_DitheringIntensity", EnviroSky.instance.fogSettings.fogDithering);

        fogMat.SetTexture("_MainTex", src);

        Graphics.Blit(src, dst, fogMat);
    }

    private void RenderVolumeFog(RenderTexture src, RenderTexture dst)
    {
        #region Volume Fog

        if (volumeLightMat == null)
            volumeLightMat = new Material(Shader.Find("Enviro/Standard/VolumeLight"));


        if (currentVolumeRes != EnviroSky.instance.volumeLightSettings.Resolution)
        {
            ChangeResolution(src.descriptor);
            currentVolumeRes = EnviroSky.instance.volumeLightSettings.Resolution;
        }

        if (_volumeLightTexture == null || (_halfVolumeLightTexture == null && EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Half) || (_quarterVolumeLightTexture == null && EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Quarter))
            ChangeResolution(src.descriptor);

        if (_volumeLightTexture != null && (_volumeLightTexture.width != myCam.pixelWidth || _volumeLightTexture.height != myCam.pixelHeight))
            ChangeResolution(src.descriptor);


        if (_preLightPass != null)
            _preLightPass.Clear();

        if (_afterLightPass != null)
            _afterLightPass.Clear();

        bool dx11 = SystemInfo.graphicsShaderLevel > 40;

        if (EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Quarter)
        {
            //Texture nullTexture = null;
            // down sample depth to half res
            _preLightPass.Blit(src, _halfDepthBuffer, _bilateralBlurMaterial, dx11 ? 4 : 10);
            // down sample depth to quarter res
            _preLightPass.Blit(src, _quarterDepthBuffer, _bilateralBlurMaterial, dx11 ? 6 : 11);

            if(EnviroSky.instance.singlePassInstancedVR)
            {
                _preLightPass.SetRenderTarget(_quarterVolumeLightTexture,0,CubemapFace.Unknown,0);
                _preLightPass.ClearRenderTarget(false, true, new Color(0, 0, 0, 1));
                _preLightPass.SetRenderTarget(_quarterVolumeLightTexture,0,CubemapFace.Unknown,1);
                _preLightPass.ClearRenderTarget(false, true, new Color(0, 0, 0, 1));
            }
            else
            {
                _preLightPass.SetRenderTarget(_quarterVolumeLightTexture);
                _preLightPass.ClearRenderTarget(false, true, new Color(0, 0, 0, 1));
            }

            
        }
        else if (EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Half)
        {
           // Texture nullTexture = null;
            // down sample depth to half res
            _preLightPass.Blit(src, _halfDepthBuffer, _bilateralBlurMaterial, dx11 ? 4 : 10);
            
            if(EnviroSky.instance.singlePassInstancedVR)
            {
                _preLightPass.SetRenderTarget(_halfVolumeLightTexture,0,CubemapFace.Unknown,0);
                _preLightPass.ClearRenderTarget(false, true, new Color(0, 0, 0, 1));
                _preLightPass.SetRenderTarget(_halfVolumeLightTexture,0,CubemapFace.Unknown,1);
                _preLightPass.ClearRenderTarget(false, true, new Color(0, 0, 0, 1));

            }
            else
            {
                _preLightPass.SetRenderTarget(_halfVolumeLightTexture);
                _preLightPass.ClearRenderTarget(false, true, new Color(0, 0, 0, 1));
            }            
        }
        else
        {   
            if(EnviroSky.instance.singlePassInstancedVR)
            {
                _preLightPass.SetRenderTarget(_volumeLightTexture,0,CubemapFace.Unknown,0);
                _preLightPass.ClearRenderTarget(false, true, new Color(0, 0, 0, 1));
                _preLightPass.SetRenderTarget(_volumeLightTexture,0,CubemapFace.Unknown,1);
                _preLightPass.ClearRenderTarget(false, true, new Color(0, 0, 0, 1));
            }
            else
            {
                _preLightPass.SetRenderTarget(_volumeLightTexture);
                _preLightPass.ClearRenderTarget(false, true, new Color(0, 0, 0, 1));
            }
        }

        UpdateMaterialParameters();

        //Dir volume
        if (EnviroSky.instance.volumeLightSettings.dirVolumeLighting)
        {
            Light _light = EnviroSky.instance.Components.DirectLight.GetComponent<Light>();
            int pass = 4;

            volumeLightMat.SetPass(pass);

            if (EnviroSky.instance.volumeLightSettings.directLightNoise)
                volumeLightMat.EnableKeyword("NOISE");
            else
                volumeLightMat.DisableKeyword("NOISE");

            volumeLightMat.SetVector("_LightDir", new Vector4(_light.transform.forward.x, _light.transform.forward.y, _light.transform.forward.z, 1.0f / (_light.range * _light.range)));
            volumeLightMat.SetVector("_LightColor", _light.color * _light.intensity);
            volumeLightMat.SetFloat("_MaxRayLength", EnviroSky.instance.volumeLightSettings.MaxRayLength);

            if (_light.cookie == null)
            {
                volumeLightMat.EnableKeyword("DIRECTIONAL");
                volumeLightMat.DisableKeyword("DIRECTIONAL_COOKIE");
            }
            else
            {
                volumeLightMat.EnableKeyword("DIRECTIONAL_COOKIE");
                volumeLightMat.DisableKeyword("DIRECTIONAL");
                volumeLightMat.SetTexture("_LightTexture0", _light.cookie);
            }

            volumeLightMat.SetInt("_SampleCount", EnviroSky.instance.volumeLightSettings.SampleCount);
            volumeLightMat.SetVector("_NoiseVelocity", new Vector4(-EnviroSky.instance.Components.windZone.transform.forward.x * EnviroSky.instance.windIntensity * 10, -EnviroSky.instance.Components.windZone.transform.forward.z * EnviroSky.instance.windIntensity * 10) * EnviroSky.instance.volumeLightSettings.noiseScale);
            volumeLightMat.SetVector("_NoiseData", new Vector4(EnviroSky.instance.volumeLightSettings.noiseScale, EnviroSky.instance.volumeLightSettings.noiseIntensity, EnviroSky.instance.volumeLightSettings.noiseIntensityOffset));
            volumeLightMat.SetVector("_MieG", new Vector4(1 - (EnviroSky.instance.volumeLightSettings.Anistropy * EnviroSky.instance.volumeLightSettings.Anistropy), 1 + (EnviroSky.instance.volumeLightSettings.Anistropy * EnviroSky.instance.volumeLightSettings.Anistropy), 2 * EnviroSky.instance.volumeLightSettings.Anistropy, 1.0f / (4.0f * Mathf.PI)));
            volumeLightMat.SetVector("_VolumetricLight", new Vector4(EnviroSky.instance.volumeLightSettings.ScatteringCoef.Evaluate(EnviroSky.instance.GameTime.solarTime), EnviroSky.instance.volumeLightSettings.ExtinctionCoef, _light.range, 1.0f));// - SkyboxExtinctionCoef));
            volumeLightMat.SetTexture("_CameraDepthTexture", GetVolumeLightDepthBuffer());
            volumeLightMat.SetFloat("_ShadowDistance", QualitySettings.shadowDistance); 
            // volumeLightMat.SetVector("_DensityParams", new Vector4(EnviroSky.instance.volumeFogSettings.skyDensity, EnviroSky.instance.volumeFogSettings.groundDensity, EnviroSky.instance.volumeFogSettings.groundFogHeight, heightDensity * 0.5f));

            //Texture tex = null;
            if (_light.shadows != LightShadows.None)
            {
                volumeLightMat.EnableKeyword("SHADOWS_DEPTH");
                Graphics.Blit(src, GetVolumeLightBuffer(), volumeLightMat, pass);
            }
            else
            {
                volumeLightMat.DisableKeyword("SHADOWS_DEPTH");
                Graphics.Blit(src, GetVolumeLightBuffer(), volumeLightMat, pass);
            }
        }

        if (EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Quarter)
        {
            RenderTexture temp = RenderTexture.GetTemporary(_quarterVolumeLightTexture.descriptor);

            // horizontal bilateral blur at quarter res
            Graphics.Blit(_quarterVolumeLightTexture, temp, _bilateralBlurMaterial, 8);
            // vertical bilateral blur at quarter res
            Graphics.Blit(temp, _quarterVolumeLightTexture, _bilateralBlurMaterial, 9);

            // upscale to full res
            Graphics.Blit(_quarterVolumeLightTexture, _volumeLightTexture, _bilateralBlurMaterial, 7);

            RenderTexture.ReleaseTemporary(temp);
        }
        else if (EnviroSky.instance.volumeLightSettings.Resolution == VolumtericResolution.Half)
        {
            RenderTexture temp = RenderTexture.GetTemporary(_halfVolumeLightTexture.descriptor);
  
            // horizontal bilateral blur at half res
            Graphics.Blit(_halfVolumeLightTexture, temp, _bilateralBlurMaterial, 2);

            // vertical bilateral blur at half res
            Graphics.Blit(temp, _halfVolumeLightTexture, _bilateralBlurMaterial, 3);

            // upscale to full res
            Graphics.Blit(_halfVolumeLightTexture, _volumeLightTexture, _bilateralBlurMaterial, 5);
            RenderTexture.ReleaseTemporary(temp);
        }
        else
        {
            RenderTexture temp = RenderTexture.GetTemporary(_volumeLightTexture.descriptor);
            temp.filterMode = FilterMode.Bilinear;

            // horizontal bilateral blur at full res
            Graphics.Blit(_volumeLightTexture, temp, _bilateralBlurMaterial, 0);
            // vertical bilateral blur at full res
            Graphics.Blit(temp, _volumeLightTexture, _bilateralBlurMaterial, 1);
            RenderTexture.ReleaseTemporary(temp);
        }

        Shader.EnableKeyword("ENVIROVOLUMELIGHT");
        Shader.SetGlobalTexture("_EnviroVolumeLightingTex", _volumeLightTexture);

        RenderFog(src, dst);
        #endregion
    }

    private void RenderDistanceBlur(RenderTexture source, RenderTexture destination)
    {
        var useRGBM = myCam.allowHDR;

        // source texture size
        var tw = source.width;
        var th = source.height;

        // halve the texture size for the low quality mode
        if (!EnviroSky.instance.distanceBlurSettings.highQuality)
        {
            tw /= 2;
            th /= 2;
        }

        if (postProcessMat == null)
            postProcessMat = new Material(Shader.Find("Hidden/EnviroDistanceBlur"));

        postProcessMat.SetTexture("_DistTex", EnviroSky.instance.ressources.distributionTexture);
        postProcessMat.SetFloat("_Distance", EnviroSky.instance.blurDistance);
        postProcessMat.SetFloat("_Radius", EnviroSky.instance.distanceBlurSettings.radius);

        // blur buffer format
        var rtFormat = useRGBM ?
            RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

        // determine the iteration count
        var logh = Mathf.Log(th, 2) + EnviroSky.instance.distanceBlurSettings.radius - 8;
        var logh_i = (int)logh;
        var iterations = Mathf.Clamp(logh_i, 1, kMaxIterations);

        // update the shader properties
        var lthresh = thresholdLinear;
        postProcessMat.SetFloat("_Threshold", lthresh);

        var knee = lthresh * 0.5f + 1e-5f;
        var curve = new Vector3(lthresh - knee, knee * 2, 0.25f / knee);
        postProcessMat.SetVector("_Curve", curve);

        var pfo = !EnviroSky.instance.distanceBlurSettings.highQuality && EnviroSky.instance.distanceBlurSettings.antiFlicker;
        postProcessMat.SetFloat("_PrefilterOffs", pfo ? -0.5f : 0.0f);

        postProcessMat.SetFloat("_SampleScale", 0.5f + logh - logh_i);
        postProcessMat.SetFloat("_Intensity", EnviroSky.instance.blurIntensity);
        postProcessMat.SetFloat("_SkyBlurring", EnviroSky.instance.blurSkyIntensity);

        // prefilter pass

        RenderTextureDescriptor renderDescriptor = source.descriptor;
        renderDescriptor.width = tw; 
        renderDescriptor.height = th;

        //if (!EnviroSky.instance.singlePassInstancedVR)
        //    renderDescriptor.vrUsage = VRTextureUsage.None;

        var prefiltered = RenderTexture.GetTemporary(renderDescriptor);

        var pass = EnviroSky.instance.distanceBlurSettings.antiFlicker ? 1 : 0;
        Graphics.Blit(source, prefiltered, postProcessMat, pass);

        // construct a mip pyramid
        var last = prefiltered;
        for (var level = 0; level < iterations; level++)
        {
            RenderTextureDescriptor lastDescriptor = last.descriptor;
            lastDescriptor.width = lastDescriptor.width / 2;
            lastDescriptor.height = lastDescriptor.height / 2;

            _blurBuffer1[level] = RenderTexture.GetTemporary(lastDescriptor);

            pass = (level == 0) ? (EnviroSky.instance.distanceBlurSettings.antiFlicker ? 3 : 2) : 4;
            Graphics.Blit(last, _blurBuffer1[level], postProcessMat, pass);

            last = _blurBuffer1[level];
        }

        // upsample and combine loop
        for (var level = iterations - 2; level >= 0; level--)
        {
            var basetex = _blurBuffer1[level];
            postProcessMat.SetTexture("_BaseTex", basetex);

            RenderTextureDescriptor baseDescriptor = basetex.descriptor;

            _blurBuffer2[level] = RenderTexture.GetTemporary(baseDescriptor);

            pass = EnviroSky.instance.distanceBlurSettings.highQuality ? 6 : 5;
            Graphics.Blit(last, _blurBuffer2[level], postProcessMat, pass);
            last = _blurBuffer2[level];
        }

        // finish process
        postProcessMat.SetTexture("_BaseTex", source);
        pass = EnviroSky.instance.distanceBlurSettings.highQuality ? 8 : 7;
        Graphics.Blit(last, destination, postProcessMat, pass);

        // release the temporary buffers
        for (var i = 0; i < kMaxIterations; i++)
        {
            if (_blurBuffer1[i] != null)
                RenderTexture.ReleaseTemporary(_blurBuffer1[i]);

            if (_blurBuffer2[i] != null)
                RenderTexture.ReleaseTemporary(_blurBuffer2[i]);

            _blurBuffer1[i] = null;
            _blurBuffer2[i] = null;
        }

        RenderTexture.ReleaseTemporary(prefiltered);
    }
    #endregion
    ////////////////////////
    #region Volume Fog Functions
    private void UpdateMaterialParameters()
    {
        if (_bilateralBlurMaterial == null)
            _bilateralBlurMaterial = new Material(Shader.Find("Hidden/EnviroBilateralBlur"));

        _bilateralBlurMaterial.SetTexture("_HalfResDepthBuffer", _halfDepthBuffer);
        _bilateralBlurMaterial.SetTexture("_HalfResColor", _halfVolumeLightTexture);
        _bilateralBlurMaterial.SetTexture("_QuarterResDepthBuffer", _quarterDepthBuffer);
        _bilateralBlurMaterial.SetTexture("_QuarterResColor", _quarterVolumeLightTexture);

        Shader.SetGlobalTexture("_DitherTexture", _ditheringTexture);
        Shader.SetGlobalTexture("_NoiseTexture", EnviroSky.instance.ressources.detailNoiseTexture);
    }
    private void GenerateDitherTexture()
    {
        if (_ditheringTexture != null)
        {
            return;
        }

        int size = 8;
#if DITHER_4_4
        size = 4;
#endif
        _ditheringTexture = new Texture2D(size, size, TextureFormat.Alpha8, false, true);
        _ditheringTexture.filterMode = FilterMode.Point;
        Color32[] c = new Color32[size * size];

        byte b;
#if DITHER_4_4
        b = (byte)(0.0f / 16.0f * 255); c[0] = new Color32(b, b, b, b);
        b = (byte)(8.0f / 16.0f * 255); c[1] = new Color32(b, b, b, b);
        b = (byte)(2.0f / 16.0f * 255); c[2] = new Color32(b, b, b, b);
        b = (byte)(10.0f / 16.0f * 255); c[3] = new Color32(b, b, b, b);

        b = (byte)(12.0f / 16.0f * 255); c[4] = new Color32(b, b, b, b);
        b = (byte)(4.0f / 16.0f * 255); c[5] = new Color32(b, b, b, b);
        b = (byte)(14.0f / 16.0f * 255); c[6] = new Color32(b, b, b, b);
        b = (byte)(6.0f / 16.0f * 255); c[7] = new Color32(b, b, b, b);

        b = (byte)(3.0f / 16.0f * 255); c[8] = new Color32(b, b, b, b);
        b = (byte)(11.0f / 16.0f * 255); c[9] = new Color32(b, b, b, b);
        b = (byte)(1.0f / 16.0f * 255); c[10] = new Color32(b, b, b, b);
        b = (byte)(9.0f / 16.0f * 255); c[11] = new Color32(b, b, b, b);

        b = (byte)(15.0f / 16.0f * 255); c[12] = new Color32(b, b, b, b);
        b = (byte)(7.0f / 16.0f * 255); c[13] = new Color32(b, b, b, b);
        b = (byte)(13.0f / 16.0f * 255); c[14] = new Color32(b, b, b, b);
        b = (byte)(5.0f / 16.0f * 255); c[15] = new Color32(b, b, b, b);
#else
        int i = 0;
        b = (byte)(1.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(49.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(13.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(61.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(4.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(52.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(16.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(64.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);

        b = (byte)(33.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(17.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(45.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(29.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(36.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(20.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(48.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(32.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);

        b = (byte)(9.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(57.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(5.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(53.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(12.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(60.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(8.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(56.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);

        b = (byte)(41.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(25.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(37.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(21.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(44.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(28.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(40.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(24.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);

        b = (byte)(3.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(51.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(15.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(63.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(2.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(50.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(14.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(62.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);

        b = (byte)(35.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(19.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(47.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(31.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(34.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(18.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(46.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(30.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);

        b = (byte)(11.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(59.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(7.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(55.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(10.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(58.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(6.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(54.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);

        b = (byte)(43.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(27.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(39.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(23.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(42.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(26.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(38.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
        b = (byte)(22.0f / 65.0f * 255); c[i++] = new Color32(b, b, b, b);
#endif

        _ditheringTexture.SetPixels32(c);
        _ditheringTexture.Apply();
    }
    private Mesh CreateSpotLightMesh()
    {
        // copy & pasted from other project, the geometry is too complex, should be simplified
        Mesh mesh = new Mesh();

        const int segmentCount = 16;
        Vector3[] vertices = new Vector3[2 + segmentCount * 3];
        Color32[] colors = new Color32[2 + segmentCount * 3];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 0, 1);

        float angle = 0;
        float step = Mathf.PI * 2.0f / segmentCount;
        float ratio = 0.9f;

        for (int i = 0; i < segmentCount; ++i)
        {
            vertices[i + 2] = new Vector3(-Mathf.Cos(angle) * ratio, Mathf.Sin(angle) * ratio, ratio);
            colors[i + 2] = new Color32(255, 255, 255, 255);
            vertices[i + 2 + segmentCount] = new Vector3(-Mathf.Cos(angle), Mathf.Sin(angle), 1);
            colors[i + 2 + segmentCount] = new Color32(255, 255, 255, 0);
            vertices[i + 2 + segmentCount * 2] = new Vector3(-Mathf.Cos(angle) * ratio, Mathf.Sin(angle) * ratio, 1);
            colors[i + 2 + segmentCount * 2] = new Color32(255, 255, 255, 255);
            angle += step;
        }

        mesh.vertices = vertices;
        mesh.colors32 = colors;

        int[] indices = new int[segmentCount * 3 * 2 + segmentCount * 6 * 2];
        int index = 0;

        for (int i = 2; i < segmentCount + 1; ++i)
        {
            indices[index++] = 0;
            indices[index++] = i;
            indices[index++] = i + 1;
        }

        indices[index++] = 0;
        indices[index++] = segmentCount + 1;
        indices[index++] = 2;

        for (int i = 2; i < segmentCount + 1; ++i)
        {
            indices[index++] = i;
            indices[index++] = i + segmentCount;
            indices[index++] = i + 1;

            indices[index++] = i + 1;
            indices[index++] = i + segmentCount;
            indices[index++] = i + segmentCount + 1;
        }

        indices[index++] = 2;
        indices[index++] = 1 + segmentCount;
        indices[index++] = 2 + segmentCount;

        indices[index++] = 2 + segmentCount;
        indices[index++] = 1 + segmentCount;
        indices[index++] = 1 + segmentCount + segmentCount;

        //------------
        for (int i = 2 + segmentCount; i < segmentCount + 1 + segmentCount; ++i)
        {
            indices[index++] = i;
            indices[index++] = i + segmentCount;
            indices[index++] = i + 1;

            indices[index++] = i + 1;
            indices[index++] = i + segmentCount;
            indices[index++] = i + segmentCount + 1;
        }

        indices[index++] = 2 + segmentCount;
        indices[index++] = 1 + segmentCount * 2;
        indices[index++] = 2 + segmentCount * 2;

        indices[index++] = 2 + segmentCount * 2;
        indices[index++] = 1 + segmentCount * 2;
        indices[index++] = 1 + segmentCount * 3;

        ////-------------------------------------
        for (int i = 2 + segmentCount * 2; i < segmentCount * 3 + 1; ++i)
        {
            indices[index++] = 1;
            indices[index++] = i + 1;
            indices[index++] = i;
        }

        indices[index++] = 1;
        indices[index++] = 2 + segmentCount * 2;
        indices[index++] = segmentCount * 3 + 1;

        mesh.triangles = indices;
        mesh.RecalculateBounds();

        return mesh;
    }
    #endregion
    ////////////////////////
    #region Volume Clouds Functions
    ////////// Clouds Functions ///////////////
    public void SetCloudProperties()
    {
        if (usedCloudsQuality.baseQuality == EnviroVolumeCloudsQualitySettings.CloudDetailQuality.Low)
            cloudsMat.SetTexture("_Noise", EnviroSky.instance.ressources.noiseTexture);
        else
            cloudsMat.SetTexture("_Noise", EnviroSky.instance.ressources.noiseTextureHigh);

        if (usedCloudsQuality.detailQuality == EnviroVolumeCloudsQualitySettings.CloudDetailQuality.Low)
            cloudsMat.SetTexture("_DetailNoise", EnviroSky.instance.ressources.detailNoiseTexture);
        else
            cloudsMat.SetTexture("_DetailNoise", EnviroSky.instance.ressources.detailNoiseTextureHigh);

        if (EnviroSky.instance.floatingPointOriginAnchor != null)
            EnviroSky.instance.floatingPointOriginMod = EnviroSky.instance.floatingPointOriginAnchor.position;

        cloudsMat.SetVector("_CameraPosition", myCam.transform.position - EnviroSky.instance.floatingPointOriginMod);

        switch (myCam.stereoActiveEye)
        {
            case Camera.MonoOrStereoscopicEye.Mono:
                projection = myCam.projectionMatrix;
                Matrix4x4 inverseProjection = projection.inverse;
                cloudsMat.SetMatrix("_InverseProjection", inverseProjection);
                inverseRotation = myCam.cameraToWorldMatrix;
                cloudsMat.SetMatrix("_InverseRotation", inverseRotation);
                break;

            case Camera.MonoOrStereoscopicEye.Left:
                projection = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                Matrix4x4 inverseProjectionLeft = projection.inverse;
                cloudsMat.SetMatrix("_InverseProjection", inverseProjectionLeft);
                inverseRotation = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse;
                cloudsMat.SetMatrix("_InverseRotation", inverseRotation);

                if (myCam.stereoEnabled)
                {
                    Matrix4x4 inverseProjectionRightSP = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right).inverse;
                    cloudsMat.SetMatrix("_InverseProjection_SP", inverseProjectionRightSP);
                    inverseRotationSPVR = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;
                    cloudsMat.SetMatrix("_InverseRotation_SP", inverseRotationSPVR);
                }
                break;

            case Camera.MonoOrStereoscopicEye.Right:
                projection = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                Matrix4x4 inverseProjectionRight = projection.inverse;
                cloudsMat.SetMatrix("_InverseProjection_SP", inverseProjectionRight);
                inverseRotation = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;
                cloudsMat.SetMatrix("_InverseRotation_SP", inverseRotation);
                break;
        }

        //Weather Map
        if (EnviroSky.instance.cloudsSettings.customWeatherMap == null)
            cloudsMat.SetTexture("_WeatherMap", EnviroSky.instance.weatherMap);
        else
            cloudsMat.SetTexture("_WeatherMap", EnviroSky.instance.cloudsSettings.customWeatherMap);

        //Curl Noise
        if (EnviroSky.instance.cloudsSettings.cloudsQualitySettings.useCurlNoise)
        {
            cloudsMat.EnableKeyword("ENVIRO_CURLNOISE");
            cloudsMat.SetTexture("_CurlNoise", EnviroSky.instance.ressources.curlMap);
        }
        else
        {
            cloudsMat.DisableKeyword("ENVIRO_CURLNOISE");
        }

        //Optimizations
        if (EnviroSky.instance.cloudsSettings.useHaltonRaymarchOffset)
        {
            cloudsMat.EnableKeyword("ENVIRO_HALTONOFFSET");
            cloudsMat.SetFloat("_RaymarchOffset", sequence.Get());
            cloudsMat.SetVector("_TexelSize", subFrameTex.texelSize);
        }
        else
        {
            cloudsMat.DisableKeyword("ENVIRO_HALTONOFFSET");
        }

        //RaymarchOffset

        if (!EnviroSky.instance.cloudsSettings.useLessSteps)
            cloudsMat.SetVector("_Steps", new Vector4(usedCloudsQuality.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale, usedCloudsQuality.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale, 0.0f, 0.0f));
        else
            cloudsMat.SetVector("_Steps", new Vector4((usedCloudsQuality.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale) * 0.75f, (usedCloudsQuality.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale) * 0.75f, 0.0f, 0.0f));
        cloudsMat.SetFloat("_BaseNoiseUV", usedCloudsQuality.baseNoiseUV);
        cloudsMat.SetFloat("_DetailNoiseUV", usedCloudsQuality.detailNoiseUV);
        cloudsMat.SetFloat("_AmbientSkyColorIntensity", EnviroSky.instance.cloudsSettings.ambientLightIntensity.Evaluate(EnviroSky.instance.GameTime.solarTime));
        cloudsMat.SetVector("_CloudsLighting", new Vector4(EnviroSky.instance.cloudsConfig.scatteringCoef, EnviroSky.instance.cloudsSettings.hgPhase, EnviroSky.instance.cloudsSettings.silverLiningIntensity, EnviroSky.instance.cloudsSettings.silverLiningSpread.Evaluate(EnviroSky.instance.GameTime.solarTime)));
        cloudsMat.SetVector("_CloudsLightingExtended", new Vector4(EnviroSky.instance.cloudsConfig.edgeDarkness, EnviroSky.instance.cloudsConfig.ambientSkyColorIntensity, EnviroSky.instance.tonemapping ? 0f : 1f, EnviroSky.instance.cloudsSettings.cloudsExposure));
        cloudsMat.SetColor("_AmbientLightColor", EnviroSky.instance.cloudsSettings.volumeCloudsAmbientColor.Evaluate(EnviroSky.instance.GameTime.solarTime));


        float bottomH = usedCloudsQuality.bottomCloudHeight + EnviroSky.instance.cloudsSettings.cloudsHeightMod;
        float topH = usedCloudsQuality.topCloudHeight + EnviroSky.instance.cloudsSettings.cloudsHeightMod;

        cloudsMat.SetVector("_CloudsParameter", new Vector4(bottomH, topH, 1 / (topH - bottomH), EnviroSky.instance.cloudsSettings.cloudsWorldScale * 10));

        if (myCam.transform.position.y > topH)
            aboveClouds = true;
        else
            aboveClouds = false;

        if (EnviroSky.instance.cloudsSettings.useLessSteps)
            cloudsMat.SetVector("_CloudDensityScale", new Vector4(EnviroSky.instance.cloudsConfig.density * 1.5f, EnviroSky.instance.cloudsConfig.lightStepModifier, EnviroSky.instance.GameTime.dayNightSwitch, EnviroSky.instance.GameTime.solarTime));
        else
            cloudsMat.SetVector("_CloudDensityScale", new Vector4(EnviroSky.instance.cloudsConfig.density, EnviroSky.instance.cloudsConfig.lightStepModifier, EnviroSky.instance.GameTime.dayNightSwitch, EnviroSky.instance.GameTime.solarTime));

        cloudsMat.SetFloat("_CloudsType", EnviroSky.instance.cloudsConfig.cloudType);
        cloudsMat.SetVector("_CloudsCoverageSettings", new Vector4(EnviroSky.instance.cloudsConfig.coverage * EnviroSky.instance.cloudsSettings.globalCloudCoverage, EnviroSky.instance.cloudsConfig.lightAbsorbtion, EnviroSky.instance.cloudsSettings.cloudsQualitySettings.transmissionToExit, 0f));
        cloudsMat.SetVector("_CloudsAnimation", new Vector4(EnviroSky.instance.cloudAnim.x, EnviroSky.instance.cloudAnim.y, EnviroSky.instance.cloudsSettings.cloudsWindDirectionX, EnviroSky.instance.cloudsSettings.cloudsWindDirectionY));
        cloudsMat.SetColor("_LightColor", EnviroSky.instance.cloudsSettings.volumeCloudsColor.Evaluate(EnviroSky.instance.GameTime.solarTime));
        cloudsMat.SetColor("_MoonLightColor", EnviroSky.instance.cloudsSettings.volumeCloudsMoonColor.Evaluate(EnviroSky.instance.GameTime.lunarTime));
        cloudsMat.SetFloat("_stepsInDepth", usedCloudsQuality.stepsInDepthModificator);
        cloudsMat.SetFloat("_LODDistance", usedCloudsQuality.lodDistance);


        if (EnviroSky.instance.lightSettings.directionalLightMode == EnviroLightSettings.LightingMode.Dual)
        {
            if (EnviroSky.instance.GameTime.dayNightSwitch < EnviroSky.instance.GameTime.solarTime)
                cloudsMat.SetVector("_LightDir", -EnviroSky.instance.Components.DirectLight.transform.forward);
            else if (EnviroSky.instance.Components.AdditionalDirectLight != null)
                cloudsMat.SetVector("_LightDir", -EnviroSky.instance.Components.AdditionalDirectLight.transform.forward);
        }
        else
            cloudsMat.SetVector("_LightDir", -EnviroSky.instance.Components.DirectLight.transform.forward);

        cloudsMat.SetFloat("_LightIntensity", EnviroSky.instance.cloudsSettings.lightIntensity.Evaluate(EnviroSky.instance.GameTime.solarTime));
        cloudsMat.SetVector("_CloudsErosionIntensity", new Vector4(1f - EnviroSky.instance.cloudsConfig.baseErosionIntensity, EnviroSky.instance.cloudsConfig.detailErosionIntensity, EnviroSky.instance.cloudsSettings.attenuationClamp.Evaluate(EnviroSky.instance.GameTime.solarTime), EnviroSky.instance.cloudAnim.z));
        // cloudsMat.SetTexture("_BlueNoise", blueNoise);        
        // cloudsMat.SetVector("_Randomness", new Vector4(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value));
    }
    private void SetBlitmaterialProperties()
    {
        Matrix4x4 inverseProjection = projection.inverse;

        blitMat.SetMatrix("_PreviousRotation", previousRotation);
        blitMat.SetMatrix("_Projection", projection);
        blitMat.SetMatrix("_InverseRotation", inverseRotation);
        blitMat.SetMatrix("_InverseProjection", inverseProjection);

        if (myCam.stereoEnabled)
        {
            Matrix4x4 inverseProjectionSPVR = projectionSPVR.inverse;
            blitMat.SetMatrix("_PreviousRotationSPVR", previousRotationSPVR);
            blitMat.SetMatrix("_ProjectionSPVR", projectionSPVR);
            blitMat.SetMatrix("_InverseRotationSPVR", inverseRotationSPVR);
            blitMat.SetMatrix("_InverseProjectionSPVR", inverseProjectionSPVR);

        }

        blitMat.SetFloat("_FrameNumber", subFrameNumber);
        blitMat.SetFloat("_ReprojectionPixelSize", reprojectionPixelSize);
        blitMat.SetVector("_SubFrameDimension", new Vector2(subFrameWidth, subFrameHeight));
        blitMat.SetVector("_FrameDimension", new Vector2(frameWidth, frameHeight));
    }
    RenderTexture DownsampleDepth(int X, int Y, Texture src, Material mat, int downsampleFactor)
    {
        Vector2 offset = new Vector2(1.0f / X, 1.0f / X);
        X /= downsampleFactor;
        Y /= downsampleFactor;
        RenderTexture lowDepth = RenderTexture.GetTemporary(X, Y, 0);
        mat.SetVector("_PixelSize", offset);
        Graphics.Blit(src, lowDepth, mat);

        return lowDepth;
    }
    private void RenderClouds(RenderTexture source, RenderTexture tex)
    {
        if (cloudsMat == null)
            cloudsMat = new Material(Shader.Find("Enviro/Standard/RaymarchClouds"));

        EnviroSky.instance.RenderCloudMaps();
        //cloudsMat.SetTexture("_MainTex", source);
        SetCloudProperties();
        //Render Clouds with downsampling tex
        Graphics.Blit(source, tex, cloudsMat);
    }

    private void CreateCloudsRenderTextures(RenderTexture source)
    {
        if (subFrameTex != null)
        {
            subFrameTex.Release();
            DestroyImmediate(subFrameTex);
            subFrameTex = null;
        }

        if (prevFrameTex != null)
        {
            prevFrameTex.Release();
            DestroyImmediate(prevFrameTex);
            prevFrameTex = null;
        }

       // RenderTextureFormat format = myCam.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

        if (subFrameTex == null)
        {
            RenderTextureDescriptor desc = source.descriptor;
            desc.width = subFrameWidth;
            desc.height = subFrameHeight;
#if UNITY_2019_3_OR_NEWER
            desc.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat;
#endif
            subFrameTex = new RenderTexture(desc);
            subFrameTex.filterMode = FilterMode.Bilinear;
            subFrameTex.hideFlags = HideFlags.HideAndDontSave;

            isFirstFrame = true;
        }

        if (prevFrameTex == null)
        {
            RenderTextureDescriptor desc = source.descriptor;
            desc.width = frameWidth;
            desc.height = frameHeight;
#if UNITY_2019_3_OR_NEWER
            desc.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat;
#endif
            prevFrameTex = new RenderTexture(desc);
            prevFrameTex.filterMode = FilterMode.Bilinear;
            prevFrameTex.hideFlags = HideFlags.HideAndDontSave;

            isFirstFrame = true;
        }
    }

    private void SetReprojectionPixelSize(EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize pSize)
    {
        switch (pSize)
        {
            case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.Off:
                reprojectionPixelSize = 1;
                break;

            case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.Low:
                reprojectionPixelSize = 2;
                break;

            case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.Medium:
                reprojectionPixelSize = 4;
                break;

            case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.High:
                reprojectionPixelSize = 8;
                break;
        }

        frameList = CalculateFrames(reprojectionPixelSize);
    }
    private void StartFrame()
    {
        textureDimensionChanged = UpdateFrameDimensions();

        switch (myCam.stereoActiveEye)
        {
            case Camera.MonoOrStereoscopicEye.Mono:
                projection = myCam.projectionMatrix;
                rotation = myCam.worldToCameraMatrix;
                inverseRotation = myCam.cameraToWorldMatrix;
                break;

            case Camera.MonoOrStereoscopicEye.Left:
                projection = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                rotation = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
                inverseRotation = rotation.inverse;
                projectionSPVR = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                rotationSPVR = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
                inverseRotationSPVR = rotationSPVR.inverse;
                break;

            case Camera.MonoOrStereoscopicEye.Right:
                projection = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                rotation = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
                inverseRotation = rotation.inverse;
                break;
        }
    }
    private void FinalizeFrame()
    {
        renderingCounter++;

        previousRotation = rotation;
        previousRotationSPVR = rotationSPVR;

        int reproSize = reprojectionPixelSize * reprojectionPixelSize;
        subFrameNumber = frameList[renderingCounter % reproSize];
    }
    private bool UpdateFrameDimensions()
    {
        //Add downsampling
        int newFrameWidth = myCam.scaledPixelWidth / EnviroSky.instance.cloudsSettings.cloudsQualitySettings.cloudsRenderResolution;
        int newFrameHeight = myCam.scaledPixelHeight / EnviroSky.instance.cloudsSettings.cloudsQualitySettings.cloudsRenderResolution;

        //Reset temporal reprojection size when zero. Needed if SkyManager starts deactivated
        if (EnviroSky.instance != null && reprojectionPixelSize == 0)
            SetReprojectionPixelSize(EnviroSky.instance.cloudsSettings.cloudsQualitySettings.reprojectionPixelSize);

        //Calculate new frame width and height
        while (newFrameWidth % reprojectionPixelSize != 0)
        {
            newFrameWidth++;
        }

        while (newFrameHeight % reprojectionPixelSize != 0)
        {
            newFrameHeight++;
        }

        int newSubFrameWidth = newFrameWidth / reprojectionPixelSize;
        int newSubFrameHeight = newFrameHeight / reprojectionPixelSize;

        //Check if diemensions changed
        if (newFrameWidth != frameWidth || newSubFrameWidth != subFrameWidth || newFrameHeight != frameHeight || newSubFrameHeight != subFrameHeight)
        {
            //Cache new dimensions
            frameWidth = newFrameWidth;
            frameHeight = newFrameHeight;
            subFrameWidth = newSubFrameWidth;
            subFrameHeight = newSubFrameHeight;
            return true;
        }
        else
        {
            //Cache new dimensions
            frameWidth = newFrameWidth;
            frameHeight = newFrameHeight;
            subFrameWidth = newSubFrameWidth;
            subFrameHeight = newSubFrameHeight;
            return false;
        }
    }
    private int[] CalculateFrames(int reproSize)
    {
        subFrameNumber = 0;

        int i = 0;
        int reproCount = reproSize * reproSize;
        int[] frameNumbers = new int[reproCount];

        for (i = 0; i < reproCount; i++)
        {
            frameNumbers[i] = i;
        }

        while (i-- > 0)
        {
            int frame = frameNumbers[i];
            int count = (int)(UnityEngine.Random.Range(0, 1) * 1000.0f) % reproCount;
            frameNumbers[i] = frameNumbers[count];
            frameNumbers[count] = frame;
        }

        return frameNumbers;
    }
    #endregion
#endif
}