using MelonLoader;
using System;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class SmootherController : MonoBehaviour
	{
		public SmootherController(IntPtr ptr) : base(ptr) { }

		public static bool InterpolateMovement;
		public GameObject visionCameraReference;

		public void OnEnable()
		{
			var visCam = this.transform.Find("VisionCamera");
			if (visCam != null)
			{
				visionCameraReference = visCam.gameObject;
			}
		}

		public void OnPreRender()
		{
			if (!InterpolateMovement)
				return;

			if (visionCameraReference != null && visionCameraReference.activeInHierarchy)
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

			if (visionCameraReference != null && visionCameraReference.activeInHierarchy)
				return;

			foreach (var obj in GameObjectInterpolation.ActiveObjects)
			{
				obj.RestoreOriginalPosition();
			}
		}
	}
}
