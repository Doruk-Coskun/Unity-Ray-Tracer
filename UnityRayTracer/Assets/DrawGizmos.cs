using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    public int level_count;

    private void OnDrawGizmosSelected()
    {
        //foreach(LinearBVHNode node in SceneParser._SceneData._BVHNodeList)
        //{
        //    Gizmos.color = Color.yellow;

        //    Vector3 gizmoCenter = (node.bounds.pMax + node.bounds.pMin) / 2;
        //    Vector3 gizmoDiff = (node.bounds.pMax - node.bounds.pMin);
        //    Gizmos.DrawWireCube(gizmoCenter, gizmoDiff);
        //}


        for (int i = 0; i < level_count; i++)
        {
            LinearBVHNode node = SceneParser._SceneData._BVHNodeList[i];

            Gizmos.color = new Color(1, 0, 0, 0.5f) + new Color(0, 1, 1, 0.5f) / level_count * i;

            if (i == level_count - 1)
            {
                Gizmos.color = Color.yellow;
            }

            Vector3 gizmoCenter = (node.bounds.pMax + node.bounds.pMin) / 2;
            Vector3 gizmoDiff = (node.bounds.pMax - node.bounds.pMin);
            Gizmos.DrawWireCube(gizmoCenter, gizmoDiff);
        }
    }
}
