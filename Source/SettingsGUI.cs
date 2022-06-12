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
				if(GUILayout.Button("LOL", null))
				{
					Screen.SetResolution(1280, 720, false);
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
		}
	}
}
