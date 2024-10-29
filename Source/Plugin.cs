using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using SuisHack.Hacks.StateStracking;
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

		RegisterTypesForIll2cpp();

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
		HarmonyInstance.PatchAll();
		InitializeManualHarmonyHooks();

		Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} has finished loading!");
	}

	private void InitializeManualHarmonyHooks()
	{

	}

	private void RegisterTypesForIll2cpp()
	{
		ClassInjector.RegisterTypeInIl2Cpp<SettingsGUI>();
		ClassInjector.RegisterTypeInIl2Cpp<MapUIManagerTracking>();
		ClassInjector.RegisterTypeInIl2Cpp<RedRoomTracking>();		
	}

	public override bool Unload()
	{
		return false;
	}

	public static void Error(string errorTest) => m_Logger.LogError(errorTest);
	public static void Message(string message) => m_Logger.LogMessage(message);
	public static void Warning(string warning) => m_Logger.LogWarning(warning);
}
