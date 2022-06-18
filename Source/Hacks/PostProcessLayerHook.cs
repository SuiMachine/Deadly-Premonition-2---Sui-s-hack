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
				foreach(var postProcess in PostProcessLayerInstances)
				{
					var camera = postProcess.GetComponent<Camera>();
					if(camera != null)
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
			__instance.antialiasingMode = SuisHackMain.Settings.Entry_Antialiasing.Value;
			if (!PostProcessLayerInstances.Contains(__instance))
				PostProcessLayerInstances.Add(__instance);

			var camera = __instance.GetComponent<Camera>();
			if(camera != null)
			{
				camera.farClipPlane = m_FarClipDistance;
			}


/*			var volume = __instance.GetComponentInChildren<PostProcessVolume>();
			if (volume != null)
			{
				var motionBlur = volume.profile.GetSetting<MotionBlur>();
				if (motionBlur != null)
				{
					SuisHackMain.loggerInst.Msg("Has motion blur");
*//*					motionBlur.active = true;
					motionBlur.shutterAngle.overrideState = true;
					motionBlur.shutterAngle.value = 270;
					motionBlur.sampleCount.overrideState = true;
					motionBlur.sampleCount.value = 16;
*//*
				}
				else
				{
					SuisHackMain.loggerInst.Msg("Motion blur was null");
				}
			}
			else
				SuisHackMain.loggerInst.Msg("Volume was null");*/

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
