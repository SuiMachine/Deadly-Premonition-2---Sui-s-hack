using System;
using System.IO;
using UnityEngine;

namespace SuisHack.Components
{
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
			if (File.Exists(path))
			{
				FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
				var binaryReader = new BinaryReader(fs);

				try
				{
					var mf = this.GetComponent<MeshFilter>();
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
					if (collider != null)
					{
						collider.sharedMesh = mf.sharedMesh;
					}
				}
				catch (Exception e)
				{
					Plugin.Error($"Error when loading {path}: {e}");
				}
				finally
				{
					binaryReader.Close();
					fs.Close();
				}
			}
		}
	}
}
