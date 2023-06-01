using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using Light2D;
using UnityEngine.Serialization;

public enum GadgetType {Proyectiles_Weapon, Light, Flyer, WaterGun};
public enum GadgetPlacement {Arms, BackPack, AlreadyInPlace}

public class Gadget : MonoBehaviour {

	[System.Serializable] public class Model
	{
		[ReadOnly2Attribute] public Transform main;
		[ReadOnly2Attribute] public Rigidbody2D rb;
		[ReadOnly2Attribute] public Vector3 mainOriginalScale;
		[ReadOnly2Attribute] public Gadget myGadget;
		public Transform offset;
		[ReadOnly2Attribute] public Vector3 originalOffset;
		public Vector3 gesticulatingOffset;
		public Vector3 ladderOffset;
		public GameObject mounted3Dmodel;
		public GameObject unMounted3Dmodel;
		[System.NonSerialized] public Gadget mountedOn;

		private Transform mainParentBackup;

	}

	public enum SetWithOwnerMode {Default, InstantHide, InstantDraw}
	[Header ("GADGET - - - - - - - - - - - -")]
	public bool debug = false;

	public Model _3DWeapon;

	[Header ("- - - -")]

    public Color color;
    public int id = 0; // correspondiente a weaponIndex
	//estas son para que otros script puedan leerlas
	public bool pressMainButtonOnEnable = false;
	public bool setWithCCOnStart = false;
	public Gadget.SetWithOwnerMode setWithOwnerMode = Gadget.SetWithOwnerMode.Default;
	public GadgetType type;
	public Weapon weapon;
	public LightGadget lightGadget;
	public Flyer flyer;
	public WaterGunGadget waterGun;

	public GadgetPlacement placement = GadgetPlacement.Arms;
	public Vector3 placementOffset = new Vector3(0,0,-0.001f);
	public bool[] armsUsed;
    public float defaultAimAngle = 90;
    public bool aimWithFreeArm = false;
	public bool rotateOnDrawHide = true;
	public float hideDrawToAngle = 0f; // usado por Arms.HideWeapon() para rotar el arma hasta este angulo antes de comenzas la animacion Hide
	public bool shotAtStart = false;
	public float shotAtStartDelay = 0f;
	public bool useProjectileHelper;
	public ProjectileHelper projectileHelper;

	[Header ("AMMO - - - - - - - - - - - -")]
	public bool infiniteAmmo = false;
	bool LimitedAmmo(){return !infiniteAmmo;}
	 public float ammo = 500;
	 public float maxAmmo = 500;
	 protected ArcadeText ammoArcadeText;

	[Header ("AUDIO - - - - - - - - - - - -")]
	public AudioClipArray audioOnDrawn;// lo dispara arms
	public AudioClipArray audioOnHide;// lo dispara arms
	public AudioClipArray audioFire;
	public AudioClipArray audioFireDown;
	public AudioClipArray audioNoAmmo;
	public AudioClipArray audioReleaseFire;
	public AudioClipArray audioOnSwitchOff;

    [Header("PHRASES - - - - - - - - - - - -")]
    public string[] noAmmo;


    public CC cC;
    public EnergyManager eM;
    bool GotCC(){return cC;}
	public PickUp pickUp;
	public Item item;
	public SWizSprite armSprite;
	public Renderer armSpriteRenderer;
	public List<Renderer> my3dRenderers;
	public AimSlave aimSlave;
	public SWizSpriteAnimator anim;
	public SWizAnimationTriggers animTriggers;
	public AudioManager aM; // al principio lo pilla del arma pero cuando lo coge un nuevo owner se cambia por el AM del owner
	public AudioManager ownAM; // siempre es el del propia arma
	public Renderer parentRenderer;
	public SWizSprite parentSprite; // el sprite del padre que tiene este arma, ya sea un CC o un EnemyGenerator
	public Transform projectileSpawnPoint;
	public Transform lightAttachPoint;
	public SWizSpriteAttachPoint attachPoint;
	public ExplosionManager explosionManager;
	public string parentName= "";
	//public float originalZ; // la loal position z con la que fue creado para poder invertir el signo al dar la vuelta al weapon // esto pudo tener sentido antes pero ya no , y me da problemas metiendo el arma detras de torso si continuas game
    public float timeOfLastShot;
    public int frameOfLastShot;
    
    Coroutine disableGameObjectDelayed;
	protected bool _mainButtonPressed;

	[System.NonSerialized] public bool initialised;

	public virtual void SwitchMainButton(bool onOff){}
	public virtual void SwitchMainButton(bool onOff, Transform givenTransform){}

	int previousNoAmmoIndex;

}
