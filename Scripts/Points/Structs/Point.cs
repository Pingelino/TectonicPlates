using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Point
{
    [SerializeField]
    public double[] coordinates;

    public Point(int dimension)
    {
        if (dimension <= 0)
            throw new ArgumentException("Dimension must be greater than zero.");
        
        coordinates = new double[dimension];
    }

    // Constructor with initial values
    public Point(params double[] initialValues)
    {
        if (initialValues == null || initialValues.Length == 0)
            throw new ArgumentException("You must provide at least one coordinate.");

        coordinates = new double[initialValues.Length];
        initialValues.CopyTo(coordinates, 0);
    }

    public double this[int index]
    {
        get
        {
            if (index < 0)
                throw new IndexOutOfRangeException("Invalid coordinate index.");
            if (index >= coordinates.Length)
                return 0;
            return coordinates[index];
        }
        set
        {
            if (index < 0 || index >= coordinates.Length)
                throw new IndexOutOfRangeException("Invalid coordinate index.");
            coordinates[index] = value;
        }
    }

    public int Dimension => coordinates.Length;
    public double x => this[0];
    public double y => this[1];
    public double z => this[2];
    public double w => this[3];

    public Point normalized => this / magnitude;//{ get{return this / magnitude;}}
    public double magnitude => Math.Sqrt(fastMagnitude);//{get{return Math.Sqrt(fastMagnitude);}}
    public double fastMagnitude => Dot(this, this);//{get{return Dot(this, this);}}
    
    public Vector3 vector3 => new Vector3((float)x, (float)y, (float)z);
    public Point rounded => Round(this);//{get{return new Point(Math.Round(x), Math.Round(y), Math.Round(z));}}
    public static Point[] Array(int length) => Array(3, length);
    public static Point[] Array(int dimension, int length)
    {
        Point[] pointArray = new Point[length];
        for(int i = 0; i < length; i++)
        {
            pointArray[i] = new Point(dimension);
        }
        return pointArray;
    }
    public static double FastDistance(Point a, Point b)
    {
        double dx = a.x - b.x;
        double dy = a.y - b.y;
        double dz = a.z - b.z;
        return dx * dx + dy * dy + dz * dz;
    }

    public static double Distance(Point a, Point b)
    {
        double dx = a.x - b.x;
        double dy = a.y - b.y;
        double dz = a.z - b.z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public static Point Cross(Point a, Point b)
    {
        double crossX = a.y * b.z - a.z * b.y;
        double crossY = a.z * b.x - a.x * b.z;
        double crossZ = a.x * b.y - a.y * b.x;

        return new Point(crossX, crossY, crossZ);
    }

    public static double Dot(Point a, Point b)
    {
        double result = 0;
        for(int i = 0; i < Math.Max(a.Dimension, b.Dimension); i++)
        {
            result += a[i] * b[i];
        }
        return result;
    }

    private static Point ApplyElementWise(Point a, Point b, Func<double, double, double> operation)
    {
        if (a.Dimension != b.Dimension)
            throw new ArgumentException("Points must have the same dimension.");

        Point result = new Point(a.Dimension);
        for (int i = 0; i < a.Dimension; i++)
        {
            result[i] = operation(a[i], b[i]);
        }
        return result;
    }

    private static Point ApplyElementWise(Point a, Func<double, double> operation)
    {
        Point result = new Point(a.Dimension);
        for (int i = 0; i < a.Dimension; i++)
        {
            result[i] = operation(a[i]);
        }
        return result;
    }

    private static Point ApplyElementWise(Point a, double b, Func<double, double, double> operation)
    {
        Point result = new Point(a.Dimension);
        for (int i = 0; i < a.Dimension; i++)
        {
            result[i] = operation(a[i], b);
        }
        return result;
    }

    public static Point operator +(Point a, Point b)
    {
        return ApplyElementWise(a, b, (x, y) => x + y);
    }
    public static Point operator +(Point a, double b)
    {
        return ApplyElementWise(a, b, (x, y) => x + y);
    }

    public static Point operator -(Point a, Point b)
    {
        return ApplyElementWise(a, b, (x, y) => x - y);
    }
    public static Point operator -(Point a, double b)
    {
        return ApplyElementWise(a, b, (x, y) => x - y);
    }
    
    public static Point operator *(Point a, Point b)
    {
        return ApplyElementWise(a, b, (x, y) => x * y);
    }
    public static Point operator *(Point a, double b)
    {
        return ApplyElementWise(a, b, (x, y) => x * y);
    }

    public static Point operator /(Point a, Point b)
    {
        return ApplyElementWise(a, b, (x, y) => x / y);
    }
    public static Point operator /(Point a, double b)
    {
        return ApplyElementWise(a, b, (x, y) => x / y);
    }

    public static Point Round(Point a)
    {
        return ApplyElementWise(a, Math.Round);
    }
    public static Point Floor(Point a)
    {
        return ApplyElementWise(a, Math.Floor);
    }
    public static Point Ceil(Point a)
    {
        return ApplyElementWise(a, Math.Ceiling);
    }
    //public static Point zero(int length) {get{return new Point(length);}}
    
    public static Point zero => new Point(0, 0, 0);
    public static Point Zeroes(int n) => new Point(n);
    public static Point one => new Point(1, 1, 1);
    public static Point Ones(int n) => Zeroes(n) + 1;
    public static Point FillWith(int n, int v) => Zeroes(n) + v;
}