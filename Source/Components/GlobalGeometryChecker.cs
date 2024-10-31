using BepInEx.Unity.IL2CPP.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace SuisHack.Components
{
	public class GlobalGeometryChecker : MonoBehaviour
	{
		public GlobalGeometryChecker(IntPtr ptr) : base(ptr) { }

		const string OpenWorldPrefix = "OpenWorld_";
		const string Terrain = "Terrain_Mesh";
		private Coroutine CheckerCoroutine;

		private void OnEnable()
		{
			if (CheckerCoroutine != null)
				StopCoroutine(CheckerCoroutine);
			CheckerCoroutine = MonoBehaviourExtensions.StartCoroutine(this, Checker());

			var normalMapHash = Shader.PropertyToID("_BumpMap");
			var metalicGlossMapHash = Shader.PropertyToID("_MetallicGlossMap");
			var parallaxMapHash = Shader.PropertyToID("_ParallaxMap");
			var occlusionMapHash = Shader.PropertyToID("_OcclusionMap");

			var roadMesh = GameObject.FindObjectsOfType<MeshRenderer>().Where(x => x.name == "Road_world").FirstOrDefault();

			if (roadMesh != null)
			{
				for (int i = 0; i < roadMesh.materials.Length; i++)
				{
					var material = roadMesh.materials[i];
					if (material == null)
						continue;

					var mainTexture = material.mainTexture;
					if (mainTexture != null)
						mainTexture.filterMode = FilterMode.Trilinear;

					var normalMap = material.GetTexture(normalMapHash);
					if (normalMap != null)
						normalMap.filterMode = FilterMode.Trilinear;

					var metallicMap = material.GetTexture(metalicGlossMapHash);
					if (metallicMap != null)
						metallicMap.filterMode = FilterMode.Trilinear;

					var parallaxMap = material.GetTexture(parallaxMapHash);
					if (parallaxMap != null)
						parallaxMap.filterMode = FilterMode.Trilinear;

					var occlusionMap = material.GetTexture(occlusionMapHash);
					if (occlusionMap != null)
						occlusionMap.filterMode = FilterMode.Trilinear;
				}
			}
			else
			{
				Plugin.Error("No mesh found!");
			}
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
							if (obj == null)
								continue;

							var components = obj.GetComponentsInChildren<FloorTypeObj>();
							yield return null;

							for (int j = 0; j < components.Length; j++)
							{
								var component = components[j];
								if (component == null)
									continue;

								if (component.GetComponent<TerrainCorrectionData>() == null)
								{
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
				StopCoroutine(CheckerCoroutine);
				CheckerCoroutine = null;
			}
		}
	}
}
