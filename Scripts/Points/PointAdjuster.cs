using System;
using UnityEngine;

public static class PointAdjuster
{
    public static Point[] AdjustPointsIterative(Point[] points, int iterations, double adjustmentPerIteration)
    {
        for(int i = 0; i < iterations; i++)
        {
            points = AdjustPoints(points, adjustmentPerIteration);
        }
        return points;
    }
    public static Point[] AdjustPoints(Point[] points, double adjustmentPerIteration)
    {
        int amount = points.Length;
        double optimalDistance = 2.0 * Math.Sqrt(Math.PI / (double)amount);
        Point[] pointAdjustments = Point.Array(amount);
        for(int ia = 0; ia < amount; ia++)
        {
            Debug.Log("pa: " + ia);
            pointAdjustments[ia] = Point.zero;
        }
        
        for(int i = 0; i < amount; i++)
        {
            Point p1 = points[i];

            Point p1Adjustment = pointAdjustments[i];
            
            for(int j = i + 1; j < amount; j++)
            {
                if(i == j)
                    continue;
                Point p2 = points[j];
                Point adjustment = GetAdjustment(p1, p2, optimalDistance);
                p1Adjustment = p1Adjustment - adjustment;

                pointAdjustments[j] = pointAdjustments[j] + adjustment;
            }
            points[i] = (p1 + p1Adjustment * adjustmentPerIteration).normalized;
        }

        return points;
    }

    public static TectonicSamplePoint[] AdjustSamplePointsIterative(TectonicSamplePoint[] points, int iterations, double adjustmentPerIteration)
    {
        for(int i = 0; i < iterations; i++)
        {
            points = AdjustSamplePoints(points, adjustmentPerIteration);
        }
        return points;
    }
    public static TectonicSamplePoint[] AdjustSamplePoints(TectonicSamplePoint[] points, double adjustmentPerIteration)
    {
        int amount = points.Length;
        double optimalDistance = 2.0 * Math.Sqrt(Math.PI / (double)amount);
        Point[] pointAdjustments = Point.Array(amount);
        for(int i = 0; i < amount; i++)
        {
            Point p1 = points[i].p;
            Point p1Adjustment = pointAdjustments[i];
            
            for(int j = i + 1; j < amount; j++)
            {
                if(i == j)
                    continue;
                Point p2 = points[j].p;
                Point adjustment = GetAdjustment(p1, p2, optimalDistance);

                p1Adjustment -= adjustment;
                pointAdjustments[j] += adjustment;
            }
            points[i].p = (p1 + p1Adjustment * adjustmentPerIteration).normalized;
        }

        return points;
    }

    public static Point GetAdjustment(Point p1, Point p2, double optimalDistance)
    {
        double rSquared = (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y) + (p1.z - p2.z) * (p1.z - p2.z);
        return (p2 - p1).normalized * GetOffset(rSquared, optimalDistance);
    }

    public static double GetOffset(double rSquared, double optimalDistance)
    {
        return Math.Pow(Math.E, -1.0 * rSquared / (optimalDistance * optimalDistance)) * optimalDistance;
    }
        
}
