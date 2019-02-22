using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class SceneXMLParser : MonoBehaviour
{
    public int _MaxRecursionDepth;
    public Vector3 _BackGroundColor;

    public CameraData[] _CameraDatas;

    public Vector3 _AmbientLight;
    public PointLightData[] _PointLightDatas;

    public MaterialData[] _MaterialDatas;
}
