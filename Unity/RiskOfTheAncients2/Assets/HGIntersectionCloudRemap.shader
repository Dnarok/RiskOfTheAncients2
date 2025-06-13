Shader "StubbedRoR2/Base/Shaders/HGIntersectionCloudRemap" {
	Properties {
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlendFloat ("Source Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlendFloat ("Destination Blend", Float) = 1
		[HDR] _TintColor ("Tint", Vector) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "grey" {}
		_Cloud1Tex ("Cloud 1 (RGB) Trans (A)", 2D) = "grey" {}
		_Cloud2Tex ("Cloud 2 (RGB) Trans (A)", 2D) = "grey" {}
		_RemapTex ("Color Remap Ramp (RGB)", 2D) = "grey" {}
		_CutoffScroll ("Cutoff Scroll Speed", Vector) = (0,0,0,0)
		_InvFade ("Soft Factor", Range(0, 30)) = 1
		_SoftPower ("Soft Power", Range(0.1, 20)) = 1
		_Boost ("Brightness Boost", Range(0, 5)) = 1
		_RimPower ("Rim Power", Range(0.1, 20)) = 1
		_RimStrength ("Rim Strength", Range(0, 5)) = 1
		_AlphaBoost ("Alpha Boost", Range(0, 20)) = 1
		_IntersectionStrength ("Intersection Strength", Range(0, 20)) = 0
		[MaterialEnum(Off,0,Front,1,Back,2)] _Cull ("Cull", Float) = 0
		[PerRendererData] _ExternalAlpha ("External Alpha", Range(0, 1)) = 1
		[Toggle(FADE_FROM_VERTEX_COLORS)] _FadeFromVertexColorsOn ("Fade Alpha from Vertex Color Luminance", Float) = 0
		[Toggle(TRIPLANAR)] _TriplanarOn ("Enable Triplanar Projections for Clouds", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_MatrixMVP;

			struct Vertex_Stage_Input
			{
				float3 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixMVP, float4(input.pos, 1.0));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, float2(input.uv.x, input.uv.y));
			}

			ENDHLSL
		}
	}
}