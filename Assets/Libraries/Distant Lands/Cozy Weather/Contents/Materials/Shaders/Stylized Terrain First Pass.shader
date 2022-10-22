// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/Stylized Terrain First Pass"
{
	Properties
	{
		[HideInInspector]_TerrainHolesTexture("_TerrainHolesTexture", 2D) = "white" {}
		[HideInInspector]_Control("Control", 2D) = "white" {}
		[HideInInspector]_Splat3("Splat3", 2D) = "white" {}
		[HideInInspector]_Splat2("Splat2", 2D) = "white" {}
		[HideInInspector]_Splat1("Splat1", 2D) = "white" {}
		[HideInInspector]_Splat0("Splat0", 2D) = "white" {}
		[HideInInspector]_Normal0("Normal0", 2D) = "white" {}
		[HideInInspector]_Normal1("Normal1", 2D) = "white" {}
		[HideInInspector]_Normal2("Normal2", 2D) = "white" {}
		[HideInInspector]_Normal3("Normal3", 2D) = "white" {}
		[HideInInspector]_Smoothness3("Smoothness3", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness1("Smoothness1", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness0("Smoothness0", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness2("Smoothness2", Range( 0 , 1)) = 1
		[HideInInspector][Gamma]_Metallic0("Metallic0", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic2("Metallic2", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic3("Metallic3", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic1("Metallic1", Range( 0 , 1)) = 0
		[HideInInspector]_Mask2("_Mask2", 2D) = "white" {}
		[HideInInspector]_Mask0("_Mask0", 2D) = "white" {}
		[HideInInspector]_Mask1("_Mask1", 2D) = "white" {}
		[HideInInspector]_Mask3("_Mask3", 2D) = "white" {}
		[HideInInspector]_Specular2("Specular2", Color) = (1,1,1,0)
		[HideInInspector]_Specular1("Specular1", Color) = (1,1,1,0)
		[HideInInspector]_Specular0("Specular0", Color) = (1,1,1,0)
		[HideInInspector]_Specular3("Specular3", Color) = (1,1,1,0)
		_RockThreshold("Rock Threshold", Float) = 0
		_RockColor("Rock Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry-100" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
		#pragma multi_compile_local __ _ALPHATEST_ON
		#pragma shader_feature_local _MASKMAP
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		uniform float4 _MaskMapRemapScale2;
		uniform float4 _MaskMapRemapOffset1;
		uniform float4 _MaskMapRemapScale0;
		uniform float4 _MaskMapRemapOffset2;
		uniform float4 _MaskMapRemapScale1;
		uniform float4 _MaskMapRemapScale3;
		uniform float4 _MaskMapRemapOffset0;
		uniform float4 _MaskMapRemapOffset3;
		uniform sampler2D _Mask2;
		uniform sampler2D _Mask0;
		uniform sampler2D _Mask1;
		uniform sampler2D _Mask3;
		#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
			sampler2D _TerrainHeightmapTexture;//ASE Terrain Instancing
			sampler2D _TerrainNormalmapTexture;//ASE Terrain Instancing
		#endif//ASE Terrain Instancing
		UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
			UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
		UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
		CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
				float4 _TerrainHeightmapScale;//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
		CBUFFER_END//ASE Terrain Instancing
		uniform sampler2D _Normal0;
		uniform sampler2D _Splat0;
		uniform float4 _Splat0_ST;
		uniform sampler2D _Control;
		uniform float4 _Control_ST;
		uniform sampler2D _Normal1;
		uniform sampler2D _Splat1;
		uniform float4 _Splat1_ST;
		uniform sampler2D _Normal2;
		uniform sampler2D _Splat2;
		uniform float4 _Splat2_ST;
		uniform sampler2D _Normal3;
		uniform sampler2D _Splat3;
		uniform float4 _Splat3_ST;
		uniform float4 _RockColor;
		uniform float4 CZY_SnowColor;
		uniform sampler2D CZY_SnowTexture;
		uniform float4 CZY_SnowTexture_ST;
		uniform float _Smoothness0;
		uniform float4 _Specular0;
		uniform float _Smoothness1;
		uniform float4 _Specular1;
		uniform float _Smoothness2;
		uniform float4 _Specular2;
		uniform float _Smoothness3;
		uniform float4 _Specular3;
		uniform sampler2D _TerrainHolesTexture;
		uniform float4 _TerrainHolesTexture_ST;
		uniform float CZY_SnowScale;
		uniform float _Metallic0;
		uniform float _Metallic1;
		uniform float _Metallic2;
		uniform float _Metallic3;
		uniform float CZY_SnowAmount;
		uniform float _RockThreshold;
		uniform float CZY_PuddleScale;
		uniform float CZY_WetnessAmount;


		void SplatmapFinalColor( Input SurfaceIn, SurfaceOutputStandard SurfaceOut, inout fixed4 FinalColor )
		{
			FinalColor *= SurfaceOut.Alpha;
		}


		void ApplyMeshModification( inout appdata_full v )
		{
			#if defined(UNITY_INSTANCING_ENABLED) && !defined(SHADER_API_D3D11_9X)
				float2 patchVertex = v.vertex.xy;
				float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);
				
				float4 uvscale = instanceData.z * _TerrainHeightmapRecipSize;
				float4 uvoffset = instanceData.xyxy * uvscale;
				uvoffset.xy += 0.5f * _TerrainHeightmapRecipSize.xy;
				float2 sampleCoords = (patchVertex.xy * uvscale.xy + uvoffset.xy);
				
				float hm = UnpackHeightmap(tex2Dlod(_TerrainHeightmapTexture, float4(sampleCoords, 0, 0)));
				v.vertex.xz = (patchVertex.xy + instanceData.xy) * _TerrainHeightmapScale.xz * instanceData.z;
				v.vertex.y = hm * _TerrainHeightmapScale.y;
				v.vertex.w = 1.0f;
				
				v.texcoord.xy = (patchVertex.xy * uvscale.zw + uvoffset.zw);
				v.texcoord3 = v.texcoord2 = v.texcoord1 = v.texcoord;
				
				#ifdef TERRAIN_INSTANCED_PERPIXEL_NORMAL
					v.normal = float3(0, 1, 0);
					//data.tc.zw = sampleCoords;
				#else
					float3 nor = tex2Dlod(_TerrainNormalmapTexture, float4(sampleCoords, 0, 0)).xyz;
					v.normal = 2.0f * nor - 1.0f;
				#endif
			#endif
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


		float2 voronoihash5_g81( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi5_g81( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash5_g81( n + g );
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


		float2 voronoihash1_g83( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi1_g83( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash1_g83( n + g );
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
			return (F2 + F1) * 0.5;
		}


		float2 voronoihash8_g83( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi8_g83( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash8_g83( n + g );
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
			ApplyMeshModification(v);;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Splat0 = i.uv_texcoord * _Splat0_ST.xy + _Splat0_ST.zw;
			float2 uv_Control = i.uv_texcoord * _Control_ST.xy + _Control_ST.zw;
			float4 tex2DNode5_g36 = tex2D( _Control, uv_Control );
			float dotResult20_g36 = dot( tex2DNode5_g36 , float4(1,1,1,1) );
			float SplatWeight22_g36 = dotResult20_g36;
			float localSplatClip74_g36 = ( SplatWeight22_g36 );
			float SplatWeight74_g36 = SplatWeight22_g36;
			{
			#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(SplatWeight74_g36 == 0.0f ? -1 : 1);
			#endif
			}
			float4 SplatControl26_g36 = ( tex2DNode5_g36 / ( localSplatClip74_g36 + 0.001 ) );
			float4 temp_output_59_0_g36 = SplatControl26_g36;
			float4 break20_g37 = temp_output_59_0_g36;
			float4 lerpResult16_g37 = lerp( float4( 0,0,0,0 ) , tex2D( _Normal0, uv_Splat0 ) , ( break20_g37.r > max( max( break20_g37.g , break20_g37.b ) , break20_g37.a ) ? 1.0 : 0.0 ));
			float2 uv_Splat1 = i.uv_texcoord * _Splat1_ST.xy + _Splat1_ST.zw;
			float4 lerpResult14_g37 = lerp( lerpResult16_g37 , tex2D( _Normal1, uv_Splat1 ) , ( break20_g37.g > max( max( break20_g37.r , break20_g37.b ) , break20_g37.a ) ? 1.0 : 0.0 ));
			float2 uv_Splat2 = i.uv_texcoord * _Splat2_ST.xy + _Splat2_ST.zw;
			float4 lerpResult8_g37 = lerp( lerpResult14_g37 , tex2D( _Normal2, uv_Splat2 ) , ( break20_g37.b > max( max( break20_g37.r , break20_g37.g ) , break20_g37.a ) ? 1.0 : 0.0 ));
			float2 uv_Splat3 = i.uv_texcoord * _Splat3_ST.xy + _Splat3_ST.zw;
			float4 lerpResult13_g37 = lerp( lerpResult8_g37 , tex2D( _Normal3, uv_Splat3 ) , ( break20_g37.a > max( max( break20_g37.r , break20_g37.g ) , break20_g37.b ) ? 1.0 : 0.0 ));
			float3 temp_output_61_0_g36 = UnpackNormal( saturate( lerpResult13_g37 ) );
			o.Normal = temp_output_61_0_g36;
			float2 uvCZY_SnowTexture = i.uv_texcoord * CZY_SnowTexture_ST.xy + CZY_SnowTexture_ST.zw;
			float4 appendResult33_g36 = (float4(1.0 , 1.0 , 1.0 , _Smoothness0));
			float4 tex2DNode4_g36 = tex2D( _Splat0, uv_Splat0 );
			float4 appendResult258_g36 = (float4(_Specular0.rgb , 1.0));
			float4 tintLayer0253_g36 = appendResult258_g36;
			float4 break20_g38 = temp_output_59_0_g36;
			float4 lerpResult16_g38 = lerp( float4( 0,0,0,0 ) , ( appendResult33_g36 * tex2DNode4_g36 * tintLayer0253_g36 ) , ( break20_g38.r > max( max( break20_g38.g , break20_g38.b ) , break20_g38.a ) ? 1.0 : 0.0 ));
			float4 appendResult36_g36 = (float4(1.0 , 1.0 , 1.0 , _Smoothness1));
			float4 tex2DNode3_g36 = tex2D( _Splat1, uv_Splat1 );
			float4 appendResult261_g36 = (float4(_Specular1.rgb , 1.0));
			float4 tintLayer1254_g36 = appendResult261_g36;
			float4 lerpResult14_g38 = lerp( lerpResult16_g38 , ( appendResult36_g36 * tex2DNode3_g36 * tintLayer1254_g36 ) , ( break20_g38.g > max( max( break20_g38.r , break20_g38.b ) , break20_g38.a ) ? 1.0 : 0.0 ));
			float4 appendResult39_g36 = (float4(1.0 , 1.0 , 1.0 , _Smoothness2));
			float4 tex2DNode6_g36 = tex2D( _Splat2, uv_Splat2 );
			float4 appendResult263_g36 = (float4(_Specular2.rgb , 1.0));
			float4 tintLayer2255_g36 = appendResult263_g36;
			float4 lerpResult8_g38 = lerp( lerpResult14_g38 , ( appendResult39_g36 * tex2DNode6_g36 * tintLayer2255_g36 ) , ( break20_g38.b > max( max( break20_g38.r , break20_g38.g ) , break20_g38.a ) ? 1.0 : 0.0 ));
			float4 appendResult42_g36 = (float4(1.0 , 1.0 , 1.0 , _Smoothness3));
			float4 tex2DNode7_g36 = tex2D( _Splat3, uv_Splat3 );
			float4 appendResult265_g36 = (float4(_Specular3.rgb , 1.0));
			float4 tintLayer3256_g36 = appendResult265_g36;
			float4 lerpResult13_g38 = lerp( lerpResult8_g38 , ( appendResult42_g36 * tex2DNode7_g36 * tintLayer3256_g36 ) , ( break20_g38.a > max( max( break20_g38.r , break20_g38.g ) , break20_g38.b ) ? 1.0 : 0.0 ));
			float4 MixDiffuse28_g36 = saturate( lerpResult13_g38 );
			float4 temp_output_60_0_g36 = MixDiffuse28_g36;
			float4 localClipHoles100_g36 = ( temp_output_60_0_g36 );
			float2 uv_TerrainHolesTexture = i.uv_texcoord * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
			float holeClipValue99_g36 = tex2D( _TerrainHolesTexture, uv_TerrainHolesTexture ).r;
			float Hole100_g36 = holeClipValue99_g36;
			{
			#ifdef _ALPHATEST_ON
				clip(Hole100_g36 == 0.0f ? -1 : 1);
			#endif
			}
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float2 appendResult3_g81 = (float2(ase_worldPos.x , ase_worldPos.z));
			float temp_output_6_0_g81 = ( 1.0 / CZY_SnowScale );
			float simplePerlin2D7_g81 = snoise( appendResult3_g81*temp_output_6_0_g81 );
			simplePerlin2D7_g81 = simplePerlin2D7_g81*0.5 + 0.5;
			float time5_g81 = 0.0;
			float2 voronoiSmoothId5_g81 = 0;
			float2 coords5_g81 = appendResult3_g81 * ( temp_output_6_0_g81 / 0.1 );
			float2 id5_g81 = 0;
			float2 uv5_g81 = 0;
			float voroi5_g81 = voronoi5_g81( coords5_g81, time5_g81, id5_g81, uv5_g81, 0, voronoiSmoothId5_g81 );
			float4 appendResult55_g36 = (float4(_Metallic0 , _Metallic1 , _Metallic2 , _Metallic3));
			float dotResult53_g36 = dot( SplatControl26_g36 , appendResult55_g36 );
			float4 lerpResult19_g81 = lerp( ( CZY_SnowColor * tex2D( CZY_SnowTexture, uvCZY_SnowTexture ) ) , localClipHoles100_g36 , ( ( pow( ( pow( ase_worldNormal.y , 7.0 ) * ( simplePerlin2D7_g81 * ( 1.0 - voroi5_g81 ) ) ) , 0.5 ) * dotResult53_g36 ) > ( 1.0 - CZY_SnowAmount ) ? 0.0 : 1.0 ));
			float4 lerpResult67 = lerp( _RockColor , lerpResult19_g81 , ( ( 1.0 - ase_worldNormal.y ) > _RockThreshold ? 0.0 : 1.0 ));
			o.Albedo = lerpResult67.rgb;
			float3 temp_cast_21 = (unity_FogParams.y).xxx;
			o.Emission = temp_cast_21;
			float temp_output_5_0_g83 = ( 1.0 / CZY_PuddleScale );
			float time1_g83 = 0.0;
			float2 voronoiSmoothId1_g83 = 0;
			float2 appendResult3_g83 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 coords1_g83 = appendResult3_g83 * temp_output_5_0_g83;
			float2 id1_g83 = 0;
			float2 uv1_g83 = 0;
			float voroi1_g83 = voronoi1_g83( coords1_g83, time1_g83, id1_g83, uv1_g83, 0, voronoiSmoothId1_g83 );
			float time8_g83 = 2.16;
			float2 voronoiSmoothId8_g83 = 0;
			float2 coords8_g83 = i.uv_texcoord * ( temp_output_5_0_g83 * 3.0 );
			float2 id8_g83 = 0;
			float2 uv8_g83 = 0;
			float voroi8_g83 = voronoi8_g83( coords8_g83, time8_g83, id8_g83, uv8_g83, 0, voronoiSmoothId8_g83 );
			o.Smoothness = ( ( ase_worldNormal.y * 2.0 * ( (1.0 + (voroi1_g83 - 0.0) * (0.0 - 1.0) / (0.4 - 0.0)) + (0.1 + (voroi8_g83 - 0.0) * (-0.3 - 0.1) / (0.21 - 0.0)) ) * (0.3 + (CZY_WetnessAmount - 0.0) * (1.0 - 0.3) / (1.0 - 0.0)) ) > ( 1.0 - ( CZY_WetnessAmount * 1.0 ) ) ? 1.0 : 0.0 );
			o.Alpha = SplatWeight22_g36;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc  finalcolor:SplatmapFinalColor

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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
		UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
	}

	Dependency "BaseMapShader"="Distant Lands/Stylized Terrain"
	Dependency "AddPassShader"="Distant Lands/Stylized Terrain Add Pass"
	Fallback "Nature/Terrain/Diffuse"
}
/*ASEBEGIN
Version=18935
0;1080;2194.286;607.5715;904.5516;41.09918;1;True;False
Node;AmplifyShaderEditor.WorldNormalVector;62;-523.6168,-99.02313;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;63;-273.4164,33.97681;Inherit;False;Property;_RockThreshold;Rock Threshold;30;0;Create;True;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;64;-250.0165,-66.12317;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;94;-490.9626,239.8136;Inherit;False;Stylized Terrain Blend;0;;36;9d4ad2a2d80f7ca4d90e641385d406e3;2,85,0,102,1;7;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0;False;58;FLOAT;0;False;201;FLOAT;0;False;62;FLOAT;0;False;7;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;200;FLOAT;19;FLOAT3;17
Node;AmplifyShaderEditor.ColorNode;65;-86.21644,-201.3231;Inherit;False;Property;_RockColor;Rock Color;31;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.3773585,0.3773585,0.3773585,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;66;-31.61659,-16.72317;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;137;-89.8521,162.4091;Inherit;False;Stylized Snow Blend;28;;81;359b695eb7172584f9df5a0d55bd52e9;0;2;34;FLOAT;1;False;22;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;67;237.3835,64.37683;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;100;264.7746,242.9527;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomExpressionNode;45;-465.027,-264.4264;Float;False;FinalColor *= SurfaceOut.Alpha@;7;Create;3;True;SurfaceIn;OBJECT;0;In;Input;Float;False;True;SurfaceOut;OBJECT;0;In;SurfaceOutputStandard;Float;False;True;FinalColor;OBJECT;0;InOut;fixed4;Float;False;SplatmapFinalColor;False;True;0;;False;4;0;FLOAT;0;False;1;OBJECT;0;False;2;OBJECT;0;False;3;OBJECT;0;False;2;FLOAT;0;OBJECT;4
Node;AmplifyShaderEditor.FogParamsNode;139;145.4484,389.0437;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;138;-51.64612,288.8251;Inherit;False;StylizedRainPuddles;-1;;83;ff913d501aadc8b4cb4ec6889f2354ca;0;1;14;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;848.0565,47.11808;Float;False;True;-1;2;;0;0;Standard;Distant Lands/Cozy/Stylized Terrain First Pass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;-100;True;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;Nature/Terrain/Diffuse;32;-1;-1;-1;0;False;2;BaseMapShader=Distant Lands/Stylized Terrain;AddPassShader=Distant Lands/Stylized Terrain Add Pass;0;False;-1;-1;0;False;-1;0;1;finalcolor:SplatmapFinalColor;0;True;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;64;0;62;2
WireConnection;66;0;64;0
WireConnection;66;1;63;0
WireConnection;137;34;94;56
WireConnection;137;22;94;0
WireConnection;67;0;65;0
WireConnection;67;1;137;0
WireConnection;67;2;66;0
WireConnection;100;0;94;14
WireConnection;0;0;67;0
WireConnection;0;1;100;0
WireConnection;0;2;139;2
WireConnection;0;4;138;0
WireConnection;0;9;94;19
ASEEND*/
//CHKSM=4826ADC5F1D99C1689E489F6C5D7180AD8523DE2