using System.Collections.Generic;
using UnityEngine;

public class GeometryData
{
    public int _MeshCount = 0;
    public int _SizeOfVertexList = 0;
    public int _SizeOfIndexList = 0;

    public List<MeshData> _MeshDataList = new List<MeshData>();
    public List<Vector3> _VertexList = new List<Vector3>();
    public List<int> _IndexList = new List<int>();
}