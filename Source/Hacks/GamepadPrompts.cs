using HarmonyLib;
using SuisHack.GlobalGameObjects;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class GamepadPromptsFix
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(UISprite), "OnInit")]
		public static void UISpriteOn(UISprite __instance)
		{
			if (GlobalReplacementAtlas.Instance.Atlas != null)
			{
				if (__instance.mSpriteName != null && __instance.mSpriteName == "NX_Cont_Button_A")
				{
					UIKeyGuideItemReplaceSprite(__instance, "A");
				}
				else if (__instance.mSpriteName != null && __instance.mSpriteName == "NX_Cont_Button_B")
				{
					UIKeyGuideItemReplaceSprite(__instance, "B");
				}
				else if (__instance.mSpriteName != null && __instance.mSpriteName == "NX_Cont_Button_X")
				{
					UIKeyGuideItemReplaceSprite(__instance, "X");
				}
				else if (__instance.mSpriteName != null && __instance.mSpriteName == "NX_Cont_Button_Y")
				{
					UIKeyGuideItemReplaceSprite(__instance, "Y");
				}
			}
		}

		private static void UIKeyGuideItemReplaceSprite(UISprite m_buttonIcon, string name)
		{
			var replacementAtlas = GlobalReplacementAtlas.Instance.Atlas;
			for (int i = 0; i < replacementAtlas.spriteList.Count; i++)
			{
				if (replacementAtlas.spriteList[i].name == name)
				{
					var replacement = replacementAtlas.spriteList[i];
					m_buttonIcon.atlas = replacementAtlas;
					m_buttonIcon.SetAtlasSprite(replacement);
					//SuisHackMain.loggerInst.Msg($"Replaced! {name}");
					return;
				}
			}

			SuisHackMain.loggerInst.Msg($"Failed to replace!");
		}
	}
}
