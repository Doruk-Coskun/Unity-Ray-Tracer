using System;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class SerializableVector3
{
    [XmlText]
    public string _data;

    public Vector3 Vector3
    {
        get
        {
            string[] strArr = _data.Split(' ');
            return new Vector3(float.Parse(strArr[0]), 
                               float.Parse(strArr[1]), 
                               float.Parse(strArr[2]));
        }
    }

    public SerializableVector3() { }
    //public SerializableVector3(Vector3 vector)
    //{
    //    double val;
    //    X = double.TryParse(vector.x.ToString(), out val) ? val : 0.0;
    //    Y = double.TryParse(vector.y.ToString(), out val) ? val : 0.0;
    //    Z = double.TryParse(vector.z.ToString(), out val) ? val : 0.0;
    //}
}