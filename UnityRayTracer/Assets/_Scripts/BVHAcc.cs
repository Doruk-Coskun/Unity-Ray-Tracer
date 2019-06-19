using System.Collections.Generic;
using System.Linq;
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

            int mid = 0;
            switch (SceneParser._instance.splitMethod)
            {
                case SceneParser.SplitMethod.EqualCounts:
                    mid = (start + end) / 2;

                    // Order Primitives in axis 'dim' and split the list into 2 equal sized lists
                    EqualCounts(ref primitiveInfos, dim, start, end);
                    break;
                case SceneParser.SplitMethod.SAH:
                    if (nPrimitives <= 4)
                    {
                        mid = (start + end) / 2;

                        // Order Primitives in axis 'dim' and split the list into 2 equal sized lists
                        EqualCounts(ref primitiveInfos, dim, start, end);
                        break;
                    }
                    else
                    {
                        const int nBuckets = 12;
                        List<BucketInfo> buckets = Enumerable.Repeat(new BucketInfo(), nBuckets).ToList();

                        //for (int i = 0; i < buckets.Capacity; i++)
                        //{
                        //    buckets[i] = new BucketInfo();
                        //}

                        for (int i = start; i < end; i++)
                        {
                            int b = nBuckets * (int) centroidBounds.Offset(primitiveInfos[i].center)[dim];
                            if (b == nBuckets) b = nBuckets - 1;
                            buckets[b].count++;
                            buckets[b].bounds = buckets[b].bounds.Union(primitiveInfos[i].bounds);
                        }

                        float[] cost = new float[nBuckets - 1];
                        for (int i = 0; i < nBuckets - 1; i++)
                        {
                            Bounds3 b0 = new Bounds3();
                            Bounds3 b1 = new Bounds3();

                            int count0 = 0, count1 = 0;
                            for (int j = 0; j <= i; j++)
                            {
                                b0 = b0.Union(buckets[j].bounds);
                                count0 += buckets[j].count;
                            }
                            for (int j = i + 1; j <= nBuckets - 1; j++)
                            {
                                b1 = b1.Union(buckets[j].bounds);
                                count1 += buckets[j].count;
                            }

                            cost[i] = 0.125f + (count0 * b0.SurfaceArea() + count1 * b1.SurfaceArea()) 
                                / bounds.SurfaceArea();
                        }

                        float minCost = cost[0];
                        int minCostSplitBucket = 0;
                        for (int i = 1; i < nBuckets - 1; i++)
                        {
                            if (cost[i] < minCost)
                            {
                                minCost = cost[i];
                                minCostSplitBucket = i;
                            }
                        }

                        List<BVHPrimitiveInfo> partitionList = new List<BVHPrimitiveInfo>();
                        for (int i = 0; i < primitiveInfos.Count; i++)
                        {
                            int b = (int)(nBuckets * centroidBounds.Offset(primitiveInfos[i].center)[dim]);
                            if (b == nBuckets) b = nBuckets - 1;
                            if (b < minCostSplitBucket)
                            {
                                partitionList.Add(primitiveInfos[i]);
                                primitiveInfos.RemoveAt(i);
                                i--;
                            }

                            mid = partitionList.Count;
                            partitionList.AddRange(primitiveInfos);
                            primitiveInfos = partitionList;
                        }
                    }

                    break;
            }



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

public class BucketInfo
{
    public int count = 0;
    public Bounds3 bounds;

    public BucketInfo() 
    {
        count = 0;
        bounds.pMax = Vector3.negativeInfinity;
        bounds.pMin = Vector3.positiveInfinity;
    }
};