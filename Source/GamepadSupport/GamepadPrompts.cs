using HarmonyLib;
using SuisHack.GlobalGameObjects;

namespace SuisHack.GamepadSupport
{
	public static class GamepadPrompts
	{
		public static void Initialize()
		{
			if (SuisHackMain.Settings.Input_Override.Value == ExposedSettings.InputType.SteamInput)
			{
				var source = typeof(UISprite).GetMethod(nameof(UISprite.OnInit), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
				var target = typeof(GamepadPrompts).GetMethod(nameof(GamepadPrompts.UISpriteOn), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

				if (source == null)
				{
					SuisHackMain.loggerInst.Msg("Failed to find source method for GamepadPrompts");
					return;
				}

				if (target == null)
				{
					SuisHackMain.loggerInst.Msg("Failed to find target method for GamepadPrompts");
					return;
				}

				SuisHackMain.harmonyInst.Patch(source, postfix: new HarmonyMethod(target));
				SuisHackMain.loggerInst.Msg("Initialized GamepadPrompts patch");
			}
		}

		public static void UISpriteOn(UISprite __instance)
		{
			if (GlobalReplacementAtlas.Instance != null && __instance.mSpriteName != null)
			{
				switch (__instance.mSpriteName)
				{
					case "NX_Cont_Button_A":
						GlobalReplacementAtlas.Instance.Replace(__instance, GamepadKeyIcons.A);
						break;
					case "common_intaract_A":
						GlobalReplacementAtlas.Instance.Replace(__instance, GamepadKeyIcons.A2);
						break;
					case "NX_Cont_Button_B":
						GlobalReplacementAtlas.Instance.Replace(__instance, GamepadKeyIcons.B);
						break;
					case "NX_Cont_Button_X":
						GlobalReplacementAtlas.Instance.Replace(__instance, GamepadKeyIcons.X);
						break;
					case "NX_Cont_Button_Y":
						GlobalReplacementAtlas.Instance.Replace(__instance, GamepadKeyIcons.Y);
						break;
				}
			}
		}
	}
}
