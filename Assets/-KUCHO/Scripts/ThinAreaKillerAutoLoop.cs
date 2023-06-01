using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThinAreaKillerAutoLoop : MonoBehaviour {

    public bool debug;
    [ReadOnly2Attribute] public Vector3  myPos ;

    public Vector3 offset;
    public Vector2 size;

    public int howManyChecksPerFrame = 2;
    public float inc = 5;
    public float incFarFactor = 2;
    public float dist = 5;
    public float stampOffset = 0;
    public ExplosionStampExtras myKiller;
    //public CC myCC;



    RaycastHit2D[] hits = new RaycastHit2D[1];
    ContactFilter2D filter = new ContactFilter2D();

}
