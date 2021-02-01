// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MudBun/Floater Splat (Built-In RP)"
{
	Properties
	{
		[HideInInspector]_Color("Color", Color) = (1,1,1,1)
		[HideInInspector]_Emission("Emission", Color) = (1,1,1,1)
		[HideInInspector]_Metallic("Metallic", Range( 0 , 1)) = 0
		[HideInInspector]_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_Frequency("Frequency", Range( 0 , 1)) = 0.1
		_OffsetAmount("Offset Amount", Float) = 0.01
		_MainTex("Albedo", 2D) = "white" {}
		_AlphaCutoutThreshold("Alpha Cutout Threshold", Range( 0 , 1)) = 0
		_Dithering("Dithering", Range( 0 , 1)) = 1
		[Toggle]_RandomDither("Random Dither", Range( 0 , 1)) = 0
		_DitherTexture("Dither Texture", 2D) = "white" {}
		_DitherTextureSize("Dither Texture Size", Int) = 256
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.5
		#define SHADER_GRAPH
		#pragma multi_compile _ MUDBUN_PROCEDURAL
		#pragma multi_compile _ MUDBUN_QUAD_SPLATS
		#include "Assets/MudBun/Shader/Render/ShaderCommon.cginc"
		#include "Assets/MudBun/Shader/Render/SplatCommon.cginc"
		#pragma exclude_renderers d3d9 gles metal 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 

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
			uint ase_vertexId : SV_VertexID;
		};
		struct Input
		{
			float2 vertexToFrag34_g286;
			float4 vertexToFrag3_g286;
			float3 vertexToFrag20_g286;
			float3 vertexToFrag27_g285;
			uint ase_vertexId;
			float3 vertexToFrag11_g286;
			float vertexToFrag2_g286;
			float vertexToFrag14_g286;
		};

		uniform float _Frequency;
		uniform float _OffsetAmount;
		uniform sampler2D _MainTex;
		uniform sampler2D _DitherTexture;
		uniform int _DitherTextureSize;
		uniform float _RandomDither;
		uniform float _AlphaCutoutThreshold;
		uniform float _Dithering;


		float3 MudBunSplatPoint( int VertexID, out float3 PositionLs, out float3 NormalWs, out float3 NormalLs, out float3 TangentWs, out float3 TangentLs, out float3 CenterWs, out float3 CenterLs, out float4 Color, out float4 EmissionHash, out float Metallic, out float Smoothness, out float2 Tex, out float4 TextureWeight, out float SdfValue, out float3 Normal2dLs, out float3 Normal2dWs )
		{
			float4 positionWs;
			float2 metallicSmoothness;
			mudbun_splat_vert(VertexID, positionWs, PositionLs, NormalWs, NormalLs, TangentWs, TangentLs, CenterWs, CenterLs, Color, EmissionHash, metallicSmoothness, Tex, TextureWeight, SdfValue, Normal2dLs, Normal2dWs);
			Metallic = metallicSmoothness.x;
			Smoothness = metallicSmoothness.y;
			return positionWs.xyz;
		}


		float3 SimplexNoiseGradient6_g283( float3 Position, float Size )
		{
			#ifdef MUDBUN_VALID
			return snoise_grad(Position / max(1e-6, Size)).xyz;
			#else
			return Position;
			#endif
		}


		float3 SimplexNoiseGradient6_g284( float3 Position, float Size )
		{
			#ifdef MUDBUN_VALID
			return snoise_grad(Position / max(1e-6, Size)).xyz;
			#else
			return Position;
			#endif
		}


		void vertexDataFunc( inout appdata_full_custom v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			int VertexID5_g286 = v.ase_vertexId;
			float3 PositionLs5_g286 = float3( 0,0,0 );
			float3 NormalWs5_g286 = float3( 0,0,0 );
			float3 NormalLs5_g286 = float3( 0,0,0 );
			float3 TangentWs5_g286 = float3( 0,0,0 );
			float3 TangentLs5_g286 = float3( 0,0,0 );
			float3 CenterWs5_g286 = float3( 0,0,0 );
			float3 CenterLs5_g286 = float3( 0,0,0 );
			float4 Color5_g286 = float4( 0,0,0,0 );
			float4 EmissionHash5_g286 = float4( 0,0,0,0 );
			float Metallic5_g286 = 0.0;
			float Smoothness5_g286 = 0.0;
			float2 Tex5_g286 = float2( 0,0 );
			float4 TextureWeight5_g286 = float4( 1,0,0,0 );
			float SdfValue5_g286 = 0.0;
			float3 Normal2dLs5_g286 = float3( 0,0,0 );
			float3 Normal2dWs5_g286 = float3( 0,0,0 );
			float3 localMudBunSplatPoint5_g286 = MudBunSplatPoint( VertexID5_g286 , PositionLs5_g286 , NormalWs5_g286 , NormalLs5_g286 , TangentWs5_g286 , TangentLs5_g286 , CenterWs5_g286 , CenterLs5_g286 , Color5_g286 , EmissionHash5_g286 , Metallic5_g286 , Smoothness5_g286 , Tex5_g286 , TextureWeight5_g286 , SdfValue5_g286 , Normal2dLs5_g286 , Normal2dWs5_g286 );
			float3 temp_output_91_52 = TangentWs5_g286;
			float3 temp_output_91_60 = CenterWs5_g286;
			float3 Position6_g283 = temp_output_91_60;
			float Size6_g283 = 0.1;
			float3 localSimplexNoiseGradient6_g283 = SimplexNoiseGradient6_g283( Position6_g283 , Size6_g283 );
			float3 Position6_g284 = ( temp_output_91_60 + ( _Time.y * _Frequency ) + ( localSimplexNoiseGradient6_g283 * 1.0 ) );
			float Size6_g284 = 0.5;
			float3 localSimplexNoiseGradient6_g284 = SimplexNoiseGradient6_g284( Position6_g284 , Size6_g284 );
			float3 temp_output_66_0 = ( localSimplexNoiseGradient6_g284 * _OffsetAmount );
			float3 temp_output_91_28 = NormalWs5_g286;
			v.vertex.xyz = ( localMudBunSplatPoint5_g286 + ( temp_output_91_52 * (temp_output_66_0).x ) + ( cross( temp_output_91_28 , temp_output_91_52 ) * (temp_output_66_0).z ) );
			v.vertex.w = 1;
			v.normal = temp_output_91_28;
			o.vertexToFrag34_g286 = Tex5_g286;
			o.vertexToFrag3_g286 = Color5_g286;
			o.vertexToFrag20_g286 = localMudBunSplatPoint5_g286;
			o.vertexToFrag27_g285 = temp_output_91_28;
			o.ase_vertexId = v.ase_vertexId;
			o.vertexToFrag11_g286 = (EmissionHash5_g286).xyz;
			o.vertexToFrag2_g286 = Metallic5_g286;
			o.vertexToFrag14_g286 = Smoothness5_g286;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 clampResult36_g286 = clamp( ( i.vertexToFrag34_g286 + float2( 0.5,0.5 ) ) , float2( 0,0 ) , float2( 1,1 ) );
			float4 temp_output_7_0_g286 = ( _Color * i.vertexToFrag3_g286 );
			float4 temp_output_69_0 = ( tex2D( _MainTex, clampResult36_g286 ) * temp_output_7_0_g286 );
			float localComputeOpaqueTransparency20_g285 = ( 0.0 );
			float4 unityObjectToClipPos17_g286 = UnityObjectToClipPos( i.vertexToFrag20_g286 );
			float4 computeScreenPos18_g286 = ComputeScreenPos( unityObjectToClipPos17_g286 );
			float2 ScreenPos20_g285 = (( ( computeScreenPos18_g286 / (computeScreenPos18_g286).w ) * _ScreenParams )).xy;
			float3 VertPos20_g285 = i.vertexToFrag27_g285;
			int VertexID5_g286 = i.ase_vertexId;
			float3 PositionLs5_g286 = float3( 0,0,0 );
			float3 NormalWs5_g286 = float3( 0,0,0 );
			float3 NormalLs5_g286 = float3( 0,0,0 );
			float3 TangentWs5_g286 = float3( 0,0,0 );
			float3 TangentLs5_g286 = float3( 0,0,0 );
			float3 CenterWs5_g286 = float3( 0,0,0 );
			float3 CenterLs5_g286 = float3( 0,0,0 );
			float4 Color5_g286 = float4( 0,0,0,0 );
			float4 EmissionHash5_g286 = float4( 0,0,0,0 );
			float Metallic5_g286 = 0.0;
			float Smoothness5_g286 = 0.0;
			float2 Tex5_g286 = float2( 0,0 );
			float4 TextureWeight5_g286 = float4( 1,0,0,0 );
			float SdfValue5_g286 = 0.0;
			float3 Normal2dLs5_g286 = float3( 0,0,0 );
			float3 Normal2dWs5_g286 = float3( 0,0,0 );
			float3 localMudBunSplatPoint5_g286 = MudBunSplatPoint( VertexID5_g286 , PositionLs5_g286 , NormalWs5_g286 , NormalLs5_g286 , TangentWs5_g286 , TangentLs5_g286 , CenterWs5_g286 , CenterLs5_g286 , Color5_g286 , EmissionHash5_g286 , Metallic5_g286 , Smoothness5_g286 , Tex5_g286 , TextureWeight5_g286 , SdfValue5_g286 , Normal2dLs5_g286 , Normal2dWs5_g286 );
			float Hash20_g285 = (EmissionHash5_g286).w;
			float ifLocalVar62_g286 = 0;
			UNITY_BRANCH 
			if( length( i.vertexToFrag34_g286 ) <= 0.5 )
				ifLocalVar62_g286 = 1.0;
			else
				ifLocalVar62_g286 = 0.0;
			#ifdef MUDBUN_QUAD_SPLATS
				float staticSwitch50_g286 = 1.0;
			#else
				float staticSwitch50_g286 = ifLocalVar62_g286;
			#endif
			float AlphaIn20_g285 = ( (temp_output_7_0_g286).a * staticSwitch50_g286 );
			float AlphaOut20_g285 = 0;
			float AlphaThreshold20_g285 = 0;
			sampler2D DitherNoiseTexture20_g285 = _DitherTexture;
			int DitherNoiseTextureSize20_g285 = _DitherTextureSize;
			int UseRandomDither20_g285 = (int)_RandomDither;
			float AlphaCutoutThreshold20_g285 = _AlphaCutoutThreshold;
			float DitherBlend20_g285 = _Dithering;
			float alpha = AlphaIn20_g285;
			computeOpaqueTransparency(ScreenPos20_g285, VertPos20_g285, Hash20_g285, DitherNoiseTexture20_g285, DitherNoiseTextureSize20_g285, UseRandomDither20_g285 > 0, AlphaCutoutThreshold20_g285, DitherBlend20_g285,  alpha, AlphaThreshold20_g285);
			AlphaOut20_g285 = alpha;
			clip( ( (temp_output_69_0).a * AlphaOut20_g285 ) - AlphaThreshold20_g285);
			o.Albedo = temp_output_69_0.rgb;
			o.Emission = ( i.vertexToFrag11_g286 * (_Emission).rgb );
			o.Metallic = ( _Metallic * i.vertexToFrag2_g286 );
			o.Smoothness = ( _Smoothness * i.vertexToFrag14_g286 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
-1542;209;1438;795;2483.159;331.085;2.0545;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;22;-1456,576;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;91;-1408,-128;Inherit;False;Mud Splat;1;;286;1947d49d4d7bb92419410ba0439aa2bc;0;0;20;COLOR;22;FLOAT2;29;FLOAT;24;FLOAT3;27;FLOAT;26;FLOAT;23;FLOAT4;45;FLOAT3;21;FLOAT3;43;FLOAT3;28;FLOAT3;42;FLOAT3;52;FLOAT3;53;FLOAT3;60;FLOAT3;61;FLOAT3;68;FLOAT3;67;FLOAT;66;FLOAT2;25;FLOAT;57
Node;AmplifyShaderEditor.RangedFloatNode;73;-1456,672;Inherit;False;Property;_Frequency;Frequency;6;0;Create;True;0;0;False;0;False;0.1;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1168,576;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;88;-1232,448;Inherit;False;Mud Noise Gradient;-1;;283;ded4656e0e0531448b1f2a26fd64d584;0;3;2;FLOAT3;0,0,0;False;5;FLOAT;0.1;False;7;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;67;-1280,-384;Inherit;True;Property;_MainTex;Albedo;8;0;Create;False;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;19;-1456,768;Inherit;False;Property;_OffsetAmount;Offset Amount;7;0;Create;True;0;0;False;0;False;0.01;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;68;-896,-384;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-960,496;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-256,-128;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;4;-1408,1088;Inherit;True;Property;_DitherTexture;Dither Texture;12;0;Create;True;0;0;False;0;False;f240bbb7854046345b218811e5681a54;f240bbb7854046345b218811e5681a54;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FunctionNode;66;-784,528;Inherit;False;Mud Noise Gradient;-1;;284;ded4656e0e0531448b1f2a26fd64d584;0;3;2;FLOAT3;0,0,0;False;5;FLOAT;0.5;False;7;FLOAT;0.1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1408,1600;Inherit;False;Property;_Dithering;Dithering;10;0;Create;True;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;8;-1408,1312;Inherit;False;Property;_DitherTextureSize;Dither Texture Size;13;0;Create;True;0;0;False;0;False;256;256;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1408,1408;Inherit;False;Property;_RandomDither;Random Dither;11;1;[Toggle];Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1408,1504;Inherit;False;Property;_AlphaCutoutThreshold;Alpha Cutout Threshold;9;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CrossProductOpNode;85;-688,752;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;70;-48,-80;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;84;-496,608;Inherit;False;False;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;9;-288,768;Inherit;False;Mud Alpha Threshold;-1;;285;926535703f4c32948ac1f55275a22bf0;0;9;8;FLOAT2;0,0;False;15;FLOAT3;0,0,0;False;18;FLOAT;0;False;22;FLOAT;0;False;19;SAMPLER2D;0;False;26;INT;256;False;9;INT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;2;FLOAT;24;FLOAT;25
Node;AmplifyShaderEditor.ComponentMaskNode;83;-496,480;Inherit;False;True;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;208,-80;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-224,608;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-224,480;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;512,384;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClipNode;72;512,-160;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1024,0;Float;False;True;-1;3;ASEMaterialInspector;0;0;Standard;MudBun/Floater Splat (Built-In RP);False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;11;d3d11_9x;d3d11;glcore;gles3;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;74;0;22;0
WireConnection;74;1;73;0
WireConnection;88;2;91;60
WireConnection;68;0;67;0
WireConnection;68;1;91;29
WireConnection;28;0;91;60
WireConnection;28;1;74;0
WireConnection;28;2;88;0
WireConnection;69;0;68;0
WireConnection;69;1;91;22
WireConnection;66;2;28;0
WireConnection;66;7;19;0
WireConnection;85;0;91;28
WireConnection;85;1;91;52
WireConnection;70;0;69;0
WireConnection;84;0;66;0
WireConnection;9;8;91;25
WireConnection;9;15;91;28
WireConnection;9;18;91;57
WireConnection;9;22;91;24
WireConnection;9;19;4;0
WireConnection;9;26;8;0
WireConnection;9;9;6;0
WireConnection;9;6;7;0
WireConnection;9;7;5;0
WireConnection;83;0;66;0
WireConnection;71;0;70;0
WireConnection;71;1;9;24
WireConnection;81;0;85;0
WireConnection;81;1;84;0
WireConnection;80;0;91;52
WireConnection;80;1;83;0
WireConnection;14;0;91;21
WireConnection;14;1;80;0
WireConnection;14;2;81;0
WireConnection;72;0;69;0
WireConnection;72;1;71;0
WireConnection;72;2;9;25
WireConnection;0;0;72;0
WireConnection;0;2;91;27
WireConnection;0;3;91;26
WireConnection;0;4;91;23
WireConnection;0;11;14;0
WireConnection;0;12;91;28
ASEEND*/
//CHKSM=5D04493FB152E5C36F98B2531BEA882FEBAFAAFD