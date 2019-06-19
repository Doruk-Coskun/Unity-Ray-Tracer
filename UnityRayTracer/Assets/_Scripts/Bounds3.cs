using UnityEngine;

public struct Bounds3
{
    public Vector3 pMin, pMax;

    public Bounds3 Union(Bounds3 other) 
    {
        Bounds3 newBounds = new Bounds3();
        newBounds.pMax = Vector3.Max(pMax, other.pMax);
        newBounds.pMin = Vector3.Min(pMin, other.pMin);
        return newBounds;
    }

    public Bounds3 Union(Vector3 vec)
    {
        Bounds3 newBounds = new Bounds3();
        newBounds.pMin.x = Mathf.Min(pMin.x, vec.x);
        newBounds.pMin.y = Mathf.Min(pMin.y, vec.y);
        newBounds.pMin.z = Mathf.Min(pMin.z, vec.z);
        newBounds.pMax.x = Mathf.Max(pMax.x, vec.x);
        newBounds.pMax.y = Mathf.Max(pMax.y, vec.y);
        newBounds.pMax.z = Mathf.Max(pMax.z, vec.z);
        return newBounds;
    }

    public int MaximumExtend()
    {
        Vector3 d = pMax - pMin;
        if (d.x > d.y && d.x > d.z)
            return 0;
        else if (d.y > d.z)
            return 1;
        else
            return 2;
    }
}
