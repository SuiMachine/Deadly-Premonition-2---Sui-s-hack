using HarmonyLib;
using Il2Cpp;

namespace SuisHack.GamepadSupport
{
	[HarmonyPatch]
	public class GamepadRumble
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Gun), nameof(Gun.Fire))]
		public static void FirePostfix()
		{
			Components.VibrationController.Instance?.TriggleRumble(60000, 0.15f);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Gun), nameof(Gun.ChargeShot))]
		public static void ChargeShot()
		{
			Components.VibrationController.Instance?.TriggleRumble(ushort.MaxValue, 0.20f);
		}


		[HarmonyPostfix]
		[HarmonyPatch(typeof(Gun), nameof(Gun.ChargeShotNoTarget))]
		public static void ChargeShotNoTarget()
		{
			Components.VibrationController.Instance?.TriggleRumble(ushort.MaxValue, 0.20f);
		}

		/*		[HarmonyPostfix]
				[HarmonyPatch(typeof(Gun), nameof(Gun.DispChargeFlash))]
				public static void DispChargeFlash()
				{
					Components.VibrationController.Instance?.TriggerChargeRumble(1000, 40000, 3f);
					//Components.VibrationController.Instance?.TriggleRumble(60000, 0.15f);
				}*/

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Gun), nameof(Gun.UnDispChargeFlash))]
		public static void UnDispChargeFlash()
		{
			Components.VibrationController.Instance?.CancelRumble();
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Gun), nameof(Gun.CancelChargeFlash))]
		public static void CancelChargeFlash()
		{
			Components.VibrationController.Instance?.CancelRumble();
		}
	}
}
