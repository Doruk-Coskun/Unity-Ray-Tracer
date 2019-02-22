using UnityEngine;

public class CameraData
{
    public int _ID;

    public Vector3 _Position = Vector3.zero;
    public Vector3 _Gaze = Vector3.zero;
    public Vector3 _Up = Vector3.zero;

    public Vector4 _NearPlane;
    public float _NearDistance = 0;

    public Vector2 _ImageResolution;
    public string _ImageName;
}