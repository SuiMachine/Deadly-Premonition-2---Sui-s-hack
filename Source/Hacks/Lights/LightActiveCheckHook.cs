using HarmonyLib;

namespace SuisHack.Hacks.Lights
{
	[HarmonyPatch]
	public static class LightActiveCheckHook
	{
		public static void Initialize()
		{
			if (SuisHackMain.Settings.Entry_Other_LightImprovements.Value == ExposedSettings.LightImprovements.All)
			{
				var sourceHook = typeof(LightActiveCheck).GetMethod(nameof(LightActiveCheck.Start), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
				var targetHook = typeof(LightActiveCheckHook).GetMethod(nameof(LightActiveCheckHook.StartPostifx), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

				if (sourceHook == null)
				{
					SuisHackMain.loggerInst.Error("Failed to found source for LightActiveCheckHook");
					return;
				}

				if (targetHook == null)
				{
					SuisHackMain.loggerInst.Error("Failed to found target for LightActiveCheckHook");
					return;
				}

				SuisHackMain.harmonyInst.Patch(sourceHook, postfix: new HarmonyMethod(targetHook));
			}
		}

		public static void StartPostifx(LightActiveCheck __instance)
		{
			if(__instance.m_light != null)
			{
				__instance.m_light.shadows = UnityEngine.LightShadows.Soft;
				__instance.m_light.renderMode = UnityEngine.LightRenderMode.Auto;
			}
		}
	}
}
