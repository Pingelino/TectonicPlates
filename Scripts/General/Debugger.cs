using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Debugger : MonoBehaviour
{
    public bool generateAll = false;

    public bool generatePoints = false;
    public bool adjustPoints = false;
    public bool instantiatePoints = false;

    public bool generateTesselation = false;
    public bool instantiateIntersections = false;
    public bool adjustSamplePoints = false;

    public int amount;
    public int seed;
    public int iterations;
    public double adjustmentPerIteration;

    public Transform parent;
    public GameObject parentPrefab;
    public GameObject pointPrefab;
    public GameObject tensorPrefab;

    public Material matte;
    public Material blue;
    public Material yellow;

    public float pointSize;
    public float tensorSize;

    public Point[] points;
    public PlanetData planetData;
    //public TectonicTesselation tesselation;

    public PerlinNoiseSettings[] settings;
    private PerlinNoiseSettings[] prevSettings = new PerlinNoiseSettings[0];
    public bool updatePerlin = false;
    public bool updatePerlinAlways = false;

    public Texture2D texture;
    public RawImage image;
    public Color backgroundColor;

    public float resolution;
    private Camera mainCam;
    public bool useMap = false;
    public Vector2 mercatorClamp = new Vector2(-90f, 90f);

    public float planeClamp = 1f;
    public GameObject planeNormal;
    public Color noiseColor;
    PerlinNoise pNoise;

    public RectTransform seaLevelImage, mantleImage, lowerCrustImage, upperCrustImage;
    public float uiMultiplier = 1f;
    public int selectedPoint = -1;
    private int lastSelectedPoint = -1;
    public TectonicColumn selectedColumn;
    public int selectedElevation = 0;

    public Vector3 normalTest;
    public Vector3 normalTestNormalized;
    public Vector3 normalTestStretched;

    public int k;
    public float t;

    private void TestNormal()
    {
        normalTestNormalized = normalTest.normalized;

        float tt = (float)k / (normalTestNormalized.x + normalTestNormalized.y + normalTestNormalized.z);
        normalTestStretched = normalTestNormalized * tt;
    }
            
    void Update()
    {
        TestNormal();
        
        if(lastSelectedPoint != selectedPoint)
        {
            lastSelectedPoint = selectedPoint;

            if(selectedPoint < 0 || selectedPoint >= planetData.tesselation.points.Length)
                return;
            selectedColumn = planetData.tesselation.points[selectedPoint].data.column;
        }
        if(lastSelectedPoint != -1)
        {
            ApplyColumnToImages();
        }
        if(generateAll)
        {
            generateAll = false;
            GeneratePoints();
            Debug.Log("1");
            AdjustPoints();
            Debug.Log("2");
            GenerateTesselation();
            Debug.Log("3");
            //AdjustSamplePoints();
            //InstantiateIntersections();
        }
        if(updatePerlin)
        {
            if(updatePerlinAlways)
            {
                image.texture = PerlinNoiseGenerator.Generate3DPerlinNoiseTexture(settings, Screen.width, Screen.height, resolution, mainCam, backgroundColor, useMap, mercatorClamp, planeClamp, planeNormal.transform.position);
            }
            else if(PerlinNoiseSettings.HasChanged(prevSettings, settings))
            {
                pNoise = new PerlinNoise(settings);
                pNoise.FindApproximateMinMax(points);
                image.texture = PerlinNoiseGenerator.Generate3DPerlinNoiseTexture(settings, Screen.width, Screen.height, resolution, mainCam, backgroundColor, useMap, mercatorClamp, planeClamp, planeNormal.transform.position);

                
                prevSettings = settings.Select(s => new PerlinNoiseSettings(s.scale, s.octaves, s.persistence, s.lacunarity, s.offset, s.rotation, s.priority)).ToArray();
                
            }
        }
        if(generatePoints)
        {
            generatePoints = false;
            Point cross = Point.Cross(new Point(1.0, 0, 0), new Point(0, 1.0, 0));
            Debug.Log("(" + ((float)cross.x).ToString("F4") + ", " + ((float)cross.y).ToString("F4") + ", " + ((float)cross.z).ToString("F4") + ")");
            GeneratePoints();
        }
        if(adjustPoints)
        {
            adjustPoints = false;
            AdjustPoints();
        }
        if(instantiatePoints)
        {
            instantiatePoints = false;
            InstantiatePoints();
        }
        if(generateTesselation)
        {
            generateTesselation = false;
            GenerateTesselation();
        }
        if(instantiateIntersections)
        {
            instantiateIntersections = false;
            InstantiateIntersections();
        }
        if(adjustSamplePoints)
        {
            adjustSamplePoints = false;
            AdjustSamplePoints();
        }
    }

    public void GeneratePoints()
    {
        points = RandomPointGenerator.GenerateRandomPoints(amount, seed);
        
    }
    public void AdjustPoints()
    {
        Debug.Log("Start");
        points = PointAdjuster.AdjustPointsIterative(points, iterations, adjustmentPerIteration);
        //points = PointAdjuster.AdjustPointsIterative(points, iterations, adjustmentPerIteration);
    }
    public void AdjustSamplePoints()
    {
        planetData.tesselation.points = PointAdjuster.AdjustSamplePoints(planetData.tesselation.points, adjustmentPerIteration);
    }
    public void InstantiatePoints()
    {
        if(parent != null)
        {
            Destroy(parent.gameObject);
        }
        GameObject go = Instantiate(parentPrefab, Vector3.zero, Quaternion.identity);
        parent = go.transform;
        int pointIndex = 0;
        foreach(Point p in points)
        {
            GameObject pGo = Instantiate(pointPrefab, p.vector3, Quaternion.identity, parent);
            pGo.name = "" + pointIndex;
            pointIndex++;
        }
    }
    public void GenerateTesselation()
    {
        planetData = new PlanetData(points);

        pNoise = new PerlinNoise(settings);
        pNoise.FindApproximateMinMax(points);
        
        planetData.ApplyInitialIsostaticValues(pNoise);
        //planetData.ApplyInitialSeaLevel();
    }
    public void InstantiateIntersections()
    {
        if(parent != null)
        {
            Destroy(parent.gameObject);
        }
        GameObject go = Instantiate(parentPrefab, Vector3.zero, Quaternion.identity);
        parent = go.transform;
    
        
        foreach(TectonicSamplePoint sp in planetData.tesselation.points)
        {
            GameObject pGo = Instantiate(pointPrefab, sp.p.vector3, Quaternion.identity, parent);
            pGo.name = "" + sp.id;
            if(sp.id >= points.Length)
                pGo.GetComponent<MeshRenderer>().material = blue;
            pGo.transform.localScale = Vector3.one * pointSize;
            pGo.GetComponent<MeshRenderer>().material = matte;
            
            /*float elevation = (float)sp.data.column.surfaceLevel;
            if(elevation < 2000)
            {
                elevation = Mathf.Clamp(elevation, 900f, 1500f);
                elevation = (elevation - 900f) / 600f;
                pGo.GetComponent<MeshRenderer>().material.color = new Color(0, 0, elevation, 1f);
            }  
            else
            {
                elevation = Mathf.Clamp(elevation, 4500f, 10000f);
                elevation = (elevation - 4500f) / 5500f;
                pGo.GetComponent<MeshRenderer>().material.color = new Color(0, elevation, 0, 1f);
            }*/
                
            float seaDepth = (float)sp.data.column.hydrosphere.thickness;
            if(seaDepth > 0)
            {
                seaDepth = Mathf.Clamp(seaDepth, 0, 4000f);
                seaDepth = seaDepth / 4000f;
                pGo.GetComponent<MeshRenderer>().material.color = new Color(0, 0, seaDepth, 1f);
            }  
            else
            {
                pGo.GetComponent<MeshRenderer>().material.color = new Color(0, 1f, 0, 1f);
            }
        }
        foreach(TectonicSamplePointTensor spt in planetData.tesselation.tensors)
        {
            GameObject pGo = CreateTensor(planetData.tesselation.points[spt.p1].p.vector3, planetData.tesselation.points[spt.p2].p.vector3, parent.transform.GetChild(spt.p1));
            pGo.GetComponent<MeshRenderer>().material = yellow;
            //pGo.SetActive(false);
        }
    }

    public GameObject CreateTensor(Vector3 v1, Vector3 v2, Transform _parent)
    {
        Vector3 midpoint = (v1 + v2) / 2f;

        Vector3 scale = Vector3.one * tensorSize;
        float distance = Vector3.Distance(v1, v2);
        scale.x = distance;

        Vector3 direction = v2 - v1;
        Quaternion rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        
        GameObject tensor = Instantiate(tensorPrefab, midpoint, rotation);
        
        tensor.transform.localScale = scale;
        tensor.transform.Rotate(0, 90, 0);
        tensor.transform.SetParent(_parent);
        return tensor;
    }

    private void ApplyColumnToImages()
    {
        int _mantleThickness = selectedColumn.lithosphericMantle.thickness;
        int _lCrustThickness = selectedColumn.lowerCrust.thickness;
        int _uCrustThickness = selectedColumn.upperCrust.thickness;

        float mantleImageSize = (float)_mantleThickness * uiMultiplier * 0.001f;
        float lowerCrustImageSize = (float)(_lCrustThickness) * uiMultiplier * 0.001f;
        float upperCrustImageSize = (float)(_uCrustThickness) * uiMultiplier * 0.001f;

        float referenceDepthUI = seaLevelImage.anchoredPosition.y - (float)selectedColumn.referenceDepth * uiMultiplier * 0.001f;
        float manteImageOffset = referenceDepthUI + mantleImageSize * 0.5f;
        float lowerCrustImageOffset = referenceDepthUI + mantleImageSize + lowerCrustImageSize * 0.5f;
        float upperCrustImageOffset = referenceDepthUI + mantleImageSize + lowerCrustImageSize + upperCrustImageSize * 0.5f;

        mantleImage.anchoredPosition = new Vector2(0, manteImageOffset);
        lowerCrustImage.anchoredPosition = new Vector2(0, lowerCrustImageOffset);
        upperCrustImage.anchoredPosition = new Vector2(0, upperCrustImageOffset);

        mantleImage.sizeDelta = new Vector2(100f, mantleImageSize);
        lowerCrustImage.sizeDelta = new Vector2(100f, lowerCrustImageSize);
        upperCrustImage.sizeDelta = new Vector2(100f, upperCrustImageSize);

        selectedElevation = (_mantleThickness + _lCrustThickness + _uCrustThickness) - selectedColumn.referenceDepth;
    }
}
