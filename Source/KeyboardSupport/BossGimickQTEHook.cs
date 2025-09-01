using HarmonyLib;
using SuisHack.GamepadSupport;
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
			//This hook is because the game doesn't set a new instance of a sprite, it just tries to change sprite name, which I think points it to a different spot in atlas - I think
			//cause this entire system is awful and I have no idea why would any developer use this over just Unity UI (which is still bad, but better than this shit)
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

			if (Plugin.Settings.Input_Override.Value != ExposedSettings.InputType.KeyboardAndMouse)
				GamepadPrompts.UISpriteOn(__instance.m_hudQTE.m_uiButton);
			else
				KeyboardPrompts.UISpriteOn(__instance.m_hudQTE.m_uiButton);
		}
	}
}
