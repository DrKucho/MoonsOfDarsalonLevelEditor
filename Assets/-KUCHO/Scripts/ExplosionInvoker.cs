using UnityEngine;
using System.Collections;

using UnityEngine.Profiling;
using UnityEngine.Serialization;

[System.Serializable]
public class Explosion {
	public ArmyType hurts;
	[Range (0,200)] public float colliderRadius;
	[Range (0,400)] public float power;
	public Collider2D[] ignoreCollisionsWith;
    [ReadOnly2Attribute] public int ignoreCollisionsWithCount;
    public ExpPart[] part;
	public ExplosionManager[] pS; // esto es confuso , creo que hice esta para poder usarla comodamente en el momento de la explosion repasando todas las explo.parts que se van trayendo del almacen, va apuntando a la que acaba de crear , asigna valores, inicializa y tal y luego puntara a la siguiente part
	[HideInInspector]
	public float[] duration; // y con esta creo que lo mismo
	//Constructor
		public Explosion(){
			hurts = ArmyType.All;
			colliderRadius = 1f;
			power = 1f;
			ignoreCollisionsWith = new Collider2D[0];
			part = new ExpPart[1];
		}
}

[System.Serializable]
public class ExpPart{
	public ItemStoreFinder storeFinder;
	[HideInInspector] public ItemStore store;
	[HideInInspector] public string findThisExplosionStore;
	[Tooltip("ExplosionStamExtras Calcula el extremo de opuesto al disparo del agujero que va a dejar la explosion, pero es demasiado grande por que lo hace en funcion al tamaño de la stamp Tex incluyendo su zona de pixeles semitransparentes, por eso se necesita este ajuste")]
	[Range(0,1)] public float stampTexOffsetRatio;
	public float offsetDelay;
	public Vector3 offset;
//	public bool setScale = true; // OJO! bug en unity 5.3.5f1 si cambio la escala aun para dejarla igual se desconecta el localPosition del position, cambias el localPosition y su valor cambia pero no afecta al World Position, por eso puse esto
	public Vector3 rotation;
	public Vector3 scale;
    public bool inheritMat;
    bool ShowMat() {return !inheritMat;}
    public Material material;
	public bool copyThisColor;
    bool ShowColor() { return copyThisColor; }
    public Color color = Color.green; // solo para ver si sale
	public Material lightSpriteMaterial;
	public bool inheritLightSpriteColor;
    bool ShowLightSpriteColor() { return !inheritLightSpriteColor; }
    public Color lightSpriteColor;
	public AudioClipArray audioArray;
	public bool notBasedOnParticleRateMult;
    bool ShowOverrides() { return useParticleRateMultOverrids && !notBasedOnParticleRateMult; }
    bool ShowUseOverrides() { return !notBasedOnParticleRateMult; }
	public bool useParticleRateMultOverrids;
    public float particleRateOverride = 1;
    public float particleSpeedOverride = 1;
    //Constructor
    public ExpPart(){
		storeFinder = new ItemStoreFinder();
		stampTexOffsetRatio = 0f;
		offset = Constants.zero3;
		offsetDelay = 0f;
		scale = new Vector3(1,1,1);
		rotation = Constants.zero3;
		material = null;
		lightSpriteMaterial = null;
		color = Color.white;
		copyThisColor = false;
		inheritMat = false;
		inheritLightSpriteColor = false;
		audioArray = new AudioClipArray();
		notBasedOnParticleRateMult = false;
	}
    public override string ToString()
    {
        if (storeFinder.store)
            return storeFinder.name;
        else
            return storeFinder.name + "(MISSING STORE)";
    }
}

public class ExplosionInvoker : MonoBehaviour {

    public KuchoDate initialisedOnEditorDate;
    public bool debug = false; 
	public ArmyType armyType; // la fija la bala que a su vez se la fijo el arma, sirve para evitar fuego amigo con los colliders ignore de cada armada
	[Range (0,1)] public float minRandomSizeFactor;
	[Range (0,1)] public float maxRandomSizeFactor;
	public Transform stickTo;
	public Explosion hurt;
	public Explosion death;
	public Explosion fire;
    [SerializeField] CC cC;
    //	Bullet bullet;
    [SerializeField] Weapon weapon;
    [SerializeField] Renderer rend;
	[SerializeField] Item item;
	[HideInInspector] public float deathDuration;
	[HideInInspector] public GrowAnimator growAnim;

    public KuchoDate DayD = new KuchoDate(2019, 05, 15, 13, 30, 0);
    public void Reset(){
		armyType = ArmyType.All;
		hurt = new Explosion();
		death = new Explosion();
		fire = new Explosion();
	}



}
