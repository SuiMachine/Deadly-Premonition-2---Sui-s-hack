using HarmonyLib;
using SuisHack.Components;

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
		}
	}
}
