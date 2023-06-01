using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using Unity.Collections;
using UnityEngine.Profiling;


public enum DecoModifierMode {Destroy, Create, None};
public enum GroundModifierMode {Destroy, Create, None}
[System.Serializable]
public class GroundmodifierModeOptions{
    public bool detailTears;
    public bool polish;
    public bool fixCorners;
    public bool searchIsolatedPixels;
    public bool lookForThinAreas;
    public ThinCheckOptions thinCheck;
}
[System.Serializable]
public class ThinCheckOptions
{
    public float offset = 5;
    public float distance = 5;
    public float inc = 5;
    public float stampOffset = 2;
}
public enum CollideWithTag {Destructible, Indestructible, BothTerrains, BothTerrainsAndUntaggedGround, Anything}

public enum MakesChangesOn
{
	Destructible,
	Indestructible
};//, BothTerrains, BothButIndestructibleAlsoHalfTrnasparentStampPixels, Background}

[ExecuteInEditMode]
public class ExplosionStampExtras : MonoBehaviour {
	
	[HideInInspector] public bool debug = false;
	[HideInInspector] public bool debug2 = false;
	[HideInInspector] public bool debugBreak = false;
	[HideInInspector] public bool useModifierKeysInEditor = false;
	[HideInInspector] public bool forceWrite = false;
	public MakesChangesOn makesChangesOn = MakesChangesOn.Destructible;
	public GroundModifierMode groundModifierMode;
    public GroundmodifierModeOptions createGroundOptions;
    public GroundmodifierModeOptions destroyGroundOptions;
    [HideInInspector] public DecoModifierMode decoModifierMode;
    public enum StampTextSelectMode{ Cycle, Random, ScaleToSolidPixelRatio, Fixed_Editor_SetExternally, UnSet}
    [HideInInspector] public StampTextSelectMode stampTextSelectMode = StampTextSelectMode.UnSet;
	[HideInInspector] public bool cycleStampTex = false;// borrame
	[HideInInspector] public bool randomStampTex = true; // borrame

	[HideInInspector]public CollideWithTag stampOnHitWithTag;
	[HideInInspector] public float maxScaleToMaxSprite = 1;
	[HideInInspector] public int stampI; // indice que apunta a la stampTex usada
	[ReadOnly2] public Texture2D stampTex; // la ultma stamptex usada , asi bullet puede acceder
	[HideInInspector] [Range(0, 1)] public RangeFloat goodGuysStampRange = new RangeFloat(0, 1);
	[HideInInspector] [Range(0, 1)] public RangeFloat badGuysStampRange = new RangeFloat(0, 1);
	[HideInInspector] public StampSprite[] stampSprites;
	PolygonCollider2D poly;
	[HideInInspector] public LPFixturePoly fixPoly;
	[HideInInspector] public int isolatedPixelsAreaAdd = 10;
	[HideInInspector] public int smallRegionThreshold = 50;
	[HideInInspector] public DecoManager decoManager; // ahora solo se destruye Gaame.grassManager por simplificar, en un futuro puedo hacer Game.destructibleDecoManagers y que este script lea eso , en lugar de que sea una variable especifica siempre se destruira lo que diga Game, tiene mas sentido creo
	[HideInInspector] public int pixelsDestroyed = 0;
	[HideInInspector] public float pixelsDestroyedFactorMult = 3f;

	// estas dos variables asumen que ninguna textura puede tener mas de 400 pixels medio solidos OJITO!
	private int[] _halfSolidData = new int[400]; // guarda los indices de ground alpha data que apuntan a los pixels del suelo que hay que comprobar si estan aislados
	private int[] _halfSolidPixel = new int[400]; // guarda los indices de la region de textura ground que apuntan a los pixels del suelo que hay que comprobar si estan aislados

	[Header ("Visible Para Debuguear Unicamente")]
	[HideInInspector] public float pixelsDestroyedFactor;
	[HideInInspector] public byte[] groundAlphaDataRegion;
    Terrain2D[] terrains2D;
    Terrain2D terrain2D;
    Texture2D alphaTex;
    byte[] alphaData;	private WorldMap map;
    ExpensiveTaskManager expensiveTaskManager;
	Transform _transform;
    [HideInInspector] public SpriteRenderer spriteRenderer; // en modo editor uso un boton que va cambiando el sprite a usar asi que para que se vea el cambio en la scene tengo que cambiar el sprite, OJO siempre se apagará el sprite renderer en cuadno el juego arranque , asi que los sprites que no son stamp como por ejemplo las balas han de ser SWizSprites
    [HideInInspector] public DualBodyManager bodyManager;

    Point[] checkPoint = {new Point(0,1), new Point(0,-1), new Point(1,0), new Point(-1,0)};

	[System.Serializable]
	public class StampSprite{
		public Sprite sprite;
		[HideInInspector] public byte[] alpha;
		public int totalPixelCount;

		[HideInInspector] public Point[] fullSolidPixel;
		public int fullSolidPixelCount;

		[HideInInspector] public Point[] halfSolidPixel;
		public int halfSolidPixelCount;

		public int totalSolidPixelCount;
		public float solidPixelRatio; // ratio en comparacion con los demas sprites, se calcula fuera de la class una vez que todos los stamp sprites han sido calculados

		public override string ToString()
		{
			if (sprite)
				return sprite.name;
			return "";
		}

		public void Initialize()
		{
			int a = 0;
			int b = 0;
			Initialize(ref a, ref b);
		}
		/// <summary>
		/// Cuenta los pixeles solidos, y mas cosas para que pueda ejecutarse correctamente el codigo de explosion.
		/// los paramentros son para, al inicializar todas las texturas, sacar los maximos y minimos comparando todas ellas y establecer los ratios
		/// </summary>
		public void Initialize(ref int maxSoliPixelCount, ref int minSolidPixelCount){
			fullSolidPixelCount = 0;
			halfSolidPixelCount = 0;
			totalSolidPixelCount = 0;
            if (sprite)
            {
                Texture2D tex = sprite.texture;
                if (tex.name != "ClipBoard") // el clipboard puede tener pixeles en los bordes
                {
                    if (HasSolidPixelsOnSide(tex.GetPixels(0, 0, tex.width, 1)))
                        return;
                    if (HasSolidPixelsOnSide(tex.GetPixels(0, 0, 1, tex.height)))
                        return;
                    if (HasSolidPixelsOnSide(tex.GetPixels(tex.width - 1, 0, 1, tex.height)))
                        return;
                    if (HasSolidPixelsOnSide(tex.GetPixels(0, tex.height - 1, tex.width, 1)))
                        return;
                }
                var pixels = tex.GetPixels(); // pilla toda la textura
				totalPixelCount = pixels.Length;
				alpha = new byte[totalPixelCount];
				for (int n = 0; n < pixels.Length; n++)
				{
					alpha[n] = (byte)(pixels[n].a * 255f); // saca el dato de transparencia FLOAT TO BYTE no puede ser mayor que 1 nunca asi que la conversion ha de ir bien
					if (alpha[n] > 127)
					{
						totalSolidPixelCount++;
						if (alpha[n] > 200)
							fullSolidPixelCount++;
						else
							halfSolidPixelCount++;
					}
				}
				if (totalSolidPixelCount > maxSoliPixelCount)
					maxSoliPixelCount = totalSolidPixelCount;
				if (totalSolidPixelCount < minSolidPixelCount)
					minSolidPixelCount = totalSolidPixelCount;
				fullSolidPixel = new Point[fullSolidPixelCount];
				halfSolidPixel = new Point[halfSolidPixelCount];
				int si = 0;
				int hi = 0;
				for (int x = 0; x < tex.width; x++)
				{
					for (int y = 0; y < tex.height; y++)
					{
						float a = tex.GetPixel(x, y).a * 255f;
						if (a > 127)
						{
							if (a > 200)
							{
								fullSolidPixel[si] = new Point(x, y);
								si++;
							}
							else
							{
								halfSolidPixel[hi] = new Point(x, y);
								hi++;
							}
						}
					}
				
				}
            }
		}
        bool HasSolidPixelsOnSide(Color[] sidePixels){
            foreach (Color p in sidePixels)
            {
                if (p.a > 0)
                {
                    Debug.LogError("STAMP SPRITE (" + sprite.texture.name + ") EN ESCENA " + SceneManager.GetActiveScene().name + " NO ES VALIDO, TIENE PIXELES SOLIDOS EN ALGUN CONTORNO");
                    return true;
                }
            }
            return false;
        }
		public void Rotate90(){
			if (sprite.texture)
			{
				Texture2D newTex = TextureHelper.Rotate90(sprite.texture);
				sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f,0.5f),1);
			}
		}
		public void MirrorLeftToRight(){
			if (sprite.texture)
			{
				Texture2D newTex = TextureHelper.MirrorLeftToRight(sprite.texture);
				sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f,0.5f),1);
			}
		}
		public void MirrorUpToDown(){
			if (sprite.texture)
			{
				Texture2D newTex = TextureHelper.MirrorUpToDown(sprite.texture);
				sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f,0.5f),1);
			}
		}
	}
	//float rayLength;
	//GameLayerMask layerMask;
	//private uint _layerMask;
	// float angleInc;
	//private Vector2 dir;
	//private Transform startAngle;
	//private Transform startPoint;
	//private Transform endAngle;
	//private Transform endPoint;
	//private Transform originalStartAngle;

	void OnValidate(){
		if (isActiveAndEnabled)
		{
			if(gameObject.activeInHierarchy == false && useModifierKeysInEditor)
				useModifierKeysInEditor = false; //TODO gameObject.activeInHierarchy == false ???
			if (useModifierKeysInEditor == true)
				stampTextSelectMode = StampTextSelectMode.Fixed_Editor_SetExternally;
			InitialiseInEditor();
		}
	}
	
	void Awake () {
		expensiveTaskManager = Game.expensiveTaskManager;
        terrains2D = new Terrain2D [1];
		_transform = transform;
		InitWhatsNeededOnRunTime();
	}
	void Start(){ //  print(this + "START ");
		InitialiseInEditor();
        expensiveTaskManager = Game.expensiveTaskManager;
        Game.onFindLevelSpecificStuff += OnFindLevelSpecificStuff;
        if (stampTex == null && stampSprites != null && stampSprites.Length > 0 && stampSprites[0] != null && stampSprites[0].sprite != null) // no recuerdo por que hice esto
			stampTex = stampSprites[0].sprite.texture;
	}

    public void OnFindLevelSpecificStuff(){ // a esta funcional la llama Game en cada nivel, es mejor que OnLevelWasLoaded por que a esa no se la llama si el GO esta apagado (item en estore)
		InitWhatsNeededOnRunTime();
	}
	void OnDestroy(){
		Game.onFindLevelSpecificStuff -= OnFindLevelSpecificStuff;
	}
	public void InitialiseInEditor(){
		if (!spriteRenderer)
			spriteRenderer = GetComponent<SpriteRenderer>();
		if (!spriteRenderer)
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		if (!poly)
			poly = GetComponent<PolygonCollider2D>();
		if (!fixPoly)
			fixPoly = GetComponent<LPFixturePoly>();
        bodyManager = GetComponent<DualBodyManager>();
		SetStampSprite(stampI);
		LookForDestructibles();
		_transform = transform;
		if (stampTextSelectMode == StampTextSelectMode.UnSet)
		{
			if (randomStampTex)
				stampTextSelectMode = StampTextSelectMode.Random;
			else if (cycleStampTex)
				stampTextSelectMode = StampTextSelectMode.Cycle;
			else
				stampTextSelectMode = StampTextSelectMode.Fixed_Editor_SetExternally;
		}
        //ResetTData();
	}
//    byte[] tData;
    //void ResetTData(){
        //int biggerSize = int.MinValue;
        //foreach (StampSprite ss in stampSprites)
        //{
        //    if (ss.sprite && ss.sprite.texture)
        //    {
        //        int size = ss.sprite.texture.width * ss.sprite.texture.height;
        //        if (size > biggerSize)
        //            biggerSize = size;
        //    }
        //}
//        if (tData == null || tData.Length < biggerSize)
//            tData = new byte[biggerSize];
    //}
	public void InitWhatsNeededOnRunTime(){
		LookForDestructibles();
		if (!decoManager)
		{
			decoManager = DecoManager.grassDecoManager;
//			GameObject decoManagerGO = null;
//			decoManagerGO = GameObject.Find(_decoManager);
//			if (decoManagerGO)
//				decoManager = decoManagerGO.GetComponent<DecoManager>();
		}
	}
	[HideInInspector] [SerializeField] int maxSoliPixelCount;
	[HideInInspector] [SerializeField]  int minSolidPixelCount;
	public void InitializeStampSprites(){
		maxSoliPixelCount = int.MinValue;
		minSolidPixelCount = int.MaxValue;

		if (stampSprites != null)
		{
			for (int t = 0; t < stampSprites.Length; t++)
			{
				stampSprites[t].Initialize(ref maxSoliPixelCount, ref minSolidPixelCount);
			}

			for (int t = 0; t < stampSprites.Length; t++)
			{
				stampSprites[t].solidPixelRatio = stampSprites[t].totalSolidPixelCount / (float)maxSoliPixelCount;
			}
		}
	}
	public void SortStampSprites(){
		int winner = 0;
		List<StampSprite> stampList = new List<StampSprite>();
		for (int c = 0; c < stampSprites.Length; c++) // copia la tabla a la lista
			stampList.Add(stampSprites[c]);
		
		stampSprites = new StampSprite[stampList.Count]; // borro la array
		int arrayIndex = 0;
		while(stampList.Count > 0) // itero la lista para ir borrando
		{
			float min = float.MaxValue;
			for (int i = 0; i < stampList.Count; i++)
			{
				if (stampList[i].solidPixelRatio <= min)
				{
					winner = i;
					min = stampList[i].solidPixelRatio;
				}
			}
			stampSprites[arrayIndex] = stampList[winner]; // me quedo con el ganador en la posicion de la tabla que toca
			arrayIndex ++; // incremento para la proxima
			stampList.RemoveAt(winner); // borro de la lista el que ya he copiado
		}
	}
#if UNITY_EDITOR
    void OnEnable(){ //  print(this + " ONENABLE ");
        if (!Application.isPlaying && !UnityEditor.BuildPipeline.isBuildingPlayer) // estamos parados , y no no haciendo una buiild
        {
            InitializeStampSprites();
            InitialiseInEditor();
        }
	}
#endif
    public void LookForDestructibles(){
		if (!WorldMap.instance)
		{
			map = FindObjectOfType<WorldMap>();
			if (map)
				map.AssignStaticTerrainVariables();
		}
		if (Application.isPlaying || WorldMap.instance)
		{
			map = WorldMap.instance;
			if (map)
			{
				switch (makesChangesOn)
				{
                    case (MakesChangesOn.Destructible):
                        if (WorldMap.destructible)
                        {
                            if (terrains2D == null || terrains2D.Length != 1)
                                terrains2D = new Terrain2D[1];
                            terrains2D[0] = WorldMap.destructible;
                        }
						break;
                    case (MakesChangesOn.Indestructible):
                        if (WorldMap.indestructible)
                        {
                            if (terrains2D == null || terrains2D.Length != 1)
                                terrains2D = new Terrain2D[1];
                            terrains2D[0] = WorldMap.indestructible;
                        }
						break;
                    /*
                    case (MakesChangesOn.Background):
                        if (WorldMap.background)
                        {
                            if (terrains2D == null || terrains2D.Length != 1)
                                terrains2D = new Terrain2D[1];
                            terrains2D[0] = WorldMap.background;
                        }
						break;
                    default:
                        if (WorldMap.destructible && WorldMap.indestructible)
                        {
                            if (terrains2D == null || terrains2D.Length != 2)
                                terrains2D = new Terrain2D[2];
                            terrains2D[0] = WorldMap.destructible;
                            terrains2D[1] = WorldMap.indestructible;
                        }
						break;
					*/
				}
			}
		}
		else
		{
			if (map)
			{
				switch (makesChangesOn)
				{
					case (MakesChangesOn.Destructible):
						GetDestructibles(true, false, false);
						break;
					case (MakesChangesOn.Indestructible):
						GetDestructibles(false, true, false);
						break;
					/*case (MakesChangesOn.Background):
						GetDestructibles(false, false, true);
						break;
						*/
					default:
						GetDestructibles(true, true, false);
						break;
				}
			}
		}
		terrain2D = terrains2D[0];
    }
	public void GetDestructibles(bool destructible, bool indestructible, bool background){
		int length;
		string whatILookFor = "";
		if (destructible && indestructible)
			length = 2;
		else
			length = 1;
        terrains2D = new Terrain2D[length];
        Terrain2D[] allTerrains2D = FindObjectsOfType<Terrain2D>();
		length = 0; // ahora sirve de indice
        foreach (Terrain2D terr in allTerrains2D)
		{
            if (destructible && terr.gameObject.CompareTag("Destructible")){
				whatILookFor = "Destructible"; // para debug
				terrains2D[length] = terr;
				length ++;
			}
            if (indestructible && terr.gameObject.CompareTag("Indestructible")){
				whatILookFor = "Indestructible"; // para debug
				terrains2D[length] = terr;
				length ++;
			}
            if (background && terr.gameObject.CompareTag("Background")){
				whatILookFor = "BackGround"; // para debug
				terrains2D[length] = terr;
				length ++;
			}
		}
		if (length == 1){
			if (terrains2D[0] == null) print (this + " NO HE ENCONTRADO " + whatILookFor);
		}
		else if (terrains2D[0] == null || terrains2D[1] == null ) print (this + " ME FALTA UNO DE LOS TERRANOS QUE BUSCO: DEST=" + destructible + " INDEST=" + indestructible + " BACKG=" + background);
	}
	public void CycleStampTex(){
		CycleStampTex(1);
	}
	public void CycleStampTex(int inc){
		int count = 0;
		do
		{
			stampI += inc;
			KuchoHelper.WrapInsideArrayLengthIfOutOfrange(ref stampI, stampSprites.Length);
			count++;
		}
		while (stampSprites[stampI].totalSolidPixelCount <= 0 || stampSprites[stampI].sprite == null || count > 200);

		SetStampSprite(stampI);
	}
	public void RandomStampText(ArmyType armyType){
        float min;
        float max;
        float m = (float)stampSprites.Length - 1;
        switch (armyType)
        {
            case (ArmyType.GoodGuys):
                min = goodGuysStampRange.min * m;
                max = goodGuysStampRange.max * m;
                break;
            case (ArmyType.BadGuys):
                min = badGuysStampRange.min * m;
                max = badGuysStampRange.max * m;
                break;
            default:
                min = 0f;
                max = 1f;
                break;
        }
        min -= 0.49f; // para que tengan las mismas posibilidades de salir los extramos, sino en el redondeo salen perdiendo
        max += 0.49f; // para que tengan las mismas posibilidades de salir los extramos, sino en el redondeo salen perdiendo
        stampI = Mathf.RoundToInt(UnityEngine.Random.Range(min,max));
		SetStampSprite(stampI);
	}
	
	public void SetTextByScale(){ // ojo solo funciona si los sprites estan ordenados de menor a mayor
		float scale = (transform.localScale.x + transform.localScale.y) / 2;
		float ratio = Mathf.InverseLerp(0, maxScaleToMaxSprite, scale);

//		print("SCALE=" + scale + " RATIO=" + ratio + " SPC=" + spc);

		bool isSet = false;

		for (int i = 0; i < stampSprites.Length; i++)
		{
			if (stampSprites[i].solidPixelRatio >= ratio)
			{
				SetStampSprite(i);
				isSet = true;
				break;
			}
		}
		if (!isSet)
			SetStampSprite(stampSprites.Length);
	}
	public void SetStampSprite(int i){
		stampI = i;
		if (stampSprites != null)
		{
			if (stampI >= 0 && stampI <= stampSprites.Length - 1 && stampSprites.Length > 0 && stampSprites[stampI].sprite)
			{
				stampTex = stampSprites[stampI].sprite.texture; // esta variable es global porque bullet necesita acceder a la ultima stamp text usada
				UpdateSpriteRenderer();
			}
		}
	}
	public void RotateCurrentSprite90(){
		stampSprites[stampI].Rotate90();
		SetStampSprite(stampI);
		stampSprites[stampI].Initialize();
	}
	public void MirrorCurrentSpriteLeftToRight(){
		stampSprites[stampI].MirrorLeftToRight();
		SetStampSprite(stampI);
		stampSprites[stampI].Initialize();
	}
	public void MirrorCurrentSpriteUpToDown(){
		stampSprites[stampI].MirrorUpToDown();
		SetStampSprite(stampI);
		stampSprites[stampI].Initialize();
	}
	public void UpdateSpriteRenderer()
	{
		if (spriteRenderer)
		{
//			Sprite newSpr = Sprite.Create(stampTex, new Rect(0,0,stampTex.width, stampTex.height), new Vector2((float)stampTex.width/2f, (float)stampTex.height/2f), 1f);
			spriteRenderer.sprite = stampSprites[stampI].sprite;
			if (!poly)
				poly = GetComponent<PolygonCollider2D>();
			if (poly)
			{
				if (stampI == 0)
				{
					poly.pathCount = polyPathsCount;
					for (int i = 0; i < polyPathsCount; i++)
					{
						poly.SetPath(i,polyPaths[i]);
					}
					poly.offset = polyOffset;
				}
				else
				{
					poly.CreatePrimitive(4,Constants.one2);
					poly.offset = Vector2.zero;
				}
			}
		}
	}
	private int polyPathsCount;
	private List<Vector2[]> polyPaths;
	private Vector2 polyOffset;
	
	public void GetPolyArea(){ // ijos
		Texture2D alphaTex = terrains2D[0].d2dSprite.AlphaTex;
		if (terrains2D.Length > 1) // hacemos cambios en d2 terrenos
		{
			print(" NO PUEDO LEER DOS TERRENOS, LEYENDO INDESTRUCTIBLE UNICAMENTE ");
			foreach (Terrain2D t in terrains2D)
			{
				if (t == WorldMap.indestructible)
				{
					alphaTex = t.d2dSprite.AlphaTex;
					break;
				}
			}
		}
		if (stampSprites.Length == 0)
		{
			stampSprites = new StampSprite[1];
			stampSprites[0] = new StampSprite();
		}
		Point min = new Point();
		Point size = new Point();

		if (!poly)
			poly = gameObject.AddComponent<PolygonCollider2D>();

		if (fixPoly && fixPoly.enabled)
		{
			fixPoly.DecomposeAndSetPaths();
			poly.pathCount = fixPoly.paths.Count;
			print("HAY " + fixPoly.paths.Count + " PATHS EN FIX POLY");
			for (int i = 0; i < fixPoly.paths.Count; i++)
			{
				print("IMPORTANDO PATH " + i + " DESDE FIX POLY A POLY");
				Vector2[] path = fixPoly.GetPath(i);
				poly.SetPath(i, path);
			}
			fixPoly.SetBounds();
			poly.offset = Constants.zero2;
//			delta =  fixPoly.bounds.center - (Vector2)transform.position;
//			poly.offset = delta;
		}
		min.x = Mathf.RoundToInt(fixPoly.bounds.min.x);
		min.y = Mathf.RoundToInt(fixPoly.bounds.min.y);
		size.x = Mathf.RoundToInt(fixPoly.bounds.size.x);
		size.y = Mathf.RoundToInt(fixPoly.bounds.size.y);

		var at = terrains2D[0].d2dSprite.AlphaTex;
		if (min.x < 0)
			min.x = 0;
		if (min.y < 0)
			min.y = 0;
		int sum = min.x + size.x;
		if (sum > at.width)
			size.x = at.width - min.x;
		sum = min.y + size.y;
		if (sum > at.height)
			size.y = at.height - min.y;
		
		if (size.x > 0 && size.y > 0)
		{
			transform.position = fixPoly.bounds.center;
			TransformHelper.SetPosZ(transform, -5);
			poly.offset = -fixPoly.bounds.center;

			print("SIZE=" + fixPoly.bounds.size + " MIN=" + fixPoly.bounds.min);
			
			//pillo todos los pixeles del rectangulo CREO
            Color[] pixRegion = alphaTex.GetPixels(min.x, min.y, size.x, size.y);
			Texture2D clipBoardTex = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
			clipBoardTex.filterMode = FilterMode.Point;
			clipBoardTex.SetPixels(pixRegion);
			clipBoardTex.Apply();
			//creo un nuevo sprite al que le asigno la textura clipboard
			stampSprites[0].sprite = Sprite.Create(clipBoardTex, new Rect(0, 0, size.x, size.y), new Vector2(0.5f, 0.5f), 1f);
			stampSprites[0].sprite.name = "ClipBoard";
			stampSprites[0].sprite.texture.name = "ClipBoard";
			// borro los pixeles que no estan conciden con el poligono? CREO
			int _max = int.MaxValue;
			int _min = int.MinValue;
			for (int y = 0; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					if (!poly.OverlapPoint(new Vector2((float) (min.x + x), (float) (min.y + y))))
					{
						stampSprites[0].sprite.texture.SetPixel(x, y, Constants.transparentBlack);
					}
					else
					{
					}
				}
			}
			stampSprites[0].sprite.texture.Apply();
			stampSprites[0].Initialize(ref _max, ref _min); // aqui los max y min no me sireven para nada
	
			spriteRenderer.sprite = stampSprites[0].sprite;
//			delta = (fixPoly.bounds.center - (Vector2)transform.position);
//			transform.position += (Vector3)delta;
//			poly.offset -= delta;
			if (fixPoly) //no borra la fixture
			{
				//no borra la fixture
				//			for (int i = 0; i < fixPoly.pointsList.Count; i++)
				//				fixPoly.pointsList[i] -= (Vector3)delta;
				// borra los puntos de la fix
				//			fixPoly.DefinePoints(new Vector3[0]); 
				// borra el componente para que pueda moverme bien con el raton y pintar
				//			DestroyImmediate(fixPoly);
			}
			stampI = 0;
			stampTex = stampSprites[0].sprite.texture;
		}
		else
		{
			print("ERROR: SIZE=" + size.x + "," + size.y);
		}
		polyPathsCount = poly.pathCount;
		polyPaths = new List<Vector2[]>(polyPathsCount);
		for (int i = 0; i < polyPathsCount; i++)
		{
			polyPaths.Add(poly.GetPath(i));
		}
		polyOffset = poly.offset;
	}


	// estas dos variables son estaticas por que son de trabajo y solo puede ocurrir una explosion a la vez asi que no tiene sentido reservar memoria para cada script
	// aunque ocurrieran varias explosiones a la vez no importaria por que todo ocurre en el mainthread asi que la segunda explosion usaria las listas cuando la primera ha acabado con ellas
	static List<Point> mapRegion = new List<Point>(); // lista de coordenadas que definiran la region 
	static Queue<Point> queue = new Queue<Point>(); // lista queue 
	DecoCell decoCell = new DecoCell();

    public void MakeSureWeHaveD2dSpritesWeNeed(){
        for (int spr = 0; spr < terrains2D.Length; spr++)
        {
            if (terrains2D[spr] == null)
            {
                LookForDestructibles();
                break;
            }
        }
    }
    [HideInInspector] public int explodedInFrame; 
    public void Explode() {
        Explode(new Vector2(transform.position.x, transform.position.y), ArmyType.WildAnimals, false);
    }
    public void Explode(Vector2 pos, ArmyType armyType, bool isPlayer){
	    if (enabled)
	    {
		    Profiler.BeginSample(" EXPLOSION STAMP PEXTRAS EXPLODE");

		    //ExpensiveTaskManager.ReportThisNewTask((ExpTaskType.DestroyPixels));

		    if (debugBreak)
		    {
			    Debug.Break();
			    Debug.Log(this + "!!!!!!!!!!!!!!!!!!!!!!!!!");
		    }

		    if (!expensiveTaskManager && Application.isPlaying)
		    {
			    Debug.Log(this + "NO PUEDO EXPLOTAR, HAY GLOBAL EXPLOSION MANAGER?");
			    return;
		    }


		    if (true) // solo explota si no hay nada pendiente en Global Explosion Manager? para que dos explosiones no puedan solaparse, se perderan algunas muy seguidas pero asi no se peta
		    {
			    System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
			    s.Start();

			    int realSolid = 200;
			    int worldMapWidth = WorldMap.width; // optimizacion
			    int worldMapHeight = WorldMap.height; // optimizacion
			    for (int terrIndex = 0; terrIndex < terrains2D.Length; terrIndex++)
			    {
				   // if (terrIndex > 0 && makesChangesOn == MakesChangesOn.BothButIndestructibleAlsoHalfTrnasparentStampPixels)
					//    realSolid = 0;
				    terrain2D = terrains2D[terrIndex];
				    alphaTex = terrain2D.d2dSprite.AlphaTex;
				    alphaData = terrain2D.d2dSprite.AlphaData;
				    //asigno variables locales asi es mas facil de leer el codigo
				    switch (stampTextSelectMode)
				    {
					    case (StampTextSelectMode.Random):
						    RandomStampText(armyType);
						    break;
					    case (StampTextSelectMode.Cycle):
						    CycleStampTex();
						    break;
					    case (StampTextSelectMode.Fixed_Editor_SetExternally):
						    stampTex = stampSprites[stampI].sprite.texture;
						    break;
					    case (StampTextSelectMode.ScaleToSolidPixelRatio):
						    SetTextByScale();
						    break;
				    }

				    if (stampTex == null)
					    stampTex = stampSprites[stampI].sprite.texture;

				    byte[] stampTexAlphaData = stampSprites[stampI].alpha;
				    Point[] fullSolidPixels = stampSprites[stampI].fullSolidPixel;
				    Point[] halfSolidPixels = stampSprites[stampI].halfSolidPixel;
				    int solidPixelCount = stampSprites[stampI].totalSolidPixelCount;

				    pixelsDestroyed = 0;
				    //	if (pickUpPixelColors) CleanGroundPixels(); // esto deberia ahorrarse, no hace falta vaciarla , sino dar un puntero de cuantos pixeles han sido leidos

				    Point mapStartPos;
				    mapStartPos.x = Mathf.RoundToInt(pos.x - (stampTex.width / 2));
				    mapStartPos.y = Mathf.RoundToInt(pos.y - (stampTex.height / 2));
				    Point mapPos = new Point(0, 0);
				    Point readPos = new Point(0, 0); // indice pra saber cuando ...a aprtir de aqui se borro pero creo que es para saber cuando incremenar en 1 la coodernada y
				    int dataIndex = 0;
				    int halfSolidIndex = 0; //indice para los pixeles medio solidos

				    int i;

				    if (debug)
				    {
					    print("POS=" + pos);
					    print("writeStartPos.x=" + mapStartPos.x);
					    print("writeStartPos.y=" + mapStartPos.y);
					    print("writePos.x=" + mapPos.x);
					    print("writePos.y=" + mapPos.y);
				    }

				    readPos.x = 0;
				    bool tryDestroyDecorative = decoManager != null & decoModifierMode == DecoModifierMode.Destroy;
				    // MODO INGAME , MAS RAPIDO , NO EXPLOTA EN LOS BORDES ##############################################################################################################

				    #region MODO INGAME , MAS RAPIDO , NO EXPLOTA EN LOS BORDES ##################################


				    bool quickStampDone = false;
				    if (!useModifierKeysInEditor)
				    {
					    if (0 <= mapStartPos.x && mapStartPos.x <= worldMapWidth - stampTex.width && // compruebo si la extraccion de pixeles se va a salir  de la textura, si se sale da error
					        0 <= mapStartPos.y && mapStartPos.y <= worldMapHeight - stampTex.height)
					    {
						    //                            groundAlphaDataRegion = alphaTex.GetPixels(mapStartPos.x, mapStartPos.y, stampTex.width, stampTex.height);
						    groundAlphaDataRegion = terrain2D.GetAlphaDataRegion(mapStartPos.x, mapStartPos.y, stampTex.width, stampTex.height);
						    for (i = 0; i < fullSolidPixels.Length; i++)
						    {
							    Point fullSolidPixel = fullSolidPixels[i];
							    // -----------P-I-X-E-L S-O-L-I-D-O E-N-C-O-N-T-R-A-D-O E-N S-T-A-M-P-------------- 
							    mapPos.x = mapStartPos.x + fullSolidPixel.x;
							    mapPos.y = mapStartPos.y + fullSolidPixel.y;
							    dataIndex = (mapPos.y * alphaTex.width) + mapPos.x;
							    //if (tryDestroyDecorative)
								    //decoManager.DestroyFast(mapPos);
							    
							    // -----------P-I-X-E-L S-O-L-I-D-O E-N-C-O-N-T-R-A-D-O E-N S-U-E-L-O--------------
							    if (alphaData[dataIndex] > 127)
							    {
								    // y en el suelo tambien es solido
								    //									if (debug2)
								    //										print(this + "PIXEL SOLIDO ENCONTRADO EN SUELO");

								    if (groundModifierMode == GroundModifierMode.Destroy)
								    {
									    int ii = fullSolidPixel.x + fullSolidPixel.y * stampTex.width;
									    groundAlphaDataRegion[ii] = 0; // haz transprente el pixels del suelo para luego volcar todos facilmente con SetPixels
									    pixelsDestroyed++;
									    //DestroyPixel_new(fullSolidPixel.x, fullSolidPixel.y, dataIndex);
								    }
							    }
							    // -----------P-I-X-E-L T-R-A-N-S-P-A-R-E-N-T-E E-N-C-O-N-T-R-A-D-O E-N S-U-E-L-O-------------- 
							    else
							    {
								    // en el suelo el pixel que hay es transparente
								    if (groundModifierMode == GroundModifierMode.Create)
								    {
									    int ii = fullSolidPixel.x + fullSolidPixel.y * stampTex.width;
									    groundAlphaDataRegion[ii] = 255; // haz transprente el pixels del suelo para luego volcar todos facilmente con SetPixels
									    //CreatePixel_new(fullSolidPixel.x, fullSolidPixel.y, dataIndex);
								    }
							    }
						    }

						    GroundmodifierModeOptions options;
						    switch (groundModifierMode)
						    {
							    case (GroundModifierMode.Create):
								    options = createGroundOptions;
								    break;
							    case (GroundModifierMode.Destroy):
								    options = destroyGroundOptions;
								    break;
							    default:
								    options = new GroundmodifierModeOptions();
								    break;
						    }


						    if (options.detailTears)
							    DetailTears(mapStartPos, stampSprites[stampI]); // ojo , esto fallará en los limites

						    if (options.polish)
							    WorldMap.PolishRegion(stampTex.width, stampTex.height, groundAlphaDataRegion);

						    if (options.fixCorners)
							    WorldMap.UglyCornerCheckStatic(stampTex.width, stampTex.height, groundAlphaDataRegion);

						    //DESTUyE PIXELS DE SUELO AISLADOS
						    if (options.searchIsolatedPixels)
						    {
							    if (pixelsDestroyed > 0)
							    {

								    Point mapEndPos;
								    mapEndPos.x = mapStartPos.x + stampTex.width;
								    mapEndPos.y = mapStartPos.y + stampTex.height;

								    int flagCount = 0;

								    for (int hi = 0; hi < halfSolidPixels.Length; hi++)
								    {
									    Point hsPixel = halfSolidPixels[hi];
									    mapPos.x = mapStartPos.x + hsPixel.x;
									    mapPos.y = mapStartPos.y + hsPixel.y;
									    if (WorldMap.terrainFlags[mapPos.x, mapPos.y] == 0) // pos no marcada? si esta marcada es por que era de otra region ( ya borrada )
									    {
										    byte tileAlpha = alphaData[
											    mapPos.x + mapPos.y *
											    worldMapWidth]; // (lo mismo que AlphaDataGetIndex(mapPos.x, mapPos.y)], pero mas rapido sin llamar a la funcion // pillo alpha del mapa
										    if (200 <= tileAlpha) // coicide con lo que buscamos? solido/hueco?
										    {
											    mapRegion.Clear(); //lista de coordenadas que definiran la region 
											    queue.Clear();
											    queue.Enqueue(mapPos); // mete la posicion inicial en la Queue
											    WorldMap.terrainFlags[mapPos.x, mapPos.y] = 1; // marca la pos del mapa como usada
											    flagCount++;

											    while (queue.Count > 0)
											    {
												    var tile = queue.Dequeue(); // Saca la coordenada de la queue y metela en tile
												    mapRegion.Add(tile); // añade la tile a la tabla que tiene la region

												    int x;
												    int y;

												    for (int p = 0; p < checkPoint.Length; p++)
												    {
													    x = tile.x + checkPoint[p].x;
													    y = tile.y + checkPoint[p].y;
													    if (IsInsideRect(x, y, mapStartPos.x, mapStartPos.y, mapEndPos.x, mapEndPos.y) && WorldMap.terrainFlags[x, y] == 0)
													    {
														    tileAlpha = alphaData[x + y * worldMapWidth]; // AlphaDataGetIndex(x, y)]; // lee del mapa el valor de transparencia
														    if (200 <= tileAlpha) // es lo que buscamos? solido/hueco?
														    {
															    WorldMap.terrainFlags[x, y] = 1; // marca casilla como ya comprobada ( para no repetir )
															    flagCount++;
															    queue.Enqueue(new Point(x, y)); // metela en la queue ( para sacarla justo despues al comienzo del loop
														    }
													    }
												    }
											    }

											    if (mapRegion.Count < smallRegionThreshold) // BORRA SI ES PEQUEÑO
											    {
												    for (int r = 0; r < mapRegion.Count; r++)
												    {
													    Point mapReg = mapRegion[r]; // small optimization
													    //if (tryDestroyDecorative)
														    //decoManager.DestroyFast(new Point(mapReg.x, mapReg.y + 1));
													    //DecoDestroy(new Point(mapReg.x, mapReg.y + 1));
													    int ii = (mapReg.x - mapStartPos.x) + (mapReg.y - mapStartPos.y) * stampTex.width;
													    groundAlphaDataRegion[ii] = 0; // haz transprente el pixels del suelo para luego volcar todos facilmente con SetPixels
													    pixelsDestroyed++;
													    //DestroyPixel_new(mapReg.x - mapStartPos.x, mapReg.y - mapStartPos.y, mapReg.x + mapReg.y * worldMapWidth ); // AlphaDataGetIndex(mapReg.x, mapReg.y));
//													d2dSprite.AlphaData[AlphaDataGetIndex(mapRegion[r].x,mapRegion[r].y)] = 0; // HAY QUE LLAMAR A DESTROY PIXEL !!!
												    }
											    }
										    }
									    }
								    }

								    WorldMap.CleanTerrainFlags(mapStartPos.x, mapStartPos.y, mapEndPos.x, mapEndPos.y);
							    }
						    }
						    // --------------------------------------------------------------------------

						    if (groundModifierMode == GroundModifierMode.Create || groundModifierMode == GroundModifierMode.Destroy && pixelsDestroyed > 0)
						    {
							    var writeEndPosX = mapStartPos.x + stampTex.width;
							    var writeEndPosY = mapStartPos.y + stampTex.height;
//								alphaTex.SetPixels(mapStartPos.x, mapStartPos.y, stampTex.width, stampTex.height, groundAlphaDataRegion);
							    terrain2D.SetAlphaDataAndRawDataRegion(mapStartPos.x, mapStartPos.y, stampTex.width, stampTex.height, groundAlphaDataRegion);
							    if (debug)
								    print(this + " LLAMANDO A NOTIFY CHANGES 1!!!!!!!");

							    terrain2D.d2dSprite.NotifyChanges(mapStartPos.x, writeEndPosX, mapStartPos.y, writeEndPosY);
							    if (Application.isPlaying)
							    {

							    }
							    else
							    {
								    ApplyChangesOnEditor();
							    }
						    }

						    quickStampDone = true;
					    }
				    }

				    #endregion
				    // MODO EDITOR MAS LENTO, SI EXPLOTA EN LOS BORDES ############################################################################################################################
				    if (useModifierKeysInEditor || !quickStampDone)
				    {
					    if (worldMapWidth <= 0 || worldMapHeight <= 0)
					    {
						    Debug.LogError("WORLD MAP SIZE INCORRECTO (" + worldMapWidth + "," + worldMapHeight + ")");
						    return;
					    }

					    int stampWidth = stampTex.width; // no usaremos stampTex.width por que se puede salir del mapa 
					    int stampHeight = stampTex.height; //  no usaremos stampTex.height por que se puede salir del mapa
//						Point originalWriteStartPos = mapStartPos; // no la uso nunca
					    int readStartIndex = 0;
					    Point readStartPos = new Point(0, 0);
					    int groundAlphaRegionIndex = 0; // indice que recorre groundAlphaPixels 
					    Point endReadPos = new Point(stampTex.width, stampTex.height);

					    if (mapStartPos.x < 0) // la stamp tiene parte fuera del mapa por la izquierda
					    {
						    readStartIndex = -mapStartPos.x;
						    readStartPos.x = -mapStartPos.x;
						    readPos.x = -mapStartPos.x;
						    stampWidth += mapStartPos.x; // es resta por que el valor que sumo es negativo
						    mapStartPos.x = 0;
					    }
					    else if (mapStartPos.x + stampTex.width > worldMapWidth) // la stamp tiene parte fuera del mapa por la derecha
					    {
						    stampWidth = worldMapWidth - mapStartPos.x;
						    endReadPos.x = stampWidth;
					    }

					    if (mapStartPos.y < 0) // la stamp tiene parte fuera del mapa por abajo ?
					    {
						    readStartIndex -= mapStartPos.y * stampTex.width;
						    readStartPos.y = -mapStartPos.y;
						    readPos.y = -mapStartPos.y; // suma en realidad
						    stampHeight += mapStartPos.y; // resta en realidad
						    mapStartPos.y = 0;
					    }
					    else if (mapStartPos.y + stampTex.height > worldMapHeight) // la stamp tiene parte fuera del mapa por arriba ?
					    {
						    stampHeight = worldMapHeight - mapStartPos.y;
						    endReadPos.y = stampHeight;
					    }

					    mapPos = mapStartPos;
					    dataIndex = (mapPos.y * alphaTex.width) + mapPos.x;

					    if (stampWidth > 0 && stampHeight > 0)
					    {
						    //groundAlphaDataRegion = alphaTex.GetPixels(mapStartPos.x, mapStartPos.y, stampWidth, stampHeight);

						    groundAlphaDataRegion = terrain2D.GetAlphaDataRegion(mapStartPos.x, mapStartPos.y, stampWidth, stampHeight);

						    for (i = readStartIndex; i < stampTexAlphaData.Length; i++)
						    {
							    // -----------P-I-X-E-L S-O-L-I-D-O E-N-C-O-N-T-R-A-D-O E-N S-T-A-M-P-------------- 
							    if (stampTexAlphaData[i] > 127 || forceWrite)
							    {
								    // si teneos un pixel solido en la stamp tex
								    if (debug2)
									    print(this + "PIXEL SOLIDO ENCONTRADO EN STAMP");
								    
								    //if (tryDestroyDecorative)
									    //decoManager.DestroyFast(mapPos);

										if (debug2)
									    print(this + "PIXEL ALPHA EN SUELO = " + alphaData[dataIndex] + " DATAINDEX =" + dataIndex + " X=" + mapPos.x + " Y=" + mapPos.y);
								    // -----------P-I-X-E-L S-O-L-I-D-O E-N-C-O-N-T-R-A-D-O E-N S-U-E-L-O-------------- 
								    if (alphaData[dataIndex] > 127)
								    {
									    // y en el suelo tambien es solido
									    if (stampTexAlphaData[i] > realSolid)
									    {
										    // pixel stamp es solido de verdad, no medio solido ( los medio solidos son para destruir solamente pixels de suelo aislados )
										    if (debug)
											    print(this + "PIXEL SOLIDO ENCONTRADO EN SUELO");
										    if (groundModifierMode == GroundModifierMode.Destroy)
										    {
											    //DestroyPixel(groundAlphaRegionIndex, dataIndex);
											    groundAlphaDataRegion[groundAlphaRegionIndex] = 0;
											    alphaData[dataIndex] = 0;
											    pixelsDestroyed++;
										    }
									    }
									    else
									    {
										    // pixel stamp era medio solido
										    _halfSolidData[halfSolidIndex] = dataIndex;
										    _halfSolidPixel[halfSolidIndex] = groundAlphaRegionIndex;
										    halfSolidIndex++;
									    }
								    }
								    // -----------P-I-X-E-L T-R-A-N-S-P-A-R-E-N-T-E E-N-C-O-N-T-R-A-D-O E-N S-U-E-L-O-------------- 
								    else
								    {
									    // en el suelo el pixel que hay es transparente
									    if (groundModifierMode == GroundModifierMode.Create)
									    {
										    groundAlphaDataRegion[groundAlphaRegionIndex] = 255; // haz SOLIDO el pixel del suelo para luego volcar todos facilmente con SetPixels
										    alphaData[dataIndex] = 255; // haz solido el pixel de alpha data
									    }
								    }
							    }

							    // incrementa la posicion de lectura
							    groundAlphaRegionIndex++; // de la textura que tiene el trozo de suelo
							    readPos.x++; // de la textura Stamp
							    if (readPos.x == endReadPos.x)
							    {
								    readPos.y++;
								    if (readPos.y == endReadPos.y)
									    break;
								    else
								    {
									    readPos.x = readStartPos.x;
									    mapPos.x = mapStartPos.x;
									    mapPos.y++;
									    dataIndex = (mapPos.y * alphaTex.width) + mapPos.x;
									    i += stampTex.width - stampWidth;
								    }
							    }
							    else
							    {
								    mapPos.x++;
								    dataIndex++;
							    }

						    }

						    GroundmodifierModeOptions options;
						    switch (groundModifierMode)
						    {
							    case (GroundModifierMode.Create):
								    options = createGroundOptions;
								    break;
							    case (GroundModifierMode.Destroy):
								    options = destroyGroundOptions;
								    break;
							    default:
								    options = new GroundmodifierModeOptions();
								    break;
						    }

						    if (Application.isEditor && stampSprites[stampI].sprite.name != "ClipBoard") // si estamos copiando y pegando trozos no necesitamos hacer estas mierdas
						    {
							    if (options.detailTears)
								    DetailTears(mapStartPos, stampSprites[stampI]); // ojo , esto fallará en los limites

							    if (options.polish)
								    WorldMap.PolishRegion(stampWidth, stampHeight, groundAlphaDataRegion);

							    if (options.fixCorners)
								    WorldMap.UglyCornerCheckStatic(stampWidth, stampHeight, groundAlphaDataRegion);
						    }

						    //COMRPUEBA PIXELS MEDIO SOLIDOS ALMACENADOS Y DESTUyE PIXELS DE SUELO AISLADOS ( ESTA FUNCION ES MEJORADA CON RESPECTO A LA DE ARRIBA, DEBERIA CAMBIARLA)
						    if (options.searchIsolatedPixels)
						    {
							    if (halfSolidIndex > 0)
							    {
								    var totalHalfSolid = halfSolidIndex;
								    int length = alphaData.Length;
								    int width = alphaTex.width;
								    int newIndex;

								    for (int n = 0; n < totalHalfSolid; n++)
								    {
									    byte up = 0;
									    byte down = 0;
									    byte left = 0;
									    byte right = 0;
									    int centerIndex = _halfSolidData[n];

									    newIndex = centerIndex + width;
									    if (newIndex < length)
										    up = alphaData[newIndex];

									    newIndex = centerIndex - width;
									    if (newIndex >= 0)
										    down = alphaData[newIndex];

									    newIndex = centerIndex - 1;
									    if (newIndex >= 0)
										    left = alphaData[newIndex];

									    newIndex = centerIndex + 1;
									    if (newIndex < length)
										    right = alphaData[newIndex];

									    int sum = up + down + left + right;
									    if (debug2)
										    print(" PIXEL SEMI SOLIDO=" + n + " Up=" + up + " down=" + down + " left=" + left + " right=" + right + " SUM = " + sum);
									    if (sum < 510)
									    {
										    //DestroyPixel(_halfSolidPixel[n], _halfSolidData[n]);
										    groundAlphaDataRegion[_halfSolidPixel[n]] = 0;
										    pixelsDestroyed++;
									    }
								    }
							    }
						    }

						    // --------------------------------------------------------------------------
						    if (groundModifierMode == GroundModifierMode.Create || groundModifierMode == GroundModifierMode.Destroy && pixelsDestroyed > 0)
						    {
							    var writeEndPosX = mapStartPos.x + stampWidth;
							    var writeEndPosY = mapStartPos.y + stampHeight;
//								alphaTex.SetPixels(mapStartPos.x, mapStartPos.y, stampWidth, stampHeight, groundAlphaDataRegion);
							    terrain2D.SetAlphaDataAndRawDataRegion(mapStartPos.x, mapStartPos.y, stampWidth, stampHeight, groundAlphaDataRegion);
							    if (debug)
								    print(this + " LLAMANDO A NOTIFY CHANGES 1!!!!!!!");
							    terrain2D.d2dSprite.NotifyChanges(mapStartPos.x, writeEndPosX, mapStartPos.y, writeEndPosY);
							    if (Application.isPlaying)
							    {

							    }
							    else
							    {
								    ApplyChangesOnEditor();
							    }
						    }
					    }
				    }

				    if (debug)
					    print(this + " STAMP TERMINADO EN FRAME " + (Time.frameCount - 1));
				    pixelsDestroyedFactor =
					    ((pixelsDestroyed + 0.0f) / solidPixelCount) * pixelsDestroyedFactorMult; // 0.0 PARA CONVERTIRLO A FLOAT, SI NO LA OPERACION DARA UN INT TRUNCANDO LOS DECIMALES !!!
				    if (debug)
					    print(this + " PIX DESTROYED =  " + pixelsDestroyed + " HOW MANY IN STAMP TEX " + solidPixelCount + " PIX DEST FACT " + pixelsDestroyedFactor);
			    }

			    s.Stop();
			    if (debug)
				    print(this + " EXPLOSION MILISEGUNDOS=" + s.ElapsedMilliseconds);
		    }
	    }

	    if (pixelsDestroyed > 0)
		    LevelCounters.groundDestructionCount++;
	    explodedInFrame = KuchoTime.frameCount;
        Profiler.EndSample();
		return;
	}
	/*void DecoDestroy(Point mapPos){
		{ // revienta las plantas aunque no haya pixel solido en el suelo
            decoManager.Read(mapPos, ref decoCell);
//            decoCell.Read(mapPos, decoManager); // asi era antes
			if (decoCell.store != 255) // hay algo
			{
				ItemStore store = Game.G.allStoresList.stores[decoCell.store];
				Item decoitem = store.item[decoCell.item];
				if (decoitem.gameObject.activeSelf == true) // el itemNumber tiene que estar almacenado en la decoMap para poder saber cual es y borrarlo!!!!!
				{
					decoitem.decorative.Destroy(true);
				}
			}
		}
	}*/
	public void ApplyChangesOnEditor(){
		alphaTex.Apply(false, false);
        terrain2D.d2dSprite.textureApplyPending = false;
        // no uso el d2dSprite.MarkAsDirty por que eso provocaria que reconstruya toda la textura , muy lento!
        terrain2D.d2dSprite.compressedAlphaDataIsUpdated = false;
        #if UNITY_EDITOR
        TS_Helper.SetDirty(terrain2D); //creo que esto lo que hace es decirle a unity que tiene que salvar
        #endif
    }
	public void NotifyChangesOnEditor(){
		if (!terrain2D)
		{
			LookForDestructibles();
		}
		if (terrain2D)
			terrain2D.d2dSprite.NotifyChangesAndRebuildColliders();
		else
		{
			if (enabled)
				Debug.LogError(this + " NO TENGO TERRENO EN EL QUE APLICAR CAMBIOS?!");
			else
				Debug.LogError(this + " MI STAMP EXTRAS SCRIPT ESTA APAGADO?!");
		}
	}
    int counter = 0;
    void DetailTears(Point _mapPos, StampSprite stamp)
    {
        int mapHeigh = terrain2D.texture.height; 
        int mapWidth = terrain2D.texture.width;
        int howManyPix = terrain2D.inGameHorizontalLumaDeep;
        int startX ;
        int startY;
        int endX;
        int endY;
        int stampW = stamp.sprite.texture.width;
        int stampH = stamp.sprite.texture.height;
        int w1 = mapWidth;// - howManyPix, TODO comprobar si se sale del mapa y si lo hace ajustar, por ahora no hace falta por que solo llamo si el stamp no se sale del mapa;

        int leftDeletionsCount = 0;
        int rightDeletionsCount = 0;

        int startMapPosX = _mapPos.x;
        int startMapPosY = _mapPos.y;

        int mapPosX;
        int mapPosY;

        if (howManyPix > 0)
        {
            startY = 1;
            endY = stampH - 1;
            startX = 1;
            endX = stampW - 1;

            for (int y = startY; y < endY; y++)
            {
                // borro pixeles de derecha a izquierda
                mapPosY = startMapPosY + y;
                for (int x = startX; x < endX; x++)
                {
                    int regionIndex = x + y * stampW;
                    if (stamp.alpha[regionIndex] > 127) // es solido en mi stamp?
                    {
                        if (groundAlphaDataRegion[regionIndex] > 127) // el pixel en mi region sampleada (stamp + lo que habia) es solido
                        {//ahora hay que leer el luma del pixel de la textura, no de la alphadata
                            mapPosX = startMapPosX + x;
                            int mapIndex = mapPosX + mapPosY * mapWidth;
                            if (alphaData[mapIndex + 1] == 0) // es un borde derecho?
                            { // hay que escarbar
                                for (int i = 0; i < howManyPix; i++) // solo 3 pixels hacia adentro en cada pase
                                {
                                    if (stamp.alpha[regionIndex - i] < 127) // si salgo de los pixeles stampsolidos deja de escarbar, esto no funcionará bien con stamps textures convexas
                                        break;
                                    if (alphaData[mapIndex - i] > 127) // es solido el de al lado? TODO esto podria ahorrarmelo , borrar solo si es solido? y si no lo es ? lo puedo borrar igualemnte
                                    {
                                        Color p = terrain2D.GetPixel(mapPosX - i, mapPosY);
                                        float luma = ColorHelper.Luma(p);
                                        if (luma < terrain2D.horizontalTearLumaThreshold) // hay que borrarlo?
                                        {
                                            groundAlphaDataRegion[regionIndex - i] = 0; // borra el pixel en la temp Data
                                            alphaData[mapIndex - i] = 0;
                                            leftDeletionsCount++;
                                        }
                                        else // hemos encontrado uno brillante que NO hay que borrar
                                        {
                                            break; // no se sigue intentando escarbar
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //borro de izquirda a derecha
                for (int x = endX - 1; x >= startX; x--)
                {
                    int regionIndex = x + y * stampW;
                    if (stamp.alpha[regionIndex] > 127) // es solido en mi stamp?
                    {
                        if (groundAlphaDataRegion[regionIndex] > 127) // el pixel en mi region sampleada (stamp + lo que habia) es solido
                        {//ahora hay que leer el luma del pixel de la textura, no de la alphadata
                            mapPosX = startMapPosX + x;
                            int mapIndex = mapPosX + mapPosY * mapWidth;
                            if (alphaData[mapIndex - 1] == 0) // es un borde izquierdo?
                            { // hay que escarbar
                                for (int i = 0; i < howManyPix; i++) // solo 3 pixels hacia adentro en cada pase
                                {
                                    if (stamp.alpha[regionIndex + i] < 127) // si salgo de los pixeles stampsolidos deja de escarbar, esto no funcionará bien con stamps textures convexas
                                        break;
                                    if (alphaData[mapIndex + i] > 127) // es solido el de al lado? TODO esto podria ahorrarmelo , borrar solo si es solido? y si no lo es ? lo puedo borrar igualemnte
                                    {
                                        Color p = terrain2D.GetPixel(mapPosX + i, mapPosY);
                                        float luma = ColorHelper.Luma(p);
                                        if (luma < terrain2D.horizontalTearLumaThreshold) // hay que borrarlo?
                                        {
                                            groundAlphaDataRegion[regionIndex + i] = 0; // borra el pixel en la temp Data
                                            alphaData[mapIndex + i] = 0;
                                            leftDeletionsCount++;
                                        }
                                        else // hemos encontrado uno brillante que NO hay que borrar
                                        {
                                            break; // no se sigue intentando escarbar
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //print("HORIZONTAL FRACTURE DETAIL TERMINADO LEFT-COUNT=" + leftDeletionsCount + " RIGHT-COUNT=" + rightDeletionsCount);

        // ------------------------ VERTICAL 


        howManyPix = terrain2D.inGameVerticalLumaDeep;

        if (howManyPix > 0)
        {
            leftDeletionsCount = 0;
            rightDeletionsCount = 0;

            startY = 1;
            endY = stampH - 1;
            startX = 1;
            endX = stampW - 1;

            for (int x = startX; x < endX; x++)
            {
                // borro pixeles de arriba a abajo
                mapPosX = startMapPosX + x;
                for (int y = startY; y < endY; y++)
                {
                    int regionIndex = x + y * stampW;
                    if (stamp.alpha[regionIndex] > 127) // es solido en mi stamp?
                    {
                        if (groundAlphaDataRegion[regionIndex] > 127) // el pixel en mi region sampleada (stamp + lo que habia) es solido
                        {//ahora hay que leer el luma del pixel de la textura, no de la alphadata
                            mapPosY = startMapPosY + y;
                            int mapIndex = mapPosX + mapPosY * mapWidth;
                            if (alphaData[mapIndex + mapWidth] <= 127) // es un borde superior?
                            { // hay que escarbar
                                for (int i = 0; i < howManyPix; i++) // solo 3 pixels hacia adentro en cada pase
                                {
                                    int regionIndexInc = i * stampW;
                                    if (stamp.alpha[regionIndex - regionIndexInc] < 127) // si salgo de los pixeles stampsolidos deja de escarbar, esto no funcionará bien con stamps textures convexas
                                        break;
                                    int mapIndexInc = i * mapWidth;
                                    if (alphaData[mapIndex - mapIndexInc] > 127) // es solido el de al lado? TODO esto podria ahorrarmelo , borrar solo si es solido? y si no lo es ? lo puedo borrar igualemnte
                                    {
                                        Color p = terrain2D.GetPixel(mapPosX - mapIndexInc, mapPosY);
                                        float luma = ColorHelper.Luma(p);
                                        if (luma < terrain2D.verticalTearLumaThreshold) // hay que borrarlo?
                                        {
                                            groundAlphaDataRegion[regionIndex - regionIndexInc] = 0; // borra el pixel en la temp Data
                                            alphaData[mapIndex - mapIndexInc] = 0;
                                            leftDeletionsCount++;
                                        }
                                        else // hemos encontrado uno brillante que NO hay que borrar
                                        {
                                            break; // no se sigue intentando escarbar
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //borro de abajo a arriba
                mapPosX = startMapPosX + x;
                for (int y = endY - 1; y >= startY; y--)
                {
                    int regionIndex = x + y * stampW;
                    if (stamp.alpha[regionIndex] > 127) // es solido en mi stamp?
                    {
                        if (groundAlphaDataRegion[regionIndex] > 127) // el pixel en mi region sampleada (stamp + lo que habia) es solido
                        {//ahora hay que leer el luma del pixel de la textura, no de la alphadata
                            mapPosY = startMapPosY + y;
                            int mapIndex = mapPosX + mapPosY * mapWidth;
                            if (alphaData[mapIndex - mapWidth] <= 127) // es un borde inferior?
                            { // hay que escarbar
                                for (int i = 0; i < howManyPix; i++) // solo 3 pixels hacia adentro en cada pase
                                {
                                    int regionIndexInc = i * stampW;
                                    if (stamp.alpha[regionIndex + regionIndexInc] < 127) // si salgo de los pixeles stampsolidos deja de escarbar, esto no funcionará bien con stamps textures convexas
                                        break;
                                    int mapIndexInc = i * mapWidth;
                                    if (alphaData[mapIndex + mapIndexInc] > 127) // es solido el de al lado? TODO esto podria ahorrarmelo , borrar solo si es solido? y si no lo es ? lo puedo borrar igualemnte
                                    {
                                        Color p = terrain2D.GetPixel(mapPosX + mapIndexInc, mapPosY);
                                        float luma = ColorHelper.Luma(p);
                                        if (luma < terrain2D.verticalTearLumaThreshold) // hay que borrarlo?
                                        {
                                            groundAlphaDataRegion[regionIndex + regionIndexInc] = 0; // borra el pixel en la temp Data
                                            alphaData[mapIndex + mapIndexInc] = 0;
                                            leftDeletionsCount++;
                                        }
                                        else // hemos encontrado uno brillante que NO hay que borrar
                                        {
                                            break; // no se sigue intentando escarbar
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //print("HORIZONTAL FRACTURE DETAIL TERMINADO DOWN-COUNT=" + leftDeletionsCount + " UP-COUNT=" + rightDeletionsCount);
            }
        }

    }

    public void DestroyPixel(int groundAlphaPixelsIndex, int alphaDataIndex){
        groundAlphaDataRegion[groundAlphaPixelsIndex] = 0; // haz transprente el pixels del suelo para luego volcar todos facilmente con SetPixels
        alphaData[alphaDataIndex] = 0; // haz transparente el pixel de alpha data
        pixelsDestroyed ++;
    }
    public void DestroyPixel_new(int stampTexX, int stampTexY, int alphaDataIndex){
        int pixelIndex = stampTexX + stampTexY * stampTex.width;
        groundAlphaDataRegion[pixelIndex] = 0; // haz transprente el pixels del suelo para luego volcar todos facilmente con SetPixels
        alphaData[alphaDataIndex] = 0; // haz transparente el pixel de alpha data
        pixelsDestroyed ++;
    }
    public void CreatePixel_new(int stampTexX, int stampTexY, int alphaDataIndex){
        int pixelIndex = stampTexX + stampTexY * stampTex.width;
        groundAlphaDataRegion[pixelIndex] = 255; //  haz SOLIDO el pixel del suelo para luego volcar todos facilmente con SetPixels
        alphaData[alphaDataIndex] = 255; // haz solido el pixel de alpha data
    }
    bool IsInsideRect(int x, int y, int sx, int sy, int ex, int ey){
        if (x <  sx) return false;
        if (y <  sy) return false;
        if (x >= ex) return false;
        if (y >= ey) return false;
        return true;
    }
    //  [MethodImplAttribute(MethodImplOptions.AggressiveInlining)] //net4.6
    int AlphaDataGetIndex(int x, int y){
        return x + y * WorldMap.width;
    }
    [HideInInspector] public bool inspectPixOnMove;
    public void InspectPixels(){
        StampSprite ss = stampSprites[stampI];
        stampTex = ss.sprite.texture;
        terrain2D = terrains2D[0];
        terrain2D.ResetADataRegion();
        Vector3 pos = _transform.position;
        Point mapStartPos;
        mapStartPos.x = Mathf.RoundToInt(pos.x - (stampTex.width / 2));
        mapStartPos.y = Mathf.RoundToInt(pos.y - (stampTex.height / 2));
        groundAlphaDataRegion = terrain2D.GetAlphaDataRegion( mapStartPos.x, mapStartPos.y, stampTex.width, stampTex.height);
    }
    public void MoveAndExplodeWithExpensiveTask(Vector2 t){
        Vector3 newPos = Constants.zero3;
        newPos.x = t.x;
        newPos.y = t.y;
        transform.position = newPos;
        Explode();
    }
}
