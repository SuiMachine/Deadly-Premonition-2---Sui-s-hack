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
		public MeshRenderer MeshRendererRef;

		public static Mesh instancedMesh;
		public static Material instancedMaterial;

		private Vector3 offsetResult;

		private Il2CppSystem.Array nativeTrianglesArray;
		private ComputeBuffer positionsBuffer;
		private ComputeBuffer trianglesBuffer;
		private ComputeBuffer argsBuffer;
		private MaterialPropertyBlock mpb;

		public GrassGPUGeneration(IntPtr ptr) : base(ptr) { }

		public void SetComponents(MeshFilter mf, MeshRenderer mr)
		{
			this.MeshFilterRef = mf;
			this.MeshRendererRef = mr;
		}
		/*
		void Awake()
		{
			if (instancedMesh == null)
			{
				var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				var mf = go.GetComponent<MeshFilter>();
				instancedMesh = mf.sharedMesh;
				var mr = go.GetComponent<MeshRenderer>();
				instancedMaterial = mr.sharedMaterial;
				Destroy(go);

				var bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "suihackshaders"));
				if (bundle == null)
				{
					SuisHackMain.loggerInst.Error("No shaders bundle?!");
					this.enabled = false;
					return;
				}

				var drawShitShader = bundle.LoadAsset("DrawShit");
				if (drawShitShader == null)
				{
					SuisHackMain.loggerInst.Error("No shader?");
					return;
				}

				instancedMaterial.shader = drawShitShader.Cast<Shader>();
				bundle.Unload(false);
			}
		}

		void Start()
		{
			GrassRenderController.Instance.Register(this);
		}

		public System.Collections.IEnumerator GenerateData(Vector4 rotation)
		{
			this.offsetResult = GetInternalOffset();
			var nativeArrayType = UnhollowerRuntimeLib.Il2CppType.Of<int>();
			nativeTrianglesArray = Il2CppSystem.Array.CreateInstance(nativeArrayType, MeshFilterRef.sharedMesh.triangles.Length);
			yield return null;
			CopyTriangles();
			yield return null;
			GeneratePositions(rotation);

			SuisHackMain.loggerInst.Msg($"Generated (frame {Time.frameCount}");
		}

		private void CopyTriangles()
		{
			var copyVerteciesShader = GrassRenderController.CopyTrinaglesComputeShader;
			if (copyVerteciesShader == null)
			{
				SuisHackMain.loggerInst.Error("No copy verticies compute shader!");
				return;
			}

			int kernel = copyVerteciesShader.FindKernel("CopyData");
			copyVerteciesShader.GetKernelThreadGroupSizes(kernel, out uint kernelSizeX, out uint kernelSizeY, out uint kernelSizeZ);
			var triangles = MeshFilterRef.sharedMesh.triangles;
			var trianglesCap = (int)Mathf.Ceil(triangles.Length / 4.0f) * 4;

			var pseudoVectorArray = new Vector4[trianglesCap / 4];
			for (int i = 0; i < triangles.Length; i += 4)
			{
				pseudoVectorArray[i / 4] = new Vector4(
					triangles[i],
					i + 1 < triangles.Length ? triangles[i + 1] : 0,
					i + 2 < triangles.Length ? triangles[i + 2] : 0,
					i + 3 < triangles.Length ? triangles[i + 3] : 0);
			}
			copyVerteciesShader.SetVectorArray("_VertexData", pseudoVectorArray);
			trianglesBuffer = new ComputeBuffer(trianglesCap, sizeof(int));
			copyVerteciesShader.SetBuffer(kernel, "_OutputTriangles", trianglesBuffer);
			copyVerteciesShader.Dispatch(kernel, Mathf.CeilToInt(trianglesCap / kernelSizeX), (int)kernelSizeY, (int)kernelSizeZ);
			SuisHackMain.loggerInst.Msg("Copied triangles");
		}

		public Bounds Bounds => this.MeshRendererRef.bounds;

		private void GeneratePositions()
		{
			var generatePositionsShader = GrassRenderController.GeneratePositionsComputeShader;
			if (generatePositionsShader == null)
			{
				SuisHackMain.loggerInst.Msg("No generate positions compute shader!");
				return;
			}

			int mainKernel = generatePositionsShader.FindKernel("CSGenerate");
			generatePositionsShader.GetKernelThreadGroupSizes(mainKernel, out uint x, out uint y, out uint _);
			generatePositionsShader.SetVector(ShaderHash_InternalSquareOffset, offsetResult);
			generatePositionsShader.SetVector(ShaderHash_ExternalOffset, this.transform.position);

			Vector4[] verts3 = MeshFilterRef.sharedMesh.vertices.Select(yeet => (Vector4)yeet).ToArray();
			positionsBuffer = new ComputeBuffer(initialInstanceCount, sizeof(float) * 4);

			generatePositionsShader.SetVectorArray(ShaderHash_VertexData, verts3);
			generatePositionsShader.SetInt(ShaderHash_VertexCount, verts3.Length);
			generatePositionsShader.SetInt(ShaderHash_TriangleCount, MeshFilterRef.sharedMesh.triangles.Length);
			generatePositionsShader.SetBuffer(mainKernel, ShaderHash_Triangles, trianglesBuffer);

			//Output
			generatePositionsShader.SetBuffer(mainKernel, ShaderHash_PositionsData, positionsBuffer);
			generatePositionsShader.Dispatch(mainKernel, Mathf.CeilToInt(sideSize / x), Mathf.CeilToInt(sideSize / y), 1);


			Il2CppSystem.Array args = Il2CppSystem.Array.CreateInstance(UnhollowerRuntimeLib.Il2CppType.Of<uint>(), 5);

			if (instancedMesh != null)
			{
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMesh.GetIndexCount(0) }.BoxIl2CppObject(), 0);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = (uint)positionsBuffer.count }.BoxIl2CppObject(), 1);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMesh.GetIndexStart(0) }.BoxIl2CppObject(), 2);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMesh.GetBaseVertex(0) }.BoxIl2CppObject(), 3);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = 0 }.BoxIl2CppObject(), 4);
			}

			argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments, 1);
			SuisHackMain.loggerInst.Msg("Step 6");
			if(argsBuffer == null)
				SuisHackMain.loggerInst.Error("Args buffer is null");

			argsBuffer.InternalSetData(args, 0, 0, args.Length, 4);
			SuisHackMain.loggerInst.Msg("Step 7");

			mpb = new MaterialPropertyBlock();
			mpb.SetBuffer(ShaderHash_PositionsData, positionsBuffer);
			SuisHackMain.loggerInst.Msg("Step 8");

			trianglesBuffer.Dispose();

			SuisHackMain.loggerInst.Msg("Generated positions");
		}

		private Vector3 GetInternalOffset()
		{
			var split = this.name.Split('_');
			var offsetXStr = float.Parse(split[split.Length - 2].Substring(1));
			var offsetYStr = float.Parse(split[split.Length - 1].Substring(1));

			return new Vector3(256 * offsetXStr, 0, 256 * offsetYStr);
		}

		public void RenderMeshes()
		{
			//GPUCull();
			Graphics.DrawMeshInstancedIndirect(instancedMesh, 0, instancedMaterial, this.MeshRendererRef.bounds, argsBuffer, 0, mpb, UnityEngine.Rendering.ShadowCastingMode.Off);
		}

		void OnDestroy()
		{
			if (GrassRenderController.Instance != null)
				GrassRenderController.Instance.Unregister(this);
		}*/
	}
}
