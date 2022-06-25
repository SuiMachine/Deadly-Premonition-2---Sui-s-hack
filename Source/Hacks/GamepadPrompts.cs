using HarmonyLib;
using SuisHack.GlobalGameObjects;
using System;
using static MelonLoader.MelonLogger;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class GamepadPromptsFix
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(UISprite), "OnInit")]
		public static void UISpriteOn(UISprite __instance)
		{
			if (GlobalReplacementAtlas.Instance != null && __instance.mSpriteName != null)
			{
				switch (GlobalReplacementAtlas.Instance.ReplacementTypeUsed)
				{
					case GlobalReplacementAtlas.ReplacementType.KeyboardAndMouse:
						ReplaceKeyboardMouse(__instance);
						break;
					default:
						ReplaceGamepadSteamInput(__instance);
						break;
				}
			}
		}

		private static void ReplaceGamepadSteamInput(UISprite instance)
		{
			switch (instance.mSpriteName)
			{
				case "NX_Cont_Button_A":
					GlobalReplacementAtlas.Instance.Replace(instance, GamepadKeys.A);
					break;
				case "common_intaract_A":
					GlobalReplacementAtlas.Instance.Replace(instance, GamepadKeys.A2);
					break;
				case "NX_Cont_Button_B":
					GlobalReplacementAtlas.Instance.Replace(instance, GamepadKeys.B);
					break;
				case "NX_Cont_Button_X":
					GlobalReplacementAtlas.Instance.Replace(instance, GamepadKeys.X);
					break;
				case "NX_Cont_Button_Y":
					GlobalReplacementAtlas.Instance.Replace(instance, GamepadKeys.Y);
					break;
			}
		}

		private static void ReplaceKeyboardMouse(UISprite instance)
		{
			switch (instance.mSpriteName)
			{
				//Seriously - fuck Nintendo and their layout!
				case "NX_Cont_Button_A":
				case "common_intaract_A":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("B_Button"));
					break;
				case "NX_Cont_Button_B":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("A_Button"));
					break;
				case "NX_Cont_Button_X":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("Y_Button"));
					break;
				case "NX_Cont_Button_Y":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("X_Button"));
					break;
				case "NX_Cont_Button_-":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("Back_Button"));
					break;
				case "NX_Cont_Button_ZL":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("LT"));
					break;
				case "NX_Cont_Button_L":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("LB"));
					break;
				case "NX_Cont_Button_+":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("Start_Button"));
					break;
				case "NX_Cont_Button_ZR":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("RT"));
					break;
				case "NX_Cont_Button_R":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("RB"));
					break;
				case "NX_stick_button_Stick_Button_L_Push":
				case "common_intaract_Lpush":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("L_Stick_Button"));
					break;
				case "NX_stick_button_Stick_Button_R_Push":
					GlobalReplacementAtlas.Instance.Replace(instance, GlobalInputHookHandler.Instance.GetReplacementPrompt("R_Stick_Button"));
					break;
			}
		}
	}
}
