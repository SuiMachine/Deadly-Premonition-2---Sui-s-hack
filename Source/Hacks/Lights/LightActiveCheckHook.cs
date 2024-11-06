using HarmonyLib;

namespace SuisHack.Hacks.Lights
{
	public static class LightActiveCheckHook
	{
		public static void Initialize()
		{
			if (ExposedSettings.Instance.Entry_Other_LightImprovements.Value == ExposedSettings.LightImprovements.All)
			{
				var sourceHook = typeof(LightActiveCheck).GetMethod(nameof(LightActiveCheck.Start), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
				var targetHook = typeof(LightActiveCheckHook).GetMethod(nameof(LightActiveCheckHook.StartPostifx), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

				if (sourceHook == null)
				{
					Plugin.Error("Failed to found source for LightActiveCheckHook");
					return;
				}

				if (targetHook == null)
				{
					Plugin.Error("Failed to found target for LightActiveCheckHook");
					return;
				}

				Plugin.HarmonyInstance.Patch(sourceHook, postfix: new HarmonyMethod(targetHook));
			}
		}

		public static void StartPostifx(LightActiveCheck __instance)
		{
			if (__instance.m_light != null)
			{
				__instance.m_light.shadows = UnityEngine.LightShadows.Soft;
				__instance.m_light.renderMode = UnityEngine.LightRenderMode.Auto;
			}
		}
	}
}
