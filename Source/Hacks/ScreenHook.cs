using HarmonyLib;
using System;
using UnityEngine;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class ScreenHook
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Screen), "SetResolution", new Type[] { typeof(int), typeof(int), typeof(bool) })]
		public static bool SetResolution1()
		{
			var settings = SuisHackMain.Settings;
			if (settings == null)
				return true;

			var refreshRate = settings.Entry_Display_RefreshRate.Value;
			if (settings.Resolution != null)
			{
				var resolution = settings.Resolution;
				SuisHackMain.loggerInst.Msg($"Setting screen resolution as: {resolution.Item1}x{resolution.Item2}@{refreshRate}. Display mode: {settings.Entry_Display_DisplayMode.Value}");
				Screen.SetResolution(resolution.Item1, resolution.Item2, settings.Entry_Display_DisplayMode.Value, refreshRate);
			}
			else
			{
				var mainScreen = Display.main;
				SuisHackMain.loggerInst.Msg($"No valid resolution provided - going with main screen resolution: {mainScreen.systemWidth}x{mainScreen.systemHeight}@{refreshRate}. Display mode: {settings.Entry_Display_DisplayMode.Value}");
				Screen.SetResolution(mainScreen.systemWidth, mainScreen.systemHeight, settings.Entry_Display_DisplayMode.Value, refreshRate);
			}
			QualitySettings.vSyncCount = settings.Entry_Display_Vsync.Value ? 1 : 0;
			SuisHackMain.loggerInst.Msg($"Setting vSyncCount: {QualitySettings.vSyncCount}");
			return false;
		}
	}
}
