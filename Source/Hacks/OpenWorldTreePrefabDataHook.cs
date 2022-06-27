using HarmonyLib;
using UnityEngine;

namespace SuisHack.Hacks
{
/*	[HarmonyPatch]
	public static class OpenWorldTreePrefabDataHook
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(OpenWorldTreePrefabData), nameof(OpenWorldTreePrefabData.Init))]
		public static void InitPostfix(OpenWorldTreePrefabData __instance)
		{
			var lodGroups = __instance.GetComponentsInChildren<LODGroup>(true);
			for(int i=0; i<lodGroups.Count; i++)
			{
				lodGroups[i].size *= 3;
			}
		}
	}*/
}
