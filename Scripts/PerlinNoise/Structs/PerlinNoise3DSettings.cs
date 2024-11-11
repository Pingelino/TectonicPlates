using UnityEngine;
[System.Serializable]
public struct PerlinNoise3DSettings
{
    public float scale;         // Scale of the noise
    public int octaves;         // Number of octaves (layers) of noise
    public float persistence;   // Amplitude multiplier per octave
    public float lacunarity;   // Frequency multiplier per octave
    public Vector3 offset;      // Offset for noise coordinates
    public float rotation;
    public int priority;

    public PerlinNoise3DSettings(float _scale, int _octaves, float _persistence, float _lacunarity, Vector3 _offset, float _rotation, int _priority)
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

    public static bool HasChanged (PerlinNoise3DSettings a, PerlinNoise3DSettings b)
    {
        return(a.scale != b.scale || a.octaves != b.octaves || a.persistence != b.persistence || a.lacunarity != b.lacunarity || a.offset != b.offset || a.rotation != b.rotation || a.priority != b.priority);
    }
    public static bool HasChanged (PerlinNoise3DSettings[] a, PerlinNoise3DSettings[] b)
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
