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

		public static GlobalInputHookHandler Instance { get; private set; }
		private Dictionary<SteamInputHook.SteamInputAnalog, KeySteamAnalogAction> AnalogInputToInput;
		private Dictionary<SteamInputHook.SteamInputDigital, KeySteamDigitalAction> DigitalInputToInput;


		public InputAnalogActionData_t GetAnalogInputReplacement(SteamInputHook.SteamInputAnalog analogActionHandle)
		{
			switch (GameStateMachine.CurrentGameState)
			{
				case GameStateMachine.Gamestate.Menu:
				case GameStateMachine.Gamestate.RedRoom:
					return MenuTranslateAnalogInput(analogActionHandle);
				default:
					return TranslateAnalogBackToInput[(int)analogActionHandle].GetInput();
			}
		}

		private InputAnalogActionData_t MenuTranslateAnalogInput(SteamInputHook.SteamInputAnalog analogActionHandle)
		{
			//Disable mouse in menu
			if (analogActionHandle == SteamInputHook.SteamInputAnalog.R_Stick)
				return new InputAnalogActionData_t();
			else
				return TranslateAnalogBackToInput[(int)analogActionHandle].GetInput();
		}

		public InputDigitalActionData_t GetDigitalInputReplacement(SteamInputHook.SteamInputDigital digitalAction)
		{
			switch (GameStateMachine.CurrentGameState)
			{
				case GameStateMachine.Gamestate.RedRoom:
				case GameStateMachine.Gamestate.Menu:
					return MenuTranslateDigitalInput(digitalAction);
				case GameStateMachine.Gamestate.Map:
					return TranslateDigitalInputMap(digitalAction);
				default:
					return TranslateDigitalBackToInput[(int)digitalAction].GetInput();
			}
		}

		private InputDigitalActionData_t MenuTranslateDigitalInput(SteamInputHook.SteamInputDigital digitalAction)
		{
			var input = MenuTranslateDigitalBackToInput[(int)digitalAction];
			if (input != KeyCode.None)
			{
				return new InputDigitalActionData_t()
				{
					bActive = 1,
					bState = Input.GetKeyDown(input) ? (byte)1 : (byte)0
				};
			}
			else
				return new InputDigitalActionData_t();

		}

		private InputDigitalActionData_t TranslateDigitalInputMap(SteamInputHook.SteamInputDigital digitalAction)
		{
			return new InputDigitalActionData_t()
			{
				bActive = 1,
				bState = Input.GetKey(MapTranslateDigitalBackToInput[(int)digitalAction]) ? (byte)1 : (byte)0
			};
		}

		private static KeySteamAnalogAction[] TranslateAnalogBackToInput = new KeySteamAnalogAction[3];
		private static KeySteamDigitalAction[] TranslateDigitalBackToInput = new KeySteamDigitalAction[17];

		private static KeyCode[] MenuTranslateDigitalBackToInput = new KeyCode[17];
		private static KeyCode[] MapTranslateDigitalBackToInput = new KeyCode[17];


		public static void Initialize()
		{
			if (Instance == null)
			{
				var gameObject = new GameObject("GlobalInputHandler");
				GameObject.DontDestroyOnLoad(gameObject);
				Instance = gameObject.AddComponent<GlobalInputHookHandler>();
			}
		}

		public KeyCode GetReplacementPrompt(SteamInputHook.SteamInputDigital gamepadKey)
		{
			switch (GameStateMachine.CurrentGameState)
			{
				case GameStateMachine.Gamestate.Menu:
					return MenuTranslateDigitalBackToInput[(int)gamepadKey];
				default:
					return DigitalInputToInput[gamepadKey].GetBind();
			}			
		}

		private void Awake()
		{
			SuisHackMain.loggerInst.Msg("Initialized Global Input Hook Handler");
			Settings = SuisHackMain.Settings;
		}

		private void Start()
		{
			InitializeInputs();
			InitializeInputsMenu();
			InitializeInputsMap();
		}

		public void InitializeInputs()
		{
			AnalogInputToInput = new Dictionary<SteamInputHook.SteamInputAnalog, KeySteamAnalogAction>()
			{
				{ SteamInputHook.SteamInputAnalog.L_Stick, new KeyActionAnalog(Settings.Input_Analog_LeftStick_Up.Value, Settings.Input_Analog_LeftStick_Down.Value, Settings.Input_Analog_LeftStick_Left.Value, Settings.Input_Analog_LeftStick_Right.Value, Settings.Input_Analog_LeftStickFloatTime.Value) },
				{ SteamInputHook.SteamInputAnalog.R_Stick, new MouseAnalog() },
			};

			DigitalInputToInput = new Dictionary<SteamInputHook.SteamInputDigital, KeySteamDigitalAction>()
			{
				{ SteamInputHook.SteamInputDigital.A_Button, new KeySteamDigitalAction(Settings.Input_Digital_A_Button.Value, false) },
				{ SteamInputHook.SteamInputDigital.B_Button, new KeySteamDigitalAction(Settings.Input_Digital_B_Button.Value, false) },
				{ SteamInputHook.SteamInputDigital.X_Button, new KeySteamDigitalAction(Settings.Input_Digital_X_Button.Value, false) },
				{ SteamInputHook.SteamInputDigital.Y_Button, new KeySteamDigitalAction(Settings.Input_Digital_Y_Button.Value, false) },
				{ SteamInputHook.SteamInputDigital.LB, new KeySteamDigitalAction(Settings.Input_Digital_LB.Value, false) },
				{ SteamInputHook.SteamInputDigital.RB, new KeySteamDigitalAction(Settings.Input_Digital_RB.Value, false) },
				{ SteamInputHook.SteamInputDigital.Back_Button, new KeySteamDigitalAction(Settings.Input_Digital_Back_Button.Value, false) },
				{ SteamInputHook.SteamInputDigital.Start_Button, new KeySteamDigitalAction(Settings.Input_Digital_Start_Button.Value, false) },
				{ SteamInputHook.SteamInputDigital.L_Stick_Button, new KeySteamDigitalAction(Settings.Input_Digital_L_Stick_Button.Value, false) },
				{ SteamInputHook.SteamInputDigital.R_Stick_Button, new KeySteamDigitalAction(Settings.Input_Digital_Right_Button.Value, false) },
				{ SteamInputHook.SteamInputDigital.Up_Button, new KeySteamDigitalAction(Settings.Input_Digital_Up_Button.Value, true) },
				{ SteamInputHook.SteamInputDigital.Right_Button, new KeySteamDigitalAction(Settings.Input_Digital_Right_Button.Value, true) },
				{ SteamInputHook.SteamInputDigital.Down_Button, new KeySteamDigitalAction(Settings.Input_Digital_Down_Button.Value, true) },
				{ SteamInputHook.SteamInputDigital.Left_Button, new KeySteamDigitalAction(Settings.Input_Digital_Left_Button.Value, true) },
				{ SteamInputHook.SteamInputDigital.LT, new KeySteamDigitalAction(Settings.Input_Digital_LT.Value, false) },
				{ SteamInputHook.SteamInputDigital.RT, new KeySteamDigitalAction(Settings.Input_Digital_RT.Value, false) }
			};

			TranslateAnalogBackToInput[0] = new KeyActionAnalog(KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None, 1);
			int i = 1;
			foreach (var analogInput in AnalogInputToInput)
			{
				TranslateAnalogBackToInput[i] = analogInput.Value;
				i++;
			}

			TranslateDigitalBackToInput[0] = new KeySteamDigitalAction(KeyCode.None, true);
			i = 1;
			foreach (var digitalInput in DigitalInputToInput)
			{
				TranslateDigitalBackToInput[i] = digitalInput.Value;
				i++;
			}
		}

		private void InitializeInputsMenu()
		{
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.None] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.A_Button] = KeyCode.Escape;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.B_Button] = KeyCode.Return;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.X_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Y_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.LB] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.RB] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Back_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Start_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.L_Stick_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.R_Stick_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Up_Button] = KeyCode.UpArrow;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Right_Button] = KeyCode.RightArrow;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Down_Button] = KeyCode.DownArrow;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Left_Button] = KeyCode.LeftArrow;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.LT] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.RT] = KeyCode.None;
		}

		private void InitializeInputsMap()
		{
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.None] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.A_Button] = KeyCode.Return;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.B_Button] = KeyCode.Escape;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.X_Button] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Y_Button] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.LB] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.RB] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Back_Button] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Start_Button] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.L_Stick_Button] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.R_Stick_Button] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Up_Button] = KeyCode.UpArrow;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Right_Button] = KeyCode.RightArrow;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Down_Button] = KeyCode.DownArrow;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Left_Button] = KeyCode.LeftArrow;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.LT] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.RT] = KeyCode.None;
		}

/*		private void Template()
		{
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.None] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.A_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.B_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.X_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Y_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.LB] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.RB] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Back_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Start_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.L_Stick_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.R_Stick_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Up_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Right_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Down_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Left_Button] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.LT] = KeyCode.None;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.RT] = KeyCode.None;
		}*/

		private void Update()
		{
			foreach (var analogInput in AnalogInputToInput)
			{
				analogInput.Value.Update();
			}
		}
	}
}
