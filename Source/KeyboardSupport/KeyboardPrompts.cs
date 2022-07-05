using HarmonyLib;
using SuisHack.GlobalGameObjects;
using static SuisHack.KeyboardSupport.GlobalInputHookHandler;

namespace SuisHack.KeyboardSupport
{
	public static class KeyboardPrompts
	{
		public static void Initialize()
		{
			if (SuisHackMain.Settings.Input_Override.Value == ExposedSettings.InputType.KeyboardAndMouse)
			{
				var source = typeof(UISprite).GetMethod(nameof(UISprite.OnInit), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
				var target = typeof(KeyboardPrompts).GetMethod(nameof(KeyboardPrompts.UISpriteOn), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

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
				SuisHackMain.loggerInst.Msg("Initialized keyboard/mouse patch");
			}
		}


		public static void UISpriteOn(UISprite __instance)
		{
			if (GlobalReplacementAtlas.Instance != null && __instance.mSpriteName != null)
			{
				//SuisHackMain.loggerInst.Msg($"Trying to replace: {__instance.mSpriteName}"); 
				switch (__instance.mSpriteName)
				{
					//Seriously - fuck Nintendo and their layout!
					case "NX_Cont_Button_A":
					case "common_intaract_A":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.B_Button));
						break;
					case "NX_Cont_Button_B":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.A_Button));
						break;
					case "NX_Cont_Button_X":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.Y_Button));
						break;
					case "NX_Cont_Button_Y":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.X_Button));
						break;
					case "NX_Cont_Button_-":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.Back_Button));
						break;
					case "NX_Cont_Button_ZL":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.LT));
						break;
					case "NX_Cont_Button_L":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.LB));
						break;
					case "NX_Cont_Button_+":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.Start_Button));
						break;
					case "NX_Cont_Button_ZR":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.RT));
						break;
					case "NX_Cont_Button_R":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.RB));
						break;
					case "NX_stick_button_Stick_Button_L_Push":
					case "common_intaract_Lpush":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.L_Stick_Button));
						break;
					case "NX_stick_button_Stick_Button_R_Push":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.R_Stick_Button));
						break;
					case "NX_Cont_Button_ARROW_L":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.Left_Button));
						break;
					case "NX_Cont_Button_ARROW_U":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.Up_Button));
						break;
					case "NX_Cont_Button_ARROW_D":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.Down_Button));
						break;
					case "NX_Cont_Button_ARROW_R":
						GlobalReplacementAtlas.Instance.Replace(__instance, Instance.GetReplacementPrompt(SteamInputHook.SteamInputDigital.Right_Button));
						break;
				}
			}
		}
	}
}
