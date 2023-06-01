using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpritePlane))]
public class CoverLightSprite : MonoBehaviour {

    public bool validateme;
    public Renderer rend;
    public SpritePlane spritePlane;
    public bool inRange = false; // lo activa LightCoverCam script
    public static List<CoverLightSprite> instances  = new List<CoverLightSprite>();
    void OnValidate () {
        if (isActiveAndEnabled)
        {
            rend = GetComponent<Renderer>();
            spritePlane = GetComponent <SpritePlane>();
            if (spritePlane.type != SpritePlane.Type.Coverground1 || spritePlane.type != SpritePlane.Type.Coverground2)
                spritePlane.type = SpritePlane.Type.Coverground1;
            gameObject.layer = Layers.defaultLayer; // la unica capa que ve coverlightCam, por cuestiones de culling
        }
    }
    private void OnEnable()
    {
        if (instances == null)
            instances = new List<CoverLightSprite>();
        Debug.Log(this + "AÑADIENDO COVER LIGHT SPRITE, COUNT=" +  instances.Count);
        instances.Add(this);
    }
    private void OnDisable()
    {
        instances.Remove(this);
    }
}
