using Steamworks;
using System.Collections.Generic;

namespace SuisHack.KeyboardSupport
{
	public static class SteamInputHook
	{
		public readonly static Dictionary<string, InputAnalogActionHandle_t> AnalogInputDictionary = new Dictionary<string, InputAnalogActionHandle_t>()
		{
			{ "None", new InputAnalogActionHandle_t(0) },
			{ "R_Stick", new InputAnalogActionHandle_t(1) },
			{ "L_Stick", new InputAnalogActionHandle_t(2) }
		};

		public readonly static Dictionary<string, InputDigitalActionHandle_t> DigitalInputDictionary = new Dictionary<string, InputDigitalActionHandle_t>()
		{
			{ "None", new InputDigitalActionHandle_t(0) },
			{ "A_Button", new InputDigitalActionHandle_t(1) },
			{ "B_Button", new InputDigitalActionHandle_t(2) },
			{ "X_Button", new InputDigitalActionHandle_t(3) },
			{ "Y_Button", new InputDigitalActionHandle_t(4) },
			{ "Back_Button", new InputDigitalActionHandle_t(5) },
			{ "Start_Button", new InputDigitalActionHandle_t(6) },
			{ "Up_Button", new InputDigitalActionHandle_t(7) },
			{ "Down_Button", new InputDigitalActionHandle_t(8) },
			{ "Left_Button", new InputDigitalActionHandle_t(9) },
			{ "Right_Button", new InputDigitalActionHandle_t(10) },
			{ "LB", new InputDigitalActionHandle_t(11) },
			{ "RB", new InputDigitalActionHandle_t(12) },
			{ "LT", new InputDigitalActionHandle_t(13) },
			{ "RT", new InputDigitalActionHandle_t(14) },
			{ "L_Stick_Button", new InputDigitalActionHandle_t(15) },
			{ "R_Stick_Button", new InputDigitalActionHandle_t(16) },
		};

		public static void InitializeKeyboardAndMouse()
		{
			var harmonyInstance = SuisHackMain.harmonyInst;
			var originalGetAnalogActionMethod = typeof(SteamInput).GetMethod(nameof(SteamInput.GetAnalogActionData), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			var targetGetAnalogActionMethod = typeof(SteamInputHook).GetMethod(nameof(GetAnalogActionDataPrefix), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			if (originalGetAnalogActionMethod == null)
			{
				SuisHackMain.loggerInst.Error("Original GetAnalogActionData was null");
				return;
			}

			if (targetGetAnalogActionMethod == null)
			{
				SuisHackMain.loggerInst.Error("Target GetAnalogActionData was null");
				return;
			}

			var originalGetDigitalActionMethod = typeof(SteamInput).GetMethod(nameof(SteamInput.GetDigitalActionData), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			var targetGetDigitalActionMethod = typeof(SteamInputHook).GetMethod(nameof(GetDigitalActionDataPrefix), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

			if (originalGetDigitalActionMethod == null)
			{
				SuisHackMain.loggerInst.Error("Original GetDigitalActionData was null");
				return;
			}

			if (targetGetDigitalActionMethod == null)
			{
				SuisHackMain.loggerInst.Error("Target GetDigitalActionData was null");
				return;
			}

			var originalGetAnalogActionHandle = typeof(SteamInput).GetMethod(nameof(SteamInput.GetAnalogActionHandle), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			var targetGetAnalogActionHandle = typeof(SteamInputHook).GetMethod(nameof(GetAnalogActionHandle), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

			if (originalGetAnalogActionHandle == null)
			{
				SuisHackMain.loggerInst.Error("Original GetAnalogActionHandle was null");
				return;
			}

			if (targetGetAnalogActionHandle == null)
			{
				SuisHackMain.loggerInst.Error("Target GetAnalogActionHandle was null");
				return;
			}

			var originalGetDigitalActionHandle = typeof(SteamInput).GetMethod(nameof(SteamInput.GetDigitalActionHandle), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			var targetGetDigitalActionHandle = typeof(SteamInputHook).GetMethod(nameof(GetDigitalActionHandle), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

			if (originalGetDigitalActionHandle == null)
			{
				SuisHackMain.loggerInst.Error("Original GetDigitalActionHandle was null");
				return;
			}

			if (targetGetDigitalActionHandle == null)
			{
				SuisHackMain.loggerInst.Error("Target GetDigitalActionHandle was null");
				return;
			}

			harmonyInstance.Patch(originalGetAnalogActionMethod, prefix: new HarmonyLib.HarmonyMethod(targetGetAnalogActionMethod));
			harmonyInstance.Patch(originalGetDigitalActionMethod, prefix: new HarmonyLib.HarmonyMethod(targetGetDigitalActionMethod));
			harmonyInstance.Patch(originalGetAnalogActionHandle, prefix: new HarmonyLib.HarmonyMethod(targetGetAnalogActionHandle));
			harmonyInstance.Patch(originalGetDigitalActionHandle, prefix: new HarmonyLib.HarmonyMethod(targetGetDigitalActionHandle));
			SuisHackMain.loggerInst.Msg("Patched Steam Input to redirect to keyboard mouse manager");
		}


		public static bool GetDigitalActionHandle(ref InputDigitalActionHandle_t __result, string pszActionName)
		{
			__result = DigitalInputDictionary[pszActionName];
			return false;
		}

		public static bool GetAnalogActionHandle(ref InputAnalogActionHandle_t __result, string pszActionName)
		{
			__result = AnalogInputDictionary[pszActionName];
			return false;
		}

		public static bool GetAnalogActionDataPrefix(ref InputAnalogActionData_t __result, InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
		{
			if (SettingsGUI.Display)
			{
				__result = new InputAnalogActionData_t();
				return false;
			}

			var replacement = GlobalInputHookHandler.Instance.GetAnalogInputReplacement(analogActionHandle);
			if (replacement == null)
				__result = new InputAnalogActionData_t();
			else
				__result = replacement.GetInput();

			return false;

		}

		public static bool GetDigitalActionDataPrefix(ref InputDigitalActionData_t __result, InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle)
		{
			if (SettingsGUI.Display)
			{
				__result = new InputDigitalActionData_t();
				return false;
			}

			var replacement = GlobalInputHookHandler.Instance.GetDigitalInputReplacement(digitalActionHandle);

			if (replacement == null)
				__result = new InputDigitalActionData_t();
			else
				__result = replacement.GetInput();

			return false;
		}
	}
}
