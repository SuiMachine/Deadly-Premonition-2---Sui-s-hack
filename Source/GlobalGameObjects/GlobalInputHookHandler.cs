using Steamworks;
using SuisHack.InputHandling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuisHack.GlobalGameObjects
{
	[MelonLoader.RegisterTypeInIl2Cpp]
	class GlobalInputHookHandler : MonoBehaviour
	{
		public GlobalInputHookHandler(IntPtr ptr) : base(ptr) { }
		public ExposedSettings Settings => SuisHackMain.Settings;

		public static GlobalInputHookHandler Instance { get; private set; }
		public static Dictionary<string, KeySteamAnalogAction> AnalogInputToInput;
		public Dictionary<string, KeySteamDigitalAction> DigitalInputToInput;

		internal KeySteamAnalogAction GetAnalogInputReplacement(InputAnalogActionHandle_t analogActionHandle)
		{
			if (TranslateAnalogBackToInput.TryGetValue(analogActionHandle, out KeySteamAnalogAction value))
			{
				return value;
			}
			return null;
		}

		internal KeySteamDigitalAction GetDigitalInputReplacement(InputDigitalActionHandle_t digitalActionHandle)
		{
			if (TranslateDigitalBackToInput.TryGetValue(digitalActionHandle, out KeySteamDigitalAction value))
			{
				return value;
			}
			return null;
		}

		public static Dictionary<InputAnalogActionHandle_t, KeySteamAnalogAction> TranslateAnalogBackToInput = new Dictionary<InputAnalogActionHandle_t, KeySteamAnalogAction>();
		public static Dictionary<InputDigitalActionHandle_t, KeySteamDigitalAction> TranslateDigitalBackToInput = new Dictionary<InputDigitalActionHandle_t, KeySteamDigitalAction>();

		public static void Initialize()
		{
			if (Instance == null)
			{
				var gameObject = new GameObject("GlobalInputHandler");
				GameObject.DontDestroyOnLoad(gameObject);
				Instance = gameObject.AddComponent<GlobalInputHookHandler>();
			}
		}

		private void Awake()
		{
			SuisHackMain.loggerInst.Msg("Initialized Global Input Hook Handler");
		}

		private void Start()
		{
			InitializeInputs();
		}

		public void InitializeInputs()
		{
			AnalogInputToInput = new Dictionary<string, KeySteamAnalogAction>()
			{
				{"L_Stick", new KeyActionAnalog(Settings.Input_Analog_LeftStick_Up.Value, Settings.Input_Analog_LeftStick_Down.Value, Settings.Input_Analog_LeftStick_Left.Value, Settings.Input_Analog_LeftStick_Right.Value, Settings.Input_Analog_LeftStickFloatTime.Value) },
				{"R_Stick", new MouseAnalog() }
			};

			DigitalInputToInput = new Dictionary<string, KeySteamDigitalAction>()
			{
				{ "A_Button", new KeySteamDigitalAction(Settings.Input_Digital_A_Button.Value, false) },
				{ "B_Button", new KeySteamDigitalAction(Settings.Input_Digital_B_Button.Value, false) },
				{ "X_Button", new KeySteamDigitalAction(Settings.Input_Digital_X_Button.Value, false) },
				{ "Y_Button", new KeySteamDigitalAction(Settings.Input_Digital_Y_Button.Value, false) },
				{ "LB", new KeySteamDigitalAction(Settings.Input_Digital_LB.Value, false) },
				{ "RB", new KeySteamDigitalAction(Settings.Input_Digital_RB.Value, false) },
				{ "Back_Button", new KeySteamDigitalAction(Settings.Input_Digital_Back_Button.Value, false) },
				{ "Start_Button", new KeySteamDigitalAction(Settings.Input_Digital_Start_Button.Value, false) },
				{ "L_Stick_Button", new KeySteamDigitalAction(Settings.Input_Digital_L_Stick_Button.Value, false) },
				{ "R_Stick_Button", new KeySteamDigitalAction(Settings.Input_Digital_Right_Button.Value, false) },
				{ "Up_Button", new KeySteamDigitalAction(Settings.Input_Digital_Up_Button.Value, true) },
				{ "Right_Button", new KeySteamDigitalAction(Settings.Input_Digital_Right_Button.Value, true) },
				{ "Down_Button", new KeySteamDigitalAction(Settings.Input_Digital_Down_Button.Value, true) },
				{ "Left_Button", new KeySteamDigitalAction(Settings.Input_Digital_Left_Button.Value, true) },
				{ "LT", new KeySteamDigitalAction(Settings.Input_Digital_LT.Value, false) },
				{ "RT", new KeySteamDigitalAction(Settings.Input_Digital_RT.Value, false) }
			};

			TranslateAnalogBackToInput.Clear();
			foreach (var analogInput in AnalogInputToInput)
			{
				var handle = SteamInput.GetAnalogActionHandle(analogInput.Key);
				TranslateAnalogBackToInput.Add(handle, analogInput.Value);
			}

			TranslateDigitalBackToInput.Clear();
			foreach (var digitalInput in DigitalInputToInput)
			{
				var handle = SteamInput.GetDigitalActionHandle(digitalInput.Key);
				TranslateDigitalBackToInput.Add(handle, digitalInput.Value);
			}
		}

		private void Update()
		{
			foreach (var analogInput in AnalogInputToInput)
			{
				analogInput.Value.Update();
			}
		}
	}
}
