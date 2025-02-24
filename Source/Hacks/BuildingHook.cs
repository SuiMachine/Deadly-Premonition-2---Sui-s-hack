using HarmonyLib;
using UnityEngine;


namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class BuildingHook
	{
		public static bool Use = true;
		public static MeshRenderer[] MeshRenderers = new MeshRenderer[10];
		public static MeshRenderer ShadowRenderer = null;

		public static Vector3[] IncorrectCoordinateBuildings = new Vector3[]
		{
			new Vector3(-985.615f, 9.375f, -558.485f)
		};


		[HarmonyPostfix]
		[HarmonyPatch(typeof(ClockObject), "Start")]
		public static void Hook(ClockObject __instance)
		{
			if (!Use)
				return;

			if (__instance.GetComponent<BuildingModel>() == null)
				return;

			for (int i = 0; i < IncorrectCoordinateBuildings.Length; i++)
			{
				if (IncorrectCoordinateBuildings[i] == __instance.transform.position)
				{
					GameObject.Destroy(__instance.gameObject);
					return;
				}
			}

			for (int i = 0; i < MeshRenderers.Length; i++)
				MeshRenderers[i] = null;
			ShadowRenderer = null;

			int j = 0;
			for (int i = __instance.transform.childCount - 1; i >= 0; i--)
			{
				var child = __instance.transform.GetChild(i);
				var mr = child.GetComponent<MeshRenderer>();
				if (mr == null)
					continue;

				if (mr.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly)
				{
					if (ShadowRenderer != null)
						Plugin.Error("Already assigned shadow renderer?!");
					else
						ShadowRenderer = mr;
				}
				else
				{
					MeshRenderers[j] = mr;
					j++;
				}
			}

			if (ShadowRenderer == null)
				return;
			else
			{
				GameObject.Destroy(ShadowRenderer.gameObject);

				for (int i = 0; i < MeshRenderers.Length; i++)
				{
					if (MeshRenderers[i] == null)
						break;
					MeshRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
					MeshRenderers[i].receiveShadows = true;
					MeshRenderers[i].castShadows = true;
				}
			}
		}
	}
}
