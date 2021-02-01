// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MudBun/Stopmotion Splat (Built-In RP)"
{
	Properties
	{
		[HideInInspector]_Color("Color", Color) = (1,1,1,1)
		[HideInInspector]_Emission("Emission", Color) = (1,1,1,1)
		[HideInInspector]_Metallic("Metallic", Range( 0 , 1)) = 0
		[HideInInspector]_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_MainTex("Albedo", 2D) = "white" {}
		_AlphaCutoutThreshold("Alpha Cutout Threshold", Range( 0 , 1)) = 0
		_Dithering("Dithering", Range( 0 , 1)) = 1
		_NoiseSize("Noise Size", Float) = 0.5
		_OffsetAmount("Offset Amount", Float) = 0.005
		_TimeInterval("Time Interval", Float) = 0.15
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
			float2 vertexToFrag34_g276;
			float4 vertexToFrag3_g276;
			float3 vertexToFrag20_g276;
			float3 vertexToFrag27_g278;
			uint ase_vertexId;
			float3 vertexToFrag11_g276;
			float vertexToFrag2_g276;
			float vertexToFrag14_g276;
		};

		uniform float _TimeInterval;
		uniform float _NoiseSize;
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


		float3 SimplexNoiseGradient6_g279( float3 Position, float Size )
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
			int VertexID5_g276 = v.ase_vertexId;
			float3 PositionLs5_g276 = float3( 0,0,0 );
			float3 NormalWs5_g276 = float3( 0,0,0 );
			float3 NormalLs5_g276 = float3( 0,0,0 );
			float3 TangentWs5_g276 = float3( 0,0,0 );
			float3 TangentLs5_g276 = float3( 0,0,0 );
			float3 CenterWs5_g276 = float3( 0,0,0 );
			float3 CenterLs5_g276 = float3( 0,0,0 );
			float4 Color5_g276 = float4( 0,0,0,0 );
			float4 EmissionHash5_g276 = float4( 0,0,0,0 );
			float Metallic5_g276 = 0.0;
			float Smoothness5_g276 = 0.0;
			float2 Tex5_g276 = float2( 0,0 );
			float4 TextureWeight5_g276 = float4( 1,0,0,0 );
			float SdfValue5_g276 = 0.0;
			float3 Normal2dLs5_g276 = float3( 0,0,0 );
			float3 Normal2dWs5_g276 = float3( 0,0,0 );
			float3 localMudBunSplatPoint5_g276 = MudBunSplatPoint( VertexID5_g276 , PositionLs5_g276 , NormalWs5_g276 , NormalLs5_g276 , TangentWs5_g276 , TangentLs5_g276 , CenterWs5_g276 , CenterLs5_g276 , Color5_g276 , EmissionHash5_g276 , Metallic5_g276 , Smoothness5_g276 , Tex5_g276 , TextureWeight5_g276 , SdfValue5_g276 , Normal2dLs5_g276 , Normal2dWs5_g276 );
			float3 temp_output_73_21 = localMudBunSplatPoint5_g276;
			float2 temp_cast_0 = (floor( ( _Time.y / _TimeInterval ) )).xx;
			float dotResult4_g277 = dot( temp_cast_0 , float2( 12.9898,78.233 ) );
			float lerpResult10_g277 = lerp( 0.0 , 10000.0 , frac( ( sin( dotResult4_g277 ) * 43758.55 ) ));
			float3 Position6_g279 = ( temp_output_73_21 + lerpResult10_g277 );
			float Size6_g279 = _NoiseSize;
			float3 localSimplexNoiseGradient6_g279 = SimplexNoiseGradient6_g279( Position6_g279 , Size6_g279 );
			v.vertex.xyz = ( temp_output_73_21 + ( localSimplexNoiseGradient6_g279 * _OffsetAmount ) );
			v.vertex.w = 1;
			float3 temp_output_73_28 = NormalWs5_g276;
			v.normal = temp_output_73_28;
			o.vertexToFrag34_g276 = Tex5_g276;
			o.vertexToFrag3_g276 = Color5_g276;
			o.vertexToFrag20_g276 = localMudBunSplatPoint5_g276;
			o.vertexToFrag27_g278 = temp_output_73_28;
			o.ase_vertexId = v.ase_vertexId;
			o.vertexToFrag11_g276 = (EmissionHash5_g276).xyz;
			o.vertexToFrag2_g276 = Metallic5_g276;
			o.vertexToFrag14_g276 = Smoothness5_g276;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 clampResult36_g276 = clamp( ( i.vertexToFrag34_g276 + float2( 0.5,0.5 ) ) , float2( 0,0 ) , float2( 1,1 ) );
			float4 temp_output_7_0_g276 = ( _Color * i.vertexToFrag3_g276 );
			float4 temp_output_69_0 = ( tex2D( _MainTex, clampResult36_g276 ) * temp_output_7_0_g276 );
			float localComputeOpaqueTransparency20_g278 = ( 0.0 );
			float4 unityObjectToClipPos17_g276 = UnityObjectToClipPos( i.vertexToFrag20_g276 );
			float4 computeScreenPos18_g276 = ComputeScreenPos( unityObjectToClipPos17_g276 );
			float2 ScreenPos20_g278 = (( ( computeScreenPos18_g276 / (computeScreenPos18_g276).w ) * _ScreenParams )).xy;
			float3 VertPos20_g278 = i.vertexToFrag27_g278;
			int VertexID5_g276 = i.ase_vertexId;
			float3 PositionLs5_g276 = float3( 0,0,0 );
			float3 NormalWs5_g276 = float3( 0,0,0 );
			float3 NormalLs5_g276 = float3( 0,0,0 );
			float3 TangentWs5_g276 = float3( 0,0,0 );
			float3 TangentLs5_g276 = float3( 0,0,0 );
			float3 CenterWs5_g276 = float3( 0,0,0 );
			float3 CenterLs5_g276 = float3( 0,0,0 );
			float4 Color5_g276 = float4( 0,0,0,0 );
			float4 EmissionHash5_g276 = float4( 0,0,0,0 );
			float Metallic5_g276 = 0.0;
			float Smoothness5_g276 = 0.0;
			float2 Tex5_g276 = float2( 0,0 );
			float4 TextureWeight5_g276 = float4( 1,0,0,0 );
			float SdfValue5_g276 = 0.0;
			float3 Normal2dLs5_g276 = float3( 0,0,0 );
			float3 Normal2dWs5_g276 = float3( 0,0,0 );
			float3 localMudBunSplatPoint5_g276 = MudBunSplatPoint( VertexID5_g276 , PositionLs5_g276 , NormalWs5_g276 , NormalLs5_g276 , TangentWs5_g276 , TangentLs5_g276 , CenterWs5_g276 , CenterLs5_g276 , Color5_g276 , EmissionHash5_g276 , Metallic5_g276 , Smoothness5_g276 , Tex5_g276 , TextureWeight5_g276 , SdfValue5_g276 , Normal2dLs5_g276 , Normal2dWs5_g276 );
			float Hash20_g278 = (EmissionHash5_g276).w;
			float ifLocalVar62_g276 = 0;
			UNITY_BRANCH 
			if( length( i.vertexToFrag34_g276 ) <= 0.5 )
				ifLocalVar62_g276 = 1.0;
			else
				ifLocalVar62_g276 = 0.0;
			#ifdef MUDBUN_QUAD_SPLATS
				float staticSwitch50_g276 = 1.0;
			#else
				float staticSwitch50_g276 = ifLocalVar62_g276;
			#endif
			float AlphaIn20_g278 = ( (temp_output_7_0_g276).a * staticSwitch50_g276 );
			float AlphaOut20_g278 = 0;
			float AlphaThreshold20_g278 = 0;
			sampler2D DitherNoiseTexture20_g278 = _DitherTexture;
			int DitherNoiseTextureSize20_g278 = _DitherTextureSize;
			int UseRandomDither20_g278 = (int)_RandomDither;
			float AlphaCutoutThreshold20_g278 = _AlphaCutoutThreshold;
			float DitherBlend20_g278 = _Dithering;
			float alpha = AlphaIn20_g278;
			computeOpaqueTransparency(ScreenPos20_g278, VertPos20_g278, Hash20_g278, DitherNoiseTexture20_g278, DitherNoiseTextureSize20_g278, UseRandomDither20_g278 > 0, AlphaCutoutThreshold20_g278, DitherBlend20_g278,  alpha, AlphaThreshold20_g278);
			AlphaOut20_g278 = alpha;
			clip( ( (temp_output_69_0).a * AlphaOut20_g278 ) - AlphaThreshold20_g278);
			o.Albedo = temp_output_69_0.rgb;
			o.Emission = ( i.vertexToFrag11_g276 * (_Emission).rgb );
			o.Metallic = ( _Metallic * i.vertexToFrag2_g276 );
			o.Smoothness = ( _Smoothness * i.vertexToFrag14_g276 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
-1542;209;1438;795;631.7072;185.8573;1.353746;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;22;-1408,672;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-1408,768;Inherit;False;Property;_TimeInterval;Time Interval;11;0;Create;True;0;0;False;0;False;0.15;0.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;24;-1152,640;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;73;-1408,-128;Inherit;False;Mud Splat;1;;276;1947d49d4d7bb92419410ba0439aa2bc;0;0;20;COLOR;22;FLOAT2;29;FLOAT;24;FLOAT3;27;FLOAT;26;FLOAT;23;FLOAT4;45;FLOAT3;21;FLOAT3;43;FLOAT3;28;FLOAT3;42;FLOAT3;52;FLOAT3;53;FLOAT3;60;FLOAT3;61;FLOAT3;68;FLOAT3;67;FLOAT;66;FLOAT2;25;FLOAT;57
Node;AmplifyShaderEditor.TexturePropertyNode;67;-1280,-384;Inherit;True;Property;_MainTex;Albedo;6;0;Create;False;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FloorOpNode;26;-992,640;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;68;-896,-384;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-1424,1280;Inherit;False;Property;_RandomDither;Random Dither;12;1;[Toggle];Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1424,1472;Inherit;False;Property;_Dithering;Dithering;8;0;Create;True;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;4;-1424,960;Inherit;True;Property;_DitherTexture;Dither Texture;13;0;Create;True;0;0;False;0;False;f240bbb7854046345b218811e5681a54;f240bbb7854046345b218811e5681a54;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.IntNode;8;-1424,1184;Inherit;False;Property;_DitherTextureSize;Dither Texture Size;14;0;Create;True;0;0;False;0;False;256;256;0;1;INT;0
Node;AmplifyShaderEditor.FunctionNode;27;-832,640;Inherit;False;Random Range;-1;;277;7b754edb8aebbfb4a9ace907af661cfc;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;10000;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1424,1376;Inherit;False;Property;_AlphaCutoutThreshold;Alpha Cutout Threshold;7;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-256,-128;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1408,512;Inherit;False;Property;_OffsetAmount;Offset Amount;10;0;Create;True;0;0;False;0;False;0.005;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1408,416;Inherit;False;Property;_NoiseSize;Noise Size;9;0;Create;True;0;0;False;0;False;0.5;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-256,608;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;9;-400,768;Inherit;False;Mud Alpha Threshold;-1;;278;926535703f4c32948ac1f55275a22bf0;0;9;8;FLOAT2;0,0;False;15;FLOAT3;0,0,0;False;18;FLOAT;0;False;22;FLOAT;0;False;19;SAMPLER2D;0;False;26;INT;256;False;9;INT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;2;FLOAT;24;FLOAT;25
Node;AmplifyShaderEditor.ComponentMaskNode;70;48,144;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;304,144;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;66;-64,560;Inherit;False;Mud Noise Gradient;-1;;279;ded4656e0e0531448b1f2a26fd64d584;0;3;2;FLOAT3;0,0,0;False;5;FLOAT;0.1;False;7;FLOAT;0.1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;304,352;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClipNode;74;672,-64;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;992,0;Float;False;True;-1;3;ASEMaterialInspector;0;0;Standard;MudBun/Stopmotion Splat (Built-In RP);False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;11;d3d11_9x;d3d11;glcore;gles3;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;24;0;22;0
WireConnection;24;1;23;0
WireConnection;26;0;24;0
WireConnection;68;0;67;0
WireConnection;68;1;73;29
WireConnection;27;1;26;0
WireConnection;69;0;68;0
WireConnection;69;1;73;22
WireConnection;28;0;73;21
WireConnection;28;1;27;0
WireConnection;9;8;73;25
WireConnection;9;15;73;28
WireConnection;9;18;73;57
WireConnection;9;22;73;24
WireConnection;9;19;4;0
WireConnection;9;26;8;0
WireConnection;9;9;6;0
WireConnection;9;6;7;0
WireConnection;9;7;5;0
WireConnection;70;0;69;0
WireConnection;71;0;70;0
WireConnection;71;1;9;24
WireConnection;66;2;28;0
WireConnection;66;5;11;0
WireConnection;66;7;19;0
WireConnection;14;0;73;21
WireConnection;14;1;66;0
WireConnection;74;0;69;0
WireConnection;74;1;71;0
WireConnection;74;2;9;25
WireConnection;0;0;74;0
WireConnection;0;2;73;27
WireConnection;0;3;73;26
WireConnection;0;4;73;23
WireConnection;0;11;14;0
WireConnection;0;12;73;28
ASEEND*/
//CHKSM=F354C812DCB44E0FB5C68DEEB991E7F972D10774