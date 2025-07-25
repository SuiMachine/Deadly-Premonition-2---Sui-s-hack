﻿using Steamworks;
using UnityEngine;

namespace SuisHack.KeyboardSupport
{
	public abstract class KeySteamAnalogAction
	{
		public abstract void Update();

		public abstract InputAnalogActionData_t GetInput();

		public abstract KeyCode GetUpKeyCode();
		public abstract KeyCode GetDownKeyCode();
		public abstract KeyCode GetLeftKeyCode();
		public abstract KeyCode GetRightKeyCode();

	}

	public class MouseAnalog : KeySteamAnalogAction
	{
		public static float Sensitivity;
		public static bool InvertYAxis;

		public override InputAnalogActionData_t GetInput()
		{
			Cursor.lockState = CursorLockMode.Locked;
			var vec = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Sensitivity;

			return new InputAnalogActionData_t()
			{
				bActive = 1,
				eMode = EInputSourceMode.k_EInputSourceMode_None,
				x = vec.x, y = vec.y 
			};
		}

		public override KeyCode GetLeftKeyCode() => KeyCode.None;

		public override KeyCode GetRightKeyCode() => KeyCode.None;

		public override KeyCode GetUpKeyCode() => KeyCode.None;
		public override KeyCode GetDownKeyCode() => KeyCode.None;


		public override void Update()
		{
		}
	}

	public class KeyActionAnalog : KeySteamAnalogAction
	{
		KeyCode keyCodeUp = KeyCode.None;
		KeyCode keyCodeDown = KeyCode.None;
		KeyCode keyCodeLeft = KeyCode.None;
		KeyCode keyCodeRight = KeyCode.None;
		readonly float maxDelta = 1;
		Vector2 analogVector;

		public KeyActionAnalog(KeyCode Up, KeyCode Down, KeyCode Left, KeyCode Right, float maxDelta)
		{
			keyCodeUp = Up;
			keyCodeDown = Down;
			keyCodeLeft = Left;
			keyCodeRight = Right;
			if (maxDelta <= 0)
				maxDelta = 0.01f;
			this.maxDelta = 1 / maxDelta;
		}

		public override InputAnalogActionData_t GetInput()
		{
			var inputValUp = Input.GetKey(keyCodeUp) ? 1.0f : 0.0f;
			var inputValDown = Input.GetKey(keyCodeDown) ? -1.0f : 0.0f;
			var inputValLeft = Input.GetKey(keyCodeLeft) ? -1.0f : 0.0f;
			var inputValRight = Input.GetKey(keyCodeRight) ? 1.0f : 0.0f;

			float newLeftRight = inputValLeft + inputValRight;
			float newUpDown = inputValUp + inputValDown;

			if (GameStateMachine.CurrentGameState == GameStateMachine.Gamestate.StandardGameplay)
			{
				//To make it a bit less jittery
				analogVector = new Vector2(
					Mathf.MoveTowards(analogVector.x, newLeftRight, Time.unscaledDeltaTime * maxDelta),
					Mathf.MoveTowards(analogVector.y, newUpDown, Time.unscaledDeltaTime * maxDelta));

				if (analogVector.sqrMagnitude > 1)
					analogVector.Normalize();

				return new InputAnalogActionData_t() { x = analogVector.x, y = analogVector.y };
			}
			else
			{
				analogVector = new Vector2(newLeftRight, newUpDown);
				if(analogVector.sqrMagnitude > 1)
					analogVector.Normalize();

				return new InputAnalogActionData_t() { x = analogVector.x, y = analogVector.y };
			}
		}

		public override void Update()
		{
		}

		public override KeyCode GetUpKeyCode() => keyCodeUp;
		public override KeyCode GetDownKeyCode() => keyCodeDown;
		public override KeyCode GetLeftKeyCode() => keyCodeLeft;
		public override KeyCode GetRightKeyCode() => keyCodeRight;
	}
}
