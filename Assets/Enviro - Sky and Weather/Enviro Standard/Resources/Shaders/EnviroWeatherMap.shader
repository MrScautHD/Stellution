Shader "Enviro/Standard/WeatherMap" {
	Properties {
		_Coverage ("Coverage", Range(0,1)) = 0.5
		_Tiling ("Tiling", Range(1,100)) = 10
	}
	SubShader {  
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass { 
		CGPROGRAM
	    #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
        #include "/Core/EnviroNoiseCore.cginc"
		#pragma target 3.0
		#pragma exclude_renderers gles 


	#define CLASSICPERLIN

		sampler2D _MainTex;

		   struct VertexInput {
  				half4 vertex : POSITION;
 				float2 uv : TEXCOORD0;	
            };

            struct VertexOutput {
           		float4 position : SV_POSITION;
 				float2 uv : TEXCOORD0;
            }; 
			          
            VertexOutput vert (VertexInput v) {
 			 	VertexOutput o;
 				o.position = UnityObjectToClipPos(v.vertex);				
 				o.uv = v.uv;
 				return o; 
            }       
 		     
 			float4x4 world_view_proj;

 			float _Coverage;  
			float _CloudsType;
			float _CoverageType;
 			int _Tiling;
 			float2 _WindDir;
			float2 _Location; 
 			float _AnimSpeedScale;
			float4 _LightingVariance;

			float set_range(float value, float low, float high) {
							return saturate((value - low)/(high - low));
			}    

			float remap(float value, float original_min, float original_max, float new_min, float new_max)
			{
  			  return new_min + saturate(((value - original_min) / (original_max - original_min)) * (new_max - new_min));
			}

			float dilate_perlin_worley(float p, float w, float x) {
				float curve = 0.75;
				if (x < 0.5) {
					x = x / 0.5;
					float n = p + w * x;
					return n * lerp(1, 0.5, pow(x, curve));
				}
				else {  
					x = (x - 0.5) / 0.5;
					float n = w + p * (1.0 - x);
					return n * lerp(0.5, 1.0, pow(x, 1.0 / curve));
				}        
			}  
		            

 			float4 frag(VertexInput input) : SV_Target 
 			{  
				float2 xy_offset = _WindDir * 10 * _AnimSpeedScale;
 				float2 xy_offset1 = xy_offset;
				float2 xy_offset2 = xy_offset + float2(50,100);
				float2 xy_offset3 = xy_offset + float2(100, 50);
				float2 xy_offset4 = xy_offset + float2(100, 500);
				                               
				float2 sampling_pos0 = float2(input.uv + xy_offset1 + _Location) * _Tiling;
				float2 sampling_pos01 = float2(input.uv + xy_offset4 + _Location) * _Tiling;
				float2 sampling_pos02 = float2(input.uv + xy_offset2 + _Location) * 2 * _Tiling;
				float2 sampling_pos03 = float2(input.uv + xy_offset3 + _Location) * _LightingVariance.y * _Tiling;
				  
				float perlinT1 = saturate(CalculatePerlinTileing5(sampling_pos0.xy,float2(_Tiling, _Tiling)));
				float perlinT2 = saturate(CalculatePerlinTileing5(sampling_pos01.xy, float2(_Tiling, _Tiling)));
				float perlinT3 = saturate(CalculatePerlinTileing5(sampling_pos02.xy, float2(_Tiling, _Tiling)));
		                 
				float perlinT = perlinT1 + saturate(perlinT2 - perlinT1);
				float worleyT = CalculateWorley3oct(sampling_pos0.xy, 1.5, 2, 2.5);
				  
				//float worley2 = saturate( pow( CalculateWorley1(sampling_pos0.xy, 5),0.5));

				float perlin_worleyCov = dilate_perlin_worley(perlinT3, worleyT, 0.6);

				float perlin_worley = dilate_perlin_worley(perlinT, perlin_worleyCov, _CoverageType);

				float worleyFull = saturate((CalculateWorley1(sampling_pos01.xy, 4) - perlin_worley));

				//float perlin_worleyFull = dilate_perlin_worley(perlinT3, worleyFull, 0.75);
				float coverage = perlin_worley + (_Coverage * worleyFull); 
				//float coverage = perlin_worley * worley2 + (_Coverage * worleyFull);

				if (_Coverage < 0)
					coverage += _Coverage;

				float type = (coverage + (_Coverage - perlin_worley) * (1-_CloudsType)) * _CloudsType;
				/////

				//Clouds Lighting Variance
				float perlinT4 = saturate(CalculatePerlinTileing5(sampling_pos03.xy, float2(_Tiling, _Tiling)));

				perlinT4 = clamp(perlinT4, _LightingVariance.x, 1);

				return float4(coverage, perlinT4, type, 0);
			}

	ENDCG
	}
	}
	FallBack "Diffuse"
}
