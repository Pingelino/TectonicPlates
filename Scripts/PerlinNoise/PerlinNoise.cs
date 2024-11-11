using System;
using UnityEngine;
[System.Serializable]
public struct PerlinNoise
{
    public PerlinNoiseSettings[] settings;
    private int totalPriority;
    public double minVal;
    public double maxVal;

    public PerlinNoise(PerlinNoiseSettings[] _settings)
    {
        settings = _settings;
        minVal = 1.0;
        maxVal = -1.0;
        totalPriority = 0;
        foreach(PerlinNoiseSettings s in settings)
        {
            totalPriority += s.priority;
        }
    }

    public void FindApproximateMinMax(Point[] points)
    {
        double minV = 0;
        double maxV = 0;

        foreach(Point p in points)
        {
            double pVal = ValueAtPoint(p.normalized);

            if(pVal > maxV)
                maxV = pVal;
            if(pVal < minV)
                minV = pVal;
        }
        minVal = minV;
        maxVal = maxV;
    }

    public double ValueAtPoint(Point p)
    {
        double finalNoiseValue = 0.0;
        double totalMaxAmplitude = 0.0;

        // Loop over all noise settings and accumulate the results
        for (int i = 0; i < settings.Length; i++)
        {
            PerlinNoiseSettings currentSettings = settings[i];

            double amplitude = 1.0;
            double frequency = 1.0;
            double noiseLayerValue = 0.0;
            double maxAmplitude = 0.0;
            //float r = settings.rotation;
            //float2 rotatedUV = mul(float2x2(cos(r), -sin(r), sin(r), cos(r)), uv - 0.5);

            // Compute the fractal Perlin noise for the current settings
            for (int octave = 0; octave < currentSettings.octaves; octave++)
            {
                noiseLayerValue += PerlinNoiseValue(p * frequency, currentSettings) * amplitude;
                maxAmplitude += amplitude;
                amplitude *= currentSettings.persistence;
                frequency *= currentSettings.lacunarity;
            }

            double currentPriority = (double)currentSettings.priority / (double)totalPriority;
            if(maxAmplitude != 0)
                finalNoiseValue += (noiseLayerValue / maxAmplitude) * currentPriority;
            totalMaxAmplitude += 1; // Each layer has a max amplitude of 1 when normalized
        }

        //finalNoiseValue = finalNoiseValue * / totalMaxAmplitude;
        if(finalNoiseValue < 0)
            finalNoiseValue /= -1f * minVal;
        else if(finalNoiseValue > 0)
            finalNoiseValue /= maxVal;

        double modifiedNoiseValue = ModifyNoiseValue(finalNoiseValue);

        return Math.Clamp(modifiedNoiseValue, -1.0, 1.0);
    }

    private static double ModifyNoiseValue(double val)
    {
        return Math.Sqrt(Math.Abs(val)) * Math.Sign(val);
    }

    private static double Fade(double t)
    {
        return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
    }

    private static double Lerp(double a, double b, double t)
    {
        return a + t * (b - a);
        //return (b - a) * ((t * (t * 6.0 - 15.0) + 10.0) * t * t * t) + a;
    }

    private static double PerlinNoiseValue(Point p, PerlinNoiseSettings currentSettings)
    {
        // Apply scale and offset to position
        p = p * currentSettings.scale + currentSettings.offset;
        Point cell = p.rounded;
        Point localPos = (p - cell) + 0.5;
        Point dotsZ0 = PerlinDot2D(localPos, cell, 0);
        Point dotsZ1 = PerlinDot2D(localPos, cell, 1);
        Point fadePos = new Point(Fade(localPos.x), Fade(localPos.y), Fade(localPos.z));
        // Get fractional part of the position within the cell

        double nx00 = Lerp(dotsZ0.x, dotsZ0.y, fadePos.x);
        double nx10 = Lerp(dotsZ0.z, dotsZ0.w, fadePos.x);

        double nx01 = Lerp(dotsZ1.x, dotsZ1.y, fadePos.x);
        double nx11 = Lerp(dotsZ1.z, dotsZ1.w, fadePos.x);

        double ny0 = Lerp(nx00, nx10, fadePos.y);
        double ny1 = Lerp(nx01, nx11, fadePos.y);

        return Lerp(ny0, ny1, fadePos.z);
    }

    private static Point PerlinDot2D(Point localPos, Point cell, int zOffset)
    {
        Point cell00z = new Point(0.0, 0.0, (double)zOffset);
        Point cell01z = new Point(1.0, 0.0, (double)zOffset);
        Point cell10z = new Point(0.0, 1.0, (double)zOffset);
        Point cell11z = new Point(1.0, 1.0, (double)zOffset);

        Point gradient00z = RandomDirection(cell + cell00z);
        Point gradient10z = RandomDirection(cell + cell01z);
        Point gradient01z = RandomDirection(cell + cell10z);
        Point gradient11z = RandomDirection(cell + cell11z);

        Point dist00 = localPos - cell00z;
        Point dist10 = localPos - cell01z;
        Point dist01 = localPos - cell10z;
        Point dist11 = localPos - cell11z;

        double dot00 = Point.Dot(gradient00z, dist00);
        double dot10 = Point.Dot(gradient10z, dist10);
        double dot01 = Point.Dot(gradient01z, dist01);
        double dot11 = Point.Dot(gradient11z, dist11);

        return new Point(dot00, dot10, dot01, dot11);
    }

    private static uint NextRandom(ref uint state)
    {
        state = state * 747796405 + 2891336453;
        uint result = ((state >> ((int)(state >> 28) + 4)) ^ state) * 277803737;
        result = (result >> 22) ^ result;
        return result;
    }

    private static double RandomValue(ref uint state)
    {
        return (double)(NextRandom(ref state) / 4294967295.0); // 2^32 - 1
    }

    // Random value in normal distribution (with mean=0 and sd=1)
    private static double RandomValueNormalDistribution(ref uint state)
    {
        // Thanks to https://stackoverflow.com/a/6178290
        double theta = 2.0 * 3.1415926 * RandomValue(ref state);
        double rho = Math.Sqrt(-2.0 * Math.Log(RandomValue(ref state)));
        return rho * Math.Cos(theta);
    }

    // Calculate a random direction
    private static Point RandomDirection(Point xyz)
    {
        uint X = (uint)Math.Round(xyz.x);
        uint Y = (uint)Math.Round(xyz.y);
        uint Z = (uint)Math.Round(xyz.z);
        uint state = (X + 23523) * 23523;
        // Thanks to https://math.stackexchange.com/a/1585996
        double x = RandomValueNormalDistribution(ref state);
        state = state * (Y + 3634);
        double y = RandomValueNormalDistribution(ref state);
        state = state * (Z + 36234);
        double z = RandomValueNormalDistribution(ref state);
        return new Point(x, y, z).normalized;
    }
}
/**/