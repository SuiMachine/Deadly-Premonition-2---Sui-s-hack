using Il2CppInterop.Runtime.Injection;
using System.IO;
using UnityEngine;
using static SuisHack.KeyboardSupport.SteamInputHook;

namespace SuisHack
{
	public class SettingsGUI : MonoBehaviour
	{
		public static SettingsGUI Instance { get; private set; }
		public static bool Display { get; private set; }
		private Category category = Category.Root;
		private ExposedSettings Settings;
		string resolutionX;
		string resolutionY;
		string refreshRate;
		float restartingToDefault = 0;
		//private RebindingActions CurrentRebinding;

		enum Category
		{
			Root,
			Display,
			Quality,
			Input,
			Other
		}

		public static void Initialize()
		{
			if (Instance == null)
			{
				ClassInjector.RegisterTypeInIl2Cpp<SettingsGUI>();
				var gameObject = new GameObject("SuisHackSettingsGUI");
				Instance = gameObject.AddComponent<SettingsGUI>();
				DontDestroyOnLoad(gameObject);
			}
		}

		void Start()
		{
			Settings = Plugin.Settings;
			resolutionX = Screen.width.ToString();
			resolutionY = Screen.height.ToString();
			refreshRate = Settings.Entry_Display_RefreshRate.Value.ToString();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F11))
			{
				//CurrentRebinding = RebindingActions.None;
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
/*			if (Display)
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
						switch (Settings.Input_Override.Value)
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
			}*/
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
					Settings.Input_Override.Value = ExposedSettings.InputType.KeyboardAndMouse;
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

			{
				GUILayout.BeginVertical(GUI.skin.box);
				Settings.Input_Controller_Vibration.Value = GUILayout.Toggle(Settings.Input_Controller_Vibration.Value, "Controller rumble / vibration");
				GUILayout.EndVertical();
			}

		}

		private void DrawKeyboardAndMouseInput()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;

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
/*				if (GlobalInputHookHandler.Instance != null)
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
				}*/

				GUILayout.EndVertical();
			}
		}

		public void DrawRebind(string text, RebindingActions rebindingKey)
		{
			//var fixedwidth = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<GUILayoutOption>(1);
			//fixedwidth[0] = GUILayout.Width(300);

			//GUILayout.BeginHorizontal();
			//GUILayout.Label(text, fixedwidth);
			//GUILayout.Label(GlobalInputHookHandler.GetInputForRebinding(rebindingKey).ToString(), fixedwidth);
			//if (rebindingKey == CurrentRebinding)
			//{
			//	GUIStyle richText = GUI.skin.label;
			//	richText.richText = true;
			//	GUILayout.Label("<color=red>Awaiting key</color>", richText, fixedwidth);
			//}
			//else if (rebindingKey != RebindingActions.None)
			//{
			//	if (GUILayout.Button("Rebind", fixedwidth))
			//	{
			//		CurrentRebinding = rebindingKey;
			//		Event.current.type = EventType.Used;
			//	}
			//}

			//GUILayout.EndHorizontal();
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
			Settings.Entry_Other_ShowAdvanced.Value = GUILayout.Toggle(Settings.Entry_Other_ShowAdvanced.Value, "Show advanced and experimental options");
		}

/*		private void DrawDisplay()
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

					Screen.SetResolution(newResX, newResY, Settings.Entry_Display_DisplayMode.Value, newRefreshRate);
					Settings.Entry_Display_Resolution.Value = $"{newResX}x{newResY}";
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
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.ExclusiveFullScreen;
					Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Settings.Entry_Display_DisplayMode.Value, Screen.currentResolution.refreshRate);
					if (Settings.Entry_Display_Vsync.Value)
						Settings.Entry_Display_RefreshRate.Value = Screen.currentResolution.refreshRate;
				}
				if (GUILayout.Button("Maximized window"))
				{
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.MaximizedWindow;
					Screen.SetResolution(Screen.width, Screen.height, Settings.Entry_Display_DisplayMode.Value, 0);
				}
				if (GUILayout.Button("Borderless window"))
				{
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.FullScreenWindow;
					Screen.SetResolution(Screen.width, Screen.height, Settings.Entry_Display_DisplayMode.Value, 0);
				}
				if (GUILayout.Button("Windowed"))
				{
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.Windowed;
					Screen.SetResolution(Screen.width, Screen.height, Settings.Entry_Display_DisplayMode.Value, 0);

				}
				GUILayout.EndHorizontal();
			}

			//Vsync
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				var oldValue = QualitySettings.vSyncCount > 0 ? true : false;
				var newValue = GUILayout.Toggle(oldValue, "V-sync");
				if (newValue != oldValue)
				{
					QualitySettings.vSyncCount = newValue ? 1 : 0;
					Settings.Entry_Display_Vsync.Value = newValue;
					if (!newValue)
					{
						Application.targetFrameRate = Settings.Entry_DesiredFramerate.Value;
					}
				}
				GUILayout.EndHorizontal();
			}

			//FPS cap
			{
				if (QualitySettings.vSyncCount == 0)
				{
					GUILayout.BeginHorizontal(GUI.skin.box);
					GUILayout.Label($"FPS cap ({Settings.Entry_DesiredFramerate.Value})");
					Settings.Entry_DesiredFramerate.Value = (int)GUILayout.HorizontalSlider(Settings.Entry_DesiredFramerate.Value, -1, 1000);
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
				//GUILayout.BeginHorizontal(GUI.skin.box);
				//GUILayout.Label($"Antialiasing filter: ({Hacks.PostProcessLayerHook.GetShortName()}):");
				//if (GUILayout.Button("None"))
				//	Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.None;
				//if (GUILayout.Button("FXAA"))
				//	Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;

				//if (GUILayout.Button("SMAA"))
				//	Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
				//if (Settings.Entry_Other_ShowAdvanced.Value)
				//{
				//	if (GUILayout.Button("TAA (Epilepsy warning!)"))
				//		Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.TemporalAntialiasing;
				//}


				//GUILayout.EndHorizontal();
			}

			//Anisotropic filtering Bias
			{
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.BeginHorizontal();
				GUILayout.Label($"Anisotropic filtering mode: ({QualitySettings.anisotropicFiltering}):");

				if (GUILayout.Button("Force disabled"))
					Settings.Entry_AnistropicFiltering.Value = AnisotropicFiltering.Disable;

				if (GUILayout.Button("Per texture"))
					Settings.Entry_AnistropicFiltering.Value = AnisotropicFiltering.Enable;

				if (GUILayout.Button("Force enabled"))
					Settings.Entry_AnistropicFiltering.Value = AnisotropicFiltering.ForceEnable;

				GUILayout.EndHorizontal();

				if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
				{
					var oldValue = Settings.Entry_AnistropicFilteringValue.Value;
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Anisotropic filtering level ({oldValue}): ");
					Settings.Entry_AnistropicFilteringValue.Value = (int)GUILayout.HorizontalSlider(Settings.Entry_AnistropicFilteringValue.Value, -1, 16);
					GUILayout.EndHorizontal();
				}

				GUILayout.EndVertical();
			}

			//Shadows
			{
				GUILayout.BeginVertical(GUI.skin.box);

				//Shadow mode
				if (Settings.Entry_Other_ShowAdvanced.Value)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Shadows ({QualitySettings.shadows}):");
					if (GUILayout.Button("None"))
						Settings.Entry_Quality_ShadowsQuality.Value = ShadowQuality.Disable;

					if (GUILayout.Button("Hard only"))
						Settings.Entry_Quality_ShadowsQuality.Value = ShadowQuality.HardOnly;

					if (GUILayout.Button("Soft"))
						Settings.Entry_Quality_ShadowsQuality.Value = ShadowQuality.All;

					GUILayout.EndHorizontal();
				}

				//Shadow resolution
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Shadow resolution ({QualitySettings.shadowResolution}):");
					if (GUILayout.Button("Low"))
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.Low;

					if (GUILayout.Button("Medium"))
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.Medium;

					if (GUILayout.Button("High"))
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.High;

					if (GUILayout.Button("Very High"))
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.VeryHigh;

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
						Settings.Entry_Quality_Use4ShadowCascades.Value = false;
					}
					if (GUILayout.Button("4"))
					{
						Settings.Entry_Quality_Use4ShadowCascades.Value = true;
					}
					GUILayout.EndHorizontal();
				}

				if (Settings.Entry_Other_ShowAdvanced.Value)
				{
					if (QualitySettings.shadowCascades == 2)
					{
						GUILayout.BeginHorizontal();
						GUILayout.Label($"Shadow cascades  2 split: ({QualitySettings.shadowCascade2Split:0.00}):");
						float value = QualitySettings.shadowCascade2Split;
						QualitySettings.shadowCascade2Split = GUILayout.HorizontalSlider(value, 0.01f, 0.5f);
						if (QualitySettings.shadowCascade2Split != value)
							Settings.Entry_Quality_ShadowTwoSplitValue.Value = QualitySettings.shadowCascade2Split;
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
							Settings.Entry_Quality_ShadowFourSplitValue1.Value = cascade1;
							Settings.Entry_Quality_ShadowFourSplitValue2.Value = cascade2;
							Settings.Entry_Quality_ShadowFourSplitValue3.Value = cascade3;
						}
					}
				}
				GUILayout.EndVertical();
			}

			{
				//GUILayout.BeginHorizontal(GUI.skin.box);
				//var oldValue = Hacks.PostProcessLayerHook.FarClipDistance;
				//GUILayout.Label($"Far clip distance ({Hacks.PostProcessLayerHook.FarClipDistance}):");
				//var newValue = Mathf.Round(GUILayout.HorizontalSlider(oldValue, 400, 2000f));
				//if (newValue != oldValue)
				//	Settings.Entry_Quality_CameraFarPlaneDistance.Value = newValue;

				//GUILayout.EndHorizontal();
			}

			//LOD Bias
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				var oldValue = QualitySettings.lodBias;
				GUILayout.Label($"LOD Bias ({QualitySettings.lodBias:0.0}):");
				QualitySettings.lodBias = GUILayout.HorizontalSlider(QualitySettings.lodBias, 0.5f, 4f);
				if (oldValue != QualitySettings.lodBias)
					Settings.Entry_Quality_LODBias.Value = QualitySettings.lodBias;

				GUILayout.EndHorizontal();
			}

			//Pixel Light Count
			if (Settings.Entry_Other_ShowAdvanced.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				var oldValue = QualitySettings.pixelLightCount;
				GUILayout.Label($"Pixel light count: ({QualitySettings.pixelLightCount}):");
				QualitySettings.pixelLightCount = (int)GUILayout.HorizontalSlider(QualitySettings.pixelLightCount, 0, 8);
				if (oldValue != QualitySettings.pixelLightCount)
					Settings.Entry_Quality_PixelLightCount.Value = QualitySettings.pixelLightCount;

				GUILayout.EndHorizontal();
			}

			//Texture Quality
			{
				GUILayout.BeginHorizontal(GUI.skin.box);
				GUILayout.Label($"Texture resolution ({GetTextureString(QualitySettings.masterTextureLimit)}):");

				if (GUILayout.Button("Full"))
					Settings.Entry_Quality_TextureQuality.Value = 0;
				if (GUILayout.Button("Half"))
					Settings.Entry_Quality_TextureQuality.Value = 1;

				GUILayout.EndHorizontal();
			}

			//Planar reflections
			{
				//GUILayout.BeginHorizontal(GUI.skin.box);
				//GUILayout.Label($"Planar reflections resolution ({Hacks.MirrorReflectionHook.ReflectionSize}):");
				//var log = Mathf.RoundToInt(Mathf.Log(Hacks.MirrorReflectionHook.ReflectionSize, 2));
				//var newLog = (int)GUILayout.HorizontalSlider(log, 7, 11);
				//if (newLog != log)
				//	Settings.Entry_Quality_MirrorReflectionResolution.Value = (int)Mathf.Pow(2, newLog);

				//GUILayout.EndHorizontal();
			}

			//HBAO
			{
				GUILayout.BeginVertical(GUI.skin.box);
				//GUILayout.BeginHorizontal();
				//GUILayout.Label($"HBAO preset ({Hacks.PostProcessLayerHook.HBAO_Preset}):");
				//if (GUILayout.Button("Fastest"))
				//	Settings.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.FastestPerformance;
				//if (GUILayout.Button("Fast"))
				//	Settings.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.FastPerformance;
				//if (GUILayout.Button("Normal"))
				//	Settings.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.Normal;
				//if (GUILayout.Button("High"))
				//	Settings.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.HighQuality;
				//if (GUILayout.Button("Highest"))
				//	Settings.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.HighestQuality;
				//GUILayout.EndHorizontal();

				//GUILayout.BeginHorizontal();
				//GUILayout.Label($"HBAO intensity ({Hacks.PostProcessLayerHook.HBAO_Intensity:0.0}):");
				//Settings.Entry_Quality_HBAO_Intensity.Value = GUILayout.HorizontalSlider(Settings.Entry_Quality_HBAO_Intensity.Value, 0.0f, 1.0f);
				//GUILayout.EndHorizontal();
				//GUILayout.EndVertical();
			}

			//Meh
			{
				GUILayout.BeginVertical(GUI.skin.box);
				Settings.Entry_Quality_EdgeDetection.Value = GUILayout.Toggle(Settings.Entry_Quality_EdgeDetection.Value, "Edge detection post process filter");

				if (Settings.Entry_Quality_EdgeDetection.Value && Settings.Entry_Quality_CameraFarPlaneDistance.Value > 800)
				{
					GUILayout.Label("Warning - extending far clip camera plane distance can cause issues with edge detection filter.\nIf you experience issues with lines on characters being displayed up close, try lowering the value below:");
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Edge detection depth {Settings.Entry_Quality_EdgeDetectionDepth.Value:0.00}");
					Settings.Entry_Quality_EdgeDetectionDepth.Value = GUILayout.HorizontalSlider(Settings.Entry_Quality_EdgeDetectionDepth.Value, 0, 1);
					GUILayout.EndHorizontal();
				}
				else
					Settings.Entry_Quality_EdgeDetectionDepth.Value = 1;

				GUILayout.EndVertical();
			}

			//SSR
			//{
			//	GUILayout.BeginVertical(GUI.skin.box);
			//	Settings.Entry_Quality_SSR_Enable.Value = GUILayout.Toggle(Settings.Entry_Quality_SSR_Enable.Value, "Enable SSR override");

			//	if (Settings.Entry_Quality_SSR_Enable.Value)
			//	{
			//		GUILayout.BeginHorizontal();
			//		GUILayout.Label($"SSR preset ({Hacks.PostProcessLayerHook.SSR_Preset}):");
			//		if (GUILayout.Button("Lower"))
			//			Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Lower;
			//		if (GUILayout.Button("Low"))
			//			Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Low;
			//		if (GUILayout.Button("Medium"))
			//			Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Medium;
			//		if (GUILayout.Button("High"))
			//			Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.High;
			//		if (GUILayout.Button("Higher"))
			//			Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Higher;
			//		if (GUILayout.Button("Ultra"))
			//			Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Ultra;
			//		if (GUILayout.Button("Overkill"))
			//			Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Overkill;
			//		GUILayout.EndHorizontal();

			//		GUILayout.BeginHorizontal();
			//		GUILayout.Label($"SSR resolution ({Hacks.PostProcessLayerHook.SSR_Resolution}):");
			//		if (GUILayout.Button("Downsampled"))
			//			Settings.Entry_Quality_SSR_Resolution.Value = ScreenSpaceReflectionResolution.Downsampled;
			//		if (GUILayout.Button("FullSize"))
			//			Settings.Entry_Quality_SSR_Resolution.Value = ScreenSpaceReflectionResolution.FullSize;
			//		if (GUILayout.Button("Supersampled"))
			//			Settings.Entry_Quality_SSR_Resolution.Value = ScreenSpaceReflectionResolution.Supersampled;
			//		GUILayout.EndHorizontal();

			//		if (Settings.Entry_Other_ShowAdvanced.Value)
			//		{
			//			GUILayout.BeginHorizontal();
			//			GUILayout.Label($"SSR Tickness ({Hacks.PostProcessLayerHook.SSR_Tickness:0.0}):");
			//			Settings.Entry_Quality_SSR_Tickness.Value = GUILayout.HorizontalSlider(Settings.Entry_Quality_SSR_Tickness.Value, 0, 1);
			//			GUILayout.EndHorizontal();

			//			GUILayout.BeginHorizontal();
			//			GUILayout.Label($"SSR Vignette ({Hacks.PostProcessLayerHook.SSR_Vignette:0.0}):");
			//			Settings.Entry_Quality_SSR_Vignette.Value = GUILayout.HorizontalSlider(Settings.Entry_Quality_SSR_Vignette.Value, 0, 1);
			//			GUILayout.EndHorizontal();

			//			GUILayout.BeginHorizontal();
			//			GUILayout.Label($"SSR Distance Fade ({Hacks.PostProcessLayerHook.SSR_DistanceFade:0.00}):");
			//			Settings.Entry_Quality_SSR_DistanceFade.Value = GUILayout.HorizontalSlider(Settings.Entry_Quality_SSR_DistanceFade.Value, 0, 0.5f);
			//			GUILayout.EndHorizontal();

			//			GUILayout.BeginHorizontal();
			//			GUILayout.Label($"SSR Max Marching Distance ({Hacks.PostProcessLayerHook.SSR_MaxMarchingDistance:0}):");
			//			Settings.Entry_Quality_SSR_MaxMarchingDistance.Value = GUILayout.HorizontalSlider(Settings.Entry_Quality_SSR_MaxMarchingDistance.Value, 50, 250);
			//			GUILayout.EndHorizontal();
			//		}
			//	}
			//	GUILayout.EndVertical();
			//}

			//Color space
			if (Settings.Entry_Other_ShowAdvanced.Value)
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
		}*/

		private void RestartToDefault()
		{
			Settings.Entry_AnistropicFiltering.Value = (AnisotropicFiltering)Settings.Entry_AnistropicFiltering.DefaultValue;
			QualitySettings.anisotropicFiltering = Settings.Entry_AnistropicFiltering.Value;

			Settings.Entry_AnistropicFilteringValue.Value = (int)Settings.Entry_AnistropicFilteringValue.DefaultValue;
			Texture.SetGlobalAnisotropicFilteringLimits(8, 16);

			Settings.Entry_Antialiasing.Value = (UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing)Settings.Entry_Antialiasing.DefaultValue;
			Settings.Entry_Quality_CameraFarPlaneDistance.Value = (float)Settings.Entry_Quality_CameraFarPlaneDistance.DefaultValue;

			Settings.Entry_Quality_LODBias.Value = (float)Settings.Entry_Quality_LODBias.DefaultValue;
			Settings.Entry_Quality_PixelLightCount.Value = (int)Settings.Entry_Quality_PixelLightCount.DefaultValue;
			Settings.Entry_Quality_ShadowDistance.Value = (float)Settings.Entry_Quality_ShadowDistance.DefaultValue;

			Settings.Entry_Quality_ShadowFourSplitValue1.Value = (float)Settings.Entry_Quality_ShadowFourSplitValue1.DefaultValue;
			Settings.Entry_Quality_ShadowFourSplitValue2.Value = (float)Settings.Entry_Quality_ShadowFourSplitValue2.DefaultValue;
			Settings.Entry_Quality_ShadowFourSplitValue3.Value = (float)Settings.Entry_Quality_ShadowFourSplitValue3.DefaultValue;
			QualitySettings.shadowCascade4Split = new Vector3(Settings.Entry_Quality_ShadowFourSplitValue1.Value, Settings.Entry_Quality_ShadowFourSplitValue2.Value, Settings.Entry_Quality_ShadowFourSplitValue3.Value);

			Settings.Entry_Quality_ShadowsQuality.Value = (ShadowQuality)Settings.Entry_Quality_ShadowsQuality.DefaultValue;
			Settings.Entry_Quality_ShadowsResolution.Value = (ShadowResolution)Settings.Entry_Quality_ShadowsResolution.DefaultValue;
			Settings.Entry_Quality_ShadowTwoSplitValue.Value = (float)Settings.Entry_Quality_ShadowTwoSplitValue.DefaultValue;
			Settings.Entry_Quality_MirrorReflectionResolution.Value = (int)Settings.Entry_Quality_MirrorReflectionResolution.DefaultValue;

			Settings.Entry_Quality_TextureQuality.Value = (int)Settings.Entry_Quality_TextureQuality.DefaultValue;
			Settings.Entry_Quality_Use4ShadowCascades.Value = (bool)Settings.Entry_Quality_Use4ShadowCascades.DefaultValue;

			Settings.Entry_Quality_SSR_Enable.Value = (bool)Settings.Entry_Quality_SSR_Enable.DefaultValue;
			Settings.Entry_Quality_SSR_DistanceFade.Value = (float)Settings.Entry_Quality_SSR_DistanceFade.DefaultValue;
			Settings.Entry_Quality_SSR_MaxMarchingDistance.Value = (float)Settings.Entry_Quality_SSR_MaxMarchingDistance.DefaultValue;
			Settings.Entry_Quality_SSR_Preset.Value = (UnityEngine.Rendering.PostProcessing.ScreenSpaceReflectionPreset)Settings.Entry_Quality_SSR_Preset.DefaultValue;
			Settings.Entry_Quality_SSR_Resolution.Value = (UnityEngine.Rendering.PostProcessing.ScreenSpaceReflectionResolution)Settings.Entry_Quality_SSR_Resolution.DefaultValue;
			Settings.Entry_Quality_SSR_Tickness.Value = (float)Settings.Entry_Quality_SSR_Tickness.DefaultValue;
			Settings.Entry_Quality_SSR_Vignette.Value = (float)Settings.Entry_Quality_SSR_Vignette.DefaultValue;

			Settings.Entry_Quality_HBAO_Preset.Value = (HBAO_Core.Preset)Settings.Entry_Quality_HBAO_Preset.DefaultValue;
			Settings.Entry_Quality_HBAO_Intensity.Value = (float)Settings.Entry_Quality_HBAO_Intensity.DefaultValue;

			Settings.Entry_Quality_EdgeDetection.Value = (bool)Settings.Entry_Quality_EdgeDetection.DefaultValue;
			Settings.Entry_Quality_EdgeDetectionDepth.Value = (float)Settings.Entry_Quality_EdgeDetectionDepth.DefaultValue;
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
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label("<b>Other settings:</b>", richText);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label("Settings with * at the beginning require game restart!");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			Settings.Entry_Other_SkipIntros.Value = GUILayout.Toggle(Settings.Entry_Other_SkipIntros.Value, "* Skip intros");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			Settings.Entry_Other_InterpolateMovement.Value = GUILayout.Toggle(Settings.Entry_Other_InterpolateMovement.Value, "Interpolate movement");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label($"* Geometry improvements ({Settings.Entry_Other_GeometryImprovements.Value}):");
			if (GUILayout.Button("Disabled"))
				Settings.Entry_Other_GeometryImprovements.Value = ExposedSettings.GeometryImprovements.Disabled;
			if (GUILayout.Button("Enabled"))
				Settings.Entry_Other_GeometryImprovements.Value = ExposedSettings.GeometryImprovements.Enabled;
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label($"* Light improvements ({Settings.Entry_Other_LightImprovements.Value}):");
			if (GUILayout.Button("Disabled"))
				Settings.Entry_Other_LightImprovements.Value = ExposedSettings.LightImprovements.Disabled;
			if (GUILayout.Button("Minor (performance safe)"))
				Settings.Entry_Other_LightImprovements.Value = ExposedSettings.LightImprovements.Minor;
			if (GUILayout.Button("All"))
				Settings.Entry_Other_LightImprovements.Value = ExposedSettings.LightImprovements.All;
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.BeginVertical();
			var promptsUsed = Settings.Entry_Other_Prompts.Value == "" ? "None" : Settings.Entry_Other_Prompts.Value;

			GUILayout.Label($"* Prompts used: \"{promptsUsed}\" - possible:");
			if (GUILayout.Button("None"))
				Settings.Entry_Other_Prompts.Value = "";

			var files = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Prompts"), "*.manifest");
			foreach (var file in files)
			{
				var split = file.Split('\\', '/', '.');
				var fileName = split[split.Length - 2];
				if (fileName == "keyboard")
					continue;

				if (GUILayout.Button(fileName))
				{
					Settings.Entry_Other_Prompts.Value = fileName;
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

	}
}
