using Il2CppSystem.IO;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class GPU_Instances_Controller : MonoBehaviour
	{
		public static Camera ActiveCamera;
		public static GPU_Instances_Controller Instance { get; private set; }
		public GPU_Instances_Controller(IntPtr ptr) : base(ptr) { }

		#region ComputeShader statics and getters
		private static ComputeShader m_CopyTrinaglesComputeShader;
		private static ComputeShader m_GeneratePositionsComputeShader;
		public static ComputeShader CopyTrinaglesComputeShader
		{
			get
			{
				if (m_CopyTrinaglesComputeShader == null)
				{
					AssetBundle shadersBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "suihackshaders"));
					if (shadersBundle == null)
					{
						SuisHackMain.loggerInst.Error("No asset bundle?!");
						return null;
					}

					m_CopyTrinaglesComputeShader = shadersBundle.LoadAsset("ComputeShader_CopyTriangles").Cast<ComputeShader>();
					if (m_CopyTrinaglesComputeShader == null)
					{
						SuisHackMain.loggerInst.Error("Copy triangles compute shader is null at load asset bunlde state!");
						return null;
					}

					SuisHackMain.loggerInst.Msg("Copy triangles compute shader loaded.");
					shadersBundle.Unload(false);
					return m_CopyTrinaglesComputeShader;
				}
				else
					return m_CopyTrinaglesComputeShader;
			}
		}

		public static ComputeShader GeneratePositionsComputeShader
		{
			get
			{
				if (m_GeneratePositionsComputeShader == null)
				{
					AssetBundle shadersBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "suihackshaders"));
					if (shadersBundle == null)
					{
						SuisHackMain.loggerInst.Error("No asset bundle?!");
						return null;
					}

					m_GeneratePositionsComputeShader = shadersBundle.LoadAsset("ComputeShader_GeneratePositions").Cast<ComputeShader>();
					if (m_GeneratePositionsComputeShader == null)
					{
						SuisHackMain.loggerInst.Error("Generate positions compute shader is null at load asset bunlde state!");
						return null;
					}

					SuisHackMain.loggerInst.Msg("Loaded generate positions compute shader loaded.");
					shadersBundle.Unload(false);
					return m_GeneratePositionsComputeShader;
				}
				else
					return m_GeneratePositionsComputeShader;
			}
		}
		#endregion


		private Queue<GPU_Terrain_Rendering_Instance> TerrainsToProcess = new Queue<GPU_Terrain_Rendering_Instance>();
		private HashSet<GPU_Terrain_Rendering_Instance> TerrainsToRender = new HashSet<GPU_Terrain_Rendering_Instance>();

		public static void Initialize()
		{
			if (Instance == null)
			{
				var go = new GameObject("GPU_Instance_Controller");
				Instance = go.AddComponent<GPU_Instances_Controller>();
				DontDestroyOnLoad(go);
			}
		}

		void Awake()
		{
			Instance = this;

			MelonCoroutines.Start(SplitUpdate());
		}

		private System.Collections.IEnumerator SplitUpdate()
		{
			yield return new WaitForSeconds(1);

			while (true)
			{
				if (TerrainsToProcess.Count > 0)
				{
					SuisHackMain.loggerInst.Msg("Generate tit!");

					var tileToProcess = TerrainsToProcess.Dequeue();
					if (tileToProcess != null)
					{
						yield return tileToProcess.GenerateData();
						TerrainsToRender.Add(tileToProcess);
					}
				}

				if (ActiveCamera == null)
					yield return null;
			}
		}


		private void Update()
		{
			var cameraMain = Camera.main;
			if (cameraMain == null)
				return;

			var cameraPos = cameraMain.transform.position;

			foreach(var terrainToRender in TerrainsToRender)
			{
				var closestPoint = terrainToRender.Bounds.ClosestPoint(cameraPos);
				if (Vector3.SqrMagnitude(closestPoint - cameraPos) < 128 * 128)
					terrainToRender.RenderMeshes();
			}
		}

		void OnGUI()
		{
			GUILayout.BeginHorizontal(null);
			GUILayout.BeginVertical(GUI.skin.box, null);

			GUILayout.Label($"Queue lenght: {TerrainsToProcess.Count}", null);
			GUILayout.Label($"Terrains to render: {TerrainsToRender.Count}", null);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		public void Register(GPU_Terrain_Rendering_Instance gPU_Terrain_Rendering_Instance)
		{
			if (!TerrainsToRender.Contains(gPU_Terrain_Rendering_Instance))
			{
				TerrainsToProcess.Enqueue(gPU_Terrain_Rendering_Instance);
			}
		}

		public void Unregister(GPU_Terrain_Rendering_Instance gPU_Terrain_Rendering_Instance)
		{
			TerrainsToRender.Remove(gPU_Terrain_Rendering_Instance);
		}
	}
}
