using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SwitchingObject
{
    public bool onOff;

    public GameObject go;
    public Collider2D col;
    public MonoBehaviour mono;
    
}

public class HingeManager3D : MonoBehaviour
{

    [ReadOnly2Attribute] public Door.Status status;
    public bool closeOnEnable = true;
    public bool openCloseOnPlayerProximity = true;
    public bool initAsClosed = true;
    public Transform hinge;

    public enum Axis
    {
        X,
        Y,
        Z
    };

    [ReadOnly2Attribute] public Axis axis = Axis.Y;
    public Vector3 closeRot3;
    public Quaternion closeRot4;
    public Vector3 openRot3;
    public Quaternion openRot4;
    public float speed;
    public AudioClipArray openSound;
    public AudioClipArray openedSound;
    public AudioClipArray closeSound;
    public AudioClipArray closedSound;
    public SwitchingObject[] switchMeOnClosed;
    public SwitchingObject[] switchMeOnOpened;
    public VehicleInput vehicle;
    public GameObject pilotSprite;
    public ColliderSettings cabinGround;
    public AudioManager aM;
    public System.Action onOpened;
    public System.Action onClosed;


    Quaternion target;

    float openedClosedAngleDiff;

    float diffAngle;
    float previousdiffangle;
    float diffDiff;
    float previousDiffDiff;
    int movementFrameCount;
}
