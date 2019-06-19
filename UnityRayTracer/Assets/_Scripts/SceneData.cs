using System.Collections.Generic;
using UnityEngine;

public class SceneData
{
    public int _MaxRecursionDepth = 8;

    public Vector3 _BackGroundColor = Vector3.zero;

    /*-----Cameras-------*/

    public int _ActiveCameraNo = 1;

    public List<CameraData> _CameraDatas = new List<CameraData>();

    public Matrix4x4 _CameraToWorldMatrix;
    public Matrix4x4 _CameraInverseProjectionMatrix;

    /*-----Lights-------*/

    public Vector3 _AmbientLight = Vector3.zero;

    public int _PointLightCount = 0;

    public List<PointLightData> _PointLightDatas = new List<PointLightData>();

    // TODO: Create custome struct in shader. Maybe use sendFloats?

    public DirectLightData _DirectLightData = new DirectLightData();

    /*-----Materials-------*/

    public int _MaterialCount = 0;
    public List<MaterialData> _MaterialDatas = new List<MaterialData>();

    /*-----GeometryData-------*/

    public int _SphereCount = 0;
    public List<Sphere> _Spheres = new List<Sphere>();

    public int _MeshCount = 0;
    public int _SizeOfVertexList = 0;
    public int _SizeOfTriangleList = 0;

    public List<MeshData> _MeshDataList = new List<MeshData>();
    public List<Vector3> _VertexList = new List<Vector3>();
    public List<Triangle> _TriangleList = new List<Triangle>();

    /*-----BVH Structures-------*/

    public int _SizeOfBVHNodeList = 0;
    public List<LinearBVHNode> _BVHNodeList = new List<LinearBVHNode>();
}