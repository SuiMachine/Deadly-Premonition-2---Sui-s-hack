using MelonLoader;
using System;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class WireRendererCorrection : MonoBehaviour
	{
		public WireRendererCorrection(IntPtr ptr) : base(ptr) { }

		public void OnEnable()
		{
			var skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>(true);
			for(int i=0; i< skinnedMeshes.Length; i++)
			{
				var newBound = new Bounds();

				var skinnedMesh = skinnedMeshes[i].transform;
				var objectsToEnapsulateParent = skinnedMesh.transform.parent;

				for (int j=0; j< objectsToEnapsulateParent.childCount; j++)
				{
					var childToEncapsulate = objectsToEnapsulateParent.GetChild(j);
					if (childToEncapsulate == skinnedMesh.transform)
						continue;

					var point = skinnedMesh.transform.InverseTransformPoint(childToEncapsulate.transform.position);
					newBound.Encapsulate(point);
				}

				skinnedMeshes[i].localBounds = newBound;
			}
		}
	}
}
