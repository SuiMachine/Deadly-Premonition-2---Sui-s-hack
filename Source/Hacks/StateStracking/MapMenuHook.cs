using HarmonyLib;

namespace SuisHack.Hacks.StateStracking
{
	[HarmonyPatch]
	class MapMenuManagerHook
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(MapMenuManager), nameof(MapMenuManager.OpenMenu))]
		public static void OpenMenuPrefix()
		{
			SuisHackMain.CurrentGameState = SuisHackMain.Gamestate.Map;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(MapMenuManager), nameof(MapMenuManager.CloseMenu))]
		public static void CloseMenuPrefix()
		{
			SuisHackMain.CurrentGameState = SuisHackMain.Gamestate.StandardGameplay;
		}
	}
}
