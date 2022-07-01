using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace SuisHack.Hacks
{
#if DEBUG
/*	[HarmonyPatch]
	public static class Testing
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(OpenWorldFenceArea), nameof(OpenWorldFenceArea.UpdateInfo))]
		public static void UpdateInfo(OpenWorldFenceArea __instance)
		{
			for(int i=0; i<__instance.m_drawInfoList.Count; i++)
			{
				var drawInfo = __instance.m_drawInfoList[i];
				for (int j=0; j< drawInfo.m_matrixList.Count; j++)
				{
					var dupa = drawInfo.m_matrixList[j];
					var position = new Vector3(dupa[0, 3], dupa[1, 3], dupa[2, 3]);
					var rotation = drawInfo.m_matrixList[j].rotation;
					var scale = new Vector3(50, 50, 50);
					drawInfo.m_matrixList[j].SetTRS(position, rotation, scale);
				}
			}
		}

	}*/
#endif
}
