using System.Collections.Generic;
using UnityEngine;

public class RayTracingMaster : MonoBehaviour
{
    public ComputeShader RayTracingShader;
    public Texture SkyboxTexture;

    private RenderTexture _target;

    private ComputeBuffer _sphereBuffer;
    private ComputeBuffer _MeshVertexBuffer;
    private ComputeBuffer _MeshIndexBuffer;
    private ComputeBuffer _MeshDataBuffer;

    public void SetUpScene()
    {
        if (_sphereBuffer != null)
            _sphereBuffer.Release();

        //// Assign to compute buffer
        //// TODO: Mind the sizeof(Spheres).
        //if (SceneParser._SceneData._SphereCount > 0)
        //{
        //    _sphereBuffer = new ComputeBuffer(SceneParser._SceneData._SphereCount, 40);
        //    _sphereBuffer.SetData(SceneParser._Spheres);
        //}

        //if (_MeshVertexBuffer != null)
        //{
        //    _MeshVertexBuffer.Release();
        //}

        //if (SceneParser._GeometryData._MeshCount > 0)
        //{
        //    _MeshVertexBuffer = new ComputeBuffer(SceneParser._GeometryData._SizeOfVertexList, 12);
        //    _MeshVertexBuffer.SetData(SceneParser._GeometryData._VertexList);

        //    _MeshIndexBuffer = new ComputeBuffer(SceneParser._GeometryData._SizeOfIndexList, 4);
        //    _MeshIndexBuffer.SetData(SceneParser._GeometryData._IndexList);

        //    _MeshDataBuffer = new ComputeBuffer(SceneParser._GeometryData._MeshCount, 52);
        //    _MeshDataBuffer.SetData(SceneParser._GeometryData._MeshDataList);
        //}
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters();
        Renderer(destination);
    }

    private void Renderer(RenderTexture destination)
    {
        InitRenderTexture();

        RayTracingShader.SetTexture(0, "Result", _target);

        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RayTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(_target, destination);
    }

    public void SetShaderParameters()
    {
        //RayTracingShader.SetMatrix("_CameraToWorldMatrix", SceneParser._SceneData._CameraToWorldMatrix);
        //RayTracingShader.SetMatrix("_CameraInverseProjectionMatrix", SceneParser._SceneData._CameraInverseProjectionMatrix);
        //RayTracingShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);

        //Vector3 d = SceneParser._SceneData._DirectionalLightDirection;
        //RayTracingShader.SetVector("_DirectionalLightDirection", 
        //           new Vector4(d.x, d.y, d.z, 1));

        //Vector3 l = SceneParser._SceneData._DirectionalLightColor;
        //RayTracingShader.SetVector("_DirectionalLightColor",
        //           new Vector4(l.x, l.y, l.z, SceneParser._SceneData._DirectionalLightIntensity));

        //RayTracingShader.SetInt("_SphereCount", SceneParser._SceneData._SphereCount);
        //if (SceneParser._SceneData._SphereCount > 0 && _sphereBuffer != null)
        //{
        //    RayTracingShader.SetBuffer(0, "_Spheres", _sphereBuffer);
        //}

        //RayTracingShader.SetInt("_MeshCount", SceneParser._GeometryData._MeshCount);
        //RayTracingShader.SetInt("_SizeOfVertexList", SceneParser._GeometryData._SizeOfVertexList);
        //RayTracingShader.SetInt("_SizeOfIndexList", SceneParser._GeometryData._SizeOfIndexList);
        //if (SceneParser._GeometryData._MeshCount > 0)
        //{
        //    RayTracingShader.SetBuffer(0, "_VertexList", _MeshVertexBuffer);
        //    RayTracingShader.SetBuffer(0, "_IndexList", _MeshIndexBuffer);
        //    RayTracingShader.SetBuffer(0, "_MeshDataList", _MeshDataBuffer);
        //}
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            if (_target != null)
            {
                _target.Release();
            }

            _target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }
}
