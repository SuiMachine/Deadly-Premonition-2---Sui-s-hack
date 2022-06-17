﻿using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class GameObjectInterpolation : MonoBehaviour
	{
		public static List<GameObjectInterpolation> ActiveObjects = new List<GameObjectInterpolation>();
		public GameObjectInterpolation(IntPtr ptr) : base(ptr) { }

		private struct PositionRecording
		{
			public bool initiated;
			public float time;
			public Vector3 position { get; }
			public Quaternion rotation { get; }
			public Vector3 localScaleScale { get; }

			public PositionRecording(Vector3 position, Quaternion rotation, Vector3 localScale)
			{
				initiated = true;
				time = Time.time;
				this.position = position;
				this.rotation = rotation;
				this.localScaleScale = localScale;
			}
		}

		PositionRecording[] records = new PositionRecording[2];
		Vector3 restorePosition;
		Quaternion restoreRotation;
		Vector3 restoreLocalScale;

		void OnEnable()
		{
			if (!ActiveObjects.Contains(this))
				ActiveObjects.Add(this);
			restorePosition = Vector3.zero;

			for (int i = 0; i < records.Length; i++)
			{
				records[i] = new PositionRecording();
			}
		}

		void OnDisable()
		{
			if (ActiveObjects.Contains(this))
				ActiveObjects.Remove(this);
		}

		void FixedUpdate()
		{
			records[1] = records[0];
			records[0] = new PositionRecording(this.transform.position, this.transform.rotation, this.transform.localScale);
		}

		public void Interpolate()
		{
			restorePosition = this.transform.localPosition;
			restoreRotation = this.transform.localRotation;
			restoreLocalScale = this.transform.localScale;

			if (records[0].initiated && records[1].initiated)
			{
				float newerTime = records[0].time;
				float olderTime = records[1].time;

				if (newerTime != olderTime)
				{
					var interpolationFactor = (Time.time - newerTime) / (newerTime - olderTime);
					this.transform.position = Vector3.LerpUnclamped(records[1].position, records[0].position, interpolationFactor);
					this.transform.rotation = Quaternion.LerpUnclamped(records[1].rotation, records[0].rotation, interpolationFactor);
					this.transform.localScale = Vector3.LerpUnclamped(records[1].localScaleScale, records[0].localScaleScale, interpolationFactor);
				}
			}
		}

		public void RestoreOriginalPosition()
		{
			this.transform.localPosition = restorePosition;
			this.transform.localRotation = restoreRotation;
			this.transform.localScale = restoreLocalScale;
		}
	}
}
