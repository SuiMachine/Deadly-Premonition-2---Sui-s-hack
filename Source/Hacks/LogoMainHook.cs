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
