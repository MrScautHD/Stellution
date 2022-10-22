// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/Stylized Clouds Mobile"
{
	Properties
	{
		[HideInInspector]_DetailScale("Detail Scale", Float) = 0.5
		[HideInInspector][HDR]_CloudColor("Cloud Color", Color) = (0.7264151,0.7264151,0.7264151,0)
		[HideInInspector][HDR]_CloudHighlightColor("Cloud Highlight Color", Color) = (1,1,1,0)
		[HideInInspector]_MainCloudScale("Main Cloud Scale", Float) = 10
		[HideInInspector]_DetailAmount("Detail Amount", Float) = 1
		[HideInInspector]_SunFlareFalloff("Sun Flare Falloff", Float) = 1
		[HideInInspector]_MaxCloudCover("Max Cloud Cover", Float) = 0
		[HideInInspector]_CumulusCoverageMultiplier("Cumulus Coverage Multiplier", Range( 0 , 2)) = 0
		[HideInInspector]_MinCloudCover("Min Cloud Cover", Float) = 0
		[HideInInspector]_CloudFlareFalloff("Cloud Flare Falloff", Float) = 1
		[HideInInspector]_WindSpeed("Wind Speed", Float) = 0
		[HideInInspector]_CloudWind2("Cloud Wind 2", Vector) = (0.3,0.2,0,0)
		[HideInInspector]_SunDirection("Sun Direction", Vector) = (0,0,0,0)
		[HideInInspector]_DetailWind("Detail Wind", Vector) = (0.3,0.2,0,0)
		[HideInInspector]_CloudWind1("Cloud Wind 1", Vector) = (0.2,-0.4,0,0)
		[HideInInspector]_ClippingThreshold("Clipping Threshold", Range( 0 , 1)) = 0.5
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
			float3 worldPos;
		};

		uniform float4 _CloudColor;
		uniform float4 _CloudHighlightColor;
		uniform float _WindSpeed;
		uniform float2 _CloudWind1;
		uniform float _MainCloudScale;
		uniform float _MinCloudCover;
		uniform float _MaxCloudCover;
		uniform float _CumulusCoverageMultiplier;
		uniform float2 _CloudWind2;
		uniform float3 _SunDirection;
		uniform half _SunFlareFalloff;
		uniform float _DetailScale;
		uniform float2 _DetailWind;
		uniform float _DetailAmount;
		uniform half _CloudFlareFalloff;
		uniform float _ClippingThreshold;


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


		float2 voronoihash148( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi148( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash148( n + g );
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


		float2 voronoihash77( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi77( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash77( n + g );
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
			float2 Pos159 = i.uv_texcoord;
			float mulTime61 = _Time.y * ( 0.001 * _WindSpeed );
			float TIme152 = mulTime61;
			float simplePerlin2D37 = snoise( ( Pos159 + ( TIme152 * _CloudWind1 ) )*( 100.0 / _MainCloudScale ) );
			simplePerlin2D37 = simplePerlin2D37*0.5 + 0.5;
			float2 temp_output_114_0 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult140 = dot( temp_output_114_0 , temp_output_114_0 );
			float CurrentCloudCover240 = ( ( ( _MinCloudCover + _MaxCloudCover ) / 2.0 ) * _CumulusCoverageMultiplier );
			float temp_output_128_0 = (0.0 + (dotResult140 - 0.0) * (CurrentCloudCover240 - 0.0) / (0.27 - 0.0));
			float time148 = 0.0;
			float2 voronoiSmoothId0 = 0;
			float2 coords148 = ( Pos159 + ( TIme152 * _CloudWind2 ) ) * ( 140.0 / _MainCloudScale );
			float2 id148 = 0;
			float2 uv148 = 0;
			float voroi148 = voronoi148( coords148, time148, id148, uv148, 0, voronoiSmoothId0 );
			float temp_output_50_0 = (0.0 + (min( ( simplePerlin2D37 + temp_output_128_0 ) , ( ( 1.0 - voroi148 ) + temp_output_128_0 ) ) - ( 1.0 - CurrentCloudCover240 )) * (1.0 - 0.0) / (1.0 - ( 1.0 - CurrentCloudCover240 )));
			float4 lerpResult53 = lerp( _CloudHighlightColor , _CloudColor , saturate( (2.0 + (temp_output_50_0 - 0.0) * (0.7 - 2.0) / (1.0 - 0.0)) ));
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult259 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float dotResult261 = dot( normalizeResult259 , _SunDirection );
			float temp_output_264_0 = abs( (dotResult261*0.5 + 0.5) );
			half LightMask267 = saturate( pow( temp_output_264_0 , _SunFlareFalloff ) );
			float temp_output_224_0 = ( voroi148 * saturate( ( CurrentCloudCover240 - 0.8 ) ) );
			float4 lerpResult227 = lerp( ( lerpResult53 + ( LightMask267 * _CloudHighlightColor * ( 1.0 - temp_output_224_0 ) ) ) , ( _CloudColor * float4( 0.5660378,0.5660378,0.5660378,0 ) ) , temp_output_224_0);
			float time77 = 0.0;
			float2 coords77 = ( Pos159 + ( TIme152 * _DetailWind ) ) * ( 100.0 / _DetailScale );
			float2 id77 = 0;
			float2 uv77 = 0;
			float fade77 = 0.5;
			float voroi77 = 0;
			float rest77 = 0;
			for( int it77 = 0; it77 <3; it77++ ){
			voroi77 += fade77 * voronoi77( coords77, time77, id77, uv77, 0,voronoiSmoothId0 );
			rest77 += fade77;
			coords77 *= 2;
			fade77 *= 0.5;
			}//Voronoi77
			voroi77 /= rest77;
			float temp_output_47_0 = ( (0.0 + (( 1.0 - voroi77 ) - 0.3) * (0.5 - 0.0) / (1.0 - 0.3)) * 0.1 * _DetailAmount );
			float temp_output_49_0 = saturate( ( temp_output_50_0 + temp_output_47_0 ) );
			float4 lerpResult171 = lerp( _CloudColor , lerpResult227 , (1.0 + (temp_output_49_0 - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)));
			float CloudDetail294 = temp_output_47_0;
			float CloudLight271 = saturate( pow( temp_output_264_0 , _CloudFlareFalloff ) );
			float4 lerpResult272 = lerp( float4( 0,0,0,0 ) , _CloudHighlightColor , ( saturate( ( CurrentCloudCover240 - 1.0 ) ) * CloudDetail294 * CloudLight271 ));
			float4 SunThroughCLouds273 = ( lerpResult272 * 1.3 );
			clip( temp_output_49_0 - _ClippingThreshold);
			o.Emission = ( lerpResult171 + SunThroughCLouds273 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18912
0;1080;2194;606;4068.187;744.3714;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;70;-2259.62,-1917.207;Inherit;False;Property;_WindSpeed;Wind Speed;10;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-2107.62,-1938.939;Inherit;False;2;2;0;FLOAT;0.001;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;61;-1974.671,-1935.404;Inherit;False;1;0;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;313;-3532.187,-613.3714;Inherit;False;Property;_MinCloudCover;Min Cloud Cover;8;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-3536.704,-520.9351;Inherit;False;Property;_MaxCloudCover;Max Cloud Cover;6;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;257;-3053.492,-1853.766;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;94;-2014.295,-2107.989;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-1806.23,-1939.509;Inherit;False;TIme;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;256;-3122.902,-1704.677;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;314;-3303.187,-576.3714;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;-2250.513,655.9659;Inherit;False;152;TIme;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;67;-3459.207,91.45112;Inherit;False;Property;_CloudWind2;Cloud Wind 2;11;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.3,0.2;0.1,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;159;-1810.859,-2113.234;Inherit;False;Pos;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;258;-2873.747,-1791.555;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;315;-3156.187,-560.3714;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;317;-3250.187,-455.3714;Inherit;False;Property;_CumulusCoverageMultiplier;Cumulus Coverage Multiplier;7;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;156;-3456.734,22.63042;Inherit;False;152;TIme;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;91;-2246.208,757.8749;Inherit;False;Property;_DetailWind;Detail Wind;13;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.3,0.2;0.3,0.8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.NormalizeNode;259;-2746.631,-1792.802;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;260;-2800.151,-1655.838;Inherit;False;Property;_SunDirection;Sun Direction;12;1;[HideInInspector];Create;True;0;0;0;False;0;False;0,0,0;0.2509345,0.952687,0.1715211;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-2033.319,688.7798;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;161;-3247.494,-211.0186;Inherit;False;159;Pos;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;157;-3455.332,-178.3424;Inherit;False;152;TIme;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-3211.19,89.5499;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;62;-3452.838,-102.2996;Inherit;False;Property;_CloudWind1;Cloud Wind 1;14;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.2,-0.4;0.6,-0.8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;43;-2055.122,793.5134;Inherit;False;Property;_DetailScale;Detail Scale;0;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.5;2.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;316;-2948.187,-519.3714;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-3263.089,-121.7626;Inherit;False;Property;_MainCloudScale;Main Cloud Scale;3;1;[HideInInspector];Create;True;0;0;0;False;0;False;10;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;-2071.709,576.1065;Inherit;False;159;Pos;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;113;-3555.36,334.7895;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;93;-1870.853,630.8893;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-3210.732,-32.52113;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;114;-3335.876,328.2248;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;261;-2601.887,-1782.875;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;240;-2689.815,-519.9056;Inherit;False;CurrentCloudCover;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;44;-1872.95,727.6557;Inherit;False;2;0;FLOAT;100;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;65;-3004.745,125.4867;Inherit;False;2;0;FLOAT;140;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-3011.608,6.357646;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;42;-3015.533,-97.85484;Inherit;False;2;0;FLOAT;100;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;140;-3137.515,338.8121;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;148;-2818.061,58.81207;Inherit;False;0;0;1;3;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.GetLocalVarNode;242;-2925.268,426.6064;Inherit;False;240;CurrentCloudCover;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-3014.333,-205.2863;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VoronoiNode;77;-1737.568,629.6122;Inherit;False;0;0;1;0;3;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.ScaleAndOffsetNode;262;-2462.253,-1786.08;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;37;-2753.309,-192.9547;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;149;-2618.524,39.49897;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;78;-1553.092,629.4844;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;264;-2238.919,-1783.942;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;128;-2700.444,338.7925;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.27;False;3;FLOAT;0;False;4;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;268;-2262.444,-1518.317;Half;False;Property;_CloudFlareFalloff;Cloud Flare Falloff;9;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;10.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;312;-2416,304;Inherit;False;240;CurrentCloudCover;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;142;-2395.751,44.55014;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;51;-1396.618,629.7917;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.3;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;241;-2414.801,-297.6468;Inherit;False;240;CurrentCloudCover;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;263;-2304.778,-1673.833;Half;False;Property;_SunFlareFalloff;Sun Flare Falloff;5;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;14.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;143;-2404.599,-155.9926;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;269;-2053.645,-1554.017;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-1392.466,804.3477;Inherit;False;Property;_DetailAmount;Detail Amount;4;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;30;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;222;-2192,304;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;69;-2224.462,-87.50346;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;278;-1956.49,-1101.501;Inherit;False;240;CurrentCloudCover;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;58;-2166.674,-205.2955;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;265;-2104.516,-1786.694;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;270;-1898.443,-1554.717;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1209.316,643.1975;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;294;-1047.167,540.6031;Inherit;False;CloudDetail;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;279;-1731.49,-1097.501;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;223;-2048,304;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;271;-1755.443,-1558.617;Inherit;False;CloudLight;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;266;-1958.44,-1788.474;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;50;-1954.567,-37.21202;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.3;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;267;-1810.267,-1795.581;Half;False;LightMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;54;-1487.6,-282.1764;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;2;False;4;FLOAT;0.7;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;282;-1613.075,-931.1725;Inherit;False;271;CloudLight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;295;-1617.724,-1009.785;Inherit;False;294;CloudDetail;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;52;-1766.23,-402.0294;Inherit;False;Property;_CloudHighlightColor;Cloud Highlight Color;2;2;[HideInInspector];[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;4.077162,4.077162,4.077162,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;277;-1582.437,-1098.79;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;250;-1693.275,454.8231;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;224;-1888,208;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;311;-1329.776,-721.7325;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;255;-1488.107,-92.48222;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;236;-1712,160;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;55;-1280.4,-285.9649;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;274;-1392.654,-1067.125;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;249;-1140.89,530.0228;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;-1748.068,-578.0109;Inherit;False;Property;_CloudColor;Cloud Color;1;2;[HideInInspector];[HDR];Create;True;0;0;0;False;0;False;0.7264151,0.7264151,0.7264151,0;0.5734241,0.6288389,0.6501523,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;214;-1311.511,-166.4265;Inherit;False;267;LightMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;226;-1507.35,-35.52195;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;53;-1138.337,-372.5875;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;205;-1126.307,-141.6064;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-1050.013,622.0901;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;284;-1152.608,-981.5635;Inherit;False;Constant;_2;2;15;0;Create;True;0;0;0;False;0;False;1.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;272;-1158.918,-1098.087;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;283;-948.608,-1094.564;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;216;-946.1294,-219.194;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;49;-785.2887,620.592;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;254;-998.9235,158.9165;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;225;-1093.746,8.678019;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.5660378,0.5660378,0.5660378,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;227;-832.2244,-61.70548;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;273;-787.0106,-1101.603;Inherit;False;SunThroughCLouds;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;201;-782.5886,433.747;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;253;-929.7689,-448.1464;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;171;-578.157,-92.04621;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;281;-634.3154,38.72198;Inherit;False;273;SunThroughCLouds;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;280;-344.2014,-31.75806;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;151;-390.9456,160.9664;Inherit;False;Property;_ClippingThreshold;Clipping Threshold;15;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClipNode;56;-121.9733,-0.2058594;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;139.7081,-38.95319;Float;False;True;-1;2;;0;0;Unlit;Distant Lands/Cozy/Stylized Clouds Mobile;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Front;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;True;-50;False;Opaque;;Transparent;ForwardOnly;18;all;True;True;True;True;0;False;-1;True;221;False;-1;255;False;-1;255;False;-1;7;False;-1;3;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;150;1;70;0
WireConnection;61;0;150;0
WireConnection;152;0;61;0
WireConnection;314;0;313;0
WireConnection;314;1;57;0
WireConnection;159;0;94;0
WireConnection;258;0;257;0
WireConnection;258;1;256;0
WireConnection;315;0;314;0
WireConnection;259;0;258;0
WireConnection;92;0;155;0
WireConnection;92;1;91;0
WireConnection;68;0;156;0
WireConnection;68;1;67;0
WireConnection;316;0;315;0
WireConnection;316;1;317;0
WireConnection;93;0;162;0
WireConnection;93;1;92;0
WireConnection;60;0;157;0
WireConnection;60;1;62;0
WireConnection;114;0;113;0
WireConnection;261;0;259;0
WireConnection;261;1;260;0
WireConnection;240;0;316;0
WireConnection;44;1;43;0
WireConnection;65;1;41;0
WireConnection;66;0;161;0
WireConnection;66;1;68;0
WireConnection;42;1;41;0
WireConnection;140;0;114;0
WireConnection;140;1;114;0
WireConnection;148;0;66;0
WireConnection;148;2;65;0
WireConnection;63;0;161;0
WireConnection;63;1;60;0
WireConnection;77;0;93;0
WireConnection;77;2;44;0
WireConnection;262;0;261;0
WireConnection;37;0;63;0
WireConnection;37;1;42;0
WireConnection;149;0;148;0
WireConnection;78;0;77;0
WireConnection;264;0;262;0
WireConnection;128;0;140;0
WireConnection;128;4;242;0
WireConnection;142;0;149;0
WireConnection;142;1;128;0
WireConnection;51;0;78;0
WireConnection;143;0;37;0
WireConnection;143;1;128;0
WireConnection;269;0;264;0
WireConnection;269;1;268;0
WireConnection;222;0;312;0
WireConnection;69;0;143;0
WireConnection;69;1;142;0
WireConnection;58;0;241;0
WireConnection;265;0;264;0
WireConnection;265;1;263;0
WireConnection;270;0;269;0
WireConnection;47;0;51;0
WireConnection;47;2;46;0
WireConnection;294;0;47;0
WireConnection;279;0;278;0
WireConnection;223;0;222;0
WireConnection;271;0;270;0
WireConnection;266;0;265;0
WireConnection;50;0;69;0
WireConnection;50;1;58;0
WireConnection;267;0;266;0
WireConnection;54;0;50;0
WireConnection;277;0;279;0
WireConnection;250;0;50;0
WireConnection;224;0;148;0
WireConnection;224;1;223;0
WireConnection;311;0;52;0
WireConnection;255;0;52;0
WireConnection;236;0;224;0
WireConnection;55;0;54;0
WireConnection;274;0;277;0
WireConnection;274;1;295;0
WireConnection;274;2;282;0
WireConnection;249;0;250;0
WireConnection;226;0;36;0
WireConnection;53;0;52;0
WireConnection;53;1;36;0
WireConnection;53;2;55;0
WireConnection;205;0;214;0
WireConnection;205;1;255;0
WireConnection;205;2;236;0
WireConnection;48;0;249;0
WireConnection;48;1;47;0
WireConnection;272;1;311;0
WireConnection;272;2;274;0
WireConnection;283;0;272;0
WireConnection;283;1;284;0
WireConnection;216;0;53;0
WireConnection;216;1;205;0
WireConnection;49;0;48;0
WireConnection;254;0;224;0
WireConnection;225;0;226;0
WireConnection;227;0;216;0
WireConnection;227;1;225;0
WireConnection;227;2;254;0
WireConnection;273;0;283;0
WireConnection;201;0;49;0
WireConnection;253;0;36;0
WireConnection;171;0;253;0
WireConnection;171;1;227;0
WireConnection;171;2;201;0
WireConnection;280;0;171;0
WireConnection;280;1;281;0
WireConnection;56;0;280;0
WireConnection;56;1;49;0
WireConnection;56;2;151;0
WireConnection;0;2;56;0
ASEEND*/
//CHKSM=13D1FE46CB1F88A88E379AC091B5312BED948FBA