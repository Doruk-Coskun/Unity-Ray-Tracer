using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleTest : MonoBehaviour
{
    public Material mat;
    GameObject newObject;
    // Start is called before the first frame update
    void Awake()
    {
        newObject = new GameObject();
        newObject.transform.parent = GameObject.Find("SceneGeometry/Meshes").transform;
        newObject.AddComponent<MeshFilter>();
        newObject.AddComponent<MeshRenderer>();
        newObject.GetComponent<MeshRenderer>().material = mat;
        GenerateTriangle();
    }

    private void GenerateTriangle()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = {
                                new Vector3(-5f, 0, 0),
                                new Vector3(0, 5f, 0),
                                new Vector3(5f, 0, 0)
                             };

        int[] triangles = {
                                0, 1, 2
                          };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        newObject.GetComponent<MeshFilter>().mesh = mesh;
    }
}
