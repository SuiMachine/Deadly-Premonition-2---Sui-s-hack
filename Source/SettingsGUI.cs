using MelonLoader;
using System;
using UnityEngine;

namespace SuisHack
{
	[RegisterTypeInIl2Cpp]
	public class SettingsGUI : MonoBehaviour
	{
		public SettingsGUI(IntPtr ptr) : base(ptr) { }
		public static SettingsGUI Instance { get; private set; }
		private bool m_Display = false;

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
			var settings = SuisHack.SuisHackMain.Settings;
			if(settings.Resolution != null)
			{
				var resolution = settings.Resolution.Value;
				Screen.SetResolution(resolution.X, resolution.Y, false);
				SuisHack.SuisHackMain.loggerInst.Msg($"Applying resolution {resolution.X}x{resolution.Y}");
			}
		}

		void Update()
		{
			if (Input.GetKey(KeyCode.F11))
				m_Display = !m_Display;
		}


		public void OnGUI()
		{
			if(m_Display)
			{
				
			}
		}
	}
}
