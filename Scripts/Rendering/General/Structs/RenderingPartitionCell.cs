using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public struct RenderingPartitionCell
{
    public List<int> objectIndices;  // Indices of points within this cell
    public int objectCount;

    public RenderingPartitionCell(int index)
    {
        objectIndices = new List<int>();
        objectCount = 0;
    }
}
