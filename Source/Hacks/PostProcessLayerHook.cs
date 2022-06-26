using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class PostProcessLayerHook
	{
		const float TAA_JitterSpread = 0.1f;
		static List<PostProcessLayer> PostProcessLayerInstances = new List<PostProcessLayer>();
		private static PostProcessLayer.Antialiasing m_Antialiasing;
		private static float m_FarClipDistance;
		public static PostProcessLayer.Antialiasing Antialiasing
		{
			get { return m_Antialiasing; }
			set
			{
				ClearNullReferences();

				m_Antialiasing = value;
				foreach (var postProcess in PostProcessLayerInstances)
				{
					postProcess.antialiasingMode = value;
					postProcess.temporalAntialiasing.jitterSpread = TAA_JitterSpread;
				}
			}
		}

		private static HBAO_Core.Preset m_HBAO_Preset = HBAO_Core.Preset.FastestPerformance;
		public static HBAO_Core.Preset HBAO_Preset
		{
			get { return m_HBAO_Preset; }
			set
			{
				if (m_HBAO_Preset != value)
				{
					ClearNullReferences();

					m_HBAO_Preset = value;
					ApplyHBAOChange();

				}
			}
		}

		private static float m_HBAO_Intensity = 1f;
		public static float HBAO_Intensity
		{
			get { return m_HBAO_Intensity; }
			set
			{
				if (m_HBAO_Intensity != value)
				{
					ClearNullReferences();

					m_HBAO_Intensity = value;
					ApplyHBAOChange();
				}
			}
		}

		private static void ApplyHBAOChange()
		{
			foreach (var postprocess in PostProcessLayerInstances)
			{
				var hbao = postprocess.GetComponent<HBAO>();
				if (hbao != null)
				{
					hbao.ApplyPreset(m_HBAO_Preset);
					var settings = hbao.aoSettings;
					settings.intensity = m_HBAO_Intensity;
					hbao.aoSettings = settings;
				}
			}
		}

		public static float FarClipDistance
		{
			get { return m_FarClipDistance; }
			set
			{
				ClearNullReferences();

				m_FarClipDistance = value;
				foreach (var postProcess in PostProcessLayerInstances)
				{
					var camera = postProcess.GetComponent<Camera>();
					if (camera != null)
					{
						camera.farClipPlane = m_FarClipDistance;
					}
				}
			}
		}

		public static void ClearNullReferences()
		{
			if (PostProcessLayerInstances.Contains(null))
			{
				PostProcessLayerInstances = PostProcessLayerInstances.Where(x => x != null).ToList();
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(PostProcessLayer), "OnEnable")]
		public static void PostProcessLayerAwakePostfix(PostProcessLayer __instance)
		{
			__instance.antialiasingMode = m_Antialiasing;
			__instance.temporalAntialiasing.jitterSpread = TAA_JitterSpread;
			if (!PostProcessLayerInstances.Contains(__instance))
				PostProcessLayerInstances.Add(__instance);

			var camera = __instance.GetComponent<Camera>();
			if (camera != null)
			{
				camera.farClipPlane = m_FarClipDistance;
			}

			var hbao = __instance.GetComponent<HBAO>();
			if (hbao != null)
			{
				hbao.ApplyPreset(m_HBAO_Preset);
				var settings = hbao.aoSettings;
				settings.intensity = m_HBAO_Intensity;
				hbao.aoSettings = settings;
			}
		}

		internal static string GetShortName()
		{
			switch (m_Antialiasing)
			{
				case PostProcessLayer.Antialiasing.FastApproximateAntialiasing:
					return "FXAA";
				case PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing:
					return "SMAA";
				case PostProcessLayer.Antialiasing.TemporalAntialiasing:
					return "TAA";
				default:
					return "None";
			}
		}
	}
}