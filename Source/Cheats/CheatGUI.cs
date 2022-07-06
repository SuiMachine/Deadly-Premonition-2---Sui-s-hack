using MelonLoader;
using System;
using UnityEngine;

namespace SuisHack.Cheats
{
	[RegisterTypeInIl2Cpp]
	public class CheatGUI : MonoBehaviour
	{
		public CheatGUI(IntPtr ptr) : base(ptr) { }
		public static CheatGUI Instance;

		public class References
		{
			public CharaControl __instance { get; private set; }
			public CharaDamage Damage { get; private set; }
			public Poison Poison { get; private set; }
			public Stench Stench { get; private set; }
			public Sunburn Sunburn { get; private set; }
			public Paralysis Paralysis { get; private set; }
			public Drunk Drunk { get; private set; }
			public Chills Chills { get; private set; }
			public Beard Beard { get; private set; }

			public References()
			{
				this.__instance = FindObjectOfType<CharaControl>();
				if (__instance != null)
					FillRef();
			}

			private void FillRef()
			{
				Damage = __instance.GetComponent<CharaDamage>();
				Poison = __instance.GetComponent<Poison>();
				Stench = __instance.GetComponent<Stench>();
				Sunburn = __instance.GetComponent<Sunburn>();
				Paralysis = __instance.GetComponent<Paralysis>();
				Drunk = __instance.GetComponent<Drunk>();
				Chills = __instance.GetComponent<Chills>();
				Beard = __instance.GetComponent<Beard>();
			}

			internal bool HasAllReferences()
			{
				return Damage != null
					&& Poison != null
					&& Stench != null
					&& Sunburn != null;
			}
		}

		public struct DisplaysPlayer
		{
			public bool Abilities;
			public bool StatsSliders;
		}

		private References Character;
		private bool Display;
		private DisplaysPlayer DisplaysCharacter;

		enum Category
		{
			Root,
			Player,
			Inventory,
		}

		private Category category = Category.Root;

		public static void Initialize()
		{
			if (Instance == null)
			{
				var go = new GameObject("CheatGUI");
				DontDestroyOnLoad(go);
				Instance = go.AddComponent<CheatGUI>();
			}
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F8))
			{
				if (Character == null)
					Character = new References();


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

		private void DrawRoot()
		{
			GUILayout.Label("Cheat categories:", null);
			if (GUILayout.Button("Player", null))
			{
				category = Category.Player;
			}
			if (GUILayout.Button("Inventory", null))
			{
				category = Category.Inventory;
			}
		}

		private void OnGUI()
		{
			if (Character == null || !Character.HasAllReferences())
				return;

			if (Display)
			{
				GUILayout.BeginVertical(null);
				GUILayout.BeginHorizontal(GUI.skin.box, null);
				DrawRoot();
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

			switch (category)
			{
				case Category.Player:
					DrawPlayerStats();
					break;
				case Category.Inventory:
					break;
			}
		}

		private void DrawPlayerStats()
		{
			GUIStyle richText = GUI.skin.label;
			richText.richText = true;

			GUILayout.BeginHorizontal(GUI.skin.box, null);
			GUILayout.Label("<b>Player info:</b>", richText, null);
			GUILayout.EndHorizontal();

			GUILayout.BeginVertical(GUI.skin.box, null);
			DisplaysCharacter.Abilities = GUILayout.Toggle(DisplaysCharacter.Abilities, "Display character abilities", null);
			if (DisplaysCharacter.Abilities)
			{
				Character.__instance.EnableBoardRide = GUILayout.Toggle(Character.__instance.EnableBoardRide, "Enable Board Ride", null);
				Character.__instance.IsNoDamageChara = GUILayout.Toggle(Character.__instance.IsNoDamageChara, "Is no damage character", null);
				Character.__instance.IsNoDamagedBoard = GUILayout.Toggle(Character.__instance.IsNoDamagedBoard, "No damage board", null);
				Character.__instance.IsSpotLight = GUILayout.Toggle(Character.__instance.IsSpotLight, "Spotlight enabled", null);
				Character.__instance.m_dashEnable = GUILayout.Toggle(Character.__instance.m_dashEnable, "Dash enabled", null);
				Character.__instance.m_dashEnable = GUILayout.Toggle(Character.__instance.m_dashEnable, "Dash enabled", null);
				Character.__instance.m_enableStaminaRecovery = GUILayout.Toggle(Character.__instance.m_enableStaminaRecovery, "Stamina recovery", null);
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical(GUI.skin.box, null);
			DisplaysCharacter.StatsSliders = GUILayout.Toggle(DisplaysCharacter.StatsSliders, "Stats sliders", null);
			if (DisplaysCharacter.StatsSliders)
			{
				Character.__instance.m_HP = (int)GUILayout.HorizontalSlider(Character.__instance.m_HP, 1, Character.__instance.m_MaxHP, null);
			}
			GUILayout.EndVertical();



		}
	}
}
