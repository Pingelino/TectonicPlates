using UnityEngine;

public struct RenderingCameraData
{
    public Matrix4x4 viewMatrix;
    public Matrix4x4 projectionMatrix;
    public Matrix4x4 inverseViewMatrix; // To compute world space rays
    public Vector3 cameraPosition;
    public Vector2 resolution; // width, height
}

