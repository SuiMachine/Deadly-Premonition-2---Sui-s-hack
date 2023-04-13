using Il2CppSystem.IO;
using MelonLoader;
using System;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class TerrainCorrectionData : MonoBehaviour
	{
		public TerrainCorrectionData(IntPtr ptr) : base(ptr) { }

		void Start()
		{
			Import();
		}

		public void Import()
		{
			var path = Path.Combine(Path.Combine(Application.streamingAssetsPath, "terrainfixes"), this.gameObject.name + ".bin");

			var mf = this.GetComponent<MeshFilter>();

			if (File.Exists(path))
			{
				FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
				var binaryReader = new BinaryReader(fs);

				try
				{
					var verticies = mf.sharedMesh.vertices;

					int i = 0;
					while (fs.Position < fs.Length)
					{
						var x = binaryReader.ReadSingle();
						var y = binaryReader.ReadSingle();
						var z = binaryReader.ReadSingle();
						verticies[i] = new Vector3(x, y, z);
						i++;
					}
					mf.sharedMesh.vertices = verticies;


					var collider = this.GetComponent<MeshCollider>();
					if(collider != null)
					{
						collider.sharedMesh = mf.sharedMesh;
					}
				}
				catch(Exception e)
				{
					SuisHackMain.loggerInst.Error($"Error when loading {path}: {e}");
				}
				finally
				{
					binaryReader.Close();
					fs.Close();
				}
			}


			if(SuisHackMain.Settings.Entry_Other_GeometryImprovements.Value == ExposedSettings.GeometryImprovements.ExtraGeometry && GrassRenderController.Instance != null)
			{
				var mr = this.GetComponent<MeshRenderer>();

				var instancing = this.gameObject.AddComponent<GrassGPUGeneration>();
				instancing.SetComponents(mf, mr);
			}
		}
	}
}
