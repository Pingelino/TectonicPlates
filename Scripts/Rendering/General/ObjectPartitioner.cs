using UnityEngine;
using System.Collections.Generic;

public class ObjectPartitioner
{
    public static float CeilToAbsolute(float value)
    {
        return Mathf.Sign(value) * Mathf.Ceil(Mathf.Abs(value));
    }
    public static Vector3 SnapBoundsToGrid(Vector3 bound, float gridSize)
    {
        return new Vector3(CeilToAbsolute(bound.x / gridSize), CeilToAbsolute(bound.y / gridSize), CeilToAbsolute(bound.z / gridSize)) * gridSize;
    }
    public static void PartitionPoints(RenderingPointObject[] points, float cellSize, ref List<List<int>> cells, ref Bounds gridBounds)
    {
        // Determine bounds of all points
        foreach (RenderingPointObject point in points)
        {
            gridBounds.Encapsulate(point.position + Vector3.one * point.radius);
            gridBounds.Encapsulate(point.position - Vector3.one * point.radius);
        }

        // Calculate grid dimensions
        Vector3 gridMin = SnapBoundsToGrid(gridBounds.min, cellSize);
        Vector3 gridMax = SnapBoundsToGrid(gridBounds.max, cellSize);
        Vector3 gridSize = gridMax - gridMin;

        int cellsX = Mathf.CeilToInt(gridSize.x / cellSize);
        int cellsY = Mathf.CeilToInt(gridSize.y / cellSize);
        int cellsZ = Mathf.CeilToInt(gridSize.z / cellSize);

        Debug.Log("CellAmount: " + cellsX + ", " + cellsY + ", " + cellsZ);

        // Initialize cells
        cells = new List<List<int>>();
        for (int i = 0; i < cellsX * cellsY * cellsZ; i++)
        {
            cells.Add(new List<int>());
        }

        // Map points to cells
        for (int i = 0; i < points.Length; i++)
        {
            RenderingPointObject point = points[i];
            Vector3 pointMin = point.position - Vector3.one * point.radius;
            Vector3 pointMax = point.position + Vector3.one * point.radius;

            // Find the min and max cells this point overlaps
            Vector3 localMin = pointMin - gridMin;
            Vector3 localMax = pointMax - gridMin;

            int minCellX = Mathf.FloorToInt(localMin.x / cellSize);
            int minCellY = Mathf.FloorToInt(localMin.y / cellSize);
            int minCellZ = Mathf.FloorToInt(localMin.z / cellSize);

            int maxCellX = Mathf.FloorToInt(localMax.x / cellSize);
            int maxCellY = Mathf.FloorToInt(localMax.y / cellSize);
            int maxCellZ = Mathf.FloorToInt(localMax.z / cellSize);

            // Clamp values to ensure they are within bounds
            minCellX = Mathf.Clamp(minCellX, 0, cellsX - 1);
            minCellY = Mathf.Clamp(minCellY, 0, cellsY - 1);
            minCellZ = Mathf.Clamp(minCellZ, 0, cellsZ - 1);

            maxCellX = Mathf.Clamp(maxCellX, 0, cellsX - 1);
            maxCellY = Mathf.Clamp(maxCellY, 0, cellsY - 1);
            maxCellZ = Mathf.Clamp(maxCellZ, 0, cellsZ - 1);

            // Iterate through all cells that this point overlaps
            for (int x = minCellX; x <= maxCellX; x++)
            {
                for (int y = minCellY; y <= maxCellY; y++)
                {
                    for (int z = minCellZ; z <= maxCellZ; z++)
                    {
                        int cellIndex = x + y * cellsX + z * (cellsX * cellsY);
                        cells[cellIndex].Add(i);
                    }
                }
            }
        }
    }

}
