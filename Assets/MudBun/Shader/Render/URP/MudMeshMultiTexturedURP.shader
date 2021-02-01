// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MudBun/Mud Mesh Multi-Textured (URP)"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector]_Color("Color", Color) = (1,1,1,1)
		[HideInInspector]_Emission("Emission", Color) = (1,1,1,1)
		[HideInInspector]_Metallic("Metallic", Range( 0 , 1)) = 0
		[HideInInspector]_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_AlphaCutoutThreshold("Alpha Cutout Threshold", Range( 0 , 1)) = 0
		_Dithering("Dithering", Range( 0 , 1)) = 1
		[Toggle]_UseTex0("Use Texture 0", Float) = 0
		_MainTex("Texture 0", 2D) = "white" {}
		[Toggle]_RandomDither("Random Dither", Range( 0 , 1)) = 0
		[Toggle]_MainTexX("Project Texture 0 X", Float) = 1
		[Toggle]_MainTexY("Project Texture 0 Y", Float) = 1
		[Toggle]_MainTexZ("Project Texture 0 Z", Float) = 1
		[Toggle]_UseTex1("Use Texture 1", Float) = 0
		_Tex1("Texture 1", 2D) = "white" {}
		[Toggle]_Tex1X("Project Texture 1 X", Float) = 1
		[Toggle]_Tex1Y("Project Texture 1 Y", Float) = 1
		[Toggle]_Tex1Z("Project Texture 1 Z", Float) = 1
		[Toggle]_UseTex2("Use Texture 2", Float) = 0
		_Tex2("Texture 2", 2D) = "white" {}
		[Toggle]_Tex2X("Project Texture 2 X", Float) = 1
		[Toggle]_Tex2Y("Project Texture 2 Y", Float) = 1
		[Toggle]_Tex2Z("Project Texture 2 Z", Float) = 1
		[Toggle]_UseTex3("Use Texture 3", Float) = 0
		_Tex3("Texture 3", 2D) = "white" {}
		_DitherTexture("Dither Texture", 2D) = "white" {}
		[Toggle]_Tex3X("Project Texture 3 X", Float) = 1
		_DitherTextureSize("Dither Texture Size", Int) = 256
		[Toggle]_Tex3Y("Project Texture 3 Y", Float) = 1
		[Toggle]_Tex3Z("Project Texture 3 Z", Float) = 1

		[HideInInspector]_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		[HideInInspector]_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		[HideInInspector]_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		[HideInInspector]_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		[HideInInspector]_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		[HideInInspector]_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		[HideInInspector]_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		[HideInInspector]_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		[HideInInspector]_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		[HideInInspector]_TessMin( "Tess Min Distance", Float ) = 10
		[HideInInspector]_TessMax( "Tess Max Distance", Float ) = 25
		[HideInInspector]_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		[HideInInspector]_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		Cull Back
		HLSLINCLUDE
		#pragma target 4.0

		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero , One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#define SHADER_GRAPH
			#pragma multi_compile _ MUDBUN_PROCEDURAL
			#include "Assets/MudBun/Shader/Render/ShaderCommon.cginc"
			#include "Assets/MudBun/Shader/Render/MeshCommon.cginc"


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				uint ase_vertexID : SV_VertexID;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				float4 ase_texcoord9 : TEXCOORD9;
				float4 ase_texcoord10 : TEXCOORD10;
				float4 ase_texcoord11 : TEXCOORD11;
				float4 ase_texcoord12 : TEXCOORD12;
				float4 ase_texcoord13 : TEXCOORD13;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _UseTex0;
			float _RandomDither;
			int _DitherTextureSize;
			float _Tex3Z;
			float _Tex3Y;
			float _Tex3X;
			float _Tex2Z;
			float _Tex2Y;
			float _Tex2X;
			float _Tex1Z;
			float _Tex1Y;
			float _Tex1X;
			float _MainTexZ;
			float _MainTexY;
			float _MainTexX;
			float _UseTex3;
			float _UseTex2;
			float _UseTex1;
			float _AlphaCutoutThreshold;
			float _Dithering;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _MainTex;
			sampler2D _Tex1;
			sampler2D _Tex2;
			sampler2D _Tex3;
			sampler2D _DitherTexture;


			float3 MudBunMeshPoint( int VertexID , out float3 PositionLs , out float3 NormalWs , out float3 NormalLs , out float4 Color , out float4 EmissionHash , out float Metallic , out float Smoothness , out float4 TextureWeight , out float SdfValue , out float3 Outward2dNormalLs , out float3 Outward2dNormalWs )
			{
				float4 positionWs;
				float2 metallicSmoothness;
				mudbun_mesh_vert(VertexID, positionWs, PositionLs, NormalWs, NormalLs, Color, EmissionHash, metallicSmoothness, TextureWeight, SdfValue, Outward2dNormalLs, Outward2dNormalWs);
				Metallic = metallicSmoothness.x;
				Smoothness = metallicSmoothness.y;
				return positionWs.xyz;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				int VertexID4_g83 = v.ase_vertexID;
				float3 PositionLs4_g83 = float3( 0,0,0 );
				float3 NormalWs4_g83 = float3( 0,0,0 );
				float3 NormalLs4_g83 = float3( 0,0,0 );
				float4 Color4_g83 = float4( 0,0,0,0 );
				float4 EmissionHash4_g83 = float4( 0,0,0,0 );
				float Metallic4_g83 = 0.0;
				float Smoothness4_g83 = 0.0;
				float4 TextureWeight4_g83 = float4( 1,0,0,0 );
				float SdfValue4_g83 = 0.0;
				float3 Outward2dNormalLs4_g83 = float3( 0,0,0 );
				float3 Outward2dNormalWs4_g83 = float3( 0,0,0 );
				float3 localMudBunMeshPoint4_g83 = MudBunMeshPoint( VertexID4_g83 , PositionLs4_g83 , NormalWs4_g83 , NormalLs4_g83 , Color4_g83 , EmissionHash4_g83 , Metallic4_g83 , Smoothness4_g83 , TextureWeight4_g83 , SdfValue4_g83 , Outward2dNormalLs4_g83 , Outward2dNormalWs4_g83 );
				float3 temp_output_198_0 = localMudBunMeshPoint4_g83;
				
				float3 vertexToFrag213 = NormalLs4_g83;
				o.ase_texcoord7.xyz = vertexToFrag213;
				float3 vertexToFrag212 = PositionLs4_g83;
				o.ase_texcoord8.xyz = vertexToFrag212;
				float4 vertexToFrag224 = TextureWeight4_g83;
				o.ase_texcoord9 = vertexToFrag224;
				float4 vertexToFrag5_g83 = Color4_g83;
				o.ase_texcoord10 = vertexToFrag5_g83;
				
				float3 vertexToFrag6_g83 = (EmissionHash4_g83).xyz;
				o.ase_texcoord11.xyz = vertexToFrag6_g83;
				
				float vertexToFrag8_g83 = Metallic4_g83;
				o.ase_texcoord7.w = vertexToFrag8_g83;
				
				float vertexToFrag7_g83 = Smoothness4_g83;
				o.ase_texcoord8.w = vertexToFrag7_g83;
				
				float3 vertexToFrag16_g83 = localMudBunMeshPoint4_g83;
				o.ase_texcoord12.xyz = vertexToFrag16_g83;
				float3 vertexToFrag27_g225 = temp_output_198_0;
				o.ase_texcoord13.xyz = vertexToFrag27_g225;
				
				o.ase_texcoord11.w = v.ase_vertexID;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord12.w = 0;
				o.ase_texcoord13.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = temp_output_198_0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = NormalWs4_g83;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				uint ase_vertexID : SV_VertexID;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord1 = v.texcoord1;
				o.vertex = v.vertex;
				o.ase_vertexID = v.ase_vertexID;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_vertexID = patch[0].ase_vertexID * bary.x + patch[1].ase_vertexID * bary.y + patch[2].ase_vertexID * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				float3 WorldNormal = normalize( IN.tSpace0.xyz );
				float3 WorldTangent = IN.tSpace1.xyz;
				float3 WorldBiTangent = IN.tSpace2.xyz;
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				#if SHADER_HINT_NICE_QUALITY
					WorldViewDirection = SafeNormalize( WorldViewDirection );
				#endif

				float3 vertexToFrag213 = IN.ase_texcoord7.xyz;
				float3 weight54_g215 = abs( vertexToFrag213 );
				float3 appendResult83_g215 = (float3((float)(int)_MainTexX , (float)(int)_MainTexY , (float)(int)_MainTexZ));
				float3 weight0119_g215 = ( weight54_g215 * appendResult83_g215 );
				float3 temp_output_11_0_g223 = weight0119_g215;
				float3 break14_g223 = temp_output_11_0_g223;
				float3 vertexToFrag212 = IN.ase_texcoord8.xyz;
				float3 temp_output_36_0_g215 = vertexToFrag212;
				float2 uvX56_g215 = (temp_output_36_0_g215).yz;
				float4 appendResult23_g223 = (float4(temp_output_11_0_g223 , 0.0));
				float4 appendResult24_g223 = (float4(temp_output_11_0_g223 , 1.0));
				float4 break10_g224 = ( ( break14_g223.x + break14_g223.y + break14_g223.z ) > 0.0 ? appendResult23_g223 : appendResult24_g223 );
				float2 uvY57_g215 = (temp_output_36_0_g215).zx;
				float2 uvZ58_g215 = (temp_output_36_0_g215).xy;
				float4 color20_g223 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 vertexToFrag224 = IN.ase_texcoord9;
				float4 break10_g216 = vertexToFrag224;
				float3 appendResult123_g215 = (float3((float)(int)_Tex1X , (float)(int)_Tex1Y , (float)(int)_Tex1Z));
				float3 weight1126_g215 = ( weight54_g215 * appendResult123_g215 );
				float3 temp_output_11_0_g217 = weight1126_g215;
				float3 break14_g217 = temp_output_11_0_g217;
				float4 appendResult23_g217 = (float4(temp_output_11_0_g217 , 0.0));
				float4 appendResult24_g217 = (float4(temp_output_11_0_g217 , 1.0));
				float4 break10_g218 = ( ( break14_g217.x + break14_g217.y + break14_g217.z ) > 0.0 ? appendResult23_g217 : appendResult24_g217 );
				float4 color20_g217 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float3 appendResult130_g215 = (float3((float)(int)_Tex2X , (float)(int)_Tex2Y , (float)(int)_Tex2Z));
				float3 weight2133_g215 = ( weight54_g215 * appendResult130_g215 );
				float3 temp_output_11_0_g219 = weight2133_g215;
				float3 break14_g219 = temp_output_11_0_g219;
				float4 appendResult23_g219 = (float4(temp_output_11_0_g219 , 0.0));
				float4 appendResult24_g219 = (float4(temp_output_11_0_g219 , 1.0));
				float4 break10_g220 = ( ( break14_g219.x + break14_g219.y + break14_g219.z ) > 0.0 ? appendResult23_g219 : appendResult24_g219 );
				float4 color20_g219 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float3 appendResult137_g215 = (float3((float)(int)_Tex3X , (float)(int)_Tex3Y , (float)(int)_Tex3Z));
				float3 weight3140_g215 = ( weight54_g215 * appendResult137_g215 );
				float3 temp_output_11_0_g221 = weight3140_g215;
				float3 break14_g221 = temp_output_11_0_g221;
				float4 appendResult23_g221 = (float4(temp_output_11_0_g221 , 0.0));
				float4 appendResult24_g221 = (float4(temp_output_11_0_g221 , 1.0));
				float4 break10_g222 = ( ( break14_g221.x + break14_g221.y + break14_g221.z ) > 0.0 ? appendResult23_g221 : appendResult24_g221 );
				float4 color20_g221 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 color182 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
				float4 vertexToFrag5_g83 = IN.ase_texcoord10;
				float4 temp_output_25_0_g83 = ( _Color * vertexToFrag5_g83 );
				float4 temp_output_175_0 = ( ( ( _UseTex0 + _UseTex1 + _UseTex2 + _UseTex3 ) > 0.0 ? ( ( ( ( (float)(int)_UseTex0 > 0.0 ? ( ( ( ( break14_g223.x > 0.0 ? tex2D( _MainTex, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.x ) + ( ( break14_g223.y > 0.0 ? tex2D( _MainTex, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.y ) + ( ( break14_g223.z > 0.0 ? tex2D( _MainTex, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.z ) + ( color20_g223 * break10_g224.w ) ) / ( break10_g224.x + break10_g224.y + break10_g224.z + break10_g224.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.x ) + ( ( (float)(int)_UseTex1 > 0.0 ? ( ( ( ( break14_g217.x > 0.0 ? tex2D( _Tex1, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.x ) + ( ( break14_g217.y > 0.0 ? tex2D( _Tex1, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.y ) + ( ( break14_g217.z > 0.0 ? tex2D( _Tex1, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.z ) + ( color20_g217 * break10_g218.w ) ) / ( break10_g218.x + break10_g218.y + break10_g218.z + break10_g218.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.y ) + ( ( (float)(int)_UseTex2 > 0.0 ? ( ( ( ( break14_g219.x > 0.0 ? tex2D( _Tex2, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.x ) + ( ( break14_g219.y > 0.0 ? tex2D( _Tex2, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.y ) + ( ( break14_g219.z > 0.0 ? tex2D( _Tex2, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.z ) + ( color20_g219 * break10_g220.w ) ) / ( break10_g220.x + break10_g220.y + break10_g220.z + break10_g220.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.z ) + ( ( (float)(int)_UseTex3 > 0.0 ? ( ( ( ( break14_g221.x > 0.0 ? tex2D( _Tex3, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.x ) + ( ( break14_g221.y > 0.0 ? tex2D( _Tex3, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.y ) + ( ( break14_g221.z > 0.0 ? tex2D( _Tex3, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.z ) + ( color20_g221 * break10_g222.w ) ) / ( break10_g222.x + break10_g222.y + break10_g222.z + break10_g222.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.w ) ) / ( break10_g216.x + break10_g216.y + break10_g216.z + break10_g216.w ) ) : color182 ) * temp_output_25_0_g83 );
				
				float3 vertexToFrag6_g83 = IN.ase_texcoord11.xyz;
				
				float vertexToFrag8_g83 = IN.ase_texcoord7.w;
				
				float vertexToFrag7_g83 = IN.ase_texcoord8.w;
				
				float localComputeOpaqueTransparency20_g225 = ( 0.0 );
				float3 vertexToFrag16_g83 = IN.ase_texcoord12.xyz;
				float4 unityObjectToClipPos17_g83 = TransformWorldToHClip(TransformObjectToWorld(vertexToFrag16_g83));
				float4 computeScreenPos18_g83 = ComputeScreenPos( unityObjectToClipPos17_g83 );
				float2 ScreenPos20_g225 = (( ( computeScreenPos18_g83 / (computeScreenPos18_g83).w ) * _ScreenParams )).xy;
				float3 vertexToFrag27_g225 = IN.ase_texcoord13.xyz;
				float3 VertPos20_g225 = vertexToFrag27_g225;
				int VertexID4_g83 = IN.ase_texcoord11.w;
				float3 PositionLs4_g83 = float3( 0,0,0 );
				float3 NormalWs4_g83 = float3( 0,0,0 );
				float3 NormalLs4_g83 = float3( 0,0,0 );
				float4 Color4_g83 = float4( 0,0,0,0 );
				float4 EmissionHash4_g83 = float4( 0,0,0,0 );
				float Metallic4_g83 = 0.0;
				float Smoothness4_g83 = 0.0;
				float4 TextureWeight4_g83 = float4( 1,0,0,0 );
				float SdfValue4_g83 = 0.0;
				float3 Outward2dNormalLs4_g83 = float3( 0,0,0 );
				float3 Outward2dNormalWs4_g83 = float3( 0,0,0 );
				float3 localMudBunMeshPoint4_g83 = MudBunMeshPoint( VertexID4_g83 , PositionLs4_g83 , NormalWs4_g83 , NormalLs4_g83 , Color4_g83 , EmissionHash4_g83 , Metallic4_g83 , Smoothness4_g83 , TextureWeight4_g83 , SdfValue4_g83 , Outward2dNormalLs4_g83 , Outward2dNormalWs4_g83 );
				float Hash20_g225 = (EmissionHash4_g83).w;
				float AlphaIn20_g225 = (temp_output_25_0_g83).a;
				float AlphaOut20_g225 = 0;
				float AlphaThreshold20_g225 = 0;
				sampler2D DitherNoiseTexture20_g225 = _DitherTexture;
				int DitherNoiseTextureSize20_g225 = _DitherTextureSize;
				int UseRandomDither20_g225 = (int)_RandomDither;
				float AlphaCutoutThreshold20_g225 = _AlphaCutoutThreshold;
				float DitherBlend20_g225 = _Dithering;
				float alpha = AlphaIn20_g225;
				computeOpaqueTransparency(ScreenPos20_g225, VertPos20_g225, Hash20_g225, DitherNoiseTexture20_g225, DitherNoiseTextureSize20_g225, UseRandomDither20_g225 > 0, AlphaCutoutThreshold20_g225, DitherBlend20_g225,  alpha, AlphaThreshold20_g225);
				AlphaOut20_g225 = alpha;
				
				float3 Albedo = temp_output_175_0.xyz;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = ( vertexToFrag6_g83 * (_Emission).rgb );
				float3 Specular = 0.5;
				float Metallic = ( _Metallic * vertexToFrag8_g83 );
				float Smoothness = ( _Smoothness * vertexToFrag7_g83 );
				float Occlusion = 1;
				float Alpha = ( (temp_output_175_0).w * AlphaOut20_g225 );
				float AlphaClipThreshold = AlphaThreshold20_g225;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					inputData.normalWS = normalize(TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal )));
				#else
					#if !SHADER_HINT_NICE_QUALITY
						inputData.normalWS = WorldNormal;
					#else
						inputData.normalWS = normalize( WorldNormal );
					#endif
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, IN.lightmapUVOrVertexSH.xyz, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif

				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, WorldNormal ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define SHADER_GRAPH
			#pragma multi_compile _ MUDBUN_PROCEDURAL
			#include "Assets/MudBun/Shader/Render/ShaderCommon.cginc"
			#include "Assets/MudBun/Shader/Render/MeshCommon.cginc"


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				uint ase_vertexID : SV_VertexID;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _UseTex0;
			float _RandomDither;
			int _DitherTextureSize;
			float _Tex3Z;
			float _Tex3Y;
			float _Tex3X;
			float _Tex2Z;
			float _Tex2Y;
			float _Tex2X;
			float _Tex1Z;
			float _Tex1Y;
			float _Tex1X;
			float _MainTexZ;
			float _MainTexY;
			float _MainTexX;
			float _UseTex3;
			float _UseTex2;
			float _UseTex1;
			float _AlphaCutoutThreshold;
			float _Dithering;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _MainTex;
			sampler2D _Tex1;
			sampler2D _Tex2;
			sampler2D _Tex3;
			sampler2D _DitherTexture;


			float3 MudBunMeshPoint( int VertexID , out float3 PositionLs , out float3 NormalWs , out float3 NormalLs , out float4 Color , out float4 EmissionHash , out float Metallic , out float Smoothness , out float4 TextureWeight , out float SdfValue , out float3 Outward2dNormalLs , out float3 Outward2dNormalWs )
			{
				float4 positionWs;
				float2 metallicSmoothness;
				mudbun_mesh_vert(VertexID, positionWs, PositionLs, NormalWs, NormalLs, Color, EmissionHash, metallicSmoothness, TextureWeight, SdfValue, Outward2dNormalLs, Outward2dNormalWs);
				Metallic = metallicSmoothness.x;
				Smoothness = metallicSmoothness.y;
				return positionWs.xyz;
			}
			

			float3 _LightDirection;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				int VertexID4_g83 = v.ase_vertexID;
				float3 PositionLs4_g83 = float3( 0,0,0 );
				float3 NormalWs4_g83 = float3( 0,0,0 );
				float3 NormalLs4_g83 = float3( 0,0,0 );
				float4 Color4_g83 = float4( 0,0,0,0 );
				float4 EmissionHash4_g83 = float4( 0,0,0,0 );
				float Metallic4_g83 = 0.0;
				float Smoothness4_g83 = 0.0;
				float4 TextureWeight4_g83 = float4( 1,0,0,0 );
				float SdfValue4_g83 = 0.0;
				float3 Outward2dNormalLs4_g83 = float3( 0,0,0 );
				float3 Outward2dNormalWs4_g83 = float3( 0,0,0 );
				float3 localMudBunMeshPoint4_g83 = MudBunMeshPoint( VertexID4_g83 , PositionLs4_g83 , NormalWs4_g83 , NormalLs4_g83 , Color4_g83 , EmissionHash4_g83 , Metallic4_g83 , Smoothness4_g83 , TextureWeight4_g83 , SdfValue4_g83 , Outward2dNormalLs4_g83 , Outward2dNormalWs4_g83 );
				float3 temp_output_198_0 = localMudBunMeshPoint4_g83;
				
				float3 vertexToFrag213 = NormalLs4_g83;
				o.ase_texcoord2.xyz = vertexToFrag213;
				float3 vertexToFrag212 = PositionLs4_g83;
				o.ase_texcoord3.xyz = vertexToFrag212;
				float4 vertexToFrag224 = TextureWeight4_g83;
				o.ase_texcoord4 = vertexToFrag224;
				float4 vertexToFrag5_g83 = Color4_g83;
				o.ase_texcoord5 = vertexToFrag5_g83;
				float3 vertexToFrag16_g83 = localMudBunMeshPoint4_g83;
				o.ase_texcoord6.xyz = vertexToFrag16_g83;
				float3 vertexToFrag27_g225 = temp_output_198_0;
				o.ase_texcoord7.xyz = vertexToFrag27_g225;
				
				o.ase_texcoord2.w = v.ase_vertexID;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.w = 0;
				o.ase_texcoord6.w = 0;
				o.ase_texcoord7.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = temp_output_198_0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = NormalWs4_g83;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				uint ase_vertexID : SV_VertexID;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.vertex = v.vertex;
				o.ase_vertexID = v.ase_vertexID;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_vertexID = patch[0].ase_vertexID * bary.x + patch[1].ase_vertexID * bary.y + patch[2].ase_vertexID * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 vertexToFrag213 = IN.ase_texcoord2.xyz;
				float3 weight54_g215 = abs( vertexToFrag213 );
				float3 appendResult83_g215 = (float3((float)(int)_MainTexX , (float)(int)_MainTexY , (float)(int)_MainTexZ));
				float3 weight0119_g215 = ( weight54_g215 * appendResult83_g215 );
				float3 temp_output_11_0_g223 = weight0119_g215;
				float3 break14_g223 = temp_output_11_0_g223;
				float3 vertexToFrag212 = IN.ase_texcoord3.xyz;
				float3 temp_output_36_0_g215 = vertexToFrag212;
				float2 uvX56_g215 = (temp_output_36_0_g215).yz;
				float4 appendResult23_g223 = (float4(temp_output_11_0_g223 , 0.0));
				float4 appendResult24_g223 = (float4(temp_output_11_0_g223 , 1.0));
				float4 break10_g224 = ( ( break14_g223.x + break14_g223.y + break14_g223.z ) > 0.0 ? appendResult23_g223 : appendResult24_g223 );
				float2 uvY57_g215 = (temp_output_36_0_g215).zx;
				float2 uvZ58_g215 = (temp_output_36_0_g215).xy;
				float4 color20_g223 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 vertexToFrag224 = IN.ase_texcoord4;
				float4 break10_g216 = vertexToFrag224;
				float3 appendResult123_g215 = (float3((float)(int)_Tex1X , (float)(int)_Tex1Y , (float)(int)_Tex1Z));
				float3 weight1126_g215 = ( weight54_g215 * appendResult123_g215 );
				float3 temp_output_11_0_g217 = weight1126_g215;
				float3 break14_g217 = temp_output_11_0_g217;
				float4 appendResult23_g217 = (float4(temp_output_11_0_g217 , 0.0));
				float4 appendResult24_g217 = (float4(temp_output_11_0_g217 , 1.0));
				float4 break10_g218 = ( ( break14_g217.x + break14_g217.y + break14_g217.z ) > 0.0 ? appendResult23_g217 : appendResult24_g217 );
				float4 color20_g217 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float3 appendResult130_g215 = (float3((float)(int)_Tex2X , (float)(int)_Tex2Y , (float)(int)_Tex2Z));
				float3 weight2133_g215 = ( weight54_g215 * appendResult130_g215 );
				float3 temp_output_11_0_g219 = weight2133_g215;
				float3 break14_g219 = temp_output_11_0_g219;
				float4 appendResult23_g219 = (float4(temp_output_11_0_g219 , 0.0));
				float4 appendResult24_g219 = (float4(temp_output_11_0_g219 , 1.0));
				float4 break10_g220 = ( ( break14_g219.x + break14_g219.y + break14_g219.z ) > 0.0 ? appendResult23_g219 : appendResult24_g219 );
				float4 color20_g219 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float3 appendResult137_g215 = (float3((float)(int)_Tex3X , (float)(int)_Tex3Y , (float)(int)_Tex3Z));
				float3 weight3140_g215 = ( weight54_g215 * appendResult137_g215 );
				float3 temp_output_11_0_g221 = weight3140_g215;
				float3 break14_g221 = temp_output_11_0_g221;
				float4 appendResult23_g221 = (float4(temp_output_11_0_g221 , 0.0));
				float4 appendResult24_g221 = (float4(temp_output_11_0_g221 , 1.0));
				float4 break10_g222 = ( ( break14_g221.x + break14_g221.y + break14_g221.z ) > 0.0 ? appendResult23_g221 : appendResult24_g221 );
				float4 color20_g221 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 color182 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
				float4 vertexToFrag5_g83 = IN.ase_texcoord5;
				float4 temp_output_25_0_g83 = ( _Color * vertexToFrag5_g83 );
				float4 temp_output_175_0 = ( ( ( _UseTex0 + _UseTex1 + _UseTex2 + _UseTex3 ) > 0.0 ? ( ( ( ( (float)(int)_UseTex0 > 0.0 ? ( ( ( ( break14_g223.x > 0.0 ? tex2D( _MainTex, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.x ) + ( ( break14_g223.y > 0.0 ? tex2D( _MainTex, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.y ) + ( ( break14_g223.z > 0.0 ? tex2D( _MainTex, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.z ) + ( color20_g223 * break10_g224.w ) ) / ( break10_g224.x + break10_g224.y + break10_g224.z + break10_g224.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.x ) + ( ( (float)(int)_UseTex1 > 0.0 ? ( ( ( ( break14_g217.x > 0.0 ? tex2D( _Tex1, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.x ) + ( ( break14_g217.y > 0.0 ? tex2D( _Tex1, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.y ) + ( ( break14_g217.z > 0.0 ? tex2D( _Tex1, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.z ) + ( color20_g217 * break10_g218.w ) ) / ( break10_g218.x + break10_g218.y + break10_g218.z + break10_g218.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.y ) + ( ( (float)(int)_UseTex2 > 0.0 ? ( ( ( ( break14_g219.x > 0.0 ? tex2D( _Tex2, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.x ) + ( ( break14_g219.y > 0.0 ? tex2D( _Tex2, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.y ) + ( ( break14_g219.z > 0.0 ? tex2D( _Tex2, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.z ) + ( color20_g219 * break10_g220.w ) ) / ( break10_g220.x + break10_g220.y + break10_g220.z + break10_g220.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.z ) + ( ( (float)(int)_UseTex3 > 0.0 ? ( ( ( ( break14_g221.x > 0.0 ? tex2D( _Tex3, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.x ) + ( ( break14_g221.y > 0.0 ? tex2D( _Tex3, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.y ) + ( ( break14_g221.z > 0.0 ? tex2D( _Tex3, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.z ) + ( color20_g221 * break10_g222.w ) ) / ( break10_g222.x + break10_g222.y + break10_g222.z + break10_g222.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.w ) ) / ( break10_g216.x + break10_g216.y + break10_g216.z + break10_g216.w ) ) : color182 ) * temp_output_25_0_g83 );
				float localComputeOpaqueTransparency20_g225 = ( 0.0 );
				float3 vertexToFrag16_g83 = IN.ase_texcoord6.xyz;
				float4 unityObjectToClipPos17_g83 = TransformWorldToHClip(TransformObjectToWorld(vertexToFrag16_g83));
				float4 computeScreenPos18_g83 = ComputeScreenPos( unityObjectToClipPos17_g83 );
				float2 ScreenPos20_g225 = (( ( computeScreenPos18_g83 / (computeScreenPos18_g83).w ) * _ScreenParams )).xy;
				float3 vertexToFrag27_g225 = IN.ase_texcoord7.xyz;
				float3 VertPos20_g225 = vertexToFrag27_g225;
				int VertexID4_g83 = IN.ase_texcoord2.w;
				float3 PositionLs4_g83 = float3( 0,0,0 );
				float3 NormalWs4_g83 = float3( 0,0,0 );
				float3 NormalLs4_g83 = float3( 0,0,0 );
				float4 Color4_g83 = float4( 0,0,0,0 );
				float4 EmissionHash4_g83 = float4( 0,0,0,0 );
				float Metallic4_g83 = 0.0;
				float Smoothness4_g83 = 0.0;
				float4 TextureWeight4_g83 = float4( 1,0,0,0 );
				float SdfValue4_g83 = 0.0;
				float3 Outward2dNormalLs4_g83 = float3( 0,0,0 );
				float3 Outward2dNormalWs4_g83 = float3( 0,0,0 );
				float3 localMudBunMeshPoint4_g83 = MudBunMeshPoint( VertexID4_g83 , PositionLs4_g83 , NormalWs4_g83 , NormalLs4_g83 , Color4_g83 , EmissionHash4_g83 , Metallic4_g83 , Smoothness4_g83 , TextureWeight4_g83 , SdfValue4_g83 , Outward2dNormalLs4_g83 , Outward2dNormalWs4_g83 );
				float Hash20_g225 = (EmissionHash4_g83).w;
				float AlphaIn20_g225 = (temp_output_25_0_g83).a;
				float AlphaOut20_g225 = 0;
				float AlphaThreshold20_g225 = 0;
				sampler2D DitherNoiseTexture20_g225 = _DitherTexture;
				int DitherNoiseTextureSize20_g225 = _DitherTextureSize;
				int UseRandomDither20_g225 = (int)_RandomDither;
				float AlphaCutoutThreshold20_g225 = _AlphaCutoutThreshold;
				float DitherBlend20_g225 = _Dithering;
				float alpha = AlphaIn20_g225;
				computeOpaqueTransparency(ScreenPos20_g225, VertPos20_g225, Hash20_g225, DitherNoiseTexture20_g225, DitherNoiseTextureSize20_g225, UseRandomDither20_g225 > 0, AlphaCutoutThreshold20_g225, DitherBlend20_g225,  alpha, AlphaThreshold20_g225);
				AlphaOut20_g225 = alpha;
				
				float Alpha = ( (temp_output_175_0).w * AlphaOut20_g225 );
				float AlphaClipThreshold = AlphaThreshold20_g225;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define SHADER_GRAPH
			#pragma multi_compile _ MUDBUN_PROCEDURAL
			#include "Assets/MudBun/Shader/Render/ShaderCommon.cginc"
			#include "Assets/MudBun/Shader/Render/MeshCommon.cginc"


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				uint ase_vertexID : SV_VertexID;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _UseTex0;
			float _RandomDither;
			int _DitherTextureSize;
			float _Tex3Z;
			float _Tex3Y;
			float _Tex3X;
			float _Tex2Z;
			float _Tex2Y;
			float _Tex2X;
			float _Tex1Z;
			float _Tex1Y;
			float _Tex1X;
			float _MainTexZ;
			float _MainTexY;
			float _MainTexX;
			float _UseTex3;
			float _UseTex2;
			float _UseTex1;
			float _AlphaCutoutThreshold;
			float _Dithering;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _MainTex;
			sampler2D _Tex1;
			sampler2D _Tex2;
			sampler2D _Tex3;
			sampler2D _DitherTexture;


			float3 MudBunMeshPoint( int VertexID , out float3 PositionLs , out float3 NormalWs , out float3 NormalLs , out float4 Color , out float4 EmissionHash , out float Metallic , out float Smoothness , out float4 TextureWeight , out float SdfValue , out float3 Outward2dNormalLs , out float3 Outward2dNormalWs )
			{
				float4 positionWs;
				float2 metallicSmoothness;
				mudbun_mesh_vert(VertexID, positionWs, PositionLs, NormalWs, NormalLs, Color, EmissionHash, metallicSmoothness, TextureWeight, SdfValue, Outward2dNormalLs, Outward2dNormalWs);
				Metallic = metallicSmoothness.x;
				Smoothness = metallicSmoothness.y;
				return positionWs.xyz;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				int VertexID4_g83 = v.ase_vertexID;
				float3 PositionLs4_g83 = float3( 0,0,0 );
				float3 NormalWs4_g83 = float3( 0,0,0 );
				float3 NormalLs4_g83 = float3( 0,0,0 );
				float4 Color4_g83 = float4( 0,0,0,0 );
				float4 EmissionHash4_g83 = float4( 0,0,0,0 );
				float Metallic4_g83 = 0.0;
				float Smoothness4_g83 = 0.0;
				float4 TextureWeight4_g83 = float4( 1,0,0,0 );
				float SdfValue4_g83 = 0.0;
				float3 Outward2dNormalLs4_g83 = float3( 0,0,0 );
				float3 Outward2dNormalWs4_g83 = float3( 0,0,0 );
				float3 localMudBunMeshPoint4_g83 = MudBunMeshPoint( VertexID4_g83 , PositionLs4_g83 , NormalWs4_g83 , NormalLs4_g83 , Color4_g83 , EmissionHash4_g83 , Metallic4_g83 , Smoothness4_g83 , TextureWeight4_g83 , SdfValue4_g83 , Outward2dNormalLs4_g83 , Outward2dNormalWs4_g83 );
				float3 temp_output_198_0 = localMudBunMeshPoint4_g83;
				
				float3 vertexToFrag213 = NormalLs4_g83;
				o.ase_texcoord2.xyz = vertexToFrag213;
				float3 vertexToFrag212 = PositionLs4_g83;
				o.ase_texcoord3.xyz = vertexToFrag212;
				float4 vertexToFrag224 = TextureWeight4_g83;
				o.ase_texcoord4 = vertexToFrag224;
				float4 vertexToFrag5_g83 = Color4_g83;
				o.ase_texcoord5 = vertexToFrag5_g83;
				float3 vertexToFrag16_g83 = localMudBunMeshPoint4_g83;
				o.ase_texcoord6.xyz = vertexToFrag16_g83;
				float3 vertexToFrag27_g225 = temp_output_198_0;
				o.ase_texcoord7.xyz = vertexToFrag27_g225;
				
				o.ase_texcoord2.w = v.ase_vertexID;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.w = 0;
				o.ase_texcoord6.w = 0;
				o.ase_texcoord7.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = temp_output_198_0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = NormalWs4_g83;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				uint ase_vertexID : SV_VertexID;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.vertex = v.vertex;
				o.ase_vertexID = v.ase_vertexID;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_vertexID = patch[0].ase_vertexID * bary.x + patch[1].ase_vertexID * bary.y + patch[2].ase_vertexID * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 vertexToFrag213 = IN.ase_texcoord2.xyz;
				float3 weight54_g215 = abs( vertexToFrag213 );
				float3 appendResult83_g215 = (float3((float)(int)_MainTexX , (float)(int)_MainTexY , (float)(int)_MainTexZ));
				float3 weight0119_g215 = ( weight54_g215 * appendResult83_g215 );
				float3 temp_output_11_0_g223 = weight0119_g215;
				float3 break14_g223 = temp_output_11_0_g223;
				float3 vertexToFrag212 = IN.ase_texcoord3.xyz;
				float3 temp_output_36_0_g215 = vertexToFrag212;
				float2 uvX56_g215 = (temp_output_36_0_g215).yz;
				float4 appendResult23_g223 = (float4(temp_output_11_0_g223 , 0.0));
				float4 appendResult24_g223 = (float4(temp_output_11_0_g223 , 1.0));
				float4 break10_g224 = ( ( break14_g223.x + break14_g223.y + break14_g223.z ) > 0.0 ? appendResult23_g223 : appendResult24_g223 );
				float2 uvY57_g215 = (temp_output_36_0_g215).zx;
				float2 uvZ58_g215 = (temp_output_36_0_g215).xy;
				float4 color20_g223 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 vertexToFrag224 = IN.ase_texcoord4;
				float4 break10_g216 = vertexToFrag224;
				float3 appendResult123_g215 = (float3((float)(int)_Tex1X , (float)(int)_Tex1Y , (float)(int)_Tex1Z));
				float3 weight1126_g215 = ( weight54_g215 * appendResult123_g215 );
				float3 temp_output_11_0_g217 = weight1126_g215;
				float3 break14_g217 = temp_output_11_0_g217;
				float4 appendResult23_g217 = (float4(temp_output_11_0_g217 , 0.0));
				float4 appendResult24_g217 = (float4(temp_output_11_0_g217 , 1.0));
				float4 break10_g218 = ( ( break14_g217.x + break14_g217.y + break14_g217.z ) > 0.0 ? appendResult23_g217 : appendResult24_g217 );
				float4 color20_g217 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float3 appendResult130_g215 = (float3((float)(int)_Tex2X , (float)(int)_Tex2Y , (float)(int)_Tex2Z));
				float3 weight2133_g215 = ( weight54_g215 * appendResult130_g215 );
				float3 temp_output_11_0_g219 = weight2133_g215;
				float3 break14_g219 = temp_output_11_0_g219;
				float4 appendResult23_g219 = (float4(temp_output_11_0_g219 , 0.0));
				float4 appendResult24_g219 = (float4(temp_output_11_0_g219 , 1.0));
				float4 break10_g220 = ( ( break14_g219.x + break14_g219.y + break14_g219.z ) > 0.0 ? appendResult23_g219 : appendResult24_g219 );
				float4 color20_g219 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float3 appendResult137_g215 = (float3((float)(int)_Tex3X , (float)(int)_Tex3Y , (float)(int)_Tex3Z));
				float3 weight3140_g215 = ( weight54_g215 * appendResult137_g215 );
				float3 temp_output_11_0_g221 = weight3140_g215;
				float3 break14_g221 = temp_output_11_0_g221;
				float4 appendResult23_g221 = (float4(temp_output_11_0_g221 , 0.0));
				float4 appendResult24_g221 = (float4(temp_output_11_0_g221 , 1.0));
				float4 break10_g222 = ( ( break14_g221.x + break14_g221.y + break14_g221.z ) > 0.0 ? appendResult23_g221 : appendResult24_g221 );
				float4 color20_g221 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 color182 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
				float4 vertexToFrag5_g83 = IN.ase_texcoord5;
				float4 temp_output_25_0_g83 = ( _Color * vertexToFrag5_g83 );
				float4 temp_output_175_0 = ( ( ( _UseTex0 + _UseTex1 + _UseTex2 + _UseTex3 ) > 0.0 ? ( ( ( ( (float)(int)_UseTex0 > 0.0 ? ( ( ( ( break14_g223.x > 0.0 ? tex2D( _MainTex, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.x ) + ( ( break14_g223.y > 0.0 ? tex2D( _MainTex, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.y ) + ( ( break14_g223.z > 0.0 ? tex2D( _MainTex, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.z ) + ( color20_g223 * break10_g224.w ) ) / ( break10_g224.x + break10_g224.y + break10_g224.z + break10_g224.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.x ) + ( ( (float)(int)_UseTex1 > 0.0 ? ( ( ( ( break14_g217.x > 0.0 ? tex2D( _Tex1, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.x ) + ( ( break14_g217.y > 0.0 ? tex2D( _Tex1, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.y ) + ( ( break14_g217.z > 0.0 ? tex2D( _Tex1, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.z ) + ( color20_g217 * break10_g218.w ) ) / ( break10_g218.x + break10_g218.y + break10_g218.z + break10_g218.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.y ) + ( ( (float)(int)_UseTex2 > 0.0 ? ( ( ( ( break14_g219.x > 0.0 ? tex2D( _Tex2, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.x ) + ( ( break14_g219.y > 0.0 ? tex2D( _Tex2, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.y ) + ( ( break14_g219.z > 0.0 ? tex2D( _Tex2, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.z ) + ( color20_g219 * break10_g220.w ) ) / ( break10_g220.x + break10_g220.y + break10_g220.z + break10_g220.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.z ) + ( ( (float)(int)_UseTex3 > 0.0 ? ( ( ( ( break14_g221.x > 0.0 ? tex2D( _Tex3, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.x ) + ( ( break14_g221.y > 0.0 ? tex2D( _Tex3, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.y ) + ( ( break14_g221.z > 0.0 ? tex2D( _Tex3, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.z ) + ( color20_g221 * break10_g222.w ) ) / ( break10_g222.x + break10_g222.y + break10_g222.z + break10_g222.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.w ) ) / ( break10_g216.x + break10_g216.y + break10_g216.z + break10_g216.w ) ) : color182 ) * temp_output_25_0_g83 );
				float localComputeOpaqueTransparency20_g225 = ( 0.0 );
				float3 vertexToFrag16_g83 = IN.ase_texcoord6.xyz;
				float4 unityObjectToClipPos17_g83 = TransformWorldToHClip(TransformObjectToWorld(vertexToFrag16_g83));
				float4 computeScreenPos18_g83 = ComputeScreenPos( unityObjectToClipPos17_g83 );
				float2 ScreenPos20_g225 = (( ( computeScreenPos18_g83 / (computeScreenPos18_g83).w ) * _ScreenParams )).xy;
				float3 vertexToFrag27_g225 = IN.ase_texcoord7.xyz;
				float3 VertPos20_g225 = vertexToFrag27_g225;
				int VertexID4_g83 = IN.ase_texcoord2.w;
				float3 PositionLs4_g83 = float3( 0,0,0 );
				float3 NormalWs4_g83 = float3( 0,0,0 );
				float3 NormalLs4_g83 = float3( 0,0,0 );
				float4 Color4_g83 = float4( 0,0,0,0 );
				float4 EmissionHash4_g83 = float4( 0,0,0,0 );
				float Metallic4_g83 = 0.0;
				float Smoothness4_g83 = 0.0;
				float4 TextureWeight4_g83 = float4( 1,0,0,0 );
				float SdfValue4_g83 = 0.0;
				float3 Outward2dNormalLs4_g83 = float3( 0,0,0 );
				float3 Outward2dNormalWs4_g83 = float3( 0,0,0 );
				float3 localMudBunMeshPoint4_g83 = MudBunMeshPoint( VertexID4_g83 , PositionLs4_g83 , NormalWs4_g83 , NormalLs4_g83 , Color4_g83 , EmissionHash4_g83 , Metallic4_g83 , Smoothness4_g83 , TextureWeight4_g83 , SdfValue4_g83 , Outward2dNormalLs4_g83 , Outward2dNormalWs4_g83 );
				float Hash20_g225 = (EmissionHash4_g83).w;
				float AlphaIn20_g225 = (temp_output_25_0_g83).a;
				float AlphaOut20_g225 = 0;
				float AlphaThreshold20_g225 = 0;
				sampler2D DitherNoiseTexture20_g225 = _DitherTexture;
				int DitherNoiseTextureSize20_g225 = _DitherTextureSize;
				int UseRandomDither20_g225 = (int)_RandomDither;
				float AlphaCutoutThreshold20_g225 = _AlphaCutoutThreshold;
				float DitherBlend20_g225 = _Dithering;
				float alpha = AlphaIn20_g225;
				computeOpaqueTransparency(ScreenPos20_g225, VertPos20_g225, Hash20_g225, DitherNoiseTexture20_g225, DitherNoiseTextureSize20_g225, UseRandomDither20_g225 > 0, AlphaCutoutThreshold20_g225, DitherBlend20_g225,  alpha, AlphaThreshold20_g225);
				AlphaOut20_g225 = alpha;
				
				float Alpha = ( (temp_output_175_0).w * AlphaOut20_g225 );
				float AlphaClipThreshold = AlphaThreshold20_g225;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define SHADER_GRAPH
			#pragma multi_compile _ MUDBUN_PROCEDURAL
			#include "Assets/MudBun/Shader/Render/ShaderCommon.cginc"
			#include "Assets/MudBun/Shader/Render/MeshCommon.cginc"


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				uint ase_vertexID : SV_VertexID;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _UseTex0;
			float _RandomDither;
			int _DitherTextureSize;
			float _Tex3Z;
			float _Tex3Y;
			float _Tex3X;
			float _Tex2Z;
			float _Tex2Y;
			float _Tex2X;
			float _Tex1Z;
			float _Tex1Y;
			float _Tex1X;
			float _MainTexZ;
			float _MainTexY;
			float _MainTexX;
			float _UseTex3;
			float _UseTex2;
			float _UseTex1;
			float _AlphaCutoutThreshold;
			float _Dithering;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _MainTex;
			sampler2D _Tex1;
			sampler2D _Tex2;
			sampler2D _Tex3;
			sampler2D _DitherTexture;


			float3 MudBunMeshPoint( int VertexID , out float3 PositionLs , out float3 NormalWs , out float3 NormalLs , out float4 Color , out float4 EmissionHash , out float Metallic , out float Smoothness , out float4 TextureWeight , out float SdfValue , out float3 Outward2dNormalLs , out float3 Outward2dNormalWs )
			{
				float4 positionWs;
				float2 metallicSmoothness;
				mudbun_mesh_vert(VertexID, positionWs, PositionLs, NormalWs, NormalLs, Color, EmissionHash, metallicSmoothness, TextureWeight, SdfValue, Outward2dNormalLs, Outward2dNormalWs);
				Metallic = metallicSmoothness.x;
				Smoothness = metallicSmoothness.y;
				return positionWs.xyz;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				int VertexID4_g83 = v.ase_vertexID;
				float3 PositionLs4_g83 = float3( 0,0,0 );
				float3 NormalWs4_g83 = float3( 0,0,0 );
				float3 NormalLs4_g83 = float3( 0,0,0 );
				float4 Color4_g83 = float4( 0,0,0,0 );
				float4 EmissionHash4_g83 = float4( 0,0,0,0 );
				float Metallic4_g83 = 0.0;
				float Smoothness4_g83 = 0.0;
				float4 TextureWeight4_g83 = float4( 1,0,0,0 );
				float SdfValue4_g83 = 0.0;
				float3 Outward2dNormalLs4_g83 = float3( 0,0,0 );
				float3 Outward2dNormalWs4_g83 = float3( 0,0,0 );
				float3 localMudBunMeshPoint4_g83 = MudBunMeshPoint( VertexID4_g83 , PositionLs4_g83 , NormalWs4_g83 , NormalLs4_g83 , Color4_g83 , EmissionHash4_g83 , Metallic4_g83 , Smoothness4_g83 , TextureWeight4_g83 , SdfValue4_g83 , Outward2dNormalLs4_g83 , Outward2dNormalWs4_g83 );
				float3 temp_output_198_0 = localMudBunMeshPoint4_g83;
				
				float3 vertexToFrag213 = NormalLs4_g83;
				o.ase_texcoord2.xyz = vertexToFrag213;
				float3 vertexToFrag212 = PositionLs4_g83;
				o.ase_texcoord3.xyz = vertexToFrag212;
				float4 vertexToFrag224 = TextureWeight4_g83;
				o.ase_texcoord4 = vertexToFrag224;
				float4 vertexToFrag5_g83 = Color4_g83;
				o.ase_texcoord5 = vertexToFrag5_g83;
				
				float3 vertexToFrag6_g83 = (EmissionHash4_g83).xyz;
				o.ase_texcoord6.xyz = vertexToFrag6_g83;
				
				float3 vertexToFrag16_g83 = localMudBunMeshPoint4_g83;
				o.ase_texcoord7.xyz = vertexToFrag16_g83;
				float3 vertexToFrag27_g225 = temp_output_198_0;
				o.ase_texcoord8.xyz = vertexToFrag27_g225;
				
				o.ase_texcoord2.w = v.ase_vertexID;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.w = 0;
				o.ase_texcoord6.w = 0;
				o.ase_texcoord7.w = 0;
				o.ase_texcoord8.w = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = temp_output_198_0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = NormalWs4_g83;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				uint ase_vertexID : SV_VertexID;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				o.vertex = v.vertex;
				o.ase_vertexID = v.ase_vertexID;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_vertexID = patch[0].ase_vertexID * bary.x + patch[1].ase_vertexID * bary.y + patch[2].ase_vertexID * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 vertexToFrag213 = IN.ase_texcoord2.xyz;
				float3 weight54_g215 = abs( vertexToFrag213 );
				float3 appendResult83_g215 = (float3((float)(int)_MainTexX , (float)(int)_MainTexY , (float)(int)_MainTexZ));
				float3 weight0119_g215 = ( weight54_g215 * appendResult83_g215 );
				float3 temp_output_11_0_g223 = weight0119_g215;
				float3 break14_g223 = temp_output_11_0_g223;
				float3 vertexToFrag212 = IN.ase_texcoord3.xyz;
				float3 temp_output_36_0_g215 = vertexToFrag212;
				float2 uvX56_g215 = (temp_output_36_0_g215).yz;
				float4 appendResult23_g223 = (float4(temp_output_11_0_g223 , 0.0));
				float4 appendResult24_g223 = (float4(temp_output_11_0_g223 , 1.0));
				float4 break10_g224 = ( ( break14_g223.x + break14_g223.y + break14_g223.z ) > 0.0 ? appendResult23_g223 : appendResult24_g223 );
				float2 uvY57_g215 = (temp_output_36_0_g215).zx;
				float2 uvZ58_g215 = (temp_output_36_0_g215).xy;
				float4 color20_g223 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 vertexToFrag224 = IN.ase_texcoord4;
				float4 break10_g216 = vertexToFrag224;
				float3 appendResult123_g215 = (float3((float)(int)_Tex1X , (float)(int)_Tex1Y , (float)(int)_Tex1Z));
				float3 weight1126_g215 = ( weight54_g215 * appendResult123_g215 );
				float3 temp_output_11_0_g217 = weight1126_g215;
				float3 break14_g217 = temp_output_11_0_g217;
				float4 appendResult23_g217 = (float4(temp_output_11_0_g217 , 0.0));
				float4 appendResult24_g217 = (float4(temp_output_11_0_g217 , 1.0));
				float4 break10_g218 = ( ( break14_g217.x + break14_g217.y + break14_g217.z ) > 0.0 ? appendResult23_g217 : appendResult24_g217 );
				float4 color20_g217 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float3 appendResult130_g215 = (float3((float)(int)_Tex2X , (float)(int)_Tex2Y , (float)(int)_Tex2Z));
				float3 weight2133_g215 = ( weight54_g215 * appendResult130_g215 );
				float3 temp_output_11_0_g219 = weight2133_g215;
				float3 break14_g219 = temp_output_11_0_g219;
				float4 appendResult23_g219 = (float4(temp_output_11_0_g219 , 0.0));
				float4 appendResult24_g219 = (float4(temp_output_11_0_g219 , 1.0));
				float4 break10_g220 = ( ( break14_g219.x + break14_g219.y + break14_g219.z ) > 0.0 ? appendResult23_g219 : appendResult24_g219 );
				float4 color20_g219 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float3 appendResult137_g215 = (float3((float)(int)_Tex3X , (float)(int)_Tex3Y , (float)(int)_Tex3Z));
				float3 weight3140_g215 = ( weight54_g215 * appendResult137_g215 );
				float3 temp_output_11_0_g221 = weight3140_g215;
				float3 break14_g221 = temp_output_11_0_g221;
				float4 appendResult23_g221 = (float4(temp_output_11_0_g221 , 0.0));
				float4 appendResult24_g221 = (float4(temp_output_11_0_g221 , 1.0));
				float4 break10_g222 = ( ( break14_g221.x + break14_g221.y + break14_g221.z ) > 0.0 ? appendResult23_g221 : appendResult24_g221 );
				float4 color20_g221 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 color182 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
				float4 vertexToFrag5_g83 = IN.ase_texcoord5;
				float4 temp_output_25_0_g83 = ( _Color * vertexToFrag5_g83 );
				float4 temp_output_175_0 = ( ( ( _UseTex0 + _UseTex1 + _UseTex2 + _UseTex3 ) > 0.0 ? ( ( ( ( (float)(int)_UseTex0 > 0.0 ? ( ( ( ( break14_g223.x > 0.0 ? tex2D( _MainTex, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.x ) + ( ( break14_g223.y > 0.0 ? tex2D( _MainTex, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.y ) + ( ( break14_g223.z > 0.0 ? tex2D( _MainTex, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.z ) + ( color20_g223 * break10_g224.w ) ) / ( break10_g224.x + break10_g224.y + break10_g224.z + break10_g224.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.x ) + ( ( (float)(int)_UseTex1 > 0.0 ? ( ( ( ( break14_g217.x > 0.0 ? tex2D( _Tex1, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.x ) + ( ( break14_g217.y > 0.0 ? tex2D( _Tex1, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.y ) + ( ( break14_g217.z > 0.0 ? tex2D( _Tex1, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.z ) + ( color20_g217 * break10_g218.w ) ) / ( break10_g218.x + break10_g218.y + break10_g218.z + break10_g218.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.y ) + ( ( (float)(int)_UseTex2 > 0.0 ? ( ( ( ( break14_g219.x > 0.0 ? tex2D( _Tex2, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.x ) + ( ( break14_g219.y > 0.0 ? tex2D( _Tex2, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.y ) + ( ( break14_g219.z > 0.0 ? tex2D( _Tex2, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.z ) + ( color20_g219 * break10_g220.w ) ) / ( break10_g220.x + break10_g220.y + break10_g220.z + break10_g220.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.z ) + ( ( (float)(int)_UseTex3 > 0.0 ? ( ( ( ( break14_g221.x > 0.0 ? tex2D( _Tex3, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.x ) + ( ( break14_g221.y > 0.0 ? tex2D( _Tex3, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.y ) + ( ( break14_g221.z > 0.0 ? tex2D( _Tex3, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.z ) + ( color20_g221 * break10_g222.w ) ) / ( break10_g222.x + break10_g222.y + break10_g222.z + break10_g222.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.w ) ) / ( break10_g216.x + break10_g216.y + break10_g216.z + break10_g216.w ) ) : color182 ) * temp_output_25_0_g83 );
				
				float3 vertexToFrag6_g83 = IN.ase_texcoord6.xyz;
				
				float localComputeOpaqueTransparency20_g225 = ( 0.0 );
				float3 vertexToFrag16_g83 = IN.ase_texcoord7.xyz;
				float4 unityObjectToClipPos17_g83 = TransformWorldToHClip(TransformObjectToWorld(vertexToFrag16_g83));
				float4 computeScreenPos18_g83 = ComputeScreenPos( unityObjectToClipPos17_g83 );
				float2 ScreenPos20_g225 = (( ( computeScreenPos18_g83 / (computeScreenPos18_g83).w ) * _ScreenParams )).xy;
				float3 vertexToFrag27_g225 = IN.ase_texcoord8.xyz;
				float3 VertPos20_g225 = vertexToFrag27_g225;
				int VertexID4_g83 = IN.ase_texcoord2.w;
				float3 PositionLs4_g83 = float3( 0,0,0 );
				float3 NormalWs4_g83 = float3( 0,0,0 );
				float3 NormalLs4_g83 = float3( 0,0,0 );
				float4 Color4_g83 = float4( 0,0,0,0 );
				float4 EmissionHash4_g83 = float4( 0,0,0,0 );
				float Metallic4_g83 = 0.0;
				float Smoothness4_g83 = 0.0;
				float4 TextureWeight4_g83 = float4( 1,0,0,0 );
				float SdfValue4_g83 = 0.0;
				float3 Outward2dNormalLs4_g83 = float3( 0,0,0 );
				float3 Outward2dNormalWs4_g83 = float3( 0,0,0 );
				float3 localMudBunMeshPoint4_g83 = MudBunMeshPoint( VertexID4_g83 , PositionLs4_g83 , NormalWs4_g83 , NormalLs4_g83 , Color4_g83 , EmissionHash4_g83 , Metallic4_g83 , Smoothness4_g83 , TextureWeight4_g83 , SdfValue4_g83 , Outward2dNormalLs4_g83 , Outward2dNormalWs4_g83 );
				float Hash20_g225 = (EmissionHash4_g83).w;
				float AlphaIn20_g225 = (temp_output_25_0_g83).a;
				float AlphaOut20_g225 = 0;
				float AlphaThreshold20_g225 = 0;
				sampler2D DitherNoiseTexture20_g225 = _DitherTexture;
				int DitherNoiseTextureSize20_g225 = _DitherTextureSize;
				int UseRandomDither20_g225 = (int)_RandomDither;
				float AlphaCutoutThreshold20_g225 = _AlphaCutoutThreshold;
				float DitherBlend20_g225 = _Dithering;
				float alpha = AlphaIn20_g225;
				computeOpaqueTransparency(ScreenPos20_g225, VertPos20_g225, Hash20_g225, DitherNoiseTexture20_g225, DitherNoiseTextureSize20_g225, UseRandomDither20_g225 > 0, AlphaCutoutThreshold20_g225, DitherBlend20_g225,  alpha, AlphaThreshold20_g225);
				AlphaOut20_g225 = alpha;
				
				
				float3 Albedo = temp_output_175_0.xyz;
				float3 Emission = ( vertexToFrag6_g83 * (_Emission).rgb );
				float Alpha = ( (temp_output_175_0).w * AlphaOut20_g225 );
				float AlphaClipThreshold = AlphaThreshold20_g225;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero , One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 999999

			#pragma enable_d3d11_debug_symbols
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#define SHADER_GRAPH
			#pragma multi_compile _ MUDBUN_PROCEDURAL
			#include "Assets/MudBun/Shader/Render/ShaderCommon.cginc"
			#include "Assets/MudBun/Shader/Render/MeshCommon.cginc"


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				uint ase_vertexID : SV_VertexID;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _UseTex0;
			float _RandomDither;
			int _DitherTextureSize;
			float _Tex3Z;
			float _Tex3Y;
			float _Tex3X;
			float _Tex2Z;
			float _Tex2Y;
			float _Tex2X;
			float _Tex1Z;
			float _Tex1Y;
			float _Tex1X;
			float _MainTexZ;
			float _MainTexY;
			float _MainTexX;
			float _UseTex3;
			float _UseTex2;
			float _UseTex1;
			float _AlphaCutoutThreshold;
			float _Dithering;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _MainTex;
			sampler2D _Tex1;
			sampler2D _Tex2;
			sampler2D _Tex3;
			sampler2D _DitherTexture;


			float3 MudBunMeshPoint( int VertexID , out float3 PositionLs , out float3 NormalWs , out float3 NormalLs , out float4 Color , out float4 EmissionHash , out float Metallic , out float Smoothness , out float4 TextureWeight , out float SdfValue , out float3 Outward2dNormalLs , out float3 Outward2dNormalWs )
			{
				float4 positionWs;
				float2 metallicSmoothness;
				mudbun_mesh_vert(VertexID, positionWs, PositionLs, NormalWs, NormalLs, Color, EmissionHash, metallicSmoothness, TextureWeight, SdfValue, Outward2dNormalLs, Outward2dNormalWs);
				Metallic = metallicSmoothness.x;
				Smoothness = metallicSmoothness.y;
				return positionWs.xyz;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				int VertexID4_g83 = v.ase_vertexID;
				float3 PositionLs4_g83 = float3( 0,0,0 );
				float3 NormalWs4_g83 = float3( 0,0,0 );
				float3 NormalLs4_g83 = float3( 0,0,0 );
				float4 Color4_g83 = float4( 0,0,0,0 );
				float4 EmissionHash4_g83 = float4( 0,0,0,0 );
				float Metallic4_g83 = 0.0;
				float Smoothness4_g83 = 0.0;
				float4 TextureWeight4_g83 = float4( 1,0,0,0 );
				float SdfValue4_g83 = 0.0;
				float3 Outward2dNormalLs4_g83 = float3( 0,0,0 );
				float3 Outward2dNormalWs4_g83 = float3( 0,0,0 );
				float3 localMudBunMeshPoint4_g83 = MudBunMeshPoint( VertexID4_g83 , PositionLs4_g83 , NormalWs4_g83 , NormalLs4_g83 , Color4_g83 , EmissionHash4_g83 , Metallic4_g83 , Smoothness4_g83 , TextureWeight4_g83 , SdfValue4_g83 , Outward2dNormalLs4_g83 , Outward2dNormalWs4_g83 );
				float3 temp_output_198_0 = localMudBunMeshPoint4_g83;
				
				float3 vertexToFrag213 = NormalLs4_g83;
				o.ase_texcoord2.xyz = vertexToFrag213;
				float3 vertexToFrag212 = PositionLs4_g83;
				o.ase_texcoord3.xyz = vertexToFrag212;
				float4 vertexToFrag224 = TextureWeight4_g83;
				o.ase_texcoord4 = vertexToFrag224;
				float4 vertexToFrag5_g83 = Color4_g83;
				o.ase_texcoord5 = vertexToFrag5_g83;
				
				float3 vertexToFrag16_g83 = localMudBunMeshPoint4_g83;
				o.ase_texcoord6.xyz = vertexToFrag16_g83;
				float3 vertexToFrag27_g225 = temp_output_198_0;
				o.ase_texcoord7.xyz = vertexToFrag27_g225;
				
				o.ase_texcoord2.w = v.ase_vertexID;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.w = 0;
				o.ase_texcoord6.w = 0;
				o.ase_texcoord7.w = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = temp_output_198_0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = NormalWs4_g83;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				uint ase_vertexID : SV_VertexID;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.vertex = v.vertex;
				o.ase_vertexID = v.ase_vertexID;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_vertexID = patch[0].ase_vertexID * bary.x + patch[1].ase_vertexID * bary.y + patch[2].ase_vertexID * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 vertexToFrag213 = IN.ase_texcoord2.xyz;
				float3 weight54_g215 = abs( vertexToFrag213 );
				float3 appendResult83_g215 = (float3((float)(int)_MainTexX , (float)(int)_MainTexY , (float)(int)_MainTexZ));
				float3 weight0119_g215 = ( weight54_g215 * appendResult83_g215 );
				float3 temp_output_11_0_g223 = weight0119_g215;
				float3 break14_g223 = temp_output_11_0_g223;
				float3 vertexToFrag212 = IN.ase_texcoord3.xyz;
				float3 temp_output_36_0_g215 = vertexToFrag212;
				float2 uvX56_g215 = (temp_output_36_0_g215).yz;
				float4 appendResult23_g223 = (float4(temp_output_11_0_g223 , 0.0));
				float4 appendResult24_g223 = (float4(temp_output_11_0_g223 , 1.0));
				float4 break10_g224 = ( ( break14_g223.x + break14_g223.y + break14_g223.z ) > 0.0 ? appendResult23_g223 : appendResult24_g223 );
				float2 uvY57_g215 = (temp_output_36_0_g215).zx;
				float2 uvZ58_g215 = (temp_output_36_0_g215).xy;
				float4 color20_g223 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 vertexToFrag224 = IN.ase_texcoord4;
				float4 break10_g216 = vertexToFrag224;
				float3 appendResult123_g215 = (float3((float)(int)_Tex1X , (float)(int)_Tex1Y , (float)(int)_Tex1Z));
				float3 weight1126_g215 = ( weight54_g215 * appendResult123_g215 );
				float3 temp_output_11_0_g217 = weight1126_g215;
				float3 break14_g217 = temp_output_11_0_g217;
				float4 appendResult23_g217 = (float4(temp_output_11_0_g217 , 0.0));
				float4 appendResult24_g217 = (float4(temp_output_11_0_g217 , 1.0));
				float4 break10_g218 = ( ( break14_g217.x + break14_g217.y + break14_g217.z ) > 0.0 ? appendResult23_g217 : appendResult24_g217 );
				float4 color20_g217 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float3 appendResult130_g215 = (float3((float)(int)_Tex2X , (float)(int)_Tex2Y , (float)(int)_Tex2Z));
				float3 weight2133_g215 = ( weight54_g215 * appendResult130_g215 );
				float3 temp_output_11_0_g219 = weight2133_g215;
				float3 break14_g219 = temp_output_11_0_g219;
				float4 appendResult23_g219 = (float4(temp_output_11_0_g219 , 0.0));
				float4 appendResult24_g219 = (float4(temp_output_11_0_g219 , 1.0));
				float4 break10_g220 = ( ( break14_g219.x + break14_g219.y + break14_g219.z ) > 0.0 ? appendResult23_g219 : appendResult24_g219 );
				float4 color20_g219 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float3 appendResult137_g215 = (float3((float)(int)_Tex3X , (float)(int)_Tex3Y , (float)(int)_Tex3Z));
				float3 weight3140_g215 = ( weight54_g215 * appendResult137_g215 );
				float3 temp_output_11_0_g221 = weight3140_g215;
				float3 break14_g221 = temp_output_11_0_g221;
				float4 appendResult23_g221 = (float4(temp_output_11_0_g221 , 0.0));
				float4 appendResult24_g221 = (float4(temp_output_11_0_g221 , 1.0));
				float4 break10_g222 = ( ( break14_g221.x + break14_g221.y + break14_g221.z ) > 0.0 ? appendResult23_g221 : appendResult24_g221 );
				float4 color20_g221 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 color182 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
				float4 vertexToFrag5_g83 = IN.ase_texcoord5;
				float4 temp_output_25_0_g83 = ( _Color * vertexToFrag5_g83 );
				float4 temp_output_175_0 = ( ( ( _UseTex0 + _UseTex1 + _UseTex2 + _UseTex3 ) > 0.0 ? ( ( ( ( (float)(int)_UseTex0 > 0.0 ? ( ( ( ( break14_g223.x > 0.0 ? tex2D( _MainTex, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.x ) + ( ( break14_g223.y > 0.0 ? tex2D( _MainTex, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.y ) + ( ( break14_g223.z > 0.0 ? tex2D( _MainTex, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g224.z ) + ( color20_g223 * break10_g224.w ) ) / ( break10_g224.x + break10_g224.y + break10_g224.z + break10_g224.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.x ) + ( ( (float)(int)_UseTex1 > 0.0 ? ( ( ( ( break14_g217.x > 0.0 ? tex2D( _Tex1, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.x ) + ( ( break14_g217.y > 0.0 ? tex2D( _Tex1, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.y ) + ( ( break14_g217.z > 0.0 ? tex2D( _Tex1, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g218.z ) + ( color20_g217 * break10_g218.w ) ) / ( break10_g218.x + break10_g218.y + break10_g218.z + break10_g218.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.y ) + ( ( (float)(int)_UseTex2 > 0.0 ? ( ( ( ( break14_g219.x > 0.0 ? tex2D( _Tex2, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.x ) + ( ( break14_g219.y > 0.0 ? tex2D( _Tex2, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.y ) + ( ( break14_g219.z > 0.0 ? tex2D( _Tex2, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g220.z ) + ( color20_g219 * break10_g220.w ) ) / ( break10_g220.x + break10_g220.y + break10_g220.z + break10_g220.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.z ) + ( ( (float)(int)_UseTex3 > 0.0 ? ( ( ( ( break14_g221.x > 0.0 ? tex2D( _Tex3, uvX56_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.x ) + ( ( break14_g221.y > 0.0 ? tex2D( _Tex3, uvY57_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.y ) + ( ( break14_g221.z > 0.0 ? tex2D( _Tex3, uvZ58_g215 ) : float4( 0,0,0,0 ) ) * break10_g222.z ) + ( color20_g221 * break10_g222.w ) ) / ( break10_g222.x + break10_g222.y + break10_g222.z + break10_g222.w ) ) : float4( 0,0,0,0 ) ) * break10_g216.w ) ) / ( break10_g216.x + break10_g216.y + break10_g216.z + break10_g216.w ) ) : color182 ) * temp_output_25_0_g83 );
				
				float localComputeOpaqueTransparency20_g225 = ( 0.0 );
				float3 vertexToFrag16_g83 = IN.ase_texcoord6.xyz;
				float4 unityObjectToClipPos17_g83 = TransformWorldToHClip(TransformObjectToWorld(vertexToFrag16_g83));
				float4 computeScreenPos18_g83 = ComputeScreenPos( unityObjectToClipPos17_g83 );
				float2 ScreenPos20_g225 = (( ( computeScreenPos18_g83 / (computeScreenPos18_g83).w ) * _ScreenParams )).xy;
				float3 vertexToFrag27_g225 = IN.ase_texcoord7.xyz;
				float3 VertPos20_g225 = vertexToFrag27_g225;
				int VertexID4_g83 = IN.ase_texcoord2.w;
				float3 PositionLs4_g83 = float3( 0,0,0 );
				float3 NormalWs4_g83 = float3( 0,0,0 );
				float3 NormalLs4_g83 = float3( 0,0,0 );
				float4 Color4_g83 = float4( 0,0,0,0 );
				float4 EmissionHash4_g83 = float4( 0,0,0,0 );
				float Metallic4_g83 = 0.0;
				float Smoothness4_g83 = 0.0;
				float4 TextureWeight4_g83 = float4( 1,0,0,0 );
				float SdfValue4_g83 = 0.0;
				float3 Outward2dNormalLs4_g83 = float3( 0,0,0 );
				float3 Outward2dNormalWs4_g83 = float3( 0,0,0 );
				float3 localMudBunMeshPoint4_g83 = MudBunMeshPoint( VertexID4_g83 , PositionLs4_g83 , NormalWs4_g83 , NormalLs4_g83 , Color4_g83 , EmissionHash4_g83 , Metallic4_g83 , Smoothness4_g83 , TextureWeight4_g83 , SdfValue4_g83 , Outward2dNormalLs4_g83 , Outward2dNormalWs4_g83 );
				float Hash20_g225 = (EmissionHash4_g83).w;
				float AlphaIn20_g225 = (temp_output_25_0_g83).a;
				float AlphaOut20_g225 = 0;
				float AlphaThreshold20_g225 = 0;
				sampler2D DitherNoiseTexture20_g225 = _DitherTexture;
				int DitherNoiseTextureSize20_g225 = _DitherTextureSize;
				int UseRandomDither20_g225 = (int)_RandomDither;
				float AlphaCutoutThreshold20_g225 = _AlphaCutoutThreshold;
				float DitherBlend20_g225 = _Dithering;
				float alpha = AlphaIn20_g225;
				computeOpaqueTransparency(ScreenPos20_g225, VertPos20_g225, Hash20_g225, DitherNoiseTexture20_g225, DitherNoiseTextureSize20_g225, UseRandomDither20_g225 > 0, AlphaCutoutThreshold20_g225, DitherBlend20_g225,  alpha, AlphaThreshold20_g225);
				AlphaOut20_g225 = alpha;
				
				
				float3 Albedo = temp_output_175_0.xyz;
				float Alpha = ( (temp_output_175_0).w * AlphaOut20_g225 );
				float AlphaClipThreshold = AlphaThreshold20_g225;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}
		
	}
	/*ase_lod*/
	CustomEditor "MudBun.MudMeshMultiTexturedMaterialEditor"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18100
-1542;209;1438;795;718.436;147.7723;1;True;False
Node;AmplifyShaderEditor.FunctionNode;198;-128,-192;Inherit;False;Mud Mesh;0;;83;4f444db5091a94140ab2b15b933d37b6;0;0;15;COLOR;9;FLOAT;13;FLOAT3;10;FLOAT;11;FLOAT;12;FLOAT4;33;FLOAT3;0;FLOAT3;32;FLOAT3;2;FLOAT3;31;FLOAT3;48;FLOAT3;46;FLOAT;45;FLOAT2;15;FLOAT;41
Node;AmplifyShaderEditor.VertexToFragmentNode;212;384,-256;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;166;128,-720;Inherit;True;Property;_Tex3;Texture 3;24;0;Create;False;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;189;128,-912;Inherit;False;Property;_Tex2Z;Project Texture 2 Z;22;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;224;384,-352;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;183;128,-2160;Inherit;False;Property;_MainTexX;Project Texture 0 X;10;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;181;128,-800;Inherit;False;Property;_UseTex3;Use Texture 3;23;1;[Toggle];Create;False;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;190;128,-992;Inherit;False;Property;_Tex2Y;Project Texture 2 Y;21;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;194;128,-528;Inherit;False;Property;_Tex3X;Project Texture 3 X;26;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;184;128,-2080;Inherit;False;Property;_MainTexY;Project Texture 0 Y;11;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;146;128,-2352;Inherit;True;Property;_MainTex;Texture 0;8;0;Create;False;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;193;128,-448;Inherit;False;Property;_Tex3Y;Project Texture 3 Y;28;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;191;128,-1072;Inherit;False;Property;_Tex2X;Project Texture 2 X;20;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;188;128,-1456;Inherit;False;Property;_Tex1Z;Project Texture 1 Z;17;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;158;128,-1808;Inherit;True;Property;_Tex1;Texture 1;14;0;Create;False;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;178;128,-2432;Inherit;False;Property;_UseTex0;Use Texture 0;7;1;[Toggle];Create;False;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;179;128,-1888;Inherit;False;Property;_UseTex1;Use Texture 1;13;1;[Toggle];Create;False;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;213;384,-176;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;161;128,-1264;Inherit;True;Property;_Tex2;Texture 2;19;0;Create;False;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;180;128,-1344;Inherit;False;Property;_UseTex2;Use Texture 2;18;1;[Toggle];Create;False;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;187;128,-1536;Inherit;False;Property;_Tex1Y;Project Texture 1 Y;16;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;192;128,-368;Inherit;False;Property;_Tex3Z;Project Texture 3 Z;29;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;186;128,-1616;Inherit;False;Property;_Tex1X;Project Texture 1 X;15;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;185;128,-2000;Inherit;False;Property;_MainTexZ;Project Texture 0 Z;12;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;182;1488,-1408;Inherit;False;Constant;_White;White;26;0;Create;True;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;215;1152,-1616;Inherit;False;Mud Spherical Triplanar Texture Blend;-1;;215;56c8cc29a7f892f4da40c8af577af701;0;23;36;FLOAT3;0,0,0;False;49;FLOAT3;0,0,0;False;9;FLOAT4;1,0,0,0;False;4;INT;0;False;18;SAMPLER2D;0,0,0,0;False;33;INT;1;False;34;INT;1;False;35;INT;1;False;97;INT;0;False;96;SAMPLER2D;0,0,0,0;False;121;INT;1;False;122;INT;1;False;120;INT;1;False;105;INT;0;False;104;SAMPLER2D;0,0,0,0;False;128;INT;1;False;129;INT;1;False;127;INT;1;False;114;INT;0;False;115;SAMPLER2D;0,0,0,0;False;135;INT;1;False;136;INT;1;False;134;INT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;173;1152,-1824;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;171;1792,-1536;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT4;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;2048,-384;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-128,784;Inherit;False;Property;_Dithering;Dithering;6;0;Create;True;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;225;-128,592;Inherit;False;Property;_RandomDither;Random Dither;9;1;[Toggle];Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-128,688;Inherit;False;Property;_AlphaCutoutThreshold;Alpha Cutout Threshold;5;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;227;-128,480;Inherit;False;Property;_DitherTextureSize;Dither Texture Size;27;0;Create;True;0;0;False;0;False;256;0;0;1;INT;0
Node;AmplifyShaderEditor.TexturePropertyNode;226;-128,256;Inherit;True;Property;_DitherTexture;Dither Texture;25;0;Create;True;0;0;False;0;False;f240bbb7854046345b218811e5681a54;f240bbb7854046345b218811e5681a54;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ComponentMaskNode;176;2304,-288;Inherit;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;125;240,144;Inherit;False;Mud Alpha Threshold;-1;;225;926535703f4c32948ac1f55275a22bf0;0;9;8;FLOAT2;0,0;False;15;FLOAT3;0,0,0;False;18;FLOAT;0;False;22;FLOAT;0;False;19;SAMPLER2D;0;False;26;INT;256;False;9;INT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;2;FLOAT;24;FLOAT;25
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;228;2528,-256;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;218;2816,-320;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;220;2944,-960;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;221;2944,-960;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;222;2944,-960;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;223;2944,-960;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;219;2816,-288;Float;False;True;-1;2;MudBun.MudMeshMultiTexturedMaterialEditor;0;2;MudBun/Mud Mesh Multi-Textured (URP);94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;16;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;4;0;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;33;Workflow;1;Surface;0;  Refraction Model;0;  Blend;0;Two Sided;1;Transmission;0;  Transmission Shadow;0.5,False,-1;Translucency;0;  Translucency Strength;1,False,-1;  Normal Distortion;0.5,False,-1;  Scattering;2,False,-1;  Direct;0.9,False,-1;  Ambient;0.1,False,-1;  Shadow;0.5,False,-1;Cast Shadows;1;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;1;Built-in Fog;1;Meta Pass;1;Override Baked GI;0;Extra Pre Pass;0;DOTS Instancing;0;Tessellation;0;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Vertex Position,InvertActionOnDeselection;0;0;6;False;True;True;True;True;True;False;;0
WireConnection;212;0;198;32
WireConnection;224;0;198;33
WireConnection;213;0;198;31
WireConnection;215;36;212;0
WireConnection;215;49;213;0
WireConnection;215;9;224;0
WireConnection;215;4;178;0
WireConnection;215;18;146;0
WireConnection;215;33;183;0
WireConnection;215;34;184;0
WireConnection;215;35;185;0
WireConnection;215;97;179;0
WireConnection;215;96;158;0
WireConnection;215;121;186;0
WireConnection;215;122;187;0
WireConnection;215;120;188;0
WireConnection;215;105;180;0
WireConnection;215;104;161;0
WireConnection;215;128;191;0
WireConnection;215;129;190;0
WireConnection;215;127;189;0
WireConnection;215;114;181;0
WireConnection;215;115;166;0
WireConnection;215;135;194;0
WireConnection;215;136;193;0
WireConnection;215;134;192;0
WireConnection;173;0;178;0
WireConnection;173;1;179;0
WireConnection;173;2;180;0
WireConnection;173;3;181;0
WireConnection;171;0;173;0
WireConnection;171;2;215;0
WireConnection;171;3;182;0
WireConnection;175;0;171;0
WireConnection;175;1;198;9
WireConnection;176;0;175;0
WireConnection;125;8;198;15
WireConnection;125;15;198;0
WireConnection;125;18;198;41
WireConnection;125;22;198;13
WireConnection;125;19;226;0
WireConnection;125;26;227;0
WireConnection;125;9;225;0
WireConnection;125;6;64;0
WireConnection;125;7;65;0
WireConnection;228;0;176;0
WireConnection;228;1;125;24
WireConnection;219;0;175;0
WireConnection;219;2;198;10
WireConnection;219;3;198;11
WireConnection;219;4;198;12
WireConnection;219;6;228;0
WireConnection;219;7;125;25
WireConnection;219;8;198;0
WireConnection;219;10;198;2
ASEEND*/
//CHKSM=EF55057426E178BC9490B1E028CAD52870F346B1