using System.Collections.Generic;
using UnityEngine;


public static class SceneData
{
    public static int _MaxRecursionDepth;
    public static Vector3 _BackGroundColor;

    /*-----Cameras-------*/

    public static List<CameraData> _CameraDatas = new List<CameraData>();

    public static Matrix4x4 _CameraToWorldMatrix;
    public static Matrix4x4 _CameraInverseProjectionMatrix;

    /*-----Lights-------*/

    public static Vector3 _AmbientLight;

    public static int _PointLightCount = 0;

    public static List<PointLightData> _PointLightDatas = new List<PointLightData>();

    // TODO: Create custome struct in shader. Maybe use sendFloats?

    public static DirectLightData _DirectLightData;

    /*-----Materials-------*/


    public static int _MaterialCount;
    public static List<MaterialData> _MaterialDatas = new List<MaterialData>();

    /*-----GeometryData-------*/

    public static int _SphereCount = 0;
    public static List<Sphere> _Spheres = new List<Sphere>();

    public static int _MeshCount = 0;
    public static int _SizeOfVertexList = 0;
    public static int _SizeOfIndexList = 0;

    public static List<MeshData> _MeshDataList = new List<MeshData>();
    public static List<Vector3> _VertexList = new List<Vector3>();
    public static List<int> _IndexList = new List<int>();
}
