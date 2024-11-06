using System;
using UnityEngine;

namespace SuisHack.Components.Interpolation
{
	public class SmootherController : MonoBehaviour
	{
		public SmootherController(IntPtr ptr) : base(ptr) { }


		public static bool InterpolateMovement;
		public GameObject visionCameraReference;
		private bool DontInterpolate => VisionModeActive || CutscenePlaying;

		private bool VisionModeActive => visionCameraReference != null && visionCameraReference.activeInHierarchy;
		private bool CutscenePlaying => CutScenePlayer.Instance != null && CutScenePlayer.Instance.isActiveAndEnabled;

		public void OnEnable()
		{
			var visCam = this.transform.Find("VisionCamera");
			if (visCam != null)
			{
				visionCameraReference = visCam.gameObject;
			}
		}

		public void OnPreCull()
		{
			if (!InterpolateMovement)
				return;

			if (DontInterpolate)
				return;

			for (int i = 0; i < GameObjectInterpolation.ActiveObjects.Count; i++)
			{
				var obj = GameObjectInterpolation.ActiveObjects[i];
				if (obj == null)
				{
					GameObjectInterpolation.ActiveObjects.RemoveAt(i);
					i--;
					continue;
				}
				obj.Interpolate();
			}
		}

		private void OnPostRender()
		{
			if (!InterpolateMovement)
				return;

			if (DontInterpolate)
				return;

			foreach (var obj in GameObjectInterpolation.ActiveObjects)
			{
				obj.RestoreOriginalPosition();
			}
		}
	}
}
