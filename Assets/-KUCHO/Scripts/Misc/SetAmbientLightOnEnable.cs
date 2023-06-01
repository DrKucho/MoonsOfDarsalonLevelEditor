using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAmbientLightOnEnable : MonoBehaviour
{
    public Color color;
    public float intensity;
    Color colorBackup;
    float intensityBackup;

    private void OnValidate()
    {
        if (isActiveAndEnabled)
        {
            Do();
        }
    }
    private void OnEnable() { Do(); }
    private void Do()
    {
        colorBackup = RenderSettings.ambientLight;
        intensityBackup = RenderSettings.ambientIntensity;
        RenderSettings.ambientLight = color;
        RenderSettings.ambientIntensity = intensity;
    }
    private void OnDisable()
    {
        RenderSettings.ambientLight = colorBackup;
        RenderSettings.ambientIntensity = intensityBackup;
    }
}
