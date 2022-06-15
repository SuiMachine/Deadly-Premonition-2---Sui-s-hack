using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering.PostProcessing;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class PostProcessLayerHook
	{
		static List<PostProcessLayer> PostProcessLayerInstances = new List<PostProcessLayer>();
		private static PostProcessLayer.Antialiasing m_Antialiasing;
		public static PostProcessLayer.Antialiasing Antialiasing
		{
			get { return m_Antialiasing; }
			set
			{
				if(PostProcessLayerInstances.Contains(null))
				{
					PostProcessLayerInstances = PostProcessLayerInstances.Where(x => x != null).ToList();
				}

				m_Antialiasing = value;
				foreach(var postProcess in PostProcessLayerInstances)
				{
					postProcess.antialiasingMode = value;
				}
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(PostProcessLayer), "OnEnable")]
		public static void PostProcessLayerAwakePostfix(PostProcessLayer __instance)
		{
#if DEBUG
			SuisHack.SuisHackMain.loggerInst.Msg($"Setting AA for {__instance} to {m_Antialiasing}");
#endif
			__instance.antialiasingMode = SuisHackMain.Settings.Entry_Antialiasing.Value;
			if (!PostProcessLayerInstances.Contains(__instance))
				PostProcessLayerInstances.Add(__instance);

		}

		internal static string GetShortName()
		{
			switch(m_Antialiasing)
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
