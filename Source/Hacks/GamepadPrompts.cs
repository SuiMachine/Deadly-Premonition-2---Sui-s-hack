using HarmonyLib;
using SuisHack.GlobalGameObjects;
using System.Xml.Linq;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class GamepadPromptsFix
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(UISprite), "OnInit")]
		public static void UISpriteOn(UISprite __instance)
		{
			if (GlobalReplacementAtlas.Instance.Atlas != null && __instance.mSpriteName != null)
			{
				switch (__instance.mSpriteName)
				{
					case "NX_Cont_Button_A":
						UIKeyGuideItemReplaceSprite(__instance, "A");
						break;
					case "common_intaract_A":
						UIKeyGuideItemReplaceSprite(__instance, "A2");
						break;
					case "NX_Cont_Button_B":
						UIKeyGuideItemReplaceSprite(__instance, "B");
						break;
					case "NX_Cont_Button_X":
						UIKeyGuideItemReplaceSprite(__instance, "X");
						break;
					case "NX_Cont_Button_Y":
						UIKeyGuideItemReplaceSprite(__instance, "Y");
						break;
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

			SuisHackMain.loggerInst.Msg($"Failed to replace {m_buttonIcon.mSpriteName}");
		}
	}
}
