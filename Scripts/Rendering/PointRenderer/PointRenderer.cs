using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Mathematics;

public class PointRenderer : MonoBehaviour
{
    public Debugger manager;
    public SphereCameraMovement scm;
    public Camera mainCamera;
    public RawImage image;

    public bool loadPlanetData = false;
    public bool partitionPoints = false;
    public bool updateTexture = false;


    private ComputeShader computeShader;
    private RenderTexture outputTexture;
    public float pointSize = 0.005f;
    public float cellSize = 0.2f;
    private RenderingPointObject[] pointObjects;
    private List<List<int>> cells;
    private Bounds gridBounds;

    public int textureWidth;
    public int textureHeight;

    private ComputeBuffer pointsBuffer;
    private ComputeBuffer cellBuffer;
    private ComputeBuffer indicesBuffer;
    private int[] flatIndicesArray;
    private int2[] renderingCells;

    public Color backgroundColor;
    public Color unitSphereColor;

    private int kernelHandle;

    public bool setRayOrigin = false;
    public Vector3 rayCellOrigin;
    public Vector3 rayCellDirection;
    public void Update()
    {
        if(setRayOrigin)
        {
            setRayOrigin = false;
            rayCellOrigin = mainCamera.transform.position;
            rayCellDirection = mainCamera.transform.forward;
        }
        if(loadPlanetData)
        {
            loadPlanetData = false;

            ConvertToPointObjects(manager.planetData);
            currentData = GetCurrentData();
        }
        if (partitionPoints)
        {
            partitionPoints = false;
            PartitionPoints();
        }
        
        if (updateTexture)
        {
            //updateTexture = false;  // Only update once unless requested again
            ReloadData();
            RenderPoints();

            image.texture = outputTexture;
        }
        
    }

    public void ConvertToPointObjects(PlanetData planet)
    {
        int amount = planet.tesselation.points.Length;
        pointObjects = new RenderingPointObject[amount];
        for(int i = 0; i < amount; i++)
        {
            RenderingPointObject p = new RenderingPointObject(planet.tesselation.points[i].p.vector3, pointSize, GetColorFromSamplePoint(planet, i));
            pointObjects[i] = p;
        }
        Debug.Log("amount: " + amount);
    }

    public Color GetColorFromSamplePoint(PlanetData planet, int pointIndex)
    {
        float seaDepth = (float)planet.tesselation.points[pointIndex].data.column.hydrosphere.thickness;
        if(seaDepth > 0)
        {
            seaDepth = Mathf.Clamp(seaDepth, 0, 4000f);
            seaDepth = seaDepth / 4000f;
            return new Color(0, 0, seaDepth, 1f);
        }  
        return new Color(0, 1f, 0, 1f);
    }

    public void RenderPoints()
    {
        computeShader.SetFloat("pointSize", pointSize);
        outputTexture = new RenderTexture(textureWidth, textureHeight, 0, RenderTextureFormat.ARGBFloat);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();
        computeShader.SetTexture(kernelHandle, "Result", outputTexture);
        // Dispatch the compute shader
        int threadGroupsX = Mathf.CeilToInt((float)textureWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt((float)textureHeight / 8.0f);
        computeShader.Dispatch(kernelHandle, threadGroupsX, threadGroupsY, 1);
    }

    public void PartitionPoints()
    {
        cells = new List<List<int>>();
        gridBounds = new Bounds(Vector3.zero, Vector3.zero);
        // Partition the points into cells
        ObjectPartitioner.PartitionPoints(pointObjects, cellSize, ref cells, ref gridBounds);
        List<int> flatIndicesList = new List<int>();
        renderingCells = new int2[cells.Count];
        for(int i = 0; i < cells.Count; i++)
        {
            int startIndex = flatIndicesList.Count;
            int amount = cells[i].Count;
            int endIndex = startIndex + amount;

            flatIndicesList.AddRange(cells[i]);
            renderingCells[i] = new int2(startIndex, endIndex);
        }
        flatIndicesArray = flatIndicesList.ToArray();
        // Initialize and set the point buffer
        if (pointsBuffer != null)
        {
            pointsBuffer.Release(); // Release old buffer before re-allocating
        }
        pointsBuffer = new ComputeBuffer(pointObjects.Length, sizeof(float) * 8); // Position + radius + color (assuming color is a float4)
        pointsBuffer.SetData(pointObjects);

        
        if (cellBuffer != null)
        {
            cellBuffer.Release();
        }
        cellBuffer = new ComputeBuffer(renderingCells.Length, sizeof(int) * 2); // startIndex + pointCount
        cellBuffer.SetData(renderingCells);

        if (indicesBuffer != null)
        {
            indicesBuffer.Release();
        }
        indicesBuffer = new ComputeBuffer(flatIndicesArray.Length, sizeof(int));
        indicesBuffer.SetData(flatIndicesArray);

        computeShader = StaticResourcesLoader.PointRenderer;
        kernelHandle = computeShader.FindKernel("PointRenderer");
        // Set the compute shader parameters
        computeShader.SetBuffer(kernelHandle, "objects", pointsBuffer);
        computeShader.SetBuffer(kernelHandle, "indices", indicesBuffer);
        computeShader.SetBuffer(kernelHandle, "cells", cellBuffer);

        computeShader.SetVector("boundsMin", gridBounds.min);
        computeShader.SetVector("boundsMax", gridBounds.max);
        computeShader.SetFloat("gridSize", cellSize);
    }

    private RenderingData currentData;
    private RenderingData previousData;
    public void ReloadData()
    {
        previousData = currentData;
        currentData = GetCurrentData();
        

        

        if(RenderingData.ScreenHasChanged(currentData, previousData))
            ReloadScreenData();

        if(RenderingData.CameraHasChanged(currentData, previousData))
            ReloadCameraData();

        
        // Set the output texture to the compute shader
        computeShader.SetVector("rayCellOrigin", rayCellOrigin);
        computeShader.SetVector("rayCellDirection", rayCellDirection.normalized);
    }

    private RenderingData GetCurrentData()
    {
        return new RenderingData(Screen.width, Screen.height, mainCamera.worldToCameraMatrix, mainCamera.projectionMatrix, (mainCamera.worldToCameraMatrix).inverse, (mainCamera.projectionMatrix).inverse);
    }

    



    private void ReloadScreenData()
    {
        // Get texture dimensions
        textureWidth = currentData.textureWidth;
        textureHeight = currentData.textureHeight;

        // Set up or recreate the output texture if necessary
        if (outputTexture != null)
        {
            outputTexture.Release(); // Release if it already exists
        }
        
        outputTexture = new RenderTexture(textureWidth, textureHeight, 0, RenderTextureFormat.ARGBFloat);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();

        // Set the output texture to the compute shader
        computeShader.SetTexture(kernelHandle, "Result", outputTexture);
    }

    private void ReloadCameraData()
    {
        // Calculate and set matrices for the compute shader
        Matrix4x4 viewMatrix = currentData.viewMatrix;
        Matrix4x4 projectionMatrix = currentData.projectionMatrix;
        Matrix4x4 inverseViewMatrix = currentData.inverseViewMatrix;
        Matrix4x4 inverseProjectionMatrix = currentData.inverseProjectionMatrix;

        computeShader.SetMatrix("viewMatrix", viewMatrix);
        computeShader.SetMatrix("projectionMatrix", projectionMatrix);
        computeShader.SetMatrix("inverseViewMatrix", inverseViewMatrix);
        computeShader.SetMatrix("inverseProjectionMatrix", inverseProjectionMatrix);

        computeShader.SetVector("cameraPosition", mainCamera.transform.position);
        computeShader.SetVector("resolution", new Vector2(textureWidth, textureHeight));

        // Set color values
        computeShader.SetVector("backgroundColor", backgroundColor);
        computeShader.SetVector("unitSphereColor", unitSphereColor);

        float planeHeight = scm.distance * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float planeWidth = planeHeight * mainCamera.aspect;
        computeShader.SetVector("ViewParams", new Vector3(planeWidth, planeHeight, scm.distance));
    }

    void OnDestroy()
    {
        // Ensure all buffers are properly released when the object is destroyed
        if (pointsBuffer != null)
        {
            pointsBuffer.Release();
        }
        if (cellBuffer != null)
        {
            cellBuffer.Release();
        }
        if (outputTexture != null)
        {
            outputTexture.Release();
        }
    }

    private struct RenderingData
    {
        public int textureWidth;
        public int textureHeight;

        public Matrix4x4 viewMatrix;
        public Matrix4x4 projectionMatrix;
        public Matrix4x4 inverseViewMatrix;
        public Matrix4x4 inverseProjectionMatrix;

        public RenderingData(int _textureWidth, int _textureHeight, Matrix4x4 _viewMatrix, Matrix4x4 _projectionMatrix, Matrix4x4 _inverseViewMatrix, Matrix4x4 _inverseProjectionMatrix)
        {
            textureWidth = _textureWidth;
            textureHeight = _textureHeight;
            viewMatrix = _viewMatrix;
            projectionMatrix = _projectionMatrix;
            inverseViewMatrix = _inverseViewMatrix;
            inverseProjectionMatrix = _inverseProjectionMatrix;
        }

        public static bool ScreenHasChanged(RenderingData a, RenderingData b)
        {
            if(a.textureWidth != b.textureWidth || a.textureHeight != b.textureHeight)
                return true;
            return false;
        }

        public static bool CameraHasChanged(RenderingData a, RenderingData b)
        {
            if(a.viewMatrix != b.viewMatrix)
                return true;
            if(a.projectionMatrix != b.projectionMatrix)
                return true;

            //Checking if the inverses have changed is unnecessary
            /*
                if(a.inverseViewMatrix != b.inverseViewMatrix)
                    return true;
                if(a.inverseProjectionMatrix != b.inverseProjectionMatrix)
                    return true;
            */
            return false;
        }
    }
}
