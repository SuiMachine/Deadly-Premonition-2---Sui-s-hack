using HarmonyLib;
using Steamworks;
using System.Collections.Generic;
using UnhollowerBaseLib;

namespace SuisHack.KeyboardSupport
{
	[HarmonyPatch]
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

			harmonyInstance.Patch(originalGetAnalogActionMethod, prefix: new HarmonyLib.HarmonyMethod(targetGetAnalogActionMethod));
			harmonyInstance.Patch(originalGetDigitalActionMethod, prefix: new HarmonyLib.HarmonyMethod(targetGetDigitalActionMethod));
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetControllerForGamepadIndex))]
		public static void GetControllerForGamepadIndex(ref InputHandle_t __result, int nIndex)
		{
			SuisHackMain.loggerInst.Error($"GetControllerForGamepadIndex: {__result.m_InputHandle} - {nIndex}");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetGamepadIndexForController))]
		public static void GetGamepadIndexForController(ref int __result, InputHandle_t ulinputHandle)
		{
			SuisHackMain.loggerInst.Error($"GetGamepadIndexForController: {__result} - {ulinputHandle.m_InputHandle}");
		}


		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetInputTypeForHandle))]
		public static void GetInputTypeForHandle(ref ESteamInputType __result, InputHandle_t inputHandle)
		{
			SuisHackMain.loggerInst.Error($"GetInputTypeForHandle: {__result} - {inputHandle.m_InputHandle}");
		}


		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetMotionData))]
		public static void GetMotionData(ref InputMotionData_t __result, InputHandle_t inputHandle)
		{
			SuisHackMain.loggerInst.Error($"GetMotionData VAL: {inputHandle.m_InputHandle}");
		}


		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetStringForActionOrigin))]
		public static void GetStringForActionOrigin(ref string __result, EInputActionOrigin eOrigin)
		{
			SuisHackMain.loggerInst.Error($"GetStringForActionOrigin {__result}: {eOrigin}");
		}


		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetActionSetHandle))]
		public static bool GetActionSetHandle(ref InputActionSetHandle_t __result, string pszActionSetName)
		{
			SuisHackMain.loggerInst.Error($"GetActionSetHandle {pszActionSetName}");

			__result = new InputActionSetHandle_t(1);
			return false;
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetCurrentActionSet))]
		public static void GetCurrentActionSet(ref InputActionSetHandle_t __result, InputHandle_t inputHandle)
		{
			SuisHackMain.loggerInst.Error($"GetCurrentActionSet");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.DeactivateActionSetLayer))]
		public static void DeactivateActionSetLayer(InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
		{
			SuisHackMain.loggerInst.Msg($"DeactivateActionSetLayer");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.DeactivateAllActionSetLayers))]
		public static void DeactivateAllActionSetLayers(InputHandle_t inputHandle)
		{
			SuisHackMain.loggerInst.Msg($"DeactivateAllActionSetLayers");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetActiveActionSetLayers))]
		public static void GetActiveActionSetLayers(ref int __result, InputHandle_t inputHandle, Il2CppStructArray<InputActionSetHandle_t> handlesOut)
		{
			SuisHackMain.loggerInst.Msg($"GetActiveActionSetLayers");
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetAnalogActionHandle))]
		public static bool GetAnalogActionHandle(ref InputAnalogActionHandle_t __result, string pszActionName)
		{
			__result = AnalogInputDictionary[pszActionName];
			return false;
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetAnalogActionOrigins))]
		public static void GetAnalogActionOrigins(ref int __result, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputAnalogActionHandle_t analogActionHandle, Il2CppStructArray<EInputActionOrigin> originsOut)
		{
			SuisHackMain.loggerInst.Msg($"GetAnalogActionOrigins");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetGlyphForActionOrigin))]
		public static void GetGlyphForActionOrigin(ref string __result, EInputActionOrigin eOrigin)
		{
			SuisHackMain.loggerInst.Msg($"GetGlyphForActionOrigin");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetDigitalActionOrigins))]
		public static void GetDigitalActionOrigins(ref int __result, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputDigitalActionHandle_t digitalActionHandle, Il2CppStructArray<EInputActionOrigin> originsOut)
		{
			SuisHackMain.loggerInst.Msg($"GetDigitalActionOrigins");
		}

		public static bool GetAnalogActionDataPrefix(ref InputAnalogActionData_t __result, InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
		{
			SuisHackMain.loggerInst.Msg($"Get input - {analogActionHandle.m_InputAnalogActionHandle}");

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

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetDigitalActionHandle))]
		public static bool GetDigitalActionHandle(ref InputDigitalActionHandle_t __result, string pszActionName)
		{
			__result = DigitalInputDictionary[pszActionName];
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
