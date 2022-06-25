using Steamworks;
using System;
using UnityEngine;

namespace SuisHack.InputHandling
{
	public class KeySteamDigitalAction
	{
		bool oneTap;
		KeyCode key;

		public KeySteamDigitalAction(KeyCode key, bool oneTap)
		{
			this.key = key;
			this.oneTap = oneTap;
		}

		public InputDigitalActionData_t GetInput()
		{
			if(oneTap)
				return new InputDigitalActionData_t() { bActive = 1, bState = (byte)(Input.GetKeyDown(key) ? 1 : 0) };
			else
				return new InputDigitalActionData_t() { bActive = 1, bState = (byte)(Input.GetKey(key) ? 1 : 0) };
		}

		internal KeyCode GetBind()
		{
			return key;
		}
	}
}
