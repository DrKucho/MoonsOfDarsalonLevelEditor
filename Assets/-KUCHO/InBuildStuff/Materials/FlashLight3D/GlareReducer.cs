using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class GlareReducer : MonoBehaviour
{
    [System.Serializable]
    public class ScanPoint
    {
        public float dist;
        public float radius;
        public float contactWeight;
        public bool contact;
        public float skyWeight;
        public bool skyContact;
    }

    [Range(0,1)] public float lengthScaleRatio = 1;
    [Range(0,3)] public float skyToIntensityRatio = 1;
    public float changeSpeedUp = 1;
    public float changeSpeedDown = 0.3f;
    public Transform scanPoint;
    public ScanPoint[] scanPoints;
    public float weightMult = 1;
    public float widthComp = 1.5f;
    private Collider2D[] found = new Collider2D[1];
    private Vector3 originalScale;
    private Transform myTrans;
    int contactCount = 0;
    
    void Awake()
    {
        myTrans = transform;
        originalScale = transform.localScale;
    }
    
    private int index;
    
    float lengthScaleFactor;
    float widthScaleFactor;

    // lo llama light2dmanager con su intensity calculado, luego este lo reduce
    // es algo chapu que este cotrole la escala y la intensidad lo haga el otro...
    public float MyUpdate()
    {
        index++;
        if (index >= scanPoints.Length)
            index = 0;

        var localPos = Constants.zero3;
        localPos.y += scanPoints[index].dist;
        scanPoint.localPosition = localPos;
        
        int hitCount = Physics2D.OverlapCircleNonAlloc(scanPoint.position, scanPoints[index].radius, found, Masks.groundAndObstacleAndSelfIllum);

        Vector3 newScale;
        if (hitCount > 0)
        {
            scanPoints[index].contact = true;
        }
        else
        {
            scanPoints[index].contact = false;
        }

        Color backPix = WorldMap.background.texture.GetPixel((int)scanPoint.position.x, (int)scanPoint.position.y);
        
        if (backPix.a > 0)
        {
            scanPoints[index].skyContact = false;
        }
        else
        {
            scanPoints[index].skyContact = true;
        }
        
        // ??? mas adelante se asigna , esto no tiene efecto
        /*
        if (lengthScaleFactor > 1)
            lengthScaleFactor = 1;
        if (lengthScaleFactor < 0.25f)
            lengthScaleFactor = 0.25f;
        */
        // ???

        float obstacleContactWeight = 0;
        float skyContactWeight = 1; // este va multiplicado
        for (int i = 0; i < scanPoints.Length; i++)
        {
            if (scanPoints[i].contact)
            {
                obstacleContactWeight += scanPoints[i].contactWeight;
            }

            if (scanPoints[i].skyContact)
            {
                skyContactWeight *= scanPoints[i].skyWeight;
            }
        }

        obstacleContactWeight *= weightMult;
        if (obstacleContactWeight > 1)
            obstacleContactWeight = 1;

        lengthScaleFactor = (1 - obstacleContactWeight) * lengthScaleRatio;
        widthScaleFactor = widthComp - lengthScaleFactor; // si lengh factor = 1, width factor sera 1 tb , si length 0 width 2
        
        newScale.y = originalScale.y * lengthScaleFactor;
        newScale.x = originalScale.x * lengthScaleFactor * widthScaleFactor;
        newScale.z = 1;

        myTrans.localScale = newScale;

        return skyContactWeight * skyToIntensityRatio * SkyManager.instance.realSaturatedSkyColorHSL.l;

    }
}
