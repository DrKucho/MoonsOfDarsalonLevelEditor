using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArcadeResolution
{
    public int width;
    public int height;
    public Vector2 size;
    public int zoom;
    public int refreshRate;
    public float aspectRatio;
    public float aspectRatioDiff;
    public Vector2 fatRes; // este puede tener decimales y ser impar
    public Vector2 fatRes2; // Este no
    public bool even; // tiene alto impar?
    public Vector2 canvasShift;
    public float cornerScaleMult = 1;
    public float blur;
    public float aberration;
    public ShaderScanlineData scanline;
    public float UIVerticalShift; // los textos y meters de playing screen (que hace overlay sobre screen Quad, neceistan esto para encajar 
    public bool checkPassed;
    public bool isIdeal;

    public static implicit operator bool(ArcadeResolution me)
    { // para poder hacer if(class) en lugar de if(class != null) NULLABLE nullable
        return me != null;
    }
    public ArcadeResolution(int width, int height, int _zoom)
    {

    }
    
}

