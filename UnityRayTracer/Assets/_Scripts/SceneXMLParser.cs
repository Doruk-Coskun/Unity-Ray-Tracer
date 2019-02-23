using UnityEngine;
using System.Xml;

public static class SceneXMLParser
{
    public static void LoadFromXML(TextAsset XMLFile) 
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(XMLFile.text);

        string[] vectorData;

        /**************MaxRecursionDepth*************/

        XmlNode node = xmlDoc.SelectSingleNode("Scene/MaxRecursionDepth");
        SceneData._MaxRecursionDepth = int.Parse(node.InnerText);

        /**************BackgroundColor*************/

        node = xmlDoc.SelectSingleNode("Scene/BackgroundColor");
        vectorData = node.InnerText.Split(' ');

        SceneData._BackGroundColor = new Vector3(float.Parse(vectorData[0]),
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

        SceneData._CameraDatas.Add(newCameraData);

        /**************Lights*************/

        node = xmlDoc.SelectSingleNode("Scene/Lights");

        vectorData = node.SelectSingleNode("AmbientLight").InnerText.Split(' ');
        SceneData._AmbientLight = new Vector3(float.Parse(vectorData[0]),
                                              float.Parse(vectorData[1]),
                                              float.Parse(vectorData[2])
                                              );

        Debug.Log(SceneData._AmbientLight);

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

            Debug.Log(newPLData._Intensity);

            SceneData._PointLightCount++;
            SceneData._PointLightDatas.Add(newPLData);
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

            SceneData._MaterialDatas.Add(newMaterialData);
            SceneData._MaterialCount++;
        }

        /**************VertexData*************/

        node = xmlDoc.SelectSingleNode("Scene/VertexData");


    }
}
