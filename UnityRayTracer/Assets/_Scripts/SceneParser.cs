using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneParser : MonoBehaviour
{
    public static SceneParser _instance = null;

    public enum GenerateScene
    {
        SceneEditor,
        FromXML
    }

    public RayTracingMaster rayTracingMaster;

    [HideInInspector]
    public static SceneData _SceneData;

    private Camera _Camera;

    [Range(1, 5)]
    public int cameraNo;

    [SerializeField]
    public GenerateScene generateScene;

    [ConditionalHide("generateScene", 0)]
    [SerializeField]
    public int maxRecursionDepth = 8;
    [ConditionalHide("generateScene", 0)]
    [SerializeField]
    public Color backgroundColor = Color.black;
    [ConditionalHide("generateScene", 0)]
    [SerializeField]
    public Color ambientLight = Color.black;
    [ConditionalHide("generateScene", 0)]
    [SerializeField]
    private bool realtimeUpdate = false;

    [ConditionalHide("generateScene", 1)]
    public TextAsset XMLFile;

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
    }

    private void Start()
    {
        SetSceneData();
        rayTracingMaster.SetUpScene();
    }

    private void Update()
    {
        if (generateScene == GenerateScene.SceneEditor && realtimeUpdate)
        {
            SetSceneData();
            rayTracingMaster.SetUpScene();
        }
    }

    public void SetSceneData()
    {
        _SceneData = new SceneData();

        _SceneData._MaxRecursionDepth = maxRecursionDepth;

        SetBackgroundColor();
        ParseSceneCameras();
        SetRenderCameraData();
        ParseSceneLights();
        ParseSceneObjects();

        Transform[] cameraTransforms = GameObject.Find("SceneCameras").GetComponentsInChildren<Transform>();
        if (cameraNo <= _SceneData._CameraDatas.Count)
        {
            rayTracingMaster = cameraTransforms[cameraNo].GetComponent<RayTracingMaster>();
        }
    }

    private void SetBackgroundColor()
    {
        _SceneData._BackGroundColor = new Vector3(
                                            backgroundColor.r,
                                            backgroundColor.g,
                                            backgroundColor.b
                                                 );
    }

    private void ParseSceneCameras()
    {
        Transform cameraTransforms = GameObject.Find("SceneCameras").GetComponent<Transform>();

        foreach (Transform cameraTransform in cameraTransforms)
        {
            CameraData newCameraData = new CameraData();
            newCameraData._Position = cameraTransform.position;
            newCameraData._Gaze = cameraTransform.forward;
            newCameraData._Up = cameraTransform.up;
            newCameraData._Fov = cameraTransform.GetComponent<Camera>().fieldOfView;
            _SceneData._CameraDatas.Add(newCameraData);
        }
    }

    private void SetRenderCameraData() 
    {
        Transform[] cameraTransforms = GameObject.Find("SceneCameras").GetComponentsInChildren<Transform>();
        if (cameraNo <= _SceneData._CameraDatas.Count)
        {
            _Camera = cameraTransforms[cameraNo].GetComponent<Camera>();
            _Camera.transform.forward = -Vector3.forward;
            _SceneData._CameraToWorldMatrix = _Camera.cameraToWorldMatrix;
            _SceneData._CameraInverseProjectionMatrix = _Camera.projectionMatrix.inverse;
        }
    }

    private void ParseSceneObjects()
    {
        ParseSceneSpheres();
        ParseSceneMeshes();
    }

    private void ParseSceneLights()
    {
        ParseAmbientLight();
        ParseDirectLight();
        ParseScenePointLights();
    }

    private void ParseAmbientLight()
    {
        _SceneData._AmbientLight = new Vector3(
                                            ambientLight.r,
                                            ambientLight.g,
                                            ambientLight.b
                                              );
    }

    private void ParseDirectLight()
    {
        GameObject directLight = GameObject.Find("SceneLights/DirectLight");
        _SceneData._DirectLightData._Direction = directLight.GetComponent<Transform>().forward;
        _SceneData._DirectLightData._Intensity = directLight.GetComponent<Light>().intensity * 
                                                 new Vector3(
                                                          directLight.GetComponent<Light>().color.r,
                                                          directLight.GetComponent<Light>().color.g,
                                                          directLight.GetComponent<Light>().color.b
                                                            ) / 255;
    }

    private void ParseScenePointLights()
    {
        Transform pointLights = GameObject.Find("SceneLights/PointLights").GetComponent<Transform>();

        foreach (Transform pointLight in pointLights)
        {
            PointLightData newPointLightData = new PointLightData();
            Light light = pointLight.GetComponent<Light>();

            newPointLightData._Position = pointLight.position;
            newPointLightData._Intensity = light.intensity * new Vector3(
                                                                        light.color.r,
                                                                        light.color.g,
                                                                        light.color.b
                                                                        ) / 255;
            _SceneData._PointLightCount++;
            _SceneData._PointLightDatas.Add(newPointLightData);
        }
    }

    private void ParseSceneMeshes()
    {
        Transform meshObjects = GameObject.Find("SceneGeometry/Meshes").GetComponent<Transform>();

        int iterator = 0;

        foreach (Transform meshObject in meshObjects)
        {
            MeshData newMeshData = new MeshData();
            newMeshData._TriangleIndexStart = _SceneData._SizeOfTriangleList;
            newMeshData._VertexIndexStart = _SceneData._SizeOfVertexList;
            Vector3 position = meshObject.position;
            float scale = meshObject.localScale.x;

            Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;

            Vector3[] newVertexList = mesh.vertices;

            for (int i = 0; i < newVertexList.Length; i++)
            {
                newVertexList[i] = meshObject.rotation * newVertexList[i];
                newVertexList[i] = meshObject.localScale.x * newVertexList[i];
                newVertexList[i] = meshObject.position + newVertexList[i];
            }

            _SceneData._VertexList.AddRange(newVertexList);
            _SceneData._SizeOfVertexList += mesh.vertexCount;

            iterator = 0;

            while (iterator < mesh.triangles.Length)
            {
                Vector3 newTriangle = new Vector3();
                newTriangle.x = mesh.triangles[iterator    ];
                newTriangle.y = mesh.triangles[iterator + 1];
                newTriangle.z = mesh.triangles[iterator + 2];

                iterator += 3;
                _SceneData._TriangleList.Add(newTriangle);
            }

            _SceneData._SizeOfTriangleList = _SceneData._TriangleList.Count;

            // TODO: Dangerous, be careful of this!
            newMeshData._TriangleIndexEnd = _SceneData._SizeOfTriangleList;

            AddMaterialData(meshObject);

            newMeshData._MaterialID = _SceneData._MaterialCount - 1;

            _SceneData._MeshDataList.Add(newMeshData);
            _SceneData._MeshCount++;
        }
    }

    private void ParseSceneSpheres()
    {
        Transform spheres = GameObject.Find("SceneGeometry/Spheres").GetComponent<Transform>();

        foreach (Transform sphere in spheres)
        {
            Sphere newSphere = new Sphere();
            newSphere._Position = sphere.position;
            newSphere._Radius = sphere.localScale.x / 2;

            AddMaterialData(sphere);

            newSphere._MaterialID = _SceneData._MaterialCount - 1;

            _SceneData._Spheres.Add(newSphere);
            _SceneData._SphereCount++;
        }
    }

    private void AddMaterialData(Transform newObject)
    {
        MaterialData newMaterialData = new MaterialData();
        Material meshMat = newObject.GetComponent<Renderer>().material;

        newMaterialData._AmbientReflectance = meshMat.GetVector("_AmbientReflectance");
        newMaterialData._DiffuseReflectance = meshMat.GetVector("_DiffuseReflectance");
        newMaterialData._SpecularReflectance = meshMat.GetVector("_SpecularReflectance");
        newMaterialData._MirrorReflectance = meshMat.GetVector("_MirrorReflectance");
        newMaterialData._Transparency = meshMat.GetVector("_Transparency");
        newMaterialData._PhongExponent = meshMat.GetFloat("_PhongExponent");
        newMaterialData._RefractionIndex = meshMat.GetFloat("_RefractionIndex");

        _SceneData._MaterialDatas.Add(newMaterialData);
        _SceneData._MaterialCount++;
    }
}
