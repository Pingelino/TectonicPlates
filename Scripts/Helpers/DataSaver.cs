using UnityEngine;

public class DataSaver : MonoBehaviour
{
    public Manager manager;
    public bool saveTesselation = false;
    public TectonicTesselation tesselation;
    private void Update()
    {
        if(saveTesselation)
        {
            saveTesselation = false;
            tesselation = manager.tesselation;
        }
    }
}
