using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class LogoMainHook
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(LogoMain), "Start")]
		public static void StartPostfix(LogoMain __instance)
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

			QualitySettings.pixelLightCount = settings.PixelLightCount;
			QualitySettings.lodBias = settings.LOD_Bias;
			QualitySettings.realtimeReflectionProbes = settings.RealtimeReflectionProbes;
			QualitySettings.masterTextureLimit = settings.TextureQuality;

			if(settings.SkipIntros)
				MelonCoroutines.Start(StartMenu(__instance.gameObject.scene));
			SuisHackMain.loggerInst.Msg($"Done applying settings!");

		}

		private static System.Collections.IEnumerator StartMenu(UnityEngine.SceneManagement.Scene scene)
		{
			yield return null;
			yield return null;
			yield return null;
			//Just to be extra sure stuff is loaded
			UnityEngine.SceneManagement.SceneManager.LoadScene(5, UnityEngine.SceneManagement.LoadSceneMode.Single);
			SuisHackMain.loggerInst.Msg($"Skipped scene");
		}
	}
}
