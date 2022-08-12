using Steamworks;
using System.Collections.Generic;

namespace SuisHack.KeyboardSupport
{
	public static class SteamInputHook
	{
		public enum RebindingActions
		{
			None,
			Forward,
			Backward,
			Left,
			Right,
			Display_Map,
			Point_Gun,
			Vision,
			Crouch,
			SwitchSlotLeft,
			SwitchSlotRight,
			SwitchAlbumDisplayUp,
			SwitchAlbumDisplayDown,
			Quest_Display,
			Fire_Weapon_Punch,
			Dash_Dodge,
			Interact_Reload_Accellerate,
			Cancel_Brake,
			Skateboard,
			Reset_Camera_Fighting_Style
		}

		public enum SteamInputAnalog
		{
			None,
			L_Stick,
			R_Stick,
		}

		public enum SteamInputDigital
		{
			None,
			A_Button,
			B_Button,
			X_Button,
			Y_Button,
			Back_Button,
			Start_Button,
			Up_Button,
			Down_Button,
			Left_Button,
			Right_Button,
			LB,
			RB,
			LT,
			RT,
			L_Stick_Button,
			R_Stick_Button,
		}

		public readonly static Dictionary<string, InputAnalogActionHandle_t> AnalogInputDictionary = new Dictionary<string, InputAnalogActionHandle_t>()
		{
			{ "None", new InputAnalogActionHandle_t((int)SteamInputAnalog.None) },
			{ "L_Stick", new InputAnalogActionHandle_t((int)SteamInputAnalog.L_Stick) },
			{ "R_Stick", new InputAnalogActionHandle_t((int)SteamInputAnalog.R_Stick) },
		};

		public readonly static Dictionary<string, InputDigitalActionHandle_t> DigitalInputDictionary = new Dictionary<string, InputDigitalActionHandle_t>()
		{
			{ "None", new InputDigitalActionHandle_t((int)SteamInputDigital.None) },
			{ "A_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.A_Button) },
			{ "B_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.B_Button) },
			{ "X_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.X_Button) },
			{ "Y_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.Y_Button) },
			{ "Back_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.Back_Button) },
			{ "Start_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.Start_Button) },
			{ "Up_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.Up_Button) },
			{ "Down_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.Down_Button) },
			{ "Left_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.Left_Button) },
			{ "Right_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.Right_Button) },
			{ "LB", new InputDigitalActionHandle_t((int)SteamInputDigital.LB) },
			{ "RB", new InputDigitalActionHandle_t((int)SteamInputDigital.RB) },
			{ "LT", new InputDigitalActionHandle_t((int)SteamInputDigital.LT) },
			{ "RT", new InputDigitalActionHandle_t((int)SteamInputDigital.RT) },
			{ "L_Stick_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.L_Stick_Button) },
			{ "R_Stick_Button", new InputDigitalActionHandle_t((int)SteamInputDigital.R_Stick_Button) },
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

			__result = GlobalInputHookHandler.Instance.GetAnalogInputReplacement((SteamInputAnalog)analogActionHandle.m_InputAnalogActionHandle);
			return false;
		}

		public static bool GetDigitalActionDataPrefix(ref InputDigitalActionData_t __result, InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle)
		{
			if (SettingsGUI.Display)
			{
				__result = new InputDigitalActionData_t();
				return false;
			}

			__result = GlobalInputHookHandler.Instance.GetDigitalInputReplacement((SteamInputDigital)digitalActionHandle.m_InputDigitalActionHandle);

			return false;
		}
	}
}
