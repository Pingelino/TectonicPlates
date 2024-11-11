using UnityEngine;
[System.Serializable]
public struct TectonicTesselation
{
    public TectonicSamplePoint[] points;
    public TectonicSamplePointTensor[] tensors;

    public TectonicTesselation(TectonicSamplePoint[] _points, TectonicSamplePointTensor[] _tensors)
    {
        points = _points;
        tensors = _tensors;
    }
}
