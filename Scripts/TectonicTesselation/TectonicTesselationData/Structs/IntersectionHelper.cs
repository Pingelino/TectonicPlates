using System.Collections.Generic;

[System.Serializable]
public struct IntersectionHelper
{
    public int index;
    public Point p;

    public List<int> neighborIndices;

    public IntersectionHelper(int _index, Point _p)
    {
        index = _index;
        p = _p;
        neighborIndices = new List<int>();
    }

    public void AddNext(int next)
    {
        int index = neighborIndices.IndexOf(next);
        if(index == -1)
        {
            neighborIndices.Add(next);
        }
    }
}