using UnityEngine;
using System.Collections;



public class SkyManager : MonoBehaviour { // TODO implementar en moons of darsalon MOD he hecho algunos cambios , mejor copiar y pegar todo, implicará añador dos variabls a worldmap, randomSky = true y skyIndex = 0

	[System.Serializable]
	public class SkyTexttureWarap
	{
		public Texture2D tex;
		public string name; // para que quede en proyecto hijo, ya que la textura la pierdo
		public bool scrollable;
		[Range(-1,1)]public float lightness;
	}

	public string DEBUGNOTE;
    public CopySunColor[] copySunColor;
	//[ReadOnly2Attribute] public GameObject currentSky;
	public bool updateGameSkyTexture = false;
	public bool skyTextureScroll = true;
	public OffsetScroller scroller;
	public Vector2 size;
	public Material skyMaterial;
	public Renderer skyRenderer;
	public SkyTexttureWarap[] skyTextures;
	public Texture2D chromaTexture;
	[ReadOnly2Attribute] public SkyTexttureWarap currentSkyTex;
	public Vector2 offset;
	public int skyLeftBorder;
	public int skyDownBorder;
	private Vector2 separation;
	public Vector2 rowsAndCols;
	[SerializeField]
	public Vector2[,] pickPos;
    public float centerMatters;
//	public Vector2 debugPos;
	[HideInInspector] public bool sumAtmosColor;
	public Color cosmosColor;
	public Color sunColor;
	public Color atmosColor;

	[Range (0,0.1f)] public float colorChangeSpeed;
	[Range (0,1)] public float averageColorThreshold = 0.089f;
	public bool dayLightReduction = false;
	[Range (0,1)] public float dayLightReductionFactor = 0.5f;
	[HideInInspector] public float dayLightReductionMultiplier = 1f;

//	[Range (0,1)] public float dayWhiteFactor = 0f; // eliminado por que parece mejor opcion desaturar
//	[HideInInspector] public Color dayWhiteColorAdd;
	public Color realSaturatedSkyColor;
	public HSLColor realSaturatedSkyColorHSL;
	//public float realSaturatedSkyColorLuminance; 
	[Range (0,1)] public float saturation = 1f;
	public Color skyColor;
	public HSLColor hslSkyColor; // Parallax que otros scripts lo puedan usar

	public Color[,] color;
	public Point i = new Point(0,0);
	public static int _Color;
	public static int _ColorSpecial;
	public static int _ColorSpecialAdd;
	public static int _ColorSpecialMult;

	public SkyLights lights;
	public static SkyManager instance;

    public void InitialiseInEditor(){
        copySunColor = GetComponentsInChildren<CopySunColor>();
        lights = GetComponentInChildren<SkyLights>();
        scroller = GetComponentInChildren<OffsetScroller>();
        skyRenderer = scroller.GetComponent<Renderer>();
        if (skyRenderer.material.HasProperty("_ColorSpecial"))
	        sumAtmosColor = true;
        if (skyRenderer)
	        skyRenderer.sharedMaterial = skyMaterial;
    }

    int skyIndex = 0;
    public void EnableCopySunColor()
    {
	    foreach (CopySunColor csc in copySunColor)
	    {
		    csc.enabled = true;
	    }
    }

    private Color skyColorBackup;

}