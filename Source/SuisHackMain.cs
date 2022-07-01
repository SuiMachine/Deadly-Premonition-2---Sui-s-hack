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
		private bool AppliedResolutionInMainMenu;

		public override void OnApplicationStart()
		{
			loggerInst = LoggerInstance;
			harmonyInst = HarmonyInstance;
			LoggerInstance.Msg("Loading Sui's Hack loaded");
			Settings = new ExposedSettings();
			switch(Settings.Input_Override.Value)
			{
				case ExposedSettings.InputType.Original:
					KeyboardSupport.KeyboardPrompts.Initialize();
					KeyboardSupport.SteamInputHook.InitializeKeyboardAndMouse();
					break;
				case ExposedSettings.InputType.KeyboardAndMouse:
					GamepadSupport.GamepadPrompts.Initialize();
					break;
			}

			base.OnApplicationStart();
		}

		public override void OnApplicationLateStart()
		{
			base.OnApplicationLateStart();

			if (Settings.Input_Override.Value == ExposedSettings.InputType.KeyboardAndMouse)
			{
				KeyboardSupport.GlobalInputHookHandler.Initialize();
			}

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

				if(sceneName == "TitleTest2")
				{
					if(!AppliedResolutionInMainMenu)
						Hacks.ScreenHook.SetResolution1();
					AppliedResolutionInMainMenu = true;
				}
				else if (sceneName == OPENWORLDSCENENAME)
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
