using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuisHack
{
	public class SuisHackMain : MelonMod
	{
		private const string OPENWORLDSCENENAME = "OpenWorld";
		public static HarmonyLib.Harmony harmonyInst;
		public static MelonLogger.Instance loggerInst;
		public static ExposedSettings Settings;

		public override void OnApplicationLateStart()
		{
			LoggerInstance.Msg("Loading Sui's Hack loaded");
			base.OnApplicationLateStart();
			loggerInst = LoggerInstance;
			harmonyInst = HarmonyInstance;
			ApplySettings();
			SettingsGUI.Initialize();
			GlobalGameObjects.GlobalReplacementAtlas.Initialize();
			LoggerInstance.Msg("Sui's Hack loaded");
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			base.OnSceneWasLoaded(buildIndex, sceneName);
			if (Settings != null)
			{
				Application.targetFrameRate = Settings.Entry_DesiredFramerate.Value;

				if (Settings.Entry_Other_FixGeometry.Value && sceneName == OPENWORLDSCENENAME)
				{
					if (GameObject.FindObjectOfType<Components.WireRenderCorrectionChecker>() == null)
					{
						var scene = SceneManager.GetSceneByName(OPENWORLDSCENENAME);
						var oldActiveScene = SceneManager.GetActiveScene();
						SceneManager.SetActiveScene(scene);
						var newGameObject = new GameObject("WireRendererCorrection");
						SceneManager.SetActiveScene(oldActiveScene);
						newGameObject.AddComponent<Components.WireRenderCorrectionChecker>();
					}
				}
			}
		}

		public void ApplySettings()
		{
			Settings = new ExposedSettings();
		}
	}
}
