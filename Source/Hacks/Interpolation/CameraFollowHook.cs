using HarmonyLib;
using SuisHack.Components;
using UnityEngine;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class CameraFollowHook
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(CameraFollow), "Init")]
		public static void CameraFollowHookInit(CameraFollow __instance)
		{
			//Originally was planning on interpolating camera, but that looked quite junk, so I just used it for pre-cull / post render.
			var smootherController = __instance.GetComponent<Components.Interpolation.SmootherController>();
			if (smootherController == null)
			{
				__instance.gameObject.AddComponent<Components.Interpolation.SmootherController>();
			}

			var interpolation = __instance.GetComponent<Components.Interpolation.GameObjectInterpolation>();
			if (interpolation == null)
			{
				__instance.gameObject.AddComponent<Components.Interpolation.GameObjectInterpolation>();
			}
		}
	}
}
