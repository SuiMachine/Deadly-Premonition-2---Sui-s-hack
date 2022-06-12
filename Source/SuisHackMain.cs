using MelonLoader;
using UnityEngine;

namespace SuisHack
{
	public class SuisHackMain : MelonMod
    {
        MelonPreferences_Category mainDisplayCategory;
		MelonPreferences_Category graphicsSettingsCategory;
		MelonPreferences_Category otherSettingsCategory;


		public static HarmonyLib.Harmony harmonyInst;
		public static MelonLogger.Instance loggerInst;
		public static ExposedSettings Settings;

		public override void OnApplicationLateStart()
		{
			mainDisplayCategory = MelonPreferences.CreateCategory("Suis Hack Main Display");
			graphicsSettingsCategory = MelonPreferences.CreateCategory("Suis Hack Graphics Settings");
			otherSettingsCategory = MelonPreferences.CreateCategory("Suis Hack Other Settings");
			LoggerInstance.Msg("Loading Sui's Hack loaded");
			base.OnApplicationLateStart();
			loggerInst = LoggerInstance;
			ApplySettings();
			LoggerInstance.Msg("Sui's Hack loaded");
		}

		public override void OnSceneWasInitialized(int buildIndex, string sceneName)
		{
			SettingsGUI.Initialize();
			GlobalGameObjects.GlobalReplacementAtlas.Initialize();
			base.OnSceneWasInitialized(buildIndex, sceneName);
		}

		public void ApplySettings()
		{
			Settings = new ExposedSettings();
			if(Hacks.ConfigParsing.ParseResolution(mainDisplayCategory.CreateEntry("Resolution", "0x0", description: "Screen or game resolution depending on display mode - if invalid resolution is specified - main screen resolution is used").Value, out (uint X, uint Y) desiredResolution))
				Settings.Resolution = ((int)desiredResolution.X, (int)desiredResolution.Y);

			Settings.DisplayMode = mainDisplayCategory.CreateEntry<FullScreenMode>("Mode", FullScreenMode.FullScreenWindow, description: "Unity's display mode. Options are: ExclusiveFullScreen / FullScreenWindow / MaximizedWindow / Windowed").Value;
			Settings.RefreshRate = (int)mainDisplayCategory.CreateEntry<uint>("Refresh_Rate", 0, description: "Refresh rate used in fullscreen. If Vsync is disabled, this is used for FPS cap.").Value;
			Settings.vSyncCount = mainDisplayCategory.CreateEntry("Vsync", true, description: "Enable vSync. True by default. If this is false and refresh rate is not 0, FPS cap is used.").Value ? 1 : 0;

			Settings.LOD_Bias = Mathf.Clamp(graphicsSettingsCategory.CreateEntry("LODBias", 2f, description: "LOD Bias - affects how far from camera the LOD changes - bigger values, push the LOD change further from camera - min. 0.5, max. 4.0. Default game value is 2.0. Originally it was probably 1.0 on Nintendo Switch.").Value, 0.5f, 4f);
			Settings.PixelLightCount = Mathf.Clamp(graphicsSettingsCategory.CreateEntry("PixelLightCount", 4, description: "Pixel Light Count - Default is 4. Affects the maximum number of pixel lights that should affect any object. If there are more lights illuminating an object, the dimmest ones will be rendered as vertex lights.").Value, 1, 4);
			Settings.TextureQuality = Mathf.Clamp(graphicsSettingsCategory.CreateEntry("Texture quality", 0, description: "Texture quality - default is 0. Higher values cause lower mip maps to be used. Can be used to reduce VRAM usage on low-end devices. 1 will cause half of the original resolution to be used, 2 will be 1/4 etc. 3 is 1/8th of original resolution.").Value, 0, 3);
			Settings.RealtimeReflectionProbes = graphicsSettingsCategory.CreateEntry("RealTimeReflectionProbes", true, description: "Allows usage of realtime reflection probes. Not sure if it used in game at all. Default is true.").Value;

			Settings.SkipIntros = otherSettingsCategory.CreateEntry("Skip intros", true, description: "Allows to skip splash screns / intros and go straight to menu! Disable in case of issues").Value;
			Settings.prompts = otherSettingsCategory.CreateEntry("Controller Prompts", ControllerPrompts.Switch, description: "Controller prompts displayed to a player. Options are Switch / Xbox / PlayStation. Make sure to disable Steam Input rebinding!").Value;
		}
	}
}
