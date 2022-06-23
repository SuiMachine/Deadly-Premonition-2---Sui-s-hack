using Harmony;
using Steamworks;
using System;
using System.Collections.Generic;
using UnhollowerBaseLib;
using UnityEngine;

namespace SuisHack.GlobalGameObjects
{
	[MelonLoader.RegisterTypeInIl2Cpp]
	class GlobalInputHookHandler : MonoBehaviour
	{
		public GlobalInputHookHandler(IntPtr ptr) : base(ptr) { }

		public class KeyActionAnalog
		{
			private bool isMouseAxis;
			private KeyCode keyCodeUp = KeyCode.None;
			private KeyCode keyCodeDown = KeyCode.None;
			private KeyCode keyCodeLeft = KeyCode.None;
			private KeyCode keyCodeRight = KeyCode.None;

			public KeyActionAnalog()
			{
				isMouseAxis = true;
				keyCodeUp = KeyCode.None;
				keyCodeDown = KeyCode.None;
				keyCodeLeft = KeyCode.None;
				keyCodeRight = KeyCode.None;
			}

			public KeyActionAnalog(KeyCode Up, KeyCode Down, KeyCode Left, KeyCode Right)
			{
				isMouseAxis = false;
				keyCodeUp = Up;
				keyCodeDown = Down;
				keyCodeLeft = Left;
				keyCodeRight = Right;
			}

			internal InputAnalogActionData_t GetInput()
			{
				if(!isMouseAxis)
				{
					var inputValUp = Input.GetKey(keyCodeUp) ? 1.0f : 0.0f;
					var inputValDown = Input.GetKey(keyCodeDown) ? -1.0f : 0.0f;
					var inputValLeft = Input.GetKey(keyCodeLeft) ? -1.0f : 0.0f;
					var inputValRight = Input.GetKey(keyCodeRight) ? 1.0f : 0.0f;

					var vector = new Vector2(inputValLeft + inputValRight, inputValUp + inputValDown);
					if (vector.magnitude > 1)
						vector.Normalize();

					return new InputAnalogActionData_t() { x = vector.x, y = vector.y };
				}
				else
				{
					Cursor.lockState = CursorLockMode.Confined;
					var vec = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * 0.2f;


					return new InputAnalogActionData_t() { x = vec.x, y = vec.y };
				}

			}
		}

		public static GlobalInputHookHandler Instance { get; private set; }
		public static Dictionary<string, KeyActionAnalog> AnalogInputToInput = new Dictionary<string, KeyActionAnalog>()
		{
			{"L_Stick", new KeyActionAnalog(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D) },
			{"R_Stick", new KeyActionAnalog() }
		};

		internal KeyActionAnalog GetAnalogInputReplacement(InputAnalogActionHandle_t analogActionHandle)
		{
			if(TranslateAnalogBackToInput.TryGetValue(analogActionHandle, out KeyActionAnalog value))
			{
				return value;
			}
			return null;				
		}

		public static Dictionary<InputAnalogActionHandle_t, KeyActionAnalog> TranslateAnalogBackToInput = new Dictionary<InputAnalogActionHandle_t, KeyActionAnalog>();

		public static void Initialize()
		{
			if(Instance == null)
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
			foreach(var analogInput in AnalogInputToInput)
			{
				var handle = SteamInput.GetAnalogActionHandle(analogInput.Key);
				TranslateAnalogBackToInput.Add(handle, analogInput.Value);
			}			
		}
	}
}
