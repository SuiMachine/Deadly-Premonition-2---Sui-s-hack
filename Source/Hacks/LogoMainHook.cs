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
			QualitySettings.pixelLightCount = settings.Entry_Quality_PixelLightCount.Value;
			QualitySettings.lodBias = settings.Entry_Quality_LODBias.Value;
			QualitySettings.realtimeReflectionProbes = settings.Entry_Quality_RealtimeReflectionProbes.Value;
			QualitySettings.masterTextureLimit = settings.Entry_Quality_TextureQuality.Value;

			if(settings.Entry_Other_SkipIntros.Value)
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
