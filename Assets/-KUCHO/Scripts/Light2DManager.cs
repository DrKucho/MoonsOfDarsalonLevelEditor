using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

using Light2D;
using Random = UnityEngine.Random;

[System.Serializable]
public class GlassReflectsSkyColorSettings{
    public Color color;
    public float greyBoost;
    public float colorBoost;
    public float greyScaleShift;
    public float skyColorIntensity;  
}
public class Light2DManager : MonoBehaviour {

	
	
	public enum LightOscilateType {None, Random, PingPong}
	public bool debug = false;
	[Header (" G E N E R A L - - - - - - - - - - - - - - - - - - - - ")]
	public float capacity = 100f;
	public float charge = 100f;
	public float power = 1f;
	public float dischargeRate = 0.1f;
	public ThreeWaySwitch onStartSwitch = ThreeWaySwitch.SwitchOn;
	public ThreeWaySwitch OnEnableSwitch = ThreeWaySwitch.SwitchOff;
	public bool fullBattOnEnable = false;
	public bool updateGameHud = false;
    [SerializeField] public Rigidbody2D rb;
	[Header ("O S C I L A T I O N - - - - - - - - - - - - - - - - - - - - ")]
	public LightOscilateType oscilate = LightOscilateType.None;
	public float intOscilation;
	public float oscSpeed = 0.1f;
	[ReadOnly2Attribute] public float oscAmount;
	public int oscAmountDirection = 1; // cambia entre 1 y -1 para saber si incrementamos o decrementamos
	[Header ("P O S I T I O N - - - - - - - - - - - - - - - - - - - - ")]
	public bool forceZ_ToGetInsideWLM = true;
	public bool updatePosition = false;
	public Transform copyPosOf;
	public bool zeroLocalPosition = false;
	public Vector3 offset;
	public bool moveWithAim = false;
	public float aimDistFactor = 10f;
	public bool rotateWithAim = false;
    [Header ("I N T E N S I T Y - - - - - - - - - - - - - - - - ")]
	public float atThisBatteryFactorLightWillDecrease = 0.6f;
	/// <summary>
	/// rangeFactor will be calculated depending on Power and LErped by this value and 1, then light2D Sprite Scale will be multiplyed by rangeFactor;
	/// </summary>
	public float minRangeFactor = 0f;
	public LimitMethod intensityLimitMethod = LimitMethod._1; // metodo nuevo en el que solo reduce si se pasa del Max, ppuede servir para alunas cosas, pero algo pasaba que para no quemar a ant no iba muy bien
    public bool getWorldLightReaderFromOwner;
    [Tooltip("Ambient Light Switch")] public WorldLightReader worldLightReader;
	public bool useWorldLightReaderOnlyForGlare = false;
    bool GotWLR() { return worldLightReader | getWorldLightReaderFromOwner; }
    bool GotWLRAndLimit2DLight() { return (worldLightReader | getWorldLightReaderFromOwner) && !useWorldLightReaderOnlyForGlare && intensityLimitMethod != LimitMethod.NoLimiting; }
	[Space(5)] public bool wlmRelative = false;
    [Range(-20,20)] public float wlmMult = 0.00009f;
	[Space(5)] public float wlmAmountToAdd;
	public enum LimitMethod {_1, _2, NoLimiting}// el 1 es el original que, ahora que calculo la luz bien parece que funciona mejor, pero aun asi no termino de entender el limit_Max, tiene sentido siempre que este a 1
    bool IntensityLimiting() { return intensityLimitMethod != LimitMethod.NoLimiting; }
    [Range(0,1)]
	public float intensityLimitTh_Min = 0f;
	[Range(0,1)]
    public float intensityLimitTh_Max = 1f;
	[Range(0, 1f)]
    public float intensityLimitRatio = 0.5f; // cuando sobrepasa el threshold.min se empieza a aplicar este ratio proporcionalmente a lo que se pasa, Esto creo que ya no pasa---> si le meto mas de 0.5 la luz se vuelve loca oscilando arriba y abajo sin encontrar el punto medio nunca
	[Range(0,1)]
	public float visibility; // copia de worldLightReader.visibility
	[Header ("para debugear")]
	[ReadOnly2Attribute] public float excess = 0f; // lo que nos estamos pasando con respecto al limite de luminosidad
    [ReadOnly2Attribute] public float intensityLimitSub; // el objetivo cuando se ajusta la luz, este es el valor al que hay que llegar
    [ReadOnly2Attribute] private float intensityLimitSub2; // este es el que se va incrementando poco a poco hasta que llega al otro
    [NonSerialized] public ColliderSettings colliderSettingsModifier;
    [Space(8)]
    public float intensityLimitCatchUpSpeed = 0.05f;
    public bool intensityCatchUpLerp = false;
	public bool instantLimitAdjust = false;
	private bool _instantCatchUp = true;
	public Light2DManager dimWhenThisLightIsOn;
	public float dimRatio;
	private float dimAmount = 0f;
	[HideInInspector]
	public float intensityFactor = 0f; // se multiplicara por originalintensity para ajustar la inteisidad en cada frame, se calcula sumando las otras variables segun cookie, oscilacion etc
	[Header ("S O U N D - - - - - - - - - - - - - - - - - - - - ")]
	public float onOffGain = 0.5f;
	public AudioClipArray audioOn;
	public AudioClipArray audioOff;
	public AudioClipArray audioNoBattery;
	[ReadOnly2Attribute] public AudioManager aM;
	[Header ("C O L O R - - - - - - - - - - - - - - - - - - - - ")]
	public MinMax randomColor;
	[Header ("O W N - S P R I T E - - - - - - - - - - - - - - - - - - ")]
	public SWizSprite haloSprite; // un sprite adicional normal y corriente para crear halos de luz o bombillas encendidas
	[Header ("MATERIAL GLASS REFLECTS SKY - - - - - - - - - - - - - - - ")]
    public Material glassMaterial;
    public GlassReflectsSkyColorSettings glassColorOn;
    public GlassReflectsSkyColorSettings glassColorOff;
    [Header("L I G H T - S P R I T E - - - - - - - - - - - - - - - - ")]
    public bool lockLightSpriteInPlane;
    public float lockPlane;
    public bool billBoardMode;
    public float allLSprsAlphaMult = 1;
    public LightSprite[] _lSprite;
	public LSprite[] lSprite;
	
	private float powerFactor = 1f;
	private float rangeFactor = 1f;
	private float realPower;

	public SpriteRenderer glare;
	public float glareOriginalIntensity;
	public GlareReducer glareReducer;
	// float lSpriteObstacleMult; // eliminado porque lightSprite vuelve a crear el material en cada frame, creo
    [SerializeField] Weapon weapon;
	private CC cC;
	private Aim aim;
    [SerializeField] Transform _transform;
	private Light2DManager[] mainLightToSwitch; // durante el fogonazo de disparo se apaga las luces princupales para ahorra gpu y evitar que se queme en persnaje, viene dada desde weapon en el momento de disparar y se vacia aqui despues de volverla a encender
	private bool mainLightPendingToBeOn = false;
	
	[Header ("INTENSITY LIGHT 3D - - - - - - - - - - - - - - - - - - ")]
	public Light l;

	
    [SerializeField] float originalIntensity;
    [SerializeField] float originalRange;
	private Vector3 position;
	public Collider2D lightCollider;
	
	[System.Serializable]
	public class LSprite{
        public bool billBoardMode;
		public LightSprite lSprite;
        public Renderer _renderer;
        public Material _material;
        public Color originalColor;
		public Vector3 originalScale;
		public float originalObstacleMult;
		public float alpha;
	}
    
    public void InitialiseInEditor(){
        aM = GetComponentInParent<AudioManager>();
        l = GetComponent<Light>();
        if (l)
        {
            originalIntensity = l.intensity;
            originalRange = l.range;
        }
        _lSprite = GetComponentsInChildren<LightSprite>();
        lSprite = new LSprite[_lSprite.Length];
        for (int s = 0; s < lSprite.Length; s++)
        {
            lSprite[s] = new LSprite();
            lSprite[s].lSprite = _lSprite[s];
            lSprite[s]._renderer = _lSprite[s].gameObject.GetComponent<Renderer>();
            lSprite[s].originalScale = lSprite[s].lSprite.transform.localScale;
            lSprite[s].originalColor = lSprite[s].lSprite.Color;
            if (lSprite[s]._renderer && !lSprite[s]._material) // tenemos renderer pero nos falta el material? esto viene de un error que pasaba al intentar ejecutar los initineditor en el prefar game cargado desde la assetdatabase, pero creo que ya no pasa nunca
            {
                if (!lSprite[s]._renderer.sharedMaterial)
                    print(this + " NO PUEDO PILLAR MATERIAL EN " + lSprite[s]._renderer.name); 
                else
                    lSprite[s]._material = lSprite[s]._renderer.sharedMaterial; // solo la quiero para verla en inspector y comprobar que la pillo, tengo un error raro en inicializacion en el prefab
            }
            if (lSprite[s]._material)
	            lSprite[s].originalObstacleMult = lSprite[s]._material.GetFloat("_ObstacleMul");
            else
	            Debug.LogError("LIGHTSPRITE " + lSprite[s].lSprite.name + " NO TIENE MATERIAL" );
            
        }
        //  if (lSprite){
        //      lSpriteOriginalScale = lSprite.transform.localScale;
        //      lSpriteOriginalColor.a = lSprite.Color.a;
        //      lSpriteOriginalObstacleMult = lSprite.renderer.material.GetFloat("_ObstacleMul");
        //      lSpriteOriginalColor = lSprite.Color;
        //  }
        glare = GetComponentInChildren<SpriteRenderer>();
        if (glare)
	        glareOriginalIntensity = glare.color.a;
        glareReducer = GetComponentInChildren<GlareReducer>();
        weapon = GetComponentInParent<Weapon>();
        _transform = GetComponent<Transform>();
        if (!getWorldLightReaderFromOwner)
	        worldLightReader = GetComponentInChildren<WorldLightReader>();
        if (!rb)
            rb = GetComponent<Rigidbody2D>();
    }
}
