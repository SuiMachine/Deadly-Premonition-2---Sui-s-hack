using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuisHack
{
	public class SuisHackMain : MelonMod
	{
		private const string OPENWORLDSCENENAME = "OpenWorld";
		public static HarmonyLib.Harmony harmonyInst { get; private set; }
		public static MelonLogger.Instance loggerInst { get; private set; }
		public static ExposedSettings Settings;

		public override void OnApplicationLateStart()
		{
			base.OnApplicationLateStart();
			LoggerInstance.Msg("Loading Sui's Hack loaded");
			loggerInst = LoggerInstance;
			harmonyInst = HarmonyInstance;
			Settings = new ExposedSettings();
			if (Settings.Input_Override.Value == ExposedSettings.InputType.KeyboardAndMouse)
				Hacks.SteamInputHook.InitializeKeyboardAndMouse();

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
				if(Settings.Input_Override.Value == ExposedSettings.InputType.KeyboardAndMouse)
					GlobalGameObjects.GlobalInputHookHandler.Initialize();

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
	}
}
