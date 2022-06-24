using Steamworks;
using UnityEngine;

namespace SuisHack.InputHandling
{
	public abstract class KeySteamAnalogAction
	{
		public abstract void Update();

		public abstract InputAnalogActionData_t GetInput();
	}

	public class MouseAnalog : KeySteamAnalogAction
	{
		public override InputAnalogActionData_t GetInput()
		{
			Cursor.lockState = CursorLockMode.Confined;
			var vec = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * 0.5f;

			return new InputAnalogActionData_t() { x = vec.x, y = vec.y };
		}

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

			var newLeftRight = inputValLeft + inputValRight;
			var newUpDown = inputValUp + inputValDown;

			analogVector = new Vector2(
				Mathf.MoveTowards(analogVector.x, newLeftRight, Time.unscaledDeltaTime * maxDelta),
				Mathf.MoveTowards(analogVector.y, newUpDown, Time.unscaledDeltaTime * maxDelta));

			if (analogVector.magnitude > 1)
				analogVector.Normalize();

			return new InputAnalogActionData_t() { x = analogVector.x, y = analogVector.y };
		}

		public override void Update()
		{
		}
	}
}
