using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using static MelonLoader.MelonLogger;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class GlobalGeometryChecker : MonoBehaviour
	{
		const string OpenWorldPrefix = "OpenWorld_";
		const string Terrain = "Terrain_Mesh";

		static HashSet<string> yeets = new HashSet<string>();


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
							if (yeets.Contains(obj.name))
							{
								yield return null;
								continue;
							}

							var components = obj.GetComponentsInChildren<FloorTypeObj>();
							yeets.Add(obj.name);
							yield return null;

							for (int j = 0; j < components.Length; j++)
							{
								var component = components[j];
								if (component != null)
								{
									SuisHackMain.loggerInst.Msg($"Wrote down {component.name}");
									Directory.CreateDirectory("Squares");

									var path = Path.Combine("Squares", component.name + ".xml");
									if (File.Exists(path))
										continue;

									var mf = component.GetComponent<MeshFilter>();
									if (mf == null)
									{
										SuisHackMain.loggerInst.Msg($"Mesh Filter was null in {component.name}");
										continue;
									}

									if (mf.sharedMesh == null)
									{
										SuisHackMain.loggerInst.Msg($"Mesh was null in {component.name}");
										continue;
									}

									var verts = mf.sharedMesh.vertices;
									var triangles = mf.sharedMesh.triangles;
									var uvs = mf.sharedMesh.uv;

									var store = new Store();
									store.verts = new Vector3[verts.Length];
									for (int l = 0; l < verts.Length; l++)
									{
										store.verts[l] = verts[l];
									}

									store.triangles = new int[triangles.Length];
									for (int l = 0; l < triangles.Length; l++)
									{
										store.triangles[l] = triangles[l];
									}

									store.uvs = new Vector2[uvs.Length];
									for (int l = 0; l < uvs.Length; l++)
									{
										store.uvs[l] = uvs[l];
									}


									var fileWritter = new StreamWriter(path);
									XmlSerializer serializer = new XmlSerializer(typeof(Store));
									serializer.Serialize(fileWritter, store);
									fileWritter.Close();
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

	[Serializable]
	public struct Store
	{
		public Vector3[] verts;
		public int[] triangles;
		public Vector2[] uvs;
	}
}
