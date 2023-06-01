using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RestoreRayAnim : MonoBehaviour
{
    public SWizSpriteAnimator anim;
    public int defaultClipID = 0; 
 
    public void InitialiseInEditor()
    {
        anim = GetComponent<SWizSpriteAnimator>();
        if (anim)
            defaultClipID = anim.DefaultClipId;
    }

    private void Awake()
    {
        anim.Library = AnimationDataBase.instance.alienSoldierSpawnerRay;
        anim.DefaultClipId = defaultClipID;
    }
}