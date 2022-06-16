using Il2CppSystem.IO;
using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SuisHack
{
	[RegisterTypeInIl2Cpp]
	public class SettingsGUI : MonoBehaviour
	{
		enum Category
		{
			Root,
			Display,
			Quality,
			Other
		}

		public SettingsGUI(IntPtr ptr) : base(ptr) { }
		public static SettingsGUI Instance { get; private set; }
		private bool m_Display = false;
		private Category category = Category.Root;
		private ExposedSettings Settings { get { return SuisHackMain.Settings; } }
		string resolutionX;
		string resolutionY;
		string refreshRate;
		float restartingToDefault = 0;

		public static void Initialize()
		{
			if (Instance == null)
			{
				var gameObject = new GameObject("SuisHackSettingsGUI");
				Instance = gameObject.AddComponent<SettingsGUI>();
				DontDestroyOnLoad(gameObject);
			}
		}

		void Start()
		{
			resolutionX = Screen.width.ToString();
			resolutionY = Screen.height.ToString();
			refreshRate = Settings.Entry_Display_RefreshRate.Value.ToString();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F11))
			{
				m_Display = !m_Display;
				if (m_Display)
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
				else
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
				}
			}
		}


		public void OnGUI()
		{
			if (m_Display)
			{
				GUILayout.BeginVertical(null);
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				DrawRoot();
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();

				switch (category)
				{
					case Category.Display:
						DrawDisplay();
						break;
					case Category.Quality:
						DrawQuality();
						break;
					case Category.Other:
						DrawOther();
						break;
				}
			}
		}



		private void DrawRoot()
		{
			GUILayout.Label("Options category:", null);
			if (GUILayout.Button("Display options", null))
			{
				category = Category.Display;
			}
			if (GUILayout.Button("Quality settings", null))
			{
				category = Category.Quality;
			}
			if (GUILayout.Button("Other settings", null))
			{
				category = Category.Other;
			}
			Settings.Entry_Other_ShowAdvanced.Value = GUILayout.Toggle(Settings.Entry_Other_ShowAdvanced.Value, "Show advanced and experimental options", null);
		}

		private void DrawDisplay()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;
			GUILayout.BeginHorizontal(GUI.skin.box, null);
			GUILayout.Label("<b>Display settings:</b>", richText, null);
			GUILayout.EndHorizontal();
			if (restartingToDefault > 5)
				restartingToDefault -= Time.deltaTime;

			//Resolution
			{
				GUILayout.BeginVertical(GUI.skin.box, null);
				GUILayout.BeginHorizontal(null);
				GUILayout.Label($"Current resolution ({Screen.width}x{Screen.height})", null);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(null);
				GUILayout.Label($"Desired resolution:", null);
				resolutionX = GUILayout.TextField(resolutionX, null);
				GUILayout.Label($"x", null);
				resolutionY = GUILayout.TextField(resolutionY, null);

				if (GUILayout.Button("Apply", null))
				{
					var newResX = Screen.width;
					var newResY = Screen.height;

					if (int.TryParse(resolutionX, out int resX))
					{
						if (resX > 0)
							newResX = resX;
					}

					if (int.TryParse(resolutionY, out int resY))
					{
						if (resY > 0)
							newResY = resY;
					}

					Screen.SetResolution(newResX, newResY, Settings.Entry_Display_DisplayMode.Value, Settings.Entry_Display_RefreshRate.Value);
					Settings.Entry_Display_Resolution.Value = $"{newResX}x{newResY}";
					resolutionX = newResX.ToString();
					resolutionY = newResY.ToString();
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

			//Display mode
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				GUILayout.Label($"Display mode ({Screen.fullScreenMode}):", null);

				if (GUILayout.Button("Exclusive fullscreen", null))
				{
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.ExclusiveFullScreen;
					Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Settings.Entry_Display_DisplayMode.Value, Screen.currentResolution.refreshRate);
					if (Settings.Entry_Display_Vsync.Value)
						Settings.Entry_Display_RefreshRate.Value = Screen.currentResolution.refreshRate;
				}
				if (GUILayout.Button("Maximized window", null))
				{
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.MaximizedWindow;
					Screen.SetResolution(Screen.width, Screen.height, Settings.Entry_Display_DisplayMode.Value, 0);
				}
				if (GUILayout.Button("Borderless window", null))
				{
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.FullScreenWindow;
					Screen.SetResolution(Screen.width, Screen.height, Settings.Entry_Display_DisplayMode.Value, 0);
				}
				if (GUILayout.Button("Windowed", null))
				{
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.Windowed;
					Screen.SetResolution(Screen.width, Screen.height, Settings.Entry_Display_DisplayMode.Value, 0);

				}
				GUILayout.EndHorizontal();
			}

			//Vsync
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				var oldValue = QualitySettings.vSyncCount > 0 ? true : false;
				var newValue = GUILayout.Toggle(oldValue, "V-sync", null);
				if (newValue != oldValue)
				{
					QualitySettings.vSyncCount = newValue ? 1 : 0;
					Settings.Entry_Display_Vsync.Value = newValue;
					if (newValue)
					{
						Application.targetFrameRate = Settings.Entry_Display_RefreshRate.Value;
					}
				}
				GUILayout.EndHorizontal();
			}

			//Refresh rate / FPS cap
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				if (QualitySettings.vSyncCount > 0)
				{
					GUILayout.Label("Refresh rate", null);
					refreshRate = GUILayout.TextField(refreshRate.ToString(), null);

					if (GUILayout.Button("Apply", null))
					{
						if (int.TryParse(refreshRate, out var result))
						{
							result = Mathf.Clamp(result, 50, 480);
							Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreenMode, result);
							Settings.Entry_Display_RefreshRate.Value = result;
							refreshRate = result.ToString();
						}
					}
				}
				else
				{
					GUILayout.Label("FPS cap", null);
					refreshRate = GUILayout.TextField(refreshRate.ToString(), null);

					if (GUILayout.Button("Apply", null))
					{
						if (int.TryParse(refreshRate, out var result))
						{
							result = Mathf.Clamp(result, -1, 9999);
							Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreenMode, 0);
							QualitySettings.vSyncCount = 0;
							Application.targetFrameRate = result;
							Settings.Entry_Display_RefreshRate.Value = result;
							refreshRate = result.ToString();
						}
					}
				}

				GUILayout.EndHorizontal();
			}
		}

		private void DrawQuality()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;
			GUILayout.BeginHorizontal(GUI.skin.box, null);
			GUILayout.Label("<b>Quality settings:</b>", richText, null);
			GUILayout.EndHorizontal();

			//Antialiasing
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				GUILayout.Label($"Antialiasing filter: ({Hacks.PostProcessLayerHook.GetShortName()}):", null);
				if (GUILayout.Button("None", null))
				{
					Hacks.PostProcessLayerHook.Antialiasing = PostProcessLayer.Antialiasing.None;
					Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.None;
				}
				if (GUILayout.Button("FXAA", null))
				{
					Hacks.PostProcessLayerHook.Antialiasing = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
					Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
				}
				if (GUILayout.Button("SMAA", null))
				{
					Hacks.PostProcessLayerHook.Antialiasing = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
					Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
				}
				if (GUILayout.Button("TAA", null))
				{
					Hacks.PostProcessLayerHook.Antialiasing = PostProcessLayer.Antialiasing.TemporalAntialiasing;
					Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.TemporalAntialiasing;
				}
				GUILayout.EndHorizontal();
			}

			//Anisotropic filtering Bias
			{
				GUILayout.BeginVertical(GUI.skin.box, null);
				GUILayout.BeginHorizontal(null);
				GUILayout.Label($"Anisotropic filtering mode: ({QualitySettings.anisotropicFiltering}):", null);

				if (GUILayout.Button("Force disabled", null))
				{
					QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
					Settings.Entry_AnistropicFiltering.Value = AnisotropicFiltering.Disable;
				}
				if (GUILayout.Button("Per texture", null))
				{
					QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
					Settings.Entry_AnistropicFiltering.Value = AnisotropicFiltering.Enable;
				}
				if (GUILayout.Button("Force enabled", null))
				{
					QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
					Settings.Entry_AnistropicFiltering.Value = AnisotropicFiltering.ForceEnable;
				}
				GUILayout.EndHorizontal();

				if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
				{
					var oldValue = Settings.Entry_AnistropicFilteringValue.Value;
					GUILayout.BeginHorizontal(null);
					GUILayout.Label($"Anisotropic filtering level ({oldValue}): ", null);
					Settings.Entry_AnistropicFilteringValue.Value = (int)GUILayout.HorizontalSlider(Settings.Entry_AnistropicFilteringValue.Value, -1, 16, null);
					GUILayout.EndHorizontal();

					if (oldValue != Settings.Entry_AnistropicFilteringValue.Value)
						Texture.SetGlobalAnisotropicFilteringLimits(Settings.Entry_AnistropicFilteringValue.Value, Settings.Entry_AnistropicFilteringValue.Value);
				}

				GUILayout.EndVertical();
			}

			//Shadows
			{
				GUILayout.BeginVertical(GUI.skin.box, null);

				//Shadow mode
				if (Settings.Entry_Other_ShowAdvanced.Value)
				{
					GUILayout.BeginHorizontal(null);
					GUILayout.Label($"Shadows ({QualitySettings.shadows}):", null);
					if (GUILayout.Button("None", null))
					{
						Settings.Entry_Quality_ShadowsQuality.Value = ShadowQuality.Disable;
						QualitySettings.shadows = ShadowQuality.Disable;
					}
					if (GUILayout.Button("Hard only", null))
					{
						Settings.Entry_Quality_ShadowsQuality.Value = ShadowQuality.HardOnly;
						QualitySettings.shadows = ShadowQuality.HardOnly;
					}
					if (GUILayout.Button("Soft", null))
					{
						Settings.Entry_Quality_ShadowsQuality.Value = ShadowQuality.All;
						QualitySettings.shadows = ShadowQuality.All;
					}
					GUILayout.EndHorizontal();
				}

				//Shadow resolution
				{
					GUILayout.BeginHorizontal(null);
					GUILayout.Label($"Shadow resolution ({QualitySettings.shadowResolution}):", null);
					if (GUILayout.Button("Low", null))
					{
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.Low;
						QualitySettings.shadowResolution = ShadowResolution.Low;
					}
					if (GUILayout.Button("Medium", null))
					{
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.Medium;
						QualitySettings.shadowResolution = ShadowResolution.Medium;
					}
					if (GUILayout.Button("High", null))
					{
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.High;
						QualitySettings.shadowResolution = ShadowResolution.High;
					}
					if (GUILayout.Button("Very High", null))
					{
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.VeryHigh;
						QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
					}
					GUILayout.EndHorizontal();
				}

				//Shadow mask mode
				if (Settings.Entry_Other_ShowAdvanced.Value)
				{
					GUILayout.BeginHorizontal(null);
					GUILayout.Label($"Shadow mask mode ({QualitySettings.shadowmaskMode}):", null);
					if (GUILayout.Button("Shadowmask", null))
					{
						Settings.Entry_Quality_ShadowMaskMode.Value = ShadowmaskMode.Shadowmask;
						QualitySettings.shadowmaskMode = ShadowmaskMode.Shadowmask;
					}
					if (GUILayout.Button("Distance shadowmask", null))
					{
						Settings.Entry_Quality_ShadowMaskMode.Value = ShadowmaskMode.DistanceShadowmask;
						QualitySettings.shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
					}
					GUILayout.EndHorizontal();
				}

				//Shadow Projection
				if (Settings.Entry_Other_ShowAdvanced.Value)
				{
					GUILayout.BeginHorizontal(null);
					GUILayout.Label($"Shadow projection mode ({QualitySettings.shadowProjection}):", null);
					if (GUILayout.Button("Close Fit", null))
					{
						Settings.Entry_Quality_ShadowProjectionMode.Value = ShadowProjection.CloseFit;
						QualitySettings.shadowProjection = ShadowProjection.CloseFit;
					}
					if (GUILayout.Button("Stable fit", null))
					{
						Settings.Entry_Quality_ShadowProjectionMode.Value = ShadowProjection.StableFit;
						QualitySettings.shadowProjection = ShadowProjection.StableFit;
					}
					GUILayout.EndHorizontal();
				}

				//Shadow distance
				{
					GUILayout.BeginHorizontal(null);
					var val = QualitySettings.shadowDistance;
					GUILayout.Label($"Shadow distance: ({QualitySettings.shadowDistance}):", null);
					val = Mathf.Round(GUILayout.HorizontalSlider(val, 10, 1000, null));

					if (val != QualitySettings.shadowDistance)
					{
						QualitySettings.shadowDistance = val;
						Settings.Entry_Quality_ShadowDistance.Value = val;
					}
					GUILayout.EndHorizontal();
				}

				{
					GUILayout.BeginHorizontal(null);
					GUILayout.Label($"Shadow cascades: ({QualitySettings.shadowCascades}):", null);
					if (GUILayout.Button("2", null))
					{
						Settings.Entry_Quality_Use4ShadowCascades.Value = false;
						QualitySettings.shadowCascades = 2;
					}
					if (GUILayout.Button("4", null))
					{
						Settings.Entry_Quality_Use4ShadowCascades.Value = true;
						QualitySettings.shadowCascades = 4;
					}
					GUILayout.EndHorizontal();
				}

				if (Settings.Entry_Other_ShowAdvanced.Value)
				{
					if (QualitySettings.shadowCascades == 2)
					{
						GUILayout.BeginHorizontal(null);
						GUILayout.Label($"Shadow cascades  2 split: ({QualitySettings.shadowCascade2Split}):", null);
						float value = QualitySettings.shadowCascade2Split;
						QualitySettings.shadowCascade2Split = GUILayout.HorizontalSlider(value, 0.01f, 0.5f, null);
						if (QualitySettings.shadowCascade2Split != value)
							Settings.Entry_Quality_ShadowTwoSplitValue.Value = QualitySettings.shadowCascade2Split;
						GUILayout.EndHorizontal();
					}
					else
					{
						float cascade1 = QualitySettings.shadowCascade4Split.x;
						float cascade2 = QualitySettings.shadowCascade4Split.y;
						float cascade3 = QualitySettings.shadowCascade4Split.z;
						GUILayout.BeginHorizontal(null);
						GUILayout.Label($"Shadow cascades 1 split: ({cascade1}):", null);
						cascade1 = GUILayout.HorizontalSlider(cascade1, 0.01f, 0.5f, null);
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(null);
						GUILayout.Label($"Shadow cascades 2 split: ({cascade2}):", null);
						cascade2 = GUILayout.HorizontalSlider(cascade2, 0.1f, 0.8f, null);
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(null);
						GUILayout.Label($"Shadow cascades 3 split: ({cascade3}):", null);
						cascade3 = GUILayout.HorizontalSlider(cascade3, 0.02f, 0.9f, null);
						GUILayout.EndHorizontal();

						if (cascade2 < cascade1)
							cascade2 = cascade1 + 0.001f;

						if (cascade3 < cascade2)
							cascade3 = cascade2 + 0.001f;

						if (cascade1 != QualitySettings.shadowCascade4Split.x || cascade2 != QualitySettings.shadowCascade4Split.y || cascade3 != QualitySettings.shadowCascade4Split.z)
						{
							QualitySettings.shadowCascade4Split = new Vector3(cascade1, cascade2, cascade3);
							Settings.Entry_Quality_ShadowFourSplitValue1.Value = cascade1;
							Settings.Entry_Quality_ShadowFourSplitValue2.Value = cascade2;
							Settings.Entry_Quality_ShadowFourSplitValue3.Value = cascade3;
						}
					}
				}
				GUILayout.EndVertical();
			}

			//LOD Bias
			if (Settings.Entry_Other_ShowAdvanced.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				var oldValue = QualitySettings.lodBias;
				GUILayout.Label($"LOD Bias ({QualitySettings.lodBias.ToString("0.0")}):", null);
				QualitySettings.lodBias = GUILayout.HorizontalSlider(QualitySettings.lodBias, 0.5f, 4f, null);
				if (oldValue != QualitySettings.lodBias)
				{
					Settings.Entry_Quality_LODBias.Value = QualitySettings.lodBias;
				}
				GUILayout.EndHorizontal();
			}

			//Pixel Light Count
			if (Settings.Entry_Other_ShowAdvanced.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				var oldValue = QualitySettings.pixelLightCount;
				GUILayout.Label($"Pixel light count: ({QualitySettings.pixelLightCount}):", null);
				QualitySettings.pixelLightCount = (int)GUILayout.HorizontalSlider(QualitySettings.pixelLightCount, 0, 8, null);
				if (oldValue != QualitySettings.pixelLightCount)
				{
					Settings.Entry_Quality_PixelLightCount.Value = QualitySettings.pixelLightCount;
				}
				GUILayout.EndHorizontal();
			}

			//Texture Quality Light Count
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				GUILayout.Label($"Texture resolution ({GetTextureString(QualitySettings.masterTextureLimit)}):", null);

				var oldValue = QualitySettings.masterTextureLimit;
				if (GUILayout.Button("Full", null))
					QualitySettings.masterTextureLimit = 0;
				if (GUILayout.Button("Half", null))
					QualitySettings.masterTextureLimit = 1;

				if (oldValue != QualitySettings.masterTextureLimit)
					Settings.Entry_Quality_TextureQuality.Value = QualitySettings.masterTextureLimit;
				GUILayout.EndHorizontal();
			}

			//Real-time reflection probes toggle
			if (Settings.Entry_Other_ShowAdvanced.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				var newValue = GUILayout.Toggle(QualitySettings.realtimeReflectionProbes, "Realtime Reflection probes", null);

				if (newValue != QualitySettings.realtimeReflectionProbes)
				{
					QualitySettings.realtimeReflectionProbes = newValue;
					Settings.Entry_Quality_RealtimeReflectionProbes.Value = newValue;
				}
				GUILayout.EndHorizontal();
			}

			//Soft particles
			if (Settings.Entry_Other_ShowAdvanced.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				var newValue = GUILayout.Toggle(QualitySettings.softParticles, "Soft particles", null);

				if (newValue != QualitySettings.softParticles)
				{
					QualitySettings.softParticles = newValue;
				}
				GUILayout.EndHorizontal();
			}

			//Soft vegetation
			if (Settings.Entry_Other_ShowAdvanced.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				var newValue = GUILayout.Toggle(QualitySettings.softVegetation, "Soft vegetation", null);

				if (newValue != QualitySettings.softVegetation)
				{
					QualitySettings.softVegetation = newValue;
				}
				GUILayout.EndHorizontal();
			}

			//Color space
			if (Settings.Entry_Other_ShowAdvanced.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				GUILayout.Label($"Color space: {QualitySettings.activeColorSpace}", null);
				GUILayout.Label($"Desired color space: {QualitySettings.desiredColorSpace}", null);
				GUILayout.EndHorizontal();
			}

			if (GUILayout.Button("Unlock restart to defaults", null))
				restartingToDefault = 5f;
			if (restartingToDefault > 0)
			{
				if (GUILayout.Button($"Restart to default {restartingToDefault:0.0} ", null))
				{
					RestartToDefault();
				}
			}
		}

		private void RestartToDefault()
		{
			Settings.Entry_AnistropicFiltering.Value = Settings.Entry_AnistropicFiltering.DefaultValue;
			QualitySettings.anisotropicFiltering = Settings.Entry_AnistropicFiltering.Value;

			Settings.Entry_AnistropicFilteringValue.Value = Settings.Entry_AnistropicFilteringValue.DefaultValue;
			Texture.SetGlobalAnisotropicFilteringLimits(8, 16);

			Settings.Entry_Antialiasing.Value = Settings.Entry_Antialiasing.DefaultValue;
			Hacks.PostProcessLayerHook.Antialiasing = Settings.Entry_Antialiasing.Value;

			Settings.Entry_Quality_LODBias.Value = Settings.Entry_Quality_LODBias.DefaultValue;
			QualitySettings.lodBias = Settings.Entry_Quality_LODBias.Value;

			Settings.Entry_Quality_PixelLightCount.Value = Settings.Entry_Quality_PixelLightCount.DefaultValue;
			QualitySettings.pixelLightCount = Settings.Entry_Quality_PixelLightCount.Value;

			Settings.Entry_Quality_RealtimeReflectionProbes.Value = Settings.Entry_Quality_RealtimeReflectionProbes.DefaultValue;
			QualitySettings.realtimeReflectionProbes = Settings.Entry_Quality_RealtimeReflectionProbes.Value;

			Settings.Entry_Quality_ShadowDistance.Value = Settings.Entry_Quality_ShadowDistance.DefaultValue;
			QualitySettings.shadowDistance = Settings.Entry_Quality_ShadowDistance.Value;

			Settings.Entry_Quality_ShadowFourSplitValue1.Value = Settings.Entry_Quality_ShadowFourSplitValue1.DefaultValue;
			Settings.Entry_Quality_ShadowFourSplitValue2.Value = Settings.Entry_Quality_ShadowFourSplitValue2.DefaultValue;
			Settings.Entry_Quality_ShadowFourSplitValue3.Value = Settings.Entry_Quality_ShadowFourSplitValue3.DefaultValue;
			QualitySettings.shadowCascade4Split = new Vector3(Settings.Entry_Quality_ShadowFourSplitValue1.Value, Settings.Entry_Quality_ShadowFourSplitValue2.Value, Settings.Entry_Quality_ShadowFourSplitValue3.Value);

			Settings.Entry_Quality_ShadowMaskMode.Value = Settings.Entry_Quality_ShadowMaskMode.DefaultValue;
			QualitySettings.shadowmaskMode = Settings.Entry_Quality_ShadowMaskMode.Value;

			Settings.Entry_Quality_ShadowProjectionMode.Value = Settings.Entry_Quality_ShadowProjectionMode.DefaultValue;
			QualitySettings.shadowProjection = Settings.Entry_Quality_ShadowProjectionMode.Value;

			Settings.Entry_Quality_ShadowsQuality.Value = Settings.Entry_Quality_ShadowsQuality.DefaultValue;
			QualitySettings.shadows = Settings.Entry_Quality_ShadowsQuality.Value;

			Settings.Entry_Quality_ShadowsResolution.Value = Settings.Entry_Quality_ShadowsResolution.DefaultValue;
			QualitySettings.shadowResolution = Settings.Entry_Quality_ShadowsResolution.Value;

			Settings.Entry_Quality_ShadowTwoSplitValue.Value = Settings.Entry_Quality_ShadowTwoSplitValue.DefaultValue;
			QualitySettings.shadowCascade2Split = Settings.Entry_Quality_ShadowTwoSplitValue.Value;

			Settings.Entry_Quality_TextureQuality.Value = Settings.Entry_Quality_TextureQuality.DefaultValue;
			QualitySettings.masterTextureLimit = Settings.Entry_Quality_TextureQuality.Value;

			Settings.Entry_Quality_Use4ShadowCascades.Value = Settings.Entry_Quality_Use4ShadowCascades.DefaultValue;
			QualitySettings.shadowCascades = Settings.Entry_Quality_Use4ShadowCascades.Value ? 4 : 2;
		}

		private string GetTextureString(int masterTextureLimit)
		{
			//I wish I could use 8.0 recursive patern
			switch (masterTextureLimit)
			{
				case 1:
					return "Half";
				default:
					return "Full";
			}
		}

		private void DrawOther()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;
			GUILayout.BeginHorizontal(GUI.skin.box, null);
			GUILayout.Label("<b>Other settings:</b>", richText, null);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box, null);
			GUILayout.Label("Following settings require game restart!", null);
			Settings.Entry_Other_SkipIntros.Value = GUILayout.Toggle(Settings.Entry_Other_SkipIntros.Value, "Skip intros", null);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box, null);
			GUILayout.BeginVertical(null);
			var promptsUsed = Settings.Entry_Other_Prompts.Value == "" ? "None" : Settings.Entry_Other_Prompts.Value;

			GUILayout.Label($"Prompts used: \"{promptsUsed}\" - possible:", null);
			if (GUILayout.Button("None", null))
				Settings.Entry_Other_Prompts.Value = "";

			var files = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Prompts"), "*.manifest");
			foreach (var file in files)
			{
				var split = file.Split('\\', '/', '.');
				var fileName = split[split.Length - 2];
				if (GUILayout.Button(fileName, null))
				{
					Settings.Entry_Other_Prompts.Value = fileName;
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
	}
}
