// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/Stlylized Leaves"
{
	Properties
	{
		_Texture("Texture", 2D) = "white" {}
		_AlphaClip("Alpha Clip", Float) = 1
		[HDR]_TopColor("Top Color", Color) = (0,0,0,0)
		_FilterColor("Filter Color", Color) = (1,1,1,0)
		[HDR]_BottomColor("Bottom Color", Color) = (0.3614275,0.5849056,0.3748917,0)
		_WindNoiseSize("Wind Noise Size", Float) = 0.1
		_GradientSmoothness("Gradient Smoothness", Float) = 0
		_WindStrength("Wind Strength", Float) = 1
		_GradientOffset("Gradient Offset", Float) = 0
		_UpperLightTransmission("Upper Light Transmission", Float) = 0
		_FlutterAmount("Flutter Amount", Float) = 3
		_LowerLightTransmission("Lower Light Transmission", Float) = 0
		_FlutterScale("Flutter Scale", Float) = 0
		_FlutterSpeed("Flutter Speed", Float) = 0
		_VariationScale("Variation Scale", Float) = 10
		_VariationAmount("Variation Amount", Float) = 10
		_AlphaVariation("Alpha Variation", Float) = 0
		_SnowAttraction("Snow Attraction", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
		};

		uniform float _FlutterSpeed;
		uniform float CZY_WindTime;
		uniform float _FlutterScale;
		uniform float _FlutterAmount;
		uniform float3 CZY_WindDirection;
		uniform float _WindNoiseSize;
		uniform float _WindStrength;
		uniform float4 CZY_SnowColor;
		uniform sampler2D CZY_SnowTexture;
		uniform float4 CZY_SnowTexture_ST;
		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;
		uniform float _VariationScale;
		uniform float _AlphaVariation;
		uniform float _AlphaClip;
		uniform float4 _BottomColor;
		uniform float4 _TopColor;
		uniform float _GradientOffset;
		uniform float _GradientSmoothness;
		uniform float4 _FilterColor;
		uniform float _VariationAmount;
		uniform float CZY_SnowScale;
		uniform float _SnowAttraction;
		uniform float CZY_SnowAmount;
		uniform float _LowerLightTransmission;
		uniform float _UpperLightTransmission;


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
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


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float2 voronoihash5_g15( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi5_g15( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash5_g15( n + g );
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
			float3 hsvTorgb101 = RGBToHSV( v.color.rgb );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 uv_TexCoord29 = v.texcoord.xy + ( ase_worldPos + ( _FlutterSpeed * CZY_WindTime ) ).xy;
			float simplePerlin2D27 = snoise( uv_TexCoord29*_FlutterScale );
			float4 transform108 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float2 appendResult113 = (float2(( ( transform108.x + transform108.z ) / 10.0 ) , CZY_WindTime));
			float2 uv_TexCoord114 = v.texcoord.xy * float2( 0,0 ) + appendResult113;
			float simplePerlin2D122 = snoise( uv_TexCoord114*_WindNoiseSize );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 temp_output_117_0 = ( CZY_WindDirection * simplePerlin2D122 * ( 0.1 * ase_vertex3Pos.y ) );
			float3 MainWind124 = ( _WindStrength * temp_output_117_0 );
			float3 FinalWind126 = ( ( hsvTorgb101.z * simplePerlin2D27 * _FlutterAmount * temp_output_117_0 ) + MainWind124 );
			float3 worldToObjDir141 = mul( unity_WorldToObject, float4( FinalWind126, 0 ) ).xyz;
			v.vertex.xyz += worldToObjDir141;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uvCZY_SnowTexture = i.uv_texcoord * CZY_SnowTexture_ST.xy + CZY_SnowTexture_ST.zw;
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float4 tex2DNode84 = tex2D( _Texture, uv_Texture );
			float4 transform2_g13 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float simplePerlin2D3_g13 = snoise( transform2_g13.xy*( 1.0 / _VariationScale ) );
			simplePerlin2D3_g13 = simplePerlin2D3_g13*0.5 + 0.5;
			clip( ( tex2DNode84.a - ( simplePerlin2D3_g13 * _AlphaVariation ) ) - _AlphaClip);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float clampResult13 = clamp( ( ( ase_vertex3Pos.y - _GradientOffset ) * _GradientSmoothness ) , 0.0 , 1.0 );
			float SunDirection97 = clampResult13;
			float4 lerpResult7 = lerp( _BottomColor , _TopColor , SunDirection97);
			float3 hsvTorgb3_g14 = RGBToHSV( _FilterColor.rgb );
			float4 transform5_g14 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float simplePerlin2D7_g14 = snoise( transform5_g14.xy*( 1.0 / _VariationScale ) );
			float3 hsvTorgb4_g14 = HSVToRGB( float3(( hsvTorgb3_g14.x + ( simplePerlin2D7_g14 * _VariationAmount ) ),hsvTorgb3_g14.y,hsvTorgb3_g14.z) );
			float3 temp_output_162_0 = saturate( hsvTorgb4_g14 );
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_worldPos = i.worldPos;
			float2 appendResult3_g15 = (float2(ase_worldPos.x , ase_worldPos.z));
			float temp_output_6_0_g15 = ( 1.0 / CZY_SnowScale );
			float simplePerlin2D7_g15 = snoise( appendResult3_g15*temp_output_6_0_g15 );
			simplePerlin2D7_g15 = simplePerlin2D7_g15*0.5 + 0.5;
			float time5_g15 = 0.0;
			float2 voronoiSmoothId5_g15 = 0;
			float2 coords5_g15 = appendResult3_g15 * ( temp_output_6_0_g15 / 0.1 );
			float2 id5_g15 = 0;
			float2 uv5_g15 = 0;
			float voroi5_g15 = voronoi5_g15( coords5_g15, time5_g15, id5_g15, uv5_g15, 0, voronoiSmoothId5_g15 );
			float4 lerpResult19_g15 = lerp( ( CZY_SnowColor * tex2D( CZY_SnowTexture, uvCZY_SnowTexture ) ) , ( ( tex2DNode84 * lerpResult7 ) * float4( temp_output_162_0 , 0.0 ) ) , ( ( pow( ( pow( ase_worldNormal.y , 7.0 ) * ( simplePerlin2D7_g15 * ( 1.0 - voroi5_g15 ) ) ) , 0.5 ) * _SnowAttraction ) > ( 1.0 - CZY_SnowAmount ) ? 0.0 : 1.0 ));
			o.Albedo = lerpResult19_g15.rgb;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 hsvTorgb3_g11 = RGBToHSV( _BottomColor.rgb );
			float4 transform5_g11 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float simplePerlin2D7_g11 = snoise( transform5_g11.xy*( 1.0 / _VariationScale ) );
			float3 hsvTorgb4_g11 = HSVToRGB( float3(( hsvTorgb3_g11.x + ( simplePerlin2D7_g11 * _VariationAmount ) ),hsvTorgb3_g11.y,hsvTorgb3_g11.z) );
			float3 hsvTorgb3_g12 = RGBToHSV( _TopColor.rgb );
			float4 transform5_g12 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float simplePerlin2D7_g12 = snoise( transform5_g12.xy*( 1.0 / _VariationScale ) );
			float3 hsvTorgb4_g12 = HSVToRGB( float3(( hsvTorgb3_g12.x + ( simplePerlin2D7_g12 * _VariationAmount ) ),hsvTorgb3_g12.y,hsvTorgb3_g12.z) );
			float4 lerpResult72 = lerp( ( ( _LowerLightTransmission * ase_lightColor ) * float4( saturate( hsvTorgb4_g11 ) , 0.0 ) ) , ( ( _UpperLightTransmission * ase_lightColor ) * float4( saturate( hsvTorgb4_g12 ) , 0.0 ) ) , SunDirection97);
			o.Emission = ( ( tex2DNode84 * lerpResult72 ) * float4( temp_output_162_0 , 0.0 ) ).rgb;
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
Version=18935
0;1080;2194.286;607.5715;6120.056;1586.203;2.857342;True;False
Node;AmplifyShaderEditor.CommentaryNode;107;-4159.107,1381.891;Inherit;False;2556.385;1135.515;;15;124;119;116;118;117;120;122;115;114;121;113;142;110;111;108;Wind Pass 1;1,1,1,1;0;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;108;-4080.264,1654.294;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;111;-3843.13,1734.128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-3994.671,1984.126;Inherit;False;Global;CZY_WindTime;CZY_WindTime;9;0;Create;True;0;0;0;False;0;False;1;228.2618;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;142;-3693.701,1782.416;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;113;-3528.412,1849.292;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;75;-4148.655,337.655;Inherit;False;2252.941;810.2963;;15;126;36;143;101;27;60;100;30;29;39;38;33;34;144;145;Wind Pass 2;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;121;-3142.249,2186.285;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;114;-3329.531,1800.61;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;115;-3322.024,1987.478;Inherit;False;Property;_WindNoiseSize;Wind Noise Size;7;0;Create;True;0;0;0;False;0;False;0.1;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;-2709.295,2047.761;Inherit;False;2;2;0;FLOAT;0.1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-4076.263,783.2023;Inherit;False;Property;_FlutterSpeed;Flutter Speed;15;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;122;-3021.918,1820.781;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;119;-2796.179,1667.281;Inherit;False;Global;CZY_WindDirection;CZY_WindDirection;11;0;Create;True;0;0;0;False;0;False;0,0,0;-0.4897841,0,-0.100556;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;11;-2672.811,-1867.762;Inherit;False;Property;_GradientOffset;Gradient Offset;10;0;Create;True;0;0;0;False;0;False;0;7.46;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;8;-2684.235,-2046.845;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;12;-2454.603,-1931.525;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-2520.61,-1743.911;Inherit;False;Property;_GradientSmoothness;Gradient Smoothness;8;0;Create;True;0;0;0;False;0;False;0;0.13;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-2507.79,1568.3;Inherit;False;Property;_WindStrength;Wind Strength;9;0;Create;True;0;0;0;False;0;False;1;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-2530.735,1806.765;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-3846.972,830.8922;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;38;-3889.44,624.4289;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-2250.944,-1885.7;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-3702.267,757.8762;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-2194.221,1599.14;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-3526.373,999.8748;Inherit;False;Property;_FlutterScale;Flutter Scale;14;0;Create;True;0;0;0;False;0;False;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;13;-2086.443,-1936.091;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;74;-4143.363,-1496.772;Inherit;False;1287.013;1235.088;Lerp between two colors for the color of the leaves;15;7;72;98;21;16;23;19;151;18;17;6;20;22;152;1;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;124;-1863.174,1607.901;Inherit;False;MainWind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.VertexColorNode;100;-3273.021,463.2006;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;-3547.914,752.5209;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;22;-3736.028,-720.9691;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;20;-3768.304,-862.1539;Inherit;False;Property;_LowerLightTransmission;Lower Light Transmission;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;159;-2711.685,-1077.332;Inherit;False;Property;_AlphaVariation;Alpha Variation;18;0;Create;True;0;0;0;False;0;False;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;27;-3225.842,744.8248;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;97;-1817.772,-1925.523;Inherit;False;SunDirection;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;17;-3730.025,-1251.852;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;60;-2906.644,826.4692;Inherit;False;Property;_FlutterAmount;Flutter Amount;12;0;Create;True;0;0;0;False;0;False;3;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;6;-4026.208,-1042.716;Inherit;False;Property;_TopColor;Top Color;4;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;1,0.9097192,0.259434,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;152;-4084.09,-750.9153;Inherit;False;Property;_VariationAmount;Variation Amount;17;0;Create;True;0;0;0;False;0;False;10;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;143;-2814.219,1047.43;Inherit;False;124;MainWind;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;84;-2798.001,-1307.699;Inherit;True;Property;_Texture;Texture;2;0;Create;True;0;0;0;False;0;False;-1;None;6e737a3a12351474383bb67c426cc689;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-4085.817,-542.3893;Inherit;False;Property;_BottomColor;Bottom Color;6;1;[HDR];Create;True;0;0;0;False;0;False;0.3614275,0.5849056,0.3748917,0;0.4433962,0.4433962,0.4433962,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;151;-4078.89,-839.3153;Inherit;False;Property;_VariationScale;Variation Scale;16;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-3775.615,-1378.319;Inherit;False;Property;_UpperLightTransmission;Upper Light Transmission;11;0;Create;True;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;101;-3092.447,467.9185;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-3508.694,-729.9916;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;-3442.909,-354.1027;Inherit;False;97;SunDirection;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;165;-2483.812,-1140.928;Inherit;False;SimpleVariation;-1;;13;cfd98d536b88d9a41b892a05fcccc09b;0;3;8;FLOAT;0;False;9;FLOAT;0;False;7;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;161;-3777.71,-1097.71;Inherit;False;HueVariation;-1;;12;014c75eecfd32e8408be368612f8be0e;0;3;12;FLOAT;0;False;13;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;160;-3782.844,-515.0247;Inherit;False;HueVariation;-1;;11;014c75eecfd32e8408be368612f8be0e;0;3;12;FLOAT;0;False;13;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-2601.313,733.6399;Inherit;True;4;4;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-2160.224,-897.3383;Inherit;False;Property;_AlphaClip;Alpha Clip;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-3502.691,-1260.873;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;145;-2444.352,978.5878;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;148;-2432,-528;Inherit;False;Property;_FilterColor;Filter Color;5;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.2239,0.6698113,0.08530613,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-3428.132,-526.9956;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;7;-3079.516,-663.9136;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;144;-2369.456,826.9582;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-3403.434,-1101.173;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClipNode;85;-2004.632,-999.0538;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;72;-3080.893,-886.8154;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;126;-2145.202,824.2262;Inherit;False;FinalWind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;162;-2192,-528;Inherit;False;HueVariation;-1;;14;014c75eecfd32e8408be368612f8be0e;0;3;12;FLOAT;0;False;13;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-1658.604,-681.5576;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-1657.949,-784.9277;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;127;-1685.648,-87.70827;Inherit;False;126;FinalWind;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;-1507.326,-681.9669;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-1598.738,-545.6181;Inherit;False;Property;_SnowAttraction;Snow Attraction;19;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;141;-1435.37,-204.9131;Inherit;False;World;Object;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;166;-1298.438,-666.5178;Inherit;False;Stylized Snow Blend;0;;15;359b695eb7172584f9df5a0d55bd52e9;0;2;34;FLOAT;0.8;False;22;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;-1508.832,-787.4256;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-908.9573,-735.5206;Float;False;True;-1;2;;0;0;Standard;Distant Lands/Cozy/Stlylized Leaves;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;111;0;108;1
WireConnection;111;1;108;3
WireConnection;142;0;111;0
WireConnection;113;0;142;0
WireConnection;113;1;110;0
WireConnection;114;1;113;0
WireConnection;120;1;121;2
WireConnection;122;0;114;0
WireConnection;122;1;115;0
WireConnection;12;0;8;2
WireConnection;12;1;11;0
WireConnection;117;0;119;0
WireConnection;117;1;122;0
WireConnection;117;2;120;0
WireConnection;33;0;34;0
WireConnection;33;1;110;0
WireConnection;9;0;12;0
WireConnection;9;1;10;0
WireConnection;39;0;38;0
WireConnection;39;1;33;0
WireConnection;118;0;116;0
WireConnection;118;1;117;0
WireConnection;13;0;9;0
WireConnection;124;0;118;0
WireConnection;29;1;39;0
WireConnection;27;0;29;0
WireConnection;27;1;30;0
WireConnection;97;0;13;0
WireConnection;101;0;100;0
WireConnection;23;0;20;0
WireConnection;23;1;22;0
WireConnection;165;8;151;0
WireConnection;165;9;159;0
WireConnection;165;7;84;4
WireConnection;161;12;151;0
WireConnection;161;13;152;0
WireConnection;161;1;6;0
WireConnection;160;12;151;0
WireConnection;160;13;152;0
WireConnection;160;1;1;0
WireConnection;36;0;101;3
WireConnection;36;1;27;0
WireConnection;36;2;60;0
WireConnection;36;3;117;0
WireConnection;19;0;18;0
WireConnection;19;1;17;0
WireConnection;145;0;143;0
WireConnection;21;0;23;0
WireConnection;21;1;160;0
WireConnection;7;0;1;0
WireConnection;7;1;6;0
WireConnection;7;2;98;0
WireConnection;144;0;36;0
WireConnection;144;1;145;0
WireConnection;16;0;19;0
WireConnection;16;1;161;0
WireConnection;85;0;84;0
WireConnection;85;1;165;0
WireConnection;85;2;86;0
WireConnection;72;0;21;0
WireConnection;72;1;16;0
WireConnection;72;2;98;0
WireConnection;126;0;144;0
WireConnection;162;12;151;0
WireConnection;162;13;152;0
WireConnection;162;1;148;0
WireConnection;87;0;85;0
WireConnection;87;1;7;0
WireConnection;88;0;85;0
WireConnection;88;1;72;0
WireConnection;147;0;87;0
WireConnection;147;1;162;0
WireConnection;141;0;127;0
WireConnection;166;34;167;0
WireConnection;166;22;147;0
WireConnection;146;0;88;0
WireConnection;146;1;162;0
WireConnection;0;0;166;0
WireConnection;0;2;146;0
WireConnection;0;11;141;0
ASEEND*/
//CHKSM=8978E5B1E056E4F6E07C9F93D59A165B1589B50C