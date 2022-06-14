using Il2CppSystem.IO;
using MelonLoader;
using System;
using UnityEngine;

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
		private Vector2 scrollPos;
		private ExposedSettings Settings => SuisHackMain.Settings;
		Vector2 resolution;
		int refreshRate;

		public static void Initialize()
		{
			if(Instance == null)
			{
				var gameObject = new GameObject("SuisHackSettingsGUI");
				Instance = gameObject.AddComponent<SettingsGUI>();
				DontDestroyOnLoad(gameObject);
			}
		}

		void Start()
		{
			resolution = new Vector2((float)Screen.currentResolution.width, (float)Screen.currentResolution.height);
			refreshRate = Settings.Entry_Display_RefreshRate.Value;
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
			if(m_Display)
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
				scrollPos = Vector2.zero;
			}
			if (GUILayout.Button("Quality settings", null))
			{
				category = Category.Quality;
				scrollPos = Vector2.zero;
			}
			if (GUILayout.Button("Other settings", null))
			{
				category = Category.Other;
				scrollPos = Vector2.zero;
			}
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
				GUILayout.Label($"Current resolution ({Screen.currentResolution.width}x{Screen.currentResolution.height})", null);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(null);
				GUILayout.Label($"Desired resolution:", null);
				var x = GUILayout.TextField(resolution.x.ToString(), null);
				GUILayout.Label($"x", null);
				var y = GUILayout.TextField(resolution.y.ToString(), null);
				var newResX = (int)resolution.x;
				var newResY = (int)resolution.y;

				if (int.TryParse(x, out int resX))
					newResX = resX;
				if (int.TryParse(y, out int resY))
					newResY = resY;

				resolution = new Vector2(newResX, newResY);
				if (GUILayout.Button("Apply", null))
				{
					Screen.SetResolution(newResX, newResY, Settings.Entry_Display_DisplayMode.Value, Settings.Entry_Display_RefreshRate.Value);
					Settings.Entry_Display_Resolution.Value = $"{newResX}x{newResY}";
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

			//Display mode
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				GUILayout.Label($"Display mode ({Screen.fullScreenMode}):", null);

				if(GUILayout.Button("Exclusive fullscreen", null))
				{
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.ExclusiveFullScreen;
					Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Settings.Entry_Display_DisplayMode.Value, Screen.currentResolution.refreshRate);
				}
				if (GUILayout.Button("Maximized window", null))
				{
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.MaximizedWindow;
					Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Settings.Entry_Display_DisplayMode.Value, Screen.currentResolution.refreshRate);
				}
				if (GUILayout.Button("Borderless window", null))
				{
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.FullScreenWindow;
					Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Settings.Entry_Display_DisplayMode.Value, Screen.currentResolution.refreshRate);
				}
				if (GUILayout.Button("Windowed", null))
				{
					Settings.Entry_Display_DisplayMode.Value = FullScreenMode.Windowed;
					Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Settings.Entry_Display_DisplayMode.Value, Screen.currentResolution.refreshRate);
				}
				GUILayout.EndHorizontal();
			}

			//Vsync
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				var oldValue = QualitySettings.vSyncCount > 0 ? true : false;
				var newValue = GUILayout.Toggle(oldValue, "V-sync", null);
				if(newValue != oldValue)
				{
					QualitySettings.vSyncCount = newValue ? 1 : 0;
					Settings.Entry_Display_Vsync.Value = newValue;
					if(newValue)
					{
						Application.targetFrameRate = refreshRate;
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
					var newValue = GUILayout.TextField(refreshRate.ToString(), null);
					if(uint.TryParse(newValue, out var result))
					{
						refreshRate = (int)Mathf.Clamp(result, 30, 480);
					}
					if(GUILayout.Button("Apply", null))
					{
						Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreenMode, refreshRate);
						Settings.Entry_Display_RefreshRate.Value = refreshRate;
					}
				}
				else
				{
					GUILayout.Label($"FPS cap {refreshRate}", null);
					var oldValue = refreshRate;
					refreshRate = (int)GUILayout.HorizontalSlider(refreshRate, -1, 1000, null);
					if(oldValue != refreshRate)
					{
						Application.targetFrameRate = refreshRate;
						Settings.Entry_Display_RefreshRate.Value = refreshRate;
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

			//LOD Bias
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				var oldValue = QualitySettings.lodBias;
				GUILayout.Label($"LOD Bias ({QualitySettings.lodBias.ToString("0.0")}):", null);
				QualitySettings.lodBias = GUILayout.HorizontalSlider(QualitySettings.lodBias, 0.5f, 4f, null);
				if(oldValue != QualitySettings.lodBias)
				{
					Settings.Entry_Quality_LODBias.Value = QualitySettings.lodBias;
				}
				GUILayout.EndHorizontal();
			}

			//Pixel Light Count
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
				if(GUILayout.Button("Full", null))
					QualitySettings.masterTextureLimit = 0;
				if (GUILayout.Button("1/2", null))
					QualitySettings.masterTextureLimit = 1;
				if (GUILayout.Button("1/4", null))
					QualitySettings.masterTextureLimit = 2;
				if (GUILayout.Button("1/8", null))
					QualitySettings.masterTextureLimit = 3;
				if (GUILayout.Button("1/16", null))
					QualitySettings.masterTextureLimit = 4;

				if (oldValue != QualitySettings.masterTextureLimit)
					Settings.Entry_Quality_TextureQuality.Value = QualitySettings.masterTextureLimit;
				GUILayout.EndHorizontal();
			}

			//Real-time reflection probes toggle
			{
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				var newValue = GUILayout.Toggle(QualitySettings.realtimeReflectionProbes, "Realtime Reflection probes", null);

				if(newValue != QualitySettings.realtimeReflectionProbes)
				{
					QualitySettings.realtimeReflectionProbes = newValue;
					Settings.Entry_Quality_RealtimeReflectionProbes.Value = newValue;
				}
				GUILayout.EndHorizontal();
			}
		}

		private string GetTextureString(int masterTextureLimit)
		{
			//I wish I could use 8.0 recursive patern
			switch(masterTextureLimit)
			{
				case 1:
					return "1/2";
				case 2:
					return "1/4";
				case 3:
					return "1/8";
				case 4:
					return "1/16";
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
