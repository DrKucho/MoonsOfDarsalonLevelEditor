using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Light2D;


using UnityEngine.Serialization;

public class ExplosionManager : MonoBehaviour {

	public bool debug=false;
	[Header ("--- GENERAL ------------------------")]
	public Transform copyPositionFrom;
    public bool copyGroundZ;
	public RandomFloat offsetZ;
	public float randomScaleRange;
	public bool forceZeroScaleZ = true;
	float randomSizeMult;
	public LightSprite lightSprite;
	public Vector3 randomRotation;
	public Quaternion originalRotation; // localRotation original
	public float luminosity = 1f; // para materiales/shaders que tengan esta propiedad
	public Vector3 originalLocalPosition;
	[HideInInspector][SerializeField] Item item;
	Vector3 offset; // lo manda explosion manager cada vez que llama a play
	float offsetDelay; // lo manda explosion invoker cada ves que llama a play
	bool offsetApplied = false;
	Renderer parentRenderer;
	float timeToApplyOffset;
	float timeToGetBackToStore;
	[HideInInspector][SerializeField] PickUpDropper[] pickupDroppers; // este lo pillamos de nuestro gameobject
	[Header ("--- LIQUID FUN ----------------------")]
    public LPFixtureCircle fixture;
    public int liquidFrameStepsToDeleteFixture = 1;
    float fixtureOriginalRadius;
    int liquidStepToDisableFixture;
	public LiquidExplosion[] liquidExp;
	float liquidExpDur = 0;
	float liquidExpSpawnDur = 0;
	[Header ("--- PARTICLE SYSTEM --------------------")]
	public bool autoAsignParticleSystems = true;
    public bool clearParticlesBeforePlay = true;
	public ParticleSystemWrap[] psw;
	float timeEmiting; // creo que esta variable lo que hace es recoger el valor del ps.duration y lo almacena para saber cuando acaban las particulas y asi poder hacer un Stop, por que quieres hacer un stop? para poder hacer un play en la siguienteemision
	public float totalDuration;
	float psDur;
	float	timeToStopEmitting;
	bool runing = false; // flag para saber si el sistema tiene particulas vivas y asi ejecutar el codigo de update
	[HideInInspector][SerializeField] ParticleSystem[] PS; // solo la necesito para pillar los particle system componentes y asignarlos luego a mis referencias
	[Header ("--- SPRITE ANIMATION --------------------")]
	float animDur;
    public SWizSpriteAnimator anim;
    public Vector3 spriteScaleOverride;
    [HideInInspector][SerializeField] SWizSpriteAnimationClip explosionClip;

    [Header("--- THIN AREA KILLER --------------------")]
    public ThinAreaKillerAfterExplosion thinAreaKiller;
	[Header ("--- STAMP --------------------")]
	public ExplosionStampExtras stampExtras;

	[Header ("--- COLLISIONS / DAMAGE --------------------")]
	public ArmyType  armyType; // la fija explosion manager, sirve para gestionar fuego amigo con los colliders ignore de cada army
	[Header ("to be copied to EnergyManager.power")]
	public float power = 1f;
	public float colliderRadius = 1f;
	public float collidersActiveTime = 0.1f;
	public ArmyType hurt = ArmyType.All;
	public bool dismissIgnoreCollisions = false;
	public Collider2D[] ignoreCollisionsWith;
    public int ignoreCollisionsWithCount;
	[HideInInspector][SerializeField]  DamageMaker damageMaker;
	float timeToDisableCollider;
	[HideInInspector][SerializeField] EnergyManager energyManager;
	[HideInInspector][SerializeField] Collider2D goodGuysCol;
	[HideInInspector][SerializeField] Collider2D badGuysCol;
	float pixelsDestroyedFactor = 1f;
	[HideInInspector][SerializeField] AudioManager aM;
	[Header ("--- AUDIO -----------------------")]
	//@Header ("(se vacia despues de cada explosion)")
	public AudioClipArray audioTemp; // lista de audio temporal que la copia aqui explosion invoker
	public AudioClipArray audioOnExplosion; // si no hay audio temporal se reproduce uno de aqui
	public AudioClipArray audioOnHitGround; // si no exist esta y chocamos con ground usaremos la anterior
	float audioDur;
    [HideInInspector] public SWizSpriteMaterialAssigner matAssigner;
	[Header ("------------- estas cuatro ya no se usan!?!?--------------")]
	public int sortingOrder; // no se usa?!?!
	public int sortingOrderIncrease=10;
	public float timeToIncreaseSortingOrder=10f; // YA NO SE USA !!!!?
	public int sortingOrderOffset=2;
	int originalSortingOrder; // ya no se usa?!?!?
	
    static string emptyString = "";
    bool useSpriteScaleOverride;
    
}
