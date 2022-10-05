
Shader "Enviro/Pro/LightShafts" {
	Properties {
	_MainTex ("Base", Any) = "" {}
	_ColorBuffer ("Color", Any)  = "" {}
	_BlurTex ("Blur", Any) = "" {}
	} 
	
	HLSLINCLUDE

	#pragma multi_compile __ ENVIROHDRP
	#if defined (ENVIROHDRP)				
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"
				
	TEXTURE2D_X(_MainTex); 
	TEXTURE2D_X(_BlurTex);
	TEXTURE2D_X(_ColorBuffer);
	TEXTURE2D_X(_Skybox);

	uniform half4 _SunThreshold;		
	uniform half4 _SunColor;
	uniform half4 _BlurRadius4;
	uniform half4 _SunPosition;
	uniform float _UpsampleFactor;

	#define SAMPLES_FLOAT 10.0f
	#define SAMPLES_INT 10
		
	struct Attributes
	{
		uint vertexID : SV_VertexID;
		float4 texcoord  : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float2 texcoord   : TEXCOORD0;
		float2 blurVector : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	Varyings Vert(Attributes input)
	{
		Varyings output;
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
		output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
		output.blurVector = (_SunPosition.xy - output.texcoord) * _BlurRadius4.xy;
		return output;
	}
 
		
	half4 fragScreen(Varyings input) : SV_Target {
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		
		//uint2 positionSS = input.texcoord * _ScreenSize.xy;
		//half4 colorA = LOAD_TEXTURE2D_X(_MainTex, positionSS);
		//half4 colorB = LOAD_TEXTURE2D_X(_ColorBuffer, positionSS * _UpsampleFactor);
				
		half4 colorA = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, input.texcoord);		
		half4 colorB = SAMPLE_TEXTURE2D_X(_ColorBuffer, s_linear_clamp_sampler, input.texcoord);

		half4 depthMask = saturate (colorB * _SunColor);
		return 1.0f - (1.0f-colorA) * (1.0f-depthMask);	
	} 

	half4 fragAdd(Varyings input) : SV_Target {
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		
		//float2 positionSS = input.texcoord * _ScreenSize.xy;
		//half4 colorA = LOAD_TEXTURE2D_X(_MainTex, positionSS);
		//half4 colorB = LOAD_TEXTURE2D_X(_ColorBuffer, positionSS * _UpsampleFactor);
				
		float2 positionSS = input.texcoord;
		half4 colorA = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, positionSS);		
		half4 colorB = SAMPLE_TEXTURE2D_X(_ColorBuffer, s_linear_clamp_sampler, positionSS);
 
		half4 depthMask = saturate (colorB * _SunColor);	
		return colorA + depthMask;	
	}
	
	half4 frag_radial(Varyings input) : SV_Target
	{	
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		half4 color = half4(0,0,0,0);

		for(int j = 0; j < SAMPLES_INT; j++)     
		{			
			//float2 uv = ClampAndScaleUVForBilinearPostProcessTexture(input.texcoord);	  
			//half4 tmpColor = LOAD_TEXTURE2D_X(_BlurTex, input.texcoord * _ScreenSize.xy * _UpsampleFactor);
			half4 tmpColor = SAMPLE_TEXTURE2D_X(_BlurTex, s_linear_clamp_sampler, input.texcoord);
			color += tmpColor;  
			input.texcoord += input.blurVector;			
		}
		return saturate(color / SAMPLES_FLOAT);
	}	

	
	half TransformColor (half4 skyboxValue) 
	{
		return dot(max(skyboxValue.rgb - _SunThreshold.rgb, half3(0,0,0)), half3(1,1,1)); //threshold and convert to greyscale
	}
	
	half4 frag_depth (Varyings input) : SV_Target {
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		
		//float2 uvs = input.texcoord * _ScreenSize.xy;
		//float depthSample = Linear01Depth(LOAD_TEXTURE2D_X(_CameraDepthTexture, uvs).r, _ZBufferParams);
		//half4 tex = LOAD_TEXTURE2D_X(_MainTex, uvs);
		
		
		float2 uv = input.texcoord;
		//float2 uv = ClampAndScaleUVForBilinearPostProcessTexture(positionSS);
		float depthSample = Linear01Depth(SAMPLE_TEXTURE2D_X(_CameraDepthTexture, s_linear_clamp_sampler, uv).r, _ZBufferParams);
		//float depthSample = LoadCameraDepth(positionSS * _ScreenSize.xy);
		//depthSample = Linear01Depth(depthSample, _ZBufferParams);
		half4 tex = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);
		
		// consider maximum radius
		half2 vec = input.blurVector;
		half dist = saturate (_SunPosition.w - length (vec.xy));		

		half4 outColor = 0;
		
		// consider shafts blockers
		if (depthSample > 0.99)
		{
			outColor = TransformColor(saturate(tex)) * dist;
		}
		return outColor;
	}
	
	half4 frag_nodepth (Varyings input) : SV_Target {
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		//float2 positionSS = input.texcoord * _ScreenSize.xy;
		float2 positionSS = input.texcoord;

		float4 sky = float4(1, 1, 1, 1);//LOAD_TEXTURE2D(_Skybox, positionSS);		
		float4 tex = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, positionSS);
		
		// consider maximum radius

		half2 vec = (_SunPosition.xy - input.texcoord) * _BlurRadius4.xy;
  

		half dist = saturate (_SunPosition.w - length (vec));			

		half4 outColor = 0;		
		
		// find unoccluded sky pixels
		// consider pixel values that differ significantly between framebuffer and sky-only buffer as occluded
		if (Luminance ( abs(sky.rgb - tex.rgb)) < 0.2)
		{
			outColor = TransformColor (sky) * dist;
		}		
		return outColor;
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

            float4 fragScreen (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			float4 frag_radial (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			float4 frag_depth (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			float4 frag_nodepth (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			float4 fragAdd (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }
	#endif

		ENDHLSL
	
Subshader {
  Tags{ "RenderPipeline" = "HDRenderPipeline" }
 Pass {
	  ZTest Always Cull Off ZWrite Off Blend Off

      HLSLPROGRAM
      
      #pragma vertex Vert
      #pragma fragment fragScreen
      
      ENDHLSL
  }

        

 Pass {
	  ZTest Always Cull Off ZWrite Off Blend Off
       
      HLSLPROGRAM
      
      #pragma vertex Vert
      #pragma fragment frag_radial
      
      ENDHLSL
  }



  Pass {
	  ZTest Always Cull Off ZWrite Off Blend Off

      HLSLPROGRAM
         
      #pragma vertex Vert
      #pragma fragment frag_depth
       
      ENDHLSL
  }
  
  Pass {
	  ZTest Always Cull Off ZWrite Off Blend Off

      HLSLPROGRAM
      
      #pragma vertex Vert
      #pragma fragment frag_nodepth
      
      ENDHLSL
  } 
  
  Pass {
	  ZTest Always Cull Off ZWrite Off Blend Off

      HLSLPROGRAM
      
      #pragma vertex Vert
      #pragma fragment fragAdd
      
      ENDHLSL
  } 
}
	
} // shader
