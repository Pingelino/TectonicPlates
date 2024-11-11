using UnityEngine;

public struct RenderingPointObject
{
    public Vector3 position;
    public float radius;
    public Color color;

    public RenderingPointObject(Vector3 _position, float _radius, Color _color)
    {
        position = _position;
        radius = _radius;
        color = _color;
    }
}
