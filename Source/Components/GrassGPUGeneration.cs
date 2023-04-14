using Il2CppSystem.IO;
using MelonLoader;
using System;
using System.Linq;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class GrassGPUGeneration : MonoBehaviour
	{
		public MeshFilter MeshFilterRef;
		public MeshRenderer squareMeshRenderer;

		public static Mesh instancedMesh;
		public static Material instancedMaterial;
		public Bounds TerrainBoundsExtended { get; private set; }

		private Vector3 offsetResult;
		public ComputeBuffer PositionsBuffer { get; private set; }
		public ComputeBuffer PositionsBufferWorldToObject { get; private set; }

		public GrassGPUGeneration(IntPtr ptr) : base(ptr) { }

		public void SetComponents(MeshFilter mf, MeshRenderer mr)
		{
			this.MeshFilterRef = mf;
			this.squareMeshRenderer = mr;
		}

		private void Start()
		{
			if (GrassRenderController.Instance != null)
			{
				GrassRenderController.Instance.Register(this);
			}
		}

		public void GenerateData(Vector4 rotation)
		{
			SuisHackMain.loggerInst.Msg("Starting generation");
			offsetResult = GetInternalOffset();

			int mainKernel = GrassRenderController.ComputeShaderGeneratePositions.FindKernel("CSGenerate");
			GrassRenderController.ComputeShaderGeneratePositions.GetKernelThreadGroupSizes(mainKernel, out uint x, out uint y, out uint _);
			GrassRenderController. ComputeShaderGeneratePositions.SetVector(GrassShaderHashes.InternalSquareOffset, offsetResult);
			GrassRenderController.ComputeShaderGeneratePositions.SetVector(GrassShaderHashes.ExternalOffset, this.transform.position);

			var verts3 = MeshFilterRef.sharedMesh.vertices.Select(yeet => (Vector4)yeet).ToArray();

			PositionsBuffer = new ComputeBuffer(GrassRenderController.INITIALINSTANCECOUNT, sizeof(float) * 4 * 4);
			PositionsBufferWorldToObject = new ComputeBuffer(GrassRenderController.INITIALINSTANCECOUNT, sizeof(float) * 4 * 4);
			GrassRenderController.ComputeShaderGeneratePositions.SetVectorArray(GrassShaderHashes.VertexData, verts3);
			GrassRenderController.ComputeShaderGeneratePositions.SetInt(GrassShaderHashes.VertexCount, verts3.Length);
			GrassRenderController.ComputeShaderGeneratePositions.SetVector(GrassShaderHashes.QuaternionRotation, rotation);

			GrassRenderController.ComputeShaderGeneratePositions.SetBuffer(mainKernel, GrassShaderHashes.PositionsData, PositionsBuffer);
			GrassRenderController.ComputeShaderGeneratePositions.SetBuffer(mainKernel, GrassShaderHashes.PositionsDataWorldToObject, PositionsBufferWorldToObject);
			GrassRenderController.ComputeShaderGeneratePositions.Dispatch(mainKernel, Mathf.CeilToInt(GrassRenderController.SIDESIZE / x), Mathf.CeilToInt(GrassRenderController.SIDESIZE / y), 1);

			TerrainBoundsExtended = this.squareMeshRenderer.bounds;
			SuisHackMain.loggerInst.Msg("Done generation");
		}

		private Vector3 GetInternalOffset()
		{
			var split = this.name.Split('_');
			var offsetXStr = float.Parse(split[split.Length - 2].Substring(1));
			var offsetYStr = float.Parse(split[split.Length - 1].Substring(1));

			return new Vector3(256 * offsetXStr, 0, 256 * offsetYStr);
		}

		private void OnDestroy()
		{
			if (GrassRenderController.Instance != null)
			{
				GrassRenderController.Instance.Unregister(this);
			}

			if (PositionsBuffer != null)
				PositionsBuffer.Release();
			PositionsBuffer = null;

			if (PositionsBufferWorldToObject != null)
				PositionsBufferWorldToObject.Release();
			PositionsBufferWorldToObject = null;
		}
	}
}
