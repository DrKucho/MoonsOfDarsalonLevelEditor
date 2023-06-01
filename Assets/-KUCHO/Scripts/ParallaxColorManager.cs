using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ParallaxColorManager : MonoBehaviour
{

    [Header("BASE PARAMETERS")]
    [Range(-180, 180)] public float hueShift;
    [Range(-2, 2)] public float bright;
    [Range(-2, 2)] public float contrast;
    [Range(-2, 2)] public float saturation;
    public Color tint;
    [HideInInspector] public Color additiveColor  = Color.white;
    
    [Header("Z CHANGE")]
    [Range(-180, 180)] public float hueShift_z;
    [Range(-0.3f, 0.3f)] public float bright_z;
    [Range(-0.3f, 0.3f)] public float contrast_z;
    [Range(-0.3f, 0.3f)] public float saturation_z;
    [Range(-0.2f, 0.2f)] public float tint_z;
    public Color[] tints;

    private WorldMap worldMap;

    bool GotWorldMap() { return worldMap != null; }

    public ParallaxMeCentered[] elements;
    public ParallaxColorManager other;
    
    public void InitialiseInEditor()
    {
        elements = GetComponentsInChildren<ParallaxMeCentered>();
        System.Array.Sort( elements, ( a, b ) => a.transform.position.z.CompareTo( b.transform.position.z ) );
        tints = new Color[elements.Length];
    }
    
    private void OnEnable()
    {
        MyUpdate();
    }

    void OnValidate()
    {
        if (isActiveAndEnabled && !worldMap)
            MyUpdate();
    }

    
    public void MyUpdate()
    {
        if (other)
        {
            other.hueShift = hueShift;
            other.bright = bright;
            other.contrast = contrast;
            other.saturation = saturation;
            other.tint = tint;
            other.additiveColor = additiveColor;
            other.hueShift_z = hueShift_z;
            other.bright_z = bright_z;
            other.contrast_z = contrast_z;
            other.bright_z = bright_z;
            other.saturation_z = saturation_z;
            other.tint_z = tint_z;
            other.MyUpdate();
            for (int i = 0; i < tints.Length; i++)
            {
                tints[i] = other.tints[i];
            }
            other.MyUpdate();
            return;
        }
        int lastIndex = elements.Length - 1;
        for (int i = 0; i < elements.Length; i++)
        {
            var mat = elements[i].mat;
            if (mat.HasProperty(ShaderProp._Hue))
            {
                mat.SetFloat(ShaderProp._Hue, hueShift + (i) * hueShift_z);
            }
            if (mat.HasProperty(ShaderProp._Val))
            {
                mat.SetFloat(ShaderProp._Val, (bright + 1) + i * bright_z);
            }
            if (mat.HasProperty(ShaderProp._Cont))
            {
                mat.SetFloat(ShaderProp._Cont, (contrast + 1) + i * contrast_z);
            }
            if (mat.HasProperty(ShaderProp._Sat))
            {
                mat.SetFloat(ShaderProp._Sat, (saturation + 1) + i * saturation_z);
            }
            
            int inverseIndex = lastIndex  - i;
            if (mat.HasProperty(ShaderProp._MultiplicativeColor))
            {
                //HSLColor hslC = HSLColor.FromRGBA(tint);
                //hslC.l += inverseIndex * tint_z;
                //Color c = hslC.ToRGBA();
                Color c = tint;
                float m = inverseIndex * tint_z;
                c.r = Mathf.Clamp01(c.r + m);
                c.g = Mathf.Clamp01(c.g + m);
                c.b = Mathf.Clamp01(c.b + m);
                mat.SetColor(ShaderProp._MultiplicativeColor, c);
                tints[i] = c;
            }
            if (mat.HasProperty(ShaderProp._AdditiveColor))
            {
                mat.SetColor(ShaderProp._AdditiveColor, additiveColor );
            }
        }
    }

}
