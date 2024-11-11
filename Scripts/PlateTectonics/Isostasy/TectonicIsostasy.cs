using System;

public class TectonicIsostasy
{
    public static void ApplyInitialIsostaticValues(ref TectonicTesselation tesselation, PerlinNoise crustNoise)
    {
        ApplyInitialLithosphereValues(ref tesselation, crustNoise);
        ApplyInitialSeaLevel(ref tesselation);
    }

    public static void ApplyInitialLithosphereValues(ref TectonicTesselation tesselation, PerlinNoise crustNoise)
    {
        for(int i = 0; i < tesselation.points.Length; i++)
        {
            Point p = tesselation.points[i].p;
            double noiseValue = crustNoise.ValueAtPoint(p);
            int crustType = (int)Math.Round(((noiseValue) + 1.0) * 0.5);
            int crustThickness = NoiseToCrustThickness(noiseValue);
            tesselation.points[i].data.column.ApplyInitialThickness(crustType, crustThickness);
        }
    }

    public static void ApplyInitialSeaLevel(ref TectonicTesselation tesselation)
    {
        int seaLevel = GetSeaLevel(tesselation);
        for(int i = 0; i < tesselation.points.Length; i++)
        {
            int surfaceLevel = tesselation.points[i].data.column.surfaceLevel;
            int seaDepth = seaLevel - surfaceLevel;

            if(seaDepth > 0)
            {
                tesselation.points[i].data.column.ApplyInitialHydrosphereThickness(seaDepth);
            }
        }
    }

    public static int GetSeaLevel(TectonicTesselation tesselation)
    {
        const int avgDepth = 2800; //Total volume of earth's water / earth's area
        
        int pointCount = tesselation.points.Length;

        int totalElevation = 0;

        for(int i = 0; i < pointCount; i++)
        {
            totalElevation += tesselation.points[i].data.column.surfaceLevel;
        }

        int avgElevation = totalElevation / pointCount;

        return avgElevation + avgDepth;
    }

    private static int NoiseToCrustThickness(double value)
    {
        // Constants for thickness in meters
        const int minOceanicCrustThickness = 6000; // minimum oceanic crust thickness due to magma supply variability
        const int maxOceanicCrustThickness = 10000; // maximum oceanic crust thickness in meters
        const int minContinentalCrustThickness = 30000; // minimum continental crust thickness in meters
        const int maxContinentalCrustThickness = 70000; // maximum continental crust thickness in meters

        // If the value is between -1 and 0, interpolate oceanic crust thickness
        if (value >= -1 && value <= 0)
        {
            // Linearly interpolate between min and max oceanic thickness
            double interpolatedThickness = minOceanicCrustThickness +
                                        ((value + 1) * (maxOceanicCrustThickness - minOceanicCrustThickness));
            return (int)Math.Round(interpolatedThickness);
        }

        // If the value is between 0 and 1, interpolate continental crust thickness
        if (value > 0 && value <= 1)
        {
            double interpolatedThickness = minContinentalCrustThickness +
                                        (value * (maxContinentalCrustThickness - minContinentalCrustThickness));
            return (int)Math.Round(interpolatedThickness);
        }

        // If value is outside the expected range, return a default or error code (optional)
        throw new ArgumentOutOfRangeException("value", "Value must be between -1 and 1.");
    }
}
