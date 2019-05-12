public class BVHBuildNode
{
    public Bounds3 bounds;
    public BVHBuildNode[] children = new BVHBuildNode[2];
    public int splitAxis, triangleNo = -1;

    public void InitLeaf(int triNo, Bounds3 b) 
    {
        triangleNo = triNo;
        bounds = b;
        children[0] = children[1] = null;
    }
    public void InitInterior(int axis, BVHBuildNode c0, BVHBuildNode c1)
    {
        children[0] = c0;
        children[1] = c1;
        bounds = c0.bounds.Union(c1.bounds);
        splitAxis = axis;
    }
}