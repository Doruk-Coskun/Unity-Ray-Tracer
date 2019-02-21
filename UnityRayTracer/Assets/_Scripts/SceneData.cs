using UnityEngine;

public class SceneData
{
    public Matrix4x4 _CameraToWorldMatrix;
    public Matrix4x4 _CameraInverseProjectionMatrix;

    public Vector3 _DirectionalLightDirection;
    public Vector3 _DirectionalLightColor;
    public float _DirectionalLightIntensity;

    public int _SphereCount = 0;
    public int _PointLightCount = 0;
}
