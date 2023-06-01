using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour {

	public enum Mode { FillLightMapAtStart, VisionBased, GenerateOnEnable };
	[HideInInspector]public bool debug = false;
	[HideInInspector]public bool debug2 = false;
	[HideInInspector]public bool debugTimer = false;
	[HideInInspector]public bool debugPauseAfterAnemyCreated = false;

    public string radarIncoming;
    [HideInInspector] public Mode mode;
	[HideInInspector] public bool fillLightMapAtStart = false;
	bool FillLightMapAtStart(){return mode == Mode.FillLightMapAtStart;}
	bool VisionBased(){return mode == Mode.VisionBased;}
	bool GenerateOnEnable(){return mode == Mode.GenerateOnEnable;}
	[HideInInspector]public float lightThreshold = 0.02f;
	int lightPixelStart= 0; // se caucula con la transform position, en la texture worldlightmap , este reprensenta elindice del light pixel donde se empezara a comprobar si hay luz suficiente para soltar un enemig
	[HideInInspector] public int lightPixelInc= 10; // en la textura worldlightmap este representa cuantos pixeles (indice) incrementaremos para intentar colocar el siguiente enemigo, si se sale de la textura, coo es un indice saldra por el otro lado con el desfaze de x correspondiente y una fila mas alto
	[HideInInspector] public int enemyFillMax = 10;
	int enemyFillCount= 0;
	[HideInInspector] public float updateRate = 0.2f;
	[HideInInspector] public bool useDifficultyLevelsToSetTheAmountOfEnemies = true;
	public int objectAmount = 1; // si negativo = enemigos infinitos
	public int maxObjectsOnScene = 1;
	[ReadOnly2Attribute] [NonSerialized] public int objectsOnScene = 0;
	bool maxEnemiesOnSceneReached = false;

	public enum DelayMode { NoDelayWhenFirstDetected, NoDelayWhenVERYFirstDetected, AlwaysDelay, DifferentDelayOnFirstDetected };
	public DelayMode delayMode = DelayMode.AlwaysDelay;
	[Range(0,10)] public float InitialDelay = 0;
	[HideInInspector] public bool noDelayWhenFirstDetected = true;
	public Weapon.WeaponType playerAmmoThresholdType;

	bool UsingAmmoThresholds() { return playerAmmoThresholdType != Weapon.WeaponType.Undefined; }

	[Range(0,1)]public float playerAmmoThreshold = 1;
	[FormerlySerializedAs("time")] public MinMax spawnDelay;
	[FormerlySerializedAs("switchOnNoEnemiesLeft")] [HideInInspector] public GameObject[] switchOnNoObjectsLeft;

	Vector2 basePosition; // posicion base son las coordenadas a partir de las cuales se calcula la posicion a general el enemigo aplicando luego factores aleatorios
	
	string win; // valdra "up" "down" "left" o "right" dependiendo del lado de la pantalla que resulte ganador en la rifa de los screen borders
	RaycastHit2D groundHit;
	int groundMask;
	bool groundOnly; // se activa cuando grounedWhereCreateWalkerEnemies es groundOnly , y asi poder evitar que se generen enemigos bajo los obstaculos
	[SerializeField] [HideInInspector] SWizSpriteAnimator[] anim; // si hay un animador o bien en el GO principal o en GO hijo se almacena aqui , con preferencia del que esta en GO hijo
	EnemyGeneratorGroup enemyGeneratorGroup;
	int visionRadius;
	[HideInInspector] public Vision vision;
	[HideInInspector] public VisibleObjectList visionList;
	[HideInInspector] public Vector3 pos; // almacenará la posicion deonde se creara el enemigo
	[HideInInspector] public ExplosionManager birthPS;
	[SerializeField] [HideInInspector] Collider2D[] allColliders;
	float originalBirthPSRotation;
	int originalBirthPSSortingOrder;
	bool firstEnemy=true; // para disparar la animacion del generador solo la primera vex que crea enemigos
	[Header ("ALL MODES")]
	[HideInInspector] public ArmyType armyType = ArmyType.WildAnimals;
	private ArmySettings armySettings;
	[HideInInspector] public AudioClipArray audioOnCreateEnemy;
	[HideInInspector] public int audioOnCreateEnemyDataBaseIndex  = -1;
	[HideInInspector] public bool animateOnlyAtFirstEnemy = true; // si esta a false dispara la animacion del generador al crear cada enemigo
	[HideInInspector] public bool moveAnimGOToEnemyBirthPos = true;
	[HideInInspector] public LookAt generatorAnimationLookAt;
	[SerializeField] [HideInInspector] AudioManager aM;
	[SerializeField] [HideInInspector] Door moveMe;
	[SerializeField] [HideInInspector] Weapon weapon;
	[SerializeField] [HideInInspector] Transform projectileTrans;
	[FormerlySerializedAs("switchOnAllEnemiesDead")] [HideInInspector] public GameObject[] switchOnAllObjectsDead;
	[SerializeField] [HideInInspector] EnergyManager eM;
	[FormerlySerializedAs("bornLookingAt")] [FormerlySerializedAs("bornLookingTo")] [Header ("OBJECT/ENEMY CUSTOMIZATION")]
	public LookAt spawnLookingAt = LookAt.Target;
	[HideInInspector] public bool followPlayerOnlyOnRealGround = false;
	[HideInInspector] public bool ignoreEnemyBirthLayerOnceIsborn = false;
	[HideInInspector] public bool collidersDeactivatedOnStart = false;
	[FormerlySerializedAs("enemyEnergyMultiplier")] [Range(0.5f, 10)] public float energyMultiplier = 1;
	[HideInInspector] public bool copyVisionToAI = false;
	bool CopyVisionToAI(){return copyVisionToAI;}
	[HideInInspector] public Collider2D copyToAIVisionCol;
	[HideInInspector] public BoxCollider2D myCopyToAiVisionBoxCol;
	[HideInInspector] public CircleCollider2D myCopyToAiVisionCircleCol;
	[HideInInspector] public bool getZFromPlayer = false;
	[HideInInspector] public float enemyWeight;
	[FormerlySerializedAs("enemyStartVelocity")] public RandomVector2 velocityAtSpawn;
	[FormerlySerializedAs("enemyStartImpulse")] public RandomVector2 impulseAtSpawn;
	[HideInInspector] public List<Item> myCreatedObjects;
	Item[] myCreatedEnemiesArray = new Item[5]; // la uso solo al devolver todos al almacen secuencialmente, necesario por que no puedo iterar la lista mientras los devuelvo ya que se van borrando de ella

	void OnValidate()
	{
		if (noDelayWhenFirstDetected)
			delayMode = DelayMode.NoDelayWhenFirstDetected;
		if (delayMode != DelayMode.DifferentDelayOnFirstDetected)
			InitialDelay = 0;
	}

	public void InitialiseInEditor(){
		if (mode != Mode.GenerateOnEnable)
		{
			if (fillLightMapAtStart)
				mode = Mode.FillLightMapAtStart;
			else
				mode = Mode.VisionBased;
		}
		vision = GetComponentInChildren<Vision>(); // pillo Script

		copyToAIVisionCol = GetComponent<Collider2D>();
		myCopyToAiVisionBoxCol = KuchoHelper.CastToBoxCollider2D(copyToAIVisionCol);
		myCopyToAiVisionCircleCol = KuchoHelper.CastToCircleCollider2D(copyToAIVisionCol);

		if (transform.parent)
			enemyGeneratorGroup = transform.parent.gameObject.GetComponent<EnemyGeneratorGroup>();
		projectileTrans = transform.Find("projectile");
		weapon = GetComponentInChildren<Weapon>();
		anim = GetComponentsInChildren<SWizSpriteAnimator>();
		eM = GetComponentInChildren<EnergyManager>();
		aM = GetComponent<AudioManager>();
		if ( transform.Find("MoveMe"))
			moveMe = transform.Find("MoveMe").gameObject.GetComponent<Door>();
		if (transform.Find("BirthPS"))
			birthPS = transform.Find("BirthPS").gameObject.GetComponent<ExplosionManager>();
		allColliders = GetComponentsInChildren<Collider2D>();
        if (weapon)
            weapon.armyType = armyType;
        else
            Debug.LogError(this + " NO TENGO WEAPON?");

    }

	float cycleStart = 0;
	float cycleStop = 0;
	[HideInInspector] float cycleElapsed = 0;
	[HideInInspector] bool detectedOnceAlready = false; // la primera vez que entra al colider
	[HideInInspector] bool firstDetection = false; //cada vez que entra al collider
	[HideInInspector] bool _continue = false;
	[HideInInspector] float delay;


}
