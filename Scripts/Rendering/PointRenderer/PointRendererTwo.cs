using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using System.Collections.Generic;
public class PointRendererTwo : MonoBehaviour
{
    public Camera mainCamera;

    public Debugger manager;
    public SphereCameraMovement scm;
    public ComputeShader computeShader;
    public RawImage image;
    private RenderTexture outputTexture;
    private int textureWidth;
    private int textureHeight;

    public Vector3 boundsMin;
    public Vector3 boundsMax;
    public float gridSize;

    public Color backgroundColor;
    public Color cellBaseColor;
    public Color sphereColor;
    public float sphereRadius;

    public bool updateTexture = false;
    public bool partitionPoints = false;

    private int kernelHandle = 0;
    
    private ComputeBuffer pointsBuffer;
    private ComputeBuffer cellBuffer;
    private ComputeBuffer indicesBuffer;
    private int[] flatIndicesArray;
    private int2[] renderingCells;
    private RenderingPointObject[] pointObjects;
    private List<List<int>> cells;
    private Bounds gridBounds;
    public float pointSize = 0.005f;
    public bool renderSphere = false;
    public void Update()
    {
        
        ReloadData();
        PartitionPoints();
        RenderCells();
    }

    private void PartitionPoints()
    {
        if(!partitionPoints)
            return;
        partitionPoints = false;
        ConvertToPointObjects(manager.planetData);

        cells = new List<List<int>>();
        gridBounds = new Bounds(Vector3.zero, Vector3.zero);
        // Partition the points into cells
        ObjectPartitioner.PartitionPoints(pointObjects, gridSize, ref cells, ref gridBounds);
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

        //computeShader = StaticResourcesLoader.PointRenderer;
        kernelHandle = computeShader.FindKernel("PointRenderer");
        // Set the compute shader parameters
        computeShader.SetBuffer(kernelHandle, "objects", pointsBuffer);
        computeShader.SetBuffer(kernelHandle, "indices", indicesBuffer);
        computeShader.SetBuffer(kernelHandle, "cells", cellBuffer);
        computeShader.SetInt("cellsLength", renderingCells.Length);
    }

    private void ConvertToPointObjects(PlanetData planet)
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
    private void RenderCells()
    {
        if(!updateTexture)
            return;

        // Dispatch the compute shader
        int threadGroupsX = Mathf.CeilToInt((float)textureWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt((float)textureHeight / 8.0f);
        computeShader.Dispatch(kernelHandle, threadGroupsX, threadGroupsY, 1);

        image.texture = outputTexture;
    }

    private void ReloadData()
    {
        kernelHandle = computeShader.FindKernel("PointRenderer");
        ReloadScreenData();
        ReloadCameraData();
        ReloadCellData();
    }

    private void ReloadScreenData()
    {
        // Get texture dimensions
        textureWidth = Screen.width;
        textureHeight = Screen.height;

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

    public void ReloadCameraData()
    {
        Matrix4x4 viewMatrix = mainCamera.worldToCameraMatrix;
        Matrix4x4 projectionMatrix = mainCamera.projectionMatrix;
        Matrix4x4 inverseViewMatrix = mainCamera.worldToCameraMatrix.inverse;
        Matrix4x4 inverseProjectionMatrix = mainCamera.projectionMatrix.inverse;

        computeShader.SetMatrix("viewMatrix", viewMatrix);
        computeShader.SetMatrix("projectionMatrix", projectionMatrix);
        computeShader.SetMatrix("inverseViewMatrix", inverseViewMatrix);
        computeShader.SetMatrix("inverseProjectionMatrix", inverseProjectionMatrix);

        computeShader.SetVector("cameraPosition", mainCamera.transform.position);
        computeShader.SetVector("resolution", new Vector2(textureWidth, textureHeight));

        // Set color values

        float planeHeight = scm.distance * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float planeWidth = planeHeight * mainCamera.aspect;
        computeShader.SetVector("ViewParams", new Vector3(planeWidth, planeHeight, scm.distance));
    }

    public void ReloadCellData()
    {
        computeShader.SetVector("backgroundColor", backgroundColor);
        computeShader.SetVector("cellBaseColor", cellBaseColor);
        computeShader.SetVector("sphereColor", sphereColor);
        computeShader.SetFloat("sphereRadius", sphereRadius);

        boundsMin = ObjectPartitioner.SnapBoundsToGrid(boundsMin, gridSize);
        boundsMax = ObjectPartitioner.SnapBoundsToGrid(boundsMax, gridSize);
        
        computeShader.SetVector("boundsMin", boundsMin);
        computeShader.SetVector("boundsMax", boundsMax);

        computeShader.SetFloat("gridSize", gridSize);
        computeShader.SetInt("renderSphere", (renderSphere ? 1 : 0));
    }
}
