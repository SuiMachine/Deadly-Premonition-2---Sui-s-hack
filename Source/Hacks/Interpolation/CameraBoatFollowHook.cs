using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace SuisHack.Hacks.Interpolation
{
	[HarmonyPatch]
	public static class BoatCameraFollowHook
	{
		public static BoatCameraFollow? LastCheckBehaviour;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(BoatCameraFollow), nameof(BoatCameraFollow.LateUpdate))]
		public static void CameraBoatFix3(BoatCameraFollow __instance)
		{
			if (LastCheckBehaviour != __instance)
			{
				if (__instance.GetComponent<Components.Interpolation.SmootherController>() != null)
				{
					var obj = __instance.GetComponent<Components.Interpolation.SmootherController>();
					MonoBehaviour.Destroy(obj);
				}
				LastCheckBehaviour = __instance;
			}
		}
	}
}
