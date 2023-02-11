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

		private Queue<GPU_Terrain_Rendering_Instance> TerrainsToProcess = new Queue<GPU_Terrain_Rendering_Instance>();
		private HashSet<GPU_Terrain_Rendering_Instance> TerrainsToRender = new HashSet<GPU_Terrain_Rendering_Instance>();

		void Awake()
		{
			
		}

		void Update()
		{
			//Only generate a single square per frame
			if (TerrainsToProcess.Count > 0)
			{
				var tileToProcess = TerrainsToProcess.Dequeue();
				if (tileToProcess != null)
				{
					tileToProcess.GenerateData();
					TerrainsToRender.Add(tileToProcess);
				}
			}

			if (ActiveCamera == null)
				return;

		}

		public void Register(GPU_Terrain_Rendering_Instance gPU_Terrain_Rendering_Instance)
		{
			if(!TerrainsToRender.Contains(gPU_Terrain_Rendering_Instance))
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
