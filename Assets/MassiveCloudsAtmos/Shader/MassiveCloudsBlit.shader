Shader "MassiveCloudsBlit"
{
	Properties
	{
	}

    CGINCLUDE
            sampler2D _MainTex;
            half4     _MainTex_ST;
            sampler2D _MassiveCloudsResultTexture;

            #if _MASSIVE_CLOUDS_HDRP
            float4 _RTHandleScale;        // { w / RTHandle.maxWidth, h / RTHandle.maxHeight } : xy = currFrame, zw = prevFrame
            float4 _RTHandleScaleHistory; // Same as above but the RTHandle handle size is that of the history buffer
            #endif

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

            v2f Vert(appdata v)
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
            
            float4 Frag(v2f i) : SV_Target
            {
                return tex2Dproj(_MassiveCloudsResultTexture, i.uv);
            }
    ENDCG

	SubShader
	{
		Pass
		{
    		Cull Off ZWrite Off ZTest Always
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			
			#pragma shader_feature _MASSIVE_CLOUDS_HDRP

			ENDCG
		}
		Pass
		{
    		Cull Off ZWrite Off ZTest LEqual
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			
			#pragma shader_feature _MASSIVE_CLOUDS_HDRP

			ENDCG
		}
	}
}
