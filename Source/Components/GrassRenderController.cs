using Il2CppSystem.IO;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuisHack.Components
{
	[RegisterTypeInIl2Cpp]
	public class GrassRenderController : MonoBehaviour
	{
		public const int SIDESIZE = 256;
		public const int INITIALINSTANCECOUNT = SIDESIZE * SIDESIZE;
		private const int NUMBER_OF_ARGS_PER_DRAW = 5; // (indexCount, instanceCount, startIndex, baseVertex, startInstance)
		private const int ARGS_BYTE_SIZE_PER_DRAW_CALL = NUMBER_OF_ARGS_PER_DRAW * sizeof(uint); // 5args * 4bytes = 20 bytes

		public static GrassRenderController Instance;
		Queue<GrassGPUGeneration> TilesToProcess = new Queue<GrassGPUGeneration>();
		HashSet<GrassGPUGeneration> TilesToRender = new HashSet<GrassGPUGeneration>();
		public Mesh instancedMeshLOD0;
		public Mesh instancedMeshLOD1;
		public Mesh instancedMeshLOD2;
		public Material instancedMaterial;
		public static ComputeShader ComputeShaderGeneratePositions { get; private set; }
		private ComputeShader ComputeShaderGPUFrustumCull;
		private ComputeBuffer positionsBufferCulled;
		private ComputeBuffer positionsBufferWorldToObjectCulledCulled;
		private MaterialPropertyBlock mpbLOD0;
		private MaterialPropertyBlock mpbLOD1;
		private MaterialPropertyBlock mpbLOD2;

		private float maxDistLOD0;
		private float maxDistLOD1;
		private float maxDistLOD2;
		private float decimateDistance;
		private Vector4 rotationCorrection;

		private GrassGPUGeneration LastGPUGeneration;

		private Il2CppSystem.Array args;

		private ComputeBuffer argsBuffer;

		public static void Initialize()
		{
			if (Instance == null)
			{
				var go = new GameObject("GPU_Instance_Controller");
				Instance = go.AddComponent<GrassRenderController>();
				DontDestroyOnLoad(go);
			}
		}

		public GrassRenderController(IntPtr ptr) : base(ptr) { }

		void Awake()
		{
			if (Instance == null)
			{
				SetValues();
				LoadAssets();

				Instance = this;
				args = Il2CppSystem.Array.CreateInstance(UnhollowerRuntimeLib.Il2CppType.Of<uint>(), 15);
				argsBuffer = new ComputeBuffer(1, 15 * sizeof(uint), ComputeBufferType.IndirectArguments, 1);

				positionsBufferCulled = new ComputeBuffer(INITIALINSTANCECOUNT * 3, sizeof(float) * 4 * 4);
				positionsBufferWorldToObjectCulledCulled = new ComputeBuffer(INITIALINSTANCECOUNT * 3, sizeof(float) * 4 * 4);


				//LOD 0
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMeshLOD0.GetIndexCount(0) }.BoxIl2CppObject(), 0);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = 0 }.BoxIl2CppObject(), 1);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMeshLOD0.GetIndexStart(0) }.BoxIl2CppObject(), 2);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMeshLOD0.GetBaseVertex(0) }.BoxIl2CppObject(), 3);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = 0 }.BoxIl2CppObject(), 4);

				//LOD 1
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMeshLOD1.GetIndexCount(0) }.BoxIl2CppObject(), 5);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = 0 }.BoxIl2CppObject(), 6);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMeshLOD1.GetIndexStart(0) }.BoxIl2CppObject(), 7);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMeshLOD1.GetBaseVertex(0) }.BoxIl2CppObject(), 8);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = 0 }.BoxIl2CppObject(), 9);

				//LOD 2
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMeshLOD2.GetIndexCount(0) }.BoxIl2CppObject(), 10);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = 0 }.BoxIl2CppObject(), 11);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMeshLOD2.GetIndexStart(0) }.BoxIl2CppObject(), 12);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = instancedMeshLOD2.GetBaseVertex(0) }.BoxIl2CppObject(), 13);
				args.SetValue(new Il2CppSystem.UInt32() { m_value = 0 }.BoxIl2CppObject(), 14);
				argsBuffer.InternalSetData(args, 0, 0, 15, 4);
				SuisHackMain.loggerInst.Msg("Args buffer ready");

			}
			else
			{
				Destroy(this.gameObject);
			}
		}

		private void SetValues()
		{
			var temp = Quaternion.Euler(new Vector3(-90, 0, 0));
			rotationCorrection = new Vector4(temp.x, temp.y, temp.z, temp.w);

			maxDistLOD0 = 10;
			maxDistLOD1 = 40;
			maxDistLOD2 = 150;
			decimateDistance = 55;
		}

		private void LoadAssets()
		{
			SuisHackMain.loggerInst.Error("Loading bundle");

			AssetBundle shadersBundleHandle;
			var path = Path.Combine(Application.streamingAssetsPath, "suihackshaders");
			if (!File.Exists(path))
			{
				SuisHackMain.loggerInst.Error("No shaders asset bundle!");
				Destroy(this.gameObject);
				return;
			}


			shadersBundleHandle = AssetBundle.LoadFromFile(path);
			if (shadersBundleHandle == null)
			{
				SuisHackMain.loggerInst.Error("No shaders asset bundle!?!?!?!?");
				Destroy(this.gameObject);
				return;
			}

			if (ComputeShaderGPUFrustumCull == null)
				ComputeShaderGPUFrustumCull = shadersBundleHandle.LoadAsset("ComputeShader_CullPoints").Cast<ComputeShader>();

			if (ComputeShaderGeneratePositions == null)
				ComputeShaderGeneratePositions = shadersBundleHandle.LoadAsset("ComputeShader_GeneratePositions").Cast<ComputeShader>();

			if (instancedMeshLOD0 == null || instancedMeshLOD1 == null || instancedMeshLOD2 == null || instancedMaterial == null)
			{
				var grassFBX = GameObject.Instantiate(shadersBundleHandle.LoadAsset("GrassPrefab").Cast<GameObject>());
				DontDestroyOnLoad(grassFBX);
				if (grassFBX == null)
					SuisHackMain.loggerInst.Error("Grass?");

				var lods = grassFBX.GetComponentsInChildren<MeshFilter>();
				var meshRenderer = grassFBX.GetComponentInChildren<MeshRenderer>();
				instancedMaterial = meshRenderer.material;

				//At least this way it won't destroy these objects
				grassFBX.gameObject.SetActive(false);
				instancedMeshLOD0 = lods[0].sharedMesh;
				instancedMeshLOD1 = lods[1].sharedMesh;
				instancedMeshLOD2 = lods[2].sharedMesh;
			}

			shadersBundleHandle.Unload(false);
		}

		private uint DBG_InstancedRenderedLOD0;
		private uint DBG_InstancedRenderedLOD1;
		private uint DBG_InstancedRenderedLOD2;

		private void Update()
		{
			//Because asset bundles expect shaders to come attached to objects already so any time the scene changes Unity just destroys them :(
			if (ComputeShaderGPUFrustumCull == null || ComputeShaderGeneratePositions == null || instancedMeshLOD0 == null || instancedMeshLOD1 == null || instancedMeshLOD2 == null || instancedMaterial == null)
			{
				LoadAssets();
				ComputeShaderGPUFrustumCull.SetBuffer(0, GrassShaderHashes.PositionsDataCulled, positionsBufferCulled);
				ComputeShaderGPUFrustumCull.SetBuffer(0, GrassShaderHashes.PositionsDataWorldToObjectCulled, positionsBufferWorldToObjectCulledCulled);
				ComputeShaderGPUFrustumCull.SetInt(GrassShaderHashes.MaxPoints, INITIALINSTANCECOUNT);
			}

			if (LastGPUGeneration != null)
			{
				TilesToRender.Add(LastGPUGeneration);
				LastGPUGeneration = null;
				return;
			}

			if (TilesToProcess.Count > 0)
			{
				var tileToProcess = TilesToProcess.Dequeue();
				if (tileToProcess != null)
				{
					tileToProcess.GenerateData(rotationCorrection);
					LastGPUGeneration = tileToProcess;
				}
			}
		}


		private void LateUpdate()
		{
			DBG_InstancedRenderedLOD0 = 0;
			DBG_InstancedRenderedLOD1 = 0;
			DBG_InstancedRenderedLOD2 = 0;

			var cameraMain = Camera.main;
			if (cameraMain == null)
				return;

			//Because Geometric utility seems to throw an error
			var frustumPlanes = CalculateManualPlanes(cameraMain);


			var bounds = new Bounds(cameraMain.transform.position, new Vector3(SIDESIZE, SIDESIZE, SIDESIZE));


			if (mpbLOD0 == null)
			{
				mpbLOD0 = new MaterialPropertyBlock();
				mpbLOD0.SetBuffer(GrassShaderHashes.PositionsData, positionsBufferCulled);
				mpbLOD0.SetBuffer(GrassShaderHashes.PositionsDataWorldToObject, positionsBufferWorldToObjectCulledCulled);
				mpbLOD0.SetInt(GrassShaderHashes.LOD_Array_Offset, INITIALINSTANCECOUNT * 0);

				mpbLOD1 = new MaterialPropertyBlock();
				mpbLOD1.SetBuffer(GrassShaderHashes.PositionsData, positionsBufferCulled);
				mpbLOD1.SetBuffer(GrassShaderHashes.PositionsDataWorldToObject, positionsBufferWorldToObjectCulledCulled);
				mpbLOD1.SetInt(GrassShaderHashes.LOD_Array_Offset, INITIALINSTANCECOUNT * 1);

				mpbLOD2 = new MaterialPropertyBlock();
				mpbLOD2.SetBuffer(GrassShaderHashes.PositionsData, positionsBufferCulled);
				mpbLOD2.SetBuffer(GrassShaderHashes.PositionsDataWorldToObject, positionsBufferWorldToObjectCulledCulled);
				mpbLOD2.SetInt(GrassShaderHashes.LOD_Array_Offset, INITIALINSTANCECOUNT * 2);
			}

			//Restart LOD counts
			args.SetValue(new Il2CppSystem.UInt32() { m_value = 0 }.BoxIl2CppObject(), 0 * 5 + 1);
			args.SetValue(new Il2CppSystem.UInt32() { m_value = 0 }.BoxIl2CppObject(), 1 * 5 + 1);
			args.SetValue(new Il2CppSystem.UInt32() { m_value = 0 }.BoxIl2CppObject(), 2 * 5 + 1);
			argsBuffer.InternalSetData(args, 0, 0, 15, 4);


			ComputeShaderGPUFrustumCull.SetVector(GrassShaderHashes.CameraPosition, cameraMain.transform.position);
			ComputeShaderGPUFrustumCull.SetVectorArray(GrassShaderHashes.CameraClippingPlanes, frustumPlanes);
			ComputeShaderGPUFrustumCull.SetBuffer(0, GrassShaderHashes.ArgsBuffer, argsBuffer);
			ComputeShaderGPUFrustumCull.SetInt(GrassShaderHashes.MaxPoints, INITIALINSTANCECOUNT);
			ComputeShaderGPUFrustumCull.SetFloat(GrassShaderHashes.Decimate_Distance, decimateDistance);
			ComputeShaderGPUFrustumCull.SetFloat(GrassShaderHashes.LOD_Distance0, maxDistLOD0);
			ComputeShaderGPUFrustumCull.SetFloat(GrassShaderHashes.LOD_Distance1, maxDistLOD1);
			ComputeShaderGPUFrustumCull.SetFloat(GrassShaderHashes.LOD_Distance2, maxDistLOD2);

			foreach (GrassGPUGeneration square in TilesToRender)
			{
				var closestPost = square.squareMeshRenderer.bounds.ClosestPoint(cameraMain.transform.position);
				var dist = Vector3.Distance(cameraMain.transform.position, closestPost);

				if (dist < 2500 )
				{
					ComputeShaderGPUFrustumCull.SetBuffer(0, GrassShaderHashes.PositionsData, square.PositionsBuffer);
					ComputeShaderGPUFrustumCull.SetBuffer(0, GrassShaderHashes.PositionsDataWorldToObject, square.PositionsBufferWorldToObject);
					ComputeShaderGPUFrustumCull.Dispatch(0, Mathf.CeilToInt(square.PositionsBuffer.count / 64), 1, 1);
				}
			}

			argsBuffer.InternalGetData(args, 0, 0, 15, 4);
			DBG_InstancedRenderedLOD0 = args.GetValue(5 * 0 + 1).Unbox<uint>();
			DBG_InstancedRenderedLOD1 = args.GetValue(5 * 1 + 1).Unbox<uint>();
			DBG_InstancedRenderedLOD2 = args.GetValue(5 * 2 + 1).Unbox<uint>();

			Graphics.DrawMeshInstancedIndirect(instancedMeshLOD0, 0, instancedMaterial, bounds, argsBuffer, ARGS_BYTE_SIZE_PER_DRAW_CALL * 0, mpbLOD0, UnityEngine.Rendering.ShadowCastingMode.Off);
			Graphics.DrawMeshInstancedIndirect(instancedMeshLOD1, 0, instancedMaterial, bounds, argsBuffer, ARGS_BYTE_SIZE_PER_DRAW_CALL * 1, mpbLOD1, UnityEngine.Rendering.ShadowCastingMode.Off);
			Graphics.DrawMeshInstancedIndirect(instancedMeshLOD2, 0, instancedMaterial, bounds, argsBuffer, ARGS_BYTE_SIZE_PER_DRAW_CALL * 2, mpbLOD2, UnityEngine.Rendering.ShadowCastingMode.Off);
		}

		private Vector4[] CalculateManualPlanes(Camera cam)
		{
			Vector4[] planes = new Vector4[6];

			//0.0174532924F is Mathf.Deg2Rad
			float halfVSize = cam.farClipPlane * Mathf.Tan(cam.fieldOfView * 0.0174532924f * 0.5f);
			float halfHSize = halfVSize * cam.aspect;

			Vector3 frontMultFar = cam.farClipPlane * cam.transform.forward;


			planes[0] = CreatePlane(Vector3.Cross(cam.transform.up, frontMultFar - cam.transform.right * halfHSize), cam.transform.position); //Right plane
			planes[1] = CreatePlane(Vector3.Cross(frontMultFar + cam.transform.right * halfHSize, cam.transform.up), cam.transform.position); //Left plane
			planes[2] = CreatePlane(Vector3.Cross(frontMultFar - cam.transform.up * halfVSize, cam.transform.right), cam.transform.position); //Bottom plane
			planes[3] = CreatePlane(Vector3.Cross(cam.transform.right, frontMultFar + cam.transform.up * halfVSize), cam.transform.position); //Top plane

			planes[4] = CreatePlane(cam.transform.forward, cam.transform.position + cam.nearClipPlane * cam.transform.forward); //Near plane
			planes[5] = CreatePlane(-cam.transform.forward, cam.transform.position + cam.farClipPlane * cam.transform.forward); //Near plane


			return planes;
		}

		public Vector4 CreatePlane(Vector3 normal, Vector3 Position)
		{
			var plane = new Plane(normal, Position);
			return new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
		}

		private void OnGUI()
		{
			GUILayout.BeginHorizontal(null);
			GUILayout.BeginVertical(GUI.skin.box, null);
			GUILayout.Label("Tiles to process: " + TilesToProcess.Count, null);
			GUILayout.Label("Tiles to render: " + TilesToRender.Count, null);
			GUILayout.Label("", null);


			GUILayout.Label("LOD0: " + DBG_InstancedRenderedLOD0, null);
			GUILayout.Label("LOD1: " + DBG_InstancedRenderedLOD1, null);
			GUILayout.Label("LOD2: " + DBG_InstancedRenderedLOD2, null);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		public void Register(GrassGPUGeneration gPU_Terrain_Rendering_Instance)
		{
			if (!TilesToRender.Contains(gPU_Terrain_Rendering_Instance))
			{
				TilesToProcess.Enqueue(gPU_Terrain_Rendering_Instance);
			}
		}

		public void Unregister(GrassGPUGeneration gPU_Terrain_Rendering_Instance)
		{
			TilesToRender.Remove(gPU_Terrain_Rendering_Instance);
		}
	}
}
