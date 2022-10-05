
 

#if defined (ENVIROHDRP)		
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

// Mobile: use RGBM instead of float/half RGB
#define USE_RGBM defined(SHADER_API_MOBILE)

TEXTURE2D_X(_MainTex);
TEXTURE2D_X(_BaseTex);
float2 _MainTex_TexelSize;
float2 _BaseTex_TexelSize;
half4 _MainTex_ST;
half4 _BaseTex_ST;

float4 _ScaledSize;
 
float4 _TexelSize;       // xy: size, zw: texel size

float _PrefilterOffs;
half _Threshold;
half3 _Curve;
float _SampleScale;
half _Intensity;

//sampler2D _CameraDepthTexture;
sampler2D _DistTex;
float _Distance;
float _Radius;
float _SkyBlurring;

float4x4 _LeftWorldFromView;
float4x4 _RightWorldFromView;
float4x4 _LeftViewFromScreen;
float4x4 _RightViewFromScreen;

//////////////////////////

half3 GammaToLinearSpace(half3 sRGB)
{
	// Approximate version from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
	return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);
}

half3 LinearToGammaSpace(half3 linRGB)
{
	linRGB = max(linRGB, half3(0.h, 0.h, 0.h));
	// An almost-perfect approximation from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
	return max(1.055h * pow(linRGB, 0.416666667h) - 0.055h, 0.h);
}


// Brightness function
half Brightness(half3 c)
{
	return max(max(c.r, c.g), c.b);
}

// 3-tap median filter
half3 Median(half3 a, half3 b, half3 c)
{
	return a + b + c - min(min(a, b), c) - max(max(a, b), c);
}

// Clamp HDR value within a safe range
half3 SafeHDR(half3 c) { return min(c, 65000); }
half4 SafeHDR(half4 c) { return min(c, 65000); }

// RGBM encoding/decoding
half4 EncodeHDR(float3 rgb)
{
#if USE_RGBM
	rgb *= 1.0 / 8;
	float m = max(max(rgb.r, rgb.g), max(rgb.b, 1e-6));
	m = ceil(m * 255) / 255;
	return half4(rgb / m, m);
#else
	return half4(rgb, 0);
#endif
}

float3 DecodeHDR(half4 rgba)
{
#if USE_RGBM
	return rgba.rgb * rgba.a * 8;
#else
	return rgba.rgb;
#endif
}

// Downsample with a 4x4 box filter
half3 DownsampleFilter(float2 uv)
{
	float4 d = _MainTex_TexelSize.xyxy * float4(-1, -1, +1, +1);

	half3 s;
	s = DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + ClampAndScaleUVForBilinear(d.xy)));
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + ClampAndScaleUVForBilinear(d.zy)));
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + ClampAndScaleUVForBilinear(d.xw)));
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + ClampAndScaleUVForBilinear(d.zw)));

	return s * (1.0 / 4);
}

// Downsample with a 4x4 box filter + anti-flicker filter
half3 DownsampleAntiFlickerFilter(float2 uv)
{
	float4 d = _MainTex_TexelSize.xyxy * float4(-1, -1, +1, +1);

	half3 s1 = DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + ClampAndScaleUVForBilinear(d.xy)));
	half3 s2 = DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + ClampAndScaleUVForBilinear(d.zy)));
	half3 s3 = DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + ClampAndScaleUVForBilinear(d.xw)));
	half3 s4 = DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + ClampAndScaleUVForBilinear(d.zw)));

	// Karis's luma weighted average (using brightness instead of luma)
	half s1w = 1 / (Brightness(s1) + 1);
	half s2w = 1 / (Brightness(s2) + 1);
	half s3w = 1 / (Brightness(s3) + 1);
	half s4w = 1 / (Brightness(s4) + 1);
	half one_div_wsum = 1 / (s1w + s2w + s3w + s4w);

	return (s1 * s1w + s2 * s2w + s3 * s3w + s4 * s4w) * one_div_wsum;
}

half3 UpsampleFilter(float2 uv)
{
#if HIGH_QUALITY
	// 9-tap bilinear upsampler (tent filter)
	float4 d = _MainTex_TexelSize.xyxy * float4(1, 1, -1, 0) * _SampleScale;

	half3 s;
	s = DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv - d.xy)));
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv - d.wy))) * 2;
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv - d.zy)));

	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv + d.zw))) * 2;
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv)) * 4;
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv + d.xw))) * 2;

	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv + d.zy)));
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv + d.wy))) * 2;
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv + d.xy)));

	return s * (1.0 / 16);
#else
	// 4-tap bilinear upsampler
	float4 d = _MainTex_TexelSize.xyxy * float4(-1, -1, +1, +1) * (_SampleScale * 0.5);

	half3 s;
	s = DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv + d.xy)));
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv + d.zy)));
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv + d.xw)));
	s += DecodeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, ClampAndScaleUVForBilinear(uv + d.zw)));

	return s * (1.0 / 4);
#endif
}


struct Attributes
{
	uint vertexID : SV_VertexID;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
	float4 positionCS : SV_POSITION;
	float2 texcoord   : TEXCOORD0;
	UNITY_VERTEX_OUTPUT_STEREO
};

Varyings Vert(Attributes input)
{
	Varyings output;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
	output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
	output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
	return output;
}

 
//
// fragment shader
//



////
half AdjustDepth(half d, half2 uv)
{
	float4x4 proj, eyeToWorld;
	if (unity_StereoEyeIndex == 0)
	{
		proj = _LeftViewFromScreen;
		eyeToWorld = _LeftWorldFromView;
	}
	else   
	{
		proj = _RightViewFromScreen;
		eyeToWorld = _RightWorldFromView;
	}
 
		float2 uvClip = uv * 2.0 - 1.0;
		float clipDepth = d; // Fix for OpenGl Core thanks to Lars Bertram
		clipDepth = (UNITY_NEAR_CLIP_VALUE < 0) ? clipDepth * 2 - 1 : clipDepth;
		float4 clipPos = float4(uvClip, clipDepth, 1.0);
		float4 viewPos = mul(proj, clipPos); // inverse projection by clip position
		viewPos /= viewPos.w; // perspective division
		float4 wsPos = float4(mul(eyeToWorld, viewPos).xyz, 1);
		float4 wsDir = wsPos - float4(_WorldSpaceCameraPos, 0);
		float3 viewDir = normalize(wsDir);

	if (d < 0.99999)
	{
		d = clamp(d * ((_ProjectionParams.z - _ProjectionParams.y) / _Distance), 0, 1);
		//d = LOAD_TEXTURE2D(_DistTex, half2(d, 0.5));
		d = tex2D(_DistTex, half2(d, 0.5));
	}
	else
	{
		d = 1 - saturate(_SkyBlurring * viewDir.y);
		d = (clamp(d, 0, 1));
	}
	return d;
}

half4 frag_prefilter(Varyings input) : SV_Target
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	float2 uv = input.texcoord + _MainTex_TexelSize.xy * _PrefilterOffs;

#if ANTI_FLICKER
	float3 d = _MainTex_TexelSize.xyx * float3(1, 1, 0);
	half4 s0 = SafeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv));
	half3 s1 = SafeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv - d.xz).rgb);
	half3 s2 = SafeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + d.xz).rgb);
	half3 s3 = SafeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv - d.zy).rgb);
	half3 s4 = SafeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + d.zy).rgb);
	half3 m = Median(Median(s0.rgb, s1, s2), s3, s4);
#else
	half4 s0 = SafeHDR(SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv));
	half3 m = s0.rgb;
#endif

#if UNITY_COLORSPACE_GAMMA
	m = GammaToLinearSpace(m);
#endif
	// Pixel brightness
	half br = Brightness(m);

	// Under-threshold part: quadratic curve
	half rq = clamp(br - _Curve.x, 0, _Curve.y);
	rq = _Curve.z * rq * rq;

	// Combine and apply the brightness response curve.
	m *= max(rq, br - _Threshold) / max(br, 1e-5);

	// Adjust Depth Texture for fullscreen blurring
	//uint2 positionSS = input.texcoord * _ScreenSize.xy;
	//half depth = Linear01Depth(LOAD_TEXTURE2D_X(_CameraDepthTexture, positionSS).r, _ZBufferParams);
	float2 positionSS = input.texcoord;
	half depth = Linear01Depth(SAMPLE_TEXTURE2D_X(_CameraDepthTexture,s_linear_clamp_sampler, positionSS).r, _ZBufferParams);
	depth = AdjustDepth(depth, input.texcoord);

	return EncodeHDR(m * depth);
} 

half4 frag_downsample1(Varyings input) : SV_Target
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
uint2 positionSS = input.texcoord * _ScreenSize.xy;

#if ANTI_FLICKER
	return EncodeHDR(DownsampleAntiFlickerFilter(input.texcoord));
#else
	return EncodeHDR(DownsampleFilter(input.texcoord));
#endif
}

half4 frag_downsample2(Varyings input) : SV_Target
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
	uint2 positionSS = input.texcoord * _ScreenSize.xy ;

	return EncodeHDR(DownsampleFilter(input.texcoord));
} 



//// MULTI
half4 frag_upsample(Varyings input) : SV_Target
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
	
	//uint2 positionSS = input.texcoord * _ScreenSize.xy;
	//half3 base = DecodeHDR(LOAD_TEXTURE2D_X(_BaseTex, positionSS));

	float2 positionSS = input.texcoord;
	half3 base = DecodeHDR(SAMPLE_TEXTURE2D_X(_BaseTex, s_linear_clamp_sampler, positionSS));

	half3 blur = UpsampleFilter(input.texcoord); 

	return EncodeHDR(base + blur);
}

half4 frag_upsample_final(Varyings input) : SV_Target
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	//uint2 positionSS = input.texcoord * _ScreenSize.xy;
	//half4 base = LOAD_TEXTURE2D_X(_BaseTex, positionSS);

	float2 positionSS = input.texcoord;
	half4 base = SAMPLE_TEXTURE2D_X(_BaseTex, s_linear_clamp_sampler, positionSS);

	half3 blur = UpsampleFilter(input.texcoord);

#if UNITY_COLORSPACE_GAMMA
	// base.rgb = GammaToLinearSpace(base.rgb);
#endif
	half3 cout = base.rgb + blur * _Intensity;
#if UNITY_COLORSPACE_GAMMA
	cout = LinearToGammaSpace(cout);
#endif

	//half depth = Linear01Depth(LOAD_TEXTURE2D_X(_CameraDepthTexture, positionSS).r, _ZBufferParams);
	half depth = Linear01Depth(SAMPLE_TEXTURE2D_X(_CameraDepthTexture, s_linear_clamp_sampler, positionSS).r, _ZBufferParams);
	depth = AdjustDepth(depth, input.texcoord);

	return lerp(base, half4(blur,1) * (1 / _Radius), clamp(depth ,0,_Intensity));
}

#else
			struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            }; 

            v2f Vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }


            sampler2D _MainTex;

            float4 frag_prefilter (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			 float4 frag_downsample1 (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			 float4 frag_downsample2 (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			float4 frag_upsample (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			float4 frag_upsample_final (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }



#endif