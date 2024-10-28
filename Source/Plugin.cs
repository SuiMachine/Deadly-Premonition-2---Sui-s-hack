﻿using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;

namespace SuisHack;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
	private const string OPENWORLDSCENENAME = "OpenWorld";
	public static Harmony HarmonyInstance { get; private set; }
	private static BepInEx.Logging.ManualLogSource m_Logger;
	public static ExposedSettings Settings;
	//		public static ExposedSettings? Settings;

	private bool AppliedResolutionInMainMenu;


	public override void Load()
    {
		Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} starting to load - report problems if there are errors between this message and notification that it finished loading!");
		HarmonyInstance = new Harmony("local.suimachine.suihack");
		m_Logger = Log;
		Settings = new ExposedSettings(Config);
		switch (Settings.Input_Override.Value)
		{
			case ExposedSettings.InputType.SteamInput:
				//GamepadSupport.GamepadPrompts.Initialize();
				break;
			case ExposedSettings.InputType.KeyboardAndMouse:
				//KeyboardSupport.KeyboardPrompts.Initialize();
				//KeyboardSupport.SteamInputHook.InitializeKeyboardAndMouse();
				break;
		}

		Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is initializing hacks!");

		if (Settings.Input_Override.Value == ExposedSettings.InputType.KeyboardAndMouse)
		{
			//KeyboardSupport.GlobalInputHookHandler.Initialize();
		}

		SettingsGUI.Initialize();
		//InitializeManualHarmonyHooks();

		Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} has finished loading!");


	}


	public static void Error(string errorTest) => m_Logger.LogError(errorTest);
}
