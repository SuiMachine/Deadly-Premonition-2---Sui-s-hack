using HarmonyLib;
using UnityEngine;

namespace SuisHack.Hacks
{
	public static class NpcTestHook
	{
		public static void Initialize()
		{
			if (ExposedSettings.Instance.Entry_Other_GeometryImprovements.Value >= ExposedSettings.GeometryImprovements.Enabled)
			{
				var sourceHook = typeof(NpcTest).GetMethod(nameof(NpcTest.Init), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
				var targetHook = typeof(NpcTestHook).GetMethod(nameof(NpcTestHook.InitPostfix), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

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

		public static void InitPostfix(NpcTest __instance)
		{
			var lodGroup = __instance.GetComponent<LODGroup>();
			if (lodGroup != null)
			{
				lodGroup.size *= 3;
			}
		}
	}
}
