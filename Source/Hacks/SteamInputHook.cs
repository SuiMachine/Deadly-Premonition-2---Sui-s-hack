using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Steamworks;
using SuisHack.GlobalGameObjects;
using UnhollowerBaseLib;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class SteamInputHook
	{
		/*
				[HarmonyPostfix]
				[HarmonyPatch(typeof(SteamInput), "GetConnectedControllers")]
				public static void GetConnectedControllersHook(Il2CppStructArray<InputHandle_t> handlesOut)
				{
					if(!SpewedNonsense)
					{
						SpewedNonsense = true;
						SuisHackMain.loggerInst.Error($"Controllers: {handlesOut.Length}");
						for (int i = 0; i <handlesOut.Length; i++)
						{
							SuisHackMain.loggerInst.Error($"Controller: {handlesOut[i].m_InputHandle}");
						}
					}
				}*/

/*		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), "GetAnalogActionData")]
		public static void GetAnalogActionDataPostfix(InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
		{
			
		}*/



		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamInput), "GetAnalogActionData")]
		public static bool GetAnalogActionDataPostfix(ref InputAnalogActionData_t __result, InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
		{
			if (GlobalInputHookHandler.Instance != null)
			{
				var replacement = GlobalInputHookHandler.Instance.GetAnalogInputReplacement(analogActionHandle);
				if (replacement == null)
					return true;
				else
				{
					__result = replacement.GetInput();
					return false;
				}
			}
			else
				return true;
		}

	}
}
