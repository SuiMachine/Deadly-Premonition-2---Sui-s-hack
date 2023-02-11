using HarmonyLib;
using System;
using UnityEngine;

namespace SuisHack.Hacks.StateStracking
{
	[HarmonyPatch]
	public static class MapMenuManagerHook
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(MapUI), nameof(MapUI.Init))]
		public static void OpenMenuPrefix(MapUI __instance)
		{
			if (__instance.GetComponent<MapUIManagerTracking>() == null)
			{
				__instance.gameObject.AddComponent<MapUIManagerTracking>();
			}
		}
	}

	[MelonLoader.RegisterTypeInIl2Cpp]
	public class MapUIManagerTracking : MonoBehaviour
	{
		public MapUIManagerTracking(IntPtr ptr) : base(ptr) { }

		private void OnEnable()
		{
			GameStateMachine.MapOpened = true;
		}

		private void OnDisable()
		{
			GameStateMachine.MapOpened = false;
		}
	}
}
