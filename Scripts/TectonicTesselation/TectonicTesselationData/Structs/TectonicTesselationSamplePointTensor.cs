using UnityEngine;
[System.Serializable]
public struct TectonicTesselationSamplePointTensor
{
    public int p1;
    public int p2;

    public int[] points {get{return new int[]{p1, p2};}}
    
    public TectonicTesselationSamplePointTensor(int _p1, int _p2)
    {
        p1 = _p1;
        p2 = _p2;
    }

    public int GetOther(int p)
    {
        return p == p1 ? p1 : p2;
    }
}
