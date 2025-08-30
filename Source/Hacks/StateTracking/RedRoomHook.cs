using HarmonyLib;
using System;
using UnityEngine;

namespace SuisHack.Hacks.StateTracking
{
	[HarmonyPatch]
	public static class RedRoomHook
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(RedRoomMain), nameof(RedRoomMain.Awake))]
		public static void OpenMenuPrefix(RedRoomMain __instance)
		{
			if (__instance.transform.childCount > 0)
			{
				var child = __instance.transform.GetChild(0).gameObject;

				if (child.GetComponent<RedRoomTracking>() == null)
				{
					child.AddComponent<RedRoomTracking>();
				}
			}
			else
				Plugin.Error("RedRoomMain has no child!");
		}
	}

	public class RedRoomTracking : MonoBehaviour
	{
		public RedRoomTracking(IntPtr ptr) : base(ptr) { }

		void OnEnable()
		{
			GameStateMachine.RedRoomOpened = true;
		}

		void OnDisable()
		{
			GameStateMachine.RedRoomOpened = false;
		}
	}
}
