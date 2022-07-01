using HarmonyLib;
using SuisHack.GlobalGameObjects;

namespace SuisHack.KeyboardSupport
{
	public static class KeyboardPrompts
	{
		public static void Initialize()
		{
			if (SuisHackMain.Settings.Input_Override.Value == ExposedSettings.InputType.KeyboardAndMouse)
			{
				var source = typeof(UISprite).GetMethod(nameof(UISprite.OnInit), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
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
			}
		}

		
		public static void UISpriteOn(UISprite __instance)
		{
			if (GlobalReplacementAtlas.Instance != null && __instance.mSpriteName != null)
			{

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
