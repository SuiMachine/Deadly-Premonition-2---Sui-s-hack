using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace SuisHack.Hacks.Interpolation
{
	[HarmonyPatch]
	public static class BoatSceneSetupHook
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(BoatSceneSetup), nameof(BoatSceneSetup.Init))]
		public static void Init(BoatSceneSetup __instance)
		{
			var smoothers = GameObject.FindObjectsOfType<Components.Interpolation.SmootherController>();
			for (int i = 0; i < smoothers.Length; i++)
			{
				if (smoothers[i].GetComponent<Components.Interpolation.BoatFollowInterpolation>() == null)
				{
					smoothers[i].gameObject.AddComponent<Components.Interpolation.BoatFollowInterpolation>();
				}
			}
		}
	}
}
