using HarmonyLib;
using SuisHack.Components;
using UnityEngine;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class CharaControlHook
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(CharaControl), "Init")]
		public static void CharaControlHookInit(CharaControl __instance)
		{
			var interpolation = __instance.GetComponent<GameObjectInterpolation>();
			if (interpolation == null)
			{
				__instance.gameObject.AddComponent<GameObjectInterpolation>();
			}

			if(SuisHackMain.Settings.Entry_Other_LightImprovements.Value >= ExposedSettings.LightImprovements.Minor)
			{
				var lights = __instance.GetComponentsInChildren<Light>(true);
				for(int i=0; i<lights.Count; i++)
				{
					var light = lights[i];
					light.shadows = LightShadows.Soft;
					light.shadowNearPlane = 0.01f;
					light.shadowBias = 0.01f;
					light.renderMode = LightRenderMode.ForcePixel;
					light.shadowSoftness = 1;
				}
			}
		}
	}
}
