using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Point3
{
    public double x;
    public double y;
    public double z;

    public Point3(double _x, double _y, double _z)
    { 
        x = _x;
        y = _y;
        z = _z;
    }

    public Point3 normalized { get{return this / magnitude;}}

    public double fastMagnitude {get{return x * x + y * y + z * z;}}
    public double magnitude {get{return Math.Sqrt(x * x + y * y + z * z);}}
    public Vector3 vector3 {get{return new Vector3((float)x, (float)y, (float)z);}}
    public Point3 rounded {get{return new Point3(Math.Round(x), Math.Round(y), Math.Round(z));}}
    public static double FastDistance(Point3 a, Point3 b)
    {
        double dx = a.x - b.x;
        double dy = a.y - b.y;
        double dz = a.z - b.z;
        return dx * dx + dy * dy + dz * dz;
    }

    public static double Distance(Point3 a, Point3 b)
    {
        double dx = a.x - b.x;
        double dy = a.y - b.y;
        double dz = a.z - b.z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public static Point3 Cross(Point3 a, Point3 b)
    {
        double crossX = a.y * b.z - a.z * b.y;
        double crossY = a.z * b.x - a.x * b.z;
        double crossZ = a.x * b.y - a.y * b.x;

        return new Point3(crossX, crossY, crossZ);
    }

    public static double Dot(Point3 a, Point3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    //public static Point3 zero = new Point3(0, 0, 0);

    public static Point3 operator +(Point3 a, Point3 b)
    {
        return new Point3(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static Point3 operator +(Point3 a, double b)
    {
        return new Point3(a.x + b, a.y + b, a.z + b);
    }
    public static Point3 operator -(Point3 a, Point3 b)
    {
        return new Point3(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    public static Point3 operator -(Point3 a, double b)
    {
        return new Point3(a.x - b, a.y - b, a.z - b);
    }
    public static Point3 operator *(Point3 a, double b)
    {
        return new Point3(a.x * b, a.y * b, a.z * b);
    }
    public static Point3 operator /(Point3 a, double b)
    {
        return new Point3(a.x / b, a.y / b, a.z / b);
    }

    
    public static Point3 zero{get{return new Point3(0, 0, 0);}}
}