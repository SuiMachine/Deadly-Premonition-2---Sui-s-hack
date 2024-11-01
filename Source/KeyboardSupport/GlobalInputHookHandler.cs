using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using static SuisHack.KeyboardSupport.SteamInputHook;

namespace SuisHack.KeyboardSupport
{
	class GlobalInputHookHandler : MonoBehaviour
	{
		public GlobalInputHookHandler(IntPtr ptr) : base(ptr) { }

		public static GlobalInputHookHandler? Instance { get; private set; }
		public Dictionary<SteamInputHook.SteamInputAnalog, KeySteamAnalogAction> AnalogInputToInput;
		public Dictionary<SteamInputHook.SteamInputDigital, KeySteamDigitalAction> DigitalInputToInput;


		public static KeyCode GetInputForRebinding(RebindingActions action)
		{
			if (Instance != null)
			{
				switch (action)
				{
					case RebindingActions.Forward:
						return Instance.AnalogInputToInput[SteamInputAnalog.L_Stick].GetUpKeyCode();
					case RebindingActions.Backward:
						return Instance.AnalogInputToInput[SteamInputAnalog.L_Stick].GetDownKeyCode();
					case RebindingActions.Left:
						return Instance.AnalogInputToInput[SteamInputAnalog.L_Stick].GetLeftKeyCode();
					case RebindingActions.Right:
						return Instance.AnalogInputToInput[SteamInputAnalog.L_Stick].GetRightKeyCode();
					case RebindingActions.Crouch:
						return Instance.DigitalInputToInput[SteamInputDigital.L_Stick_Button].GetBind();
					case RebindingActions.Dash_Dodge:
						return Instance.DigitalInputToInput[SteamInputDigital.RB].GetBind();
					case RebindingActions.Fire_Weapon_Punch:
						return Instance.DigitalInputToInput[SteamInputDigital.RT].GetBind();
					case RebindingActions.Point_Gun:
						return Instance.DigitalInputToInput[SteamInputDigital.LT].GetBind();
					case RebindingActions.Vision:
						return Instance.DigitalInputToInput[SteamInputDigital.LB].GetBind();
					case RebindingActions.Interact_Reload_Accellerate:
						return Instance.DigitalInputToInput[SteamInputDigital.A_Button].GetBind();
					case RebindingActions.Cancel_Brake:
						return Instance.DigitalInputToInput[SteamInputDigital.B_Button].GetBind();
					case RebindingActions.Reset_Camera_Fighting_Style:
						return Instance.DigitalInputToInput[SteamInputDigital.R_Stick_Button].GetBind();
					case RebindingActions.Display_Map:
						return Instance.DigitalInputToInput[SteamInputDigital.Back_Button].GetBind();
					case RebindingActions.Quest_Display:
						return Instance.DigitalInputToInput[SteamInputDigital.Start_Button].GetBind();
					case RebindingActions.Skateboard:
						return Instance.DigitalInputToInput[SteamInputDigital.Y_Button].GetBind();
					case RebindingActions.SwitchSlotLeft:
						return Instance.DigitalInputToInput[SteamInputDigital.Left_Button].GetBind();
					case RebindingActions.SwitchSlotRight:
						return Instance.DigitalInputToInput[SteamInputDigital.Right_Button].GetBind();
					case RebindingActions.SwitchAlbumDisplayDown:
						return Instance.DigitalInputToInput[SteamInputDigital.Down_Button].GetBind();
					case RebindingActions.SwitchAlbumDisplayUp:
						return Instance.DigitalInputToInput[SteamInputDigital.Up_Button].GetBind();
					default:
						return KeyCode.None;
				}
			}
			else
				return KeyCode.None;
		}

		public InputAnalogActionData_t GetAnalogInputReplacement(SteamInputHook.SteamInputAnalog analogActionHandle)
		{
			switch (GameStateMachine.CurrentGameState)
			{
				case GameStateMachine.Gamestate.Menu:
				case GameStateMachine.Gamestate.RedRoom:
					return MenuTranslateAnalogInput(analogActionHandle);
				case GameStateMachine.Gamestate.Map:
					return MapTranslateAnalogInput(analogActionHandle);
				default:
					if (MouseAnalog.InvertYAxis)
					{
						var input = TranslateAnalogBackToInput[(int)analogActionHandle].GetInput();
						input.y *= -1.0f;
						return input;
					}
					else
						return TranslateAnalogBackToInput[(int)analogActionHandle].GetInput();
			}
		}

		private InputAnalogActionData_t MapTranslateAnalogInput(SteamInputHook.SteamInputAnalog analogActionHandle)
		{
			//Disable keyboard WSAD
			if (analogActionHandle == SteamInputHook.SteamInputAnalog.L_Stick)
				return TranslateAnalogBackToInput[(int)SteamInputHook.SteamInputAnalog.R_Stick].GetInput();
			else
				return new InputAnalogActionData_t();
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
				case GameStateMachine.Gamestate.Menu:
					return MenuTranslateDigitalInput(digitalAction);
				case GameStateMachine.Gamestate.RedRoom:
					return RedRoomTranslateDigitalInput(digitalAction);
				case GameStateMachine.Gamestate.Map:
					return TranslateDigitalInputMap(digitalAction);
				default:
					return TranslateDigitalBackToInput[(int)digitalAction].GetInput();
			}
		}

		private InputDigitalActionData_t MenuTranslateDigitalInput(SteamInputHook.SteamInputDigital digitalAction)
		{
			return new InputDigitalActionData_t()
			{
				bActive = 1,
				bState = Input.GetKeyDown(MenuTranslateDigitalBackToInput[(int)digitalAction]) ? (byte)1 : (byte)0
			};
		}

		private InputDigitalActionData_t RedRoomTranslateDigitalInput(SteamInputHook.SteamInputDigital digitalAction)
		{
			return new InputDigitalActionData_t()
			{
				bActive = 1,
				bState = Input.GetKeyDown(RedRoomTranslateDigitalBackToInput[(int)digitalAction]) ? (byte)1 : (byte)0
			};
		}

		private InputDigitalActionData_t TranslateDigitalInputMap(SteamInputHook.SteamInputDigital digitalAction)
		{
			//Zoom in
			if (digitalAction == SteamInputHook.SteamInputDigital.LB)
			{
				if (Input.mouseScrollDelta.y < -0.5f)
					return new InputDigitalActionData_t() { bActive = 1, bState = 1 };
				else
					return new InputDigitalActionData_t()
					{
						bActive = 1,
						bState = Input.GetKey(MapTranslateDigitalBackToInput[(int)digitalAction]) ? (byte)1 : (byte)0
					};
			}
			else if (digitalAction == SteamInputHook.SteamInputDigital.RB)
			{
				if (Input.mouseScrollDelta.y > 0.5f)
					return new InputDigitalActionData_t() { bActive = 1, bState = 1 };
				else
					return new InputDigitalActionData_t()
					{
						bActive = 1,
						bState = Input.GetKey(MapTranslateDigitalBackToInput[(int)digitalAction]) ? (byte)1 : (byte)0
					};
			}

			return new InputDigitalActionData_t()
			{
				bActive = 1,
				bState = Input.GetKey(MapTranslateDigitalBackToInput[(int)digitalAction]) ? (byte)1 : (byte)0
			};
		}

		private static KeySteamAnalogAction[] TranslateAnalogBackToInput = new KeySteamAnalogAction[3];
		private static KeySteamDigitalAction[] TranslateDigitalBackToInput = new KeySteamDigitalAction[17];

		private static KeyCode[] MenuTranslateDigitalBackToInput = new KeyCode[17];
		private static KeyCode[] RedRoomTranslateDigitalBackToInput = new KeyCode[17];
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
				case GameStateMachine.Gamestate.RedRoom:
					return RedRoomTranslateDigitalBackToInput[(int)gamepadKey];
				case GameStateMachine.Gamestate.Map:
					return MapTranslateDigitalBackToInput[(int)gamepadKey];
				default:
					return DigitalInputToInput[gamepadKey].GetBind();
			}
		}

		private void Awake()
		{
			Plugin.Message("Initialized Global Input Hook Handler");
			this.hideFlags = HideFlags.HideAndDontSave;
		}

		private void Start()
		{
			InitializeInputs();
			InitializeInputsMenu();
			InitializeRedRoomMenu();
			InitializeInputsMap();
		}

		public void InitializeInputs()
		{
			var Settings = ExposedSettings.Instance;

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
				{ SteamInputHook.SteamInputDigital.Back_Button, new KeySteamDigitalAction(Settings.Input_Digital_Back_Button.Value, false) },
				{ SteamInputHook.SteamInputDigital.Start_Button, new KeySteamDigitalAction(Settings.Input_Digital_Start_Button.Value, false) },
				{ SteamInputHook.SteamInputDigital.Up_Button, new KeySteamDigitalAction(Settings.Input_Digital_Up_Button.Value, true) },
				{ SteamInputHook.SteamInputDigital.Down_Button, new KeySteamDigitalAction(Settings.Input_Digital_Down_Button.Value, true) },
				{ SteamInputHook.SteamInputDigital.Left_Button, new KeySteamDigitalAction(Settings.Input_Digital_Left_Button.Value, true) },
				{ SteamInputHook.SteamInputDigital.Right_Button, new KeySteamDigitalAction(Settings.Input_Digital_Right_Button.Value, true) },
				{ SteamInputHook.SteamInputDigital.LB, new KeySteamDigitalAction(Settings.Input_Digital_LB.Value, false) },
				{ SteamInputHook.SteamInputDigital.RB, new KeySteamDigitalAction(Settings.Input_Digital_RB.Value, false) },
				{ SteamInputHook.SteamInputDigital.LT, new KeySteamDigitalAction(Settings.Input_Digital_LT.Value, false) },
				{ SteamInputHook.SteamInputDigital.RT, new KeySteamDigitalAction(Settings.Input_Digital_RT.Value, false) },
				{ SteamInputHook.SteamInputDigital.L_Stick_Button, new KeySteamDigitalAction(Settings.Input_Digital_L_Stick_Button.Value, false) },
				{ SteamInputHook.SteamInputDigital.R_Stick_Button, new KeySteamDigitalAction(Settings.Input_Digital_Right_Button.Value, false) },
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
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Y_Button] = KeyCode.Backspace;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.LB] = KeyCode.Insert;
			MenuTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.RB] = KeyCode.PageUp;
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

		private void InitializeRedRoomMenu()
		{
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.None] = KeyCode.None;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.A_Button] = KeyCode.Escape;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.B_Button] = KeyCode.Return;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.X_Button] = KeyCode.None;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Y_Button] = KeyCode.Backspace;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.LB] = KeyCode.None;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.RB] = KeyCode.Tab;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Back_Button] = KeyCode.None;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Start_Button] = KeyCode.None;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.L_Stick_Button] = KeyCode.None;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.R_Stick_Button] = KeyCode.None;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Up_Button] = KeyCode.UpArrow;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Right_Button] = KeyCode.RightArrow;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Down_Button] = KeyCode.DownArrow;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Left_Button] = KeyCode.LeftArrow;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.LT] = KeyCode.None;
			RedRoomTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.RT] = KeyCode.None;
		}

		private void InitializeInputsMap()
		{
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.None] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.A_Button] = KeyCode.Escape;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.B_Button] = KeyCode.Mouse0;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.X_Button] = KeyCode.None;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.Y_Button] = KeyCode.Tab;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.LB] = KeyCode.KeypadMinus;
			MapTranslateDigitalBackToInput[(int)SteamInputHook.SteamInputDigital.RB] = KeyCode.KeypadPlus;
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
			foreach (var analogInput in AnalogInputToInput!)
			{
				analogInput.Value.Update();
			}
		}
	}
}
