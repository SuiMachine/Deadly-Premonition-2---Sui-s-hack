using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuisHack.Components.Interpolation
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

		public class RigidBodiesStoredVelocities
		{
			public Rigidbody rigidBody;
			public Vector3 velocity;
			private bool WasKinematic;

			public void StoreVelocity()
			{
				if (rigidBody != null)
				{
					this.velocity = rigidBody.velocity;

					//A stupid hack to prevent a player floating after interpolation :(
					if (velocity.y == 0 && rigidBody.velocity.y == 0)
						velocity += Physics.gravity * Time.deltaTime;

					this.WasKinematic = rigidBody.isKinematic;
					this.rigidBody.isKinematic = true;
				}
			}

			public void RestoreVelocity()
			{
				if (rigidBody != null)
				{
					this.rigidBody.isKinematic = WasKinematic;
					this.rigidBody.velocity = velocity;
				}
			}

			public RigidBodiesStoredVelocities(Rigidbody riggedBodyRef)
			{
				this.rigidBody = riggedBodyRef;
				this.velocity = this.rigidBody.velocity;
				this.WasKinematic = this.rigidBody.isKinematic;
			}
		}

		public RigidBodiesStoredVelocities[] riggidBodiesStored;

		PositionRecording[] records = new PositionRecording[2];
		Vector3 restorePosition;
		Quaternion restoreRotation;
		Vector3 restoreLocalScale;

		void OnEnable()
		{
			if (!ActiveObjects.Contains(this))
				ActiveObjects.Add(this);
			restorePosition = Vector3.zero;

			var riggedBodies = this.GetComponentsInChildren<Rigidbody>(true);
			this.riggidBodiesStored = new RigidBodiesStoredVelocities[riggedBodies.Length];
			for (int i = 0; i < riggedBodies.Length; i++)
			{
				riggidBodiesStored[i] = new RigidBodiesStoredVelocities(riggedBodies[i]);
			}

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
				foreach (var riggedBody in riggidBodiesStored)
					riggedBody.StoreVelocity();

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

			foreach (var riggedBody in riggidBodiesStored)
				riggedBody.RestoreVelocity();
		}
	}
}
