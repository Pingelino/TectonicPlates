using UnityEngine;
using System;
[System.Serializable]
public struct TectonicSamplePointData
{
    public Point relativeVelocity;
    public TectonicColumn column;


    //public int crustType; //0: oceanic, 1: continental
    //public int crustDensity;
    //public int crustAge;
    //public int crustThickness;
    //public int crustElevation; //- height from midpoint of an ideal, homogenous crust. Ground level is radius + crustElevation + crustThickness
    public TectonicSamplePointData(int id)
    {
        relativeVelocity = Point.zero;
        column = new TectonicColumn(id);
    }

    /*public static TectonicSamplePointData CrustDataFromNoiseValue(double noiseValue)
    {

    }*/
    

} 