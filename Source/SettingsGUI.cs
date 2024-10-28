using System;
using UnityEngine;

namespace SuisHack
{
	public class SettingsGUI : MonoBehaviour
	{
		public SettingsGUI(IntPtr handle) : base(handle) {}

		public static SettingsGUI Instance { get; private set; }
		public static bool Display { get; private set; }

		//private RebindingActions CurrentRebinding;


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
			Plugin.Error("Dupa awake");
			DontDestroyOnLoad(gameObject);
		}

		public void Start()
		{
			Plugin.Error("Dupa start");
			DontDestroyOnLoad(gameObject);
		}

		public void Update()
		{
			Plugin.Error("Kurwa");
		}

		void OnDestroy()
		{
			Plugin.Error("Destrey start");
		}
	}
}
