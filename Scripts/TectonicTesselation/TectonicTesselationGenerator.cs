using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


public class TectonicTesselationGenerator : MonoBehaviour
{
    public static TectonicTesselation GenerateTesselation(Point[] points)
    {
        TectonicTesselationInstantiationData instantiationData = GenerateInstantiationData(points);
        
        
        TectonicSamplePoint[] tPoints = ConvertToSamplePoints(instantiationData.intersectionHelpers);
        TectonicSamplePointTensor[] tTensors = instantiationData.connectionHelpers.Select(c => new TectonicSamplePointTensor(c.x, c.y)).ToArray();
        return new TectonicTesselation(tPoints, tTensors);
    }

    public static TectonicTesselationInstantiationData GenerateInstantiationData(Point[] points)
    {
        int pointCount = points.Length;
        List<int3> sortedIntersectionIndices = new List<int3>();
        List<IntersectionHelper> intersectionHelpers = AddPointsAsIntersections(points);
        List<int2> connectionHelpers = new List<int2>();
        for(int i = 0; i < pointCount; i++)
        {
            int[] closestPointIndices = ClosestPointIndices(points, i);
            /*if(i == 0)
            {
                foreach(int k in closestPointIndices)
                {
                    Point sp = points[k];
                    
                    
                }
                //return intersectionHelpers.ToArray();
            }*/
            int3[] intersectionIndices = FindIntersectionIndices(points, closestPointIndices);
            
            for(int j = 0; j < intersectionIndices.Length; j++)
            {
                int3 intersection = intersectionIndices[j];
                int indexOfThisIntersection = intersectionHelpers.Count;
                if(intersection.x == i)
                {
                    
                    Point intersectionPoint = PlanePlanePlaneIntersection(points[intersection.x], 1.0, points[intersection.y], 1.0, points[intersection.z], 1.0, intersection);
                    intersectionHelpers.Add(new IntersectionHelper(indexOfThisIntersection, intersectionPoint));
                    sortedIntersectionIndices.Add(intersection);

                    
                }
                int3 nextIntersection = intersectionIndices[(j + 1) % intersectionIndices.Length];
                int indexOfNextIntersection = sortedIntersectionIndices.IndexOf(nextIntersection);
                if(indexOfNextIntersection == -1)
                    indexOfNextIntersection = sortedIntersectionIndices.Count;

                int thisIndex = sortedIntersectionIndices.IndexOf(intersection) + pointCount;
                int nextIndex = indexOfNextIntersection + pointCount;
                if(!connectionHelpers.Contains(new int2(thisIndex, nextIndex)) && !connectionHelpers.Contains(new int2(nextIndex, thisIndex)))
                    connectionHelpers.Add(new int2(thisIndex, nextIndex));
            }
        }
        int sortedIntersectionIndexCount = sortedIntersectionIndices.Count;
        for(int i = 0; i < sortedIntersectionIndexCount; i++)
        {
            int3 intersection = sortedIntersectionIndices[i];

            
            int2 i1 = new int2(pointCount + i, intersection.x);
            int2 i2 = new int2(pointCount + i, intersection.y);
            int2 i3 = new int2(pointCount + i, intersection.z);

            connectionHelpers.Add(i1);
            connectionHelpers.Add(i2);
            connectionHelpers.Add(i3);

            AddConnectionToIntersection(connectionHelpers[i], i);
            AddConnectionToIntersection(i1, sortedIntersectionIndexCount + i * 3 + 0);
            AddConnectionToIntersection(i2, sortedIntersectionIndexCount + i * 3 + 1);
            AddConnectionToIntersection(i3, sortedIntersectionIndexCount + i * 3 + 2);
        }

        
        /*for(int i = 0; i < sortedIntersectionIndices.Count; i++)
        {
            AddConnectionToIntersection(connectionHelpers[i * 4 + 0], i * 4 + 0);
            AddConnectionToIntersection(connectionHelpers[i * 4 + 1], i * 4 + 1);
            AddConnectionToIntersection(connectionHelpers[i * 4 + 2], i * 4 + 2);
            AddConnectionToIntersection(connectionHelpers[i * 4 + 3], i * 4 + 3);
        }*/

        void AddConnectionToIntersection(int2 connection, int index)
        {
            Debug.Log("sortedIntersectionIndexCount: " + sortedIntersectionIndexCount);
            Debug.Log("i: " + index + ", connection: (" + connection.x + ", " + connection.y + ")");

            intersectionHelpers[connection.x].AddNext(index);
            intersectionHelpers[connection.y].AddNext(index);
        }

        return new TectonicTesselationInstantiationData(intersectionHelpers.ToArray(), connectionHelpers.ToArray());
    }

    public static TectonicSamplePoint[] ConvertToSamplePoints(IntersectionHelper[] intersectionHelpers)
    {
        TectonicSamplePoint[] tPoints = new TectonicSamplePoint[intersectionHelpers.Length];
        for(int i = 0; i < intersectionHelpers.Length; i++)
        {
            tPoints[i] = new TectonicSamplePoint(intersectionHelpers[i].index, intersectionHelpers[i].p, intersectionHelpers[i].neighborIndices.ToArray(), new TectonicSamplePointData(i));
        }
        return tPoints;
    }

    /*public static TectonicSamplePoint[] CreateSamplePoints(IntersectionHelper[] intersections, PerlinNoise crustNoise)
    {
        TectonicSamplePoint[] samplePoints = new TectonicSamplePoint[intersections.Length];
        for(int i = 0; i < intersections.Length; i++)
        {

        }

    }*/

    /*public static VoronoiTile GenerateTile(VoronoiPoint[] points, int[] closestPointIndices)
    {
        int tile = closestPointIndices[0];
        if(tile == 0)
            
        int closestTile = closestPointIndices[1];
        VoronoiPoint p1 = points[tile];
        VoronoiPoint p2 = points[closestTile];
        VoronoiPoint cross = VoronoiPoint.Cross(p1, p2);
        VoronoiVertex v0 = new VoronoiVertex(PlanePlanePlaneIntersection(p1, 1.0, p2, 1.0, cross, 0), -1, closestTile, tile);

        List<VoronoiVertex> vList = new List<VoronoiVertex>();
        vList.Add(v0);
        for(int i = 0; i < points.Length; i++)
        {
            VoronoiVertex v1 = FindNextPoint(points, closestPointIndices, tile, v0.id, v0.id_prev, v0.p);
            
            if(v1.id == closestTile)
            {
                vList[0] = v1;
                vList.Add(v1);
                break;
            }
            vList.Add(v1);
            v0 = v1;
        }
        return new VoronoiTile(tile, p1, vList.ToArray());
    }*/

    public static int[] ClosestPointIndices(Point[] points, int i)
    {
        Point selectedPoint = points[i];
        // Use LINQ to order the points by distance to the selected point
        var closestIndices = points
            .Select((point, index) => new { Point = point, Index = index })  // Get both the point and its index
            .OrderBy(p => Point.FastDistance(p.Point, selectedPoint))  // Order by distance to the selected point
            .Select(p => p.Index)  // Get the indices
            .Take(10)
            .ToArray();  // Convert to array of indices

        return closestIndices;
    }

    public static List<IntersectionHelper> AddPointsAsIntersections(Point[] points)
    {
        return points
            .Select((point, index) => new IntersectionHelper(index, point))  // Get both the point and its index
            .ToList();
    }

    public static int3[] FindIntersectionIndices(Point[] points, int[] closestPointIndices)
    {
        
        List<int3> intersectionIndices = new List<int3>();

        Point p1 = points[closestPointIndices[0]];
        Point p2 = points[closestPointIndices[1]];
        
        Point cross = Point.Cross(p1, p2);
        Point v1 = PlanePlanePlaneIntersection(p1, 1.0, p2, 1.0, cross, 0, new int3(closestPointIndices[0], closestPointIndices[1], -1));
        
        
        
        
        

        int lastConnection = -1;
        int currentConnection = 1;
        for(int i = 0; i < closestPointIndices.Length; i++)
        {
            double minDist = 1e6;
            int minIndex = -1;

            Point p3 = Point.zero;
            Point v2 = Point.zero;
            
            for(int j = 1; j < closestPointIndices.Length; j++)
            {
                if(currentConnection == j || lastConnection == j)
                    continue;
                Point tempP3 = points[closestPointIndices[j]];
                
                Point intersect = PlanePlanePlaneIntersection(p1, 1.0, p2, 1.0, tempP3, 1.0, new int3(closestPointIndices[0], closestPointIndices[currentConnection], closestPointIndices[j]));

                if(Point.Dot((intersect - v1), cross) < 0)
                    continue;
                double dist = Point.FastDistance(v1, intersect);
                if(dist < minDist)
                {
                    minDist = dist;
                    minIndex = j;
                    v2 = intersect;
                    p3 = tempP3;
                    
                }
            }

            if(minIndex == -1)
            {
                string s = "";
                foreach(int k in closestPointIndices)
                {
                    s += k + ", ";
                }
                Debug.LogError(s);
            }
            
            v1 = v2;
            p2 = new Point(p3.x, p3.y, p3.z);
            cross = Point.Cross(p1, p2);
            intersectionIndices.Add(SortVertexIndex(new int3(closestPointIndices[0], closestPointIndices[currentConnection], closestPointIndices[minIndex])));
            lastConnection = currentConnection;
            currentConnection = minIndex;
            
            
            //vertIndices.Add(new int3(closestPointIndices[0], closestPointIndices[currentConnection], closestPointIndices[minIndex]));
            if(minIndex == 1)
                break;
        }
        return intersectionIndices.ToArray();
    }

    public static int3 SortVertexIndex(int3 v)
    {
        int x = (int)Math.Min(v.x, Math.Min(v.y, v.z));
        int z = (int)Math.Max(v.x, Math.Max(v.y, v.z));
        int y = v.x;
        if(y == x || y == z)
            y = v.y;
        if(y == x || y == z)
            y = v.z;
        return new int3(x, y, z);
    }

    public static Point PlanePlanePlaneIntersection(Point normal1, double d1, Point normal2, double d2, Point normal3, double d3, int3 vertex)
    {
        // Set up the 3x3 matrix
        double[,] A = {
            { normal1.x, normal1.y, normal1.z },
            { normal2.x, normal2.y, normal2.z },
            { normal3.x, normal3.y, normal3.z }
        };

        // Calculate the determinant of matrix A
        double detA = Determinant3x3(A);

        // If the determinant is close to zero, the planes are parallel or coincident
        if (Math.Abs(detA) < 1E-15)
        {
            Debug.LogError("The planes do not intersect at a unique point (they are parallel or coincident).");
            return Point.zero; // Return an invalid point
        }

        // Now we solve the system using Cramer's rule
        // We replace each column of A with the vector of dot products (d1, d2, d3) to solve for x, y, and z

        // Replace first column and calculate determinant
        double[,] A1 = {
            { d1, normal1.y, normal1.z },
            { d2, normal2.y, normal2.z },
            { d3, normal3.y, normal3.z }
        };
        double x = Determinant3x3(A1) / detA;

        // Replace second column and calculate determinant
        double[,] A2 = {
            { normal1.x, d1, normal1.z },
            { normal2.x, d2, normal2.z },
            { normal3.x, d3, normal3.z }
        };
        double y = Determinant3x3(A2) / detA;

        // Replace third column and calculate determinant
        double[,] A3 = {
            { normal1.x, normal1.y, d1 },
            { normal2.x, normal2.y, d2 },
            { normal3.x, normal3.y, d3 }
        };
        double z = Determinant3x3(A3) / detA;

        // Return the intersection point
        return new Point(x, y, z);
    }
    private static double Determinant3x3(double[,] m)
    {
        return m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1])
             - m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0])
             + m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
    }

}

[System.Serializable]
public struct TectonicTesselationInstantiationData
{
    public IntersectionHelper[] intersectionHelpers;
    public int2[] connectionHelpers;

    public TectonicTesselationInstantiationData(IntersectionHelper[] _intersectionHelpers, int2[] _connectionHelpers)
    {
        intersectionHelpers = _intersectionHelpers;
        connectionHelpers = _connectionHelpers;
    }
}