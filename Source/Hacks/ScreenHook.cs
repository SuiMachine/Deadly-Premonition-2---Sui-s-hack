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
			var clampedRefresh = Mathf.Clamp(settings.Entry_Display_RefreshRate.Value, 30, 480);
			if (settings.Resolution != null)
			{
				var resolution = settings.Resolution.Value;
				SuisHackMain.loggerInst.Msg($"Setting screen resolution as: {resolution.X}x{resolution.Y}@{clampedRefresh}. Display mode: {settings.Entry_Display_DisplayMode.Value}");
				Screen.SetResolution(resolution.X, resolution.Y, settings.Entry_Display_DisplayMode.Value, clampedRefresh);
			}
			else
			{
				var mainScreen = Display.main;
				SuisHackMain.loggerInst.Msg($"No valid resolution provided - going with main screen resolution: {mainScreen.systemWidth}x{mainScreen.systemHeight}@{clampedRefresh}. Display mode: {settings.Entry_Display_DisplayMode.Value}");
				Screen.SetResolution(mainScreen.systemWidth, mainScreen.systemHeight, settings.Entry_Display_DisplayMode.Value, clampedRefresh);
			}
			QualitySettings.vSyncCount = settings.Entry_Display_Vsync.Value ? 1 : 0;
			SuisHackMain.loggerInst.Msg($"Setting vSyncCount: {QualitySettings.vSyncCount}");

			if (QualitySettings.vSyncCount == 0)
			{
				Application.targetFrameRate = settings.Entry_Display_RefreshRate.Value;
				SuisHackMain.loggerInst.Msg($"No vsync count, but a refresh rate is {settings.Entry_Display_RefreshRate.Value} - setting FPS cap!");
			}
			return false;
		}
	}
}
