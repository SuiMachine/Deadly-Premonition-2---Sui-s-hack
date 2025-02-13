using HarmonyLib;
using UnityEngine;


namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class BuildingHook
	{
		public static bool Use = true;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(ClockObject), "Start")]
		public static void Hook(ClockObject __instance)
		{
			if (!Use)
				return;

			if (__instance.GetComponent<BuildingModel>() == null)
				return;

			for (int i = __instance.transform.childCount - 1; i >= 0; i--)
			{
				var child = __instance.transform.GetChild(i);
				if (child.name.ToLower().EndsWith("_shadow"))
				{
					GameObject.Destroy(child.gameObject);
					//Plugin.Message("Destroying shadow mesh");
				}
				else
				{
					var mr = child.GetComponent<MeshRenderer>();
					if (mr == null)
						continue;

					mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
					mr.receiveShadows = true;
					//Plugin.Message("Setting shadows");
				}
			}
		}
	}
}
