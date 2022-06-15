using MelonLoader;
using MelonLoader.Preferences;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SuisHack
{
	public class ExposedSettings
	{
		//Categories
		MelonPreferences_Category Category_mainDisplay;
		MelonPreferences_Category Category_graphicsSettings;
		MelonPreferences_Category Category_otherSettings;

		//Display settings
		public MelonPreferences_Entry<string> Entry_Display_Resolution;
		public MelonPreferences_Entry<int> Entry_Display_RefreshRate;
		public MelonPreferences_Entry<FullScreenMode> Entry_Display_DisplayMode;
		public MelonPreferences_Entry<bool> Entry_Display_Vsync;

		//Quality settings
		public MelonPreferences_Entry<PostProcessLayer.Antialiasing> Entry_Antialiasing;
		public MelonPreferences_Entry<AnisotropicFiltering> Entry_AnistropicFiltering;
		public MelonPreferences_Entry<int> Entry_AnistropicFiltering_Min;
		public MelonPreferences_Entry<int> Entry_AnistropicFiltering_Max;

		public MelonPreferences_Entry<float> Entry_Quality_LODBias;
		public MelonPreferences_Entry<int> Entry_Quality_PixelLightCount;
		public MelonPreferences_Entry<int> Entry_Quality_TextureQuality;
		public MelonPreferences_Entry<bool> Entry_Quality_RealtimeReflectionProbes;

		//Other settings
		public MelonPreferences_Entry<bool> Entry_Other_SkipIntros;
		public MelonPreferences_Entry<string> Entry_Other_Prompts;

		public ExposedSettings()
		{
			Category_mainDisplay = MelonPreferences.CreateCategory("Suis Hack Main Display");
			Category_graphicsSettings = MelonPreferences.CreateCategory("Suis Hack Graphics Settings");
			Category_otherSettings = MelonPreferences.CreateCategory("Suis Hack Other Settings");

			Entry_Display_Resolution = Category_mainDisplay.CreateEntry("Resolution", "0x0", description: "Screen or game resolution depending on display mode - if invalid resolution is specified - main screen resolution is used");
			Entry_Display_RefreshRate = Category_mainDisplay.CreateEntry("Refresh_Rate", -1, description: "Refresh rate used in fullscreen. If Vsync is disabled, this is used for FPS cap. (-1 is platform default, 0 is uncapped, anything higher is FPS capped)", validator: new ValueRange<int>(-1, 480));

			if (Hacks.ConfigParsing.ParseResolution(Entry_Display_Resolution.Value, out LemonTuple<int,int> desiredResolution))
				Resolution = desiredResolution;
			else
				Entry_Display_Resolution.Value = "0x0";

			Entry_Display_DisplayMode = Category_mainDisplay.CreateEntry("Mode", FullScreenMode.FullScreenWindow, description: "Unity's display mode. Options are: ExclusiveFullScreen / FullScreenWindow / MaximizedWindow / Windowed");
			Entry_Display_Vsync = Category_mainDisplay.CreateEntry("Vsync", true, description: "Enable vSync. True by default. If this is false and refresh rate is not 0, FPS cap is used.");

			Entry_Antialiasing = Category_graphicsSettings.CreateEntry("Antialiasing", PostProcessLayer.Antialiasing.FastApproximateAntialiasing, description: "Experimental: Antialiasing used by the game. Options are: \"None\" (disables AA) / \"FastApproximateAntialiasing\" (FXAA) / \"SubpixelMorphologicalAntialiasing\" (SMAA) / \"TemporalAntialiasing\" (TAA). Default on PC is \"FastApproximateAntialiasing\". MSAA is not available due to the game using deferred rendering path.");
			Entry_AnistropicFiltering = Category_graphicsSettings.CreateEntry("Anisotropic filtering", AnisotropicFiltering.ForceEnable, description: "Main anisotropic setting that applies to all textures. Options are \"Disable\" (disables all Anisotropic filtering) / \"Enable\" (makes it so the game uses per texture settings that were specified by the developer) / \"ForceEnable\" (Forces override - see \"Anisotropic filtering override min\" and \"Anisotropic filtering override max\"). Default on PC is ForceEnable (on Switch it was probably Enable)");
			Entry_AnistropicFiltering_Min = Category_graphicsSettings.CreateEntry("Anisotropic filtering override min", 8, description: "Minimal anisotropic filtering level when using ForceEnable anisotropic filtering", validator: new ValueRange<int>(-1, 16));
			Entry_AnistropicFiltering_Max = Category_graphicsSettings.CreateEntry("Anisotropic filtering override max", 16, description: "Maximum anisotropic filtering level when using ForceEnable anisotropic filtering", validator: new ValueRange<int>(-1, 16));

			Entry_Quality_LODBias = Category_graphicsSettings.CreateEntry("LODBias", 2f, description: "LOD Bias - affects how far from camera the LOD changes - bigger values, push the LOD change further from camera - min. 0.5, max. 4.0. Default game value is 2.0. Originally it was probably 1.0 on Nintendo Switch.", validator: new ValueRange<float>(0.5f, 4f));
			Entry_Quality_PixelLightCount = Category_graphicsSettings.CreateEntry("PixelLightCount", 4, description: "Pixel Light Count - Default is 4. Affects the maximum number of pixel lights that should affect any object. If there are more lights illuminating an object, the dimmest ones will be rendered as vertex lights.", validator: new ValueRange<int>(0, 8));
			Entry_Quality_TextureQuality = Category_graphicsSettings.CreateEntry("Texture quality", 0, description: "Texture quality - default is 0. Higher values cause lower mip maps to be used. Can be used to reduce VRAM usage on low-end devices. 1 will cause half of the original resolution to be used, 2 will be 1/4 etc. 3 is 1/8th of original resolution. Max is 4.", validator: new ValueRange<int>(0, 4));
			Entry_Quality_RealtimeReflectionProbes = Category_graphicsSettings.CreateEntry("RealTimeReflectionProbes", true, description: "Allows usage of realtime reflection probes. Not sure if it used in game at all. Default is true.");

			Entry_Other_SkipIntros = Category_otherSettings.CreateEntry("Skip intros", true, description: "Allows to skip splash screns / intros and go straight to menu! Disable in case of issues");
			Entry_Other_Prompts = Category_otherSettings.CreateEntry("Controller Prompts", "", description: "Asset bundle file name that is containing replacement prompts atlas. Make sure to use correct Steam Input binding for the keys to correspond to displayed prompts.");
		}

		public LemonTuple<int, int> Resolution;
	}
}
