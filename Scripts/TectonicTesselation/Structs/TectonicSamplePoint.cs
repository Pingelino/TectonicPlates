using UnityEngine;
[System.Serializable]
public struct TectonicSamplePoint
{
    public int id;
    public Point p;
    public int[] tensors;
    public TectonicSamplePointData data;
    
    public TectonicSamplePoint(int _id, Point _p, int[] _tensors, TectonicSamplePointData _data)
    {
        id = _id;
        p = _p;
        tensors = _tensors;
        data = _data;
    }
}
