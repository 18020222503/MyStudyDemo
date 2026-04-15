Shader "Lumi/LumiDazzling"
{
	Properties
	{
		[BFoldout(1,2,0,1,1)] _Basic("基础设置 _Foldout", Float) = 1
		[HDR]_ColorA("ColorA",Color) = (1,1,1,1)
		[HDR]_ColorB("ColorB",Color) = (0,0,0,1)
		_CloudTex ("云图使用UV2", 2D) = "black" {}
		_Flow("X:U Y:V Z:Speed",Vector) = (1,1,1,0)

		[BFoldout(1,2,0,1,1)] _StarSection("自定义图案设置,这里也是2U _Foldout", Float) = 1
		[NoScaleOffset]_StarTex("R:星星 G:自定义图案", 2D) = "white" {}
		_StarTiling("星星分布密度", Float) = 18
		_StarDensity("星星出现概率", Range(0,1)) = 0.2
		_StarMinScale("星星最小值", Float) = 0.4
		_StarMaxScale("星星最大值", Float) = 1.0
		_StarColorBRange("星星根据ClorB的范围", Range(0,1)) = 0.5

		[Space (20 )]
		_HeartTiling("爱心分布密度", Float) = 9
		_HeartDensity("爱心出现概率", Range(0,1)) = 0.08
		_HeartMinScale("爱心最小值", Float) = 1.0
		_HeartMaxScale("爱心最大值", Float) = 2.0
		_HeartSpeed("爱心缩放速度", Float) = 1.5
		_HeartColorMin("爱心最小时颜色", Color) = (1, 0.4, 0.8, 1)
		//_HeartColorMid("爱心中间色", Color) = (1, 0.5, 0.5, 1)
		_HeartColorMid1("爱心中间色1", Color) = (1, 0.5, 0.5, 1)
		_HeartColorMid2("爱心中间色2", Color) = (1, 1, 1, 1)
		_HeartColorMax("爱心最大时颜色", Color) = (0.6, 0.0, 1.0, 1)
		_HeartHighPos("爱心高色阈值", Range(0,1)) = 0.6
		_HeartMid2Pos("爱心第二中间阈值", Range(0,1)) = 0.5
		_HeartLowPos("爱心低色阈值", Range(0,1)) = 0.4
		_HeartMidPos("爱心高低过度,0是硬切", Range(0.01, 0.2)) = 0.2
		_HeartColorLerp("爱心融合强度,0不融合,1融合",Range(0,1)) = 0


		[BFoldout(1,2,0,1,1)] _StarSection("亮部设置 _Foldout", Float) = 1
		_customHighLightMap ("自定义高光图:A通道是范围和强度", 2D) = "black" {}
		[HDR]_SpecColor("高光颜色", Color) = (1,1,1,1)
		_customHighLightMapUV("高光UV速度", Vector) = (0,0,0,0)
		_customHighIntensity("高光强度",Range(0,1)) = 1
		[MaterialToggle(_enableCustomHightLight)]_enableCustomHightLight("单独显示这张高光图",Float) = 0


		[BFoldout(1,2,0,1,1)] _OutlineSection("描边设置 _Foldout", Float) = 1
		[MaterialToggle(OutLineSmeshEnable)]_OutLineSmeshEnable("不使用Smesh数据", int) = 0
		[MaterialToggle(OutLineChannle)]_OutLineChannle("使用顶点色A通道", int) = 0
		_OutlineWidth("描边宽度", Range(0.01, 10)) = 1
		_OutlinePara("正交视角描边系数,默认0.4",range(0.01,1)) = 0.4
		_OutLineColor("描边颜色", Color) = (0.5,0.5,0.5,1)
		_OutlineOffset("描边偏移", Range(-0.1, 10)) = 0
		[BFoldout(1,2,1,0,1, CurvedEnable)] _CurvedEnable("使用球面映射 _Foldout", Float) = 1


		// Blending state_SSRimAdd
		[HideInInspector] _Surface("__surface", Float) = 0.0
		[HideInInspector] _BlendMode("__blend", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
		[HideInInspector] _CullingMode("__cull", Float) = 2.0
		[HideInInspector] _AlphaClip("__clip", Float) = 0.0
		[HideInInspector]_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		[BFoldoutOut(1)] _FoldoutOut5("Foldoutout_Foldout", float) = 1
	}

	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
			"RenderPipeline" = "UniversalPipeline"
			"IgnoreProjector" = "True"
			"Queue" = "Geometry"
		}

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "./Assets/Shaders/Common/CommonFunction.hlsl"

		CBUFFER_START(UnityPerMaterial)
			// tilling
			float4 _customHighLightMap_ST;
			float4 _CloudTex_ST;
			half4 _BaseColor;
			half _BlockScale;
			half4 _Star;
			half4 _Flow;
			half4 _ColorA;
			half4 _ColorB;
			half _NoiseScale;
			half _WarpStrength;
			half _CurvedEnable;

			//enum
			half _Surface;
			half _Cutoff;

			float _StarTiling;
			float _StarDensity;
			float _StarMinScale;
			float _StarMaxScale;
			float4 _StarColor;
			float _StarIntensity;
			half4 _OutLineColor;
			float _OutlineWidth;
			half _OutLineClipEnable;
			float _OutLineClip;
			half _OutLineChannle;
			half _OutlinePara;
			half _OutLineSmeshEnable;
			half _OutlineOffset;
			half4 _StarColorMin;
			half4 _StarColorMax;
			half4 _SpecColor;
			half4 _customHighLightMapUV;
			half _enableCustomHightLight;
			half _StarColorBRange;
			float _HeartTiling;
			float _HeartDensity;
			float _HeartMinScale;
			float _HeartMaxScale;
			float _HeartSpeed;
			half4 _HeartColorMin;
			half4 _HeartColorMax;
			//half4 _HeartColorMid;
			half _customHighIntensity;
			half _HeartColorLerp;
			half _HeartMidPos;
			float _HeartHighPos;
			float _HeartMid2Pos;
			float _HeartLowPos;
			half4 _HeartColorMid1;
			half4 _HeartColorMid2;
		CBUFFER_END


		TEXTURE2D(_customHighLightMap);
		SAMPLER(sampler_customHighLightMap);
		TEXTURE2D(_CloudTex);
		SAMPLER(sampler_CloudTex);
		TEXTURE2D(_StarTex);
		SAMPLER(sampler_StarTex);
		ENDHLSL

		Pass
		{
			Name "ForwardLit"
			Tags
			{
				"LightMode" = "UniversalForward"
			}
			Blend [_SrcBlend][_DstBlend]
			ZWrite On
			Cull [_CullingMode]

			HLSLPROGRAM
			//适配
			#pragma only_renderers gles3  glcore d3d11 metal vulkan
			#pragma target 3.0
			#pragma vertex vertSimple
			#pragma fragment fragSimple

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local _ALPHATEST_ON


			float hash21(float2 p)
			{
				p = frac(p * float2(123.34, 456.21));
				p += dot(p, p + 45.32);
				return frac(p.x * p.y);
			}

			float2 hash22(float2 p)
			{
				float n = hash21(p);
				float m = hash21(p + 17.13);
				return float2(n, m);
			}

			float2 rotateUV(float2 uv, float angle)
			{
				float s = sin(angle);
				float c = cos(angle);

				float2 p = uv - 0.5;

				float2 r;
				r.x = p.x * c - p.y * s;
				r.y = p.x * s + p.y * c;

				return r + 0.5;
			}

			struct DecoData
			{
				float starMask;
				float heartMask;
				float4 heartColor;
			};

			DecoData SampleDecorations(float2 uv, float cloudRange)
			{
				DecoData o;
				o.starMask = 0;
				o.heartMask = 0;
				o.heartColor = 0;

				// STAR
				{
					float2 gv = uv * _StarTiling;
					float2 id = floor(gv);
					float2 f = frac(gv);

					float rndExist = hash21(id + 11.3);
					float starExist = step(rndExist, _StarDensity);

					float2 center = lerp(float2(0.2, 0.2), float2(0.8, 0.8), hash22(id + 3.7));

					float scaleRnd = hash21(id + 31.7);
					float phase = hash21(id + 73.1) * 6.2831853;
					float pulse = 0.5 + 0.5 * sin(_Time.y * 3.0 + phase);

					float starScale = lerp(_StarMinScale, _StarMaxScale, scaleRnd) * pulse;

					float2 localUV = (f - center) / max(starScale, 1e-5) + 0.5;

					float angle = hash21(id + 91.7) * 6.2831853;
					localUV = rotateUV(localUV, angle);

					float inRange = step(0.0, localUV.x) * step(0.0, localUV.y) * step(localUV.x, 1.0) * step(
						localUV.y, 1.0);

					float4 tex = SAMPLE_TEXTURE2D(_StarTex, sampler_StarTex, localUV);

					float starShape = tex.r; // * tex.a;
					float starAA = max(fwidth(starShape), 1e-4);
					starShape = smoothstep(0.5 - starAA, 0.5 + starAA, starShape);
					//starShape = step(0.5, starShape);

					o.starMask = starShape * starExist * inRange * cloudRange;
				}

				// HEART
				{
					float2 gv = uv * _HeartTiling;
					float2 id = floor(gv);
					float2 f = frac(gv);

					float rndExist = hash21(id + 211.3);
					float heartExist = step(rndExist, _HeartDensity);

					float2 center = lerp(float2(0.3, 0.3), float2(0.7, 0.7), hash22(id + 123.7));

					float scaleRnd = hash21(id + 231.7);
					float phase = hash21(id + 273.1) * 6.2831853;
					float pulse = 0.5 + 0.5 * sin(_Time.y * _HeartSpeed + phase);

					float heartScale = lerp(_HeartMinScale, _HeartMaxScale, scaleRnd) * pulse;

					float2 local = (f - center) / max(heartScale, 1e-5);
					float2 localUV = local + 0.5;
					float angle = hash21(id + 291.7) * 6.2831853;
					localUV = rotateUV(localUV, angle);
					float inRange = step(-0.25, localUV.x) * step(-0.25, localUV.y) * step(localUV.x, 1.25) * step(
						localUV.y, 1.25);

					float2 sampleUV = saturate(localUV);
					float4 tex = SAMPLE_TEXTURE2D(_StarTex, sampler_StarTex, sampleUV);

					float heartShape = tex.g; // * tex.a;
					float heartAA = max(fwidth(heartShape), 1e-4);
					heartShape = smoothstep(0.5 - heartAA, 0.5 + heartAA, heartShape);
					//heartShape = step(0.5, heartShape);

					o.heartMask = heartShape * heartExist * inRange; // * cloudRange;
					o.heartMask = lerp(o.heartMask, o.heartMask * cloudRange, _HeartColorLerp);
					float mid = _HeartMidPos;
					float invMid = 1.0 / max(mid, 1e-4);
					float invTail = 1.0 / max(1.0 - mid, 1e-4);
					//float t1 = saturate(pulse * invMid);
					//float t2 = saturate((pulse - mid) * invTail);
					//float3 c1 = lerp(_HeartColorMin.rgb, _HeartColorMid.rgb, t1);
					//float3 c2 = lerp(_HeartColorMid.rgb, _HeartColorMax.rgb, t2);
					float high = _HeartHighPos; // 例如 0.75
					float mid2 = _HeartMid2Pos; // 例如 0.5
					float low = _HeartLowPos; // 例如 0.25
					float w = _HeartMidPos * 0.5; // 过渡宽度

					float3 colMin = _HeartColorMin.rgb;
					float3 colMid1 = _HeartColorMid1.rgb;
					float3 colMid2 = _HeartColorMid2.rgb;
					float3 colMax = _HeartColorMax.rgb;

					// 三个软阈值
					float lowMask = smoothstep(low - w, low + w, pulse);
					float mid2Mask = smoothstep(mid2 - w, mid2 + w, pulse);
					float highMask = smoothstep(high - w, high + w, pulse);

					// 四段颜色
					float3 color0 = lerp(colMin, colMid1, lowMask);
					float3 color1 = lerp(color0, colMid2, mid2Mask);
					float3 finalheartColor = lerp(color1, colMax, highMask);
					o.heartColor = half4(finalheartColor, 1);
				}

				return o;
			}

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				float3 normalOS : NORMAL;
				float2 uv2 : TEXCOORD1;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				float3 normalWS : TEXCOORD2;
				float4 positionOS : TEXCOORD3;
				float3 normalOS : TEXCOORD4;
				float2 uv2 : TEXCOORD5;
			};

			Varyings vertSimple(Attributes input)
			{
				Varyings output;
				CurvedWorld_LittlePlanet_Y(input.positionOS, _PivotPosition.xyz, _BendSize, _BendOffset, _CurvedEnable);
				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.normalWS = TransformObjectToWorldNormal(input.normalOS);
				output.normalOS = input.normalOS;
				output.positionOS = input.positionOS;
				output.uv = input.uv;
				output.uv2 = input.uv2;
				return output;
			}

			half4 fragSimple(Varyings input) : SV_Target
			{
				float2 uv = input.uv2 * _CloudTex_ST.xy + _CloudTex_ST.zw;
				uv = uv + _Time.y * _Flow.xy * _Flow.z;

				float cloud = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, uv).r;

				DecoData deco = SampleDecorations(input.uv2, cloud);


				half4 baseColor = lerp(_ColorA, _ColorB, cloud);

				//星星
				half4 color = lerp(baseColor, 1, smoothstep(_StarColorBRange, 1, deco.starMask));

				//爱心
				color.rgb = lerp(color.rgb, deco.heartColor.rgb, deco.heartMask);

				float3 objToView = normalize(TransformWorldToView(input.positionWS));
				float3 viewNorm = cross(objToView, mul(UNITY_MATRIX_V, float4(input.normalWS, 0.0)).xyz);
				float2 viewCross = float2(-viewNorm.y, viewNorm.x);

				float2 DataUV = viewCross * 0.5 * _customHighLightMap_ST.xy
					+ 0.5
					+ _customHighLightMap_ST.zw
					+ _Time.y * half2(_customHighLightMapUV.x, _customHighLightMapUV.y);

				float4 SpecMap = SAMPLE_TEXTURE2D(_customHighLightMap, sampler_customHighLightMap, DataUV);

				color = lerp(color, _SpecColor, SpecMap.r * SpecMap.a * _customHighIntensity);
				color = lerp(color, SpecMap, _enableCustomHightLight);

				return color;
			}
			ENDHLSL
		}


		Pass
		{
			Name "Outline"
			Tags
			{
				"LightMode"="Outline_Card"
			}
			Cull Front
			Offset 1,1

			HLSLPROGRAM
			//适配
			#pragma only_renderers gles3 glcore d3d11 metal vulkan

			#pragma target 3.0
			#pragma vertex vertPlus
			#pragma fragment frag
			#pragma shader_feature_local _ALPHATEST_ON
			#include "./Assets/Shaders/Common/outlinepass.hlsl"
			ENDHLSL
		}




	}
	CustomEditor "LumiShader.ShaderLib.ShaderGUI.CharShaderGUI"
}
