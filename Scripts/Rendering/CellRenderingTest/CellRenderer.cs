using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
public class CellRenderer : MonoBehaviour
{
    public Camera mainCamera;
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

    public Vector3 sunDirection;

    public Vector3 rayCellOrigin;
    public Vector3 rayCellDirection;
    public int rayCellIndex;
    private ComputeBuffer rayCellBuffer;
    public int3[] rayCellArray;
    private ComputeBuffer testOffsetBuffer;
    public Vector3[] testOffsetArray;

    public bool updateTexture = false;
    public bool setRayOrigin = false;
    public bool setSunDirection = false;
    public bool showAllCells = false;

    public int3 testCell;
    public int testCellIndex;

    public bool onlyShowGrid;
    public float gridWireFrameSize;

    private int kernelHandle = 0;
    

    public void Update()
    {
        ReloadData();
        RenderCells();
    }

    
    
    private void RenderCells()
    {
        if(!updateTexture)
            return;
        computeShader.SetInt("onlyShowGrid", (onlyShowGrid ? 1 : 0));
        computeShader.SetFloat("gridWireFrameSize", gridWireFrameSize);
        

        computeShader.SetInt("rayCellIndex", rayCellIndex);
        computeShader.SetInt("showAllCells", (showAllCells ? 1 : 0));

        computeShader.SetVector("testCell", new Vector3(testCell.x, testCell.y, testCell.z));
        computeShader.SetInt("testCellIndex", testCellIndex);
        // Dispatch the compute shader
        int threadGroupsX = Mathf.CeilToInt((float)textureWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt((float)textureHeight / 8.0f);
        computeShader.Dispatch(kernelHandle, threadGroupsX, threadGroupsY, 1);

        image.texture = outputTexture;
        rayCellBuffer.GetData(rayCellArray);
        testOffsetBuffer.GetData(testOffsetArray);
    }

    private void ReloadData()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            setRayOrigin = true;
        if(setRayOrigin)
        {
            setRayOrigin = false;
            rayCellOrigin = mainCamera.transform.position;
            rayCellDirection = mainCamera.transform.forward;
        }
        if(setSunDirection)
        {
            sunDirection = -1f * mainCamera.transform.position.normalized;
            setSunDirection = false;
        }
        kernelHandle = computeShader.FindKernel("CellRenderer");
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
        computeShader.SetVector("sunDirection", sunDirection);

        computeShader.SetVector("rayCellOrigin", rayCellOrigin);
        computeShader.SetVector("rayCellDirection", rayCellDirection);

        boundsMin = ObjectPartitioner.SnapBoundsToGrid(boundsMin, gridSize);
        boundsMax = ObjectPartitioner.SnapBoundsToGrid(boundsMax, gridSize);
        
        computeShader.SetVector("boundsMin", boundsMin);
        computeShader.SetVector("boundsMax", boundsMax);

        computeShader.SetFloat("gridSize", gridSize);

        if (rayCellBuffer != null)
        {
            rayCellBuffer.Release();
        }

        if (testOffsetBuffer != null)
        {
            testOffsetBuffer.Release();
        }

        int rayCellAmount = Mathf.RoundToInt(((boundsMax.x - boundsMin.x) / gridSize)) * 2;
        rayCellArray = new int3[rayCellAmount];
        rayCellBuffer = new ComputeBuffer(rayCellAmount, sizeof(int) * 3); // startIndex + pointCount
        rayCellBuffer.SetData(rayCellArray);
        computeShader.SetBuffer(kernelHandle, "rayCellBuffer", rayCellBuffer);
        computeShader.SetInt("rayCellAmount", rayCellAmount);

        testOffsetArray = new Vector3[3];
        testOffsetBuffer = new ComputeBuffer(3, sizeof(int) * 3); // startIndex + pointCount
        testOffsetBuffer.SetData(testOffsetArray);
        computeShader.SetBuffer(kernelHandle, "testOffsetBuffer", testOffsetBuffer);
    }
}
