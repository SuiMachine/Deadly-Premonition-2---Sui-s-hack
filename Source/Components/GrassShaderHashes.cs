using UnityEngine;

public static class GrassShaderHashes
{
	public static readonly int InternalSquareOffset = Shader.PropertyToID("_InternalSquareOffset");
	public static readonly int ExternalOffset = Shader.PropertyToID("_ExternalOffset");

	public static readonly int PositionsData = Shader.PropertyToID("_PositionsData");
	public static readonly int PositionsDataWorldToObject = Shader.PropertyToID("_PositionsDataWorldToObject");

	public static readonly int PositionsDataCulled = Shader.PropertyToID("_PositionsDataCulled");
	public static readonly int PositionsDataWorldToObjectCulled = Shader.PropertyToID("_PositionsDataWorldToObjectCulled");

	public static readonly int ArgsBuffer = Shader.PropertyToID("_argsBuffer");
	public static readonly int MaxPoints = Shader.PropertyToID("_maxPoints");
	public static readonly int LOD_Distance0 = Shader.PropertyToID("_maxDistLOD0");
	public static readonly int LOD_Distance1 = Shader.PropertyToID("_maxDistLOD1");
	public static readonly int LOD_Distance2 = Shader.PropertyToID("_maxDistLOD2");
	public static readonly int Decimate_Distance = Shader.PropertyToID("_decimateRange");



	public static readonly int VertexData = Shader.PropertyToID("_VertexData");
	public static readonly int VertexCount = Shader.PropertyToID("_VertsCount");

	public static readonly int CameraPosition = Shader.PropertyToID("_cameraPos");
	public static readonly int CameraClippingPlanes = Shader.PropertyToID("_cameraClipPlanes");

	public static readonly int QuaternionRotation = Shader.PropertyToID("_quaternionRotation");

	public static readonly int LOD_Array_Offset = Shader.PropertyToID("_LOD_ArrayOffset");
}