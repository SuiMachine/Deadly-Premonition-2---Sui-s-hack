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
		public MelonPreferences_Entry<int> Entry_AnistropicFilteringValue;

		public MelonPreferences_Entry<float> Entry_Quality_CameraFarPlaneDistance;

		public MelonPreferences_Entry<float> Entry_Quality_ShadowDistance;
		public MelonPreferences_Entry<bool> Entry_Quality_Use4ShadowCascades;
		public MelonPreferences_Entry<ShadowQuality> Entry_Quality_ShadowsQuality;
		public MelonPreferences_Entry<ShadowResolution> Entry_Quality_ShadowsResolution;
		public MelonPreferences_Entry<ShadowmaskMode> Entry_Quality_ShadowMaskMode;
		public MelonPreferences_Entry<ShadowProjection> Entry_Quality_ShadowProjectionMode;
		public MelonPreferences_Entry<float> Entry_Quality_ShadowTwoSplitValue;
		public MelonPreferences_Entry<float> Entry_Quality_ShadowFourSplitValue1;
		public MelonPreferences_Entry<float> Entry_Quality_ShadowFourSplitValue2;
		public MelonPreferences_Entry<float> Entry_Quality_ShadowFourSplitValue3;

		public MelonPreferences_Entry<float> Entry_Quality_LODBias;
		public MelonPreferences_Entry<int> Entry_Quality_PixelLightCount;
		public MelonPreferences_Entry<int> Entry_Quality_TextureQuality;
		public MelonPreferences_Entry<int> Entry_Quality_MirrorReflectionResolution;

		//Other settings
		public MelonPreferences_Entry<bool> Entry_Other_SkipIntros;
		public MelonPreferences_Entry<string> Entry_Other_Prompts;
		public MelonPreferences_Entry<bool> Entry_Other_ShowAdvanced;
		public MelonPreferences_Entry<bool> Entry_Other_InterpolateMovement;
		public MelonPreferences_Entry<bool> Entry_Other_FixGeometry;

		public ExposedSettings()
		{
			Category_mainDisplay = MelonPreferences.CreateCategory("Suis Hack Main Display");
			Category_graphicsSettings = MelonPreferences.CreateCategory("Suis Hack Graphics Settings");
			Category_otherSettings = MelonPreferences.CreateCategory("Suis Hack Other Settings");

			Entry_Display_Resolution = Category_mainDisplay.CreateEntry("Resolution", "0x0", description: "Screen or game resolution depending on display mode - if invalid resolution is specified - main screen resolution is used");
			Entry_Display_RefreshRate = Category_mainDisplay.CreateEntry("Refresh_Rate", -1, description: "Refresh rate used in fullscreen. If Vsync is disabled, this is used for FPS cap. (-1 is platform default, 0 is uncapped, anything higher is FPS capped)", validator: new ValueRange<int>(-1, 480));

			if (Hacks.ConfigParsing.ParseResolution(Entry_Display_Resolution.Value, out LemonTuple<int, int> desiredResolution))
				Resolution = desiredResolution;
			else
				Entry_Display_Resolution.Value = "0x0";

			Entry_Display_DisplayMode = Category_mainDisplay.CreateEntry("Mode", FullScreenMode.FullScreenWindow, description: "Unity's display mode. Options are: ExclusiveFullScreen / FullScreenWindow / MaximizedWindow / Windowed");
			Entry_Display_Vsync = Category_mainDisplay.CreateEntry("Vsync", true, description: "Enable vSync. True by default. If this is false and refresh rate is not 0, FPS cap is used.");

			Entry_Antialiasing = Category_graphicsSettings.CreateEntry("Antialiasing", PostProcessLayer.Antialiasing.FastApproximateAntialiasing, description: "Experimental: Antialiasing used by the game. Options are: \"None\" (disables AA) / \"FastApproximateAntialiasing\" (FXAA) / \"SubpixelMorphologicalAntialiasing\" (SMAA) / \"TemporalAntialiasing\" (TAA). Default on PC is \"FastApproximateAntialiasing\". MSAA is not available due to the game using deferred rendering path.");
			Entry_Antialiasing.OnValueChanged += (PostProcessLayer.Antialiasing oldVal, PostProcessLayer.Antialiasing newVal) => { Hacks.PostProcessLayerHook.Antialiasing = newVal; };

			Entry_AnistropicFiltering = Category_graphicsSettings.CreateEntry("Anisotropic filtering", AnisotropicFiltering.ForceEnable, description: "Main anisotropic setting that applies to all textures. Options are \"Disable\" (disables all Anisotropic filtering) / \"Enable\" (makes it so the game uses per texture settings that were specified by the developer) / \"ForceEnable\" (Forces override - see \"Anisotropic filtering override min\" and \"Anisotropic filtering override max\"). Default on PC is ForceEnable (on Switch it was probably Enable)");
			Entry_AnistropicFiltering.OnValueChanged += (AnisotropicFiltering oldAF, AnisotropicFiltering newAF) => { QualitySettings.anisotropicFiltering = newAF; };

			Entry_AnistropicFilteringValue = Category_graphicsSettings.CreateEntry("Anisotropic filtering override value", 8, description: "Anisotropic filtering level when using ForceEnable anisotropic filtering", validator: new ValueRange<int>(-1, 16));
			Entry_AnistropicFilteringValue.OnValueChanged += (int oldValue, int newValue) => { Texture.SetGlobalAnisotropicFilteringLimits(newValue, newValue); };

			Entry_Quality_CameraFarPlaneDistance = Category_graphicsSettings.CreateEntry("Far plane distance", 700f, description: "Sets camera's maximum draw distance (far clip plane). The default is: 700.");
			Entry_Quality_CameraFarPlaneDistance.OnValueChanged += (float oldValue, float newValue) => { Hacks.PostProcessLayerHook.FarClipDistance = newValue; };

			Entry_Quality_ShadowDistance = Category_graphicsSettings.CreateEntry("Shadow distance", 150f, description: "Affects the distance at which the shadows are rendered. The default is 150. Careful as this option is tied with shadow cascades. Min. is 10, max 1000", validator: new ValueRange<float>(10, 1000));
			Entry_Quality_ShadowDistance.OnValueChanged += (float oldValue, float newValue) => { QualitySettings.shadowDistance = newValue; };

			Entry_Quality_ShadowsQuality = Category_graphicsSettings.CreateEntry("Shadows quality", ShadowQuality.All, description: "Sets Unity\'s shadadow quality, which is actually the types of shadows allowed. Options are \"Disable\" (to disable any shadows) / \"HardOnly\" (to allow only hard shadows) / All (to allow both smooth shadows). By default the game uses All.");
			Entry_Quality_ShadowsQuality.OnValueChanged += (ShadowQuality oldValue, ShadowQuality newValue) => { QualitySettings.shadows = newValue; };

			Entry_Quality_ShadowsResolution = Category_graphicsSettings.CreateEntry("Shadows resolution", ShadowResolution.High, description: "Sets shadow resolution. There are 4 options: Low / Medium / High / VeryHigh. By default the PC version uses High. Very high shouldn't provide much of a difference above High unless 4 cascades are used.");
			Entry_Quality_ShadowsResolution.OnValueChanged += (ShadowResolution oldValue, ShadowResolution newValue) => { QualitySettings.shadowResolution = newValue; };

			Entry_Quality_Use4ShadowCascades = Category_graphicsSettings.CreateEntry("Use 4 Shadow cascades", false, description: "Makes it so the game uses 4 shadow cascades instead of 2. This should significently increase shadows quality in the game if used. By default in PC version the game uses 2 cascades.");
			Entry_Quality_Use4ShadowCascades.OnValueChanged += (bool oldValue, bool newValue) => { QualitySettings.shadowCascades = newValue ? 4 : 2; };

			Entry_Quality_ShadowMaskMode = Category_graphicsSettings.CreateEntry("Shadows mask mode", ShadowmaskMode.DistanceShadowmask, description: "Sets Unity\'s Shadowmask mode. Options are: Shadowmask / DistanceShadowmask. See https://docs.unity3d.com/ScriptReference/ShadowmaskMode.html cause I can't be bothered. By default the game uses DistanceShadowmask.");
			Entry_Quality_ShadowMaskMode.OnValueChanged += (ShadowmaskMode oldValue, ShadowmaskMode newValue) => { QualitySettings.shadowmaskMode = newValue; };

			Entry_Quality_LODBias = Category_graphicsSettings.CreateEntry("LODBias", 2f, description: "LOD Bias - affects how far from camera the LOD changes - bigger values, push the LOD change further from camera - min. 0.5, max. 4.0. Default game value is 2.0. Originally it was probably 1.0 on Nintendo Switch.", validator: new ValueRange<float>(0.5f, 4f));
			Entry_Quality_LODBias.OnValueChanged += (float oldValue, float newValue) => { QualitySettings.lodBias = newValue; };

			Entry_Quality_ShadowTwoSplitValue = Category_graphicsSettings.CreateEntry("Shadow split 2 percent", 0.333333333f, description: "Sets percentage distance from maximum shadow distance where first cascade transitions to second one. Default value is 0.333333333. This value is not used when using 4 cascades. Essentially what this means that with a default shadow distance of 150 and value split percentage 0.333333333, the first cascade will transition to second one at a distance of approx. 50 units from camera. Min value is 0.01, max is 0.5", validator: new ValueRange<float>(0.01f, 0.5f));
			Entry_Quality_ShadowTwoSplitValue.OnValueChanged += (float oldValue, float newValue) => { QualitySettings.shadowCascade2Split = newValue; };

			Entry_Quality_ShadowFourSplitValue1 = Category_graphicsSettings.CreateEntry("Shadow split 4 percent 1", 0.06666667f, description: "See \"Shadow split 2 percent\" - this is the same, except used for 4 cascades - this one is percentage distance between cascade 1 and 2. Default value is 0.06666667f. Min value is 0.01, max is 0.5", validator: new ValueRange<float>(0.01f, 0.5f));
			Entry_Quality_ShadowFourSplitValue2 = Category_graphicsSettings.CreateEntry("Shadow split 4 percent 2", 0.2f, description: "See \"Shadow split 2 percent\" - this is the same, except used for 4 cascades - this one is percentage distance between cascade 2 and 3. Default value is 0.2. Min value is 0.1, max is 0.8", validator: new ValueRange<float>(0.1f, 0.8f));
			Entry_Quality_ShadowFourSplitValue3 = Category_graphicsSettings.CreateEntry("Shadow split 4 percent 3", 0.4666667f, description: "See \"Shadow split 2 percent\" - this is the same, except used for 4 cascades - this one is percentage distance between cascade 3 and 4. Default value is 0.4666667. Min value is 0.02, max is 0.9", validator: new ValueRange<float>(0.02f, 0.9f));
			Entry_Quality_ShadowProjectionMode = Category_graphicsSettings.CreateEntry("Shadow project mode", ShadowProjection.StableFit, description: "Shadow projection mode. Options \"CloseFit\" / \"StableFit\". Default is StableFit. See https://docs.unity3d.com/ScriptReference/ShadowProjection.html");
			Entry_Quality_ShadowProjectionMode.OnValueChanged += (ShadowProjection oldValue, ShadowProjection newValue) => { QualitySettings.shadowProjection = newValue; };

			Entry_Quality_MirrorReflectionResolution = Category_graphicsSettings.CreateEntry("Mirrors reflection reflection", 512, description: "Overrides render texture resolution for planar reflections - the resolution has to be a number that is a power of 2 and at least 128 and at most 2048.", validator: new PowerOfTwoValidatorWithRange(128, 2048));
			Entry_Quality_MirrorReflectionResolution.OnValueChanged += (int oldValue, int newValue) => { Hacks.MirrorReflectionHook.ReflectionSize = newValue; };
			Hacks.MirrorReflectionHook.ReflectionSize = Entry_Quality_MirrorReflectionResolution.Value;

			Entry_Quality_PixelLightCount = Category_graphicsSettings.CreateEntry("PixelLightCount", 4, description: "Pixel Light Count - Default is 4. Affects the maximum number of pixel lights that should affect any object. If there are more lights illuminating an object, the dimmest ones will be rendered as vertex lights.", validator: new ValueRange<int>(0, 8));
			Entry_Quality_PixelLightCount.OnValueChanged += (int oldValue, int newValue) => { QualitySettings.pixelLightCount = newValue; };

			Entry_Quality_TextureQuality = Category_graphicsSettings.CreateEntry("Texture quality", 0, description: "Texture quality - default is 0. Higher values cause lower mip maps to be used. Can be used to reduce VRAM usage on low-end devices. 1 will cause half of the original resolution to be used", validator: new ValueRange<int>(0, 1));
			Entry_Quality_TextureQuality.OnValueChanged += (int oldValue, int newValue) => { QualitySettings.masterTextureLimit = newValue; };

			Entry_Other_SkipIntros = Category_otherSettings.CreateEntry("Skip intros", true, description: "Allows to skip splash screns / intros and go straight to menu! Disable in case of issues");
			Entry_Other_Prompts = Category_otherSettings.CreateEntry("Controller Prompts", "", description: "Asset bundle file name that is containing replacement prompts atlas. Make sure to use correct Steam Input binding for the keys to correspond to displayed prompts.");
			Entry_Other_ShowAdvanced = Category_otherSettings.CreateEntry("Show advanced settings", false, description: "Shows advanced options in GUI.");

			Entry_Other_InterpolateMovement = Category_otherSettings.CreateEntry("Interpolate movement", true, description: "Experimental: Enabled hooks related to movement interpolation of some rendered objects (protagonist and camera) to work around 50Hz Fixed Update stuttering - to my speedrunning friends: DO NOT allow this option for speedrunning.");
			Entry_Other_InterpolateMovement.OnValueChanged += (bool oldValue, bool newVal) => { Components.SmootherController.InterpolateMovement = newVal; };
			Components.SmootherController.InterpolateMovement = Entry_Other_InterpolateMovement.Value;

			Entry_Other_FixGeometry = Category_otherSettings.CreateEntry("Fix geometry", true, description: "Applies additional scripts on level load to fix some geometry issues");
		}

		public LemonTuple<int, int> Resolution;
	}
}
