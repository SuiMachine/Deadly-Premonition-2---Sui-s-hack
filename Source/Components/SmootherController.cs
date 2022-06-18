using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class SmootherController : MonoBehaviour
	{
		public SmootherController(IntPtr ptr) : base(ptr) { }

		private object postRenderRoutine;
		public GameObject visionCameraReference;

		public void OnEnable()
		{
			var visCam = this.transform.Find("VisionCamera");
			if(visCam != null)
			{
				visionCameraReference = visCam.gameObject;
			}
		}

		public void OnPreCull()
		{
			if (postRenderRoutine == null)
			{
				postRenderRoutine = MelonCoroutines.Start(PostRender());
			}

			if(visionCameraReference != null && visionCameraReference.activeInHierarchy)
			{
				return;
			}

			for (int i=0; i<GameObjectInterpolation.ActiveObjects.Count; i++)
			{
				var obj = GameObjectInterpolation.ActiveObjects[i];
				if(obj == null)
				{
					GameObjectInterpolation.ActiveObjects.RemoveAt(i);
					i--;
					continue;
				}
				obj.Interpolate();
			}
		}

		private System.Collections.IEnumerator PostRender()
		{
			while(true)
			{
				while(visionCameraReference.activeInHierarchy)
				{
					yield return new WaitForEndOfFrame();
				}

				yield return new WaitForEndOfFrame();
				foreach(var obj in GameObjectInterpolation.ActiveObjects)
				{
					obj.RestoreOriginalPosition();
				}
			}
		}

		public void OnDisable()
		{
			MelonCoroutines.Stop(postRenderRoutine);
			postRenderRoutine = null;
		}
	}
}
