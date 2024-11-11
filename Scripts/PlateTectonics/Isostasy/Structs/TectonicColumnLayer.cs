using UnityEngine;

[System.Serializable]
public struct TectonicColumnLayer
{
    public TectonicColumnLayerType layerType;
    public TectonicColumnLayerBoundary boundary;
    public int thickness;
    public TectonicColumnLayerBaseValues baseValues;
    

    public TectonicColumnLayer(TectonicColumnLayerType _layerType, int _crustType)
    {
        layerType = _layerType;
        boundary = new TectonicColumnLayerBoundary(0, 0);
        thickness = 0;

        if(_crustType == 1)
            baseValues = TectonicColumnLayerBaseValues.DefaultValues.Continental[layerType];
        else
            baseValues = TectonicColumnLayerBaseValues.DefaultValues.Oceanic[layerType];
    }
}

//public double Density { get; set; }
//public double ThermalConductivity { get; set; }
//public double HeatCapacity { get; set; }
//public double ElasticModulus { get; set; }
//public double YieldStrength { get; set; }
//public double Viscosity { get; set; }


