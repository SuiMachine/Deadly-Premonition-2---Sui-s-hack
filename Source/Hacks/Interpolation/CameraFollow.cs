using HarmonyLib;
using UnityEngine;

namespace SuisHack.Hacks.Interpolation
{
	[HarmonyPatch]
	public static class BoatCameraFollowHook
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(BoatSceneSetup), nameof(BoatSceneSetup.Init))]
		public static void Init(BoatSceneSetup __instance)
		{
			var smoothers = GameObject.FindObjectsOfType<Components.Interpolation.SmootherController>();
			for(int i=0; i<smoothers.Count; i++)
			{
				if (smoothers[i].GetComponent<Components.Interpolation.BoatFollowInterpolation>() == null)
				{
					smoothers[i].gameObject.AddComponent<Components.Interpolation.BoatFollowInterpolation>();
				}
			}
		}
	}
}
