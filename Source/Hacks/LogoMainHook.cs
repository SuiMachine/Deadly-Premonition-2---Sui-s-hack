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
			var Settings = SuisHackMain.Settings;

			QualitySettings.anisotropicFiltering = Settings.Entry_AnistropicFiltering.Value;
			Texture.SetGlobalAnisotropicFilteringLimits(Settings.Entry_AnistropicFilteringValue.Value, Settings.Entry_AnistropicFilteringValue.Value);
			PostProcessLayerHook.Antialiasing = Settings.Entry_Antialiasing.Value;
			PostProcessLayerHook.FarClipDistance = Settings.Entry_Quality_CameraFarPlaneDistance.Value;
			QualitySettings.lodBias = Settings.Entry_Quality_LODBias.Value;
			QualitySettings.pixelLightCount = Settings.Entry_Quality_PixelLightCount.Value;
			QualitySettings.shadowDistance = Settings.Entry_Quality_ShadowDistance.Value;
			QualitySettings.shadowCascade4Split = new Vector3(Settings.Entry_Quality_ShadowFourSplitValue1.Value, Settings.Entry_Quality_ShadowFourSplitValue2.Value, Settings.Entry_Quality_ShadowFourSplitValue3.Value);
			QualitySettings.shadows = Settings.Entry_Quality_ShadowsQuality.Value;
			QualitySettings.shadowResolution = Settings.Entry_Quality_ShadowsResolution.Value;
			QualitySettings.shadowCascade2Split = Settings.Entry_Quality_ShadowTwoSplitValue.Value;
			QualitySettings.masterTextureLimit = Settings.Entry_Quality_TextureQuality.Value;
			QualitySettings.shadowCascades = Settings.Entry_Quality_Use4ShadowCascades.Value ? 4 : 2;

			if (Settings.Entry_Other_SkipIntros.Value)
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
