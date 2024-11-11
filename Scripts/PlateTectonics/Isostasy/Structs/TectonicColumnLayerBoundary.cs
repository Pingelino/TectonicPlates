using UnityEngine;
[System.Serializable]
public struct TectonicColumnLayerBoundary
{
    public int depth;
    public int temperature;
    public TectonicColumnLayerBoundary(int _depth, int _temperature)
    {
        depth = _depth;
        temperature = _temperature;
    }
}