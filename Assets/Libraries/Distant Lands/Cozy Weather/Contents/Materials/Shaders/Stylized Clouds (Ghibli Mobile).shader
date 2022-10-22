// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/Stylized Clouds Ghibli Mobile"
{
	Properties
	{
		[HideInInspector][HDR][Header(General Cloud Settings)]_CloudColor("Cloud Color", Color) = (0.7264151,0.7264151,0.7264151,0)
		[HideInInspector][HDR]_CloudHighlightColor("Cloud Highlight Color", Color) = (1,1,1,0)
		[HideInInspector]_WindSpeed("Wind Speed", Float) = 0
		[HideInInspector][Header(Cumulus Clouds)]_CumulusCoverageMultiplier("Cumulus Coverage Multiplier", Range( 0 , 2)) = 1
		[HideInInspector]_CloudCohesion("Cloud Cohesion", Range( 0 , 1)) = 0.887
		[HideInInspector]_MainCloudScale("Main Cloud Scale", Float) = 0.8
		[HideInInspector]_Spherize("Spherize", Range( 0 , 1)) = 0.36
		[HideInInspector]_ClippingThreshold("Clipping Threshold", Range( 0 , 1)) = 0
		[HideInInspector]_MaxCloudCover("Max Cloud Cover", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent-50" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		Stencil
		{
			Ref 221
			Comp Always
			Pass Replace
		}
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _CloudHighlightColor;
		uniform float4 _CloudColor;
		uniform float _Spherize;
		uniform float _MainCloudScale;
		uniform float _WindSpeed;
		uniform float _CloudCohesion;
		uniform float _CumulusCoverageMultiplier;
		uniform float _MaxCloudCover;
		uniform float _ClippingThreshold;


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


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 CloudHighlightColor334 = _CloudHighlightColor;
			float4 CloudColor332 = _CloudColor;
			float2 temp_output_1043_0 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult1045 = dot( temp_output_1043_0 , temp_output_1043_0 );
			float RawDot1238 = dotResult1045;
			Gradient gradient1198 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.3029374 ), float4( 1, 1, 1, 0.520592 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float Dot1071 = saturate( (0.85 + (dotResult1045 - 0.0) * (3.0 - 0.85) / (1.0 - 0.0)) );
			float time35_g41 = 0.0;
			float2 voronoiSmoothId35_g41 = 0;
			float2 CentralUV998 = ( i.uv_texcoord + float2( -0.5,-0.5 ) );
			float2 temp_output_21_0_g41 = CentralUV998;
			float2 break2_g41 = abs( temp_output_21_0_g41 );
			float saferPower4_g41 = abs( break2_g41.x );
			float saferPower3_g41 = abs( break2_g41.y );
			float saferPower6_g41 = abs( ( pow( saferPower4_g41 , 2.0 ) + pow( saferPower3_g41 , 2.0 ) ) );
			float Spherize1078 = _Spherize;
			float Flatness1076 = ( 20.0 * _Spherize );
			float Scale1080 = ( _MainCloudScale * 0.1 );
			float mulTime61 = _Time.y * ( 0.001 * _WindSpeed );
			float Time152 = mulTime61;
			float2 Wind1035 = ( Time152 * float2( 0.1,0.2 ) );
			float2 temp_output_10_0_g41 = (( ( temp_output_21_0_g41 * ( pow( saferPower6_g41 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale1080 * 1.5 ) ) + ( Wind1035 * float2( 0.5,0.5 ) ));
			float2 coords35_g41 = temp_output_10_0_g41 * 60.0;
			float2 id35_g41 = 0;
			float2 uv35_g41 = 0;
			float fade35_g41 = 0.5;
			float voroi35_g41 = 0;
			float rest35_g41 = 0;
			for( int it35_g41 = 0; it35_g41 <2; it35_g41++ ){
			voroi35_g41 += fade35_g41 * voronoi35_g41( coords35_g41, time35_g41, id35_g41, uv35_g41, 0,voronoiSmoothId35_g41 );
			rest35_g41 += fade35_g41;
			coords35_g41 *= 2;
			fade35_g41 *= 0.5;
			}//Voronoi35_g41
			voroi35_g41 /= rest35_g41;
			float time13_g41 = 0.0;
			float2 voronoiSmoothId13_g41 = 0;
			float2 coords13_g41 = temp_output_10_0_g41 * 25.0;
			float2 id13_g41 = 0;
			float2 uv13_g41 = 0;
			float fade13_g41 = 0.5;
			float voroi13_g41 = 0;
			float rest13_g41 = 0;
			for( int it13_g41 = 0; it13_g41 <2; it13_g41++ ){
			voroi13_g41 += fade13_g41 * voronoi13_g41( coords13_g41, time13_g41, id13_g41, uv13_g41, 0,voronoiSmoothId13_g41 );
			rest13_g41 += fade13_g41;
			coords13_g41 *= 2;
			fade13_g41 *= 0.5;
			}//Voronoi13_g41
			voroi13_g41 /= rest13_g41;
			float time11_g41 = 17.23;
			float2 voronoiSmoothId11_g41 = 0;
			float2 coords11_g41 = temp_output_10_0_g41 * 9.0;
			float2 id11_g41 = 0;
			float2 uv11_g41 = 0;
			float fade11_g41 = 0.5;
			float voroi11_g41 = 0;
			float rest11_g41 = 0;
			for( int it11_g41 = 0; it11_g41 <2; it11_g41++ ){
			voroi11_g41 += fade11_g41 * voronoi11_g41( coords11_g41, time11_g41, id11_g41, uv11_g41, 0,voronoiSmoothId11_g41 );
			rest11_g41 += fade11_g41;
			coords11_g41 *= 2;
			fade11_g41 *= 0.5;
			}//Voronoi11_g41
			voroi11_g41 /= rest11_g41;
			float2 temp_output_1056_0 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult1057 = dot( temp_output_1056_0 , temp_output_1056_0 );
			float ModifiedCohesion1074 = ( _CloudCohesion * 1.0 * ( 1.0 - dotResult1057 ) );
			float lerpResult15_g41 = lerp( saturate( ( voroi35_g41 + voroi13_g41 ) ) , voroi11_g41 , ModifiedCohesion1074);
			float CumulusCoverage376 = ( _CumulusCoverageMultiplier * _MaxCloudCover );
			float lerpResult16_g41 = lerp( lerpResult15_g41 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
			float temp_output_1203_0 = saturate( SampleGradient( gradient1198, saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g41 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) ) ).r );
			float4 lerpResult1234 = lerp( CloudHighlightColor334 , CloudColor332 , ( (1.0 + (RawDot1238 - 0.0) * (0.0 - 1.0) / (0.2 - 0.0)) * temp_output_1203_0 ));
			float4 IT2Color1207 = lerpResult1234;
			float IT2Alpha1202 = temp_output_1203_0;
			clip( IT2Alpha1202 - _ClippingThreshold);
			o.Emission = IT2Color1207.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18935
0;1080;2194.286;607.5715;4116.917;2083.118;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;372;-4450.156,-2723.177;Inherit;False;2555.466;1283.535;;49;997;1095;985;986;1094;1071;1052;1078;998;1051;1083;1076;1161;1080;1035;1074;947;1045;956;931;1043;925;1036;955;1058;1042;1040;906;1059;1041;1057;1056;1055;1222;376;375;334;52;332;36;152;61;150;70;1225;1226;1229;1233;1238;Variable Declaration;0.6196079,0.9508546,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-3519.35,-2327.424;Inherit;False;Property;_WindSpeed;Wind Speed;2;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-3367.351,-2349.156;Inherit;False;2;2;0;FLOAT;0.001;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1055;-3408,-1696;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;61;-3234.401,-2345.619;Inherit;False;1;0;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-3065.96,-2349.724;Inherit;False;Time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;1056;-3184,-1696;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;925;-4408,-1814;Inherit;False;Property;_MainCloudScale;Main Cloud Scale;5;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.8;30;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;1057;-3056,-1696;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1040;-2688,-2400;Inherit;False;152;Time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1042;-4304,-2064;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;1041;-2720,-2288;Inherit;False;Constant;_MainCloudWindDir;Main Cloud Wind Dir;11;0;Create;True;0;0;0;False;0;False;0.1,0.2;0.3,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;1059;-2928,-1696;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;955;-3200,-2080;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;947;-3936,-1808;Inherit;False;Property;_Spherize;Spherize;6;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.36;0.206;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1225;-2705.094,-2501.984;Inherit;False;Property;_MaxCloudCover;Max Cloud Cover;10;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;375;-2782.653,-2580.27;Inherit;False;Property;_CumulusCoverageMultiplier;Cumulus Coverage Multiplier;3;2;[HideInInspector];[Header];Create;True;1;Cumulus Clouds;0;0;False;0;False;1;0.818;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1168;-1856,-2720;Inherit;False;2985.083;856.5;;1;1170;Iteration 2;1,0.8737146,0.572549,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;1043;-4079,-2064;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1036;-2464,-2368;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1226;-4243.582,-1806.441;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;906;-3104,-1824;Inherit;False;Property;_CloudCohesion;Cloud Cohesion;4;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.887;0.778;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1170;-1808,-2640;Inherit;False;2885.064;747.3666;Color;27;1202;1207;1234;1203;1235;1204;1200;1197;1198;1196;1195;1193;1194;1192;1186;1191;1215;1213;1185;1187;1188;1189;1190;1236;1237;1240;1241;;1,0.8737146,0.572549,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1058;-2672,-1744;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;956;-2976,-2080;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;-0.5,-0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;1045;-3952,-2064;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1080;-4130,-1813;Inherit;False;Scale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1229;-3572.989,-1910.844;Inherit;False;2;2;0;FLOAT;20;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1222;-2496,-2576;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1035;-2288,-2384;Inherit;False;Wind;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;1051;-3824,-2064;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.85;False;4;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1074;-2464,-1744;Inherit;False;ModifiedCohesion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1189;-1744,-2528;Inherit;False;1035;Wind;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1078;-3664,-1808;Inherit;False;Spherize;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1190;-1744,-2208;Inherit;False;1080;Scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;998;-2832,-2080;Inherit;False;CentralUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1076;-3312,-1824;Inherit;False;Flatness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;376;-2307,-2580;Inherit;False;CumulusCoverage;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1213;-1499,-2224;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1215;-1520,-2496;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1191;-1744,-2448;Inherit;False;998;CentralUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1186;-1744,-2368;Inherit;False;1078;Spherize;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1052;-3648,-2064;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1185;-1744,-2048;Inherit;False;376;CumulusCoverage;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1187;-1744,-2128;Inherit;False;1074;ModifiedCohesion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1188;-1744,-2288;Inherit;False;1076;Flatness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1192;-1280,-2352;Inherit;True;Ghibli Clouds;-1;;41;bce7362c867d47d49a15818b7e6650d4;0;7;37;FLOAT2;0,0;False;21;FLOAT2;0,0;False;19;FLOAT;1;False;20;FLOAT;1;False;23;FLOAT;1;False;24;FLOAT;0;False;27;FLOAT;0.5;False;2;FLOAT;33;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1071;-3470,-2064;Inherit;False;Dot;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1194;-928,-2432;Inherit;False;1071;Dot;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1193;-928,-2352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1195;-752,-2368;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;1196;-608,-2368;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.6;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;1198;-496,-2448;Inherit;False;0;2;2;0.06119964,0.06119964,0.06119964,0.3029374;1,1,1,0.520592;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SaturateNode;1197;-432,-2368;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;1200;-288,-2416;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1238;-3809.977,-2186.148;Inherit;False;RawDot;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;-4018.127,-2611.486;Inherit;False;Property;_CloudColor;Cloud Color;0;3;[HideInInspector];[HDR];[Header];Create;True;1;General Cloud Settings;0;0;False;0;False;0.7264151,0.7264151,0.7264151,0;0.5061553,0.4965099,0.5009958,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1237;64,-2384;Inherit;False;1238;RawDot;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;52;-4015.761,-2428.211;Inherit;False;Property;_CloudHighlightColor;Cloud Highlight Color;1;2;[HideInInspector];[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;6.442703,3.563737,2.772021,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;1240;32,-2304;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;334;-3796.106,-2428.746;Inherit;False;CloudHighlightColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;332;-3793.636,-2612.531;Inherit;False;CloudColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;1203;272,-2224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;1241;223,-2384;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.2;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1236;432,-2384;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1235;272,-2544;Inherit;False;334;CloudHighlightColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1204;272,-2464;Inherit;False;332;CloudColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;1234;576,-2544;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1202;432,-2224;Inherit;False;IT2Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1207;736,-2544;Inherit;False;IT2Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1219;-924.733,-432.013;Inherit;False;1207;IT2Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1217;-927,-319;Inherit;False;1202;IT2Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1224;-1024,-224;Inherit;False;Property;_ClippingThreshold;Clipping Threshold;9;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0.861;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1094;-2800,-1904;Inherit;False;1071;Dot;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1161;-3872,-1648;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1095;-2304,-2080;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;986;-2911,-1984;Inherit;False;Property;_ShadowingDistance;Shadowing Distance;8;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.07;0.0288;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1083;-3728,-1664;Inherit;False;Coverage;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClipNode;1223;-706.1743,-379.016;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;985;-2560,-2016;Inherit;True;4;4;0;FLOAT2;0,0;False;1;FLOAT2;-1,-1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;997;-2192,-2080;Inherit;False;ShadowUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;931;-4215,-1605;Inherit;False;Property;_Coverage;Coverage;7;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.3574152;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1233;-2832,-1824;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-406.0242,-441.7039;Float;False;True;-1;2;;0;0;Unlit;Distant Lands/Cozy/Stylized Clouds Ghibli Mobile;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Front;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;True;-50;False;Opaque;;Transparent;ForwardOnly;18;all;True;True;True;True;0;False;-1;True;221;False;-1;255;False;-1;255;False;-1;7;False;-1;3;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;150;1;70;0
WireConnection;61;0;150;0
WireConnection;152;0;61;0
WireConnection;1056;0;1055;0
WireConnection;1057;0;1056;0
WireConnection;1057;1;1056;0
WireConnection;1059;0;1057;0
WireConnection;1043;0;1042;0
WireConnection;1036;0;1040;0
WireConnection;1036;1;1041;0
WireConnection;1226;0;925;0
WireConnection;1058;0;906;0
WireConnection;1058;2;1059;0
WireConnection;956;0;955;0
WireConnection;1045;0;1043;0
WireConnection;1045;1;1043;0
WireConnection;1080;0;1226;0
WireConnection;1229;1;947;0
WireConnection;1222;0;375;0
WireConnection;1222;1;1225;0
WireConnection;1035;0;1036;0
WireConnection;1051;0;1045;0
WireConnection;1074;0;1058;0
WireConnection;1078;0;947;0
WireConnection;998;0;956;0
WireConnection;1076;0;1229;0
WireConnection;376;0;1222;0
WireConnection;1213;0;1190;0
WireConnection;1215;0;1189;0
WireConnection;1052;0;1051;0
WireConnection;1192;37;1215;0
WireConnection;1192;21;1191;0
WireConnection;1192;19;1186;0
WireConnection;1192;20;1188;0
WireConnection;1192;23;1213;0
WireConnection;1192;24;1187;0
WireConnection;1192;27;1185;0
WireConnection;1071;0;1052;0
WireConnection;1193;0;1192;33
WireConnection;1195;0;1194;0
WireConnection;1195;1;1193;0
WireConnection;1196;0;1195;0
WireConnection;1197;0;1196;0
WireConnection;1200;0;1198;0
WireConnection;1200;1;1197;0
WireConnection;1238;0;1045;0
WireConnection;1240;0;1200;1
WireConnection;334;0;52;0
WireConnection;332;0;36;0
WireConnection;1203;0;1240;0
WireConnection;1241;0;1237;0
WireConnection;1236;0;1241;0
WireConnection;1236;1;1203;0
WireConnection;1234;0;1235;0
WireConnection;1234;1;1204;0
WireConnection;1234;2;1236;0
WireConnection;1202;0;1203;0
WireConnection;1207;0;1234;0
WireConnection;1161;0;931;0
WireConnection;1095;0;998;0
WireConnection;1095;1;985;0
WireConnection;1083;0;1161;0
WireConnection;1223;0;1219;0
WireConnection;1223;1;1217;0
WireConnection;1223;2;1224;0
WireConnection;985;0;998;0
WireConnection;985;2;986;0
WireConnection;985;3;1094;0
WireConnection;997;0;1095;0
WireConnection;1233;0;906;0
WireConnection;0;2;1223;0
ASEEND*/
//CHKSM=5268BA8245333E64D6786E112A7295EE4C5DD3EC