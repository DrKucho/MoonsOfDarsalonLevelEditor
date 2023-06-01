using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class DragMeHandle : MonoBehaviour
{
    [HideInInspector] public Renderer rend;
#if UNITY_EDITOR
    public PivotMode pivotMode;
#endif
    public bool useLimits;
    public Vector3 min = new Vector3(float.MinValue, float.MinValue,float.MinValue);
    public Vector3 max = new Vector3(float.MaxValue, float.MaxValue,float.MaxValue);
    public DoorMovementExtension extension;

    bool UseLimits() { return useLimits;}

#if UNITY_EDITOR

    public void InitialiseInEditor()
    {
        rend = GetComponent<Renderer>();
        if (gameObject.layer != Layers.defaultLayer && gameObject.layer != Layers.vision)
            gameObject.layer = Layers.defaultLayer;
    }

    void Awake()
    {
        if (rend)
        {
            rend.enabled = !KuchoTime.appIsPlaying;
        }

        if (!KuchoTime.appIsPlaying && useLimits)
        {
            if (min.x == float.MinValue && min.y == float.MinValue && min.z == float.MinValue)
            {
                min = transform.localPosition;
            }
            if (max.x == float.MaxValue && max.y == float.MaxValue && max.z == float.MaxValue)
            {
                max = transform.localPosition;
            }
        }
    }

    private void OnValidate()
    {
        if (isActiveAndEnabled)
        {
            if (useLimits)
            {
                if (max.x < min.x)
                    max.x = min.x;
                if (max.y < min.y)
                    max.y = min.y;
                if (max.z < min.z)
                    max.z = min.z;
            }

            if (!rend)
            {
                rend = GetComponent<Renderer>();
                if (!rend)
                {
                    var spr = gameObject.AddComponent<SpriteRenderer>();
                    spr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/-KUCHO/Sprites/Misc/Handle Horizontal 0.psd");
                }
            }

            if (useLimits && transform.parent == null)
            {
                Debug.Log(this + " TENGO USE LIMITS ACTIVADO PERO NO TENGO PADRE? NO DEBERIA, DESACTIVO USELIMITS PARA EVITAR PROBLEMAS");
                useLimits = false;
            }
        }
    }

    private Vector3 oldPosition;
    [HideInInspector] public bool transformHasChanged;
    void Update()
    {
        transformHasChanged = false;
        if (Selection.gameObjects.Length > 0 && Selection.gameObjects[0] == gameObject)
        {
            Tools.pivotMode = pivotMode;
        }

        if (!rend)
            rend = GetComponent<Renderer>();
        if (gameObject.layer != Layers.defaultLayer && gameObject.layer != Layers.vision)
            gameObject.layer = Layers.defaultLayer;
        
        if (transform.position != oldPosition && useLimits)
        {
            transformHasChanged = true;
            Vector3 pos = transform.localPosition;
            if (pos.x < min.x)
                pos.x = min.x;
            if (pos.y < min.y)
                pos.y = min.y;
            if (pos.z < min.z)
                pos.z = min.z;
            if (pos.x > max.x)
                pos.x = max.x;
            if (pos.y > max.y)
                pos.y = max.y;
            if (pos.z > max.z)
                pos.z = max.z;

            transform.localPosition = pos;
            
            if (extension)
                extension.OnDoorMovement();
            oldPosition = transform.position;
        }
    }
    #endif
}

