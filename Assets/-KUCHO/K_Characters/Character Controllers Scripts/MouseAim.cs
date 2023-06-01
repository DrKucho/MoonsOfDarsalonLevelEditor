using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MouseAim : MonoBehaviour
{
    public SpriteRenderer sprRend;
    public SnapMeEachFrame sprSnap;
    public AnimationCurve alpha;
    private Vector2 accum;
    public float maxMag = 80;
    public float minMag = 0;
    public float noAimingThreshold = 20;
    [ReadOnly2Attribute] public float mag;
    [ReadOnly2Attribute]public float e;

    public void InitialiseInEditor()
    {
        sprRend = GetComponentInChildren<SpriteRenderer>();
        sprSnap = sprRend.GetComponent<SnapMeEachFrame>();
        sprSnap.enabled = false; // lo actualizo yo
    }

    public Vector2 MyUpdate(Vector2 input)
    {
        if (gameObject.activeSelf  == false)
            gameObject.SetActive(true);
        float inputMag = input.magnitude;
        Vector2 inputDir = input.normalized;
		                
        accum += input;
        mag = accum.magnitude;
        Vector2 dir = accum.normalized;
        if (mag > maxMag)
            mag = maxMag;
        if (mag < minMag)
            mag = minMag;
        accum = dir * mag;
        transform.localPosition = accum;
        Color c = sprRend.color;
        e = 1 - (maxMag - mag)/maxMag;
        c.a = alpha.Evaluate(e);
        sprRend.color = c;
        sprSnap.DoIt();
        if (mag < noAimingThreshold)
            dir = Constants.zero2;
        return dir;
        //Debug.Log("Inp" + input + " Acc" + mouseAimAccum + " Mag" + mag + " Dir" + mouseAimDir);
    }
}
