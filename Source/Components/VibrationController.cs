using BepInEx.Unity.IL2CPP.Utils;
using Steamworks;
using System;
using UnityEngine;

namespace SuisHack.Components
{
	public class VibrationController : MonoBehaviour
	{
		private InputHandle_t ControllerHandle;
		public VibrationController(IntPtr ptr) : base(ptr) { }

		public static VibrationController Instance { get; private set; }
		private Coroutine RumbleRoutine = null;
		public static bool UseRumble = false;

		public static void Initialize()
		{
			if (Instance == null)
			{
				var go = new GameObject("VibrationController");
				GameObject.DontDestroyOnLoad(go);
				Instance = go.AddComponent<VibrationController>();
				UseRumble = ExposedSettings.Instance.Input_Controller_Vibration.Value;
			}

			if (Instance != null)
				Instance.GetHandle();
		}

		public void GetHandle()
		{
			if (UseRumble && ControllerHandle.m_InputHandle == 0)
			{
				if (ControllerHandle.m_InputHandle == 0)
				{
					var controllers = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<InputHandle_t>(16);
					var amountOfControllers = SteamInput.GetConnectedControllers(controllers);
					if (amountOfControllers > 0)
					{
						ControllerHandle = controllers[0];
					}
				}
			}
		}

		public void TriggleRumble(ushort strenght, float lenght)
		{
			if (UseRumble && ControllerHandle.m_InputHandle != 0)
			{
				if (RumbleRoutine != null)
					StopCoroutine(RumbleRoutine);

				RumbleRoutine = MonoBehaviourExtensions.StartCoroutine(this, ChargeRumble(strenght, strenght, lenght));

			}
		}

		public void TriggerChargeRumble(ushort startStrenght, ushort maxStrenght, float lenght)
		{
			if (UseRumble && ControllerHandle.m_InputHandle != 0)
			{
				if (RumbleRoutine != null)
					StopCoroutine(RumbleRoutine);

				RumbleRoutine = MonoBehaviourExtensions.StartCoroutine(this, ChargeRumble(startStrenght, maxStrenght, lenght));
			}
		}

		internal void CancelRumble()
		{
			if (UseRumble && ControllerHandle.m_InputHandle != 0)
			{
				StopAllCoroutines();
				SteamInput.TriggerVibration(ControllerHandle, 0, 0);
			}
		}

		public void TriggleRumble(ushort lStrenght, ushort rStrenght, float lenght)
		{
			if (UseRumble && ControllerHandle.m_InputHandle != 0)
			{
				if (RumbleRoutine != null)
				{
					StopCoroutine(RumbleRoutine);
				}
				RumbleRoutine = MonoBehaviourExtensions.StartCoroutine(this, FireRumble(lStrenght, rStrenght, lenght));
			}
		}

		private System.Collections.IEnumerator ChargeRumble(ushort startRumble, ushort maxRumble, float lenght)
		{
			float t = 0;
			while (t < lenght)
			{
				t += Time.deltaTime;
				var value = (ushort)Mathf.Lerp(startRumble, maxRumble, t);
				SteamInput.TriggerVibration(ControllerHandle, value, value);
				yield return null;
			}
			SteamInput.TriggerVibration(ControllerHandle, maxRumble, maxRumble);
		}

		private System.Collections.IEnumerator FireRumble(ushort lStrenght, ushort rStrenght, float lenght)
		{
			SteamInput.TriggerVibration(ControllerHandle, lStrenght, rStrenght);
			yield return new WaitForSecondsRealtime(lenght);
			SteamInput.TriggerVibration(ControllerHandle, 0, 0);
		}
	}
}
