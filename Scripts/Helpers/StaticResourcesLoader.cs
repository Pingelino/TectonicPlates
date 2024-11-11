using UnityEngine;

public static class StaticResourcesLoader
{
    public static ComputeShader PointRenderer {get; private set; }
    public static ComputeShader PerlinNoiseGenerator {get; private set; }
    public static ComputeShader PerlinSphere {get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void LoadStaticAssets()
    {
        PointRenderer = Resources.Load<ComputeShader>("PointRenderer");
        PerlinNoiseGenerator = Resources.Load<ComputeShader>("PerlinNoiseGenerator");
        PerlinSphere = Resources.Load<ComputeShader>("PerlinSphere");
    }
}
