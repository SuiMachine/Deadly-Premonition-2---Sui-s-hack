using MelonLoader;
using System;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class CameraFollowSmoother : MonoBehaviour
	{
		public CameraFollowSmoother(IntPtr ptr) : base(ptr) { }


		private struct PositionRecording
		{
			public bool initiated;
			public float time;
			public Vector3 position;

			public PositionRecording(Vector3 position)
			{
				initiated = true;
				time = Time.time;
				this.position = position;
			}
		}
		private float lastInterpolationFactor;

		PositionRecording[] positionRecord = new PositionRecording[2];
		Vector3 restorePosition;
		public CameraFollow cameraFollowRef;
		private object postRenderRoutine;

		public void FixedUpdate()
		{
			positionRecord[1] = positionRecord[0];
			positionRecord[0] = new PositionRecording(this.transform.position);
		}

		public void LateUpdate()
		{
			restorePosition = this.transform.localPosition;
			if(postRenderRoutine == null)
			{
				postRenderRoutine = MelonCoroutines.Start(PostRender());
			}

			if (positionRecord[0].initiated && positionRecord[1].initiated)
			{
				float newerTime = positionRecord[0].time;
				float olderTime = positionRecord[1].time;

				if(newerTime != olderTime)
				{
					var interpolationFactor = (Time.time - newerTime) / (newerTime - olderTime);
					lastInterpolationFactor = interpolationFactor;
					this.transform.position = Vector3.LerpUnclamped(positionRecord[1].position, positionRecord[0].position, interpolationFactor);
				}
			}
		}

/*		private void OnGUI()
		{
			GUILayout.BeginHorizontal(GUI.skin.box, null);
			GUILayout.BeginVertical(null);
			GUILayout.Label($"Interpolation: {lastInterpolationFactor}", null);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}*/

		private System.Collections.IEnumerator PostRender()
		{
			while(true)
			{
				yield return new WaitForEndOfFrame();
				this.transform.localPosition = restorePosition;
			}
		}

		public void OnDisable()
		{
			MelonCoroutines.Stop(postRenderRoutine);
			postRenderRoutine = null;
			restorePosition = Vector3.zero;
			for(int i=0; i<positionRecord.Length; i++)
			{
				positionRecord[i] = new PositionRecording();
			}
		}
	}
}
