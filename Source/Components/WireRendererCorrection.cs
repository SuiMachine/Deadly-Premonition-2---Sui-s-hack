using MelonLoader;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class WireRendererCorrection : MonoBehaviour
	{
		public WireRendererCorrection(IntPtr ptr): base(ptr) { }

		public static Dictionary<string, List<PoleDefinitionProtoplast>> DisableUtterlyBroken = new Dictionary<string, List<PoleDefinitionProtoplast>>()
		{
/*			{
				"OpenWorld_J_09_Location_ElectricPole(Clone)", new List<PoleDefinitionProtoplast>()
				{
					new PoleDefinitionDisable("Pole Standard (9)", Vector3.zero),
				}
			},*/
			{
				"OpenWorld_L_07_Location_ElectricPole(Clone)", new List<PoleDefinitionProtoplast>()
				{
					new PoleDefinitionDisableRootInside("Pole Standard (5)", Vector3.zero, "Root", 0)
				}
			}
		};

		public void OnEnable()
		{
			var skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>(true);
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
							pole.Process(this.transform, skinnedMeshes);
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
		public abstract void Process(Transform transform, SkinnedMeshRenderer[] skinnedMeshes);
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

		public override void Process(Transform transform, SkinnedMeshRenderer[] skinnedMeshes)
		{
			SuisHackMain.loggerInst!.Error("Implement that!");

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

		public override void Process(Transform transform, SkinnedMeshRenderer[] skinnedMeshes)
		{
			SuisHackMain.loggerInst!.Msg("Implement that!");
		}
	}

	public class PoleDefinitionDisableRootInside : PoleDefinitionProtoplast
	{
		public Vector3 LocalEulerRotation { get; private set; }
		public string RootName { get; private set; }
		public int RootID { get; private set; }

		public PoleDefinitionDisableRootInside(string Name, Vector3 LocalEulerRotation, string RootName, int RootID) : base(Name)
		{
			this.LocalEulerRotation = LocalEulerRotation;
			this.RootName = RootName;
			this.RootID = RootID;
		}

		protected override bool IsValidCompare(Transform transform)
		{
			return this.LocalEulerRotation == LocalEulerRotation && this.Name == transform.name;
		}

		public override void Process(Transform transform, SkinnedMeshRenderer[] skinnedMeshes)
		{
			int j = 0;
			for (int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);
				if (child.name == RootName)
				{
					if (j == RootID)
					{
						child.gameObject.SetActive(false);
#if DEBUG
						SuisHackMain.loggerInst!.Msg($"[WireCorrection] Disabled broken child root at \"{child.transform.position.x}, {child.transform.position.y}, {child.transform.position.z}\"");
#endif
						break;
					}
					j++;
				}
			}
		}
	}
}
