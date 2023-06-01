using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AddSubLightIntensityOfChilds : MonoBehaviour {

    [Range (0,0.2f)]public float value = 0.1f;
    
    void AddLightIntensity () {
        var allLights = GetComponentsInChildren<Light>();
        foreach (Light l in allLights)
            if (l.isActiveAndEnabled)
                l.intensity += value;
	}
	
    
    void SubLightIntensity () {
        var allLights = GetComponentsInChildren<Light>();
        foreach (Light l in allLights)
            if (l.isActiveAndEnabled)
                l.intensity -= value;
	}
    int shadowTypeIndex = 0;
    
    void ChangeShadowType(){
        var allLights = GetComponentsInChildren<Light>();
        shadowTypeIndex++;
        if (shadowTypeIndex > 2)
            shadowTypeIndex = 0;
        
        foreach (Light l in allLights)
            if (l.isActiveAndEnabled)
            {
                l.shadows = (LightShadows)shadowTypeIndex;
            }
    }
    
    void AddShadowStrength () {
        var allLights = GetComponentsInChildren<Light>();
        foreach (Light l in allLights)
            if (l.isActiveAndEnabled)
                l.shadowStrength += value;
    }

    
    void SubShadowStrength () {
        var allLights = GetComponentsInChildren<Light>();
        foreach (Light l in allLights)
            if (l.isActiveAndEnabled)
                l.shadowStrength -= value;
    }
}
