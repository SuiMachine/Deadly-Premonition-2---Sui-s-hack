using MelonLoader;
using System;
using System.Collections.Generic;
using UnhollowerBaseLib;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class WireRendererCorrection : MonoBehaviour
	{
		public static Dictionary<string, List<PoleDefinitionProtoplast>> DisableUtterlyBroken = new Dictionary<string, List<PoleDefinitionProtoplast>>()
		{
			{
				"OpenWorld_J_09_Location_ElectricPole(Clone)", new List<PoleDefinitionProtoplast>()
				{
					new PoleDefinitionDisable("Pole Standard (9)", Vector3.zero),
				}
			}
		};

		public WireRendererCorrection(IntPtr ptr) : base(ptr) { }

		public void OnEnable()
		{
			Il2CppArrayBase<SkinnedMeshRenderer> skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>(true);
			for (int i = 0; i < skinnedMeshes.Length; i++)
			{
				var newBound = new Bounds();

				var skinnedMesh = skinnedMeshes[i].transform;
				var objectsToEnapsulateParent = skinnedMesh.transform.parent;

				for (int j = 0; j < objectsToEnapsulateParent.childCount; j++)
				{
					var childToEncapsulate = objectsToEnapsulateParent.GetChild(j);
					if (childToEncapsulate == skinnedMesh.transform)
						continue;

					var point = skinnedMesh.transform.InverseTransformPoint(childToEncapsulate.transform.position);
					newBound.Encapsulate(point);
				}

				skinnedMeshes[i].localBounds = newBound;
			}

			if (transform.parent != null)
			{
				if (DisableUtterlyBroken.TryGetValue(transform.parent.name, out var poles))
				{
					foreach (var pole in poles)
					{
						if (pole.Name == this.transform.name)
						{
							pole.Process(skinnedMeshes);
						}
					}
				}
			}
		}
	}

	public abstract class PoleDefinitionProtoplast
	{
		public string Name { get; private set; }

		public PoleDefinitionProtoplast(string Name)
		{
			this.Name = Name;
		}

		public static bool operator ==(PoleDefinitionProtoplast p, Transform transform)
		{
			return p.IsValidCompare(transform);
		}

		public static bool operator !=(PoleDefinitionProtoplast p, Transform transform)
		{
			return !p.IsValidCompare(transform);
		}

		protected abstract bool IsValidCompare(Transform transform);
		public abstract void Process(UnhollowerBaseLib.Il2CppArrayBase<SkinnedMeshRenderer> skinnedMeshes);
	}

	public class PoleDefinitionDisable : PoleDefinitionProtoplast
	{
		public Vector3 LocalEulerRotation { get; private set; }

		public PoleDefinitionDisable(string Name, Vector3 LocalEulerRotation) : base(Name)
		{
			this.LocalEulerRotation = LocalEulerRotation;
		}

		protected override bool IsValidCompare(Transform transform)
		{
			return this.LocalEulerRotation == LocalEulerRotation && this.Name == transform.name;
		}

		public override void Process(Il2CppArrayBase<SkinnedMeshRenderer> skinnedMeshes)
		{
			if(skinnedMeshes.Length > 0)
			{
				skinnedMeshes[0].transform.parent.gameObject.SetActive(false);
			}
		}
	}

	public class PoleDefinitionCorrection : PoleDefinitionProtoplast
	{
		public Vector3 LocalEulerRotation { get; private set; }

		public PoleDefinitionCorrection(string Name, Vector3 LocalEulerRotation) : base(Name)
		{
			this.LocalEulerRotation = LocalEulerRotation;
		}

		protected override bool IsValidCompare(Transform transform)
		{
			return this.LocalEulerRotation == LocalEulerRotation && this.Name == transform.name;
		}

		public override void Process(Il2CppArrayBase<SkinnedMeshRenderer> skinnedMeshes)
		{
			SuisHackMain.loggerInst.Msg("Implement that!");
		}
	}
}
