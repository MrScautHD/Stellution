// MODIFIED FOR ENVIRO POST PROCESSING
//
// Kino/Bloom v2 - Bloom filter for Unity
//  
// Copyright (C) 2015, 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.  
//         
Shader "Hidden/EnviroDistanceBlurHDRP"
{           
	Properties  
	{
		_MainTex("", Any) = "" {}
		_BaseTex("", Any) = "" {}
	} 
		SubShader  
	{   
Tags{ "RenderPipeline" = "HDRenderPipeline" }
		// 0: Prefilter
		Pass
	{
		ZTest Always Cull Off ZWrite Off
		HLSLPROGRAM
		#pragma multi_compile _ UNITY_COLORSPACE_GAMMA
		#pragma multi_compile __ ENVIROHDRP
		#include "/Core/EnviroBlurCore.hlsl"
		#pragma vertex Vert
		#pragma fragment frag_prefilter
		#pragma target 3.0
		ENDHLSL    
	} 
		// 1: Prefilter with anti-flicker
		Pass
	{  
		ZTest Always Cull Off ZWrite Off
		HLSLPROGRAM
		#define ANTI_FLICKER 1
		#pragma multi_compile _ UNITY_COLORSPACE_GAMMA
		#pragma multi_compile __ ENVIROHDRP
		#include "/Core/EnviroBlurCore.hlsl"
		#pragma vertex Vert
		#pragma fragment frag_prefilter
		#pragma target 3.0
		ENDHLSL   
	}
		// 2: First level downsampler
		Pass
	{     
		ZTest Always Cull Off ZWrite Off
		HLSLPROGRAM
		#pragma multi_compile __ ENVIROHDRP
		#include "/Core/EnviroBlurCore.hlsl"
		#pragma vertex Vert
		#pragma fragment frag_downsample1
		#pragma target 3.0
		ENDHLSL
	}  
		// 3: First level downsampler with anti-flicker
		Pass
	{
		ZTest Always Cull Off ZWrite Off
		HLSLPROGRAM
		#define ANTI_FLICKER 1
		#pragma multi_compile __ ENVIROHDRP
		#include "/Core/EnviroBlurCore.hlsl"
		#pragma vertex Vert
		#pragma fragment frag_downsample1
		#pragma target 3.0
		ENDHLSL
	}
		// 4: Second level downsampler
		Pass      
	{
		ZTest Always Cull Off ZWrite Off
		HLSLPROGRAM
		#pragma multi_compile __ ENVIROHDRP
		#include "/Core/EnviroBlurCore.hlsl"
		#pragma vertex Vert  
		#pragma fragment frag_downsample2
		#pragma target 3.0
		ENDHLSL
	}                    

		//MULTITex 
		 
		// 5: Upsampler
		Pass 
	{
		ZTest Always Cull Off ZWrite Off
		HLSLPROGRAM
		#pragma multi_compile __ ENVIROHDRP
		#include "/Core/EnviroBlurCore.hlsl"
		#pragma vertex Vert
		#pragma fragment frag_upsample 
		#pragma target 3.0
		ENDHLSL 
	}    
		// 6: High quality upsampler
		Pass
	{
		ZTest Always Cull Off ZWrite Off
		HLSLPROGRAM
		#define HIGH_QUALITY 1
		#pragma multi_compile __ ENVIROHDRP
		#include "/Core/EnviroBlurCore.hlsl"
		#pragma vertex Vert
		#pragma fragment frag_upsample
		#pragma target 3.0
		ENDHLSL
	}
		// 7: Combiner
		Pass
	{ 
		ZTest Always Cull Off ZWrite Off
		HLSLPROGRAM
		#pragma multi_compile __ ENVIROHDRP
		#pragma multi_compile _ UNITY_COLORSPACE_GAMMA
		#include "/Core/EnviroBlurCore.hlsl"
		#pragma vertex Vert
		#pragma fragment frag_upsample_final
		#pragma target 3.0
		ENDHLSL
	} 
		// 8: High quality combiner
		Pass
	{ 
		ZTest Always Cull Off ZWrite Off
		HLSLPROGRAM
		#pragma multi_compile __ ENVIROHDRP
		#define HIGH_QUALITY 1
		#pragma multi_compile _ UNITY_COLORSPACE_GAMMA
		#include "/Core/EnviroBlurCore.hlsl"
		#pragma vertex Vert
		#pragma fragment frag_upsample_final
		#pragma target 3.0
			ENDHLSL
	}
	}
}
