// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mud Locked Mesh (Built-In RP)"
{
	Properties
	{
		_AlphaCutoutThreshold("Alpha Cutout Threshold", Range( 0 , 1)) = 0
		_Dithering("Dithering", Range( 0 , 1)) = 1
		_DitherTexture("Dither Texture", 2D) = "white" {}
		_DitherTextureSize("Dither Texture Size", Int) = 256
		[Toggle]_RandomDither("Random Dither", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.5
		#define SHADER_GRAPH
		#include "Assets/MudBun/Shader/Render/ShaderCommon.cginc"
		#pragma exclude_renderers d3d9 d3d11_9x gles xbox360 psp2 n3ds wiiu 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)

		struct appdata_full_custom
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
			float4 ase_texcoord6 : TEXCOORD6;
			float4 ase_texcoord7 : TEXCOORD7;
		};
		struct Input
		{
			float4 vertexColor : COLOR;
			float4 screenPos;
			float3 vertexToFrag27_g43;
			float4 ase_texcoord7;
			float2 ase_texcoord8;
		};

		uniform sampler2D _DitherTexture;
		uniform int _DitherTextureSize;
		uniform float _RandomDither;
		uniform float _AlphaCutoutThreshold;
		uniform float _Dithering;

		void vertexDataFunc( inout appdata_full_custom v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			v.vertex.xyz = ase_vertex3Pos;
			v.vertex.w = 1;
			float3 ase_vertexNormal = v.normal.xyz;
			v.normal = ase_vertexNormal;
			float3 break46_g41 = ase_vertex3Pos;
			float4 appendResult45_g41 = (float4(break46_g41.x , break46_g41.y , break46_g41.z , 1.0));
			float4 transform44_g41 = mul(unity_ObjectToWorld,appendResult45_g41);
			o.vertexToFrag27_g43 = (transform44_g41).xyz;
			o.ase_texcoord7 = v.ase_texcoord6;
			o.ase_texcoord8 = v.ase_texcoord7;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float localComputeOpaqueTransparency20_g43 = ( 0.0 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 ScreenPos20_g43 = (( ase_screenPosNorm * _ScreenParams )).xy;
			float3 VertPos20_g43 = i.vertexToFrag27_g43;
			float Hash20_g43 = i.ase_texcoord7.w;
			float AlphaIn20_g43 = i.vertexColor.a;
			float AlphaOut20_g43 = 0;
			float AlphaThreshold20_g43 = 0;
			sampler2D DitherNoiseTexture20_g43 = _DitherTexture;
			int DitherNoiseTextureSize20_g43 = _DitherTextureSize;
			int UseRandomDither20_g43 = (int)_RandomDither;
			float AlphaCutoutThreshold20_g43 = _AlphaCutoutThreshold;
			float DitherBlend20_g43 = _Dithering;
			float alpha = AlphaIn20_g43;
			computeOpaqueTransparency(ScreenPos20_g43, VertPos20_g43, Hash20_g43, DitherNoiseTexture20_g43, DitherNoiseTextureSize20_g43, UseRandomDither20_g43 > 0, AlphaCutoutThreshold20_g43, DitherBlend20_g43,  alpha, AlphaThreshold20_g43);
			AlphaOut20_g43 = alpha;
			clip( AlphaOut20_g43 - AlphaThreshold20_g43);
			o.Albedo = (i.vertexColor).rgb;
			o.Emission = (i.ase_texcoord7).xyz;
			o.Metallic = i.ase_texcoord8.x;
			o.Smoothness = i.ase_texcoord8.y;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18500
-1542;203;1438;801;553.2913;389.6757;1.504229;True;False
Node;AmplifyShaderEditor.RangedFloatNode;9;-256,512;Inherit;False;Property;_RandomDither;Random Dither;4;1;[Toggle];Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-256,608;Inherit;False;Property;_AlphaCutoutThreshold;Alpha Cutout Threshold;0;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-256,704;Inherit;False;Property;_Dithering;Dithering;1;0;Create;True;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;50;-256,-128;Inherit;False;Mud Locked Mesh;-1;;41;7b8a07bde06607c4284a51a0d43ac96d;0;0;11;FLOAT3;0;FLOAT;3;FLOAT3;2;FLOAT;4;FLOAT;5;FLOAT3;43;FLOAT3;6;FLOAT3;31;FLOAT3;1;FLOAT2;7;FLOAT;23
Node;AmplifyShaderEditor.TexturePropertyNode;48;-256,192;Inherit;True;Property;_DitherTexture;Dither Texture;2;0;Create;True;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.IntNode;49;-256,416;Inherit;False;Property;_DitherTextureSize;Dither Texture Size;3;0;Create;True;0;0;False;0;False;256;256;0;1;INT;0
Node;AmplifyShaderEditor.FunctionNode;52;288,224;Inherit;False;Mud Alpha Threshold;-1;;43;926535703f4c32948ac1f55275a22bf0;0;9;8;FLOAT2;0,0;False;15;FLOAT3;0,0,0;False;18;FLOAT;0;False;22;FLOAT;0;False;19;SAMPLER2D;0;False;26;INT;256;False;9;INT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;2;FLOAT;24;FLOAT;25
Node;AmplifyShaderEditor.ClipNode;51;896,-128;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1280,-128;Float;False;True;-1;3;;0;0;Standard;Mud Locked Mesh (Built-In RP);False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;7;d3d11;glcore;gles3;metal;vulkan;xboxone;ps4;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;3;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;52;8;50;7
WireConnection;52;15;50;43
WireConnection;52;18;50;23
WireConnection;52;22;50;3
WireConnection;52;19;48;0
WireConnection;52;26;49;0
WireConnection;52;9;9;0
WireConnection;52;6;3;0
WireConnection;52;7;2;0
WireConnection;51;0;50;0
WireConnection;51;1;52;24
WireConnection;51;2;52;25
WireConnection;0;0;51;0
WireConnection;0;2;50;2
WireConnection;0;3;50;4
WireConnection;0;4;50;5
WireConnection;0;11;50;6
WireConnection;0;12;50;1
ASEEND*/
//CHKSM=3C2E22B1C8D41414FBBF3744855EA38B7C17E01B