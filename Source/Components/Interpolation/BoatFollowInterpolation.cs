using MelonLoader;
using System;
using UnityEngine;

namespace SuisHack.Components.Interpolation
{
	[RegisterTypeInIl2Cpp]
	class BoatFollowInterpolation : MonoBehaviour
	{
		public BoatFollowInterpolation(IntPtr ptr) : base(ptr) { }

		BoatCameraFollow boatFollowReference;
		GameObjectInterpolation interpolator;

		void Start()
		{
			boatFollowReference = this.GetComponent<BoatCameraFollow>();
			interpolator = this.GetComponent<GameObjectInterpolation>();
			if (interpolator != null)
				interpolator.enabled = false;
			else
				Destroy(this);
		}

		void Update()
		{
			if (boatFollowReference == null)
			{
				interpolator.enabled = true;
				Destroy(this);
			}
		}
	}
}
