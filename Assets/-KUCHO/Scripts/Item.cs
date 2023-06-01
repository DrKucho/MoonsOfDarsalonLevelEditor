using UnityEngine;
using System.Collections;


[System.Serializable]
public class Item : MonoBehaviour
{

    public bool debug = false;
    public bool isOriginal = true;

    public bool outOfStoreAtStart = false;
    public float weight;
    public bool dontDestroyOnLoad = false;
    public bool restoreTransforms = false;
    public bool restoreHingeJoints = false;
    public bool restoreRigidbodies = false;

    public Transform inverScaleThis;
    [Header("OnBackToStore")]
    public bool destroyIfNoStoreOnSendingBack = false;
    public bool sendMyGeneratedEnemiesToStoreOnSendingBack = false;
    public bool backToStoreIfOutOfScreen = true; // solo funciona si el itemStore tambien lo tiene activado

    bool switchOffAtStart = false;

    [Header(" Don't Touch -----------------")]

    public ItemGroup ItemGroup;// lineas de monedas
    public bool inUse = false;
    public bool isOutOfStore = false;
    public float outOfStoreTime = -1;

    public int originalLayer;
    public float originalRbMass;
    public float originalRbGravityScale;

    public Transform trans;
    public ItemStore store; // lo fija el itemStore al arrancar
    public DecoThingsCreatedInEditor myDecoThingsInEditorManager;

    public int itemNumberInStore; // ESTO AL FINAL VALE PARA ALGO? SI! para el bitmap de objetos decorativos, en cada posicion se alamcena este numero y asi se cual tengo que destruir si le da una bala
    public float itemNumberInStoreParity; // lo uso en gheckground para hacer unas fromes los pares y otras los impares
    public ObjectSpawner generatorWhoCreatedMe;
    public GameObject objectWhoCreatedMe;
    public ExplosionManager explosionWhoCreatedMe;

    public ObjectSpawner[] myGenerators;

    public Material pickedUpMaterial;
    public Renderer firstRenderer;
    public Renderer[] allRenderers;
    public EnergyManager energyManager;
    public SWizSpriteAnimator anim;
    public GroundFiller groundFiller;
    public Decorative decorative;
    public GrowAnimator growAnimator;
    public Rigidbody2D rb;
    public bool rbSleepCheckTimeStarted;
    public float rbSleepCheckTimeStart;
    public CC cC;
    public AI aI;
    public DamageMaker damageMaker;
    public AudioManager aM;
    public SelfDestroy selfDestroy;
    public PickUp pickUp;
    public Gadget gadget;
    public DualBodyManager bodyManager;
    public LPBody lpBody;
    public Light2DManager lightManager;
    public Bullet bullet;
    public Base baseArrival;
    public SWizSpriteMaterialAssigner[] matAssigner;
    public SWizOnSpriteChangedGlobal[] onSpriteChangedCGlobal;
    public ExplosionManager exploManager;
    //[HideInInspector] public TruckController truckController;
    public VehicleInput vehicleInput;
    public Mark mark;
    public SkyLight skylight;
    public PickUpText pickUpText;
    public SecuentialAudioPlayerOnEnable secuentialAudioOnEnable;
    public OnEnableDelayedChanges delayedChanges;

    public TransformRestoreData[] transf;
    public HingeJointRestoreData[] hingeJ;
    public RigidbodyRestoreData[] bodies;
    public Vector3 myLazyTransformPosition; // la va poniendo el store no muy rapidamente, sirve para detectar si estan en rango o no, OutOfScreen
    public bool renderersShouldBeEnabled = true; // la va poniendo el store no muy rapidamente, sirve para saber si ha de activar renderers o no
    public bool ccEconomyModeShouldBeEnabled = false; // la va poniendo el store no muy rapidamente, sirve para saber si ha de activar renderers o no
    public bool gameObjectShouldBeOn = true;
    public bool specialCheckIsHappening = true;
    public Vector3 bt_pos;
    public bool bt_isActiveAndEnabled;
    public string stickerWhoCreatedMe;

    public delegate void OnSwitch();
    public event OnSwitch onSwitch;

    public delegate void OnGetBackToStore();
    public event OnGetBackToStore onGetBackToStore;

    [System.Serializable]
    public class TransformRestoreData
    {
        public Transform originalTransform;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;

    }
    [System.Serializable]
    public class HingeJointRestoreData
    {
        public HingeJoint2D originalHingeJoint;
        public Vector2 anchor;
        public Vector2 connectedAnchor;
        public Rigidbody2D connectedBody;

    }
    [System.Serializable]
    public class RigidbodyRestoreData
    {
        public Rigidbody2D originalRigidbody;
        public Vector3 localPosition;
        public Quaternion localRotation; // mas simple que tener que pillar solo la Z y luego hacer un euleranglesZ con la cosa de que no puedo modificar solo el Z ...gññ
        public float mass;
        public float gravityScale;
        public float drag;
        public float angularDrag;
        public bool freezeRotation;
    }


    public Delegates.Simple onDisableDelegate;
   
}
