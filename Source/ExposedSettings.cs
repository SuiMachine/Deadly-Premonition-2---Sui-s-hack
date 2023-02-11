using MelonLoader;
using MelonLoader.Preferences;
using SuisHack.KeyboardSupport;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SuisHack
{
	public class ExposedSettings
	{
		public enum InputType
		{
			SteamInput,
			KeyboardAndMouse
		}

		public enum GeometryImprovements
		{
			Disabled,
			Enabled,
			ExtraGeometry
		}

		public enum LightImprovements
		{
			Disabled,
			Minor,
			All
		}

		//Categories
		MelonPreferences_Category Category_mainDisplay;
		MelonPreferences_Category Category_graphicsSettings;
		MelonPreferences_Category Category_inputSettings;
		MelonPreferences_Category Category_otherSettings;

		//Display settings
		public MelonPreferences_Entry<string> Entry_Display_Resolution;
		public MelonPreferences_Entry<int> Entry_Display_RefreshRate;
		public MelonPreferences_Entry<int> Entry_DesiredFramerate;
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
		public MelonPreferences_Entry<float> Entry_Quality_ShadowTwoSplitValue;
		public MelonPreferences_Entry<float> Entry_Quality_ShadowFourSplitValue1;
		public MelonPreferences_Entry<float> Entry_Quality_ShadowFourSplitValue2;
		public MelonPreferences_Entry<float> Entry_Quality_ShadowFourSplitValue3;

		public MelonPreferences_Entry<float> Entry_Quality_LODBias;
		public MelonPreferences_Entry<int> Entry_Quality_PixelLightCount;
		public MelonPreferences_Entry<int> Entry_Quality_TextureQuality;
		public MelonPreferences_Entry<int> Entry_Quality_MirrorReflectionResolution;
		public MelonPreferences_Entry<HBAO_Core.Preset> Entry_Quality_HBAO_Preset;
		public MelonPreferences_Entry<float> Entry_Quality_HBAO_Intensity;

		public MelonPreferences_Entry<bool> Entry_Quality_SSR_Enable;
		public MelonPreferences_Entry<ScreenSpaceReflectionPreset> Entry_Quality_SSR_Preset;
		public MelonPreferences_Entry<ScreenSpaceReflectionResolution> Entry_Quality_SSR_Resolution;
		public MelonPreferences_Entry<float> Entry_Quality_SSR_Tickness;
		public MelonPreferences_Entry<float> Entry_Quality_SSR_Vignette;
		public MelonPreferences_Entry<float> Entry_Quality_SSR_DistanceFade;
		public MelonPreferences_Entry<float> Entry_Quality_SSR_MaxMarchingDistance;
		public MelonPreferences_Entry<bool> Entry_Quality_EdgeDetection;
		public MelonPreferences_Entry<float> Entry_Quality_EdgeDetectionDepth;


		//Input settings
		public MelonPreferences_Entry<InputType> Input_Override;
		public MelonPreferences_Entry<float> Input_Analog_LeftStickFloatTime;
		public MelonPreferences_Entry<KeyCode> Input_Analog_LeftStick_Up;
		public MelonPreferences_Entry<KeyCode> Input_Analog_LeftStick_Right;
		public MelonPreferences_Entry<KeyCode> Input_Analog_LeftStick_Down;
		public MelonPreferences_Entry<KeyCode> Input_Analog_LeftStick_Left;
		public MelonPreferences_Entry<KeyCode> Input_Digital_A_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_B_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_X_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_Y_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_LB;
		public MelonPreferences_Entry<KeyCode> Input_Digital_RB;
		public MelonPreferences_Entry<KeyCode> Input_Digital_Back_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_Start_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_L_Stick_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_R_Stick_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_Up_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_Right_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_Down_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_Left_Button;
		public MelonPreferences_Entry<KeyCode> Input_Digital_LT;
		public MelonPreferences_Entry<KeyCode> Input_Digital_RT;
		public MelonPreferences_Entry<float> Input_Mouse_Sensitivity;
		public MelonPreferences_Entry<bool> Input_MouseYAxisInversion;
		public MelonPreferences_Entry<bool> Input_Controller_Vibration;


		//Other settings
		public MelonPreferences_Entry<bool> Entry_Other_SkipIntros;
		public MelonPreferences_Entry<string> Entry_Other_Prompts;
		public MelonPreferences_Entry<bool> Entry_Other_ShowAdvanced;
		public MelonPreferences_Entry<bool> Entry_Other_InterpolateMovement;
		public MelonPreferences_Entry<GeometryImprovements> Entry_Other_GeometryImprovements;
		public MelonPreferences_Entry<LightImprovements> Entry_Other_LightImprovements;
		public MelonPreferences_Entry<bool> Entry_Other_EnableCheats;

		public ExposedSettings()
		{
			Category_mainDisplay = MelonPreferences.CreateCategory("Suis Hack Main Display");
			Category_graphicsSettings = MelonPreferences.CreateCategory("Suis Hack Graphics Settings");
			Category_inputSettings = MelonPreferences.CreateCategory("Suis Hack Input Settings");
			Category_otherSettings = MelonPreferences.CreateCategory("Suis Hack Other Settings");

			RegisterMainDisplay();
			RegisterGraphicsSettings();
			RegisterInputSettings();
			RegisterOtherSettings();
		}

		private void RegisterMainDisplay()
		{
			Entry_Display_Resolution = Category_mainDisplay.CreateEntry("Resolution", "0x0", description: "Screen or game resolution depending on display mode - if invalid resolution is specified - main screen resolution is used");
			Entry_Display_RefreshRate = Category_mainDisplay.CreateEntry("Refresh_Rate", 0, description: "Refresh rate used in fullscreen. Normally it should be 0 (uses screen default). Only really matters if the game is set to Exclusive Fullscreen mode.", validator: new ValueRange<int>(0, 560));
			Entry_DesiredFramerate = Category_mainDisplay.CreateEntry("DesiredFPS", -1, description: "Desired framerate. This is only used when Vsync is disabled. -1 is platform default, 0 is uncapped, max is 1000.", validator: new ValueRange<int>(-1, 1000));
			Entry_DesiredFramerate.OnValueChanged += (int oldValue, int newValue) => { Application.targetFrameRate = newValue; };

			if (Hacks.ConfigParsing.ParseResolution(Entry_Display_Resolution.Value, out LemonTuple<int, int> desiredResolution))
				Resolution = desiredResolution;
			else
				Entry_Display_Resolution.Value = "0x0";

			Entry_Display_DisplayMode = Category_mainDisplay.CreateEntry("Mode", FullScreenMode.FullScreenWindow, description: "Unity's display mode. Options are: ExclusiveFullScreen / FullScreenWindow / MaximizedWindow / Windowed");
			Entry_Display_Vsync = Category_mainDisplay.CreateEntry("Vsync", true, description: "Enable vSync. True by default. If this is false and refresh rate is not 0, FPS cap is used.");
		}

		private void RegisterGraphicsSettings()
		{
			//I was originally planning on writting some wrapper to set up these events, but why should I when I can have a wall of text!
			Entry_Antialiasing = Category_graphicsSettings.CreateEntry("Antialiasing", PostProcessLayer.Antialiasing.FastApproximateAntialiasing, description: "Experimental: Antialiasing used by the game. Options are: \"None\" (disables AA) / \"FastApproximateAntialiasing\" (FXAA) / \"SubpixelMorphologicalAntialiasing\" (SMAA) / \"TemporalAntialiasing\" (TAA). Default on PC is \"FastApproximateAntialiasing\". MSAA is not available due to the game using deferred rendering path. TAA causes issues with phantoms and as such SMAA is recommended.");
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

			Entry_Quality_LODBias = Category_graphicsSettings.CreateEntry("LODBias", 2f, description: "LOD Bias - affects how far from camera the LOD changes - bigger values, push the LOD change further from camera - min. 0.5, max. 4.0. Default game value is 2.0. Originally it was probably 1.0 on Nintendo Switch.", validator: new ValueRange<float>(0.5f, 4f));
			Entry_Quality_LODBias.OnValueChanged += (float oldValue, float newValue) => { QualitySettings.lodBias = newValue; };

			Entry_Quality_ShadowTwoSplitValue = Category_graphicsSettings.CreateEntry("Shadow split 2 percent", 0.333333333f, description: "Sets percentage distance from maximum shadow distance where first cascade transitions to second one. Default value is 0.333333333. This value is not used when using 4 cascades. Essentially what this means that with a default shadow distance of 150 and value split percentage 0.333333333, the first cascade will transition to second one at a distance of approx. 50 units from camera. Min value is 0.01, max is 0.5", validator: new ValueRange<float>(0.01f, 0.5f));
			Entry_Quality_ShadowTwoSplitValue.OnValueChanged += (float oldValue, float newValue) => { QualitySettings.shadowCascade2Split = newValue; };

			Entry_Quality_ShadowFourSplitValue1 = Category_graphicsSettings.CreateEntry("Shadow split 4 percent 1", 0.06666667f, description: "See \"Shadow split 2 percent\" - this is the same, except used for 4 cascades - this one is percentage distance between cascade 1 and 2. Default value is 0.06666667f. Min value is 0.01, max is 0.5", validator: new ValueRange<float>(0.01f, 0.5f));
			Entry_Quality_ShadowFourSplitValue2 = Category_graphicsSettings.CreateEntry("Shadow split 4 percent 2", 0.2f, description: "See \"Shadow split 2 percent\" - this is the same, except used for 4 cascades - this one is percentage distance between cascade 2 and 3. Default value is 0.2. Min value is 0.1, max is 0.8", validator: new ValueRange<float>(0.1f, 0.8f));
			Entry_Quality_ShadowFourSplitValue3 = Category_graphicsSettings.CreateEntry("Shadow split 4 percent 3", 0.4666667f, description: "See \"Shadow split 2 percent\" - this is the same, except used for 4 cascades - this one is percentage distance between cascade 3 and 4. Default value is 0.4666667. Min value is 0.02, max is 0.9", validator: new ValueRange<float>(0.02f, 0.9f));

			Entry_Quality_MirrorReflectionResolution = Category_graphicsSettings.CreateEntry("Mirrors reflection reflection", 512, description: "Overrides render texture resolution for planar reflections - the resolution has to be a number that is a power of 2 and at least 128 and at most 2048. Default is 512.", validator: new PowerOfTwoValidatorWithRange(128, 2048));
			Entry_Quality_MirrorReflectionResolution.OnValueChanged += (int oldValue, int newValue) => { Hacks.MirrorReflectionHook.ReflectionSize = newValue; };
			Hacks.MirrorReflectionHook.ReflectionSize = Entry_Quality_MirrorReflectionResolution.Value;

			Entry_Quality_HBAO_Preset = Category_graphicsSettings.CreateEntry("HBAO Preset", HBAO_Core.Preset.FastestPerformance, description: "Preset to use to override HBAO. Options are: FastestPerformance / FastPerformance / Normal / HighQuality / HighestQuality. Default is FastestPerformance. For Normal and higher, consider lowering HBAO intensity.");
			Entry_Quality_HBAO_Preset.OnValueChanged += (HBAO_Core.Preset oldValue, HBAO_Core.Preset newValue) => { Hacks.PostProcessLayerHook.HBAO_Preset = newValue; };
			Hacks.PostProcessLayerHook.HBAO_Preset = Entry_Quality_HBAO_Preset.Value;

			Entry_Quality_HBAO_Intensity = Category_graphicsSettings.CreateEntry("HBAO Intensity", 1.0f, description: "HBAO intensity - this probably should be lower than 1.0 when using Normal and higher presets.", validator: new ValueRange<float>(0, 1));
			Entry_Quality_HBAO_Intensity.OnValueChanged += (float oldValue, float newValue) => { Hacks.PostProcessLayerHook.HBAO_Intensity = newValue; };
			Hacks.PostProcessLayerHook.HBAO_Intensity = Entry_Quality_HBAO_Intensity.Value;

			Entry_Quality_SSR_Enable = Category_graphicsSettings.CreateEntry("SSR Enable", false, description: "Enables Screen Space Reflection override");
			Entry_Quality_SSR_Enable.OnValueChanged += (bool oldValue, bool newValue) => { Hacks.PostProcessLayerHook.SSR_Enabled = newValue; };
			Hacks.PostProcessLayerHook.SSR_Enabled = Entry_Quality_SSR_Enable.Value;

			Entry_Quality_SSR_Preset = Category_graphicsSettings.CreateEntry("SSR Preset", ScreenSpaceReflectionPreset.Medium, description: "Sets Screen Space Reflection preset. Can be Lower / Low / Medium / High / Higher / Ultra / Overkill. Generally you shouldn't use Overkill.");
			Entry_Quality_SSR_Preset.OnValueChanged += (ScreenSpaceReflectionPreset oldValue, ScreenSpaceReflectionPreset newValue) => { Hacks.PostProcessLayerHook.SSR_Preset = newValue; };
			Hacks.PostProcessLayerHook.SSR_Preset = Entry_Quality_SSR_Preset.Value;

			Entry_Quality_SSR_Resolution = Category_graphicsSettings.CreateEntry("SSR Resolution", ScreenSpaceReflectionResolution.Downsampled, description: "Sets Screen Space Reflections resolution. Can be Downsampled / FullSize / Supersampled. Generally you shouldn't use Supersampled.");
			Entry_Quality_SSR_Resolution.OnValueChanged += (ScreenSpaceReflectionResolution oldValue, ScreenSpaceReflectionResolution newValue) => { Hacks.PostProcessLayerHook.SSR_Resolution = newValue; };
			Hacks.PostProcessLayerHook.SSR_Resolution = Entry_Quality_SSR_Resolution.Value;

			Entry_Quality_SSR_Tickness = Category_graphicsSettings.CreateEntry("SSR Tickness", 1f, description: "Sets Screen Space Reflections tickness value", validator: new ValueRange<float>(0, 1));
			Entry_Quality_SSR_Tickness.OnValueChanged += (float oldValue, float newValue) => { Hacks.PostProcessLayerHook.SSR_Tickness = newValue; };
			Hacks.PostProcessLayerHook.SSR_Tickness = Entry_Quality_SSR_Tickness.Value;

			Entry_Quality_SSR_Vignette = Category_graphicsSettings.CreateEntry("SSR Vignette", 0.15f, description: "Sets Screen Space Reflections vigniette value, which smoothly disables reflections towards the edges of the screen", validator: new ValueRange<float>(0, 1));
			Entry_Quality_SSR_Vignette.OnValueChanged += (float oldValue, float newValue) => { Hacks.PostProcessLayerHook.SSR_Vignette = newValue; };
			Hacks.PostProcessLayerHook.SSR_Vignette = Entry_Quality_SSR_Vignette.Value;

			Entry_Quality_SSR_DistanceFade = Category_graphicsSettings.CreateEntry("SSR Distance fade", 0.01f, description: "Sets Screen Space Reflections distance fade value. Keep it really low to reduce SSR glow-like effect around silhouettes", validator: new ValueRange<float>(0, 0.5f));
			Entry_Quality_SSR_DistanceFade.OnValueChanged += (float oldValue, float newValue) => { Hacks.PostProcessLayerHook.SSR_DistanceFade = newValue; };
			Hacks.PostProcessLayerHook.SSR_DistanceFade = Entry_Quality_SSR_DistanceFade.Value;

			Entry_Quality_SSR_MaxMarchingDistance = Category_graphicsSettings.CreateEntry("SSR Max Marching Distance", 100f, description: "Sets Screen Space Reflections max maching distance value, after which the ray is terminated.", validator: new ValueRange<float>(50, 250));
			Entry_Quality_SSR_MaxMarchingDistance.OnValueChanged += (float oldValue, float newValue) => { Hacks.PostProcessLayerHook.SSR_MaxMarchingDistance = newValue; };
			Hacks.PostProcessLayerHook.SSR_MaxMarchingDistance = Entry_Quality_SSR_MaxMarchingDistance.Value;

			Entry_Quality_PixelLightCount = Category_graphicsSettings.CreateEntry("PixelLightCount", 4, description: "Pixel Light Count - Default is 4. Affects the maximum number of pixel lights that should affect any object. If there are more lights illuminating an object, the dimmest ones will be rendered as vertex lights.", validator: new ValueRange<int>(0, 8));
			Entry_Quality_PixelLightCount.OnValueChanged += (int oldValue, int newValue) => { QualitySettings.pixelLightCount = newValue; };

			Entry_Quality_TextureQuality = Category_graphicsSettings.CreateEntry("Texture quality", 0, description: "Texture quality - default is 0. Higher values cause lower mip maps to be used. Can be used to reduce VRAM usage on low-end devices. 1 will cause half of the original resolution to be used", validator: new ValueRange<int>(0, 1));
			Entry_Quality_TextureQuality.OnValueChanged += (int oldValue, int newValue) => { QualitySettings.masterTextureLimit = newValue; };

			Entry_Quality_EdgeDetection = Category_graphicsSettings.CreateEntry("Edge Detection Filter", true, description: "Responsible for Enabling/Disabling edge detection post process filter.");
			Entry_Quality_EdgeDetection.OnValueChanged += (bool oldValue, bool newValue) => { Hacks.PostProcessLayerHook.EnableEdgeDetectionFilter = newValue; };
			Hacks.PostProcessLayerHook.EnableEdgeDetectionFilter = Entry_Quality_EdgeDetection.Value;

			Entry_Quality_EdgeDetectionDepth = Category_graphicsSettings.CreateEntry("Edge Detection Filter Depth", 1.0f, description: "Responsible for figuring out how to apply edges based on distance from camera. This might have to be changed to lower values when extending camera's far plane to avoid glitches.");
			Entry_Quality_EdgeDetectionDepth.OnValueChanged += (float oldValue, float newValue) => { Hacks.PostProcessLayerHook.EnableEdgeDetectionFilterDepth = newValue; };
			Hacks.PostProcessLayerHook.EnableEdgeDetectionFilterDepth = Entry_Quality_EdgeDetectionDepth.Value;
		}

		private void RegisterInputSettings()
		{
			Input_Override = Category_inputSettings.CreateEntry("Input type", InputType.SteamInput, description: "Overrides controls handling - options are: SteamInput (leaves the game using Steam Input as it is) / KeyboardAndMouse (hooks input to read keyboard and mouse instead)");
			Input_Analog_LeftStickFloatTime = Category_inputSettings.CreateEntry("Left Stick Float Time", 0.1f, description: "How long does it take a stick to get to desired spot - this allows the game to handle rotations slightly better, although prevents the input from being instantanious. Min value: 0.01, Max 1.", validator: new ValueRange<float>(0.01f, 1f));
			Input_Analog_LeftStickFloatTime.OnValueChanged += (float oldValue, float newValue) => { GlobalInputHookHandler.Instance.AnalogInputToInput[SteamInputHook.SteamInputAnalog.L_Stick] = new KeyActionAnalog(Input_Analog_LeftStick_Up.Value, Input_Analog_LeftStick_Down.Value, Input_Analog_LeftStick_Left.Value, Input_Analog_LeftStick_Right.Value, newValue); GlobalInputHookHandler.Instance?.InitializeInputs(); };

			Input_Analog_LeftStick_Up = Category_inputSettings.CreateEntry("Left Stick Key Up", KeyCode.W, description: "Key used to as replacement for reading up on left analog's Y axis");
			Input_Analog_LeftStick_Up.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.AnalogInputToInput[SteamInputHook.SteamInputAnalog.L_Stick] = new KeyActionAnalog(newValue, Input_Analog_LeftStick_Down.Value, Input_Analog_LeftStick_Left.Value, Input_Analog_LeftStick_Right.Value, Input_Analog_LeftStickFloatTime.Value); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Analog_LeftStick_Right = Category_inputSettings.CreateEntry("Left Stick Key Right", KeyCode.D, description: "Key used to as replacement for reading left on left analog's X axis");
			Input_Analog_LeftStick_Right.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.AnalogInputToInput[SteamInputHook.SteamInputAnalog.L_Stick] = new KeyActionAnalog(Input_Analog_LeftStick_Up.Value, Input_Analog_LeftStick_Down.Value, Input_Analog_LeftStick_Left.Value, newValue, Input_Analog_LeftStickFloatTime.Value); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Analog_LeftStick_Down = Category_inputSettings.CreateEntry("Left Stick Key Down", KeyCode.S, description: "Key used to as replacement for reading down on left analog's Y axis");
			Input_Analog_LeftStick_Down.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.AnalogInputToInput[SteamInputHook.SteamInputAnalog.L_Stick] = new KeyActionAnalog(Input_Analog_LeftStick_Up.Value, newValue, Input_Analog_LeftStick_Left.Value, Input_Analog_LeftStick_Right.Value, Input_Analog_LeftStickFloatTime.Value); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Analog_LeftStick_Left = Category_inputSettings.CreateEntry("Left Stick Key Left", KeyCode.A, description: "Key used to as replacement for reading right on left analog's X axis");
			Input_Analog_LeftStick_Left.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.AnalogInputToInput[SteamInputHook.SteamInputAnalog.L_Stick] = new KeyActionAnalog(Input_Analog_LeftStick_Up.Value, Input_Analog_LeftStick_Down.Value, newValue, Input_Analog_LeftStick_Right.Value, Input_Analog_LeftStickFloatTime.Value); GlobalInputHookHandler.Instance?.InitializeInputs(); };

			Input_Digital_A_Button = Category_inputSettings.CreateEntry("Controller button B", KeyCode.Q, description: "Key used as replacement for Xbox's B key (originally Switch's A key) - generally cancel action");
			Input_Digital_A_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.A_Button] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Digital_B_Button = Category_inputSettings.CreateEntry("Controller button A", KeyCode.E, description: "Key used as replacement for Xbox's A key (originally Switch's B key) - generally confirm and contextual action");
			Input_Digital_B_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.B_Button] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Digital_X_Button = Category_inputSettings.CreateEntry("Controller button Y", KeyCode.Space, description: "Key used as replacement for Xbox's Y key (originally Switch's X key) - generally used for skateboard");
			Input_Digital_X_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.X_Button] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Digital_Y_Button = Category_inputSettings.CreateEntry("Controller button X", KeyCode.Escape, description: "Key used as replacement for Xbox's Y key (originally Switch's X key) - generally used for opening Red Room");
			Input_Digital_Y_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.Y_Button] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };

			Input_Digital_LB = Category_inputSettings.CreateEntry("Controller LB", KeyCode.Alpha2, description: "Key used as replacement for Left Bumper - generally used for vision mode");
			Input_Digital_LB.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.LB] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Digital_RB = Category_inputSettings.CreateEntry("Controller RB", KeyCode.LeftShift, description: "Key used as replacement for Right Bumper - generally used for sprint");
			Input_Digital_RB.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.RB] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };

			Input_Digital_Back_Button = Category_inputSettings.CreateEntry("Controller Back", KeyCode.Tab, description: "Key used as replacement for Back / Select button");
			Input_Digital_Back_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.Back_Button] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Digital_Start_Button = Category_inputSettings.CreateEntry("Controller Start", KeyCode.O, description: "Key used as replacement for Start button");
			Input_Digital_Start_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.Start_Button] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };

			Input_Digital_L_Stick_Button = Category_inputSettings.CreateEntry("Controller Left Stick Click", KeyCode.LeftControl, description: "Key used as replacement for Left stick click - generally used for crouching");
			Input_Digital_L_Stick_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.L_Stick_Button] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Digital_R_Stick_Button = Category_inputSettings.CreateEntry("Controller Right Stick Click", KeyCode.Mouse2, description: "Key used as replacement for Right stick click");
			Input_Digital_R_Stick_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.R_Stick_Button] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };

			Input_Digital_Up_Button = Category_inputSettings.CreateEntry("Controller Dpad up", KeyCode.UpArrow, description: "Key used as replacement for D-pad up");
			Input_Digital_Up_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.Up_Button] = new KeySteamDigitalAction(newValue, true); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Digital_Right_Button = Category_inputSettings.CreateEntry("Controller Dpad right", KeyCode.RightArrow, description: "Key used as replacement for D-pad right");
			Input_Digital_Right_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.Right_Button] = new KeySteamDigitalAction(newValue, true); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Digital_Down_Button = Category_inputSettings.CreateEntry("Controller Dpad down", KeyCode.DownArrow, description: "Key used as replacement for D-pad down");
			Input_Digital_Down_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.Down_Button] = new KeySteamDigitalAction(newValue, true); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Digital_Left_Button = Category_inputSettings.CreateEntry("Controller Dpad left", KeyCode.LeftArrow, description: "Key used as replacement for D-pad left");
			Input_Digital_Left_Button.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.Left_Button] = new KeySteamDigitalAction(newValue, true); GlobalInputHookHandler.Instance?.InitializeInputs(); };

			Input_Digital_LT = Category_inputSettings.CreateEntry("Controller Left Trigger", KeyCode.Mouse1, description: "Key used as replacemen for Left trigger pull - generally aiming");
			Input_Digital_LT.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.LT] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };
			Input_Digital_RT = Category_inputSettings.CreateEntry("Controller Right Trigger", KeyCode.Mouse0, description: "Key used as replacemen for Right trigger pull - generally punching / shooting");
			Input_Digital_RT.OnValueChanged += (KeyCode oldValue, KeyCode newValue) => { GlobalInputHookHandler.Instance.DigitalInputToInput[SteamInputHook.SteamInputDigital.RT] = new KeySteamDigitalAction(newValue, false); GlobalInputHookHandler.Instance?.InitializeInputs(); };

			Input_Mouse_Sensitivity = Category_inputSettings.CreateEntry("Mouse sensitivity", 1.0f, description: "Mouse sensitivity multiplier.", validator: new ValueRange<float>(0.05f, 2f));
			Input_Mouse_Sensitivity.OnValueChanged += (float oldValue, float newValue) => { MouseAnalog.Sensitivity = newValue; };
			MouseAnalog.Sensitivity = Input_Mouse_Sensitivity.Value;

			Input_MouseYAxisInversion = Category_inputSettings.CreateEntry("Mouse Y axis inversion", false, description: "Inverts mouse's Y axis.");
			Input_MouseYAxisInversion.OnValueChanged += (bool oldValue, bool newValue) => { MouseAnalog.InvertYAxis = newValue; };
			MouseAnalog.InvertYAxis = Input_MouseYAxisInversion.Value;

			Input_Controller_Vibration = Category_inputSettings.CreateEntry("Controller vibration", false, description: "Adds controller vibration in few places.");
			Input_Controller_Vibration.OnValueChanged += (bool oldValue, bool newValue) => { Components.VibrationController.UseRumble = newValue; };
			Components.VibrationController.UseRumble = Input_Controller_Vibration.Value;
		}

		private void RegisterOtherSettings()
		{
			Entry_Other_SkipIntros = Category_otherSettings.CreateEntry("Skip intros", true, description: "Allows to skip splash screns / intros and go straight to menu! Disable in case of issues");
			Entry_Other_Prompts = Category_otherSettings.CreateEntry("Controller Prompts", "", description: "Asset bundle file name that is containing replacement prompts atlas. Make sure to use correct Steam Input binding for the keys to correspond to displayed prompts.");
			Entry_Other_ShowAdvanced = Category_otherSettings.CreateEntry("Show advanced settings", false, description: "Shows advanced options in GUI.");

			Entry_Other_InterpolateMovement = Category_otherSettings.CreateEntry("Interpolate movement", true, description: "Experimental: Enabled hooks related to movement interpolation of some rendered objects (protagonist and camera) to work around 50Hz Fixed Update stuttering - to my speedrunning friends: DO NOT allow this option for speedrunning.");
			Entry_Other_InterpolateMovement.OnValueChanged += (bool oldValue, bool newVal) => { Components.Interpolation.SmootherController.InterpolateMovement = newVal; };
			Components.Interpolation.SmootherController.InterpolateMovement = Entry_Other_InterpolateMovement.Value;

			Entry_Other_GeometryImprovements = Category_otherSettings.CreateEntry("Geometry improvements", GeometryImprovements.Enabled, description: "Runs additional scripts and shader replacement to improve geometry. Options are: \"Disabled\" / \"Enabled\" (only fixes some geometry issues and modifiers a few really bad LOD distance groups)");
			Entry_Other_LightImprovements = Category_otherSettings.CreateEntry("Improve lights", LightImprovements.Minor, description: "Modifies light sources to improve the game's looks. Options are: Disabled / Minor / All - All can introduce some performance problems. Minor should be generally safe.");
			Entry_Other_EnableCheats = Category_otherSettings.CreateEntry("Enable cheats", true, description: "Enables access to custom cheat menu.");
		}

		public LemonTuple<int, int> Resolution;
	}
}