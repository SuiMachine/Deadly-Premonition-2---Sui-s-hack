using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuisHack.KeyboardSupport
{
	[MelonLoader.RegisterTypeInIl2Cpp]
	class GlobalInputHookHandler : MonoBehaviour
	{
		public GlobalInputHookHandler(IntPtr ptr) : base(ptr) { }
		public ExposedSettings Settings;

		public enum SteamInputAnalog
		{
			L_Stick,
			R_Stick
		}

		public enum SteamInputDigital
		{
			A_Button,
			B_Button,
			X_Button,
			Y_Button,
			LB,
			RB,
			Back_Button,
			Start_Button,
			L_Stick_Button,
			R_Stick_Button,
			Up_Button,
			Right_Button,
			Down_Button,
			Left_Button,
			LT,
			RT
		}

		public static GlobalInputHookHandler Instance { get; private set; }
		public static Dictionary<SteamInputAnalog, KeySteamAnalogAction> AnalogInputToInput;
		public Dictionary<SteamInputDigital, KeySteamDigitalAction> DigitalInputToInput;


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

		public KeyCode GetReplacementPrompt(SteamInputDigital gamepadKey)
		{
			return DigitalInputToInput[gamepadKey].GetBind();
		}

		private void Awake()
		{
			SuisHackMain.loggerInst.Msg("Initialized Global Input Hook Handler");
			Settings = SuisHackMain.Settings;
		}

		private void Start()
		{
			InitializeInputs();
		}

		public void InitializeInputs()
		{
			AnalogInputToInput = new Dictionary<SteamInputAnalog, KeySteamAnalogAction>()
			{
				{SteamInputAnalog.L_Stick, new KeyActionAnalog(Settings.Input_Analog_LeftStick_Up.Value, Settings.Input_Analog_LeftStick_Down.Value, Settings.Input_Analog_LeftStick_Left.Value, Settings.Input_Analog_LeftStick_Right.Value, Settings.Input_Analog_LeftStickFloatTime.Value) },
				{SteamInputAnalog.R_Stick, new MouseAnalog() }
			};

			DigitalInputToInput = new Dictionary<SteamInputDigital, KeySteamDigitalAction>()
			{
				{ SteamInputDigital.A_Button, new KeySteamDigitalAction(Settings.Input_Digital_A_Button.Value, false) },
				{ SteamInputDigital.B_Button, new KeySteamDigitalAction(Settings.Input_Digital_B_Button.Value, false) },
				{ SteamInputDigital.X_Button, new KeySteamDigitalAction(Settings.Input_Digital_X_Button.Value, false) },
				{ SteamInputDigital.Y_Button, new KeySteamDigitalAction(Settings.Input_Digital_Y_Button.Value, false) },
				{ SteamInputDigital.LB, new KeySteamDigitalAction(Settings.Input_Digital_LB.Value, false) },
				{ SteamInputDigital.RB, new KeySteamDigitalAction(Settings.Input_Digital_RB.Value, false) },
				{ SteamInputDigital.Back_Button, new KeySteamDigitalAction(Settings.Input_Digital_Back_Button.Value, false) },
				{ SteamInputDigital.Start_Button, new KeySteamDigitalAction(Settings.Input_Digital_Start_Button.Value, false) },
				{ SteamInputDigital.L_Stick_Button, new KeySteamDigitalAction(Settings.Input_Digital_L_Stick_Button.Value, false) },
				{ SteamInputDigital.R_Stick_Button, new KeySteamDigitalAction(Settings.Input_Digital_Right_Button.Value, false) },
				{ SteamInputDigital.Up_Button, new KeySteamDigitalAction(Settings.Input_Digital_Up_Button.Value, true) },
				{ SteamInputDigital.Right_Button, new KeySteamDigitalAction(Settings.Input_Digital_Right_Button.Value, true) },
				{ SteamInputDigital.Down_Button, new KeySteamDigitalAction(Settings.Input_Digital_Down_Button.Value, true) },
				{ SteamInputDigital.Left_Button, new KeySteamDigitalAction(Settings.Input_Digital_Left_Button.Value, true) },
				{ SteamInputDigital.LT, new KeySteamDigitalAction(Settings.Input_Digital_LT.Value, false) },
				{ SteamInputDigital.RT, new KeySteamDigitalAction(Settings.Input_Digital_RT.Value, false) }
			};

			TranslateAnalogBackToInput.Clear();
			foreach (var analogInput in AnalogInputToInput)
			{
				var handle = SteamInput.GetAnalogActionHandle(analogInput.Key.ToString());
				TranslateAnalogBackToInput.Add(handle, analogInput.Value);
			}

			TranslateDigitalBackToInput.Clear();
			foreach (var digitalInput in DigitalInputToInput)
			{
				var handle = SteamInput.GetDigitalActionHandle(digitalInput.Key.ToString());
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
