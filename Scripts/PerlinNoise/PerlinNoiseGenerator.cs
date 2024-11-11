using UnityEngine;

public static class PerlinNoiseGenerator
{
    /*public static double NoiseValueAtPoint(Point p)
    {

    }*/


    public static RenderTexture GeneratePerlinNoise(PerlinNoiseSettings[] settings, int textureSize)
    {
        if(StaticResourcesLoader.PerlinNoiseGenerator == null)
            StaticResourcesLoader.LoadStaticAssets();
        ComputeShader computeShader = StaticResourcesLoader.PerlinNoiseGenerator;
        // Set up the noise texture
        RenderTexture noiseTexture = new RenderTexture(textureSize, textureSize, 0);
        noiseTexture.enableRandomWrite = true;
        noiseTexture.filterMode = FilterMode.Bilinear;
        noiseTexture.Create();

        int totalPriority = 0;
        ShaderPerlinNoiseSettings[] shaderSettings = new ShaderPerlinNoiseSettings[settings.Length];
        for(int i = 0; i < settings.Length; i++)
        {
            shaderSettings[i] = new ShaderPerlinNoiseSettings(settings[i]);
            totalPriority += settings[i].priority;
        }

        // Create a buffer to hold multiple noise settings
        ComputeBuffer settingsBuffer = new ComputeBuffer(shaderSettings.Length, ShaderPerlinNoiseSettings.stride);
        settingsBuffer.SetData(shaderSettings);

        

        // Set the buffer and texture in the compute shader
        int kernelHandle = computeShader.FindKernel("PerlinNoiseGenerator");
        computeShader.SetTexture(kernelHandle, "Result", noiseTexture);
        computeShader.SetBuffer(kernelHandle, "Settings", settingsBuffer);
        computeShader.SetInt("textureSize", textureSize);
        computeShader.SetInt("settingsCount", settings.Length);
        computeShader.SetInt("totalPriority", totalPriority);

        computeShader.Dispatch(kernelHandle, textureSize, textureSize, 1);
        settingsBuffer.Dispose();

        return noiseTexture;
    }
    public static RenderTexture Generate3DPerlinNoiseTexture(PerlinNoiseSettings[] settings, int width, int height, float resolution, Camera cam, Color backgroundColor, bool _useMap, Vector2 mercatorClamp, float intersectPlaneDistance, Vector3 planeNormal)
    {
        int useMap = 0;
        if(_useMap)
            useMap = 1;
        int textureWidth = (int)Mathf.RoundToInt(width * resolution);
        int textureHeight = (int)Mathf.RoundToInt(height * resolution);
        if(StaticResourcesLoader.PerlinSphere == null)
            StaticResourcesLoader.LoadStaticAssets();
        ComputeShader computeShader = StaticResourcesLoader.PerlinSphere;
        // Set up the noise texture
        RenderTexture noiseTexture = new RenderTexture(textureWidth, textureHeight, 0);
        noiseTexture.enableRandomWrite = true;
        noiseTexture.filterMode = FilterMode.Bilinear;
        noiseTexture.Create();


        int totalPriority = 0;
        ShaderPerlinNoiseSettings[] shaderSettings = new ShaderPerlinNoiseSettings[settings.Length];
        for(int i = 0; i < settings.Length; i++)
        {
            shaderSettings[i] = new ShaderPerlinNoiseSettings(settings[i]);
            totalPriority += settings[i].priority;
        }

        // Create a buffer to hold multiple noise settings
        ComputeBuffer settingsBuffer = new ComputeBuffer(shaderSettings.Length, ShaderPerlinNoiseSettings.stride);
        settingsBuffer.SetData(shaderSettings);


        // Set the buffer and texture in the compute shader
        int kernelHandle = computeShader.FindKernel("PerlinSphere");
        computeShader.SetTexture(kernelHandle, "Result", noiseTexture);
        computeShader.SetBuffer(kernelHandle, "Settings", settingsBuffer);
        computeShader.SetInt("textureWidth", textureWidth);
        computeShader.SetInt("textureHeight", textureHeight);
        computeShader.SetInt("settingsCount", settings.Length);
        computeShader.SetInt("totalPriority", totalPriority);
        computeShader.SetInt("useMap", useMap);

        float planeHeight = 1f * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float planeWidth = planeHeight * cam.aspect;
        // Send data to shader
        computeShader.SetVector("ViewParams", new Vector3(planeWidth, planeHeight, 1f));
        computeShader.SetMatrix("CamLocalToWorldMatrix", cam.transform.localToWorldMatrix);
        computeShader.SetVector("camPos", cam.transform.position);
        computeShader.SetVector("backgroundColor", backgroundColor);
        computeShader.SetVector("mercatorClamp", mercatorClamp * Mathf.PI / 180f);
        
        computeShader.SetFloat("intersectPlaneDistance", intersectPlaneDistance);
        computeShader.SetVector("planeNormal", planeNormal.normalized);

        computeShader.Dispatch(kernelHandle, (int)Mathf.RoundToInt((float)textureWidth / 8f), (int)Mathf.RoundToInt((float)textureHeight / 8f), 1);
        settingsBuffer.Dispose();

        return noiseTexture;
    }
}