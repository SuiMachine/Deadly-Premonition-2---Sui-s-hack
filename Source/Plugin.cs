using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using SuisHack.Components;
using SuisHack.Components.Interpolation;
using SuisHack.GlobalGameObjects;
using SuisHack.Hacks.StateStracking;
using SuisHack.KeyboardSupport;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuisHack;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
	private const string OPENWORLDSCENENAME = "OpenWorld";
	public static Harmony HarmonyInstance { get; private set; }
	private static BepInEx.Logging.ManualLogSource m_Logger;
	public static ExposedSettings Settings;

	private bool AppliedResolutionInMainMenu;


	public override void Load()
	{
		Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} starting to load - report problems if there are errors between this message and notification that it finished loading!");
		HarmonyInstance = new Harmony("local.suimachine.suihack");
		m_Logger = Log;
		Settings = new ExposedSettings(Config);

		RegisterTypesForIll2cpp();

		switch (Settings.Input_Override.Value)
		{
			case ExposedSettings.InputType.SteamInput:
				GamepadSupport.GamepadPrompts.Initialize();
				break;
			case ExposedSettings.InputType.KeyboardAndMouse:
				KeyboardPrompts.Initialize();
				SteamInputHook.InitializeKeyboardAndMouse();
				break;
		}

		Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is initializing hacks!");

		if (Settings.Input_Override.Value == ExposedSettings.InputType.KeyboardAndMouse)
		{
			GlobalInputHookHandler.Initialize();
		}

		SettingsGUI.Initialize();
		HarmonyInstance.PatchAll();
		InitializeManualHarmonyHooks();

		Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} has finished loading!");
		SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, loadMode) =>
		{
			var sceneName = scene.name;

			if (Settings != null)
			{
				Application.targetFrameRate = Settings.Entry_DesiredFramerate.Value;

				if (sceneName == "TitleTest2")
				{
					if (!AppliedResolutionInMainMenu)
						Hacks.ScreenHook.SetResolution1();
					AppliedResolutionInMainMenu = true;
					GameStateMachine.MainMenu = true;
				}
				else if (sceneName == OPENWORLDSCENENAME)
				{
					GameStateMachine.Gameplay = true;
					if (Settings.Entry_Other_GeometryImprovements.Value >= ExposedSettings.GeometryImprovements.Enabled)
					{
						if (GameObject.FindObjectOfType<Components.GlobalGeometryChecker>() == null)
						{
							var oldActiveScene = SceneManager.GetActiveScene();
							SceneManager.SetActiveScene(scene);
							var newGameObject = new GameObject("WireRendererCorrection");
							SceneManager.SetActiveScene(oldActiveScene);
							newGameObject.AddComponent<Components.GlobalGeometryChecker>();
						}
					}
				}
				else
				{
					GameStateMachine.Gameplay = true;
					LightImprovement.ModifyLights.ModifyOnSceneLoad(sceneName.ToLower());
				}
			}
		}));
	}

	private void InitializeManualHarmonyHooks()
	{
		GlobalReplacementAtlas.Initialize();
		Hacks.NpcTestHook.Initialize();
		Hacks.Lights.LightActiveCheckHook.Initialize();
		Hacks.Lights.NpcVehicleHook.Initialize();
	}

	private void RegisterTypesForIll2cpp()
	{
		ClassInjector.RegisterTypeInIl2Cpp<SettingsGUI>();
		ClassInjector.RegisterTypeInIl2Cpp<MapUIManagerTracking>();
		ClassInjector.RegisterTypeInIl2Cpp<RedRoomTracking>();
		ClassInjector.RegisterTypeInIl2Cpp<GlobalReplacementAtlas>();
		ClassInjector.RegisterTypeInIl2Cpp<GlobalGeometryChecker>();
		ClassInjector.RegisterTypeInIl2Cpp<TerrainCorrectionData>();
		ClassInjector.RegisterTypeInIl2Cpp<BoatFollowInterpolation>();
		ClassInjector.RegisterTypeInIl2Cpp<GameObjectInterpolation>();
		ClassInjector.RegisterTypeInIl2Cpp<SmootherController>();
		ClassInjector.RegisterTypeInIl2Cpp<WireRendererCorrection>();
		ClassInjector.RegisterTypeInIl2Cpp<GlobalInputHookHandler>();
	}

	public override bool Unload()
	{
		return false;
	}

	public static void Error(string errorTest) => m_Logger.LogError(errorTest);
	public static void Message(string message) => m_Logger.LogMessage(message);
	public static void Warning(string warning) => m_Logger.LogWarning(warning);
}
