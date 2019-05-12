using UnityEngine;

public struct BVHPrimitiveInfo
{
    public int triangleNo;
    public Bounds3 bounds;
    public Vector3 center;

    public BVHPrimitiveInfo(int triNo, Bounds3 bound)
    {
        triangleNo = triNo;
        bounds = bound;
        center = 0.5f * bounds.pMax + 0.5f * bounds.pMin;
    }
}
