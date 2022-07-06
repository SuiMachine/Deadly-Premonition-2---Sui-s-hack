using Il2CppSystem.IO;
using MelonLoader;
using SuisHack.KeyboardSupport;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static SuisHack.KeyboardSupport.SteamInputHook;

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
			Input,
			Other
		}

		public SettingsGUI(IntPtr ptr) : base(ptr) { }
		public static SettingsGUI Instance { get; private set; }
		public static bool Display { get; private set; }
		private Category category = Category.Root;
		private ExposedSettings Settings;
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
				DontDestroyOnLoad(gameObject);
			}
		}

		void Start()
		{
			Settings = Settings = SuisHackMain.Settings;
			resolutionX = Screen.width.ToString();
			resolutionY = Screen.height.ToString();
			refreshRate = Settings.Entry_Display_RefreshRate.Value.ToString();
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
				GUILayout.BeginVertical(null);
				GUILayout.BeginHorizontal(GUI.skin.box, null);
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
			}
		}

		private void DrawSteamInput()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;

			{
				GUILayout.BeginVertical(GUI.skin.box, null);
				GUILayout.Label("Options starting with * requires full game restart", null);
				GUILayout.BeginHorizontal(null);
				GUILayout.Label("* Input mode:", null);
				if (GUILayout.Button("Change to keyboard and mouse", null))
					Settings.Input_Override.Value = ExposedSettings.InputType.KeyboardAndMouse;
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

			{
				GUILayout.BeginVertical(GUI.skin.box, null);
				Settings.Input_Controller_Vibration.Value = GUILayout.Toggle(Settings.Input_Controller_Vibration.Value, "Controller rumble / vibration", null);
				GUILayout.EndVertical();
			}

		}

		private void DrawKeyboardAndMouseInput()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;

			{
				GUILayout.BeginVertical(GUI.skin.box, null);
				GUILayout.BeginHorizontal(null);
				GUILayout.Label("* Input mode:", null);
				if (GUILayout.Button("Change to gamepad", null))
					Settings.Input_Override.Value = ExposedSettings.InputType.SteamInput;

				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
			{ 
				GUILayout.BeginVertical(GUI.skin.box, null);
				GUILayout.Label("Options starting with * requires full game restart", null);
				GUILayout.Label("For keyboard and mouse to work a controller is still required!", null);
				GUILayout.Label("A fake / emulated controller can work, provided it is detected by SteamInput.", null);
				GUILayout.Label("For mouse support to work, a modified gamemangers file is required.", null);
				GUILayout.EndVertical();
			}

			{
				GUILayout.BeginVertical(GUI.skin.box, null);
				GUILayout.BeginHorizontal(null);
				GUILayout.Label($"Mouse sensitivity: {Settings.Input_Mouse_Sensitivity.Value:0.00}", null);
				Settings.Input_Mouse_Sensitivity.Value = GUILayout.HorizontalSlider(Settings.Input_Mouse_Sensitivity.Value, 0.05f, 2, null);
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

			{
				GUILayout.BeginVertical(GUI.skin.box, null);
				if (GlobalInputHookHandler.Instance != null)
				{
					GUILayout.Label("<b>Key rebinding (affects only gameplay section)</b>", richText, null);

					GUILayout.Label("", null);
					GUILayout.Label("<b>Movement</b>", richText, null);
					DrawRebind("Move forward", RebindingActions.Forward);
					DrawRebind("Move backward", RebindingActions.Backward);
					DrawRebind("Move left", RebindingActions.Left);
					DrawRebind("Move right", RebindingActions.Right);
					DrawRebind("Crouch", RebindingActions.Crouch);
					DrawRebind("Sprint / dodge", RebindingActions.Dash_Dodge);

					GUILayout.Label("", null);
					GUILayout.Label("<b>Actions</b>", richText, null);
					DrawRebind("Fire / Punch", RebindingActions.Fire_Weapon_Punch);
					DrawRebind("Aim a gun", RebindingActions.Point_Gun);
					DrawRebind("Vision", RebindingActions.Vision);
					DrawRebind("Interact / Reload / Accelerate", RebindingActions.Interact_Reload_Accellerate);
					DrawRebind("Cancel / Break", RebindingActions.Cancel_Brake);
					DrawRebind("Reset camera / fighting style", RebindingActions.Reset_Camera_Fighting_Style);

					GUILayout.Label("", null);
					GUILayout.Label("<b>Actions</b>", richText, null);
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
						if(e.type != EventType.Used)
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
					GUILayout.Label("<b>Keyboard / mouse not initialized - please restart the game!</b>", richText, null);
				}

				GUILayout.EndVertical();
			}
		}

		private void RebindingAction(RebindingActions currentRebinding, KeyCode tempKey)
		{
			switch(currentRebinding)
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
			var fixedwidth = new UnhollowerBaseLib.Il2CppReferenceArray<GUILayoutOption>(1);
			fixedwidth[0] = GUILayout.Width(300);

			GUILayout.BeginHorizontal(null);
			GUILayout.Label(text, fixedwidth);
			GUILayout.Label(GlobalInputHookHandler.GetInputForRebinding(rebindingKey).ToString(), fixedwidth);
			if(rebindingKey == CurrentRebinding)
			{
				GUIStyle richText = GUI.skin.label;
				richText.richText = true;
				GUILayout.Label("<color=red>Awaiting key</color>", richText, fixedwidth);
			}
			else if(rebindingKey != RebindingActions.None)
			{
				if (GUILayout.Button("Rebind", fixedwidth))
				{
					CurrentRebinding = rebindingKey;
					Event.current.type = EventType.Used;
				}
			}

			GUILayout.EndHorizontal();
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
			if (GUILayout.Button("Input settings", null))
			{
				category = Category.Input;
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
				GUILayout.Label($"@", null);
				refreshRate = GUILayout.TextField(refreshRate, null);
				GUILayout.Label($"Hz", null);

				if (GUILayout.Button("Apply", null))
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
					GUILayout.BeginHorizontal(GUI.skin.box, null);
					GUILayout.Label($"FPS cap ({Settings.Entry_DesiredFramerate.Value})", null);
					Settings.Entry_DesiredFramerate.Value = (int)GUILayout.HorizontalSlider(Settings.Entry_DesiredFramerate.Value, -1, 1000, null);
					GUILayout.EndHorizontal();
				}
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
					Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.None;
				if (GUILayout.Button("FXAA", null))
					Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;

				if (GUILayout.Button("SMAA", null))
					Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;

				if (GUILayout.Button("TAA", null))
					Settings.Entry_Antialiasing.Value = PostProcessLayer.Antialiasing.TemporalAntialiasing;

				GUILayout.EndHorizontal();
			}

			//Anisotropic filtering Bias
			{
				GUILayout.BeginVertical(GUI.skin.box, null);
				GUILayout.BeginHorizontal(null);
				GUILayout.Label($"Anisotropic filtering mode: ({QualitySettings.anisotropicFiltering}):", null);

				if (GUILayout.Button("Force disabled", null))
					Settings.Entry_AnistropicFiltering.Value = AnisotropicFiltering.Disable;

				if (GUILayout.Button("Per texture", null))
					Settings.Entry_AnistropicFiltering.Value = AnisotropicFiltering.Enable;

				if (GUILayout.Button("Force enabled", null))
					Settings.Entry_AnistropicFiltering.Value = AnisotropicFiltering.ForceEnable;

				GUILayout.EndHorizontal();

				if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
				{
					var oldValue = Settings.Entry_AnistropicFilteringValue.Value;
					GUILayout.BeginHorizontal(null);
					GUILayout.Label($"Anisotropic filtering level ({oldValue}): ", null);
					Settings.Entry_AnistropicFilteringValue.Value = (int)GUILayout.HorizontalSlider(Settings.Entry_AnistropicFilteringValue.Value, -1, 16, null);
					GUILayout.EndHorizontal();
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
						Settings.Entry_Quality_ShadowsQuality.Value = ShadowQuality.Disable;

					if (GUILayout.Button("Hard only", null))
						Settings.Entry_Quality_ShadowsQuality.Value = ShadowQuality.HardOnly;

					if (GUILayout.Button("Soft", null))
						Settings.Entry_Quality_ShadowsQuality.Value = ShadowQuality.All;

					GUILayout.EndHorizontal();
				}

				//Shadow resolution
				{
					GUILayout.BeginHorizontal(null);
					GUILayout.Label($"Shadow resolution ({QualitySettings.shadowResolution}):", null);
					if (GUILayout.Button("Low", null))
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.Low;

					if (GUILayout.Button("Medium", null))
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.Medium;

					if (GUILayout.Button("High", null))
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.High;

					if (GUILayout.Button("Very High", null))
						Settings.Entry_Quality_ShadowsResolution.Value = ShadowResolution.VeryHigh;

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
					}
					GUILayout.EndHorizontal();
				}

				{
					GUILayout.BeginHorizontal(null);
					GUILayout.Label($"Shadow cascades: ({QualitySettings.shadowCascades}):", null);
					if (GUILayout.Button("2", null))
					{
						Settings.Entry_Quality_Use4ShadowCascades.Value = false;
					}
					if (GUILayout.Button("4", null))
					{
						Settings.Entry_Quality_Use4ShadowCascades.Value = true;
					}
					GUILayout.EndHorizontal();
				}

				if (Settings.Entry_Other_ShowAdvanced.Value)
				{
					if (QualitySettings.shadowCascades == 2)
					{
						GUILayout.BeginHorizontal(null);
						GUILayout.Label($"Shadow cascades  2 split: ({QualitySettings.shadowCascade2Split:0.00}):", null);
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
						GUILayout.Label($"Shadow cascades 1 split: ({cascade1:0.000}):", null);
						cascade1 = GUILayout.HorizontalSlider(cascade1, 0.01f, 0.5f, null);
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(null);
						GUILayout.Label($"Shadow cascades 2 split: ({cascade2:0.000}):", null);
						cascade2 = GUILayout.HorizontalSlider(cascade2, 0.1f, 0.8f, null);
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(null);
						GUILayout.Label($"Shadow cascades 3 split: ({cascade3:0.000}):", null);
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

			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				var oldValue = Hacks.PostProcessLayerHook.FarClipDistance;
				GUILayout.Label($"Far clip distance ({Hacks.PostProcessLayerHook.FarClipDistance}):", null);
				var newValue = Mathf.Round(GUILayout.HorizontalSlider(oldValue, 400, 2000f, null));
				if (newValue != oldValue)
					Settings.Entry_Quality_CameraFarPlaneDistance.Value = newValue;

				GUILayout.EndHorizontal();
			}

			//LOD Bias
			if (Settings.Entry_Other_ShowAdvanced.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				var oldValue = QualitySettings.lodBias;
				GUILayout.Label($"LOD Bias ({QualitySettings.lodBias:0.0}):", null);
				QualitySettings.lodBias = GUILayout.HorizontalSlider(QualitySettings.lodBias, 0.5f, 4f, null);
				if (oldValue != QualitySettings.lodBias)
					Settings.Entry_Quality_LODBias.Value = QualitySettings.lodBias;

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
					Settings.Entry_Quality_PixelLightCount.Value = QualitySettings.pixelLightCount;

				GUILayout.EndHorizontal();
			}

			//Texture Quality
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				GUILayout.Label($"Texture resolution ({GetTextureString(QualitySettings.masterTextureLimit)}):", null);

				if (GUILayout.Button("Full", null))
					Settings.Entry_Quality_TextureQuality.Value = 0;
				if (GUILayout.Button("Half", null))
					Settings.Entry_Quality_TextureQuality.Value = 1;

				GUILayout.EndHorizontal();
			}

			//Planar reflections
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				GUILayout.Label($"Planar reflections resolution ({Hacks.MirrorReflectionHook.ReflectionSize}):", null);
				var log = Mathf.RoundToInt(Mathf.Log(Hacks.MirrorReflectionHook.ReflectionSize, 2));
				var newLog = (int)GUILayout.HorizontalSlider(log, 7, 11, null);
				if (newLog != log)
					Settings.Entry_Quality_MirrorReflectionResolution.Value = (int)Mathf.Pow(2, newLog);

				GUILayout.EndHorizontal();
			}

			//HBAO
			{
				GUILayout.BeginVertical(GUI.skin.box, null);
				GUILayout.BeginHorizontal(null);
				GUILayout.Label($"HBAO preset ({Hacks.PostProcessLayerHook.HBAO_Preset}):", null);
				if (GUILayout.Button("Fastest", null))
					Settings.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.FastestPerformance;
				if (GUILayout.Button("Fast", null))
					Settings.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.FastPerformance;
				if (GUILayout.Button("Normal", null))
					Settings.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.Normal;
				if (GUILayout.Button("High", null))
					Settings.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.HighQuality;
				if (GUILayout.Button("Highest", null))
					Settings.Entry_Quality_HBAO_Preset.Value = HBAO_Core.Preset.HighestQuality;
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(null);
				GUILayout.Label($"HBAO intensity ({Hacks.PostProcessLayerHook.HBAO_Intensity:0.0}):", null);
				Settings.Entry_Quality_HBAO_Intensity.Value = GUILayout.HorizontalSlider(Settings.Entry_Quality_HBAO_Intensity.Value, 0.0f, 1.0f, null);
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

			//Meh
			{
				GUILayout.BeginVertical(GUI.skin.box, null);
				Settings.Entry_Quality_EdgeDetection.Value = GUILayout.Toggle(Settings.Entry_Quality_EdgeDetection.Value, "Edge detection post process filter", null);
				GUILayout.EndVertical();
			}

			//SSR
			{
				GUILayout.BeginVertical(GUI.skin.box, null);
				Settings.Entry_Quality_SSR_Enable.Value = GUILayout.Toggle(Settings.Entry_Quality_SSR_Enable.Value, "Enable SSR override", null);

				if (Settings.Entry_Quality_SSR_Enable.Value)
				{
					GUILayout.BeginHorizontal(null);
					GUILayout.Label($"SSR preset ({Hacks.PostProcessLayerHook.SSR_Preset}):", null);
					if (GUILayout.Button("Lower", null))
						Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Lower;
					if (GUILayout.Button("Low", null))
						Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Low;
					if (GUILayout.Button("Medium", null))
						Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Medium;
					if (GUILayout.Button("High", null))
						Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.High;
					if (GUILayout.Button("Higher", null))
						Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Higher;
					if (GUILayout.Button("Ultra", null))
						Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Ultra;
					if (GUILayout.Button("Overkill", null))
						Settings.Entry_Quality_SSR_Preset.Value = ScreenSpaceReflectionPreset.Overkill;
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal(null);
					GUILayout.Label($"SSR resolution ({Hacks.PostProcessLayerHook.SSR_Resolution}):", null);
					if (GUILayout.Button("Downsampled", null))
						Settings.Entry_Quality_SSR_Resolution.Value = ScreenSpaceReflectionResolution.Downsampled;
					if (GUILayout.Button("FullSize", null))
						Settings.Entry_Quality_SSR_Resolution.Value = ScreenSpaceReflectionResolution.FullSize;
					if (GUILayout.Button("Supersampled", null))
						Settings.Entry_Quality_SSR_Resolution.Value = ScreenSpaceReflectionResolution.Supersampled;
					GUILayout.EndHorizontal();

					if(Settings.Entry_Other_ShowAdvanced.Value)
					{
						GUILayout.BeginHorizontal(null);
						GUILayout.Label($"SSR Tickness ({Hacks.PostProcessLayerHook.SSR_Tickness:0.0}):", null);
						Settings.Entry_Quality_SSR_Tickness.Value = GUILayout.HorizontalSlider(Settings.Entry_Quality_SSR_Tickness.Value, 0, 1, null);
						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal(null);
						GUILayout.Label($"SSR Vignette ({Hacks.PostProcessLayerHook.SSR_Vignette:0.0}):", null);
						Settings.Entry_Quality_SSR_Vignette.Value = GUILayout.HorizontalSlider(Settings.Entry_Quality_SSR_Vignette.Value, 0, 1, null);
						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal(null);
						GUILayout.Label($"SSR Distance Fade ({Hacks.PostProcessLayerHook.SSR_DistanceFade:0.00}):", null);
						Settings.Entry_Quality_SSR_DistanceFade.Value = GUILayout.HorizontalSlider(Settings.Entry_Quality_SSR_DistanceFade.Value, 0, 0.5f, null);
						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal(null);
						GUILayout.Label($"SSR Max Marching Distance ({Hacks.PostProcessLayerHook.SSR_MaxMarchingDistance:0}):", null);
						Settings.Entry_Quality_SSR_MaxMarchingDistance.Value = GUILayout.HorizontalSlider(Settings.Entry_Quality_SSR_MaxMarchingDistance.Value, 50, 250, null);
						GUILayout.EndHorizontal();
					}
				}
				GUILayout.EndVertical();
			}

			//Color space
			if (Settings.Entry_Other_ShowAdvanced.Value)
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				GUILayout.Label($"Color space: {QualitySettings.activeColorSpace}", null);
				GUILayout.Label($"Desired color space: {QualitySettings.desiredColorSpace}", null);
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal(GUI.skin.box, null);
			if (GUILayout.Button("Unlock restart to defaults", null))
				restartingToDefault = 5f;
			if (restartingToDefault > 0)
			{
				if (GUILayout.Button($"Restart to default {restartingToDefault:0.0}s", null))
				{
					RestartToDefault();
				}
			}
			GUILayout.EndHorizontal();
		}

		private void RestartToDefault()
		{
			Settings.Entry_AnistropicFiltering.Value = Settings.Entry_AnistropicFiltering.DefaultValue;
			QualitySettings.anisotropicFiltering = Settings.Entry_AnistropicFiltering.Value;

			Settings.Entry_AnistropicFilteringValue.Value = Settings.Entry_AnistropicFilteringValue.DefaultValue;
			Texture.SetGlobalAnisotropicFilteringLimits(8, 16);

			Settings.Entry_Antialiasing.Value = Settings.Entry_Antialiasing.DefaultValue;
			Settings.Entry_Quality_CameraFarPlaneDistance.Value = Settings.Entry_Quality_CameraFarPlaneDistance.DefaultValue;

			Settings.Entry_Quality_LODBias.Value = Settings.Entry_Quality_LODBias.DefaultValue;
			Settings.Entry_Quality_PixelLightCount.Value = Settings.Entry_Quality_PixelLightCount.DefaultValue;
			Settings.Entry_Quality_ShadowDistance.Value = Settings.Entry_Quality_ShadowDistance.DefaultValue;

			Settings.Entry_Quality_ShadowFourSplitValue1.Value = Settings.Entry_Quality_ShadowFourSplitValue1.DefaultValue;
			Settings.Entry_Quality_ShadowFourSplitValue2.Value = Settings.Entry_Quality_ShadowFourSplitValue2.DefaultValue;
			Settings.Entry_Quality_ShadowFourSplitValue3.Value = Settings.Entry_Quality_ShadowFourSplitValue3.DefaultValue;
			QualitySettings.shadowCascade4Split = new Vector3(Settings.Entry_Quality_ShadowFourSplitValue1.Value, Settings.Entry_Quality_ShadowFourSplitValue2.Value, Settings.Entry_Quality_ShadowFourSplitValue3.Value);

			Settings.Entry_Quality_ShadowsQuality.Value = Settings.Entry_Quality_ShadowsQuality.DefaultValue;
			Settings.Entry_Quality_ShadowsResolution.Value = Settings.Entry_Quality_ShadowsResolution.DefaultValue;
			Settings.Entry_Quality_ShadowTwoSplitValue.Value = Settings.Entry_Quality_ShadowTwoSplitValue.DefaultValue;
			Settings.Entry_Quality_MirrorReflectionResolution.Value = Settings.Entry_Quality_MirrorReflectionResolution.DefaultValue;

			Settings.Entry_Quality_TextureQuality.Value = Settings.Entry_Quality_TextureQuality.DefaultValue;
			Settings.Entry_Quality_Use4ShadowCascades.Value = Settings.Entry_Quality_Use4ShadowCascades.DefaultValue;
			
			Settings.Entry_Quality_SSR_Enable.Value = Settings.Entry_Quality_SSR_Enable.DefaultValue;
			Settings.Entry_Quality_SSR_DistanceFade.Value = Settings.Entry_Quality_SSR_DistanceFade.DefaultValue;
			Settings.Entry_Quality_SSR_MaxMarchingDistance.Value = Settings.Entry_Quality_SSR_MaxMarchingDistance.DefaultValue;
			Settings.Entry_Quality_SSR_Preset.Value = Settings.Entry_Quality_SSR_Preset.DefaultValue;
			Settings.Entry_Quality_SSR_Resolution.Value = Settings.Entry_Quality_SSR_Resolution.DefaultValue;
			Settings.Entry_Quality_SSR_Tickness.Value = Settings.Entry_Quality_SSR_Tickness.DefaultValue;
			Settings.Entry_Quality_SSR_Vignette.Value = Settings.Entry_Quality_SSR_Vignette.DefaultValue;

			Settings.Entry_Quality_EdgeDetection.Value = Settings.Entry_Quality_EdgeDetection.DefaultValue;
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
			GUILayout.Label("Settings with * at the beginning require game restart!", null);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box, null);
			Settings.Entry_Other_SkipIntros.Value = GUILayout.Toggle(Settings.Entry_Other_SkipIntros.Value, "* Skip intros", null);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box, null);
			Settings.Entry_Other_InterpolateMovement.Value = GUILayout.Toggle(Settings.Entry_Other_InterpolateMovement.Value, "Interpolate movement", null);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box, null);
			GUILayout.Label($"* Geometry improvements ({Settings.Entry_Other_GeometryImprovements.Value}):", null);
			if (GUILayout.Button("Disabled", null))
				Settings.Entry_Other_GeometryImprovements.Value = ExposedSettings.GeometryImprovements.Disabled;
			if (GUILayout.Button("Minor (only fixes)", null))
				Settings.Entry_Other_GeometryImprovements.Value = ExposedSettings.GeometryImprovements.Minor;
			if (GUILayout.Button("All (tesselation etc.)", null))
				Settings.Entry_Other_GeometryImprovements.Value = ExposedSettings.GeometryImprovements.All;
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box, null);
			GUILayout.Label($"* Light improvements ({Settings.Entry_Other_LightImprovements.Value}):", null);
			if (GUILayout.Button("Disabled", null))
				Settings.Entry_Other_LightImprovements.Value = ExposedSettings.LightImprovements.Disabled;
			if (GUILayout.Button("Minor (performance safe)", null))
				Settings.Entry_Other_LightImprovements.Value = ExposedSettings.LightImprovements.Minor;
			if (GUILayout.Button("All", null))
				Settings.Entry_Other_LightImprovements.Value = ExposedSettings.LightImprovements.All;
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUI.skin.box, null);
			GUILayout.BeginVertical(null);
			var promptsUsed = Settings.Entry_Other_Prompts.Value == "" ? "None" : Settings.Entry_Other_Prompts.Value;

			GUILayout.Label($"* Prompts used: \"{promptsUsed}\" - possible:", null);
			if (GUILayout.Button("None", null))
				Settings.Entry_Other_Prompts.Value = "";

			var files = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Prompts"), "*.manifest");
			foreach (var file in files)
			{
				var split = file.Split('\\', '/', '.');
				var fileName = split[split.Length - 2];
				if (fileName == "keyboard")
					continue;

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