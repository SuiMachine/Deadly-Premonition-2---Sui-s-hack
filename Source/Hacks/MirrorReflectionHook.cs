using HarmonyLib;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class MirrorReflectionHook
	{
		public static int ReflectionSize;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(MirrorReflection), "OnWillRenderObject")]
		public static void OnWillRenderObjectPostfix(MirrorReflection __instance)
		{
			if(__instance.m_TextureSize != ReflectionSize)
			{
				__instance.m_TextureSize = ReflectionSize;
			}
		}
	}
}
