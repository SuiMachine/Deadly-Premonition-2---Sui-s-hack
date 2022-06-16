using HarmonyLib;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class CameraFollowHook
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(CameraFollow), "Init")]
		public static void CameraFollowHookInit(CameraFollow __instance)
		{
			var component = __instance.GetComponent<Components.CameraFollowSmoother>();
			if(component == null)
			{
				__instance.gameObject.AddComponent<Components.CameraFollowSmoother>();
			}
		}
	}
}
