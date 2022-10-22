// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Stlylized Tree Trunk"
{
	Properties
	{
		_MainColor("Color", Color) = (0,0,0,0)
		_WindStrength("Wind Strength", Float) = 1
		_WindNoiseSize("Wind Noise Size", Float) = 0.1
		_SnowAttraction1("Snow Attraction", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			float3 worldPos;
		};

		uniform float3 CZY_WindDirection;
		uniform float _WindStrength;
		uniform float CZY_WindTime;
		uniform float _WindNoiseSize;
		uniform float4 CZY_SnowColor;
		uniform sampler2D CZY_SnowTexture;
		uniform float4 CZY_SnowTexture_ST;
		uniform float4 _MainColor;
		uniform float CZY_SnowScale;
		uniform float _SnowAttraction1;
		uniform float CZY_SnowAmount;


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


		float2 voronoihash5_g1( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi5_g1( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash5_g1( n + g );
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
			float4 transform63 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float2 appendResult68 = (float2(( ( transform63.x + transform63.z ) / 10.0 ) , CZY_WindTime));
			float2 uv_TexCoord69 = v.texcoord.xy * float2( 0,0 ) + appendResult68;
			float simplePerlin2D74 = snoise( uv_TexCoord69*_WindNoiseSize );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float clampResult85 = clamp( ase_vertex3Pos.y , 0.0 , 1000.0 );
			float3 MainWind83 = ( CZY_WindDirection * ( _WindStrength * simplePerlin2D74 * ( 0.1 * clampResult85 ) ) );
			float3 worldToObjDir61 = mul( unity_WorldToObject, float4( MainWind83, 0 ) ).xyz;
			v.vertex.xyz += worldToObjDir61;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uvCZY_SnowTexture = i.uv_texcoord * CZY_SnowTexture_ST.xy + CZY_SnowTexture_ST.zw;
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_worldPos = i.worldPos;
			float2 appendResult3_g1 = (float2(ase_worldPos.x , ase_worldPos.z));
			float temp_output_6_0_g1 = ( 1.0 / CZY_SnowScale );
			float simplePerlin2D7_g1 = snoise( appendResult3_g1*temp_output_6_0_g1 );
			simplePerlin2D7_g1 = simplePerlin2D7_g1*0.5 + 0.5;
			float time5_g1 = 0.0;
			float2 voronoiSmoothId5_g1 = 0;
			float2 coords5_g1 = appendResult3_g1 * ( temp_output_6_0_g1 / 0.1 );
			float2 id5_g1 = 0;
			float2 uv5_g1 = 0;
			float voroi5_g1 = voronoi5_g1( coords5_g1, time5_g1, id5_g1, uv5_g1, 0, voronoiSmoothId5_g1 );
			float4 lerpResult19_g1 = lerp( ( CZY_SnowColor * tex2D( CZY_SnowTexture, uvCZY_SnowTexture ) ) , _MainColor , ( ( pow( ( pow( ase_worldNormal.y , 7.0 ) * ( simplePerlin2D7_g1 * ( 1.0 - voroi5_g1 ) ) ) , 0.5 ) * _SnowAttraction1 ) > ( 1.0 - CZY_SnowAmount ) ? 0.0 : 1.0 ));
			o.Albedo = lerpResult19_g1.rgb;
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
0;1080;2194.286;607.5715;3934.144;-669.4683;1.311493;True;False
Node;AmplifyShaderEditor.CommentaryNode;62;-3102.156,612.8757;Inherit;False;2459.885;1017.709;;18;83;80;76;77;74;72;75;70;69;85;71;68;66;84;67;64;65;63;Main Wind Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;63;-3007.333,877.1843;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;67;-2751.335,933.0962;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;84;-2613.434,1007.712;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-2937.719,1215.111;Inherit;False;Global;CZY_WindTime;CZY_WindTime;3;0;Create;True;0;0;0;False;0;False;1;225.7272;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;68;-2471.461,1080.277;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PosVertexDataNode;71;-2084.459,1430.003;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;69;-2272.579,1031.595;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;70;-2265.072,1218.461;Inherit;False;Property;_WindNoiseSize;Wind Noise Size;4;0;Create;True;0;0;0;False;0;False;0.1;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;85;-1825.257,1395.195;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1000;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;74;-1964.967,1051.765;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-1763.848,918.5268;Inherit;False;Property;_WindStrength;Wind Strength;3;0;Create;True;0;0;0;False;0;False;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-1652.344,1278.744;Inherit;False;2;2;0;FLOAT;0.1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;76;-1465.965,811.3168;Inherit;False;Global;CZY_WindDirection;CZY_WindDirection;3;0;Create;True;0;0;0;False;0;False;0,0,0;-0.4896795,0,-0.1010641;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-1473.783,1037.75;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-1137.27,830.1246;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;83;-886.0038,849.5569;Inherit;False;MainWind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;88;-387.5068,-48.97424;Inherit;False;Property;_SnowAttraction1;Snow Attraction;5;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;-217.3257,387.7493;Inherit;False;83;MainWind;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;1;-326.4081,55.64683;Inherit;False;Property;_MainColor;Color;0;0;Create;False;0;0;0;False;0;False;0,0,0,0;0.509434,0.277534,0.1658062,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-2665.806,1223.224;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;61;-4.122723,373.3203;Inherit;False;World;Object;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;86;-67.50684,36.02577;Inherit;False;Stylized Snow Blend;1;;1;359b695eb7172584f9df5a0d55bd52e9;0;2;34;FLOAT;1;False;22;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;65;-2918.32,1342.237;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;271.3246,41.95449;Float;False;True;-1;2;;0;0;Standard;Distant Lands/Stlylized Tree Trunk;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;67;0;63;1
WireConnection;67;1;63;3
WireConnection;84;0;67;0
WireConnection;68;0;84;0
WireConnection;68;1;64;0
WireConnection;69;1;68;0
WireConnection;85;0;71;2
WireConnection;74;0;69;0
WireConnection;74;1;70;0
WireConnection;75;1;85;0
WireConnection;77;0;72;0
WireConnection;77;1;74;0
WireConnection;77;2;75;0
WireConnection;80;0;76;0
WireConnection;80;1;77;0
WireConnection;83;0;80;0
WireConnection;66;0;64;0
WireConnection;66;1;65;0
WireConnection;61;0;60;0
WireConnection;86;34;88;0
WireConnection;86;22;1;0
WireConnection;0;0;86;0
WireConnection;0;11;61;0
ASEEND*/
//CHKSM=DBC0FCEA76FB8CD1BFE7F13E109FBCB2916E4613