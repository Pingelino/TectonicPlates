using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public struct TectonicColumnLayerBaseValues
{
    public int baseDensity {get; set;}
    public double baseThermalConductivity {get; set;}
    public int baseHeatCapacity {get; set;}
    public int baseElasticModulus {get; set;}
    public int baseYieldStrength {get; set;}
    public int baseViscosity {get; set;}
    public TectonicColumnLayerBaseValues(int _baseDensity, double _baseThermalConductivity, int _baseHeatCapacity, int _baseElasticModulus, int _baseYieldStrength, int _baseViscosity)
    {
        baseDensity = _baseDensity;
        baseThermalConductivity = _baseThermalConductivity;
        baseHeatCapacity = _baseHeatCapacity;
        baseElasticModulus = _baseElasticModulus;
        baseYieldStrength = _baseYieldStrength;
        baseViscosity = _baseViscosity;
    }

    public static class DefaultValues
    {
        public static readonly Dictionary<TectonicColumnLayerType, TectonicColumnLayerBaseValues> Oceanic = new()
        {
            { TectonicColumnLayerType.Hydrosphere, new TectonicColumnLayerBaseValues(1000, 0.6, 4200, 0, 0, -3) },
            { TectonicColumnLayerType.Cryosphere, new TectonicColumnLayerBaseValues(917, 2.1, 2050, 9, 2, 14) },
            { TectonicColumnLayerType.Sediment, new TectonicColumnLayerBaseValues(2500, 1.5, 800, 10, 20, 16) },
            { TectonicColumnLayerType.UpperCrust, new TectonicColumnLayerBaseValues(2700, 2.5, 900, 30, 100, 24) },
            { TectonicColumnLayerType.LowerCrust, new TectonicColumnLayerBaseValues(2900, 2.9, 1000, 50, 150, 22) },
            { TectonicColumnLayerType.LithosphericMantle, new TectonicColumnLayerBaseValues(3300, 3.2, 1200, 70, 200, 21) }
        };
        public static readonly Dictionary<TectonicColumnLayerType, TectonicColumnLayerBaseValues> Continental = new()
        {
            { TectonicColumnLayerType.Hydrosphere, new TectonicColumnLayerBaseValues(1000, 0.6, 4200, 0, 0, -3) },
            { TectonicColumnLayerType.Cryosphere, new TectonicColumnLayerBaseValues(917, 2.1, 2050, 9, 2, 14) },
            { TectonicColumnLayerType.Sediment, new TectonicColumnLayerBaseValues(2500, 1.5, 800, 10, 30, 17) },
            { TectonicColumnLayerType.UpperCrust, new TectonicColumnLayerBaseValues(2700, 2.5, 900, 40, 200, 25) },
            { TectonicColumnLayerType.LowerCrust, new TectonicColumnLayerBaseValues(2900, 2.9, 1000, 60, 200, 23) },
            { TectonicColumnLayerType.LithosphericMantle, new TectonicColumnLayerBaseValues(3300, 3.2, 1200, 70, 300, 21) }
        };
    }

    
}






