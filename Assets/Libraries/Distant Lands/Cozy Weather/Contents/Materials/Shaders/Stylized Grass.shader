// Upgrade NOTE: upgraded instancing buffer 'DistantLandsCozyStylizedGrass' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/Stylized Grass"
{
	Properties
	{
		_GrassTexture("Grass Texture", 2D) = "white" {}
		_AlphaClip("Alpha Clip", Float) = 0
		_TopColor("Top Color", Color) = (0.359336,0.8018868,0.5062882,0)
		_BottomColor("Bottom Color", Color) = (0.359336,0.8018868,0.5062882,0)
		_GradientAmount("Gradient Amount", Float) = 0
		_WindScale("Wind Scale", Float) = 0
		_WindSpeed("Wind Speed", Float) = 0
		_WindStrength("Wind Strength", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
			float4 vertexColor : COLOR;
		};

		uniform float _WindSpeed;
		uniform float _WindScale;
		uniform float4 CZY_SnowColor;
		uniform sampler2D CZY_SnowTexture;
		uniform float4 _BottomColor;
		uniform float CZY_SnowScale;
		uniform float CZY_SnowAmount;
		uniform float4 _TopColor;
		uniform float _GradientAmount;
		uniform sampler2D _GrassTexture;
		uniform float _AlphaClip;

		UNITY_INSTANCING_BUFFER_START(DistantLandsCozyStylizedGrass)
			UNITY_DEFINE_INSTANCED_PROP(float4, CZY_SnowTexture_ST)
#define CZY_SnowTexture_ST_arr DistantLandsCozyStylizedGrass
			UNITY_DEFINE_INSTANCED_PROP(float4, _GrassTexture_ST)
#define _GrassTexture_ST_arr DistantLandsCozyStylizedGrass
			UNITY_DEFINE_INSTANCED_PROP(float3, _WindStrength)
#define _WindStrength_arr DistantLandsCozyStylizedGrass
		UNITY_INSTANCING_BUFFER_END(DistantLandsCozyStylizedGrass)


		inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }

		inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }

		inline float valueNoise (float2 uv)
		{
			float2 i = floor(uv);
			float2 f = frac( uv );
			f = f* f * (3.0 - 2.0 * f);
			uv = abs( frac(uv) - 0.5);
			float2 c0 = i + float2( 0.0, 0.0 );
			float2 c1 = i + float2( 1.0, 0.0 );
			float2 c2 = i + float2( 0.0, 1.0 );
			float2 c3 = i + float2( 1.0, 1.0 );
			float r0 = noise_randomValue( c0 );
			float r1 = noise_randomValue( c1 );
			float r2 = noise_randomValue( c2 );
			float r3 = noise_randomValue( c3 );
			float bottomOfGrid = noise_interpolate( r0, r1, f.x );
			float topOfGrid = noise_interpolate( r2, r3, f.x );
			float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
			return t;
		}


		float SimpleNoise(float2 UV)
		{
			float t = 0.0;
			float freq = pow( 2.0, float( 0 ) );
			float amp = pow( 0.5, float( 3 - 0 ) );
			t += valueNoise( UV/freq )*amp;
			freq = pow(2.0, float(1));
			amp = pow(0.5, float(3-1));
			t += valueNoise( UV/freq )*amp;
			freq = pow(2.0, float(2));
			amp = pow(0.5, float(3-2));
			t += valueNoise( UV/freq )*amp;
			return t;
		}


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float2 voronoihash5_g2( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi5_g2( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash5_g2( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F1;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 _WindStrength_Instance = UNITY_ACCESS_INSTANCED_PROP(_WindStrength_arr, _WindStrength);
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime11 = _Time.y * 3.0;
			float2 uv_TexCoord17 = v.texcoord.xy + ( ase_worldPos + ( _WindSpeed * mulTime11 * 3.0 ) ).xy;
			float simpleNoise21 = SimpleNoise( uv_TexCoord17*_WindScale );
			simpleNoise21 = simpleNoise21*2 - 1;
			float4 transform25 = mul(unity_WorldToObject,float4( ( _WindStrength_Instance * simpleNoise21 * v.color.r ) , 0.0 ));
			v.vertex.xyz += transform25.xyz;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 CZY_SnowTexture_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(CZY_SnowTexture_ST_arr, CZY_SnowTexture_ST);
			float2 uvCZY_SnowTexture = i.uv_texcoord * CZY_SnowTexture_ST_Instance.xy + CZY_SnowTexture_ST_Instance.zw;
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_worldPos = i.worldPos;
			float2 appendResult3_g2 = (float2(ase_worldPos.x , ase_worldPos.z));
			float temp_output_6_0_g2 = ( 1.0 / CZY_SnowScale );
			float simplePerlin2D7_g2 = snoise( appendResult3_g2*temp_output_6_0_g2 );
			simplePerlin2D7_g2 = simplePerlin2D7_g2*0.5 + 0.5;
			float time5_g2 = 0.0;
			float2 voronoiSmoothId0 = 0;
			float2 coords5_g2 = appendResult3_g2 * ( temp_output_6_0_g2 / 0.1 );
			float2 id5_g2 = 0;
			float2 uv5_g2 = 0;
			float voroi5_g2 = voronoi5_g2( coords5_g2, time5_g2, id5_g2, uv5_g2, 0, voronoiSmoothId0 );
			float4 lerpResult19_g2 = lerp( ( CZY_SnowColor * tex2D( CZY_SnowTexture, uvCZY_SnowTexture ) ) , _BottomColor , ( ( pow( ( pow( ase_worldNormal.y , 7.0 ) * ( simplePerlin2D7_g2 * ( 1.0 - voroi5_g2 ) ) ) , 0.5 ) * 1.0 ) > ( 1.0 - CZY_SnowAmount ) ? 0.0 : 1.0 ));
			float4 lerpResult40 = lerp( lerpResult19_g2 , _TopColor , saturate( ( i.vertexColor * _GradientAmount ) ));
			float4 _GrassTexture_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_GrassTexture_ST_arr, _GrassTexture_ST);
			float2 uv_GrassTexture = i.uv_texcoord * _GrassTexture_ST_Instance.xy + _GrassTexture_ST_Instance.zw;
			float4 tex2DNode29 = tex2D( _GrassTexture, uv_GrassTexture );
			clip( tex2DNode29.a - _AlphaClip);
			o.Albedo = ( lerpResult40 * tex2DNode29 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18912
0;1080;2194;606;2141.973;873.5466;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;35;-1782.613,66.60786;Inherit;False;1507.094;1005.034;Use a noise generator to generate wind;17;37;36;25;18;24;21;23;22;19;20;17;16;15;11;12;13;58;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1703.651,485.5193;Inherit;False;Constant;_FlutterMultiplier;Flutter Multiplier;12;0;Create;True;0;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;11;-1682.79,390.8949;Inherit;False;1;0;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1677.037,300.8261;Inherit;False;Property;_WindSpeed;Wind Speed;8;0;Create;True;0;0;0;False;0;False;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;58;-1636.008,121.7565;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1451.369,330.3996;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-1306.666,257.3826;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;17;-1152.312,252.0274;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;18;-928.2369,387.4195;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;38;-1590.463,-897.5292;Inherit;False;1209.692;839.5466;Color and texture;12;45;32;27;33;40;29;39;1;48;54;60;64;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1467.443,-339.6823;Inherit;False;Property;_GradientAmount;Gradient Amount;6;0;Create;True;0;0;0;False;0;False;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;19;-1271.59,418.6251;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1497.601,718.4606;Inherit;False;Property;_WindScale;Wind Scale;7;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;54;-1535.582,-506.6434;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;22;-1173.989,877.2496;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-1443.419,-764.3652;Inherit;False;Property;_BottomColor;Bottom Color;5;0;Create;True;0;0;0;False;0;False;0.359336,0.8018868,0.5062882,0;0.2391438,0.3768399,0.338181,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1324.15,-473.8273;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;23;-1194.742,713.3535;Inherit;False;InstancedProperty;_WindStrength;Wind Strength;9;0;Create;True;0;0;0;False;0;False;0,0,0;0.2,0,0.2;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NoiseGeneratorNode;21;-1197.072,463.4114;Inherit;True;Simple;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;48;-1036.834,-464.5709;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-799.1182,711.2886;Inherit;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;39;-1132.697,-672.6563;Inherit;False;Property;_TopColor;Top Color;4;0;Create;True;0;0;0;False;0;False;0.359336,0.8018868,0.5062882,0;0.2634151,0.4653729,0.3827902,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;66;-1197.016,-797.7238;Inherit;False;Stylized Snow Blend;1;;2;359b695eb7172584f9df5a0d55bd52e9;0;2;34;FLOAT;1;False;22;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;40;-899.3847,-665.2943;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;36;-585.0059,693.1005;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;29;-1101.511,-315.3161;Inherit;True;Property;_GrassTexture;Grass Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;644c93aeaa02b8f428826fab1464148a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-762.3417,-338.0333;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-752.1147,-180.6272;Inherit;False;Property;_AlphaClip;Alpha Clip;3;0;Create;True;0;0;0;False;0;False;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;37;-673.2333,541.8539;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClipNode;32;-583.4438,-325.1021;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;25;-570.4775,398.1627;Inherit;True;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;;0;0;Standard;Distant Lands/Cozy/Stylized Grass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;0;13;0
WireConnection;15;1;11;0
WireConnection;15;2;12;0
WireConnection;16;0;58;0
WireConnection;16;1;15;0
WireConnection;17;1;16;0
WireConnection;18;0;17;0
WireConnection;19;0;18;0
WireConnection;60;0;54;0
WireConnection;60;1;45;0
WireConnection;21;0;19;0
WireConnection;21;1;20;0
WireConnection;48;0;60;0
WireConnection;24;0;23;0
WireConnection;24;1;21;0
WireConnection;24;2;22;1
WireConnection;66;22;1;0
WireConnection;40;0;66;0
WireConnection;40;1;39;0
WireConnection;40;2;48;0
WireConnection;36;0;24;0
WireConnection;27;0;40;0
WireConnection;27;1;29;0
WireConnection;37;0;36;0
WireConnection;32;0;27;0
WireConnection;32;1;29;4
WireConnection;32;2;33;0
WireConnection;25;0;37;0
WireConnection;0;0;32;0
WireConnection;0;11;25;0
ASEEND*/
//CHKSM=CFAFD8FBB326C532B8052BAFC6F18B7729829024