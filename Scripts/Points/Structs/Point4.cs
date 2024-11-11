using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Point4
{
    public double x;
    public double y;
    public double z;
    public double w;

    public Point4(double _x, double _y, double _z, double _w)
    { 
        x = _x;
        y = _y;
        z = _z;
        w = _w;
    }

    public Point4 normalized { get{return this / magnitude;}}

    public double fastMagnitude {get{return x * x + y * y + z * z + w * w;}}
    public double magnitude {get{return Math.Sqrt(x * x + y * y + z * z);}}
    //public Vector3 vector3 {get{return new Vector3((float)x, (float)y, (float)z);}}
    public Point4 rounded {get{return new Point4(Math.Round(x), Math.Round(y), Math.Round(z), Math.Round(w));}}
    public static double FastDistance(Point4 a, Point4 b)
    {
        double dx = a.x - b.x;
        double dy = a.y - b.y;
        double dz = a.z - b.z;
        double dw = a.w - b.w;
        return dx * dx + dy * dy + dz * dz + dw * dw;
    }

    public static double Distance(Point4 a, Point4 b)
    {
        double dx = a.x - b.x;
        double dy = a.y - b.y;
        double dz = a.z - b.z;
        double dw = a.w - b.w;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
    }

    /*public static Point4 Cross(Point4 a, Point4 b)
    {
        double crossX = a.y * b.z - a.z * b.y;
        double crossY = a.z * b.x - a.x * b.z;
        double crossZ = a.x * b.y - a.y * b.x;

        return new Point4(crossX, crossY, crossZ);
    }*/

    public static double Dot(Point4 a, Point4 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
    }

    //public static Point4 zero = new Point4(0, 0, 0);

    public static Point4 operator +(Point4 a, Point4 b)
    {
        return new Point4(a.x + b.x, a.y + b.y, a.z + b.z, a.w * b.w);
    }
    public static Point4 operator +(Point4 a, double b)
    {
        return new Point4(a.x + b, a.y + b, a.z + b, a.w * b);
    }
    public static Point4 operator -(Point4 a, Point4 b)
    {
        return new Point4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
    }
    public static Point4 operator -(Point4 a, double b)
    {
        return new Point4(a.x - b, a.y - b, a.z - b, a.w - b);
    }
    public static Point4 operator *(Point4 a, double b)
    {
        return new Point4(a.x * b, a.y * b, a.z * b, a.w * b);
    }
    public static Point4 operator /(Point4 a, double b)
    {
        return new Point4(a.x / b, a.y / b, a.z / b, a.z / b);
    }

    
    public static Point4 zero{get{return new Point4(0, 0, 0, 0);}}
}