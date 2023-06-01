using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityStandardAssets.ImageEffects;

[System.Serializable]
public class PalleteColorAdjust
{
	public string name;
	[Range(-1, 2)] public float preBrightness = 1;
	[Range(-1, 2)] public float preContrast = 1;
	[Range(-1, 2)] public float preSaturation = 1;
	[Header("---")]
    [Range(-1, 1)] public float paletteBoost = 0;
    [Range(-1, 1)] public float darkThreshold = 0;
    [Range(-1, 1)] public float darkBoost = 0;
    [Header("---")]
    [Range(-1, 2)] public float brightness = 1;
    [Range(-1, 2)] public float contrast = 1;
    [Range(-1, 3)] public float saturation = 1;

    string ToString()
    {
	    return name;
    }

    public PalleteColorAdjust(){
        paletteBoost = 0;
        darkThreshold = 0;
        darkBoost = 0;
        brightness = 1;
        contrast = 1;
        saturation = 1;
    }
    public void CopyFrom(PalleteColorAdjust o)
    {
	    name = o.name;
	    preBrightness = o.preBrightness;
	    preContrast = o.preContrast;
	    preSaturation = o.preSaturation;
	    
        paletteBoost = o.paletteBoost;
        darkThreshold = o.darkThreshold;
        darkBoost = o.darkBoost;
        
        brightness = o.brightness;
        contrast = o.contrast;
        saturation = o.saturation;
    }
}
	