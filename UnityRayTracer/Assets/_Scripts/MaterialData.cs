using UnityEngine;

public class MaterialData
{
    public int _ID = 0;

    public Vector3 _AmbientReflectense = Vector3.zero;
    public Vector3 _DiffuseReflectense = Vector3.zero;
    public Vector3 _SpecularReflectense = Vector3.zero;
    public Vector3 _MirrorReflectance = Vector3.zero;
    public Vector3 _Transparency = Vector3.zero;

    public float _PhongExponent = 0;
    public float _refractionIndex = 0;
}