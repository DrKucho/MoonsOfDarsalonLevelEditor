using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// la idea es crear una tabla con distintas animaciones que cada una se corresponde con una inclinacion
[System.Serializable]
public class SmartAnimation
{
    public string clipName; // el SWizanimationClip a reproducir siempre que el vector de inclinacion del personaje este entre los valores min max de vector
    public SWizSpriteAnimationClip clip;
    public Vector2 vectorMax;
    public Vector2 vectorMin;
    public bool frameDoesntFollowLegs = false;
    public int right;
    public Vector3[] colPos; // la z la uso para fijar trigger , si z = 0 trgger = false en caso contrario trigger = true

    public void GetClip(SWizSpriteAnimator anim)
    {
        clip = anim.GetClipByName(clipName);
        if (clip == null || clip.name == "Default" || clip.frames.Length == 0)
            Debug.Log("NO PUEDO OBTENER UN CLIP VALIDO PARA " + clipName);
    }
    public override string ToString()
    {
        return clipName;
    }
}

public static class SWizSpriteHelper
{
    public static void SetAlpha(SWizSprite spr, float alpha)
    {
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, alpha);
    }
    public static void SetAlpha(SWizBaseSprite spr, float alpha)
    {
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, alpha);
    }
    public static void SetScaleX(SWizSprite spr, float x)
    {
        spr.scale = new Vector3(x, spr.scale.y, spr.scale.z);
    }
    public static void SetScaleY(SWizSprite spr, float y)
    {
        spr.scale = new Vector3(spr.scale.x, y, spr.scale.z);
    }
}
