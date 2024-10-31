using HarmonyLib;
using SCPE;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class PostProcessLayerHook
	{
		static List<PostProcessLayer> PostProcessLayerInstances = new List<PostProcessLayer>();
		static List<PostProcessVolume> PostProcessVolumeInstances = new List<PostProcessVolume>();


		#region Antialiasing
		private static PostProcessLayer.Antialiasing m_Antialiasing;
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
					postProcess.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.High;
				}
			}
		}
		#endregion

		#region HBAO
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
		#endregion

		#region Cameras Far Clip Distance
		private static float m_FarClipDistance;
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
		#endregion

		#region Screen space reflections
		private static bool m_SSR_Enabled;
		public static bool SSR_Enabled
		{
			get { return m_SSR_Enabled; }
			set
			{
				if (m_SSR_Enabled != value)
				{
					ClearNullReferences();

					m_SSR_Enabled = value;
					ApplySSRChange();
				}
			}
		}

		private static ScreenSpaceReflectionPreset m_SSR_Preset;
		public static ScreenSpaceReflectionPreset SSR_Preset
		{
			get { return m_SSR_Preset; }
			set
			{
				if (m_SSR_Preset != value)
				{
					ClearNullReferences();

					m_SSR_Preset = value;
					ApplySSRChange();
				}
			}
		}

		private static ScreenSpaceReflectionResolution m_SSR_Resolution;
		public static ScreenSpaceReflectionResolution SSR_Resolution
		{
			get { return m_SSR_Resolution; }
			set
			{
				if (m_SSR_Resolution != value)
				{
					ClearNullReferences();

					m_SSR_Resolution = value;
					ApplySSRChange();
				}
			}
		}

		private static float m_SSR_Tickness;
		public static float SSR_Tickness
		{
			get { return m_SSR_Tickness; }
			set
			{
				if (m_SSR_Tickness != value)
				{
					ClearNullReferences();

					m_SSR_Tickness = value;
					ApplySSRChange();
				}
			}
		}

		private static float m_SSR_Vignette;
		public static float SSR_Vignette
		{
			get { return m_SSR_Vignette; }
			set
			{
				if (m_SSR_Vignette != value)
				{
					ClearNullReferences();

					m_SSR_Vignette = value;
					ApplySSRChange();
				}
			}
		}

		private static float m_SSR_DistanceFade;
		public static float SSR_DistanceFade
		{
			get { return m_SSR_DistanceFade; }
			set
			{
				if (m_SSR_DistanceFade != value)
				{
					ClearNullReferences();

					m_SSR_DistanceFade = value;
					ApplySSRChange();
				}
			}
		}

		private static float m_SSR_MaxMarchingDistance;
		public static float SSR_MaxMarchingDistance
		{
			get { return m_SSR_MaxMarchingDistance; }
			set
			{
				if (m_SSR_MaxMarchingDistance != value)
				{
					ClearNullReferences();

					m_SSR_MaxMarchingDistance = value;
					ApplySSRChange();
				}
			}
		}
		#endregion

		#region Edge Detection
		private static bool m_EnableEdgeDetectionFilter;
		public static bool EnableEdgeDetectionFilter
		{
			get { return m_EnableEdgeDetectionFilter; }
			set
			{
				if (m_EnableEdgeDetectionFilter != value)
				{
					ClearNullReferences();

					m_EnableEdgeDetectionFilter = value;
					ApplyEdgeDetectionChange();
				}
			}
		}

		private static float m_EnableEdgeDetectionFilterDepth;
		public static float EnableEdgeDetectionFilterDepth
		{
			get { return m_EnableEdgeDetectionFilterDepth; }
			set
			{
				if (m_EnableEdgeDetectionFilterDepth != value)
				{
					ClearNullReferences();

					m_EnableEdgeDetectionFilterDepth = value;
					ApplyEdgeDetectionChange();
				}
			}
		}
		#endregion

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

		private static void ApplySSRChange()
		{
			foreach (var volume in PostProcessVolumeInstances)
			{
				if (volume == null)
				{
					Plugin.Error("Volume was null!");
					continue;
				}
				else if (volume.profile == null)
				{
					Plugin.Error("Profile was null!");
					continue;
				}

				ScreenSpaceReflections ssr = volume.profile.GetSetting<ScreenSpaceReflections>();
				if (ssr == null)
				{
					Plugin.Error("Ssr was null");
					continue;
				}

				if (m_SSR_Enabled)
				{
					ssr.enabled.overrideState = true;
					if (volume.gameObject.scene.name.ToLower() == "map_002_1_coldwarehouse")
					{
						ssr.enabled.value = false;
						ssr.active = false;
					}
					else
					{
						ssr.enabled.value = true;
						ssr.active = true;
					}

					ssr.preset.overrideState = true;
					ssr.preset.value = m_SSR_Preset;

					ssr.resolution.overrideState = true;
					ssr.resolution.value = m_SSR_Resolution;

					ssr.thickness.overrideState = true;
					ssr.thickness.value = m_SSR_Tickness; //0.3

					ssr.vignette.overrideState = true;
					ssr.vignette.value = m_SSR_Vignette; //0.3

					ssr.distanceFade.overrideState = true;
					ssr.distanceFade.value = m_SSR_DistanceFade; //0.1

					ssr.maximumMarchDistance.overrideState = true;
					ssr.maximumMarchDistance.value = m_SSR_MaxMarchingDistance;
				}
				else
					ssr.enabled.overrideState = false;
			}
		}

		private static void ApplyEdgeDetectionChange()
		{
			foreach (var volume in PostProcessVolumeInstances)
			{
				if (volume == null)
				{
					Plugin.Error("Volume was null!");
					continue;
				}
				else if (volume.profile == null)
				{
					Plugin.Error("Profile was null!");
					continue;
				}

				EdgeDetection edgeDetection = volume.profile.GetSetting<EdgeDetection>();
				if (edgeDetection == null)
				{
					Plugin.Error("EdgeDetection was null");
					continue;
				}
				edgeDetection.enabled.value = m_EnableEdgeDetectionFilter;
				edgeDetection.sensitivityDepth.value = m_EnableEdgeDetectionFilterDepth;
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
			__instance.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.High;
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

		[HarmonyPostfix]
		[HarmonyPatch(typeof(PostProcessVolume), "OnEnable")]
		public static void PostProcessVolumeOnEnablePostfix(PostProcessVolume __instance)
		{
			if (!PostProcessVolumeInstances.Contains(__instance))
				PostProcessVolumeInstances.Add(__instance);

			else if (__instance.profile == null)
			{
				Plugin.Error("Profile was null!");
				return;
			}


			EdgeDetection edgeDetection = __instance.profile.GetSetting<EdgeDetection>();
			if (edgeDetection != null)
			{
				edgeDetection.enabled.value = m_EnableEdgeDetectionFilter;
				edgeDetection.sensitivityDepth.value = m_EnableEdgeDetectionFilterDepth;
			}

			if (!m_SSR_Enabled)
				return;

			ScreenSpaceReflections ssr = __instance.profile.GetSetting<ScreenSpaceReflections>();
			if (ssr != null)
			{
				ssr.enabled.overrideState = true;
				if (__instance.gameObject.scene.name.ToLower() == "map_002_1_coldwarehouse")
				{
					ssr.enabled.value = false;
					ssr.active = false;
				}
				else
				{
					ssr.enabled.value = true;
					ssr.active = true;
				}

				ssr.preset.overrideState = true;
				ssr.preset.value = m_SSR_Preset;

				ssr.resolution.overrideState = true;
				ssr.resolution.value = m_SSR_Resolution;

				ssr.thickness.overrideState = true;
				ssr.thickness.value = m_SSR_Tickness; //0.3

				ssr.vignette.overrideState = true;
				ssr.vignette.value = m_SSR_Vignette; //0.3

				ssr.distanceFade.overrideState = true;
				ssr.distanceFade.value = m_SSR_DistanceFade; //0.1

				ssr.maximumMarchDistance.overrideState = true;
				ssr.maximumMarchDistance.value = m_SSR_MaxMarchingDistance;
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(PostProcessVolume), "OnDisable")]
		public static void PostProcessVolumeOnDisablePostfix(PostProcessVolume __instance)
		{
			if (PostProcessVolumeInstances.Contains(__instance))
				PostProcessVolumeInstances.Remove(__instance);
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
