using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public static class SceneXMLParser
{
    public static void LoadSceneFromXML(TextAsset XMLFile)
    {
        SceneParser sceneParser = GameObject.Find("SceneParser").GetComponent<SceneParser>();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(XMLFile.text);

        string[] vectorData;

        /**************MaxRecursionDepth*************/

        XmlNode node = xmlDoc.SelectSingleNode("Scene/MaxRecursionDepth");
        if (node != null)
        {
            sceneParser.maxRecursionDepth = int.Parse(node.InnerText);
        }

        /**************BackgroundColor*************/

        node = xmlDoc.SelectSingleNode("Scene/BackgroundColor");
        vectorData = node.InnerText.Split(' ');

        sceneParser.backgroundColor = new Color(float.Parse(vectorData[0]) / 255,
                                                 float.Parse(vectorData[1]) / 255,
                                                 float.Parse(vectorData[2]) / 255
                                                );

        /**************Cameras*************/

        node = xmlDoc.SelectSingleNode("Scene/Cameras");

        XmlNodeList cameraNodes = node.SelectNodes("Camera");
        Transform cameras = GameObject.Find("SceneCameras").GetComponent<Transform>();

        foreach (XmlNode cameraNode in cameraNodes)
        {
            GameObject newCameraObj = new GameObject();
            newCameraObj.AddComponent<Camera>();
            newCameraObj.AddComponent<RayTracingMaster>();
            newCameraObj.name = "Camera " + cameraNode.Attributes["id"].Value;

            vectorData = cameraNode.SelectSingleNode("Position").InnerText.Trim().Split(' ');
            newCameraObj.transform.position = new Vector3(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1]),
                                                  float.Parse(vectorData[2])
                                                  );

            vectorData = cameraNode.SelectSingleNode("Gaze").InnerText.Split(' ');
            Vector3 forward =  new Vector3(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1]),
                                                  float.Parse(vectorData[2])
                                                  );

            vectorData = cameraNode.SelectSingleNode("Up").InnerText.Split(' ');
            Vector3 up = new Vector3(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1]),
                                                  float.Parse(vectorData[2])
                                                  );

            Quaternion rotation = Quaternion.LookRotation(forward, up);
            newCameraObj.transform.rotation = rotation;

            vectorData = cameraNode.SelectSingleNode("NearPlane").InnerText.Split(' ');
            Vector4 nearPlane = new Vector4(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1]),
                                                  float.Parse(vectorData[2]),
                                                  float.Parse(vectorData[3])
                                                  );

            float nearDistance = int.Parse(cameraNode.SelectSingleNode("NearDistance").InnerText);
            newCameraObj.GetComponent<Camera>().nearClipPlane = nearDistance;

            float alfa = Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs(nearPlane.z / nearDistance));
            newCameraObj.GetComponent<Camera>().fieldOfView = 2 * alfa;

            vectorData = cameraNode.SelectSingleNode("ImageResolution").InnerText.Split(' ');
            Vector3 ImageResolution = new Vector3(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1])
                                                  );

            string ImageName = cameraNode.SelectSingleNode("ImageName").InnerText;

            newCameraObj.transform.parent = cameras;
        }

        /**************Lights*************/

        node = xmlDoc.SelectSingleNode("Scene/Lights");
        Transform pointLights = GameObject.Find("SceneLights/PointLights").GetComponent<Transform>();

        vectorData = node.SelectSingleNode("AmbientLight").InnerText.Split(' ');
        sceneParser.ambientLight = new Color(float.Parse(vectorData[0]) / 255,
                                              float.Parse(vectorData[1]) / 255,
                                              float.Parse(vectorData[2]) / 255
                                              );
                                             
        XmlNodeList lightNodes = node.SelectNodes("PointLight");
        foreach (XmlNode lightNode in lightNodes)
        {
            GameObject newPointLightObj = new GameObject();
            Light newLight = newPointLightObj.AddComponent<Light>();

            newPointLightObj.name = "PointLight " + lightNode.Attributes["id"].Value;

            newLight.type = LightType.Point;

            vectorData = lightNode.SelectSingleNode("Position").InnerText.Split(' ');
            newPointLightObj.transform.position = new Vector3(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1]),
                                                  float.Parse(vectorData[2])
                                                  );

            newLight.color = Color.white;
            vectorData = lightNode.SelectSingleNode("Intensity").InnerText.Split(' ');
            Vector3 intensity = new Vector3(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1]),
                                                  float.Parse(vectorData[2])
                                                  );
            newLight.intensity = intensity.x;

            newPointLightObj.transform.parent = pointLights;
        }

        ///**************Materials*************/

        //node = xmlDoc.SelectSingleNode("Scene/Materials");

        //XmlNodeList materialNodes = node.SelectNodes("Material");

        //foreach (XmlNode materialNode in materialNodes)
        //{
        //    XmlNode tempNode;
        //    MaterialData newMaterialData = new MaterialData();
        //    newMaterialData._ID = int.Parse(materialNode.Attributes["id"].Value);

        //    vectorData = materialNode.SelectSingleNode("AmbientReflectance").InnerText.Split(' ');
        //    newMaterialData._AmbientReflectense = new Vector3(float.Parse(vectorData[0]),
        //                                          float.Parse(vectorData[1]),
        //                                          float.Parse(vectorData[2])
        //                                          );

        //    vectorData = materialNode.SelectSingleNode("DiffuseReflectance").InnerText.Split(' ');
        //    newMaterialData._DiffuseReflectense = new Vector3(float.Parse(vectorData[0]),
        //                                          float.Parse(vectorData[1]),
        //                                          float.Parse(vectorData[2])
        //                                          );

        //    vectorData = materialNode.SelectSingleNode("SpecularReflectance").InnerText.Split(' ');
        //    newMaterialData._SpecularReflectense = new Vector3(float.Parse(vectorData[0]),
        //                                          float.Parse(vectorData[1]),
        //                                          float.Parse(vectorData[2])
        //                                          );

        //    tempNode = materialNode.SelectSingleNode("MirrorReflectance");
        //    if (tempNode != null)
        //    {
        //        vectorData = tempNode.InnerText.Split(' ');
        //        newMaterialData._MirrorReflectance = new Vector3(float.Parse(vectorData[0]),
        //                                              float.Parse(vectorData[1]),
        //                                              float.Parse(vectorData[2])
        //                                              );
        //    }

        //    tempNode = materialNode.SelectSingleNode("Transparency");
        //    if (tempNode != null)
        //    {
        //        vectorData = tempNode.InnerText.Split(' ');
        //        newMaterialData._Transparency = new Vector3(float.Parse(vectorData[0]),
        //                                              float.Parse(vectorData[1]),
        //                                              float.Parse(vectorData[2])
        //                                              );
        //    }

        //    tempNode = materialNode.SelectSingleNode("PhongExponent");
        //    if (tempNode != null)
        //    {
        //        newMaterialData._PhongExponent = float.Parse(tempNode.InnerText);
        //    }

        //    tempNode = materialNode.SelectSingleNode("RefractionIndex");
        //    if (tempNode != null)
        //    {
        //        newMaterialData._RefractionIndex = float.Parse(tempNode.InnerText);
        //    }

        //    _SceneData._MaterialDatas.Add(newMaterialData);
        //    _SceneData._MaterialCount++;
        //}

        ///**************VertexData*************/

        //_SceneData._SizeOfVertexList = 0;
        //_SceneData._VertexList.Clear();

        //node = xmlDoc.SelectSingleNode("Scene/VertexData");
        //string[] vectorDataList = node.InnerText.Split('\n');

        //foreach (string vectorDataSingle in vectorDataList)
        //{
        //    string trimmedString = vectorDataSingle.Trim();
        //    if (trimmedString.Length < 2)
        //    {
        //        continue;
        //    }

        //    vectorData = trimmedString.Split(' ');
        //    Vector3 newVector = new Vector3(float.Parse(vectorData[0]),
        //                                    float.Parse(vectorData[1]),
        //                                    float.Parse(vectorData[2]));
        //    _SceneData._VertexList.Add(newVector);
        //    _SceneData._SizeOfVertexList++;
        //}

        ///**************Objects*************/

        //_SceneData._SizeOfTriangleList = 0;
        //_SceneData._MeshCount = 0;
        //_SceneData._SphereCount = 0;

        //_SceneData._Spheres.Clear();
        //_SceneData._MeshDataList.Clear();
        //_SceneData._TriangleList.Clear();

        //node = xmlDoc.SelectSingleNode("Scene/Objects");

        //XmlNodeList meshNodes = node.SelectNodes("Mesh");

        //foreach (XmlNode meshNode in meshNodes)
        //{
        //    MeshData newMeshData = new MeshData();

        //    newMeshData._MeshID = int.Parse(meshNode.Attributes["id"].Value);
        //    newMeshData._MaterialID = int.Parse(meshNode.SelectSingleNode("Material").InnerText);

        //    newMeshData._VertexIndexStart = 0;
        //    newMeshData._TriangleIndexStart = _SceneData._SizeOfTriangleList;

        //    newMeshData._Scale = 1;

        //    string[] facesDataList = meshNode.SelectSingleNode("Faces").InnerText.Split('\n');

        //    foreach (string faceDataSingle in facesDataList)
        //    {
        //        string trimmedString = faceDataSingle.Trim();
        //        if (trimmedString.Length < 2)
        //        {
        //            continue;
        //        }

        //        vectorData = trimmedString.Split(' ');
        //        Vector3 newVector = new Vector3(int.Parse(vectorData[0]) - 1,
        //                                        int.Parse(vectorData[1]) - 1,
        //                                        int.Parse(vectorData[2]) - 1
        //                                        );

        //        _SceneData._TriangleList.Add(newVector);
        //        _SceneData._SizeOfTriangleList++;
        //    }

        //    newMeshData._TriangleIndexEnd = _SceneData._SizeOfTriangleList;

        //    _SceneData._MeshDataList.Add(newMeshData);
        //    _SceneData._MeshCount++;
        //}

        //XmlNodeList triangleNodes = node.SelectNodes("Triangle");

        //foreach (XmlNode triangleNode in triangleNodes)
        //{
        //    MeshData newMeshData = new MeshData();

        //    newMeshData._MeshID = int.Parse(triangleNode.Attributes["id"].Value);
        //    newMeshData._MaterialID = int.Parse(triangleNode.SelectSingleNode("Material").InnerText);

        //    newMeshData._VertexIndexStart = 0;
        //    newMeshData._TriangleIndexStart = _SceneData._SizeOfTriangleList;

        //    string[] facesDataList = triangleNode.SelectSingleNode("Indices").InnerText.Split('\n');

        //    foreach (string faceDataSingle in facesDataList)
        //    {
        //        string trimmedString = faceDataSingle.Trim();
        //        if (trimmedString.Length < 2)
        //        {
        //            continue;
        //        }

        //        vectorData = trimmedString.Split(' ');
        //        Vector3 newVector = new Vector3(int.Parse(vectorData[0]) - 1,
        //                                        int.Parse(vectorData[1]) - 1,
        //                                        int.Parse(vectorData[2]) - 1
        //                                        );

        //        _SceneData._TriangleList.Add(newVector);
        //        _SceneData._SizeOfTriangleList++;
        //    }

        //    newMeshData._TriangleIndexEnd = _SceneData._SizeOfTriangleList;

        //    _SceneData._MeshDataList.Add(newMeshData);
        //    _SceneData._MeshCount++;
        //}

        //XmlNodeList sphereNodes = node.SelectNodes("Sphere");

        //foreach (XmlNode sphereNode in sphereNodes)
        //{
        //    Sphere newSphere = new Sphere();

        //    newSphere._SphereID = int.Parse(sphereNode.Attributes["id"].Value);
        //    newSphere._MaterialID = int.Parse(sphereNode.SelectSingleNode("Material").InnerText);

        //    int index = int.Parse(sphereNode.SelectSingleNode("Center").InnerText);
        //    newSphere._Position = _SceneData._VertexList[index - 1];

        //    newSphere._Radius = float.Parse(sphereNode.SelectSingleNode("Radius").InnerText);

        //    _SceneData._Spheres.Add(newSphere);
        //    _SceneData._SphereCount++;
        //}
    }
}