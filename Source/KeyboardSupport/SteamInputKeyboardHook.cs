using HarmonyLib;
using Steamworks;
using System;
using UnhollowerBaseLib;

namespace SuisHack.KeyboardSupport
{
	[HarmonyPatch]
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

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.Init))]
		public static bool InitPrefix(ref bool __result)
		{
			__result = true;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.Shutdown))]
		public static bool ShutdownPrefix(ref bool __result)
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
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetDigitalActionHandle))]
		public static bool GetDigitalActionHandle(ref InputDigitalActionHandle_t __result, string pszActionName)
		{
			var parse = (GlobalInputHookHandler.SteamInputDigital)Enum.Parse(typeof(GlobalInputHookHandler.SteamInputDigital), pszActionName);

			__result = new InputDigitalActionHandle_t()
			{
				m_InputDigitalActionHandle = (ulong)parse
			};
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetAnalogActionHandle))]
		public static bool GetAnalogActionHandle(ref InputAnalogActionHandle_t __result, string pszActionName)
		{
			var parse = (GlobalInputHookHandler.SteamInputAnalog)Enum.Parse(typeof(GlobalInputHookHandler.SteamInputAnalog), pszActionName);
			__result = new InputAnalogActionHandle_t()
			{
				m_InputAnalogActionHandle = (ulong)parse
			};
			return false;
		}


		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetActionSetHandle))]
		public static bool GetActionSetHandle(ref InputActionSetHandle_t __result, string pszActionSetName)
		{
			SuisHackMain.loggerInst.Msg("Get ActionSetHandle: " + pszActionSetName);
			return false;
		}


		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), nameof(SteamInput.GetConnectedControllers))]
		public static bool GetConnectedControllers(ref int __result, Il2CppStructArray<InputHandle_t> handlesOut)
		{
			handlesOut = new Il2CppStructArray<InputHandle_t>(1);
			handlesOut[0] = new InputHandle_t()
			{
				m_InputHandle = 0
			};
			__result = 1;
			return false;
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
