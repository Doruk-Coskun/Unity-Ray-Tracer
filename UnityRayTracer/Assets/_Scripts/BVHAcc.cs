using System.Collections.Generic;
using UnityEngine;

public class BVHAcc
{
    public BVHBuildNode root;
    public LinearBVHNode[] LinearBVHArr;

    public BVHAcc(ref List<BVHPrimitiveInfo> primitiveInfos) 
    {
        //Debug.Log("count of primitiveInfos: " + primitiveInfos.Count);

        int totalNodes = 0;
        root = BuildRecursiveBVH(ref primitiveInfos, 0, primitiveInfos.Count, ref totalNodes);

        //Debug.Log("Total nodes: " + totalNodes);
        LinearBVHArr = new LinearBVHNode[totalNodes];
        int offset = 0;
        FlattenBVHTree(root, ref offset);
        //Debug.Log("offset: " + offset);
    }

    private int FlattenBVHTree(BVHBuildNode node, ref int offset)
    {
        int myOffset = offset++;
        LinearBVHArr[myOffset].bounds = node.bounds;
        LinearBVHArr[myOffset].primitiveOffset = node.triangleNo;

        // Inner node
        if (node.triangleNo < 0)
        {
            FlattenBVHTree(node.children[0], ref offset);
            LinearBVHArr[myOffset].secondChildOffset =
                FlattenBVHTree(node.children[1], ref offset);

        }

        return myOffset;
    }

    // List of BVHPrimitive infos including each triangle of the object
    // Start/end/total nodes for recursive split of the list
    private BVHBuildNode BuildRecursiveBVH(ref List<BVHPrimitiveInfo> primitiveInfos,
                                           int start, int end, ref int totalNodes)
    {
        BVHBuildNode node = new BVHBuildNode();
        totalNodes++;

        Bounds3 bounds = new Bounds3();
        bounds.pMin = Vector3.positiveInfinity;
        bounds.pMax = Vector3.negativeInfinity;

        // Calculate bounds of the parent BVH node from its children
        for (int i = start; i < end; i++)
        {
            bounds = bounds.Union(primitiveInfos[i].bounds);
        }

        int nPrimitives = end - start;
        // Create leaf
        if (nPrimitives == 1)
        {
            node.InitLeaf(primitiveInfos[start].triangleNo, bounds);
            return node;
        } 

        else
        {
            Bounds3 centroidBounds = new Bounds3();
            centroidBounds.pMin = Vector3.positiveInfinity;
            centroidBounds.pMax = Vector3.negativeInfinity;

            for (int i = start; i < end; i++)
            {
                centroidBounds = centroidBounds.Union(primitiveInfos[i].center);
            }
            int dim = centroidBounds.MaximumExtend();

            int mid = (start + end) / 2;

            // Order Primitives in axis 'dim' and split the list into 2 equal sized lists
            EqualCounts(ref primitiveInfos, dim, start, end);

            node.InitInterior(dim,
                              BuildRecursiveBVH(ref primitiveInfos, start, mid, ref totalNodes),
                              BuildRecursiveBVH(ref primitiveInfos, mid, end, ref totalNodes)
                             );
            return node;
        }


    }

    private void EqualCounts(ref List<BVHPrimitiveInfo> primitiveInfos, int dim, int start, int end)
    {
        primitiveInfos.Sort(start, end - start, new CFG(dim));
    }
}

class CFG : IComparer<BVHPrimitiveInfo>
{
    int dim;

    public CFG(int newDim)
    {
        dim = newDim;
    }


    public int Compare (BVHPrimitiveInfo x, BVHPrimitiveInfo y)
    {
        return x.center[dim].CompareTo(y.center[dim]);
    }
}