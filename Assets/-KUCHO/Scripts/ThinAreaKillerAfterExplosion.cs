using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThinAreaKillerAfterExplosion : MonoBehaviour {

    public bool debug;

    public float size = 50;

    public int howManyChecksPerFrame = 2;
    public float inc = 5;
    public float dist = 5;
    public float excludeInnerArea = 10;
    public float stampOffset = 0;
    public ExplosionStampExtras myKiller;
    //public CC myCC;

    public void InitialiseInEditor()
    {
        myKiller = GetComponentInChildren<ExplosionStampExtras>();
        gameObject.SetActive(false);
    }

    RaycastHit2D[] hits = new RaycastHit2D[1];
    ContactFilter2D filter = new ContactFilter2D();

}
