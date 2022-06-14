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
			if (settings.Resolution != null)
			{
				var resolution = settings.Resolution.Value;
				SuisHackMain.loggerInst.Msg($"Setting screen resolution as: {resolution.X}x{resolution.Y}@{settings.RefreshRate}. Display mode: {settings.DisplayMode}");
				Screen.SetResolution(resolution.X, resolution.Y, settings.DisplayMode, settings.RefreshRate);
			}
			else
			{
				var mainScreen = Display.main;
				SuisHackMain.loggerInst.Msg($"No valid resolution provided - going with main screen resolution: {mainScreen.systemWidth}x{mainScreen.systemHeight}@{settings.RefreshRate}. Display mode: {settings.DisplayMode}");
				Screen.SetResolution(mainScreen.systemWidth, mainScreen.systemHeight, settings.DisplayMode, settings.RefreshRate);
			}
			QualitySettings.vSyncCount = settings.vSyncCount;
			SuisHackMain.loggerInst.Msg($"Setting vSyncCount: {QualitySettings.vSyncCount}");

			if (settings.vSyncCount == 0 && settings.RefreshRate > 0)
			{
				Application.targetFrameRate = settings.RefreshRate;
				SuisHackMain.loggerInst.Msg($"No vsync count, but a refresh rate is {settings.RefreshRate} - setting FPS cap!");
			}
			return false;
		}
	}
}
