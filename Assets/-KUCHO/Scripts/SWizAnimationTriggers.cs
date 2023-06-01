using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;



public class SWizAnimationTriggers : MonoBehaviour
{
    public bool debug = false;

    public AudioClipWrap[] frameAudio;
    public AudioClipArray frameRandomAudio;
    [Header("Enable/Disable With Commonad G")]
    public GameObject switchMe;
    [Header("Z SHIFT With Commonad Z")]
    public Transform zShiftL;
    public Transform zShiftR;
    public bool divideFrameIncs = false;
    public float t = 0.1f; // variable tiempo de audio clip para aplicar segun comando "q"
    public float forceMultiplier = 1;
    public Collider2D[] colliders;
    [SerializeField] Transform transf;

    [Header("----------------------------------")]
    [ReadOnly2]public SWizSpriteAnimator anim;
    [ReadOnly2]public CC cC;
    [ReadOnly2]public EnergyManager eM;
    [ReadOnly2]public AudioManager aM;
    [ReadOnly2]public ObjectSpawner enemyGenerator;
    [ReadOnly2]public SWizSprite sprite;
    [ReadOnly2]public SWizOnSpriteChanged onSpriteChanged;
    [ReadOnly2]public Light2DManager light2D;


}
