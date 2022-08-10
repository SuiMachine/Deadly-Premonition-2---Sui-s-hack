using Il2CppSystem.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SuisHack.Components
{
	public class TerrainCorrectionData : MonoBehaviour
	{
		void Start()
		{
			Import();
		}

		public void Import()
		{
			var path = Path.Combine(Path.Combine(Application.streamingAssetsPath, "TerrainFixes"), this.gameObject.name + ".bin");
			if(File.Exists(path))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

				SuisHackMain.loggerInst.Msg($"Trying to load corrected verts for: {this.gameObject.name}");
				try
				{
					var verts = (List<Vector3>)formatter.Deserialize(fs);
					var mf = this.GetComponent<MeshFilter>();
					mf.sharedMesh.SetVertices(verts);
				}
				catch(Exception e)
				{
					SuisHackMain.loggerInst.Error($"Error when loading {path}: {e}");
				}
				finally
				{
					fs.Close();
				}
			}
		}
	}
}
