using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Profiling;


public class GroundFiller : MonoBehaviour {

    public bool debug = false;
    public bool debugFillOnly = false;
    public bool debugNote = false;
    string debugNoteText = ""; //campo de texto para notas que seran copiadas en el gameobject creado
    public enum Execution {oneStepPerRandomFrames, AllAtStart, Static, EveryFrameIfICan}
    public Execution execution = Execution.oneStepPerRandomFrames;
    public MinMaxInt executeRandomValue= new MinMaxInt(1,1);
    [Header ("----GROUND DETECTION")]
    public int fillCycles= 400;
    public int noFillCycles = 50;
    int noFillCycleCount = 0;
    public Vector2 rayStartOffset = new Vector2(3,8);
    public float rayOffsetXRandomRange;
    public Vector2 rayDirection = new Vector2(0,-1);
    public int rayLength = 300;
    [Space (5)]

    public bool pixelRaycast = false;
    bool PixelRaycast(){return pixelRaycast;}
    bool NormalRaycast(){return !pixelRaycast;}
    public bool fillDestructible = true;
    public bool fillIndestructible = true;
     public PixelRaycastHit pixelHit;
    public MaskType _groundMask = MaskType.GroundObstacleAndLevel;
    public float maxInclination = 0.6f;

    int groundMask;
    [Header ("----CLONES")]
    public bool createClones = true;
    public int fallCloneThreshold = 8;
    public Transform makeClonesChildsOf;
    public Timer cloneTimer;
    public int clonationCount = 0;
    public int maxClonations = 5;
    public enum PlaceThingsMethod {useLightNeedsOfThings, cycleThingStoreSequentially, randomThingStore}
    public PlaceThingsMethod placeThingsMethod = PlaceThingsMethod.useLightNeedsOfThings;
    [Header ("----LIGHT")]
    public Vector2 readLightOffsetPos;
    public WorldLightPixel lightPixel;
    [Header("----THINGS")]
    public bool isGrassFiller;
    public ItemStoreFinder[] storeFinders;
    private DecoManager decoManager;
    public string _decoManager;
    public float instantGrowScreenOffset = 0;
    [Header("---Color")]
    [Range (0,4)] public float groundColorMult = 2f;
    [Range (0,1)] public float groundColorMinValue = 0.4f;
    bool ShowGroundColorStuff() { return pixelRaycast & groundColorMult > 0; }
    public Material colorBaseMat;
    [Range(0, 180)] public float minHueDeviation;
    [Range(0, 180)] public float maxHueDeviation;
    [Range(0,1)] public float levDeviation;
    [Range(0,1)] public float satDeviation;
    [Range(0,1)] public float alphaDeviation;
    public bool copyMyMatToThing = false;

    public enum PosOffset {Random, Cycle}
    [Header("---Position")]
    public PosOffset posOffset = PosOffset.Random;
    bool ShowPosZLuma() { return posOffset == PosOffset.Random; }
     public bool zOffsetBasedOnLuma;
    public bool useSpritePlanes;
    bool NotUsingSpritePlanes(){return !useSpritePlanes;}
    bool UsingSpritePlanes(){return useSpritePlanes;}
    public Vector3 posOffsetMin;
    public Vector3 posOffsetMax;
    public MinMax posOffsetNoZ;
    public Vector3 posOffsetInc;
    [ReadOnly2Attribute] public Vector3 finalPosOffset;

    public DecoPostOffset offset;
    Vector3 cyclingOffset;

    public static GroundFiller grassFiller;

    int cycle = 0; // en caso de no usar la luz para decidir que item hay que traer, se va rotando con esta variable
    [Header ("----MISC")]
    public bool shift1PixelOnFlipSprite = false;
    [System.NonSerialized] public float right;

    
    Vector3 previousPosition;
    int counter = 0;
    bool jump = false; // flag, se activa si hemos cuando de un rayo a otro la distancia y es mayor que fallCloneThreshold
    bool thereWasPlant = false; // flag, se activa si encontramos planta para mas adelante tomar una decision
    Item item;
    Renderer rend;
    GroundFound ground = new GroundFound();
    [HideInInspector] public bool doItPending;
    public enum Found {Valid, Invalid, NotFound}

    public class GroundFound{
        public Found found;
        public float normal;
        public float absNormal;
    }
    Transform myTransform;

    bool waiting = false;
     

    private HSLColor finalColorHSLA;
    private float hueSign = 1;

    float relativeSpritePlanesOffsetZ;
    DecoCell thing = new DecoCell();
    public void EndLife(){
        counter = fillCycles; // para que se acabe el bucle de la corutine
    }
}
