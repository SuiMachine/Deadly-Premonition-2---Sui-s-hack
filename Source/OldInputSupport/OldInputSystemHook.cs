using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using UnhollowerBaseLib;
using UnityEngine;

namespace SuisHack.OldInputSupport
{
	[HarmonyPatch]
	public static class OldInputSystemHook
	{
		public static Dictionary<string, InputAnalogActionHandle_t> AnalogInputDictionary = new Dictionary<string, InputAnalogActionHandle_t>()
		{
			{ "None", new InputAnalogActionHandle_t(0) },
			{ "R_Stick", new InputAnalogActionHandle_t(1) },
			{ "L_Stick", new InputAnalogActionHandle_t(2) }
		};

		public static Dictionary<string, InputDigitalActionHandle_t> DigitalInputDictionary = new Dictionary<string, InputDigitalActionHandle_t>()
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

		public static void InitializeOldInput()
		{
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.Init))]
		public static bool Init(ref bool __result)
		{
			__result = true;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.Shutdown))]
		public static bool Shutdown(ref bool __result)
		{
			__result = true;
			return false;
		}


		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.RunFrame))]
		public static bool RunFrame()
		{
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetConnectedControllers))]
		public static bool GetConnectedControllers(ref int __result, Il2CppStructArray<InputHandle_t> handlesOut)
		{
			handlesOut = new Il2CppStructArray<InputHandle_t>(16);
			handlesOut[0] = new InputHandle_t(0451);
			for (int i=0; i<handlesOut.Count; i++)
			{
				handlesOut[i] = new InputHandle_t(0);
			}
			__result = 1;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetActionSetHandle))]
		public static bool GetActionSetHandle(ref InputActionSetHandle_t __result, string pszActionSetName)
		{
			__result = new InputActionSetHandle_t(1);
			return false;
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.ActivateActionSet))]
		public static void ActivateActionSet(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle)
		{
			SuisHackMain.loggerInst.Msg($"ActivateActionSet");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetCurrentActionSet))]
		public static void GetCurrentActionSet(ref InputActionSetHandle_t __result, InputHandle_t inputHandle)
		{
			SuisHackMain.loggerInst.Msg($"GetCurrentActionSet");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.ActivateActionSetLayer))]
		public static void ActivateActionSetLayer(InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
		{
			SuisHackMain.loggerInst.Msg($"ActivateActionSetLayer");
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
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetDigitalActionHandle))]
		public static bool GetDigitalActionHandle(ref InputDigitalActionHandle_t __result, string pszActionName)
		{
			__result = DigitalInputDictionary[pszActionName];
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetDigitalActionData))]
		public static bool GetDigitalActionData(ref InputDigitalActionData_t __result, InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle)
		{
			var button = digitalActionHandle.m_InputDigitalActionHandle + (ulong)(UnityEngine.KeyCode.Joystick1Button0 - 1);
			__result = new InputDigitalActionData_t()
			{
				bActive = 1,
				bState = Input.GetKey((KeyCode)button) ? (byte)1 : (byte)0
			};

			return false;
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetDigitalActionOrigins))]
		public static void GetDigitalActionOrigins(ref int __result, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputDigitalActionHandle_t digitalActionHandle, Il2CppStructArray<EInputActionOrigin> originsOut)
		{
			SuisHackMain.loggerInst.Msg($"GetDigitalActionOrigins");
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetAnalogActionHandle))]
		public static bool GetAnalogActionHandle(ref InputAnalogActionHandle_t __result, string pszActionName)
		{
			__result = AnalogInputDictionary[pszActionName];
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetAnalogActionData))]
		public static bool GetAnalogActionData(ref InputAnalogActionData_t __result, InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
		{
			switch(analogActionHandle.m_InputAnalogActionHandle)
			{
				case 0:
					__result = new InputAnalogActionData_t()
					{
						bActive = 1,
						eMode = EInputSourceMode.k_EInputSourceMode_JoystickMove,
						x = Input.GetAxis("Player0 Stick0 X"),
						y = Input.GetAxis("Player0 Stick0 Y")
					};
					break;
				case 1:
					__result = new InputAnalogActionData_t()
					{
						bActive = 1,
						eMode = EInputSourceMode.k_EInputSourceMode_JoystickMove,
						x = Input.GetAxis("Player0 Stick1 X"),
						y = Input.GetAxis("Player0 Stick1 Y")
					};
					break;
			}
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
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetStringForActionOrigin))]
		public static void GetStringForActionOrigin(ref string __result, EInputActionOrigin eOrigin)
		{
			SuisHackMain.loggerInst.Msg($"GetStringForActionOrigin");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.StopAnalogActionMomentum))]
		public static void StopAnalogActionMomentum(InputHandle_t inputHandle, InputAnalogActionHandle_t eAction)
		{
			SuisHackMain.loggerInst.Msg($"StopAnalogActionMomentum");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetMotionData))]
		public static void GetMotionData(ref InputMotionData_t __result, InputHandle_t inputHandle)
		{
			SuisHackMain.loggerInst.Msg($"GetMotionData");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.TriggerVibration))]
		public static void TriggerVibration(InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed)
		{
			SuisHackMain.loggerInst.Msg($"TriggerVibration");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.SetLEDColor))]
		public static void SetLEDColor(InputHandle_t inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags)
		{
			SuisHackMain.loggerInst.Msg($"SetLEDColor");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.TriggerHapticPulse))]
		public static void TriggerHapticPulse(InputHandle_t inputHandle, ESteamControllerPad eTargetPad, ushort usDurationMicroSec)
		{
			SuisHackMain.loggerInst.Msg($"TriggerHapticPulse");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.TriggerRepeatedHapticPulse))]
		public static void TriggerRepeatedHapticPulse(InputHandle_t inputHandle, ESteamControllerPad eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
		{
			SuisHackMain.loggerInst.Msg($"TriggerRepeatedHapticPulse");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.ShowBindingPanel))]
		public static void ShowBindingPanel(ref bool __result, InputHandle_t inputHandle)
		{
			SuisHackMain.loggerInst.Msg($"ShowBindingPanel");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetInputTypeForHandle))]
		public static void GetInputTypeForHandle(ref ESteamInputType __result, InputHandle_t inputHandle)
		{
			SuisHackMain.loggerInst.Msg($"GetInputTypeForHandle");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetControllerForGamepadIndex))]
		public static void GetControllerForGamepadIndex(ref InputHandle_t __result, int nIndex)
		{
			SuisHackMain.loggerInst.Msg($"GetControllerForGamepadIndex");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetGamepadIndexForController))]
		public static void GetGamepadIndexForController(ref int __result, InputHandle_t ulinputHandle)
		{
			SuisHackMain.loggerInst.Msg($"GetGamepadIndexForController - result: {__result}");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetStringForXboxOrigin))]
		public static void GetStringForXboxOrigin(ref string __result, EXboxOrigin eOrigin)
		{
			SuisHackMain.loggerInst.Msg($"GetStringForXboxOrigin - result: {__result}");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetGlyphForXboxOrigin))]
		public static void GetGlyphForXboxOrigin(ref string __result, EXboxOrigin eOrigin)
		{
			SuisHackMain.loggerInst.Msg($"GetGlyphForXboxOrigin - result: {__result}");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetActionOriginFromXboxOrigin))]
		public static void GetActionOriginFromXboxOrigin(ref EInputActionOrigin __result, InputHandle_t inputHandle, EXboxOrigin eOrigin)
		{
			SuisHackMain.loggerInst.Msg($"GetActionOriginFromXboxOrigin");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.TranslateActionOrigin))]
		public static void TranslateActionOrigin(ref EInputActionOrigin __result, ESteamInputType eDestinationInputType, EInputActionOrigin eSourceOrigin)
		{
			SuisHackMain.loggerInst.Msg($"TranslateActionOrigin");
		}

		//[HarmonyPostfix]
		//[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.TranslateActionOrigin))]
		//public static void GetDeviceBindingRevision(ref bool __result, InputHandle_t inputHandle, out int pMajor, out int pMinor)
		//{
		//}


	}
}
