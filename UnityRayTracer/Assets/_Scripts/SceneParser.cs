using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneParser : MonoBehaviour
{
    public static SceneParser _instance = null;

    public enum GenerateScene
    {
        Random,
        SceneEditor
    }

    public RayTracingMaster rayTracingMaster;

    [SerializeField]
    private Camera _Camera;
    [SerializeField]
    private Light _DirectionalLight;

    [HideInInspector]
    public static SceneData _SceneData;
    public static GeometryData _GeometryData;

    [HideInInspector]
    public static List<Sphere> _Spheres = new List<Sphere>();

    [HideInInspector]
    public static List<Light> _PointLights = new List<Light>();

    [SerializeField]
    public GenerateScene generateScene;

    [ConditionalHide("generateScene", 1)]
    [SerializeField]
    private bool realtimeUpdate = false;

    [ConditionalHide("generateScene", 0)]
    public Vector2 SphereRadius = new Vector2(3.0f, 8.0f);
    [ConditionalHide("generateScene", 0)]
    public uint SpheresMax = 100;
    [ConditionalHide("generateScene", 0)]
    public float SpherePlacementRadius = 100.0f;

    public struct Sphere
    {
        public Vector3 position;
        public float radius;
        public Vector3 albedo;
        public Vector3 specular;
    };

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        _SceneData = new SceneData();
        _GeometryData = new GeometryData();
    }

    private void Start()
    {
        switch (generateScene)
        {   
            case GenerateScene.Random:
                SetUpRandomScene();
                break;
            case GenerateScene.SceneEditor:
                ParseSceneObjects();
                break;
            default:
                break;
        }

        SetSceneData();
        rayTracingMaster.SetUpScene();
    }

    private void Update()
    {
        SetSceneData();

        if (generateScene == GenerateScene.SceneEditor && realtimeUpdate)
        {
            ParseSceneObjects();
        }
    }

    public void ParseSceneObjects()
    {
        ParseSceneSpheres();
        ParseScenePointLights();
        ParseSceneMeshes();

        rayTracingMaster.SetUpScene();
    }

    private void ParseScenePointLights()
    {

    }

    private void ParseSceneMeshes()
    {
        _GeometryData._MeshCount = 0;
        _GeometryData._SizeOfIndexList = 0;
        _GeometryData._SizeOfVertexList = 0;

        _GeometryData._MeshDataList.Clear();
        _GeometryData._VertexList.Clear();
        _GeometryData._IndexList.Clear();

        Transform meshObjects = GameObject.Find("SceneGeometry/Meshes").GetComponent<Transform>();

        foreach (Transform meshObject in meshObjects)
        {
            MeshData newMeshData = new MeshData();
            newMeshData._TriangleIndexStart = _GeometryData._SizeOfIndexList;
            newMeshData._VertexIndexStart = _GeometryData._SizeOfVertexList;
            newMeshData.position = meshObject.position;
            newMeshData.scale = meshObject.localScale.x;

            Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;

            _GeometryData._VertexList.AddRange(mesh.vertices);
            _GeometryData._SizeOfVertexList += mesh.vertexCount;

            _GeometryData._IndexList.AddRange(mesh.triangles);
            _GeometryData._SizeOfIndexList += mesh.triangles.Length;

            // TODO: Dangerous, be careful of this!
            newMeshData._TriangleIndexEnd = _GeometryData._SizeOfIndexList;

            Material meshMat = meshObject.GetComponent<Renderer>().material;

            newMeshData.albedo = new Vector3(meshMat.color.r, meshMat.color.g, meshMat.color.b);
            newMeshData.specular = new Vector3(meshMat.GetFloat("_Metallic"),
                                               meshMat.GetFloat("_Metallic"),
                                               meshMat.GetFloat("_Metallic")
                                               );

            _GeometryData._MeshDataList.Add(newMeshData);
            _GeometryData._MeshCount++;
        }
    }

    private void ParseSceneSpheres()
    {
        _Spheres.Clear();
        Transform spheres = GameObject.Find("SceneGeometry/Spheres").GetComponent<Transform>();

        foreach (Transform sphere in spheres)
        {
            Sphere newSphere = new Sphere();
            newSphere.position = sphere.position;
            newSphere.radius = sphere.localScale.x / 2;

            Material sphereMat = sphere.GetComponent<Renderer>().material;
            newSphere.albedo = new Vector3(sphereMat.color.r, sphereMat.color.g, sphereMat.color.b);
            newSphere.specular = new Vector3(sphereMat.GetFloat("_Metallic"),
                                             sphereMat.GetFloat("_Metallic"),
                                             sphereMat.GetFloat("_Metallic")
                                             );

            _Spheres.Add(newSphere);
        }

        _SceneData._SphereCount = _Spheres.Count;
    }

    private void SetSceneData()
    {
        _SceneData._CameraToWorldMatrix = _Camera.cameraToWorldMatrix;
        _SceneData._CameraInverseProjectionMatrix = _Camera.projectionMatrix.inverse;

        _SceneData._DirectionalLightDirection = _DirectionalLight.transform.forward;
        _SceneData._DirectionalLightColor = new Vector3(
                                                _DirectionalLight.color.r,
                                                _DirectionalLight.color.g,
                                                _DirectionalLight.color.b
                                                        );
        _SceneData._DirectionalLightIntensity = _DirectionalLight.intensity;

    }

    public void SetUpRandomScene()
    {
        _Spheres.Clear();

        // Add a number of random spheres
        for (int i = 0; i < SpheresMax; i++)
        {
            Sphere sphere = new Sphere();
            // Radius and radius
            sphere.radius = SphereRadius.x + Random.value * (SphereRadius.y - SphereRadius.x);
            Vector2 randomPos = Random.insideUnitCircle * SpherePlacementRadius;
            sphere.position = new Vector3(randomPos.x, sphere.radius, randomPos.y);
            // Reject spheres that are intersecting others
            foreach (Sphere other in _Spheres)
            {
                float minDist = sphere.radius + other.radius;
                if (Vector3.SqrMagnitude(sphere.position - other.position) < minDist * minDist)
                    goto SkipSphere;
            }
            // Albedo and specular color
            Color color = Random.ColorHSV();
            bool metal = Random.value < 0.5f;
            sphere.albedo = metal ? Vector3.zero : new Vector3(color.r, color.g, color.b);
            sphere.specular = metal ? new Vector3(color.r, color.g, color.b) : Vector3.one * 0.04f;
            // Add the sphere to the list
            _Spheres.Add(sphere);
            _SceneData._SphereCount++;
        SkipSphere:
            continue;
        }

        rayTracingMaster.SetUpScene();
    }
}
