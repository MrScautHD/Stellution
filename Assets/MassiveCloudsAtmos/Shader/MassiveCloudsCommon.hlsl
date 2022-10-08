#ifndef MASSIVE_CLOUDS_COMMON_INCLUDED
#define MASSIVE_CLOUDS_COMMON_INCLUDED

#include "Includes/PipelineDependent/PipelineDependent.hlsl"

sampler2D _MainTex;
half4     _MainTex_ST;
float4    _MainTex_TexelSize;

#ifdef MASSIVE_CLOUDS_HLSL

    struct appdata
    {
        float4 uv : TEXCOORD0;
        uint vertexID : SV_VertexID;
    };
                
    struct v2f
    {
        float4 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
    };

    v2f MassiveCloudsVert(appdata v)
    {
        v2f o;
        uint vertexID = v.vertexID;
        float2 uv = float2((vertexID << 1) & 2, vertexID & 2);
        float4 posCS = float4(uv * 2.0 - 1.0, 0.0, 1.0);
        #if UNITY_UV_STARTS_AT_TOP
        uv.y = 1.0 - uv.y;
        #endif
        o.vertex = posCS;
        #if _MASSIVE_CLOUDS_HDRP
        // HDRP
        o.uv = float4(uv / _RTHandleScale.xy, 0, 1);
        #else
        // Standard RP
        o.uv = float4(uv.xy, 0, 1);
        #endif
        return o;
    }

#else

    struct appdata
    {
        float4 vertex : POSITION;
        float4 uv : TEXCOORD0;
    };

    struct v2f
    {
        float4 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
    };

    float4 _RTHandleScale;        // { w / RTHandle.maxWidth, h / RTHandle.maxHeight } : xy = currFrame, zw = prevFrame
    float4 _RTHandleScaleHistory; // Same as above but the RTHandle handle size is that of the history buffer

    v2f MassiveCloudsVert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        #if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0) {
            o.uv.y = 1 - o.uv.y;
        }
        #endif
        return o;
    }

#endif


#if defined(USING_STEREO_MATRICES)
#define MC_TEX2DPROJ(texname, uv) tex2Dproj(texname, UnityStereoScreenSpaceUVAdjust(uv, texname##_ST))
#define MC_TEX2DPROJ_ST(texname, uv, stname) tex2Dproj(texname, UnityStereoScreenSpaceUVAdjust(uv, stname))
#else
#define MC_TEX2DPROJ(texname, uv) tex2Dproj(texname, uv)
#define MC_TEX2DPROJ_ST(texname, uv, stname) tex2Dproj(texname, uv)
#endif

#if defined(USING_STEREO_MATRICES)
#define MC_TEX2DLOD(texname, uv) tex2Dlod(texname, UnityStereoScreenSpaceUVAdjust(uv, texname##_ST))
#define MC_TEX2DLOD_ST(texname, uv, stname) tex2Dlod(texname, UnityStereoScreenSpaceUVAdjust(uv, stname))
#else
#define MC_TEX2DLOD(texname, uv) tex2Dlod(texname, uv)
#define MC_TEX2DLOD_ST(texname, uv, stname) tex2Dlod(texname, uv)
#endif

#endif