// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/Stylized Sky Mobile"
{
	Properties
	{
		[HideInInspector][HDR]_HorizonColor("Horizon Color", Color) = (0.6399965,0.9474089,0.9622642,0)
		[HideInInspector][HDR]_ZenithColor("Zenith Color", Color) = (0.4000979,0.6638572,0.764151,0)
		[HideInInspector]_Offset("Offset", Float) = 1
		[HideInInspector]_Power("Power", Float) = 1
		_SunFlareFalloff("Sun Flare Falloff", Float) = 1
		[HideInInspector][HDR]_SunFlareColor("Sun Flare Color", Color) = (0.355693,0.4595688,0.4802988,1)
		[HideInInspector]_SunSize("Sun Size", Float) = 0
		[HideInInspector][HDR]_SunColor("Sun Color", Color) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Pass
		{
			ColorMask 0
			ZWrite On
		}

		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent-100" "IsEmissive" = "true"  }
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
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _HorizonColor;
		uniform float4 _ZenithColor;
		uniform float _Offset;
		uniform float _Power;
		uniform float3 CZY_SunDirection;
		uniform float _SunFlareFalloff;
		uniform float4 _SunFlareColor;
		uniform float4 _SunColor;
		uniform float _SunSize;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float4 transform18 = mul(unity_WorldToObject,float4( ase_worldPos , 0.0 ));
			float4 lerpResult3 = lerp( _HorizonColor , _ZenithColor , saturate( pow( ( ( abs( transform18.y ) * 0.01 ) + _Offset ) , _Power ) ));
			float3 normalizeResult26 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float dotResult27 = dot( normalizeResult26 , CZY_SunDirection );
			half LightMask35 = saturate( pow( abs( (dotResult27*0.5 + 0.5) ) , _SunFlareFalloff ) );
			o.Emission = ( lerpResult3 + abs( ( LightMask35 * _SunFlareColor ) ) + ( _SunColor * ( ( 1.0 - dotResult27 ) > ( _SunSize * 0.001 ) ? 0.0 : 1.0 ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18912
0;1080;2194;606;2340.879;-405.4253;1;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;22;-2047.766,608.3867;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;23;-2117.176,757.4752;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;24;-1868.021,670.598;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;49;-1787.906,795.2533;Inherit;False;Global;CZY_SunDirection;CZY_SunDirection;8;1;[HideInInspector];Create;True;0;0;0;False;0;False;0,0,0;0.4986527,0.8516705,0.1612533;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;26;-1740.905,669.3503;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;17;-1947.624,122.992;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;27;-1569.161,675.2775;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;18;-1778.794,123.3444;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;29;-1429.527,679.0722;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;21;-1602.034,168.4667;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-1293.752,813.7197;Inherit;False;Property;_SunFlareFalloff;Sun Flare Falloff;4;0;Create;True;0;0;0;False;0;False;1;32.41;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;32;-1234.193,678.2104;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1495.793,268.7507;Inherit;False;Property;_Offset;Offset;2;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-1484.503,169.329;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;33;-1098.79,675.4581;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-1348.964,170.9033;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;34;-943.7123,672.6779;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1350.573,268.1994;Inherit;False;Property;_Power;Power;3;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;0.364;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;12;-1211.454,170.4201;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;35;-807.5402,665.5712;Half;False;LightMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-1521.529,459.0146;Inherit;False;Property;_SunSize;Sun Size;6;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0.56;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-1151.493,-38.73587;Inherit;False;Property;_ZenithColor;Zenith Color;1;2;[HideInInspector];[HDR];Create;True;0;0;0;False;0;False;0.4000979,0.6638572,0.764151,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;43;-1368.453,578.2216;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-1156.344,-217.2174;Inherit;False;Property;_HorizonColor;Horizon Color;0;2;[HideInInspector];[HDR];Create;True;0;0;0;False;0;False;0.6399965,0.9474089,0.9622642,0;0.02943663,0.04203962,0.04245283,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-1317.448,466.5813;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.001;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;13;-1064.823,167.3865;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;-817.0525,139.1839;Inherit;False;35;LightMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;37;-853.2534,227.1194;Inherit;False;Property;_SunFlareColor;Sun Flare Color;5;2;[HideInInspector];[HDR];Create;True;0;0;0;False;0;False;0.355693,0.4595688,0.4802988,1;0.257921,0.3264449,0.3396226,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;41;-1128.13,480.2983;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;3;-869.4722,-100.6007;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-633.0845,179.3311;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;45;-1125.745,305.0071;Inherit;False;Property;_SunColor;Sun Color;7;2;[HideInInspector];[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;10.43295,10.43295,10.43295,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-793.8229,457.3288;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;40;-450.8567,26.58327;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.AbsOpNode;47;-497.3607,179.2732;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;36;-277.1601,148.8452;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;;0;0;Unlit;Distant Lands/Cozy/Stylized Sky Mobile;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Front;0;False;-1;7;False;-1;False;0;False;-1;0;False;-1;True;0;Translucent;0.5;True;True;-100;False;Opaque;;Transparent;All;18;all;True;True;True;True;0;False;-1;True;221;False;-1;255;False;-1;255;False;-1;7;False;-1;3;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;24;0;22;0
WireConnection;24;1;23;0
WireConnection;26;0;24;0
WireConnection;27;0;26;0
WireConnection;27;1;49;0
WireConnection;18;0;17;0
WireConnection;29;0;27;0
WireConnection;21;0;18;2
WireConnection;32;0;29;0
WireConnection;20;0;21;0
WireConnection;33;0;32;0
WireConnection;33;1;31;0
WireConnection;11;0;20;0
WireConnection;11;1;15;0
WireConnection;34;0;33;0
WireConnection;12;0;11;0
WireConnection;12;1;14;0
WireConnection;35;0;34;0
WireConnection;43;0;27;0
WireConnection;46;0;42;0
WireConnection;13;0;12;0
WireConnection;41;0;43;0
WireConnection;41;1;46;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;3;2;13;0
WireConnection;38;0;39;0
WireConnection;38;1;37;0
WireConnection;44;0;45;0
WireConnection;44;1;41;0
WireConnection;40;0;3;0
WireConnection;47;0;38;0
WireConnection;36;0;40;0
WireConnection;36;1;47;0
WireConnection;36;2;44;0
WireConnection;0;2;36;0
ASEEND*/
//CHKSM=D3EDB2EE68E64840377AC1EC53B5275C7D0F686F