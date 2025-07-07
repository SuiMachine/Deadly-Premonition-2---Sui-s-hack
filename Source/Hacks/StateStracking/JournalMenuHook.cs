using HarmonyLib;

namespace SuisHack.Hacks.StateStracking
{
	[HarmonyPatch]
	public static class JournalMenuHook
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(JournalMenu), nameof(JournalMenu.OnFinished))]
		public static void OnFinishedPrefix(JournalMenu __instance)
		{
			GameStateMachine.RedRoomQuestMenu = false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(JournalMenu), nameof(JournalMenu.Close))]
		public static void OnClosePrefix(JournalMenu __instance)
		{
			Plugin.Message("Menu Close");
			GameStateMachine.RedRoomQuestMenu = false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(JournalMenu), nameof(JournalMenu.OpenChoiceWindow))]
		public static void ChoiceMenuPrefix(JournalMenu __instance)
		{
			Plugin.Message("Menu Choice");
			GameStateMachine.RedRoomQuestMenu = true;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(JournalMenu), nameof(JournalMenu.OpenFreeQuestMenu))]
		public static void FreeQuestPrefix(JournalMenu __instance)
		{
			Plugin.Message("Menu Free quest");
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(JournalMenu), nameof(JournalMenu.OpenSideQuestMenu))]
		public static void SideQuestPrefix(JournalMenu __instance)
		{
			Plugin.Message("Menu side quest");
		}
	}
}
