using MelonLoader;
using System;
using System.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class GPU_Terrain_Rendering_Instance : MonoBehaviour
	{
		public MeshFilter MeshFilterRef;
		public MeshRenderer MeshRendererRef;

		public static Mesh instancedMesh;
		public static Material instancedMaterial;

		private const int sideSize = 255;
		private const int initialInstanceCount = sideSize * sideSize;
		public static readonly int ShaderHash_InternalSquareOffset = Shader.PropertyToID("_InternalSquareOffset");
		public static readonly int ShaderHash_ExternalOffset = Shader.PropertyToID("_ExternalOffset");
		public static readonly int ShaderHash_PositionsData = Shader.PropertyToID("_PositionsData");
		public static readonly int ShaderHash_VertexData = Shader.PropertyToID("_VertexData");

		public static readonly int ShaderHash_VertexCount = Shader.PropertyToID("_VertsCount");
		public static readonly int ShaderHash_Triangles = Shader.PropertyToID("_Triangles");
		public static readonly int ShaderHash_TriangleCount = Shader.PropertyToID("_TriangleCount");
		private Vector3 offsetResult;

		private ComputeShader ComputeShaderGeneratePositions;
		private ComputeBuffer positionsBuffer;
		private ComputeBuffer vertexBuffer;
		private ComputeBuffer trianglesBuffer;
		private ComputeBuffer argsBuffer;
		private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
		private MaterialPropertyBlock mpb;

		public GPU_Terrain_Rendering_Instance(IntPtr ptr) : base(ptr) { }

		public void SetComponents(MeshFilter mf, MeshRenderer mr)
		{
			this.MeshFilterRef = mf;
			this.MeshRendererRef = mr;
		}

		void Awake()
		{
			if(instancedMesh == null)
			{
				var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				var mf = go.GetComponent<MeshFilter>();
				instancedMesh = mf.sharedMesh;
				var mr = go.GetComponent<MeshRenderer>();
				instancedMaterial = mr.sharedMaterial;
				Destroy(go);
			}
		}

		void Start()
		{
			argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments, 0);
			GPU_Instances_Controller.Instance.Register(this);
		}

		public void GenerateData()
		{
			//TODO: Implement rotation against hit
			offsetResult = GetInternalOffset();
			if (positionsBuffer != null)
				positionsBuffer.Release();
			if (vertexBuffer != null)
				vertexBuffer.Release();

			int mainKernel = ComputeShaderGeneratePositions.FindKernel("CSGenerate");
			ComputeShaderGeneratePositions.GetKernelThreadGroupSizes(mainKernel, out uint x, out uint y, out uint _);
			ComputeShaderGeneratePositions.SetVector(ShaderHash_InternalSquareOffset, offsetResult);
			ComputeShaderGeneratePositions.SetVector(ShaderHash_ExternalOffset, this.transform.position);


			var array = Il2CppSystem.Array.CreateInstance(Vector4.Il2CppType, MeshFilterRef.sharedMesh.vertices.Length);
			{
				for(int i=0; i< MeshFilterRef.sharedMesh.vertices.Length; i++)
				{
					var vec = MeshFilterRef.sharedMesh.vertices[i];
					array.SetValue(new Vector4(vec.x, vec.y, vec.z, 0), i);
				}
				foreach(var element in MeshFilterRef.sharedMesh.vertices)
				{

				}
				var verts3Temp = MeshFilterRef.sharedMesh.vertices.Select(yeet => (Vector4)yeet);
			}

			var triangles = MeshFilterRef.sharedMesh.triangles;

			vertexBuffer = new ComputeBuffer(verts3.Length, sizeof(float) * 4);
			vertexBuffer.SetData(verts3);
			trianglesBuffer = new ComputeBuffer(triangles.Length, sizeof(int));
			trianglesBuffer.SetData(triangles);

			positionsBuffer = new ComputeBuffer(initialInstanceCount, sizeof(float) * 4);
			ComputeShaderGeneratePositions.SetBuffer(mainKernel, ShaderHash_VertexData, vertexBuffer);
			ComputeShaderGeneratePositions.SetInt(ShaderHash_VertexCount, verts3.Length);
			ComputeShaderGeneratePositions.SetBuffer(mainKernel, ShaderHash_Triangles, trianglesBuffer);
			ComputeShaderGeneratePositions.SetInt(ShaderHash_TriangleCount, triangles.Length);

			ComputeShaderGeneratePositions.SetBuffer(mainKernel, ShaderHash_PositionsData, positionsBuffer);
			ComputeShaderGeneratePositions.Dispatch(mainKernel, Mathf.CeilToInt(sideSize / x), Mathf.CeilToInt(sideSize / y), 1);
			//instancedMaterial.SetBuffer(ShaderHash_PositionsData, positionsBuffer);

			if (instancedMesh != null)
			{
				args[0] = (uint)instancedMesh.GetIndexCount(0);
				args[1] = (uint)positionsBuffer.count;
				args[2] = (uint)instancedMesh.GetIndexStart(0);
				args[3] = (uint)instancedMesh.GetBaseVertex(0);
			}
			argsBuffer.SetData(args);

			mpb = new MaterialPropertyBlock();
			mpb.SetBuffer(ShaderHash_PositionsData, positionsBuffer);
		}

		private Vector3 GetInternalOffset()
		{
			var split = this.name.Split('_');
			var offsetXStr = float.Parse(split[split.Length - 2].Substring(1));
			var offsetYStr = float.Parse(split[split.Length - 1].Substring(1));

			return new Vector3(256 * offsetXStr, 0, 256 * offsetYStr);
		}

		public void RenderMeshes(ComputeShader computeShaderGenerate)
		{
			ComputeShaderGeneratePositions = Instantiate(computeShaderGenerate);
			GPUCull();
		}

		private void GPUCull()
		{
			//TODO: Cull elements on GPU
			Graphics.DrawMeshInstancedIndirect(instancedMesh, 0, instancedMaterial, new Bounds(Vector3.zero, Vector3.one * 9999), argsBuffer, 0, mpb);
		}

		void OnDestroy()
		{
			if (GPU_Instances_Controller.Instance != null)
				GPU_Instances_Controller.Instance.Unregister(this);
		}
	}
}
