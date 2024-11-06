using HarmonyLib;
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
			var interpolation = __instance.GetComponent<Components.Interpolation.GameObjectInterpolation>();
			if (interpolation == null)
			{
				__instance.gameObject.AddComponent<Components.Interpolation.GameObjectInterpolation>();
			}

			if (ExposedSettings.Instance.Entry_Other_LightImprovements.Value >= ExposedSettings.LightImprovements.Minor)
			{
				var lights = __instance.GetComponentsInChildren<Light>(true);
				for (int i = 0; i < lights.Length; i++)
				{
					var light = lights[i];
					light.shadows = LightShadows.Soft;
					light.shadowNearPlane = 0.01f;
					light.shadowBias = 0.01f;
					light.renderMode = LightRenderMode.ForcePixel;
				}
			}
		}
	}
}
