using System.Collections.Generic;
using UnityEngine;

public class RayTracingMaster : MonoBehaviour
{
    private ComputeShader RayTracingShader;
    public Texture SkyboxTexture;

    private RenderTexture _target;

    private ComputeBuffer _sphereBuffer;
    private ComputeBuffer _MeshVertexBuffer;
    private ComputeBuffer _MeshIndexBuffer;
    private ComputeBuffer _MeshDataBuffer;
    private ComputeBuffer _MaterialBuffer;
    private ComputeBuffer _PointLightBuffer;
    private ComputeBuffer _MeshBBLBuffer;
    private ComputeBuffer _LinearBVHNodesBuffer;

    public void Awake()
    {
        RayTracingShader = (ComputeShader)Resources.Load("RayTracingShader");
    }

    public void SetUpScene()
    {
        //Debug.Log("SetUpScene");
        if (_sphereBuffer != null)
            _sphereBuffer.Dispose();

        // Assign to compute buffer
        // TODO: Mind the sizeof(Spheres).
        if (SceneParser._SceneData._SphereCount > 0)
        {
            _sphereBuffer = new ComputeBuffer(SceneParser._SceneData._SphereCount, 20);
            _sphereBuffer.SetData(SceneParser._SceneData._Spheres);
        }

        if (_MeshVertexBuffer != null)
        {
            _MeshVertexBuffer.Dispose();
            _MeshIndexBuffer.Dispose();
            _MeshDataBuffer.Dispose();
        }

        if (_MaterialBuffer != null)
        {
            _MaterialBuffer.Release();
        }

        if (_PointLightBuffer != null)
        {
            _PointLightBuffer.Dispose();
            _PointLightBuffer = null;
        }

        if (_LinearBVHNodesBuffer != null)
        {
            _LinearBVHNodesBuffer.Dispose();
            _LinearBVHNodesBuffer = null;
        }

        if (SceneParser._SceneData._MeshCount > 0)
        {
            _MeshVertexBuffer = new ComputeBuffer(SceneParser._SceneData._SizeOfVertexList, 12);
            _MeshVertexBuffer.SetData(SceneParser._SceneData._VertexList);

            _MeshIndexBuffer = new ComputeBuffer(SceneParser._SceneData._SizeOfTriangleList, 12);
            _MeshIndexBuffer.SetData(SceneParser._SceneData._TriangleList);

            _MeshDataBuffer = new ComputeBuffer(SceneParser._SceneData._MeshCount, 20);
            _MeshDataBuffer.SetData(SceneParser._SceneData._MeshDataList);
        }

        if (SceneParser._SceneData._MaterialCount > 0)
        {
            _MaterialBuffer = new ComputeBuffer(SceneParser._SceneData._MaterialCount, 68);
            _MaterialBuffer.SetData(SceneParser._SceneData._MaterialDatas);
        }

        if (SceneParser._SceneData._PointLightCount > 0)
        {
            _PointLightBuffer = new ComputeBuffer(SceneParser._SceneData._PointLightCount, 24);
            _PointLightBuffer.SetData(SceneParser._SceneData._PointLightDatas);
        }

        if (SceneParser._SceneData._SizeOfBVHNodeList > 0)
        {
            _LinearBVHNodesBuffer = new ComputeBuffer(SceneParser._SceneData._SizeOfBVHNodeList, 32);
            _LinearBVHNodesBuffer.SetData(SceneParser._SceneData._BVHNodeList);
        }

        SetShaderParameters();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Renderer(destination);
    }

    private void Renderer(RenderTexture destination)
    {
        InitRenderTexture();

        RayTracingShader.SetTexture(0, "Result", _target);

        int threadGroupsX = Mathf.CeilToInt(Screen.width / 4.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 4.0f);
        RayTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(_target, destination);
    }

    public void SetShaderParameters()
    {
        Debug.Log("SetShaderParameters");
        RayTracingShader.SetInt("_MaxRecursionDepth", SceneParser._SceneData._MaxRecursionDepth);

        if (SkyboxTexture != null)
        {
            RayTracingShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
        }

        RayTracingShader.SetVector("_BackgroundColor", SceneParser._SceneData._BackGroundColor);


        RayTracingShader.SetMatrix("_CameraToWorldMatrix", 
            SceneParser._SceneData._CameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraInverseProjectionMatrix", 
            SceneParser._SceneData._CameraInverseProjectionMatrix);


        RayTracingShader.SetVector("_DirectLightDirection", 
                   SceneParser._SceneData._DirectLightData._Direction);

        RayTracingShader.SetVector("_DirectLightIntensity",
                   SceneParser._SceneData._DirectLightData._Intensity);

        RayTracingShader.SetVector("_AmbientLight", SceneParser._SceneData._AmbientLight);

        RayTracingShader.SetInt("_PointLightCount", SceneParser._SceneData._PointLightCount);
        if (SceneParser._SceneData._PointLightCount > 0 && _PointLightBuffer != null)
        {
            RayTracingShader.SetBuffer(0, "_PointLightList", _PointLightBuffer);
        }


        RayTracingShader.SetInt("_MaterialCount", SceneParser._SceneData._MaterialCount);
        if (SceneParser._SceneData._MaterialCount > 0 && _MaterialBuffer != null)
        {
            RayTracingShader.SetBuffer(0, "_MaterialList", _MaterialBuffer);
        }


        RayTracingShader.SetInt("_SphereCount", SceneParser._SceneData._SphereCount);
        if (SceneParser._SceneData._SphereCount > 0 && _sphereBuffer != null)
        {
            RayTracingShader.SetBuffer(0, "_SphereList", _sphereBuffer);
        }

        RayTracingShader.SetInt("_MeshCount", SceneParser._SceneData._MeshCount);
        RayTracingShader.SetInt("_SizeOfVertexList", SceneParser._SceneData._SizeOfVertexList);
        RayTracingShader.SetInt("_SizeOfTriangleList", SceneParser._SceneData._SizeOfTriangleList);
        if (SceneParser._SceneData._MeshCount > 0 && _MeshDataBuffer != null)
        {
            RayTracingShader.SetBuffer(0, "_VertexList", _MeshVertexBuffer);
            RayTracingShader.SetBuffer(0, "_TriangleList", _MeshIndexBuffer);
            RayTracingShader.SetBuffer(0, "_MeshDataList", _MeshDataBuffer);
        }

        RayTracingShader.SetInt("_BVHNodeCount", SceneParser._SceneData._SizeOfBVHNodeList);
        if (SceneParser._SceneData._SizeOfBVHNodeList > 0 && _LinearBVHNodesBuffer != null)
        {
            RayTracingShader.SetBuffer(0, "_BVHNodeList", _LinearBVHNodesBuffer);
        }
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            if (_target != null)
            {
                _target.Release();
            }

            _target = new RenderTexture(Screen.width, Screen.height , 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);

            //_target = new RenderTexture((int)SceneParser._SceneData._CameraDatas[0]._ImageResolution.x,
            //                            (int)SceneParser._SceneData._CameraDatas[0]._ImageResolution.y,
            //                            0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;

            _target.Create();
        }
    }
}