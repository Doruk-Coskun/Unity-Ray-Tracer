using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public static class SceneXMLParser
{
    public static void Deseralize(TextAsset XMLFile, ref SceneData _SceneData)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(SceneData));
        byte[] byteArray = Encoding.UTF8.GetBytes(XMLFile.text);
        MemoryStream stream = new MemoryStream(byteArray);
        object obj = xmlSerializer.Deserialize(stream);
        _SceneData = (SceneData)obj;
    }


    public static void LoadFromXML(TextAsset XMLFile, ref SceneData _SceneData)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(XMLFile.text);

        string[] vectorData;

        /**************MaxRecursionDepth*************/

        XmlNode node = xmlDoc.SelectSingleNode("Scene/MaxRecursionDepth");
        if (node != null)
        {
            _SceneData._MaxRecursionDepth = int.Parse(node.InnerText);
        }

        /**************BackgroundColor*************/

        node = xmlDoc.SelectSingleNode("Scene/BackgroundColor");
        vectorData = node.InnerText.Split(' ');

        _SceneData._BackGroundColor = new Vector3(float.Parse(vectorData[0]),
                                                 float.Parse(vectorData[1]),
                                                 float.Parse(vectorData[2])
                                                );

        /**************Cameras*************/

        // TODO: Need rework for multiple cameras
        node = xmlDoc.SelectSingleNode("Scene/Cameras").FirstChild;

        CameraData newCameraData = new CameraData();
        newCameraData._ID = int.Parse(node.Attributes["id"].Value);

        vectorData = node.SelectSingleNode("Position").InnerText.Split(' ');
        newCameraData._Position = new Vector3(float.Parse(vectorData[0]),
                                              float.Parse(vectorData[1]),
                                              float.Parse(vectorData[2])
                                              );

        vectorData = node.SelectSingleNode("Gaze").InnerText.Split(' ');
        newCameraData._Gaze = new Vector3(float.Parse(vectorData[0]),
                                              float.Parse(vectorData[1]),
                                              float.Parse(vectorData[2])
                                              );

        vectorData = node.SelectSingleNode("Up").InnerText.Split(' ');
        newCameraData._Up = new Vector3(float.Parse(vectorData[0]),
                                              float.Parse(vectorData[1]),
                                              float.Parse(vectorData[2])
                                              );

        vectorData = node.SelectSingleNode("NearPlane").InnerText.Split(' ');
        newCameraData._NearPlane = new Vector4(float.Parse(vectorData[0]),
                                              float.Parse(vectorData[1]),
                                              float.Parse(vectorData[2]),
                                              float.Parse(vectorData[3])
                                              );

        newCameraData._NearDistance = int.Parse(node.SelectSingleNode("NearDistance").InnerText);

        vectorData = node.SelectSingleNode("ImageResolution").InnerText.Split(' ');
        newCameraData._ImageResolution = new Vector3(float.Parse(vectorData[0]),
                                              float.Parse(vectorData[1])
                                              );

        newCameraData._ImageName = node.SelectSingleNode("ImageName").InnerText;

        _SceneData._CameraDatas.Add(newCameraData);

        /**************Lights*************/

        _SceneData._PointLightCount = 0;
        _SceneData._PointLightDatas.Clear();

        node = xmlDoc.SelectSingleNode("Scene/Lights");

        vectorData = node.SelectSingleNode("AmbientLight").InnerText.Split(' ');
        _SceneData._AmbientLight = new Vector3(float.Parse(vectorData[0]),
                                              float.Parse(vectorData[1]),
                                              float.Parse(vectorData[2])
                                              );
                                             
        XmlNodeList lightNodes = node.SelectNodes("PointLight");
        foreach (XmlNode lightNode in lightNodes)
        {
            PointLightData newPLData = new PointLightData();
            newPLData._ID = int.Parse(lightNode.Attributes["id"].Value);

            vectorData = lightNode.SelectSingleNode("Position").InnerText.Split(' ');
            newPLData._Position = new Vector3(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1]),
                                                  float.Parse(vectorData[2])
                                                  );

            vectorData = lightNode.SelectSingleNode("Intensity").InnerText.Split(' ');
            newPLData._Intensity = new Vector3(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1]),
                                                  float.Parse(vectorData[2])
                                                  );

            _SceneData._PointLightCount++;
            _SceneData._PointLightDatas.Add(newPLData);
        }

        /**************Materials*************/

        node = xmlDoc.SelectSingleNode("Scene/Materials");

        XmlNodeList materialNodes = node.SelectNodes("Material");

        foreach (XmlNode materialNode in materialNodes)
        {
            XmlNode tempNode;
            MaterialData newMaterialData = new MaterialData();
            newMaterialData._ID = int.Parse(materialNode.Attributes["id"].Value);

            vectorData = materialNode.SelectSingleNode("AmbientReflectance").InnerText.Split(' ');
            newMaterialData._AmbientReflectense = new Vector3(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1]),
                                                  float.Parse(vectorData[2])
                                                  );

            vectorData = materialNode.SelectSingleNode("DiffuseReflectance").InnerText.Split(' ');
            newMaterialData._DiffuseReflectense = new Vector3(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1]),
                                                  float.Parse(vectorData[2])
                                                  );

            vectorData = materialNode.SelectSingleNode("SpecularReflectance").InnerText.Split(' ');
            newMaterialData._SpecularReflectense = new Vector3(float.Parse(vectorData[0]),
                                                  float.Parse(vectorData[1]),
                                                  float.Parse(vectorData[2])
                                                  );

            tempNode = materialNode.SelectSingleNode("MirrorReflectance");
            if (tempNode != null)
            {
                vectorData = tempNode.InnerText.Split(' ');
                newMaterialData._MirrorReflectance = new Vector3(float.Parse(vectorData[0]),
                                                      float.Parse(vectorData[1]),
                                                      float.Parse(vectorData[2])
                                                      );
            }

            tempNode = materialNode.SelectSingleNode("Transparency");
            if (tempNode != null)
            {
                vectorData = tempNode.InnerText.Split(' ');
                newMaterialData._Transparency = new Vector3(float.Parse(vectorData[0]),
                                                      float.Parse(vectorData[1]),
                                                      float.Parse(vectorData[2])
                                                      );
            }

            tempNode = materialNode.SelectSingleNode("PhongExponent");
            if (tempNode != null)
            {
                newMaterialData._PhongExponent = float.Parse(tempNode.InnerText);
            }

            tempNode = materialNode.SelectSingleNode("RefractionIndex");
            if (tempNode != null)
            {
                newMaterialData._RefractionIndex = float.Parse(tempNode.InnerText);
            }

            _SceneData._MaterialDatas.Add(newMaterialData);
            _SceneData._MaterialCount++;
        }

        /**************VertexData*************/

        _SceneData._SizeOfVertexList = 0;
        _SceneData._VertexList.Clear();

        node = xmlDoc.SelectSingleNode("Scene/VertexData");
        string[] vectorDataList = node.InnerText.Split('\n');

        foreach (string vectorDataSingle in vectorDataList)
        {
            string trimmedString = vectorDataSingle.Trim();
            if (trimmedString.Length < 2)
            {
                continue;
            }

            vectorData = trimmedString.Split(' ');
            Vector3 newVector = new Vector3(float.Parse(vectorData[0]),
                                            float.Parse(vectorData[1]),
                                            float.Parse(vectorData[2]));
            _SceneData._VertexList.Add(newVector);
            _SceneData._SizeOfVertexList++;
        }

        /**************Objects*************/

        _SceneData._SizeOfTriangleList = 0;
        _SceneData._MeshCount = 0;
        _SceneData._SphereCount = 0;

        _SceneData._Spheres.Clear();
        _SceneData._MeshDataList.Clear();
        _SceneData._TriangleList.Clear();

        node = xmlDoc.SelectSingleNode("Scene/Objects");

        XmlNodeList meshNodes = node.SelectNodes("Mesh");

        foreach (XmlNode meshNode in meshNodes)
        {
            MeshData newMeshData = new MeshData();

            newMeshData._MeshID = int.Parse(meshNode.Attributes["id"].Value);
            newMeshData._MaterialID = int.Parse(meshNode.SelectSingleNode("Material").InnerText);

            newMeshData._VertexIndexStart = 0;
            newMeshData._TriangleIndexStart = _SceneData._SizeOfTriangleList;

            newMeshData._Scale = 1;

            string[] facesDataList = meshNode.SelectSingleNode("Faces").InnerText.Split('\n');

            foreach (string faceDataSingle in facesDataList)
            {
                string trimmedString = faceDataSingle.Trim();
                if (trimmedString.Length < 2)
                {
                    continue;
                }

                vectorData = trimmedString.Split(' ');
                Vector3 newVector = new Vector3(int.Parse(vectorData[0]) - 1,
                                                int.Parse(vectorData[1]) - 1,
                                                int.Parse(vectorData[2]) - 1
                                                );

                _SceneData._TriangleList.Add(newVector);
                _SceneData._SizeOfTriangleList++;
            }

            newMeshData._TriangleIndexEnd = _SceneData._SizeOfTriangleList;

            _SceneData._MeshDataList.Add(newMeshData);
            _SceneData._MeshCount++;
        }

        XmlNodeList triangleNodes = node.SelectNodes("Triangle");

        foreach (XmlNode triangleNode in triangleNodes)
        {
            MeshData newMeshData = new MeshData();

            newMeshData._MeshID = int.Parse(triangleNode.Attributes["id"].Value);
            newMeshData._MaterialID = int.Parse(triangleNode.SelectSingleNode("Material").InnerText);

            newMeshData._VertexIndexStart = 0;
            newMeshData._TriangleIndexStart = _SceneData._SizeOfTriangleList;

            string[] facesDataList = triangleNode.SelectSingleNode("Indices").InnerText.Split('\n');

            foreach (string faceDataSingle in facesDataList)
            {
                string trimmedString = faceDataSingle.Trim();
                if (trimmedString.Length < 2)
                {
                    continue;
                }

                vectorData = trimmedString.Split(' ');
                Vector3 newVector = new Vector3(int.Parse(vectorData[0]) - 1,
                                                int.Parse(vectorData[1]) - 1,
                                                int.Parse(vectorData[2]) - 1
                                                );

                _SceneData._TriangleList.Add(newVector);
                _SceneData._SizeOfTriangleList++;
            }

            newMeshData._TriangleIndexEnd = _SceneData._SizeOfTriangleList;

            _SceneData._MeshDataList.Add(newMeshData);
            _SceneData._MeshCount++;
        }

        XmlNodeList sphereNodes = node.SelectNodes("Sphere");

        foreach (XmlNode sphereNode in sphereNodes)
        {
            Sphere newSphere = new Sphere();

            newSphere._SphereID = int.Parse(sphereNode.Attributes["id"].Value);
            newSphere._MaterialID = int.Parse(sphereNode.SelectSingleNode("Material").InnerText);

            int index = int.Parse(sphereNode.SelectSingleNode("Center").InnerText);
            newSphere._Position = _SceneData._VertexList[index - 1];

            newSphere._Radius = float.Parse(sphereNode.SelectSingleNode("Radius").InnerText);

            _SceneData._Spheres.Add(newSphere);
            _SceneData._SphereCount++;
        }
    }
}
