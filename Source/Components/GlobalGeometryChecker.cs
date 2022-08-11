using MelonLoader;
using System;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class GlobalGeometryChecker : MonoBehaviour
	{
		const string OpenWorldPrefix = "OpenWorld_";
		const string Terrain = "Terrain_Mesh";
		public GlobalGeometryChecker(IntPtr ptr) : base(ptr) { }
		private object CheckerCoroutine;

		private void OnEnable()
		{
			if (CheckerCoroutine != null)
				MelonCoroutines.Stop(CheckerCoroutine);
			CheckerCoroutine = MelonCoroutines.Start(Checker());
		}

		private System.Collections.IEnumerator Checker()
		{
			while (true)
			{
				var rootObject = this.gameObject.scene.GetRootGameObjects();
				yield return null;

				for (int i = 0; i < rootObject.Length; i++)
				{
					var obj = rootObject[i];
					if (obj != null)
					{
						if (obj.name.StartsWith(OpenWorldPrefix))
						{
							var components = obj.GetComponentsInChildren<ElectricPoleModel>();
							yield return null;
							for (int j = 0; j < components.Length; j++)
							{
								var component = components[j];

								//since we wait after adding, this can become null
								if (component != null)
								{
									if (component.GetComponent<WireRendererCorrection>() == null)
									{
										component.gameObject.AddComponent<WireRendererCorrection>();
										yield return null;
									}
								}
							}
						}

						if (obj.name.Contains(Terrain))
						{
							yield return null;
							var components = obj.GetComponentsInChildren<FloorTypeObj>();
							yield return null;

							for (int j = 0; j < components.Length; j++)
							{
								var component = components[j];
								if (component.GetComponent<TerrainCorrectionData>() == null)
								{
									yield return null;
									component.gameObject.AddComponent<TerrainCorrectionData>();
								}
								yield return null;
							}
						}
						yield return null;
					}
					else
						yield return null;
				}
			}
		}

		private void OnDisable()
		{
			if (CheckerCoroutine != null)
			{
				MelonCoroutines.Stop(CheckerCoroutine);
				CheckerCoroutine = null;
			}
		}
	}
}
