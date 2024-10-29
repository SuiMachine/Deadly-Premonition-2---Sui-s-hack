using HarmonyLib;
using UnityEngine;

namespace SuisHack.Hacks.Lights
{
	public static class NpcVehicleHook
	{
		public static void Initialize()
		{
			if (ExposedSettings.Instance.Entry_Other_LightImprovements.Value == ExposedSettings.LightImprovements.All)
			{
				var sourceHook = typeof(NpcVehicle).GetMethod(nameof(NpcVehicle.Awake), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
				var targetHook = typeof(NpcVehicleHook).GetMethod(nameof(NpcVehicleHook.NpcVehiclePostfix), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

				if (sourceHook == null)
				{
					Plugin.Error("Failed to found source for NpcTestHook");
					return;
				}

				if (targetHook == null)
				{
					Plugin.Error("Failed to found target for NpcTestHook");
					return;
				}

				Plugin.HarmonyInstance.Patch(sourceHook, postfix: new HarmonyMethod(targetHook));
			}
		}

		public static void NpcVehiclePostfix(NpcVehicle __instance)
		{
			var lights = __instance.GetComponentsInChildren<Light>(true);
			for (int i = 0; i < lights.Length; i++)
			{
				lights[i].shadows = LightShadows.Soft;
			}
		}
	}
}
