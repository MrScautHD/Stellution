Shader "Enviro/Lite/SkyboxSimple"
{
    Properties
    {
     	_SkyColor ("Sky Color", Color) = (0, 0, 0, 0)
     	_HorizonColor ("Horizon Color", Color) = (0, 0, 0, 0)
        _SunColor ("Sun Color", Color) = (0, 0, 0, 0)
		_Stars ("StarsMap", Cube) = "black" {}
		_MoonTex("Moon Tex", 2D) = "black" {}
		_FlatCloudsBaseTexture("Base Map", 2D) = "black" {}
		_FlatCloudsDetailTexture("Detail Map", 2D) = "black" {}
    }
	
    SubShader
    {
		Lod 300
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "IgnoreProjector"="True" }
		
        Pass
        {
            Cull Back
            ZWrite Off
		 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			#pragma target 3.0 
			//#pragma multi_compile_fog

			uniform half4 _SkyColor;
			uniform half4 _HorizonColor;
			uniform half4 _SunColor;
			uniform samplerCUBE _Stars;			
			uniform float4x4 _StarsMatrix;
			uniform half _StarsIntensity;
			uniform half _SunDiskSizeSimple;
			uniform float4 _weatherSkyMod;
			uniform half _BlackGround;
			uniform float3 _SunDir;
			uniform sampler2D _MoonTex;
			uniform float3 _MoonDir;
			uniform float4 _MoonColor;
			uniform float4 _moonParams;
		  
			struct VertexInput 
             {
                float4 vertex : POSITION;
                float3 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
             };


            struct v2f {
                float4 position : POSITION;
                float4 WorldPosition : TEXCOORD0;
                float3 starPos : TEXCOORD1;
				half3 vertex : TEXCOORD2;
				float3 moonPos : TEXCOORD3;
				UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(VertexInput v) {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v); //Insert
				UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Ins
				float3 viewDir = normalize(v.texcoord + float3(0.0, 0.1, 0.0));

                o.position  = UnityObjectToClipPos(v.vertex);
				o.WorldPosition = normalize(mul((float4x4)unity_ObjectToWorld, v.vertex)).xyzw;
				o.starPos = mul((float3x3)_StarsMatrix,v.vertex.xyz);
				o.vertex = -v.vertex;
				float3 r = normalize(cross(_MoonDir.xyz, float3(0, -1, 0)));
				float3 u = cross(_MoonDir.xyz, r);
				o.moonPos.xy = float2(dot(r, v.vertex.xyz), dot(u, v.vertex.xyz)) * (21.0 - _moonParams.x) + 0.5;
				o.moonPos.z = saturate(dot(-_MoonDir.xyz, viewDir));
				
                return o;
            }


			float MoonPhaseFactor(float2 uv, float phase)
			{
				float alpha = 1.0;


				float srefx = uv.x - 0.5;
				float refx = abs(uv.x - 0.5);

				if (phase > 0)
				{
					srefx = (1 - uv.x) - 0.5;
					refx = abs((1 - uv.x) - 0.5);
				}

				phase = abs(_moonParams.w);
				float refy = abs(uv.y - 0.5);
				float refxfory = sqrt(0.25 - refy * refy);
				float xmin = -refxfory;
				float xmax = refxfory;
				float xmin1 = (xmax - xmin) * (phase / 2) + xmin;
				float xmin2 = (xmax - xmin) * phase + xmin;

				if (srefx < xmin1)
				{
					alpha = 0;
				}
				else if (srefx < xmin2 && xmin1 != xmin2)
				{
					alpha = (srefx - xmin1) / (xmin2 - xmin1);
				}

				return alpha;
			}

			half getMiePhase(half eyeCos, half eyeCos2, half y)
			{
				half temp = 1.0 + 0.9801 - 2.0 * (-0.990) * eyeCos;
				temp = pow(temp, pow(_SunDiskSizeSimple, 0.65) * 10);
				temp = max(temp, 1.0e-4); // prevent division by zero, esp. in half precision
				temp = 1.5 * ((1.0 - 0.9801) / (2.0 + 0.9801)) * (1.0 + eyeCos2) / temp;
//#if defined(UNITY_COLORSPACE_GAMMA) && SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
//				temp = pow(temp, .454545);
//#endif
				return temp;
			}

            fixed4 frag(v2f i) : COLOR 
            {
				half3 ray = normalize(mul((float3x3)unity_ObjectToWorld, i.vertex));
				half y = ray.y / 0.02;
				float4 skyColor = float4(0, 0, 0, 1);

				if(_BlackGround == 1.0 && y > 5.0)
				   skyColor = float4(0, 0, 0, 1);
				else
				{		
            		float3 viewDir = normalize(i.WorldPosition + float3(0,0.2,0));

					float4 moonSampler = tex2D(_MoonTex, i.moonPos.xy);
					float alpha = MoonPhaseFactor(i.moonPos.xy, _moonParams.w);
					float3 moonArea = clamp(moonSampler * 10, 0, 1) * i.moonPos.z;
					moonSampler = lerp(float4(0, 0, 0, 0), moonSampler, alpha);
					moonSampler = (moonSampler * _MoonColor) * 2;
					float starsBehindMoon = 1 - clamp((moonArea * 5), 0, 1);

            		float3 starsMap = texCUBE(_Stars, i.starPos.xyz);
					float4 nightSky = float4(((_StarsIntensity * 50) * starsMap.rgb),1) * starsBehindMoon;
					skyColor = lerp(_HorizonColor,_SkyColor,smoothstep(dot(viewDir.y, float3(0,2,0)),0,0.3));

					if (y < 50.0 && y > 5.0)
						skyColor = _HorizonColor;

					skyColor = skyColor + (1 - skyColor.a) * nightSky;
			
					half eyeCos = dot(_SunDir, ray);
					half eyeCos2 = eyeCos * eyeCos;
					half mie = getMiePhase(eyeCos, eyeCos2,y);
					skyColor += mie * _SunColor;

					skyColor.rgb += (moonSampler.rgb * i.moonPos.z);

					skyColor = lerp(skyColor, (lerp(skyColor, _weatherSkyMod, _weatherSkyMod.a)), _weatherSkyMod.a);
				}

                return skyColor;
            }
            ENDCG
        }

			//Cirrus Clouds
			Pass
			{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _CloudMap;
			uniform float _CloudAlpha;
			uniform float _CloudCoverage;
			uniform float _CloudAltitude;
			uniform float4 _CloudColor;
			uniform float _CloudColorPower;
			uniform float2 _CloudAnimation;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 Position : SV_POSITION;
				float4 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v); //Insert
				UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Ins

				o.Position = UnityObjectToClipPos(v.vertex);
				o.worldPos = normalize(v.vertex).xyz;
				float3 viewDir = normalize(o.worldPos + float3(0,1,0));
				o.worldPos.y *= 1 - dot(viewDir.y + _CloudAltitude, float3(0,-0.15,0));
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float3 uvs = normalize(i.worldPos);

				float4 uv1;
				float4 uv2;

				uv1.xy = (uvs.xz * 0.2) + _CloudAnimation;
				uv2.xy = (uvs.xz * 0.4) + _CloudAnimation;

				float4 clouds1 = tex2D(_CloudMap, uv1.xy);
				float4 clouds2 = tex2D(_CloudMap, uv2.xy);

				float color1 = pow(clouds1.g + clouds2.g, 0.1);
				float color2 = pow(clouds2.b * clouds1.r, 0.2);

				float4 finalClouds = lerp(clouds1, clouds2, color1 * color2);
				float cloudExtinction = pow(uvs.y , 2);


				finalClouds.a *= _CloudAlpha;
				finalClouds.a *= cloudExtinction;

				if (uvs.y < 0)
					finalClouds.a = 0;

				finalClouds.rgb = finalClouds.a * pow(_CloudColor,_CloudColorPower);
				finalClouds.rgb = pow(finalClouds.rgb,1 - _CloudCoverage);

				return finalClouds;
			}
				ENDCG
			}
		


			//Flat Clouds
				Pass
			{
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

				uniform sampler2D _FlatCloudsBaseTexture;
			uniform sampler2D _FlatCloudsDetailTexture;
			uniform float4 _FlatCloudsAnimation;
			uniform float3 _FlatCloudsLightDirection;
			uniform float3 _FlatCloudsLightColor;
			uniform float3 _FlatCloudsAmbientColor;
			uniform float4 _FlatCloudsLightingParams; // x = LightIntensity, y = AmbientIntensity, z = Absorbtion, w = HgPhase
			uniform float4 _FlatCloudsParams; // x = Coverage, y = Density, z = Altitude, w = tonemapping
			uniform float4 _FlatCloudsTiling; // x = Base, y = Detail
			uniform float _CloudsExposure;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 Position : SV_POSITION;
				float4 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.Position = UnityObjectToClipPos(v.vertex);
				o.uv = normalize(v.vertex).xyzw;
				float3 viewDir = normalize(o.uv + float3(0, 1, 0));
				o.uv.y *= 1 - dot(viewDir.y + _FlatCloudsParams.z, float3(0, -0.2, 0));
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			float Remap(float org_val, float org_min, float org_max, float new_min, float new_max)
			{
				return new_min + saturate(((org_val - org_min) / (org_max - org_min))*(new_max - new_min));
			}

			float HenryGreenstein(float cosTheta, float g)
			{
				float k = 3.0 / (8.0 * 3.1415926f) * (1.0 - g * g) / (2.0 + g * g);
				return k * (1.0 + cosTheta * cosTheta) / pow(abs(1.0 + g * g - 2.0 * g * cosTheta), 1.5);
			}
			half3 tonemapACES(half3 color, float Exposure)
			{
				color *= Exposure;

				// See https://knarkowicz.wordpress.com/2016/01/06/aces-filmic-tone-mapping-curve/
				const half a = 2.51;
				const half b = 0.03;
				const half c = 2.43;
				const half d = 0.59;
				const half e = 0.14;
				return saturate((color * (a * color + b)) / (color * (c * color + d) + e));
			}

			float CalculateCloudDensity(float2 posBase, float2 posDetail, float coverage)
			{
				float4 baseNoise = tex2D(_FlatCloudsBaseTexture, posBase);
				float low_freq_fBm = (baseNoise.g * 0.625) + (baseNoise.b * 0.25) + (baseNoise.a * 0.125);
				float base_cloud = Remap(baseNoise.r, -(1.0 - low_freq_fBm), 1.0, 0.0, 1.0) * coverage;

				float4 detailNoise = tex2D(_FlatCloudsDetailTexture, posDetail * 2);
				float high_freq_fBm = (detailNoise.r * 0.625) + (detailNoise.g * 0.25) + (detailNoise.b * 0.125);
				float density = Remap(base_cloud, 1.0 - high_freq_fBm * 0.5, 1.0, 0.0, 1.0);

				density *= pow(high_freq_fBm, 0.4);
				density *= _FlatCloudsParams.y;

				return density;
			}

			half4 frag(v2f i) : SV_Target
			{
				half4 col = 0;
				float3 uvs = normalize(i.uv);

				float4 uv1;
				uv1.xy = (uvs.xz * _FlatCloudsTiling.x) + _FlatCloudsAnimation.xy;
				uv1.zw = (uvs.xz * _FlatCloudsTiling.y) + _FlatCloudsAnimation.zw;

				float cloudExtinction = pow(uvs.y, 2);
				half density = CalculateCloudDensity(uv1.xy, uv1.zw, _FlatCloudsParams.x);

				//Lighting	
				fixed absorbtion = exp2(-1 * (density * _FlatCloudsLightingParams.z));
				float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
				float inscatterAngle = dot(normalize(_FlatCloudsLightDirection), -viewDir);
				fixed hg = HenryGreenstein(inscatterAngle, _FlatCloudsLightingParams.w) * 2 * absorbtion;
				fixed lighting = density * (absorbtion + hg);
				float3 lightColor = pow(_FlatCloudsLightColor, 2) * (_FlatCloudsLightingParams.x);
				col.rgb = lightColor * lighting;
				col.rgb = col.rgb + (_FlatCloudsAmbientColor * _FlatCloudsLightingParams.y);

				//Tonemapping
				if (_FlatCloudsParams.w == 1)
					col.rgb = tonemapACES(col.rgb, _CloudsExposure);

				col.a = saturate(density * cloudExtinction);

				if (uvs.y < 0)
					col.a = 0;

				return col;
			}
				ENDCG
			}

    }
    FallBack Off
}
