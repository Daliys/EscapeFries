// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MudBun/Stopmotion Mesh (Built-In RP)"
{
	Properties
	{
		[HideInInspector]_Color("Color", Color) = (1,1,1,1)
		[HideInInspector]_Emission("Emission", Color) = (1,1,1,1)
		[HideInInspector]_Metallic("Metallic", Range( 0 , 1)) = 0
		[HideInInspector]_Smoothness("Smoothness", Range( 0 , 1)) = 1
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
		#include "Assets/MudBun/Shader/Render/ShaderCommon.cginc"
		#include "Assets/MudBun/Shader/Render/MeshCommon.cginc"
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
			float4 vertexToFrag5_g272;
			float3 vertexToFrag16_g272;
			float3 vertexToFrag27_g270;
			uint ase_vertexId;
			float3 vertexToFrag6_g272;
			float vertexToFrag8_g272;
			float vertexToFrag7_g272;
		};

		uniform float _TimeInterval;
		uniform float _NoiseSize;
		uniform float _OffsetAmount;
		uniform sampler2D _DitherTexture;
		uniform int _DitherTextureSize;
		uniform float _RandomDither;
		uniform float _AlphaCutoutThreshold;
		uniform float _Dithering;


		float3 MudBunMeshPoint( int VertexID, out float3 PositionLs, out float3 NormalWs, out float3 NormalLs, out float4 Color, out float4 EmissionHash, out float Metallic, out float Smoothness, out float4 TextureWeight, out float SdfValue, out float3 Normal2dLs, out float3 Normal2dWs )
		{
			float4 positionWs;
			float2 metallicSmoothness;
			mudbun_mesh_vert(VertexID, positionWs, PositionLs, NormalWs, NormalLs, Color, EmissionHash, metallicSmoothness, TextureWeight, SdfValue, Normal2dLs, Normal2dWs);
			Metallic = metallicSmoothness.x;
			Smoothness = metallicSmoothness.y;
			return positionWs.xyz;
		}


		float3 SimplexNoiseGradient6_g271( float3 Position, float Size )
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
			int VertexID4_g272 = v.ase_vertexId;
			float3 PositionLs4_g272 = float3( 0,0,0 );
			float3 NormalWs4_g272 = float3( 0,0,0 );
			float3 NormalLs4_g272 = float3( 0,0,0 );
			float4 Color4_g272 = float4( 0,0,0,0 );
			float4 EmissionHash4_g272 = float4( 0,0,0,0 );
			float Metallic4_g272 = 0.0;
			float Smoothness4_g272 = 0.0;
			float4 TextureWeight4_g272 = float4( 1,0,0,0 );
			float SdfValue4_g272 = 0.0;
			float3 Normal2dLs4_g272 = float3( 0,0,0 );
			float3 Normal2dWs4_g272 = float3( 0,0,0 );
			float3 localMudBunMeshPoint4_g272 = MudBunMeshPoint( VertexID4_g272 , PositionLs4_g272 , NormalWs4_g272 , NormalLs4_g272 , Color4_g272 , EmissionHash4_g272 , Metallic4_g272 , Smoothness4_g272 , TextureWeight4_g272 , SdfValue4_g272 , Normal2dLs4_g272 , Normal2dWs4_g272 );
			float3 temp_output_66_0 = localMudBunMeshPoint4_g272;
			float2 temp_cast_0 = (floor( ( _Time.y / _TimeInterval ) )).xx;
			float dotResult4_g269 = dot( temp_cast_0 , float2( 12.9898,78.233 ) );
			float lerpResult10_g269 = lerp( 0.0 , 10000.0 , frac( ( sin( dotResult4_g269 ) * 43758.55 ) ));
			float3 Position6_g271 = ( temp_output_66_0 + lerpResult10_g269 );
			float Size6_g271 = _NoiseSize;
			float3 localSimplexNoiseGradient6_g271 = SimplexNoiseGradient6_g271( Position6_g271 , Size6_g271 );
			v.vertex.xyz = ( temp_output_66_0 + ( localSimplexNoiseGradient6_g271 * _OffsetAmount ) );
			v.vertex.w = 1;
			v.normal = NormalWs4_g272;
			o.vertexToFrag5_g272 = Color4_g272;
			o.vertexToFrag16_g272 = localMudBunMeshPoint4_g272;
			o.vertexToFrag27_g270 = temp_output_66_0;
			o.ase_vertexId = v.ase_vertexId;
			o.vertexToFrag6_g272 = (EmissionHash4_g272).xyz;
			o.vertexToFrag8_g272 = Metallic4_g272;
			o.vertexToFrag7_g272 = Smoothness4_g272;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_output_25_0_g272 = ( _Color * i.vertexToFrag5_g272 );
			float localComputeOpaqueTransparency20_g270 = ( 0.0 );
			float4 unityObjectToClipPos17_g272 = UnityObjectToClipPos( i.vertexToFrag16_g272 );
			float4 computeScreenPos18_g272 = ComputeScreenPos( unityObjectToClipPos17_g272 );
			float2 ScreenPos20_g270 = (( ( computeScreenPos18_g272 / (computeScreenPos18_g272).w ) * _ScreenParams )).xy;
			float3 VertPos20_g270 = i.vertexToFrag27_g270;
			int VertexID4_g272 = i.ase_vertexId;
			float3 PositionLs4_g272 = float3( 0,0,0 );
			float3 NormalWs4_g272 = float3( 0,0,0 );
			float3 NormalLs4_g272 = float3( 0,0,0 );
			float4 Color4_g272 = float4( 0,0,0,0 );
			float4 EmissionHash4_g272 = float4( 0,0,0,0 );
			float Metallic4_g272 = 0.0;
			float Smoothness4_g272 = 0.0;
			float4 TextureWeight4_g272 = float4( 1,0,0,0 );
			float SdfValue4_g272 = 0.0;
			float3 Normal2dLs4_g272 = float3( 0,0,0 );
			float3 Normal2dWs4_g272 = float3( 0,0,0 );
			float3 localMudBunMeshPoint4_g272 = MudBunMeshPoint( VertexID4_g272 , PositionLs4_g272 , NormalWs4_g272 , NormalLs4_g272 , Color4_g272 , EmissionHash4_g272 , Metallic4_g272 , Smoothness4_g272 , TextureWeight4_g272 , SdfValue4_g272 , Normal2dLs4_g272 , Normal2dWs4_g272 );
			float Hash20_g270 = (EmissionHash4_g272).w;
			float AlphaIn20_g270 = (temp_output_25_0_g272).a;
			float AlphaOut20_g270 = 0;
			float AlphaThreshold20_g270 = 0;
			sampler2D DitherNoiseTexture20_g270 = _DitherTexture;
			int DitherNoiseTextureSize20_g270 = _DitherTextureSize;
			int UseRandomDither20_g270 = (int)_RandomDither;
			float AlphaCutoutThreshold20_g270 = _AlphaCutoutThreshold;
			float DitherBlend20_g270 = _Dithering;
			float alpha = AlphaIn20_g270;
			computeOpaqueTransparency(ScreenPos20_g270, VertPos20_g270, Hash20_g270, DitherNoiseTexture20_g270, DitherNoiseTextureSize20_g270, UseRandomDither20_g270 > 0, AlphaCutoutThreshold20_g270, DitherBlend20_g270,  alpha, AlphaThreshold20_g270);
			AlphaOut20_g270 = alpha;
			clip( AlphaOut20_g270 - AlphaThreshold20_g270);
			o.Albedo = temp_output_25_0_g272.rgb;
			o.Emission = ( i.vertexToFrag6_g272 * (_Emission).rgb );
			o.Metallic = ( _Metallic * i.vertexToFrag8_g272 );
			o.Smoothness = ( _Smoothness * i.vertexToFrag7_g272 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
-1542;209;1438;795;2176.275;68.61555;1.670302;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;22;-1408,672;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-1408,768;Inherit;False;Property;_TimeInterval;Time Interval;10;0;Create;True;0;0;False;0;False;0.15;0.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;24;-1152,640;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;26;-992,640;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;66;-1408,0;Inherit;False;Mud Mesh;1;;272;4f444db5091a94140ab2b15b933d37b6;0;0;15;COLOR;9;FLOAT;13;FLOAT3;10;FLOAT;11;FLOAT;12;FLOAT4;33;FLOAT3;0;FLOAT3;32;FLOAT3;2;FLOAT3;31;FLOAT3;48;FLOAT3;46;FLOAT;45;FLOAT2;15;FLOAT;41
Node;AmplifyShaderEditor.FunctionNode;27;-832,640;Inherit;False;Random Range;-1;;269;7b754edb8aebbfb4a9ace907af661cfc;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;10000;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-256,608;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1408,512;Inherit;False;Property;_OffsetAmount;Offset Amount;9;0;Create;True;0;0;False;0;False;0.005;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1424,1280;Inherit;False;Property;_RandomDither;Random Dither;11;1;[Toggle];Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;8;-1424,1184;Inherit;False;Property;_DitherTextureSize;Dither Texture Size;13;0;Create;True;0;0;False;0;False;256;256;0;1;INT;0
Node;AmplifyShaderEditor.TexturePropertyNode;4;-1424,960;Inherit;True;Property;_DitherTexture;Dither Texture;12;0;Create;True;0;0;False;0;False;f240bbb7854046345b218811e5681a54;f240bbb7854046345b218811e5681a54;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;7;-1424,1376;Inherit;False;Property;_AlphaCutoutThreshold;Alpha Cutout Threshold;6;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1408,416;Inherit;False;Property;_NoiseSize;Noise Size;8;0;Create;True;0;0;False;0;False;0.5;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1424,1472;Inherit;False;Property;_Dithering;Dithering;7;0;Create;True;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;9;-400,768;Inherit;False;Mud Alpha Threshold;-1;;270;926535703f4c32948ac1f55275a22bf0;0;9;8;FLOAT2;0,0;False;15;FLOAT3;0,0,0;False;18;FLOAT;0;False;22;FLOAT;0;False;19;SAMPLER2D;0;False;26;INT;256;False;9;INT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;2;FLOAT;24;FLOAT;25
Node;AmplifyShaderEditor.FunctionNode;65;-64,560;Inherit;False;Mud Noise Gradient;-1;;271;ded4656e0e0531448b1f2a26fd64d584;0;3;2;FLOAT3;0,0,0;False;5;FLOAT;0.1;False;7;FLOAT;0.1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClipNode;16;544,512;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;256,512;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1024,0;Float;False;True;-1;3;ASEMaterialInspector;0;0;Standard;MudBun/Stopmotion Mesh (Built-In RP);False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;11;d3d11_9x;d3d11;glcore;gles3;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;24;0;22;0
WireConnection;24;1;23;0
WireConnection;26;0;24;0
WireConnection;27;1;26;0
WireConnection;28;0;66;0
WireConnection;28;1;27;0
WireConnection;9;8;66;15
WireConnection;9;15;66;0
WireConnection;9;18;66;41
WireConnection;9;22;66;13
WireConnection;9;19;4;0
WireConnection;9;26;8;0
WireConnection;9;9;6;0
WireConnection;9;6;7;0
WireConnection;9;7;5;0
WireConnection;65;2;28;0
WireConnection;65;5;11;0
WireConnection;65;7;19;0
WireConnection;16;0;66;9
WireConnection;16;1;9;24
WireConnection;16;2;9;25
WireConnection;14;0;66;0
WireConnection;14;1;65;0
WireConnection;0;0;16;0
WireConnection;0;2;66;10
WireConnection;0;3;66;11
WireConnection;0;4;66;12
WireConnection;0;11;14;0
WireConnection;0;12;66;2
ASEEND*/
//CHKSM=635DFBC3B3FDBD950CA80F7CF65589326BDC9BB8