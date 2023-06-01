using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FlyerBlockedDetectorOnCollisionStay : MonoBehaviour
{
    public bool debug;
    bool DebugIsOn() { return debug; }
    public int disableColliderFrames;
    public int maxContacts = 50;
    public GameObject colliderParent;
    public float penetratingRepulse = 10000;
    public float centerCollidersRange = 20;
    [ReadOnly2Attribute] public Collider2D[] cols;
    [ReadOnly2Attribute] public Collider2D[] rearCols;
    [ReadOnly2Attribute] public Collider2D[] frontCols;
    [ReadOnly2Attribute] public Collider2D[] centerCols;
    [ReadOnly2Attribute] public List<ContactPoint2D> penetratingContacts = new List<ContactPoint2D>();

    ContactPoint2D[] points;
    Rigidbody2D myRB;
    [ReadOnly2Attribute] public int cindex;
    [ReadOnly2Attribute] public int contactCount;
    [ReadOnly2Attribute] public bool blocked = false;
    [ReadOnly2Attribute] public bool xBlocked = false;
    [ReadOnly2Attribute] public bool leftBlocked = false;
    [ReadOnly2Attribute] public bool rightBlocked = false;
    [ReadOnly2Attribute] public bool yBlocked = false;
    [ReadOnly2Attribute] public bool upBlocked = false;
    [ReadOnly2Attribute] public bool downBlocked = false;
    [ReadOnly2Attribute] public int leftContactCount = 0;
    [ReadOnly2Attribute] public int rightContactCount = 0;
    [ReadOnly2Attribute] public int upContactCount = 0;
    [ReadOnly2Attribute] public int downContactCount = 0;
    [ReadOnly2Attribute] public Vector2 allNormals;
    [ReadOnly2Attribute] public float allSeparation;

    
}
