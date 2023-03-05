using HarmonyLib;
using SuisHack.Components;
using UnityEngine;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class BeautifyHack
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(BeautifyEffect.Beautify), nameof(BeautifyEffect.Beautify.LateUpdate))]
		public static void LateUpdatePrefix()
		{
/*			if (___currentCamera != null)
			{
				GPU_Instances_Controller.ActiveCamera = ___currentCamera;
			}*/
		}
	}
}
