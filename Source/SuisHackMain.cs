using MelonLoader;
using UnityEngine;

namespace SuisHack
{
	public class SuisHackMain : MelonMod
    {
        MelonPreferences_Category category;
		public static HarmonyLib.Harmony harmonyInst;
		public static MelonLogger.Instance loggerInst;
		public static ExposedSettings Settings;

		public override void OnApplicationLateStart()
		{
			category = MelonPreferences.CreateCategory("Suis Hack");
			base.OnApplicationLateStart();
			loggerInst = LoggerInstance;
			ApplySettings();
			harmonyInst = new HarmonyLib.Harmony("local.suishack.suicidemachine");
			harmonyInst.PatchAll();
			LoggerInstance.Msg("Dupa loaded");
		}

		public override void OnSceneWasInitialized(int buildIndex, string sceneName)
		{
			SettingsGUI.Initialize();
			base.OnSceneWasInitialized(buildIndex, sceneName);
		}

		public void ApplySettings()
		{
			Settings = new ExposedSettings();
			if(Hacks.ConfigParsing.ParseResolution(category.CreateEntry("Resolution", "0x0", description: "Screen or game resolution depending on display mode").Value, out (uint X, uint Y) desiredResolution))
			{
				var mode = category.CreateEntry<FullScreenMode>("Mode", FullScreenMode.Windowed, description: "Unity's display mode. Options are: ExclusiveFullScreen / FullScreenWindow / MaximizedWindow / Windowed");
				var refreshRate = category.CreateEntry<uint>("Refresh_Rate", 0, description: "Refresh rate used in fullscreen. If Vsync is disabled, this is used for FPS cap.");
				LoggerInstance.Msg($"Setting screen resolution as: {desiredResolution.X}x{desiredResolution.Y}@{refreshRate.Value}. Display mode: {mode}");
				Settings.Resolution = ((int)desiredResolution.X, (int)desiredResolution.Y);
				Settings.RefreshRate = (int)refreshRate.Value;
				Settings.DisplayMode = mode.Value;
			}

			Settings.LOD_Bias = Mathf.Clamp(category.CreateEntry("LODBias", 2f, description: "LOD Bias - affects how far from camera the LOD changes - bigger values, push the LOD change further from camera - min. 0.5, max. 4.0. Default game value is 2.0. Originally it was probably 1.0 on Nintendo Switch.").Value, 0.5f, 4f);
			Settings.PixelLightCount = Mathf.Clamp(category.CreateEntry("PixelLightCount", 4, description: "Pixel Light Count - Default is 4. Affects the maximum number of pixel lights that should affect any object. If there are more lights illuminating an object, the dimmest ones will be rendered as vertex lights.").Value, 1, 4);
			Settings.TextureQuality = Mathf.Clamp(category.CreateEntry("Texture quality", 0, description: "Texture quality - default is 0. Higher values cause lower mip maps to be used. Can be used to reduce VRAM usage on low-end devices. 1 will cause half of the original resolution to be used, 2 will be 1/4 etc. 3 is 1/8th of original resolution.").Value, 0, 3);
			Settings.RealtimeReflectionProbes = category.CreateEntry("RealTimeReflectionProbes", true, description: "Allows usage of realtime reflection probes. Not sure if it used in game at all. Default is true.").Value;
			Settings.vSyncCount = category.CreateEntry("Vsync", true, description: "Enable vSync. True by default. If this is false and refresh rate is not 0, FPS cap is used.").Value ? 1 : 0;
		}
	}
}
