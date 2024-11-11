using UnityEngine;
[System.Serializable]
public struct TectonicTesselationSamplePoint
{
    public int id;
    public Point p;
    public int[] tensors;
    
    public TectonicTesselationSamplePoint(int _id, Point _p, int[] _tensors)
    {
        id = _id;
        p = _p;
        tensors = _tensors;
    }
}
