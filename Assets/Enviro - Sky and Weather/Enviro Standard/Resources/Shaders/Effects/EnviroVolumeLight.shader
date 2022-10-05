//  Copyright(c) 2016, Michal Skalsky
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification,
//  are permitted provided that the following conditions are met:
//
//  1. Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
// 
//  2. Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
// 
//  3. Neither the name of the copyright holder nor the names of its contributors
//     may be used to endorse or promote products derived from this software without
//     specific prior written permission.
//      
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
//  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
//  OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.IN NO EVENT
//  SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
//  OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
//  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
//  TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
//  EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
  
//Modified for use with Enviro - Sky and Weather
  
   

Shader "Enviro/Standard/VolumeLight"
{
	Properties
	{	_MainTex("Texture", any) = "" {}
		[HideInInspector]_ZTest ("ZTest", Float) = 0
		[HideInInspector]_LightColor("_LightColor", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGINCLUDE
		#if defined(SHADOWS_DEPTH) || defined(SHADOWS_CUBE)
		#define SHADOWS_NATIVE
		#endif
		
		#include "UnityCG.cginc"
		#include "UnityDeferredLibrary.cginc"
		#include "../Core/EnviroVolumeLightCore.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float4 uv : TEXCOORD0;
			float3 wpos : TEXCOORD1;
			float4 interpolatedRay : TEXCOORD2;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		v2f vert(appdata v)
		{
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v); //Insert
			UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Ins
    		if (unity_StereoEyeIndex == 0)
				o.pos = mul(_WorldViewProj, v.vertex);
			else  
				o.pos = mul(_WorldViewProj_SP, v.vertex);

			o.uv = ComputeScreenPos(o.pos);
			o.wpos = mul(unity_ObjectToWorld, v.vertex);

			return o;
		}

		ENDCG
		// pass 0 - point light, camera inside
		Pass
		{
			ZTest Off
			Cull Front
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragPointInside
			#pragma target 3.5
			#pragma exclude_renderers d3d9 gles 

			#define UNITY_HDR_ON

			#pragma shader_feature HEIGHT_FOG
			#pragma shader_feature NOISE
			#pragma shader_feature SHADOWS_CUBE
			#pragma shader_feature POINT_COOKIE
			#pragma shader_feature POINT

			#ifdef SHADOWS_DEPTH
			#define SHADOWS_NATIVE
			#endif
						
			
			fixed4 fragPointInside(v2f i) : SV_Target
			{	
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				float2 uv = i.uv.xy / i.uv.w;

#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				if(unity_StereoEyeIndex != _Eye)
				   return float4(0,0,0,0);
#endif

				// read depth and reconstruct world position
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(uv,_CameraDepthTexture_ST));

				float3 rayStart = _WorldSpaceCameraPos;
				float3 rayEnd = i.wpos;

				float3 rayDir = (rayEnd - rayStart);
				float rayLength = length(rayDir);

				rayDir /= rayLength;

				float linearDepth = LinearEyeDepth(depth);
				float projectedDepth = linearDepth / dot(_CameraForward, rayDir);
				rayLength = min(rayLength, projectedDepth);
				
				return RayMarch(i.pos.xy, rayStart, rayDir, rayLength);
			}
			ENDCG
		}

		// pass 1 - spot light, camera inside
		Pass
		{
			ZTest Off
			Cull Front
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragSpotInside
			#pragma target 3.5
			#pragma exclude_renderers d3d9 gles

			#define UNITY_HDR_ON

			#pragma shader_feature HEIGHT_FOG
			#pragma shader_feature NOISE
			#pragma shader_feature SHADOWS_DEPTH
			#pragma shader_feature SPOT

			#ifdef SHADOWS_DEPTH
			#define SHADOWS_NATIVE
			#endif

			fixed4 fragSpotInside(v2f i) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				float2 uv = i.uv.xy / i.uv.w;

#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				if(unity_StereoEyeIndex != _Eye)
				   return float4(0,0,0,0);
#endif

				// read depth and reconstruct world position
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(uv,_CameraDepthTexture_ST));

				float3 rayStart = _WorldSpaceCameraPos;
				float3 rayEnd = i.wpos;

				float3 rayDir = (rayEnd - rayStart);
				float rayLength = length(rayDir);

				rayDir /= rayLength;

				float linearDepth = LinearEyeDepth(depth);
				float projectedDepth = linearDepth / dot(_CameraForward, rayDir);
				rayLength = min(rayLength, projectedDepth);

				return RayMarch(i.pos.xy, rayStart, rayDir, rayLength);
			}
			ENDCG
		}

		// pass 2 - point light, camera outside
		Pass
		{
			//ZTest Off
			ZTest [_ZTest]
			Cull Back
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragPointOutside
			#pragma target 3.5
			#pragma exclude_renderers d3d9 gles

			#define UNITY_HDR_ON

			#pragma shader_feature HEIGHT_FOG
			#pragma shader_feature SHADOWS_CUBE
			#pragma shader_feature NOISE
			#pragma shader_feature POINT_COOKIE
			#pragma shader_feature POINT

			fixed4 fragPointOutside(v2f i) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				float2 uv = i.uv.xy / i.uv.w;

#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				if(unity_StereoEyeIndex != _Eye)
				   return float4(0,0,0,0);
#endif

				// read depth and reconstruct world position
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(uv,_CameraDepthTexture_ST));
			
				float3 rayStart = _WorldSpaceCameraPos;
				float3 rayEnd = i.wpos;

				float3 rayDir = (rayEnd - rayStart);
				float rayLength = length(rayDir);

				rayDir /= rayLength;

				float3 lightToCamera = _WorldSpaceCameraPos - _LightPos;

				float b = dot(rayDir, lightToCamera);
				float c = dot(lightToCamera, lightToCamera) - (_VolumetricLight.z * _VolumetricLight.z);

				float d = sqrt((b*b) - c);
				float start = -b - d;
				float end = -b + d;

				float linearDepth = LinearEyeDepth(depth);
				float projectedDepth = linearDepth / dot(_CameraForward, rayDir);
				end = min(end, projectedDepth);

				rayStart = rayStart + rayDir * start;
				rayLength = end - start;

				return RayMarch(i.pos.xy, rayStart, rayDir, rayLength);
			}
			ENDCG
		}
				
		// pass 3 - spot light, camera outside
		Pass
		{
			//ZTest Off
			ZTest[_ZTest]
			Cull Back
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragSpotOutside
			#pragma target 3.5
			#pragma exclude_renderers d3d9 gles

			#define UNITY_HDR_ON

			#pragma shader_feature HEIGHT_FOG
			#pragma shader_feature SHADOWS_DEPTH
			#pragma shader_feature NOISE
			#pragma shader_feature SPOT

			#ifdef SHADOWS_DEPTH
			#define SHADOWS_NATIVE
			#endif
			
			float _CosAngle;
			float4 _ConeAxis;
			float4 _ConeApex;
			float _PlaneD;

			fixed4 fragSpotOutside(v2f i) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				float2 uv = i.uv.xy / i.uv.w;

#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				if(unity_StereoEyeIndex != _Eye)
				   return float4(0,0,0,0);
#endif

				// read depth and reconstruct world position
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(uv,_CameraDepthTexture_ST));

				float3 rayStart = _WorldSpaceCameraPos;
				float3 rayEnd = i.wpos;

				float3 rayDir = (rayEnd - rayStart);
				float rayLength = length(rayDir);

				rayDir /= rayLength;


				// inside cone
				float3 r1 = rayEnd + rayDir * 0.001;

				// plane intersection
				float planeCoord = RayPlaneIntersect(_ConeAxis, _PlaneD, r1, rayDir);
				// ray cone intersection
				float2 lineCoords = RayConeIntersect(_ConeApex, _ConeAxis, _CosAngle, r1, rayDir);

				float linearDepth = LinearEyeDepth(depth);
				float projectedDepth = linearDepth / dot(_CameraForward, rayDir);

				float z = (projectedDepth - rayLength);
				rayLength = min(planeCoord, min(lineCoords.x, lineCoords.y));
				rayLength = min(rayLength, z);

				return RayMarch(i.pos.xy, rayEnd, rayDir, rayLength);
			}
				ENDCG
		}

			// pass 4 - directional light
				Pass
			{
				ZTest Off
				Cull Off
				ZWrite Off
				Blend One One, One Zero

				CGPROGRAM

				#pragma vertex vertDir
				#pragma fragment fragDir
				#pragma target 3.5
				#pragma exclude_renderers d3d9 gles

				#define UNITY_HDR_ON

				#pragma shader_feature HEIGHT_FOG
				#pragma shader_feature NOISE
				#pragma shader_feature SHADOWS_DEPTH
				#pragma shader_feature DIRECTIONAL_COOKIE
				#pragma shader_feature DIRECTIONAL

				#ifdef SHADOWS_DEPTH
				#define SHADOWS_NATIVE
				#endif

			v2f vertDir(appdata_img v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v); 
				UNITY_INITIALIZE_OUTPUT(v2f, o); 
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.pos = UnityObjectToClipPos(v.vertex);
				//o.pos = v.vertex * float4(2,2,1,1) + float4(-1,-1,0,0);
				o.uv.xy = v.texcoord.xy;
#if UNITY_UV_STARTS_AT_TOP
				//o.uv.y = 1.0f - o.uv.y; //blit flips the uv for some reason
#endif
				return o; 
			}

			fixed4 fragDir(v2f i) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				float2 uv = i.uv.xy;

				//return lerp(float4(1,0,0,1),float4(0,0,1,1),unity_StereoEyeIndex);
 

				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);

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

				//bit of matrix math to take the screen space coord (u,v,depth) and transform to world space
				float2 uvClip = i.uv * 2.0 - 1.0;
				float clipDepth = depth; // Fix for OpenGl Core thanks to Lars Bertram
				clipDepth = (UNITY_NEAR_CLIP_VALUE < 0) ? clipDepth * 2 - 1 : clipDepth;
				float4 clipPos = float4(uvClip, clipDepth, 1.0);
				float4 viewPos = mul(proj, clipPos); // inverse projection by clip position
				viewPos /= viewPos.w; // perspective division
				float3 wpos = mul(eyeToWorld, viewPos).xyz;


				//float3 wpos = i.interpolatedRay + _WorldSpaceCameraPos;

				float linearDepth = Linear01Depth(depth);

				float3 rayStart = _WorldSpaceCameraPos;
				float3 rayDir = wpos - _WorldSpaceCameraPos;
				
				//rayDir *= linearDepth;

				float rayLength = length(rayDir);
				rayDir /= rayLength;

				rayLength = min(rayLength, _MaxRayLength);

				float4 color = RayMarch(i.pos.xy, rayStart, rayDir, rayLength);


				if (linearDepth > 0.999999)
				{
					color.w = lerp(color.w, 1, _VolumetricLight.w);
				}

				return color;
			}
				ENDCG
			}

		// pass 5 - black
			Pass
			{
				ZTest Off
				Cull Off
				ZWrite Off
				Blend One One, One Zero

				CGPROGRAM

				#pragma vertex vertW
				#pragma fragment fragW
				#pragma target 3.5
				#pragma exclude_renderers d3d9 gles

				#ifdef SHADOWS_DEPTH
				#define SHADOWS_NATIVE
				#endif


			v2f vertW(appdata_img i)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(i); //Insert
				UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Ins
				half index = i.vertex.z;
				i.vertex.z = 0.1;
				o.pos = UnityObjectToClipPos(i.vertex);
				o.uv.xy = i.texcoord.xy;
				return o;
			}

			fixed4 fragW(v2f i) : SV_Target
			{
				return float4(0,0,0,0);
			}
				ENDCG
			}
	}
}
