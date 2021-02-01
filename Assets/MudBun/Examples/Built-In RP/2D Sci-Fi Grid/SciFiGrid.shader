// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Sci-Fi Grid"
{
	Properties
	{
		[HideInInspector]_Color("Color", Color) = (1,1,1,1)
		[HideInInspector]_Emission("Emission", Color) = (1,1,1,1)
		[HideInInspector]_Metallic("Metallic", Range( 0 , 1)) = 0
		[HideInInspector]_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_GridSize("Grid Size", Range( 0 , 1)) = 1
		_GridLineThickness("Grid Line Thickness", Range( 0 , 1)) = 0.05
		_OutlineThickness("Outline Thickness", Range( 0 , 1)) = 0.1
		_OuterFadeDistance("Outer Fade Distance", Range( 0.01 , 1)) = 1
		_DitherTexture("Dither Texture", 2D) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 4.0
		#define SHADER_GRAPH
		#pragma multi_compile _ MUDBUN_PROCEDURAL
		#include "Assets/MudBun/Shader/Render/ShaderCommon.cginc"
		#include "Assets/MudBun/Shader/Render/MeshCommon.cginc"
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
			float4 vertexToFrag5_g1;
			float3 vertexToFrag31;
			float vertexToFrag30;
			float3 vertexToFrag16_g1;
			float3 vertexToFrag27_g6;
			uint ase_vertexId;
			float3 vertexToFrag6_g1;
			float vertexToFrag8_g1;
			float vertexToFrag7_g1;
		};

		uniform float _GridLineThickness;
		uniform float _GridSize;
		uniform float _OuterFadeDistance;
		uniform float _OutlineThickness;
		uniform sampler2D _DitherTexture;


		float3 MudBunMeshPoint( int VertexID, out float3 PositionLs, out float3 NormalWs, out float3 NormalLs, out float4 Color, out float4 EmissionHash, out float Metallic, out float Smoothness, out float4 TextureWeight, out float SdfValue, out float3 Outward2dNormalLs, out float3 Outward2dNormalWs )
		{
			float4 positionWs;
			float2 metallicSmoothness;
			mudbun_mesh_vert(VertexID, positionWs, PositionLs, NormalWs, NormalLs, Color, EmissionHash, metallicSmoothness, TextureWeight, SdfValue, Outward2dNormalLs, Outward2dNormalWs);
			Metallic = metallicSmoothness.x;
			Smoothness = metallicSmoothness.y;
			return positionWs.xyz;
		}


		void vertexDataFunc( inout appdata_full_custom v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			int VertexID4_g1 = v.ase_vertexId;
			float3 PositionLs4_g1 = float3( 0,0,0 );
			float3 NormalWs4_g1 = float3( 0,0,0 );
			float3 NormalLs4_g1 = float3( 0,0,0 );
			float4 Color4_g1 = float4( 0,0,0,0 );
			float4 EmissionHash4_g1 = float4( 0,0,0,0 );
			float Metallic4_g1 = 0.0;
			float Smoothness4_g1 = 0.0;
			float4 TextureWeight4_g1 = float4( 1,0,0,0 );
			float SdfValue4_g1 = 0.0;
			float3 Outward2dNormalLs4_g1 = float3( 0,0,0 );
			float3 Outward2dNormalWs4_g1 = float3( 0,0,0 );
			float3 localMudBunMeshPoint4_g1 = MudBunMeshPoint( VertexID4_g1 , PositionLs4_g1 , NormalWs4_g1 , NormalLs4_g1 , Color4_g1 , EmissionHash4_g1 , Metallic4_g1 , Smoothness4_g1 , TextureWeight4_g1 , SdfValue4_g1 , Outward2dNormalLs4_g1 , Outward2dNormalWs4_g1 );
			float3 temp_output_1_0 = localMudBunMeshPoint4_g1;
			v.vertex.xyz = temp_output_1_0;
			v.vertex.w = 1;
			v.normal = NormalWs4_g1;
			o.vertexToFrag5_g1 = Color4_g1;
			o.vertexToFrag31 = temp_output_1_0;
			o.vertexToFrag30 = SdfValue4_g1;
			o.vertexToFrag16_g1 = localMudBunMeshPoint4_g1;
			o.vertexToFrag27_g6 = float3( 0,0,0 );
			o.ase_vertexId = v.ase_vertexId;
			o.vertexToFrag6_g1 = (EmissionHash4_g1).xyz;
			o.vertexToFrag8_g1 = Metallic4_g1;
			o.vertexToFrag7_g1 = Smoothness4_g1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_output_25_0_g1 = ( _Color * i.vertexToFrag5_g1 );
			float3 temp_cast_1 = (( _GridLineThickness * ( _GridSize * 0.5 ) )).xxx;
			float3 temp_cast_2 = (_GridSize).xxx;
			float3 fmodResult41 = frac(i.vertexToFrag31/temp_cast_2)*temp_cast_2;
			float3 break28 = ( temp_cast_1 - abs( ( fmodResult41 + ( _GridSize * -0.5 ) ) ) );
			float temp_output_3_0_g3 = ( max( max( break28.x , break28.y ) , break28.z ) - 0.0 );
			float temp_output_3_0_g5 = ( _OutlineThickness - i.vertexToFrag30 );
			float localComputeOpaqueTransparency20_g6 = ( 0.0 );
			float4 unityObjectToClipPos17_g1 = UnityObjectToClipPos( i.vertexToFrag16_g1 );
			float4 computeScreenPos18_g1 = ComputeScreenPos( unityObjectToClipPos17_g1 );
			float2 ScreenPos20_g6 = (( ( computeScreenPos18_g1 / (computeScreenPos18_g1).w ) * _ScreenParams )).xy;
			float3 VertPos20_g6 = i.vertexToFrag27_g6;
			int VertexID4_g1 = i.ase_vertexId;
			float3 PositionLs4_g1 = float3( 0,0,0 );
			float3 NormalWs4_g1 = float3( 0,0,0 );
			float3 NormalLs4_g1 = float3( 0,0,0 );
			float4 Color4_g1 = float4( 0,0,0,0 );
			float4 EmissionHash4_g1 = float4( 0,0,0,0 );
			float Metallic4_g1 = 0.0;
			float Smoothness4_g1 = 0.0;
			float4 TextureWeight4_g1 = float4( 1,0,0,0 );
			float SdfValue4_g1 = 0.0;
			float3 Outward2dNormalLs4_g1 = float3( 0,0,0 );
			float3 Outward2dNormalWs4_g1 = float3( 0,0,0 );
			float3 localMudBunMeshPoint4_g1 = MudBunMeshPoint( VertexID4_g1 , PositionLs4_g1 , NormalWs4_g1 , NormalLs4_g1 , Color4_g1 , EmissionHash4_g1 , Metallic4_g1 , Smoothness4_g1 , TextureWeight4_g1 , SdfValue4_g1 , Outward2dNormalLs4_g1 , Outward2dNormalWs4_g1 );
			float Hash20_g6 = (EmissionHash4_g1).w;
			float AlphaIn20_g6 = (temp_output_25_0_g1).a;
			float AlphaOut20_g6 = 0;
			float AlphaThreshold20_g6 = 0;
			sampler2D DitherNoiseTexture20_g6 = _DitherTexture;
			int DitherNoiseTextureSize20_g6 = 256;
			int UseRandomDither20_g6 = 0;
			float AlphaCutoutThreshold20_g6 = 0.0;
			float DitherBlend20_g6 = 1.0;
			float alpha = AlphaIn20_g6;
			computeOpaqueTransparency(ScreenPos20_g6, VertPos20_g6, Hash20_g6, DitherNoiseTexture20_g6, DitherNoiseTextureSize20_g6, UseRandomDither20_g6 > 0, AlphaCutoutThreshold20_g6, DitherBlend20_g6,  alpha, AlphaThreshold20_g6);
			AlphaOut20_g6 = alpha;
			clip( max( ( saturate( ( temp_output_3_0_g3 / fwidth( temp_output_3_0_g3 ) ) ) * ( 1.0 - saturate( ( i.vertexToFrag30 / _OuterFadeDistance ) ) ) ) , saturate( ( temp_output_3_0_g5 / fwidth( temp_output_3_0_g5 ) ) ) ) - AlphaThreshold20_g6);
			o.Albedo = temp_output_25_0_g1.rgb;
			o.Emission = ( i.vertexToFrag6_g1 * (_Emission).rgb );
			o.Metallic = ( _Metallic * i.vertexToFrag8_g1 );
			o.Smoothness = ( _Smoothness * i.vertexToFrag7_g1 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
-1542;209;1438;795;1722.593;66.04582;1.3;True;False
Node;AmplifyShaderEditor.FunctionNode;1;-1280,0;Inherit;False;Mud Mesh;0;;1;4f444db5091a94140ab2b15b933d37b6;0;0;15;COLOR;9;FLOAT;13;FLOAT3;10;FLOAT;11;FLOAT;12;FLOAT4;33;FLOAT3;0;FLOAT3;32;FLOAT3;2;FLOAT3;31;FLOAT3;48;FLOAT3;46;FLOAT;45;FLOAT2;15;FLOAT;41
Node;AmplifyShaderEditor.RangedFloatNode;33;-1280,448;Inherit;False;Property;_GridSize;Grid Size;5;0;Create;True;0;0;False;0;False;1;0.01;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;31;-832,320;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimplifiedFModOpNode;41;-608,320;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-832,512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-800,784;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-416,320;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;-0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1280,544;Inherit;False;Property;_GridLineThickness;Grid Line Thickness;6;0;Create;True;0;0;False;0;False;0.05;0.01;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-640,736;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;23;-288,320;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;22;-160,320;Inherit;False;2;0;FLOAT;0.1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1280,736;Inherit;False;Property;_OuterFadeDistance;Outer Fade Distance;8;0;Create;True;0;0;False;0;False;1;0;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;30;-832,432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;28;0,320;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMaxOpNode;27;240,320;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;18;-320,608;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;29;384,352;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;19;-192,608;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;24;512,352;Inherit;False;Step Antialiasing;-1;;3;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1280,640;Inherit;False;Property;_OutlineThickness;Outline Thickness;7;0;Create;True;0;0;False;0;False;0.1;0.01;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;20;-48,608;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;4;-1280,832;Inherit;True;Property;_DitherTexture;Dither Texture;9;0;Create;True;0;0;False;0;False;f240bbb7854046345b218811e5681a54;f240bbb7854046345b218811e5681a54;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;736,384;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;39;-224,480;Inherit;False;Step Antialiasing;-1;;5;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;38;896,416;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;3;-832,960;Inherit;False;Mud Alpha Threshold;-1;;6;926535703f4c32948ac1f55275a22bf0;0;9;8;FLOAT2;0,0;False;15;FLOAT3;0,0,0;False;18;FLOAT;0;False;22;FLOAT;0;False;19;SAMPLER2D;_Sampler193;False;26;INT;256;False;9;INT;0;False;6;FLOAT;0;False;7;FLOAT;1;False;2;FLOAT;24;FLOAT;25
Node;AmplifyShaderEditor.ClipNode;5;896,-32;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1152,0;Float;False;True;-1;4;ASEMaterialInspector;0;0;Standard;Sci-Fi Grid;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;31;0;1;0
WireConnection;41;0;31;0
WireConnection;41;1;33;0
WireConnection;34;0;33;0
WireConnection;36;0;33;0
WireConnection;21;0;41;0
WireConnection;21;1;34;0
WireConnection;35;0;32;0
WireConnection;35;1;36;0
WireConnection;23;0;21;0
WireConnection;22;0;35;0
WireConnection;22;1;23;0
WireConnection;30;0;1;45
WireConnection;28;0;22;0
WireConnection;27;0;28;0
WireConnection;27;1;28;1
WireConnection;18;0;30;0
WireConnection;18;1;17;0
WireConnection;29;0;27;0
WireConnection;29;1;28;2
WireConnection;19;0;18;0
WireConnection;24;2;29;0
WireConnection;20;0;19;0
WireConnection;26;0;24;0
WireConnection;26;1;20;0
WireConnection;39;1;30;0
WireConnection;39;2;37;0
WireConnection;38;0;26;0
WireConnection;38;1;39;0
WireConnection;3;8;1;15
WireConnection;3;18;1;41
WireConnection;3;22;1;13
WireConnection;3;19;4;0
WireConnection;5;0;1;9
WireConnection;5;1;38;0
WireConnection;5;2;3;25
WireConnection;0;0;5;0
WireConnection;0;2;1;10
WireConnection;0;3;1;11
WireConnection;0;4;1;12
WireConnection;0;11;1;0
WireConnection;0;12;1;2
ASEEND*/
//CHKSM=80688A43F1B8A2DD8100FA42B0429F41656A77CB