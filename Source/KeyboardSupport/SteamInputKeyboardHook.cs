using Steamworks;

namespace SuisHack.KeyboardSupport
{
	public static class SteamInputHook
	{
		public static void InitializeKeyboardAndMouse()
		{
			var harmonyInstance = SuisHackMain.harmonyInst;
			var originalGetAnalogActionMethod = typeof(SteamInput).GetMethod(nameof(SteamInput.GetAnalogActionData), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			var targetGetAnalogActionMethod = typeof(SteamInputHook).GetMethod(nameof(GetAnalogActionDataPrefix), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			if(originalGetAnalogActionMethod == null)
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

			harmonyInstance.Patch(originalGetAnalogActionMethod, prefix: new HarmonyLib.HarmonyMethod(targetGetAnalogActionMethod));
			harmonyInstance.Patch(originalGetDigitalActionMethod, prefix: new HarmonyLib.HarmonyMethod(targetGetDigitalActionMethod));
		}

		public static bool GetAnalogActionDataPrefix(ref InputAnalogActionData_t __result, InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
		{
			if (GlobalInputHookHandler.Instance != null)
			{
				if (SettingsGUI.Display)
				{
					__result = new InputAnalogActionData_t();
					return false;
				}

				var replacement = GlobalInputHookHandler.Instance.GetAnalogInputReplacement(analogActionHandle);
				if (replacement == null)
					return true;

				__result = replacement.GetInput();
				return false;
			}
			else
				return true;
		}

		public static bool GetDigitalActionDataPrefix(ref InputDigitalActionData_t __result, InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle)
		{
			if (GlobalInputHookHandler.Instance != null)
			{
				if(SettingsGUI.Display)
				{
					__result = new InputDigitalActionData_t();
					return false;
				}

				var replacement = GlobalInputHookHandler.Instance.GetDigitalInputReplacement(digitalActionHandle);
				if (replacement == null)
					return true;

				__result = replacement.GetInput();
				return false;
			}
			else
				return true;
		}
	}
}
