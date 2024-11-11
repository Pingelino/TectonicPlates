using UnityEngine;
using System;
[System.Serializable]
public struct TectonicColumn
{
    public TectonicColumnLayer hydrosphere;
    public TectonicColumnLayer cryosphere;
    public TectonicColumnLayer sediment;
    public TectonicColumnLayer upperCrust;
    public TectonicColumnLayer lowerCrust;
    public TectonicColumnLayer lithosphericMantle;

    public int referenceDepth;  // Reference depth in meters
    public int surfaceLevel {get{return hydrosphere.thickness + cryosphere.thickness + sediment.thickness + upperCrust.thickness + lowerCrust.thickness + lithosphericMantle.thickness - referenceDepth;}}

    public TectonicColumn(int initialValue)
    {
        hydrosphere = new TectonicColumnLayer(TectonicColumnLayerType.Hydrosphere, 0);
        cryosphere = new TectonicColumnLayer(TectonicColumnLayerType.Cryosphere, 0);
        sediment = new TectonicColumnLayer(TectonicColumnLayerType.Sediment, 0);
        upperCrust = new TectonicColumnLayer(TectonicColumnLayerType.UpperCrust, 0);
        lowerCrust = new TectonicColumnLayer(TectonicColumnLayerType.LowerCrust, 0);
        lithosphericMantle = new TectonicColumnLayer(TectonicColumnLayerType.LithosphericMantle, 0);
        referenceDepth = 0;
    }

    public void ApplyInitialThickness(int crustType, int crustThickness)
    {
        upperCrust.thickness = crustThickness / 2;
        lowerCrust.thickness = crustThickness / 2;

        referenceDepth = EstimateReferenceDepth(crustType, crustThickness);

        CalculateLithosphericMantleThickness();
    }

    public void ApplyInitialHydrosphereThickness(int hydrosphereThickness)
    {
        hydrosphere.thickness = hydrosphereThickness;
    }

    private int EstimateReferenceDepth(int crustType, int crustThickness)
    {
        // Constants for global average reference depths (in meters)
        const int oceanicBaseReferenceDepth = 90000; // average oceanic reference depth in meters
        const int continentalBaseReferenceDepth = 150000; // average continental reference depth in meters

        // Constants for densities in kg/m^3
        double rhoCrust = (upperCrust.baseValues.baseDensity + lowerCrust.baseValues.baseDensity) / 2.0;
        double rhoMantle = lithosphericMantle.baseValues.baseDensity;

        // Base reference depth based on crust type
        int baseReferenceDepth = crustType == 1 ? continentalBaseReferenceDepth : oceanicBaseReferenceDepth;

        // Adjust the base reference depth based on crust thickness
        // Here we make a simple estimate where thicker crust leads to deeper reference depth
        double adjustmentFactor = (rhoCrust / rhoMantle) * crustThickness * 0.1;
        int adjustedReferenceDepth = (int)(baseReferenceDepth + adjustmentFactor);

        return adjustedReferenceDepth;
    }

    private void CalculateLithosphericMantleThickness()
    {
        // Densities in kg/m^3
        double rhoUpperCrust = upperCrust.baseValues.baseDensity;
        double rhoLowerCrust = lowerCrust.baseValues.baseDensity;
        double rhoMantle = lithosphericMantle.baseValues.baseDensity;

        // Thickness of crust layers in meters
        int thicknessUpperCrust = upperCrust.thickness;
        int thicknessLowerCrust = lowerCrust.thickness;

        // Total crust thickness
        int totalCrustThickness = thicknessUpperCrust + thicknessLowerCrust;

        // Calculate equivalent mantle thickness using Airy isostasy
        double mantleThickness = referenceDepth 
                                - ((rhoUpperCrust * thicknessUpperCrust + rhoLowerCrust * thicknessLowerCrust) / rhoMantle);

        lithosphericMantle.thickness = (int)Math.Round(mantleThickness);
    }
}
[System.Serializable]
public enum TectonicColumnLayerType
{
    Hydrosphere,
    Cryosphere,
    Sediment,
    UpperCrust,
    LowerCrust,
    LithosphericMantle 
}

//Now, most of the thicknesses are still zero, but the one that's the most important right now is the thickness of the lithosphericMantle. We could solve this by creating a function for isostatic adjustment, Could you create a function for Airy Isostasy, called IsostaticAdjustment()?
