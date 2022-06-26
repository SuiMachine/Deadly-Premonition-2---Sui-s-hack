using MelonLoader;
using System;
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
			InitializeManualHarmonyHooks();

			LoggerInstance.Msg("Sui's Hack loaded");
		}

		private void InitializeManualHarmonyHooks()
		{
			GlobalGameObjects.GlobalReplacementAtlas.Initialize();
			Hacks.NpcTestHook.Initialize();
			Hacks.Lights.LightActiveCheckHook.Initialize();
			Hacks.Lights.NpcVehicleHook.Initialize();
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			base.OnSceneWasLoaded(buildIndex, sceneName);
			if (Settings != null)
			{
				Application.targetFrameRate = Settings.Entry_DesiredFramerate.Value;
				if (Settings.Input_Override.Value == ExposedSettings.InputType.KeyboardAndMouse)
					GlobalGameObjects.GlobalInputHookHandler.Initialize();

				if (sceneName == OPENWORLDSCENENAME)
				{
					if (Settings.Entry_Other_GeometryImprovements.Value >= ExposedSettings.GeometryImprovements.Minor)
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
				else
				{
					LightImprovement.ModifyLights.ModifyOnSceneLoad(sceneName.ToLower());
				}
			}
		}
	}
}
