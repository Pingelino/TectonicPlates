using UnityEngine;
[System.Serializable]
public struct PerlinNoiseSettings
{
    public double scale;         // Scale of the noise
    public int octaves;         // Number of octaves (layers) of noise
    public double persistence;   // Amplitude multiplier per octave
    public double lacunarity;   // Frequency multiplier per octave
    public Point offset;      // Offset for noise coordinates
    public double rotation;
    public int priority;

    public PerlinNoiseSettings(double _scale, int _octaves, double _persistence, double _lacunarity, Point _offset, double _rotation, int _priority)
    {
        scale = _scale;
        octaves = _octaves;
        persistence = _persistence;
        lacunarity = _lacunarity;
        offset = _offset;
        rotation = _rotation;
        priority = _priority;
        if(_priority == 0)
            priority = 1;
    }

    public static bool HasChanged (PerlinNoiseSettings a, PerlinNoiseSettings b)
    {
        return(a.scale != b.scale || a.octaves != b.octaves || a.persistence != b.persistence || a.lacunarity != b.lacunarity || ((a.offset.x != b.offset.x) || (a.offset.y != b.offset.y) || (a.offset.z != b.offset.z)) || a.rotation != b.rotation || a.priority != b.priority);
    }
    public static bool HasChanged (PerlinNoiseSettings[] a, PerlinNoiseSettings[] b)
    {
        if(a.Length != b.Length)
            return true;
        for(int i = 0; i < a.Length; i++)
        {
            if(HasChanged(a[i], b[i]))
                return true;
        }
        return false;
    }
    /*public static bool operator != (PerlinNoiseSettings a, PerlinNoiseSettings b)
    {
        return
        (
            a.scale != b.scale || 
            a.octaves != b.octaves || 
            a.persistence != b.persistence || 
            a.lacunarity != b.lacunarity || 
            a.offset != b.offset || 
            a.rotation != b.rotation || 
            a.priority != b.priority
        );
    }*/
    
}
public struct ShaderPerlinNoiseSettings
{
    public float scale;       
    public int octaves;       
    public float persistence; 
    public float lacunarity;  
    public Vector3 offset;    
    public float rotation;
    public int priority;

    public ShaderPerlinNoiseSettings(PerlinNoiseSettings psn)
    {
        scale = (float)psn.scale;
        octaves = psn.octaves;
        persistence = (float)psn.persistence;
        lacunarity = (float)psn.lacunarity;
        offset = psn.offset.vector3;
        rotation = (float)psn.rotation;
        priority = psn.priority;
    }

    public ShaderPerlinNoiseSettings(double _scale, int _octaves, double _persistence, double _lacunarity, Point _offset, double _rotation, int _priority)
    {
        scale = (float)_scale;
        octaves = _octaves;
        persistence = (float)_persistence;
        lacunarity = (float)_lacunarity;
        offset = _offset.vector3;
        rotation = (float)_rotation;
        priority = _priority;
    }
    public static int stride {get{return sizeof(float) * 7 + sizeof(int) * 2;}}
}