// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/Stylized Clouds Ghibli"
{
	Properties
	{
		[HideInInspector][HDR][Header(General Cloud Settings)]_CloudColor("Cloud Color", Color) = (0.7264151,0.7264151,0.7264151,0)
		[HideInInspector][HDR]_CloudHighlightColor("Cloud Highlight Color", Color) = (1,1,1,0)
		[HideInInspector]_WindSpeed("Wind Speed", Float) = 0
		[HideInInspector][Header(Cumulus Clouds)]_CumulusCoverageMultiplier("Cumulus Coverage Multiplier", Range( 0 , 2)) = 1
		[HideInInspector]_MaxCloudCover("Max Cloud Cover", Float) = 1
		[HideInInspector]_CloudCohesion("Cloud Cohesion", Range( 0 , 1)) = 0.887
		[HideInInspector]_MainCloudScale("Main Cloud Scale", Float) = 0.8
		[HideInInspector]_Spherize("Spherize", Range( 0 , 1)) = 0.36
		[HideInInspector]_ShadowingDistance("Shadowing Distance", Range( 0 , 0.1)) = 0.07
		[HideInInspector]_MainCloudWindDir("Main Cloud Wind Dir", Vector) = (0.1,0.2,0,0)
		[HideInInspector][HDR]_SecondLayer("Second Layer", Color) = (0.8396226,0.8396226,0.8396226,0)
		[HDR]_AltoCloudColor("Alto Cloud Color", Color) = (0.8160377,0.9787034,1,0)
		[HideInInspector]_CloudThickness("CloudThickness", Range( 0 , 4)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent-50" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		Stencil
		{
			Ref 221
			Comp Always
			Pass Replace
		}
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _AltoCloudColor;
		uniform float4 _CloudHighlightColor;
		uniform float4 _CloudColor;
		uniform float4 _SecondLayer;
		uniform float _Spherize;
		uniform float _MainCloudScale;
		uniform float _WindSpeed;
		uniform float2 _MainCloudWindDir;
		uniform float _CloudCohesion;
		uniform float _CumulusCoverageMultiplier;
		uniform float _MaxCloudCover;
		uniform float _ShadowingDistance;
		uniform float _CloudThickness;


		struct Gradient
		{
			int type;
			int colorsLength;
			int alphasLength;
			float4 colors[8];
			float2 alphas[8];
		};


		Gradient NewGradient(int type, int colorsLength, int alphasLength, 
		float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
		float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
		{
			Gradient g;
			g.type = type;
			g.colorsLength = colorsLength;
			g.alphasLength = alphasLength;
			g.colors[ 0 ] = colors0;
			g.colors[ 1 ] = colors1;
			g.colors[ 2 ] = colors2;
			g.colors[ 3 ] = colors3;
			g.colors[ 4 ] = colors4;
			g.colors[ 5 ] = colors5;
			g.colors[ 6 ] = colors6;
			g.colors[ 7 ] = colors7;
			g.alphas[ 0 ] = alphas0;
			g.alphas[ 1 ] = alphas1;
			g.alphas[ 2 ] = alphas2;
			g.alphas[ 3 ] = alphas3;
			g.alphas[ 4 ] = alphas4;
			g.alphas[ 5 ] = alphas5;
			g.alphas[ 6 ] = alphas6;
			g.alphas[ 7 ] = alphas7;
			return g;
		}


		float2 voronoihash35_g40( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi35_g40( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash35_g40( n + g );
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


		float2 voronoihash13_g40( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi13_g40( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash13_g40( n + g );
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


		float2 voronoihash11_g40( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi11_g40( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash11_g40( n + g );
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


		float2 voronoihash35_g38( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi35_g38( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash35_g38( n + g );
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


		float2 voronoihash13_g38( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi13_g38( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash13_g38( n + g );
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


		float2 voronoihash11_g38( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi11_g38( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash11_g38( n + g );
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


		float2 voronoihash35_g39( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi35_g39( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash35_g39( n + g );
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


		float2 voronoihash13_g39( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi13_g39( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash13_g39( n + g );
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


		float2 voronoihash11_g39( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi11_g39( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash11_g39( n + g );
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


		float4 SampleGradient( Gradient gradient, float time )
		{
			float3 color = gradient.colors[0].rgb;
			UNITY_UNROLL
			for (int c = 1; c < 8; c++)
			{
			float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1));
			color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
			}
			#ifndef UNITY_COLORSPACE_GAMMA
			color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
			#endif
			float alpha = gradient.alphas[0].x;
			UNITY_UNROLL
			for (int a = 1; a < 8; a++)
			{
			float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1));
			alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
			}
			return float4(color, alpha);
		}


		float2 voronoihash35_g41( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi35_g41( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash35_g41( n + g );
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


		float2 voronoihash13_g41( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi13_g41( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash13_g41( n + g );
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


		float2 voronoihash11_g41( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi11_g41( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash11_g41( n + g );
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


		float2 voronoihash35_g42( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi35_g42( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash35_g42( n + g );
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


		float2 voronoihash13_g42( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi13_g42( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash13_g42( n + g );
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


		float2 voronoihash11_g42( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi11_g42( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash11_g42( n + g );
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


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 CloudHighlightColor334 = _CloudHighlightColor;
			float4 CloudColor332 = _CloudColor;
			Gradient gradient1145 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.5411765 ), float4( 1, 1, 1, 0.6441138 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float2 temp_output_1043_0 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult1045 = dot( temp_output_1043_0 , temp_output_1043_0 );
			float Dot1071 = saturate( (0.85 + (dotResult1045 - 0.0) * (3.0 - 0.85) / (1.0 - 0.0)) );
			float time35_g40 = 0.0;
			float2 voronoiSmoothId0 = 0;
			float2 CentralUV998 = ( i.uv_texcoord + float2( -0.5,-0.5 ) );
			float2 temp_output_21_0_g40 = (CentralUV998*1.58 + 0.0);
			float2 break2_g40 = abs( temp_output_21_0_g40 );
			float saferPower4_g40 = max( break2_g40.x , 0.0001 );
			float saferPower3_g40 = max( break2_g40.y , 0.0001 );
			float saferPower6_g40 = max( ( pow( saferPower4_g40 , 2.0 ) + pow( saferPower3_g40 , 2.0 ) ) , 0.0001 );
			float Spherize1078 = _Spherize;
			float Flatness1076 = ( 20.0 * _Spherize );
			float Scale1080 = ( _MainCloudScale * 0.1 );
			float mulTime61 = _Time.y * ( 0.001 * _WindSpeed );
			float Time152 = mulTime61;
			float2 Wind1035 = ( Time152 * _MainCloudWindDir );
			float2 temp_output_10_0_g40 = (( ( temp_output_21_0_g40 * ( pow( saferPower6_g40 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale1080 ) + Wind1035);
			float2 coords35_g40 = temp_output_10_0_g40 * 60.0;
			float2 id35_g40 = 0;
			float2 uv35_g40 = 0;
			float fade35_g40 = 0.5;
			float voroi35_g40 = 0;
			float rest35_g40 = 0;
			for( int it35_g40 = 0; it35_g40 <2; it35_g40++ ){
			voroi35_g40 += fade35_g40 * voronoi35_g40( coords35_g40, time35_g40, id35_g40, uv35_g40, 0,voronoiSmoothId0 );
			rest35_g40 += fade35_g40;
			coords35_g40 *= 2;
			fade35_g40 *= 0.5;
			}//Voronoi35_g40
			voroi35_g40 /= rest35_g40;
			float time13_g40 = 0.0;
			float2 coords13_g40 = temp_output_10_0_g40 * 25.0;
			float2 id13_g40 = 0;
			float2 uv13_g40 = 0;
			float fade13_g40 = 0.5;
			float voroi13_g40 = 0;
			float rest13_g40 = 0;
			for( int it13_g40 = 0; it13_g40 <2; it13_g40++ ){
			voroi13_g40 += fade13_g40 * voronoi13_g40( coords13_g40, time13_g40, id13_g40, uv13_g40, 0,voronoiSmoothId0 );
			rest13_g40 += fade13_g40;
			coords13_g40 *= 2;
			fade13_g40 *= 0.5;
			}//Voronoi13_g40
			voroi13_g40 /= rest13_g40;
			float time11_g40 = 17.23;
			float2 coords11_g40 = temp_output_10_0_g40 * 9.0;
			float2 id11_g40 = 0;
			float2 uv11_g40 = 0;
			float fade11_g40 = 0.5;
			float voroi11_g40 = 0;
			float rest11_g40 = 0;
			for( int it11_g40 = 0; it11_g40 <2; it11_g40++ ){
			voroi11_g40 += fade11_g40 * voronoi11_g40( coords11_g40, time11_g40, id11_g40, uv11_g40, 0,voronoiSmoothId0 );
			rest11_g40 += fade11_g40;
			coords11_g40 *= 2;
			fade11_g40 *= 0.5;
			}//Voronoi11_g40
			voroi11_g40 /= rest11_g40;
			float2 temp_output_1056_0 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult1057 = dot( temp_output_1056_0 , temp_output_1056_0 );
			float ModifiedCohesion1074 = ( _CloudCohesion * 1.0 * ( 1.0 - dotResult1057 ) );
			float lerpResult15_g40 = lerp( saturate( ( voroi35_g40 + voroi13_g40 ) ) , voroi11_g40 , ModifiedCohesion1074);
			float CumulusCoverage376 = ( _CumulusCoverageMultiplier * _MaxCloudCover );
			float lerpResult16_g40 = lerp( lerpResult15_g40 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
			float time35_g38 = 0.0;
			float2 temp_output_21_0_g38 = CentralUV998;
			float2 break2_g38 = abs( temp_output_21_0_g38 );
			float saferPower4_g38 = max( break2_g38.x , 0.0001 );
			float saferPower3_g38 = max( break2_g38.y , 0.0001 );
			float saferPower6_g38 = max( ( pow( saferPower4_g38 , 2.0 ) + pow( saferPower3_g38 , 2.0 ) ) , 0.0001 );
			float2 temp_output_10_0_g38 = (( ( temp_output_21_0_g38 * ( pow( saferPower6_g38 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale1080 ) + Wind1035);
			float2 coords35_g38 = temp_output_10_0_g38 * 60.0;
			float2 id35_g38 = 0;
			float2 uv35_g38 = 0;
			float fade35_g38 = 0.5;
			float voroi35_g38 = 0;
			float rest35_g38 = 0;
			for( int it35_g38 = 0; it35_g38 <2; it35_g38++ ){
			voroi35_g38 += fade35_g38 * voronoi35_g38( coords35_g38, time35_g38, id35_g38, uv35_g38, 0,voronoiSmoothId0 );
			rest35_g38 += fade35_g38;
			coords35_g38 *= 2;
			fade35_g38 *= 0.5;
			}//Voronoi35_g38
			voroi35_g38 /= rest35_g38;
			float time13_g38 = 0.0;
			float2 coords13_g38 = temp_output_10_0_g38 * 25.0;
			float2 id13_g38 = 0;
			float2 uv13_g38 = 0;
			float fade13_g38 = 0.5;
			float voroi13_g38 = 0;
			float rest13_g38 = 0;
			for( int it13_g38 = 0; it13_g38 <2; it13_g38++ ){
			voroi13_g38 += fade13_g38 * voronoi13_g38( coords13_g38, time13_g38, id13_g38, uv13_g38, 0,voronoiSmoothId0 );
			rest13_g38 += fade13_g38;
			coords13_g38 *= 2;
			fade13_g38 *= 0.5;
			}//Voronoi13_g38
			voroi13_g38 /= rest13_g38;
			float time11_g38 = 17.23;
			float2 coords11_g38 = temp_output_10_0_g38 * 9.0;
			float2 id11_g38 = 0;
			float2 uv11_g38 = 0;
			float fade11_g38 = 0.5;
			float voroi11_g38 = 0;
			float rest11_g38 = 0;
			for( int it11_g38 = 0; it11_g38 <2; it11_g38++ ){
			voroi11_g38 += fade11_g38 * voronoi11_g38( coords11_g38, time11_g38, id11_g38, uv11_g38, 0,voronoiSmoothId0 );
			rest11_g38 += fade11_g38;
			coords11_g38 *= 2;
			fade11_g38 *= 0.5;
			}//Voronoi11_g38
			voroi11_g38 /= rest11_g38;
			float lerpResult15_g38 = lerp( saturate( ( voroi35_g38 + voroi13_g38 ) ) , voroi11_g38 , ModifiedCohesion1074);
			float lerpResult16_g38 = lerp( lerpResult15_g38 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
			float temp_output_1054_0 = saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g38 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
			float IT1PreAlpha1159 = temp_output_1054_0;
			float time35_g39 = 0.0;
			float2 temp_output_21_0_g39 = CentralUV998;
			float2 break2_g39 = abs( temp_output_21_0_g39 );
			float saferPower4_g39 = max( break2_g39.x , 0.0001 );
			float saferPower3_g39 = max( break2_g39.y , 0.0001 );
			float saferPower6_g39 = max( ( pow( saferPower4_g39 , 2.0 ) + pow( saferPower3_g39 , 2.0 ) ) , 0.0001 );
			float2 temp_output_10_0_g39 = (( ( temp_output_21_0_g39 * ( pow( saferPower6_g39 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale1080 * 1.5 ) ) + ( Wind1035 * float2( 0.5,0.5 ) ));
			float2 coords35_g39 = temp_output_10_0_g39 * 60.0;
			float2 id35_g39 = 0;
			float2 uv35_g39 = 0;
			float fade35_g39 = 0.5;
			float voroi35_g39 = 0;
			float rest35_g39 = 0;
			for( int it35_g39 = 0; it35_g39 <2; it35_g39++ ){
			voroi35_g39 += fade35_g39 * voronoi35_g39( coords35_g39, time35_g39, id35_g39, uv35_g39, 0,voronoiSmoothId0 );
			rest35_g39 += fade35_g39;
			coords35_g39 *= 2;
			fade35_g39 *= 0.5;
			}//Voronoi35_g39
			voroi35_g39 /= rest35_g39;
			float time13_g39 = 0.0;
			float2 coords13_g39 = temp_output_10_0_g39 * 25.0;
			float2 id13_g39 = 0;
			float2 uv13_g39 = 0;
			float fade13_g39 = 0.5;
			float voroi13_g39 = 0;
			float rest13_g39 = 0;
			for( int it13_g39 = 0; it13_g39 <2; it13_g39++ ){
			voroi13_g39 += fade13_g39 * voronoi13_g39( coords13_g39, time13_g39, id13_g39, uv13_g39, 0,voronoiSmoothId0 );
			rest13_g39 += fade13_g39;
			coords13_g39 *= 2;
			fade13_g39 *= 0.5;
			}//Voronoi13_g39
			voroi13_g39 /= rest13_g39;
			float time11_g39 = 17.23;
			float2 coords11_g39 = temp_output_10_0_g39 * 9.0;
			float2 id11_g39 = 0;
			float2 uv11_g39 = 0;
			float fade11_g39 = 0.5;
			float voroi11_g39 = 0;
			float rest11_g39 = 0;
			for( int it11_g39 = 0; it11_g39 <2; it11_g39++ ){
			voroi11_g39 += fade11_g39 * voronoi11_g39( coords11_g39, time11_g39, id11_g39, uv11_g39, 0,voronoiSmoothId0 );
			rest11_g39 += fade11_g39;
			coords11_g39 *= 2;
			fade11_g39 *= 0.5;
			}//Voronoi11_g39
			voroi11_g39 /= rest11_g39;
			float lerpResult15_g39 = lerp( saturate( ( voroi35_g39 + voroi13_g39 ) ) , voroi11_g39 , ( ModifiedCohesion1074 * 1.1 ));
			float lerpResult16_g39 = lerp( lerpResult15_g39 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
			float temp_output_1183_0 = saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g39 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
			float IT2PreAlpha1184 = temp_output_1183_0;
			float temp_output_1143_0 = (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g40 ) ) - 0.6) * (max( IT1PreAlpha1159 , IT2PreAlpha1184 ) - 0.0) / (1.5 - 0.6));
			float clampResult1158 = clamp( temp_output_1143_0 , 0.0 , 0.9 );
			float AdditionalLayer1147 = SampleGradient( gradient1145, clampResult1158 ).r;
			float4 lerpResult1150 = lerp( CloudColor332 , ( CloudColor332 * _SecondLayer ) , AdditionalLayer1147);
			float4 ModifiedCloudColor1165 = lerpResult1150;
			Gradient gradient1014 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4411841 ), float4( 1, 1, 1, 0.5794156 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float time35_g41 = 0.0;
			float2 ShadowUV997 = ( CentralUV998 + ( CentralUV998 * float2( -1,-1 ) * _ShadowingDistance * Dot1071 ) );
			float2 temp_output_21_0_g41 = ShadowUV997;
			float2 break2_g41 = abs( temp_output_21_0_g41 );
			float saferPower4_g41 = max( break2_g41.x , 0.0001 );
			float saferPower3_g41 = max( break2_g41.y , 0.0001 );
			float saferPower6_g41 = max( ( pow( saferPower4_g41 , 2.0 ) + pow( saferPower3_g41 , 2.0 ) ) , 0.0001 );
			float2 temp_output_10_0_g41 = (( ( temp_output_21_0_g41 * ( pow( saferPower6_g41 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale1080 ) + Wind1035);
			float2 coords35_g41 = temp_output_10_0_g41 * 60.0;
			float2 id35_g41 = 0;
			float2 uv35_g41 = 0;
			float fade35_g41 = 0.5;
			float voroi35_g41 = 0;
			float rest35_g41 = 0;
			for( int it35_g41 = 0; it35_g41 <2; it35_g41++ ){
			voroi35_g41 += fade35_g41 * voronoi35_g41( coords35_g41, time35_g41, id35_g41, uv35_g41, 0,voronoiSmoothId0 );
			rest35_g41 += fade35_g41;
			coords35_g41 *= 2;
			fade35_g41 *= 0.5;
			}//Voronoi35_g41
			voroi35_g41 /= rest35_g41;
			float time13_g41 = 0.0;
			float2 coords13_g41 = temp_output_10_0_g41 * 25.0;
			float2 id13_g41 = 0;
			float2 uv13_g41 = 0;
			float fade13_g41 = 0.5;
			float voroi13_g41 = 0;
			float rest13_g41 = 0;
			for( int it13_g41 = 0; it13_g41 <2; it13_g41++ ){
			voroi13_g41 += fade13_g41 * voronoi13_g41( coords13_g41, time13_g41, id13_g41, uv13_g41, 0,voronoiSmoothId0 );
			rest13_g41 += fade13_g41;
			coords13_g41 *= 2;
			fade13_g41 *= 0.5;
			}//Voronoi13_g41
			voroi13_g41 /= rest13_g41;
			float time11_g41 = 17.23;
			float2 coords11_g41 = temp_output_10_0_g41 * 9.0;
			float2 id11_g41 = 0;
			float2 uv11_g41 = 0;
			float fade11_g41 = 0.5;
			float voroi11_g41 = 0;
			float rest11_g41 = 0;
			for( int it11_g41 = 0; it11_g41 <2; it11_g41++ ){
			voroi11_g41 += fade11_g41 * voronoi11_g41( coords11_g41, time11_g41, id11_g41, uv11_g41, 0,voronoiSmoothId0 );
			rest11_g41 += fade11_g41;
			coords11_g41 *= 2;
			fade11_g41 *= 0.5;
			}//Voronoi11_g41
			voroi11_g41 /= rest11_g41;
			float lerpResult15_g41 = lerp( saturate( ( voroi35_g41 + voroi13_g41 ) ) , voroi11_g41 , ModifiedCohesion1074);
			float lerpResult16_g41 = lerp( lerpResult15_g41 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
			float4 lerpResult989 = lerp( CloudHighlightColor334 , ModifiedCloudColor1165 , saturate( SampleGradient( gradient1014, saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g41 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) ) ).r ));
			float4 IT1Color923 = lerpResult989;
			Gradient gradient1198 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4411841 ), float4( 1, 1, 1, 0.5794156 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float time35_g42 = 0.0;
			float2 temp_output_21_0_g42 = ShadowUV997;
			float2 break2_g42 = abs( temp_output_21_0_g42 );
			float saferPower4_g42 = max( break2_g42.x , 0.0001 );
			float saferPower3_g42 = max( break2_g42.y , 0.0001 );
			float saferPower6_g42 = max( ( pow( saferPower4_g42 , 2.0 ) + pow( saferPower3_g42 , 2.0 ) ) , 0.0001 );
			float2 temp_output_10_0_g42 = (( ( temp_output_21_0_g42 * ( pow( saferPower6_g42 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale1080 * 1.5 ) ) + ( Wind1035 * float2( 0.5,0.5 ) ));
			float2 coords35_g42 = temp_output_10_0_g42 * 60.0;
			float2 id35_g42 = 0;
			float2 uv35_g42 = 0;
			float fade35_g42 = 0.5;
			float voroi35_g42 = 0;
			float rest35_g42 = 0;
			for( int it35_g42 = 0; it35_g42 <2; it35_g42++ ){
			voroi35_g42 += fade35_g42 * voronoi35_g42( coords35_g42, time35_g42, id35_g42, uv35_g42, 0,voronoiSmoothId0 );
			rest35_g42 += fade35_g42;
			coords35_g42 *= 2;
			fade35_g42 *= 0.5;
			}//Voronoi35_g42
			voroi35_g42 /= rest35_g42;
			float time13_g42 = 0.0;
			float2 coords13_g42 = temp_output_10_0_g42 * 25.0;
			float2 id13_g42 = 0;
			float2 uv13_g42 = 0;
			float fade13_g42 = 0.5;
			float voroi13_g42 = 0;
			float rest13_g42 = 0;
			for( int it13_g42 = 0; it13_g42 <2; it13_g42++ ){
			voroi13_g42 += fade13_g42 * voronoi13_g42( coords13_g42, time13_g42, id13_g42, uv13_g42, 0,voronoiSmoothId0 );
			rest13_g42 += fade13_g42;
			coords13_g42 *= 2;
			fade13_g42 *= 0.5;
			}//Voronoi13_g42
			voroi13_g42 /= rest13_g42;
			float time11_g42 = 17.23;
			float2 coords11_g42 = temp_output_10_0_g42 * 9.0;
			float2 id11_g42 = 0;
			float2 uv11_g42 = 0;
			float fade11_g42 = 0.5;
			float voroi11_g42 = 0;
			float rest11_g42 = 0;
			for( int it11_g42 = 0; it11_g42 <2; it11_g42++ ){
			voroi11_g42 += fade11_g42 * voronoi11_g42( coords11_g42, time11_g42, id11_g42, uv11_g42, 0,voronoiSmoothId0 );
			rest11_g42 += fade11_g42;
			coords11_g42 *= 2;
			fade11_g42 *= 0.5;
			}//Voronoi11_g42
			voroi11_g42 /= rest11_g42;
			float lerpResult15_g42 = lerp( saturate( ( voroi35_g42 + voroi13_g42 ) ) , voroi11_g42 , ( ModifiedCohesion1074 * 1.1 ));
			float lerpResult16_g42 = lerp( lerpResult15_g42 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
			float4 lerpResult1206 = lerp( CloudHighlightColor334 , ModifiedCloudColor1165 , saturate( SampleGradient( gradient1198, saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g42 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) ) ).r ));
			float4 IT2Color1207 = lerpResult1206;
			Gradient gradient1199 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float IT2Alpha1202 = SampleGradient( gradient1199, temp_output_1183_0 ).r;
			float4 lerpResult1218 = lerp( ( _AltoCloudColor * IT1Color923 ) , IT2Color1207 , IT2Alpha1202);
			o.Emission = lerpResult1218.rgb;
			Gradient gradient1021 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float IT1Alpha953 = SampleGradient( gradient1021, temp_output_1054_0 ).r;
			float temp_output_1216_0 = max( IT1Alpha953 , IT2Alpha1202 );
			o.Alpha = saturate( ( temp_output_1216_0 + ( temp_output_1216_0 * 2.0 * _CloudThickness ) ) );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows exclude_path:deferred 

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
				float3 worldPos : TEXCOORD2;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
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
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
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
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18912
0;1080;2194;606;4793.952;2149.887;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;372;-4450.156,-2723.177;Inherit;False;2555.466;1283.535;;51;997;1095;985;986;1094;1071;1052;1078;998;1051;1083;1076;1161;1080;1035;1074;947;1045;956;931;1043;925;1036;955;1058;1042;1040;906;1059;1041;1057;1056;1055;1222;376;796;375;159;797;94;334;52;332;36;152;61;150;70;1223;1230;1231;Variable Declaration;0.6196079,0.9508546,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-3519.35,-2327.424;Inherit;False;Property;_WindSpeed;Wind Speed;3;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1055;-3408,-1696;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-3367.351,-2349.156;Inherit;False;2;2;0;FLOAT;0.001;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;61;-3234.401,-2345.619;Inherit;False;1;0;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;1056;-3184,-1696;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-3065.96,-2349.724;Inherit;False;Time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;1057;-3056,-1696;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;906;-3104,-1824;Inherit;False;Property;_CloudCohesion;Cloud Cohesion;6;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.887;0.837;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1042;-4304,-2064;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1040;-2688,-2400;Inherit;False;152;Time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;925;-4431,-1809;Inherit;False;Property;_MainCloudScale;Main Cloud Scale;7;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.8;30;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1059;-2928,-1696;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;1041;-2720,-2288;Inherit;False;Property;_MainCloudWindDir;Main Cloud Wind Dir;11;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.1,0.2;0.3,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;1043;-4080,-2064;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;1168;-1275.921,-2528.838;Inherit;False;2636.823;1492.163;;2;1170;1169;Iteration 2;1,0.8737146,0.572549,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1058;-2784,-1744;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;955;-3200,-2080;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1230;-4291.664,-1807.987;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;947;-3936,-1808;Inherit;False;Property;_Spherize;Spherize;8;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.36;0.361;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1223;-2789.537,-2521.582;Inherit;False;Property;_MaxCloudCover;Max Cloud Cover;5;1;[HideInInspector];Create;True;1;Cumulus Clouds;0;0;False;0;False;1;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;375;-2788.653,-2608.27;Inherit;False;Property;_CumulusCoverageMultiplier;Cumulus Coverage Multiplier;4;2;[HideInInspector];[Header];Create;True;1;Cumulus Clouds;0;0;False;0;False;1;0.818;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1036;-2464,-2368;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1035;-2288,-2384;Inherit;False;Wind;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1074;-2656,-1744;Inherit;False;ModifiedCohesion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1231;-3624.209,-1901.833;Inherit;False;2;2;0;FLOAT;20;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1162;-1291.921,-4336.837;Inherit;False;2487.393;1546.128;;2;1164;1163;Iteration 1;0.6466299,1,0.5707547,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1080;-4160,-1808;Inherit;False;Scale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;1045;-3952,-2064;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;956;-2976,-2080;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;-0.5,-0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;1169;-1211.921,-2464.838;Inherit;False;2070.976;624.3994;Alpha;20;1202;1201;1199;1184;1183;1182;1181;1179;1180;1178;1212;1211;1210;1173;1176;1175;1172;1177;1171;1174;;1,0.8737146,0.572549,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1222;-2496,-2576;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1078;-3664,-1808;Inherit;False;Spherize;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;1051;-3824,-2064;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.85;False;4;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;376;-2288,-2576;Inherit;False;CumulusCoverage;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1177;-1179.921,-2016.838;Inherit;False;1074;ModifiedCohesion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1163;-1227.921,-4304.837;Inherit;False;1970.693;633.926;IT1 Alpha;17;1034;953;1017;1021;1159;1054;1053;1046;1016;1072;1109;1075;1079;1084;999;1077;1081;;0.6466299,1,0.5707547,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;1171;-1179.921,-2416.838;Inherit;False;1035;Wind;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;998;-2832,-2080;Inherit;False;CentralUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1076;-3312,-1824;Inherit;False;Flatness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1174;-1179.921,-2096.838;Inherit;False;1080;Scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;999;-1195.921,-4176.837;Inherit;False;998;CentralUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1084;-1195.921,-3776.838;Inherit;False;376;CumulusCoverage;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1212;-969.675,-2025.596;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1077;-1195.921,-4096.837;Inherit;False;1076;Flatness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1172;-1179.921,-2256.838;Inherit;False;1076;Flatness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1075;-1195.921,-3856.839;Inherit;False;1074;ModifiedCohesion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1034;-1195.921,-4256.837;Inherit;False;1035;Wind;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1176;-1179.921,-2336.838;Inherit;False;998;CentralUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1173;-1179.921,-1936.838;Inherit;False;376;CumulusCoverage;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1211;-972.675,-2119.596;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1052;-3648,-2064;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1210;-985.2195,-2413.893;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1175;-1179.921,-2176.838;Inherit;False;1078;Spherize;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1081;-1195.921,-3936.839;Inherit;False;1080;Scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1079;-1195.921,-4016.839;Inherit;False;1078;Spherize;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1109;-875.9211,-4128.837;Inherit;True;Ghibli Clouds;-1;;38;bce7362c867d47d49a15818b7e6650d4;0;7;37;FLOAT2;0,0;False;21;FLOAT2;0,0;False;19;FLOAT;1;False;20;FLOAT;1;False;23;FLOAT;1;False;24;FLOAT;0;False;27;FLOAT;0.5;False;2;FLOAT;33;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1178;-763.9213,-2272.838;Inherit;True;Ghibli Clouds;-1;;39;bce7362c867d47d49a15818b7e6650d4;0;7;37;FLOAT2;0,0;False;21;FLOAT2;0,0;False;19;FLOAT;1;False;20;FLOAT;1;False;23;FLOAT;1;False;24;FLOAT;0;False;27;FLOAT;0.5;False;2;FLOAT;33;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1071;-3472,-2064;Inherit;False;Dot;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1072;-475.9215,-4208.837;Inherit;False;1071;Dot;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1179;-363.9215,-2272.838;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1016;-475.9215,-4128.837;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1180;-363.9215,-2352.838;Inherit;False;1071;Dot;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1167;1522.421,-2232.243;Inherit;False;2326.557;1124.512;;27;1150;1165;1154;1152;1127;1151;1144;1147;1146;1158;1145;1143;1142;1160;1141;1140;1139;1133;1134;1138;1149;1135;1137;1136;1132;1208;1209;Additional Layer;0.7721605,0.4669811,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1046;-283.9215,-4160.837;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1181;-171.9215,-2304.838;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;1182;-27.92151,-2304.838;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.6;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1132;1572.821,-1702.374;Inherit;False;998;CentralUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;1053;-139.9215,-4160.837;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.6;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1183;148.0785,-2304.838;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1054;36.0785,-4160.837;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;986;-2912,-1984;Inherit;False;Property;_ShadowingDistance;Shadowing Distance;10;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.07;0.0288;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1138;1732.821,-1318.374;Inherit;False;1074;ModifiedCohesion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1133;1732.821,-1238.374;Inherit;False;376;CumulusCoverage;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1137;1732.821,-1558.374;Inherit;False;1076;Flatness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1135;1732.821,-1478.374;Inherit;False;1078;Spherize;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1136;1732.821,-1766.374;Inherit;False;1035;Wind;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;1149;1732.821,-1686.374;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.58;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1094;-2800,-1904;Inherit;False;1071;Dot;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1134;1732.821,-1398.374;Inherit;False;1080;Scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;985;-2560,-2016;Inherit;True;4;4;0;FLOAT2;0,0;False;1;FLOAT2;-1,-1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1159;196.0785,-4032.839;Inherit;False;IT1PreAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1184;308.0785,-2176.838;Inherit;False;IT2PreAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1139;2020.822,-1654.374;Inherit;True;Ghibli Clouds;-1;;40;bce7362c867d47d49a15818b7e6650d4;0;7;37;FLOAT2;0,0;False;21;FLOAT2;0,0;False;19;FLOAT;1;False;20;FLOAT;1;False;23;FLOAT;1;False;24;FLOAT;0;False;27;FLOAT;0.5;False;2;FLOAT;33;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1209;2452.822,-1462.374;Inherit;False;1184;IT2PreAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1141;2420.822,-1638.374;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1095;-2304,-2080;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1140;2420.822,-1718.374;Inherit;False;1071;Dot;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1160;2452.822,-1542.374;Inherit;False;1159;IT1PreAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1170;-1195.921,-1808.838;Inherit;False;2506.716;730.6439;Color;23;1207;1206;1203;1205;1204;1200;1198;1197;1196;1195;1193;1194;1192;1214;1213;1188;1185;1186;1215;1191;1190;1189;1187;;1,0.8737146,0.572549,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;997;-2192,-2080;Inherit;False;ShadowUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;1208;2650.741,-1535.378;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1164;-1211.921,-3648.838;Inherit;False;2346.81;781.6527;IT1 Color;20;1029;923;989;1166;1031;1012;1062;1014;1061;1060;1073;1013;1108;1088;1086;1092;1089;1091;1090;1085;;0.6466299,1,0.5707547,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1142;2612.822,-1686.374;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1187;-1131.921,-1296.838;Inherit;False;1074;ModifiedCohesion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1086;-1147.921,-3536.838;Inherit;False;1035;Wind;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1088;-1147.921,-3296.838;Inherit;False;1076;Flatness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1085;-1147.921,-3456.838;Inherit;False;997;ShadowUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;1143;2804.822,-1686.374;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.6;False;2;FLOAT;1.5;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1090;-1147.921,-3216.838;Inherit;False;1080;Scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1089;-1147.921,-3376.838;Inherit;False;1078;Spherize;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1091;-1147.921,-3136.838;Inherit;False;1074;ModifiedCohesion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1190;-1131.921,-1376.838;Inherit;False;1080;Scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1189;-1131.921,-1696.838;Inherit;False;1035;Wind;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1092;-1147.921,-3056.838;Inherit;False;376;CumulusCoverage;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1108;-843.9212,-3360.838;Inherit;True;Ghibli Clouds;-1;;41;bce7362c867d47d49a15818b7e6650d4;0;7;37;FLOAT2;0,0;False;21;FLOAT2;0,0;False;19;FLOAT;1;False;20;FLOAT;1;False;23;FLOAT;1;False;24;FLOAT;0;False;27;FLOAT;0.5;False;2;FLOAT;33;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;1158;2996.822,-1606.374;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1191;-1131.921,-1616.838;Inherit;False;997;ShadowUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1214;-891.9211,-1296.838;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;-4018.127,-2611.486;Inherit;False;Property;_CloudColor;Cloud Color;0;3;[HideInInspector];[HDR];[Header];Create;True;1;General Cloud Settings;0;0;False;0;False;0.7264151,0.7264151,0.7264151,0;0.449027,0.4865523,0.5068226,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1186;-1131.921,-1536.838;Inherit;False;1078;Spherize;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1185;-1131.921,-1216.837;Inherit;False;376;CumulusCoverage;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1213;-891.9211,-1392.838;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1215;-907.9211,-1664.838;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1188;-1131.921,-1456.838;Inherit;False;1076;Flatness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;1145;2980.822,-1750.374;Inherit;False;0;2;2;0.06119964,0.06119964,0.06119964,0.5411765;1,1,1,0.6441138;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.OneMinusNode;1013;-491.9215,-3360.838;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1192;-667.9213,-1520.838;Inherit;True;Ghibli Clouds;-1;;42;bce7362c867d47d49a15818b7e6650d4;0;7;37;FLOAT2;0,0;False;21;FLOAT2;0,0;False;19;FLOAT;1;False;20;FLOAT;1;False;23;FLOAT;1;False;24;FLOAT;0;False;27;FLOAT;0.5;False;2;FLOAT;33;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;332;-3793.636,-2612.531;Inherit;False;CloudColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1073;-491.9215,-3440.838;Inherit;False;1071;Dot;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;1146;3204.822,-1734.374;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1151;1956.822,-2038.374;Inherit;False;Property;_SecondLayer;Second Layer;12;2;[HideInInspector];[HDR];Create;True;0;0;0;False;0;False;0.8396226,0.8396226,0.8396226,0;0.9056604,0.9056604,0.9056604,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1194;-315.9215,-1600.838;Inherit;False;1071;Dot;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1127;2036.822,-2150.374;Inherit;False;332;CloudColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;1193;-315.9215,-1520.838;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1060;-315.9215,-3376.838;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1147;3524.822,-1718.374;Inherit;False;AdditionalLayer;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1195;-139.9215,-1536.838;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1152;2086.653,-1862.374;Inherit;False;1147;AdditionalLayer;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1154;2180.822,-2070.374;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;1061;-171.9215,-3376.838;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.6;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;52;-4015.761,-2428.211;Inherit;False;Property;_CloudHighlightColor;Cloud Highlight Color;2;2;[HideInInspector];[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;1.475943,1.4402,1.429988,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientNode;1014;-59.92148,-3456.838;Inherit;False;0;2;2;0.06119964,0.06119964,0.06119964,0.4411841;1,1,1,0.5794156;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.LerpOp;1150;2372.822,-2086.374;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;1062;4.078499,-3376.838;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;1021;-27.92151,-4240.837;Inherit;False;0;2;2;0.06119964,0.06119964,0.06119964,0.4617685;1,1,1,0.5117723;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.TFHCRemapNode;1196;4.078499,-1536.838;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.6;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;1199;84.07853,-2384.838;Inherit;False;0;2;2;0.06119964,0.06119964,0.06119964,0.4617685;1,1,1,0.5117723;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.GradientSampleNode;1012;148.0785,-3424.838;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;1197;180.0785,-1536.838;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;1201;308.0785,-2368.838;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;334;-3796.106,-2428.746;Inherit;False;CloudHighlightColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientNode;1198;116.0785,-1616.838;Inherit;False;0;2;2;0.06119964,0.06119964,0.06119964,0.4411841;1,1,1,0.5794156;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.GradientSampleNode;1017;196.0785,-4224.837;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1165;2644.822,-2086.374;Inherit;False;ModifiedCloudColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1166;468.0785,-3472.838;Inherit;False;1165;ModifiedCloudColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientSampleNode;1200;324.0785,-1584.838;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;1029;548.0785,-3360.838;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1031;468.0785,-3552.838;Inherit;False;334;CloudHighlightColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1202;628.0785,-2352.838;Inherit;False;IT2Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;953;516.0785,-4208.837;Inherit;False;IT1Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1203;724.0786,-1520.838;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1217;-1488,-128;Inherit;False;1202;IT2Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1204;644.0785,-1712.838;Inherit;False;334;CloudHighlightColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;989;756.0786,-3424.838;Inherit;False;3;0;COLOR;1,1,1,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1205;644.0785,-1632.838;Inherit;False;1165;ModifiedCloudColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;799;-1472,-256;Inherit;False;953;IT1Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;1216;-1264,-176;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1226;-1359.246,-31.71698;Inherit;False;Property;_CloudThickness;CloudThickness;14;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;2.534;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;923;916.0785,-3424.838;Inherit;False;IT1Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;1206;932.0785,-1584.838;Inherit;False;3;0;COLOR;1,1,1,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1220;-1466.563,-661.4504;Inherit;False;Property;_AltoCloudColor;Alto Cloud Color;13;1;[HDR];Create;True;0;0;0;False;0;False;0.8160377,0.9787034,1,0;0.5198421,0.6047655,0.6337125,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1227;-1071.246,-111.717;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;352;-1420,-496.0308;Inherit;False;923;IT1Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1207;1092.078,-1584.838;Inherit;False;IT2Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1228;-943.2463,-175.717;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1219;-1240.733,-407.013;Inherit;False;1207;IT2Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1221;-1207.563,-518.4504;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;1229;-815.2463,-175.717;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1218;-1026.733,-468.013;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;797;-3329.596,-2629.492;Inherit;False;MoonlightColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;796;-3549.25,-2628.957;Inherit;False;Property;_MoonColor;Moon Color;1;2;[HideInInspector];[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;0.1100036,0.2264151,0.2252752,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;159;-3070.588,-2523.448;Inherit;False;Pos;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;1144;3012.822,-1686.374;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1161;-3872,-1648;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;94;-3274.025,-2518.204;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1083;-3728,-1664;Inherit;False;Coverage;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;931;-4192,-1664;Inherit;False;Property;_Coverage;Coverage;9;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.3574152;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-670.0242,-403.7039;Float;False;True;-1;2;;0;0;Unlit;Distant Lands/Cozy/Stylized Clouds Ghibli;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Front;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;-50;False;Transparent;;Transparent;ForwardOnly;18;all;True;True;True;True;0;False;-1;True;221;False;-1;255;False;-1;255;False;-1;7;False;-1;3;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;150;1;70;0
WireConnection;61;0;150;0
WireConnection;1056;0;1055;0
WireConnection;152;0;61;0
WireConnection;1057;0;1056;0
WireConnection;1057;1;1056;0
WireConnection;1059;0;1057;0
WireConnection;1043;0;1042;0
WireConnection;1058;0;906;0
WireConnection;1058;2;1059;0
WireConnection;1230;0;925;0
WireConnection;1036;0;1040;0
WireConnection;1036;1;1041;0
WireConnection;1035;0;1036;0
WireConnection;1074;0;1058;0
WireConnection;1231;1;947;0
WireConnection;1080;0;1230;0
WireConnection;1045;0;1043;0
WireConnection;1045;1;1043;0
WireConnection;956;0;955;0
WireConnection;1222;0;375;0
WireConnection;1222;1;1223;0
WireConnection;1078;0;947;0
WireConnection;1051;0;1045;0
WireConnection;376;0;1222;0
WireConnection;998;0;956;0
WireConnection;1076;0;1231;0
WireConnection;1212;0;1177;0
WireConnection;1211;0;1174;0
WireConnection;1052;0;1051;0
WireConnection;1210;0;1171;0
WireConnection;1109;37;1034;0
WireConnection;1109;21;999;0
WireConnection;1109;19;1079;0
WireConnection;1109;20;1077;0
WireConnection;1109;23;1081;0
WireConnection;1109;24;1075;0
WireConnection;1109;27;1084;0
WireConnection;1178;37;1210;0
WireConnection;1178;21;1176;0
WireConnection;1178;19;1175;0
WireConnection;1178;20;1172;0
WireConnection;1178;23;1211;0
WireConnection;1178;24;1212;0
WireConnection;1178;27;1173;0
WireConnection;1071;0;1052;0
WireConnection;1179;0;1178;33
WireConnection;1016;0;1109;33
WireConnection;1046;0;1072;0
WireConnection;1046;1;1016;0
WireConnection;1181;0;1180;0
WireConnection;1181;1;1179;0
WireConnection;1182;0;1181;0
WireConnection;1053;0;1046;0
WireConnection;1183;0;1182;0
WireConnection;1054;0;1053;0
WireConnection;1149;0;1132;0
WireConnection;985;0;998;0
WireConnection;985;2;986;0
WireConnection;985;3;1094;0
WireConnection;1159;0;1054;0
WireConnection;1184;0;1183;0
WireConnection;1139;37;1136;0
WireConnection;1139;21;1149;0
WireConnection;1139;19;1135;0
WireConnection;1139;20;1137;0
WireConnection;1139;23;1134;0
WireConnection;1139;24;1138;0
WireConnection;1139;27;1133;0
WireConnection;1141;0;1139;33
WireConnection;1095;0;998;0
WireConnection;1095;1;985;0
WireConnection;997;0;1095;0
WireConnection;1208;0;1160;0
WireConnection;1208;1;1209;0
WireConnection;1142;0;1140;0
WireConnection;1142;1;1141;0
WireConnection;1143;0;1142;0
WireConnection;1143;4;1208;0
WireConnection;1108;37;1086;0
WireConnection;1108;21;1085;0
WireConnection;1108;19;1089;0
WireConnection;1108;20;1088;0
WireConnection;1108;23;1090;0
WireConnection;1108;24;1091;0
WireConnection;1108;27;1092;0
WireConnection;1158;0;1143;0
WireConnection;1214;0;1187;0
WireConnection;1213;0;1190;0
WireConnection;1215;0;1189;0
WireConnection;1013;0;1108;33
WireConnection;1192;37;1215;0
WireConnection;1192;21;1191;0
WireConnection;1192;19;1186;0
WireConnection;1192;20;1188;0
WireConnection;1192;23;1213;0
WireConnection;1192;24;1214;0
WireConnection;1192;27;1185;0
WireConnection;332;0;36;0
WireConnection;1146;0;1145;0
WireConnection;1146;1;1158;0
WireConnection;1193;0;1192;33
WireConnection;1060;0;1073;0
WireConnection;1060;1;1013;0
WireConnection;1147;0;1146;1
WireConnection;1195;0;1194;0
WireConnection;1195;1;1193;0
WireConnection;1154;0;1127;0
WireConnection;1154;1;1151;0
WireConnection;1061;0;1060;0
WireConnection;1150;0;1127;0
WireConnection;1150;1;1154;0
WireConnection;1150;2;1152;0
WireConnection;1062;0;1061;0
WireConnection;1196;0;1195;0
WireConnection;1012;0;1014;0
WireConnection;1012;1;1062;0
WireConnection;1197;0;1196;0
WireConnection;1201;0;1199;0
WireConnection;1201;1;1183;0
WireConnection;334;0;52;0
WireConnection;1017;0;1021;0
WireConnection;1017;1;1054;0
WireConnection;1165;0;1150;0
WireConnection;1200;0;1198;0
WireConnection;1200;1;1197;0
WireConnection;1029;0;1012;1
WireConnection;1202;0;1201;1
WireConnection;953;0;1017;1
WireConnection;1203;0;1200;1
WireConnection;989;0;1031;0
WireConnection;989;1;1166;0
WireConnection;989;2;1029;0
WireConnection;1216;0;799;0
WireConnection;1216;1;1217;0
WireConnection;923;0;989;0
WireConnection;1206;0;1204;0
WireConnection;1206;1;1205;0
WireConnection;1206;2;1203;0
WireConnection;1227;0;1216;0
WireConnection;1227;2;1226;0
WireConnection;1207;0;1206;0
WireConnection;1228;0;1216;0
WireConnection;1228;1;1227;0
WireConnection;1221;0;1220;0
WireConnection;1221;1;352;0
WireConnection;1229;0;1228;0
WireConnection;1218;0;1221;0
WireConnection;1218;1;1219;0
WireConnection;1218;2;1217;0
WireConnection;797;0;796;0
WireConnection;159;0;94;0
WireConnection;1144;0;1143;0
WireConnection;1161;0;931;0
WireConnection;1083;0;1161;0
WireConnection;0;2;1218;0
WireConnection;0;9;1229;0
ASEEND*/
//CHKSM=CEC25869B83F4ED1B9FC02BA5C0E44818C5D4F5C