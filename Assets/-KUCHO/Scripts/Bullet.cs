using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

using Light2D;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

public enum Flipable {FaceUpDownLeftRight, FaceLeftRight, None}

public class Bullet : MonoBehaviour {
	public bool debug = false;
	public bool debug2 = false;
    public SpritePlane.Type allwaysThisPlane;
    RangeFloat spritePlaneRegion;
    bool forceSpritePlaneRegion;
	public float angle = 0f;
	public bool restoreTransform = false;
	public float spriteScaleInc = 0f;
	public float spriteScaleMax = 1f;
    public float circleColliderInc = 0f;
    [SerializeField] float originalCircleColliderRadius;
	public Vector2 rigidbodyVelocityBackup;
	public Vector2 speed;
	public bool allwaysSameSpeed;
	[ReadOnly2Attribute] public Vector2 givenSpeed;// Vecctor resultante de hacia donde disparamos * weapon.bulletSpeed , lo envia weapon
	[ReadOnly2Attribute] public Vector2 givenSpeedAbs;
	public float rotationSpeed;
	//float rotationOffset; // la apago por que ahora la rotacion se hace en weapon antes de activar
	public bool matchAngleWithMovement = true;
	public float givenRotationSpeed;
	public float clipFpsSpeedFactor = 0f;
	Vector2 startSpeed;
	[HideInInspector]
	public float rotation;//angulo con el que se rota el sprite del proyectil segun sea disparo a los lados arriba o abajo, lo fija Weapon al disparar]
	int destroyMask;
	int stampMask;
	public RandomFloat _bulletLife;
	float bulletLife = 3f; // se calcula con el randomfloat anterior y se guarda aqui
	public float bulletLifeFactor = 1f;
	public Vector2 randomStartPos;
	public bool useSharedAnimationObject = true;
	public int animations;
	public Flipable flipable = Flipable.FaceUpDownLeftRight;
	public GameObject childToActivateWhileAlive;
    [Header("-------- AUDIO ------")]
    public MinMax stopMonoChannelOnDestroy;
    public bool fadeOnDestroy;
    [Range(0f, 1f)] public float gainReductionOnDestroy = 0.5f;
    [Range(-0.2f, 0f)] public float fadeSpeedOnDestroy = 0.005f;
	public BulletSounds sounds;
    [Header("-------- CRASH -- DESTRUCTION -- EXPLOSION ------")]
    [Range(0,1)]public float transPosCollPointLerp = 0.5f;
    [ReadOnly2Attribute] public ArmyType armyType; // lo fia weapon
    [ReadOnly2Attribute] public bool isPlayer; // lo fia weapon
    [ReadOnly2Attribute] public ArmySettings armySettings; // lo fija Weapon
	public BulletMask destroyOnCrashWith;
	public BulletMask stampOnCrashWith;
    [ReadOnly2Attribute] public ExplosionInvoker explosionInvoker;
    public ExplosionInvoker noStampExplosionInvoker; 
    public ExplosionInvoker gasExplosionInvoker;
    public Texture2D destroyShape;
	public bool destroyAtEndOfLife = false;
	public bool stickToOnCrash = false;
	public bool disableOnStick = false; // esto ha dejado de funcionar, si desactivo el padre, el hijo que es el palito de luz propiamente dicho no funciona por que el padre esta apgado
	public int onStickLocalZ = 1;
	public BulletMask stickTo;
	int stickToMask;
	public bool zeroVelocityOnCrash = false;
	public float collisionZ; // lo fija CC con el mismo sorting order que cC.ground , para detectar colisiones con playerBulletCollisioner solo si tienen el mismo Z
	bool crashCompleted = false;
	double destroyTime; // almacenará el momento en el tiempo cuando sera destuida la bala (momento del golpe + duracion del sonido explosion)
	double vanishTime; // el momento en el tiempo en que la bala muere de vieja
	AudioClip audioClipHit; // el audio clip de la explosion para calcular el momento de destruccion del gameobject bala
	string crashAnim="";
	SWizSpriteAnimationClip vanishAnimClip = null;
	float vanishAnimDuration = -1f;
	SWizSpriteAnimationClip crashAnimClip;
//	bool thereIsCrashAnimation = false; // para saber realmente si se encontro animacion crash o no , ya que unity pone a default los clips que se pueden ver en inspector 
	bool crashing= false; //flag para saber si ya esta explotando mas rapido que comprobar la animacion en curso y asi ademas sirve para balas sin animador
	[Header ("(nadie usa esta variable ya... o si?)")]
	public int pixelSumMin = 3;
	[Header ("------------------------------------------------------------------------------------")]
    public GameObject goWhoShotMe; // el enemigo padre que crea esta bala, viene dado por el personaje
    public AI aiWhoShotMe; // el enemigo padre que crea esta bala, viene dado por el personaje
	public Collider2D[] ignoreCollisionsWith; // viene dada por la weapon quela dispara y esta la pilla del CC que la dispara
    public int ignoreCollisionsWithCount; // el numero de colliders a ignorar ya que la tabla es fija a 20 pero no siempre esta llena , viene dado por weapon
    [HideInInspector] public Item item; // el script item de este mismo gameobject que tiene todos los datos sobre el item store
	int groundMask;
	int enemyGroundMask;
	int playerGroundMask;
	string oppositeTag;
	string noHurtTag;
	Rect groundRect; // el rectangulo de la textura Game.ground, no deberia esto estar alli? dependera de como lo haga al final cuando meta varias texturas de mapa en cada nivel
	public float bulletTimeIncMult = 1f;
	public float bulletTimeLimitMult = 1f;
	public GameObject markPrefab;
	public GameObject destructionShapePrefab;
	float pixelsDestroyedFactor;
	
	[HideInInspector][SerializeField] SWizSpriteAnimator anim;
	[HideInInspector][SerializeField] SWizSprite spr;
	[HideInInspector][SerializeField] Renderer _renderer;
	[HideInInspector][SerializeField] public DamageMaker damageMaker;
	[HideInInspector][SerializeField] AudioManager aM;
    [HideInInspector][SerializeField] ExplosionStampExtras explosionStampExtras;
	[HideInInspector][SerializeField] public Rigidbody2D rb2D;
	[HideInInspector][SerializeField] DualBodyManager dualBodyManager;
	[HideInInspector][SerializeField] Collider2D myHarmlessCollider; // las granadas (aparte de la explosion) tienen colider normal que no hace daño, para rebotar con las paredes etc
    [HideInInspector][SerializeField] CircleCollider2D myCircleCollider; // las granadas (aparte de la explosion) tienen colider normal que no hace daño, para rebotar con las paredes etc
    [HideInInspector][SerializeField] LampCone lampCone;
	[HideInInspector][SerializeField] Vector3 scaleBackup;
	[HideInInspector][SerializeField] Vector3 spriteScaleBackup;
	[HideInInspector][SerializeField] float originalAlpha;

	bool allCollidersEnabled = false;
	bool weShouldStamp;
	// GameObject player;

	[System.Serializable]
	public class BulletSounds{
		public AudioClipArray start;
		public AudioClipArray collision;
		public AudioClipArray vanish;
		public AudioClipArray destruction;
	}
	
}
