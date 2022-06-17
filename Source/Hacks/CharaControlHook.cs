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

			if (SuisHackMain.Settings.Entry_Other_InterpolateMovement.Value)
			{
				var interpolation = __instance.GetComponent<GameObjectInterpolation>();
				if (interpolation == null)
				{
					__instance.gameObject.AddComponent<GameObjectInterpolation>();
				}
			}
		}
	}
}
