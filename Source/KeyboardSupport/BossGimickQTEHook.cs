using HarmonyLib;
using System.Xml.Linq;

namespace SuisHack.KeyboardSupport
{
	[HarmonyPatch]
	public static class BossGimickQTEHook
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(BossGimickQTE), nameof(BossGimickQTE.EndCutScene))]
		public static void EndCutsceneDetourPost(BossGimickQTE __instance)
		{
			if (Plugin.Settings.Input_Override.Value != ExposedSettings.InputType.KeyboardAndMouse)
				return;

			if (__instance.m_hudQTE == null || __instance.m_hudQTE.m_uiButton == null)
				return;

			//keys - circle, cross, triangle, square - why the fuck are these Playstation in a game that was ported to PC
			switch (__instance.m_buttonIndex)
			{
				case 0:
					__instance.m_hudQTE.m_uiButton.spriteName = "NX_Cont_Button_A";
					break;
				case 1:
					__instance.m_hudQTE.m_uiButton.spriteName = "NX_Cont_Button_B";
					break;
				case 2:
					__instance.m_hudQTE.m_uiButton.spriteName = "NX_Cont_Button_X";
					break;
				case 3:
					__instance.m_hudQTE.m_uiButton.spriteName = "NX_Cont_Button_Y";
					break;
			}

			//Plugin.Message($"Replace QTE: {__instance.m_hudQTE.m_uiButton.spriteName}");
			KeyboardPrompts.UISpriteOn(__instance.m_hudQTE.m_uiButton);
		}
	}
}
