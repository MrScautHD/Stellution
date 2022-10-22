// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Stylized Clouds (Static Texture)"
{
	Properties
	{
		_CloudTexture("Cloud Texture", 2D) = "white" {}
		[HideInInspector][HDR][Header(General Cloud Settings)]_CloudColor("Cloud Color", Color) = (0.7264151,0.7264151,0.7264151,0)
		[HideInInspector]_WindSpeed("Wind Speed", Float) = 0
		[HideInInspector]_ClippingThreshold("Clipping Threshold", Range( 0 , 1)) = 0.5
		_TexturePanDirection("Texture Pan Direction", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Front
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _CloudTexture;
		uniform float3 _TexturePanDirection;
		uniform float _WindSpeed;
		uniform float4 _CloudColor;
		uniform float _ClippingThreshold;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 break30 = _TexturePanDirection;
			float mulTime17 = _Time.y * ( _WindSpeed * 0.005 );
			float TIme18 = mulTime17;
			float2 appendResult32 = (float2(( break30.x * 0.0025 * TIme18 ) , ( TIme18 * 0.0025 * break30.z )));
			float2 uv_TexCoord14 = i.uv_texcoord + appendResult32;
			float cos13 = cos( ( TIme18 * break30.y ) );
			float sin13 = sin( ( TIme18 * break30.y ) );
			float2 rotator13 = mul( uv_TexCoord14 - float2( 0.5,0.5 ) , float2x2( cos13 , -sin13 , sin13 , cos13 )) + float2( 0.5,0.5 );
			float4 tex2DNode2 = tex2D( _CloudTexture, rotator13 );
			float4 CloudTex8 = tex2DNode2;
			float4 CloudColor6 = _CloudColor;
			float CloudAlpha12 = ( tex2DNode2.r * tex2DNode2.g * tex2DNode2.b * tex2DNode2.a );
			clip( CloudAlpha12 - _ClippingThreshold);
			o.Emission = ( CloudTex8 * CloudColor6 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
0;1080;2194.286;607.5715;2107.08;293.6526;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;19;-2048,-160;Inherit;False;Property;_WindSpeed;Wind Speed;2;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0.74;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1872,-160;Inherit;False;2;2;0;FLOAT;0.001;False;1;FLOAT;0.005;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;17;-1744,-160;Inherit;False;1;0;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;26;-2736,-48;Inherit;False;Property;_TexturePanDirection;Texture Pan Direction;4;0;Create;True;0;0;0;False;0;False;0,0,0;0,1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-1584,-160;Inherit;False;TIme;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;30;-2528,-48;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;20;-2589,-143;Inherit;False;18;TIme;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-2352,-160;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0.0025;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-2352,64;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0.0025;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;-2208,-48;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-2048,-48;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-2352,-32;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;13;-1808,-48;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;10;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-1616,-48;Inherit;True;Property;_CloudTexture;Cloud Texture;0;0;Create;True;0;0;0;False;0;False;-1;404f80e8b03e57a48baef75373097c56;404f80e8b03e57a48baef75373097c56;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-2048,-352;Inherit;False;Property;_CloudColor;Cloud Color;1;3;[HideInInspector];[HDR];[Header];Create;True;1;General Cloud Settings;0;0;False;0;False;0.7264151,0.7264151,0.7264151,0;0.02156863,0.0372549,0.04313726,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;6;-1824,-352;Inherit;False;CloudColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-1328,32;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-1328,-48;Inherit;False;CloudTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1136,32;Inherit;True;CloudAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;7;-864,-16;Inherit;False;6;CloudColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;9;-864,-96;Inherit;False;8;CloudTex;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;11;-656,0;Inherit;False;12;CloudAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-736,80;Inherit;False;Property;_ClippingThreshold;Clipping Threshold;3;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-656,-96;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClipNode;3;-448,-32;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Stylized Clouds (Static Texture);False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Front;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;False;0;False;Opaque;;Transparent;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;16;0;19;0
WireConnection;17;0;16;0
WireConnection;18;0;17;0
WireConnection;30;0;26;0
WireConnection;27;0;30;0
WireConnection;27;2;20;0
WireConnection;34;0;20;0
WireConnection;34;2;30;2
WireConnection;32;0;27;0
WireConnection;32;1;34;0
WireConnection;14;1;32;0
WireConnection;33;0;20;0
WireConnection;33;1;30;1
WireConnection;13;0;14;0
WireConnection;13;2;33;0
WireConnection;2;1;13;0
WireConnection;6;0;5;0
WireConnection;22;0;2;1
WireConnection;22;1;2;2
WireConnection;22;2;2;3
WireConnection;22;3;2;4
WireConnection;8;0;2;0
WireConnection;12;0;22;0
WireConnection;10;0;9;0
WireConnection;10;1;7;0
WireConnection;3;0;10;0
WireConnection;3;1;11;0
WireConnection;3;2;24;0
WireConnection;0;2;3;0
ASEEND*/
//CHKSM=CC39A6FF688AA176935783C75F05946702C08469