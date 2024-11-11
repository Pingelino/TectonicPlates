using UnityEngine;
[System.Serializable]
public struct PlanetData
{
    public TectonicTesselation tesselation;
    
    public PlanetData(Point[] voronoiPoints)
    {
        tesselation = TectonicTesselationGenerator.GenerateTesselation(voronoiPoints);
    }

    public void ApplyInitialIsostaticValues(PerlinNoise crustNoise)
    {
        TectonicIsostasy.ApplyInitialIsostaticValues(ref tesselation, crustNoise);
    }

    /*public void ApplyInitialSeaLevel()
    {
        TectonicIsostasy.ApplyInitialSeaLevel(ref tesselation);
    }*/
}
