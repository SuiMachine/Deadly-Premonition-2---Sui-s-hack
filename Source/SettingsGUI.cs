using Il2CppSystem.IO;
using SuisHack.Hacks;
using SuisHack.KeyboardSupport;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static SuisHack.KeyboardSupport.SteamInputHook;

namespace SuisHack
{
	public class SettingsGUI : MonoBehaviour
	{
		enum Category
		{
			Root,
			Display,
			Quality,
			Input,
			Other
		}

		public SettingsGUI(IntPtr handle) : base(handle) { }

		public static SettingsGUI Instance { get; private set; }
		public static bool Display { get; private set; }
		private Category category = Category.Root;
		string resolutionX;
		string resolutionY;
		string refreshRate;
		float restartingToDefault = 0;
		private RebindingActions CurrentRebinding;

		public static void Initialize()
		{
			if (Instance == null)
			{
				var gameObject = new GameObject("SuisHackSettingsGUI");
				Instance = gameObject.AddComponent<SettingsGUI>();
			}
		}

		public void Awake()
		{
			DontDestroyOnLoad(this.gameObject);
			this.gameObject.hideFlags = HideFlags.HideAndDontSave;
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F11))
			{
				CurrentRebinding = RebindingActions.None;
				Display = !Display;
				if (Display)
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
			if (Display)
			{
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal(GUI.skin.box);
				DrawRoot();
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				if (restartingToDefault > 0)
					restartingToDefault -= Time.deltaTime;

				switch (category)
				{
					case Category.Display:
						DrawDisplay();
						break;
					case Category.Quality:
						DrawQuality();
						break;
					case Category.Input:
						switch (ExposedSettings.Instance.Input_Override.Value)
						{
							case ExposedSettings.InputType.SteamInput:
								DrawSteamInput();
								break;
							case ExposedSettings.InputType.KeyboardAndMouse:
								DrawKeyboardAndMouseInput();
								break;
						}
						break;
					case Category.Other:
						DrawOther();
						break;
				}
			}
		}

		private void DrawRoot()
		{
			GUILayout.Label("Options category:");
			if (GUILayout.Button("Display options"))
			{
				category = Category.Display;
			}
			if (GUILayout.Button("Quality settings"))
			{
				category = Category.Quality;
			}
			if (GUILayout.Button("Input settings"))
			{
				category = Category.Input;
			}
			if (GUILayout.Button("Other settings"))
			{
				category = Category.Other;
			}
			ExposedSettings.Instance.Entry_Other_ShowAdvanced.Value = GUILayout.Toggle(ExposedSettings.Instance.Entry_Other_ShowAdvanced.Value, "Show advanced and experimental options");
		}

		private void DrawDisplay()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;

			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label("<b>Display settings:</b>", richText);
			GUILayout.EndHorizontal();

			//Resolution
			{
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.BeginHorizontal();
				GUILayout.Label($"Current resolution ({Screen.width}x{Screen.height})");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label($"Desired resolution:");
				resolutionX = GUILayout.TextField(resolutionX);
				GUILayout.Label($"x");
				resolutionY = GUILayout.TextField(resolutionY);
				GUILayout.Label($"@");
				refreshRate = GUILayout.TextField(refreshRate);
				GUILayout.Label($"Hz");

				if (GUILayout.Button("Apply"))
				{
					var newResX = Screen.width;
					var newResY = Screen.height;
					var newRefreshRate = Screen.currentResolution.refreshRate;

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

					if (int.TryParse(refreshRate, out int refr))
					{
						if (refr >= 0)
							newRefreshRate = refr;
					}

					Screen.SetResolution(newResX, newResY, ExposedSettings.Instance.Entry_Display_DisplayMode.Value, newRefreshRate);
					ExposedSettings.Instance.Entry_Display_Resolution.Value = $"{newResX}x{newResY}";
					resolutionX = newResX.ToString();
					resolutionY = newResY.ToString();
					refreshRate = newRefreshRate.ToString();
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

			//Display mode
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				GUILayout.Label($"Display mode ({Screen.fullScreenMode}):");

				if (GUILayout.Button("Exclusive fullscreen"))
				{
					ExposedSettings.Instance.Entry_Display_DisplayMode.Value = FullScreenMode.ExclusiveFullScreen;
					Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, ExposedSettings.Instance.Entry_Display_DisplayMode.Value, Screen.currentResolution.refreshRate);
					if (ExposedSettings.Instance.Entry_Display_Vsync.Value)
						ExposedSettings.Instance.Entry_Display_RefreshRate.Value = Screen.currentResolution.refreshRate;
				}
				if (GUILayout.Button("Maximized window"))
				{
					ExposedSettings.Instance.Entry_Display_DisplayMode.Value = FullScreenMode.MaximizedWindow;
					Screen.SetResolution(Screen.width, Screen.height, ExposedSettings.Instance.Entry_Display_DisplayMode.Value, 0);
				}
				if (GUILayout.Button("Borderless window"))
				{
					ExposedSettings.Instance.Entry_Display_DisplayMode.Value = FullScreenMode.FullScreenWindow;
					Screen.SetResolution(Screen.width, Screen.height, ExposedSettings.Instance.Entry_Display_DisplayMode.Value, 0);
				}
				if (GUILayout.Button("Windowed"))
				{
					ExposedSettings.Instance.Entry_Display_DisplayMode.Value = FullScreenMode.Windowed;
					Screen.SetResolution(Screen.width, Screen.height, ExposedSettings.Instance.Entry_Display_DisplayMode.Value, 0);

				}
				GUILayout.EndHorizontal();
			}

			//Vsync
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				var oldValue = QualitySettings.vSyncCount > 0;
				var newValue = GUILayout.Toggle(oldValue, "V-sync");
				if (newValue != oldValue)
				{
					QualitySettings.vSyncCount = newValue ? 1 : 0;
					ExposedSettings.Instance.Entry_Display_Vsync.Value = newValue;
					if (!newValue)
					{
						Application.targetFrameRate = ExposedSettings.Instance.Entry_DesiredFramerate.Value;
					}
				}
				GUILayout.EndHorizontal();
			}

			//FPS cap
			{
				if (QualitySettings.vSyncCount == 0)
				{
					GUILayout.BeginHorizontal(GUI.skin.box);
					GUILayout.Label($"FPS cap ({ExposedSettings.Instance.Entry_DesiredFramerate.Value})");
					ExposedSettings.Instance.Entry_DesiredFramerate.Value = (int)GUILayout.HorizontalSlider(ExposedSettings.Instance.Entry_DesiredFramerate.Value, -1, 1000);
					GUILayout.EndHorizontal();
				}
			}
		}

		private void DrawQuality()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label("<b>Quality settings:</b>", richText);
			GUILayout.EndHorizontal();

			//Antialiasing
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				GUILayout.Label($"Antialiasing filter: ({Hacks.PostProcessLayerHook.GetShortName()}):");
				if (GUILayout.Button("None"))
					ExposedSettings.Instance.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.None;
				if (GUILayout.Button("FXAA"))
					ExposedSettings.Instance.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;

				if (GUILayout.Button("SMAA"))
					ExposedSettings.Instance.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
				if (ExposedSettings.Instance.Entry_Other_ShowAdvanced.Value)
				{
					if (GUILayout.Button("TAA (Epilepsy warning!)"))
						ExposedSettings.Instance.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.TemporalAntialiasing;
				}


				GUILayout.EndHorizontal();
			}

			//Anisotropic filtering Bias
			{
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.BeginHorizontal();
				GUILayout.Label($"Anisotropic filtering mode: ({QualitySettings.anisotropicFiltering}):");

				if (GUILayout.Button("Force disabled"))
					ExposedSettings.Instance.Entry_AnistropicFiltering.Value = AnisotropicFiltering.Disable;

				if (GUILayout.Button("Per texture"))
					ExposedSettings.Instance.Entry_AnistropicFiltering.Value = AnisotropicFiltering.Enable;

				if (GUILayout.Button("Force enabled"))
					ExposedSettings.Instance.Entry_AnistropicFiltering.Value = AnisotropicFiltering.ForceEnable;

				GUILayout.EndHorizontal();

				if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
				{
					var oldValue = ExposedSettings.Instance.Entry_AnistropicFilteringValue.Value;
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Anisotropic filtering level ({oldValue}): ");
					ExposedSettings.Instance.Entry_AnistropicFilteringValue.Value = (int)GUILayout.HorizontalSlider(ExposedSettings.Instance.Entry_AnistropicFilteringValue.Value, -1, 16);
					GUILayout.EndHorizontal();
				}

				GUILayout.EndVertical();
			}

			//Shadows
			{
				GUILayout.BeginVertical(GUI.skin.box);

				//Shadow mode
				if (ExposedSettings.Instance.Entry_Other_ShowAdvanced.Value)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Shadows ({QualitySettings.shadows}):");
					if (GUILayout.Button("None"))
						ExposedSettings.Instance.Entry_Quality_ShadowsQuality.Value = ShadowQuality.Disable;

					if (GUILayout.Button("Hard only"))
						ExposedSettings.Instance.Entry_Quality_ShadowsQuality.Value = ShadowQuality.HardOnly;

					if (GUILayout.Button("Soft"))
						ExposedSettings.Instance.Entry_Quality_ShadowsQuality.Value = ShadowQuality.All;

					GUILayout.EndHorizontal();
				}

				//Shadow resolution
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Shadow resolution ({QualitySettings.shadowResolution}):");
					if (GUILayout.Button("Low"))
						ExposedSettings.Instance.Entry_Quality_ShadowsResolution.Value = ShadowResolution.Low;

					if (GUILayout.Button("Medium"))
						ExposedSettings.Instance.Entry_Quality_ShadowsResolution.Value = ShadowResolution.Medium;

					if (GUILayout.Button("High"))
						ExposedSettings.Instance.Entry_Quality_ShadowsResolution.Value = ShadowResolution.High;

					if (GUILayout.Button("Very High"))
						ExposedSettings.Instance.Entry_Quality_ShadowsResolution.Value = ShadowResolution.VeryHigh;

					GUILayout.EndHorizontal();
				}

				//Shadow distance
				{
					GUILayout.BeginHorizontal();
					var val = QualitySettings.shadowDistance;
					GUILayout.Label($"Shadow distance: ({QualitySettings.shadowDistance}):");
					val = Mathf.Round(GUILayout.HorizontalSlider(val, 10, 1000));

					if (val != QualitySettings.shadowDistance)
					{
						QualitySettings.shadowDistance = val;
					}
					GUILayout.EndHorizontal();
				}

				{
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Shadow cascades: ({QualitySettings.shadowCascades}):");
					if (GUILayout.Button("2"))
					{
						ExposedSettings.Instance.Entry_Quality_Use4ShadowCascades.Value = false;
					}
					if (GUILayout.Button("4"))
					{
						ExposedSettings.Instance.Entry_Quality_Use4ShadowCascades.Value = true;
					}
					GUILayout.EndHorizontal();
				}

				//Mesh replacement
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Replace shadow meshes: {ExposedSettings.Instance.Entry_Quality_ReplaceShadowMeshes.Value}:");
					if (GUILayout.Button("Enable"))
						ExposedSettings.Instance.Entry_Quality_ReplaceShadowMeshes.Value = true;
					if (GUILayout.Button("Disable"))
						ExposedSettings.Instance.Entry_Quality_ReplaceShadowMeshes.Value = false;
					GUILayout.EndHorizontal();
				}

				if (ExposedSettings.Instance.Entry_Other_ShowAdvanced.Value)
				{
					if (QualitySettings.shadowCascades == 2)
					{
						GUILayout.BeginHorizontal();
						GUILayout.Label($"Shadow cascades  2 split: ({QualitySettings.shadowCascade2Split:0.00}):");
						float value = QualitySettings.shadowCascade2Split;
						QualitySettings.shadowCascade2Split = GUILayout.HorizontalSlider(value, 0.01f, 0.5f);
						if (QualitySettings.shadowCascade2Split != value)
							ExposedSettings.Instance.Entry_Quality_ShadowTwoSplitValue.Value = QualitySettings.shadowCascade2Split;
						GUILayout.EndHorizontal();
					}
					else
					{
						float cascade1 = QualitySettings.shadowCascade4Split.x;
						float cascade2 = QualitySettings.shadowCascade4Split.y;
						float cascade3 = QualitySettings.shadowCascade4Split.z;
						GUILayout.BeginHorizontal();
						GUILayout.Label($"Shadow cascades 1 split: ({cascade1:0.000}):");
						cascade1 = GUILayout.HorizontalSlider(cascade1, 0.01f, 0.5f);
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
						GUILayout.Label($"Shadow cascades 2 split: ({cascade2:0.000}):");
						cascade2 = GUILayout.HorizontalSlider(cascade2, 0.1f, 0.8f);
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
						GUILayout.Label($"Shadow cascades 3 split: ({cascade3:0.000}):");
						cascade3 = GUILayout.HorizontalSlider(cascade3, 0.02f, 0.9f);
						GUILayout.EndHorizontal();

						if (cascade2 < cascade1)
							cascade2 = cascade1 + 0.001f;

						if (cascade3 < cascade2)
							cascade3 = cascade2 + 0.001f;

						if (cascade1 != QualitySettings.shadowCascade4Split.x || cascade2 != QualitySettings.shadowCascade4Split.y || cascade3 != QualitySettings.shadowCascade4Split.z)
						{
							QualitySettings.shadowCascade4Split = new Vector3(cascade1, cascade2, cascade3);
							ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue1.Value = cascade1;
							ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue2.Value = cascade2;
							ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue3.Value = cascade3;
						}
					}
				}
				GUILayout.EndVertical();
			}

			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				var oldValue = Hacks.PostProcessLayerHook.FarClipDistance;
				GUILayout.Label($"Far clip distance ({Hacks.PostProcessLayerHook.FarClipDistance}):");
				var newValue = Mathf.Round(GUILayout.HorizontalSlider(oldValue, 400, 2000f));
				if (newValue != oldValue)
					ExposedSettings.Instance.Entry_Quality_CameraFarPlaneDistance.Value = newValue;

				GUILayout.EndHorizontal();
			}

			//LOD Bias
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				var oldValue = QualitySettings.lodBias;
				GUILayout.Label($"LOD Bias ({QualitySettings.lodBias:0.0}):");
				QualitySettings.lodBias = GUILayout.HorizontalSlider(QualitySettings.lodBias, 0.5f, 4f);
				if (oldValue != QualitySettings.lodBias)
					ExposedSettings.Instance.Entry_Quality_LODBias.Value = QualitySettings.lodBias;

				GUILayout.EndHorizontal();
			}

			//Pixel Light Count
			if (ExposedSettings.Instance.Entry_Other_ShowAdvanced.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				var oldValue = QualitySettings.pixelLightCount;
				GUILayout.Label($"Pixel light count: ({QualitySettings.pixelLightCount}):");
				QualitySettings.pixelLightCount = (int)GUILayout.HorizontalSlider(QualitySettings.pixelLightCount, 0, 8);
				if (oldValue != QualitySettings.pixelLightCount)
					ExposedSettings.Instance.Entry_Quality_PixelLightCount.Value = QualitySettings.pixelLightCount;

				GUILayout.EndHorizontal();
			}

			//Texture Quality
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				GUILayout.Label($"Texture resolution ({GetTextureString(QualitySettings.masterTextureLimit)}):");

				if (GUILayout.Button("Full"))
					ExposedSettings.Instance.Entry_Quality_TextureQuality.Value = 0;
				if (GUILayout.Button("Half"))
					ExposedSettings.Instance.Entry_Quality_TextureQuality.Value = 1;

				GUILayout.EndHorizontal();
			}

			//Planar reflections
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				GUILayout.Label($"Planar reflections resolution ({Hacks.MirrorReflectionHook.ReflectionSize}):");
				var log = Mathf.RoundToInt(Mathf.Log(Hacks.MirrorReflectionHook.ReflectionSize, 2));
				var newLog = (int)GUILayout.HorizontalSlider(log, 7, 11);
				if (newLog != log)
					ExposedSettings.Instance.Entry_Quality_MirrorReflectionResolution.Value = (int)Mathf.Pow(2, newLog);

				GUILayout.EndHorizontal();
			}

			//HBAO
			{
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.BeginHorizontal();
				GUILayout.Label($"HBAO preset ({Hacks.PostProcessLayerHook.HBAO_Preset}):");
				if (GUILayout.Button("Fastest"))
					ExposedSettings.Instance.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.FastestPerformance;
				if (GUILayout.Button("Fast"))
					ExposedSettings.Instance.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.FastPerformance;
				if (GUILayout.Button("Normal"))
					ExposedSettings.Instance.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.Normal;
				if (GUILayout.Button("High"))
					ExposedSettings.Instance.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.HighQuality;
				if (GUILayout.Button("Highest"))
					ExposedSettings.Instance.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.HighestQuality;
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label($"HBAO intensity ({Hacks.PostProcessLayerHook.HBAO_Intensity:0.0}):");
				ExposedSettings.Instance.Entry_Quality_HBAO_Intensity.Value = GUILayout.HorizontalSlider(ExposedSettings.Instance.Entry_Quality_HBAO_Intensity.Value, 0.0f, 1.0f);
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

			//Meh
			{
				GUILayout.BeginVertical(GUI.skin.box);
				ExposedSettings.Instance.Entry_Quality_EdgeDetection.Value = GUILayout.Toggle(ExposedSettings.Instance.Entry_Quality_EdgeDetection.Value, "Edge detection post process filter");

				if (ExposedSettings.Instance.Entry_Quality_EdgeDetection.Value && ExposedSettings.Instance.Entry_Quality_CameraFarPlaneDistance.Value > 800)
				{
					GUILayout.Label("Warning - extending far clip camera plane distance can cause issues with edge detection filter.\nIf you experience issues with lines on characters being displayed up close, try lowering the value below:");
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Edge detection depth {ExposedSettings.Instance.Entry_Quality_EdgeDetectionDepth.Value:0.00}");
					ExposedSettings.Instance.Entry_Quality_EdgeDetectionDepth.Value = GUILayout.HorizontalSlider(ExposedSettings.Instance.Entry_Quality_EdgeDetectionDepth.Value, 0, 1);
					GUILayout.EndHorizontal();
				}
				else
					ExposedSettings.Instance.Entry_Quality_EdgeDetectionDepth.Value = 1;

				GUILayout.EndVertical();
			}

			//SSR
			{
				GUILayout.BeginVertical(GUI.skin.box);
				ExposedSettings.Instance.Entry_Quality_SSR_Enable.Value = GUILayout.Toggle(ExposedSettings.Instance.Entry_Quality_SSR_Enable.Value, "Enable SSR override");

				if (ExposedSettings.Instance.Entry_Quality_SSR_Enable.Value)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label($"SSR preset ({Hacks.PostProcessLayerHook.SSR_Preset}):");
					if (GUILayout.Button("Lower"))
						ExposedSettings.Instance.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Lower;
					if (GUILayout.Button("Low"))
						ExposedSettings.Instance.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Low;
					if (GUILayout.Button("Medium"))
						ExposedSettings.Instance.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Medium;
					if (GUILayout.Button("High"))
						ExposedSettings.Instance.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.High;
					if (GUILayout.Button("Higher"))
						ExposedSettings.Instance.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Higher;
					if (GUILayout.Button("Ultra"))
						ExposedSettings.Instance.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Ultra;
					if (GUILayout.Button("Overkill"))
						ExposedSettings.Instance.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Overkill;
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.Label($"SSR resolution ({Hacks.PostProcessLayerHook.SSR_Resolution}):");
					if (GUILayout.Button("Downsampled"))
						ExposedSettings.Instance.Entry_Quality_SSR_Resolution.Value = ScreenSpaceReflectionResolution.Downsampled;
					if (GUILayout.Button("FullSize"))
						ExposedSettings.Instance.Entry_Quality_SSR_Resolution.Value = ScreenSpaceReflectionResolution.FullSize;
					if (GUILayout.Button("Supersampled"))
						ExposedSettings.Instance.Entry_Quality_SSR_Resolution.Value = ScreenSpaceReflectionResolution.Supersampled;
					GUILayout.EndHorizontal();

					if (ExposedSettings.Instance.Entry_Other_ShowAdvanced.Value)
					{
						GUILayout.BeginHorizontal();
						GUILayout.Label($"SSR Tickness ({Hacks.PostProcessLayerHook.SSR_Tickness:0.0}):");
						ExposedSettings.Instance.Entry_Quality_SSR_Tickness.Value = GUILayout.HorizontalSlider(ExposedSettings.Instance.Entry_Quality_SSR_Tickness.Value, 0, 1);
						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal();
						GUILayout.Label($"SSR Vignette ({Hacks.PostProcessLayerHook.SSR_Vignette:0.0}):");
						ExposedSettings.Instance.Entry_Quality_SSR_Vignette.Value = GUILayout.HorizontalSlider(ExposedSettings.Instance.Entry_Quality_SSR_Vignette.Value, 0, 1);
						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal();
						GUILayout.Label($"SSR Distance Fade ({Hacks.PostProcessLayerHook.SSR_DistanceFade:0.00}):");
						ExposedSettings.Instance.Entry_Quality_SSR_DistanceFade.Value = GUILayout.HorizontalSlider(ExposedSettings.Instance.Entry_Quality_SSR_DistanceFade.Value, 0, 0.5f);
						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal();
						GUILayout.Label($"SSR Max Marching Distance ({Hacks.PostProcessLayerHook.SSR_MaxMarchingDistance:0}):");
						ExposedSettings.Instance.Entry_Quality_SSR_MaxMarchingDistance.Value = GUILayout.HorizontalSlider(ExposedSettings.Instance.Entry_Quality_SSR_MaxMarchingDistance.Value, 50, 250);
						GUILayout.EndHorizontal();
					}
				}
				GUILayout.EndVertical();
			}

			//Color space
			if (ExposedSettings.Instance.Entry_Other_ShowAdvanced.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				GUILayout.Label($"Color space: {QualitySettings.activeColorSpace}");
				GUILayout.Label($"Desired color space: {QualitySettings.desiredColorSpace}");
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal(GUI.skin.box);
			if (GUILayout.Button("Unlock restart to defaults"))
				restartingToDefault = 5f;
			if (restartingToDefault > 0)
			{
				if (GUILayout.Button($"Restart to default {restartingToDefault:0.0}s"))
				{
					RestartToDefault();
				}
			}
			GUILayout.EndHorizontal();
		}

		private void RestartToDefault()
		{
			ExposedSettings.Instance.Entry_AnistropicFiltering.Value = (AnisotropicFiltering)ExposedSettings.Instance.Entry_AnistropicFiltering.DefaultValue;
			QualitySettings.anisotropicFiltering = ExposedSettings.Instance.Entry_AnistropicFiltering.Value;

			ExposedSettings.Instance.Entry_AnistropicFilteringValue.Value = (int)ExposedSettings.Instance.Entry_AnistropicFilteringValue.DefaultValue;
			Texture.SetGlobalAnisotropicFilteringLimits(8, 16);

			ExposedSettings.Instance.Entry_Antialiasing.Value = (PostProcessLayer.Antialiasing)ExposedSettings.Instance.Entry_Antialiasing.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_CameraFarPlaneDistance.Value = (float)ExposedSettings.Instance.Entry_Quality_CameraFarPlaneDistance.DefaultValue;

			ExposedSettings.Instance.Entry_Quality_LODBias.Value = (float)ExposedSettings.Instance.Entry_Quality_LODBias.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_PixelLightCount.Value = (int)ExposedSettings.Instance.Entry_Quality_PixelLightCount.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_ShadowDistance.Value = (float)ExposedSettings.Instance.Entry_Quality_ShadowDistance.DefaultValue;

			ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue1.Value = (float)ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue1.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue2.Value = (float)ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue2.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue3.Value = (float)ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue3.DefaultValue;
			QualitySettings.shadowCascade4Split = new Vector3(ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue1.Value, ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue2.Value, ExposedSettings.Instance.Entry_Quality_ShadowFourSplitValue3.Value);

			ExposedSettings.Instance.Entry_Quality_ShadowsQuality.Value = (ShadowQuality)ExposedSettings.Instance.Entry_Quality_ShadowsQuality.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_ShadowsResolution.Value = (ShadowResolution)ExposedSettings.Instance.Entry_Quality_ShadowsResolution.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_ShadowTwoSplitValue.Value = (float)ExposedSettings.Instance.Entry_Quality_ShadowTwoSplitValue.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_MirrorReflectionResolution.Value = (int)ExposedSettings.Instance.Entry_Quality_MirrorReflectionResolution.DefaultValue;

			ExposedSettings.Instance.Entry_Quality_TextureQuality.Value = (int)ExposedSettings.Instance.Entry_Quality_TextureQuality.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_Use4ShadowCascades.Value = (bool)ExposedSettings.Instance.Entry_Quality_Use4ShadowCascades.DefaultValue;

			ExposedSettings.Instance.Entry_Quality_SSR_Enable.Value = (bool)ExposedSettings.Instance.Entry_Quality_SSR_Enable.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_SSR_DistanceFade.Value = (float)ExposedSettings.Instance.Entry_Quality_SSR_DistanceFade.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_SSR_MaxMarchingDistance.Value = (float)ExposedSettings.Instance.Entry_Quality_SSR_MaxMarchingDistance.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_SSR_Preset.Value = (ScreenSpaceReflectionPreset)ExposedSettings.Instance.Entry_Quality_SSR_Preset.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_SSR_Resolution.Value = (ScreenSpaceReflectionResolution)ExposedSettings.Instance.Entry_Quality_SSR_Resolution.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_SSR_Tickness.Value = (float)ExposedSettings.Instance.Entry_Quality_SSR_Tickness.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_SSR_Vignette.Value = (float)ExposedSettings.Instance.Entry_Quality_SSR_Vignette.DefaultValue;

			ExposedSettings.Instance.Entry_Quality_HBAO_Preset.Value = (HBAO_Core.Preset)ExposedSettings.Instance.Entry_Quality_HBAO_Preset.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_HBAO_Intensity.Value = (float)ExposedSettings.Instance.Entry_Quality_HBAO_Intensity.DefaultValue;

			ExposedSettings.Instance.Entry_Quality_EdgeDetection.Value = (bool)ExposedSettings.Instance.Entry_Quality_EdgeDetection.DefaultValue;
			ExposedSettings.Instance.Entry_Quality_EdgeDetectionDepth.Value = (float)ExposedSettings.Instance.Entry_Quality_EdgeDetectionDepth.DefaultValue;
		}

		private void DrawSteamInput()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;

			{
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Label("Options starting with * requires full game restart");
				GUILayout.BeginHorizontal();
				GUILayout.Label("* Input mode:");
				if (GUILayout.Button("Change to keyboard and mouse"))
					ExposedSettings.Instance.Input_Override.Value = ExposedSettings.InputType.KeyboardAndMouse;
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
		}

		private void DrawKeyboardAndMouseInput()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;

			var Settings = ExposedSettings.Instance;
			{
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.BeginHorizontal();
				GUILayout.Label("* Input mode:");
				if (GUILayout.Button("Change to gamepad"))
					Settings.Input_Override.Value = ExposedSettings.InputType.SteamInput;

				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
			{
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Label("Options starting with * requires full game restart");
				GUILayout.Label("For keyboard and mouse to work a controller is still required!");
				GUILayout.Label("A fake / emulated controller can work, provided it is detected by SteamInput.");
				GUILayout.Label("For mouse support to work, a modified gamemangers file is required.");
				GUILayout.EndVertical();
			}

			{
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.BeginHorizontal();
				GUILayout.Label($"Mouse sensitivity: {Settings.Input_Mouse_Sensitivity.Value:0.00}");
				Settings.Input_Mouse_Sensitivity.Value = GUILayout.HorizontalSlider(Settings.Input_Mouse_Sensitivity.Value, 0.05f, 2);
				Settings.Input_MouseYAxisInversion.Value = GUILayout.Toggle(Settings.Input_MouseYAxisInversion.Value, "Mouse Y Axis inversion");
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

			{
				GUILayout.BeginVertical(GUI.skin.box);
				if (GlobalInputHookHandler.Instance != null)
				{
					GUILayout.Label("<b>Key rebinding (affects only gameplay section)</b>", richText);

					GUILayout.Label("");
					GUILayout.Label("<b>Movement</b>", richText);
					DrawRebind("Move forward", RebindingActions.Forward);
					DrawRebind("Move backward", RebindingActions.Backward);
					DrawRebind("Move left", RebindingActions.Left);
					DrawRebind("Move right", RebindingActions.Right);
					DrawRebind("Crouch", RebindingActions.Crouch);
					DrawRebind("Sprint / dodge", RebindingActions.Dash_Dodge);

					GUILayout.Label("");
					GUILayout.Label("<b>Actions</b>", richText);
					DrawRebind("Fire / Punch", RebindingActions.Fire_Weapon_Punch);
					DrawRebind("Aim a gun", RebindingActions.Point_Gun);
					DrawRebind("Vision", RebindingActions.Vision);
					DrawRebind("Interact / Reload / Accelerate", RebindingActions.Interact_Reload_Accellerate);
					DrawRebind("Cancel / Break", RebindingActions.Cancel_Brake);
					DrawRebind("Reset camera / fighting style", RebindingActions.Reset_Camera_Fighting_Style);

					GUILayout.Label("");
					GUILayout.Label("<b>Actions</b>", richText);
					DrawRebind("Display map", RebindingActions.Display_Map);
					DrawRebind("Quest display", RebindingActions.Quest_Display);
					DrawRebind("Skateboard", RebindingActions.Skateboard);

					DrawRebind("Switch slot to left", RebindingActions.SwitchSlotLeft);
					DrawRebind("Switch slot to right", RebindingActions.SwitchSlotRight);
					DrawRebind("Switch album display up", RebindingActions.SwitchAlbumDisplayUp);
					DrawRebind("Switch album display down", RebindingActions.SwitchAlbumDisplayDown);

					if (CurrentRebinding != RebindingActions.None)
					{
						Event e = Event.current;
						if (e.type != EventType.Used)
						{
							var tempKey = KeyCode.None;
							if (e.isKey)
								tempKey = e.keyCode;
							else if (e.isMouse)
							{
								if (Input.GetKeyDown(KeyCode.Mouse0))
									tempKey = KeyCode.Mouse0;
								else if (Input.GetKeyDown(KeyCode.Mouse1))
									tempKey = KeyCode.Mouse1;
								else if (Input.GetKeyDown(KeyCode.Mouse2))
									tempKey = KeyCode.Mouse2;
								else if (Input.GetKeyDown(KeyCode.Mouse3))
									tempKey = KeyCode.Mouse3;
								else if (Input.GetKeyDown(KeyCode.Mouse4))
									tempKey = KeyCode.Mouse4;
							}

							if (tempKey != KeyCode.None)
							{
								RebindingAction(CurrentRebinding, tempKey);
								CurrentRebinding = RebindingActions.None;
							}
						}
					}
				}
				else
				{
					GUILayout.Label("<b>Keyboard / mouse not initialized - please restart the game!</b>", richText);
				}

				GUILayout.EndVertical();
			}
		}


		private void RebindingAction(RebindingActions currentRebinding, KeyCode tempKey)
		{
			var Settings = ExposedSettings.Instance;

			switch (currentRebinding)
			{
				case RebindingActions.Forward:
					Settings.Input_Analog_LeftStick_Up.Value = tempKey;
					break;
				case RebindingActions.Backward:
					Settings.Input_Analog_LeftStick_Down.Value = tempKey;
					break;
				case RebindingActions.Left:
					Settings.Input_Analog_LeftStick_Left.Value = tempKey;
					break;
				case RebindingActions.Right:
					Settings.Input_Analog_LeftStick_Right.Value = tempKey;
					break;
				case RebindingActions.Display_Map:
					Settings.Input_Digital_Back_Button.Value = tempKey;
					break;
				case RebindingActions.Quest_Display:
					Settings.Input_Digital_Start_Button.Value = tempKey;
					break;
				case RebindingActions.Point_Gun:
					Settings.Input_Digital_LT.Value = tempKey;
					break;
				case RebindingActions.Vision:
					Settings.Input_Digital_LB.Value = tempKey;
					break;
				case RebindingActions.Crouch:
					Settings.Input_Digital_L_Stick_Button.Value = tempKey;
					break;
				case RebindingActions.Fire_Weapon_Punch:
					Settings.Input_Digital_RT.Value = tempKey;
					break;
				case RebindingActions.Cancel_Brake:
					Settings.Input_Digital_B_Button.Value = tempKey;
					break;
				case RebindingActions.Dash_Dodge:
					Settings.Input_Digital_RB.Value = tempKey;
					break;
				case RebindingActions.Interact_Reload_Accellerate:
					Settings.Input_Digital_A_Button.Value = tempKey;
					break;
				case RebindingActions.Reset_Camera_Fighting_Style:
					Settings.Input_Digital_R_Stick_Button.Value = tempKey;
					break;
				case RebindingActions.Skateboard:
					Settings.Input_Digital_Y_Button.Value = tempKey;
					break;
				case RebindingActions.SwitchAlbumDisplayDown:
					Settings.Input_Digital_Down_Button.Value = tempKey;
					break;
				case RebindingActions.SwitchAlbumDisplayUp:
					Settings.Input_Digital_Up_Button.Value = tempKey;
					break;
				case RebindingActions.SwitchSlotLeft:
					Settings.Input_Digital_Left_Button.Value = tempKey;
					break;
				case RebindingActions.SwitchSlotRight:
					Settings.Input_Digital_Right_Button.Value = tempKey;
					break;
			}
		}

		public void DrawRebind(string text, RebindingActions rebindingKey)
		{
			var fixedwidth = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<GUILayoutOption>(1);
			fixedwidth[0] = GUILayout.Width(300);

			GUILayout.BeginHorizontal();
			GUILayout.Label(text, fixedwidth);
			GUILayout.Label(GlobalInputHookHandler.GetInputForRebinding(rebindingKey).ToString(), fixedwidth);
			if (rebindingKey == CurrentRebinding)
			{
				GUIStyle richText = GUI.skin.label;
				richText.richText = true;
				GUILayout.Label("<color=red>Awaiting key</color>", richText, fixedwidth);
			}
			else if (rebindingKey != RebindingActions.None)
			{
				if (GUILayout.Button("Rebind", fixedwidth))
				{
					CurrentRebinding = rebindingKey;
					Event.current.type = EventType.Used;
				}
			}

			GUILayout.EndHorizontal();
		}

		private void DrawOther()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label("<b>Other settings:</b>", richText);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label("Settings with * at the beginning require game restart!");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			ExposedSettings.Instance.Entry_Other_SkipIntros.Value = GUILayout.Toggle(ExposedSettings.Instance.Entry_Other_SkipIntros.Value, "* Skip intros");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			ExposedSettings.Instance.Entry_Other_InterpolateMovement.Value = GUILayout.Toggle(ExposedSettings.Instance.Entry_Other_InterpolateMovement.Value, "Interpolate movement");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label($"* Geometry improvements ({ExposedSettings.Instance.Entry_Other_GeometryImprovements.Value}):");
			if (GUILayout.Button("Disabled"))
				ExposedSettings.Instance.Entry_Other_GeometryImprovements.Value = ExposedSettings.GeometryImprovements.Disabled;
			if (GUILayout.Button("Enabled"))
				ExposedSettings.Instance.Entry_Other_GeometryImprovements.Value = ExposedSettings.GeometryImprovements.Enabled;
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label($"* Light improvements ({ExposedSettings.Instance.Entry_Other_LightImprovements.Value}):");
			if (GUILayout.Button("Disabled"))
				ExposedSettings.Instance.Entry_Other_LightImprovements.Value = ExposedSettings.LightImprovements.Disabled;
			if (GUILayout.Button("Minor (performance safe)"))
				ExposedSettings.Instance.Entry_Other_LightImprovements.Value = ExposedSettings.LightImprovements.Minor;
			if (GUILayout.Button("All"))
				ExposedSettings.Instance.Entry_Other_LightImprovements.Value = ExposedSettings.LightImprovements.All;
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.BeginVertical();
			var promptsUsed = ExposedSettings.Instance.Entry_Other_Prompts.Value == "" ? "None" : ExposedSettings.Instance.Entry_Other_Prompts.Value;

			GUILayout.Label($"* Prompts used: \"{promptsUsed}\" - possible:");
			if (GUILayout.Button("None"))
				ExposedSettings.Instance.Entry_Other_Prompts.Value = "";

			var files = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Prompts"), "*.manifest");
			foreach (var file in files)
			{
				var split = file.Split('\\', '/', '.');
				var fileName = split[^2];
				if (fileName == "keyboard")
					continue;

				if (GUILayout.Button(fileName))
				{
					ExposedSettings.Instance.Entry_Other_Prompts.Value = fileName;
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		private static string GetTextureString(int masterTextureLimit)
		{
			//I wish I could use 8.0 recursive patern
			return masterTextureLimit switch
			{
				1 => "Half",
				_ => "Full",
			};
		}

		private void OnDestroy() => Plugin.Warning("GUI was destroyed - was this intended or Unity cleaned it up?");
	}
}
