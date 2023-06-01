using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Metadata.W3cXsd2001; // QUE COÑO ES ESTO?
using UnityEngine.Serialization;


public enum BulletWillGo { GhostsNGoblins, Static, AIM_360_AimSlave, ToPlayer, IsEnemyAndPlaceMark};
public enum WeaponStartPositionMethod { GivenTransform, RandomOffsetFromPlayer, ScreenBorders, ReadGroundAlphaInAllMap , PlayerXAndMapHeight};

public class Weapon : Gadget
{

	public bool debug2 = false;

	[Header("GENERAL SETTINGS - - - - - - - - - - - - - -")]
	public float enableWarmUpTime = 0.1f; // tiemp que tarda el arma en estar lista cada vez que se activa el script (onEnable)

	float enabledTime; // se actualiza cada vez que se enciende el script y se comprueba en la corutina warm up time
	public float updateRate = 0.2f; // frecuenci en la que se ejecuta MyUpdate donde se revisan tiempos de autofire y demas
	bool ready = true; // se activa despues de warm up y se comprueba en PressFire()

	public enum WeaponType
	{
		Undefined,
		LaserGun,
		GroundMaker,
		Grenades,
		LampGun
	}

	public WeaponType weaponType = WeaponType.Undefined;

	[Header("WEAPON SETTINGS - - - - - - - - - - - - - -")]
	public float angleError = 0f;

	float angleErrorSign = 1f; // el error del angulo de dispara hace vibrar el arma una vz arriba y una vez abajo , para evitar que la suerte haga disparar muchos disparos erroneos parecidos
	public bool autoFire = false;
	public bool immediateAutoFire = false;
	public MinMax autoFireTime;
	public Animation unityAnimation; // usada para disparar en characters del tipo flyer 3D vectorial , no sprite
	public bool shotOnButtonRelease = false;
	public float fireButtonSpeedFactor = 1f;
	float finalFireButtonSpeedFactor = 1f;

	float fireButtonPressedAtTime;
	float fireButtonPressedForTime;
	Transform releaseGivenTrans; // si el arma se dispara al soltar y se ha llamado a PressFire(givenTrans) , la trans se almacena aqui para usarla en el momento de soltar el boton
	AudioClip fireAudioClip; // usado para extraer su longitud en segundos y usarla como limite para finalFireButtonSpeedFactor

	[Tooltip("Inherits From CC or EnemyGenerator")] [ReadOnly2Attribute]
	public ArmyType armyType;

	public ArmyType hurt = ArmyType.All;
	int myArmyMask;
	ArmySettings myArmySettings;
	public float ignoreRadius = 20f;
	public Collider2D[] nearColliders = new Collider2D[5];

	[Header("AMMO SETTINGS - - - - - - - - - - - - - -")]
	public ItemStoreFinder ammoStoreFinder;

	public ItemStore ammoStore;
	public ItemStoreFinder ammoStoreAimDownFinder;
	public ItemStore ammoStoreAimDown;
	public int aimDownAngle = 80;
	public int howManyProjectilesAtOnce = 1;
	public Material projectileMat;
	public Color bulletLightColor;

	[Header("PROJECTILE MOVEMENT - - - - - - - - - - -")]
	public BulletWillGo bulletDirectionMode;

	bool GotCCandBulletmodeGNG()
	{
		return BulletModeGNG() & cC;
	}

	bool BulletModeGNG()
	{
		return bulletDirectionMode == BulletWillGo.GhostsNGoblins;
	}

	bool Bulletmode360()
	{
		return bulletDirectionMode == BulletWillGo.AIM_360_AimSlave;
	}

	bool Bulletmode360andCC()
	{
		return Bulletmode360() & cC;
	}

	bool Bulletmode360OrToPlayer()
	{
		return Bulletmode360() | bulletDirectionMode == BulletWillGo.ToPlayer;
	}

	bool Bulletmode360OrToPlayerAndCC()
	{
		return Bulletmode360OrToPlayer() & cC;
	}

	bool IsEnemyAndPlaceMark()
	{
		return bulletDirectionMode == BulletWillGo.IsEnemyAndPlaceMark;
	}

	public float _bulletSpeed = 0.35f;
	public float _characterSpeedInheritRate = 0.2f;
	float _randomBulletSpeed = 0;
	public Vector2 bulletSpeedVector = new Vector2(1, 0.5f);
	public Vector2 bulletSpeedDuck = new Vector2(0.85f, 0.25f);
	public Vector2 characterSpeedInheritRate = new Vector2(0.25f, 0.1f);
	public Vector2 bulletSpeedRandomizer = Constants.zero2;
	public RandomPos markPosition;


	public float rotationOffset = 0f; // se aplica al sprite del proyectil , nada que ver con la direccion en la que se moverá
	public float rotationSpeed = 0f;
	public float rotationSpeedRandomizer = 0f;
	public OnEnableDelayedChanges delayedChanges;

	[Header("PROJECTILE START POS - - - - - - - - - - -")]
	public int bulletStartPosAimOffset = 0;

	public WeaponStartPositionMethod weapStartPosMethod;

	bool IsScreenBorder()
	{
		if (weapStartPosMethod == WeaponStartPositionMethod.ScreenBorders)
		{
			useRandomPos = true;
			return true;
		}

		return false;
	}

	public ScreenBorder screenBorder;
	public bool useRandomPos = true;

	bool ShowRandomPos()
	{
		return useRandomPos;
	}

	public RandomPos randomPos;
	public FindGroundFrom findGroundFrom;

	bool WeFindGround()
	{
		return findGroundFrom != FindGroundFrom.Dont;
	}

	public Vector2 findGroundOffset;
	public bool mustBeInsideWorld = true;
	public bool findClearArea = false;

	bool IsFindClearArea()
	{
		return findClearArea;
	}

	public float clearAreaRadius = 10f;

	bool ShowGroundMask()
	{
		return findClearArea | WeFindGround() | findStraightLineToPlayer;
	}

	public GroundMask whatIsGround;
	public bool findStraightLineToPlayer = false;
	public bool findTransparentPixel = false;
	public int howManyTriesToFindFinalPos = 20;

	[Header("PROJECTILE MISC - - - - - - - - - - - - -")]
	public float projectileLifeFactor = 1f; // sera copiado a bullet.bulletLifeFactor

	public LookAt projectileLookAt = LookAt.ParentLookingAt;
	public bool makeProjectileMyChild = false;
	int groundMask;
	bool groundOnly = false; // de uso interno para hacer raycasts con ground + obstaculo pero luego poder reintentar si encuentra obstaculo , asi evito losobstaculos , no hay otra forma
	bool windowBorderStartRandomX = false;
	bool windowBorderStartRandomY = false;
	RaycastHit2D groundHit;
	[HideInInspector] [SerializeField] ExplosionInvoker explosionInvoker;
	[HideInInspector] public Item[] projectileInstance; // las ultimas bala o balas creadas
	[HideInInspector] public Item lastProjectile; //la ultima bala creada


	[SerializeField]
	public Birth projectileBirth; // plantilla parcial solo pra modificar el nacimiento, añadir un BirthSC al GO Weapon

	//EnemyTemplate enemyTemplate; // la plantilla para inicializar a los enemigos, antes era un CC
	public string fireAnimClipName;
	SWizSpriteAnimationClip fireAnimClip;
	bool fireAnim = false;
	float timeToShot;

	[HideInInspector] [SerializeField]
	ObjectSpawner objectSpawner;

	[HideInInspector] [SerializeField] SimpleEnemyGenerator simpleEnemyGenerator;

	[HideInInspector] [SerializeField] Light2DManager fireLight;
	Vector3 projectileStartPos;
	bool iCanCreateProjectile = false;
	

	bool groundFound = false; // se activa si la rutina de deteccion de suelo encuentra un suelo valido acorde con what is ground
	bool straightLineToPlayerFound = false;
	bool transparentPixelFound = false;
	bool clearAreaFound = false;
	bool isInsideScreenLimits = true; // se activa si la posicion de crear enemigo esta dentro de los limites de la pantalla
	bool isInsideWorldMapPlusTopExtraHeight;




}