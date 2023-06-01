using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.Design;

using UnityEngine.Profiling;
using UnityEngine.Serialization;
 
// using TreeEditor; WTF???
using UnityEngine.SceneManagement;
using Object = System.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ObjectNode
{
    public Point pos; // coordenadas en el mundo donde debe ser creado este objeto
    public Vector3 rot; // x e y son localScale, z es localRotation.EulerAngles.z
    public int obj; // indice que corresponde con la tabla objects
    //Constructor
    public ObjectNode(Point _pos, Vector3 _rot, int _obj)
    {
        pos = _pos;
        rot = _rot;
        obj = _obj;
    }
}
[System.Serializable]
public class GrassSettings
{
    public AnimationCurve size;
    [Range(0, 3)] public float destructibleSize = 1f;
    [Range(0, 3)] public float inDestructibleSize = 1f;
    public Color color;
    [Range(0,1)] public float individualColor;
    [Range(0,1)] public float alphaCut;
    [Range(1,2)] public float alphaPush;
    private Material mat;
    private Material exploMat;

    public void Init()
    {
        mat = MaterialDataBase.instance.grassMat;
        exploMat = MaterialDataBase.instance.grassExploMat;
    }

    public void ResetDefaultShaderValues()
    {
        if (Game.G)
        {
            CopyFrom(MaterialDataBase.instance.grass);
        }
    }

    public void SetValuesOnMaterials()
    {
        if (mat)
        {
            mat.color = color;
            mat.SetFloat("_SpriteColorFactor", individualColor);
            mat.SetFloat("_AlphaCut", alphaCut);
            mat.SetFloat("_AlphaPush", alphaPush);
        }

        if (exploMat)
        {
            exploMat.color = color;
            exploMat.SetFloat("_SpriteColorFactor", individualColor);
            exploMat.SetFloat("_AlphaCut", alphaCut);
            exploMat.SetFloat("_AlphaPush", alphaPush);
        }
    }

    public void CopyFrom(GrassSettings o)
    {
        mat = o.mat;
        exploMat = o.exploMat;
        size = o.size;
        destructibleSize = o.destructibleSize;
        inDestructibleSize = o.inDestructibleSize;
        color = o.color;
        individualColor = o.individualColor;
        alphaCut = o.alphaCut;
        alphaPush = o.alphaPush;
    }
}

[ExecuteInEditMode]
public class WorldMap : MonoBehaviour
{
    public bool debug = false;
    float t;
    static public WorldMap instance = null;
    [HideInInspector] public bool useDesignedMap = true;
    [HideInInspector] public bool useLevelEditor = false;
    public static GroundType workingOnMap = GroundType.Destructible;
    public static TS_DynamicSprite[] destructibleSprites;
    public static Terrain2D selectedTerrain;
    public static Terrain2D destructible;
    public Terrain2D _destructible;
    public static Terrain2D indestructible;
    public Terrain2D _indestructible;
    public static Terrain2D background;
    public Terrain2D _background;
    public static Terrain2D[] terrains2D;
    public Terrain2D[] _terrains2D;
    public static SpritePlanes spritePlanes;
    public SpritePlanes _spritePlanes;

    [Header(" ----- SUN -")]
    public float sunStartPosition = 0f;
    public bool _staticSun = false;
    public bool forceSunColor;
    bool ForceSunColor() { return forceSunColor;}
    bool DontForceSunColor() { return !forceSunColor;}
    public Color sunColor = Color.white;
    [Range (0,1)] public float skyToSunColorRatio = 0.5f;
    [Range (0,2)] public float sunSaturation = 1f;
    [Range (-1,1)] public float sunIntensity = 0f;
    [Header(" ----- SKY LIGHTS -")]
    [Range (0,1)] public float sunToSkylightsColorRatio = 0.5f;
    [Range(0, 1)] public float skyLightsIntensity = 0.764f;
    [Range(0, 1)] public float skyLightsSaturation = 0.7f;
    [Range (0,1)] public float dayLightReduction = 0.468f;
    [Header(" ----- SKY -")]
    public bool skyTextureScroll = true;
    public Vector2 skyOffset;
    bool IsNotRandomSky() { return !randomSky; }
    public bool randomSky = true;
    public int skyIndex;
    [Range(0,1)] public float sunColorToSky = 0.5f;
    public Color skyColor = new Color(0.227451f, 0.3727797f, 0.6431373f);
    public Color nightColor;

    [Range(-180, 180)] public float cosmosHueShift = 1;
    [Range(0, 2)] public float cosmosSaturation = 1;
    [Range(0, 2)] public float cosmosBright = 1;
    [Range(0, 2)] public float cosmosContrast = 1;
    public bool useDayAndNightCSV = false;
    [Range(0, 2)] public float cosmosAtNightSaturation = 1;
    [Range(0, 2)] public float cosmosAtnightBright = 1;
    [Range(0, 2)] public float cosmosAtNightContrast = 1;

    [Header(" ----- BACKGROUND WALL -")]
    [Range(0, 2)] public float wallGain = 1;
    [Range(-1, 1)] public float wallShift = 0;
    public bool contrastInc;
    public bool contrastDec;

    [Header(" ----- PARALLAX - ")]
    [Range(0, 10)]public int parallaxIndex;
    [ReadOnly2Attribute] public string parallaxName;
    
    bool GotColorManager(){return parallaxColorManager != null; }
    bool ShowOldStuff(){return parallaxColorManager != null; }
    bool DynamicParallax() { return !fixedColorParallax; }
    bool FixedColorParallax() { return fixedColorParallax; }

    //public GameObject parallaxPrefab;
    //[ReadOnly2Attribute] public ParallaxFather parallax;
    public Vector2 parallaxOffset;
    public ParallaxColorManager parallaxColorManager;
    public bool fixedColorParallax = true;
    [Header("only for day and night cycle")]
    [Range(-180, 180)] public float parallaxHueShift;
    bool ShowHueShift() { return fixedColorParallax == false & parallaxColorManager == null; }
    [HideInInspector] public Color parallaxColorAdd = Constants.solidWhite;
    [HideInInspector] [Range(-0.005f, 0.005f)] public float zAddFactor;
    [HideInInspector] public Color parallaxColorMult = Constants.solidWhite;
    [HideInInspector] [Range(-0.005f, 0.005f)] public float zMulFactor;
    [HideInInspector] [Range (0,0.0002f) ] public float joinSeparateValue = 0.0002f; 
    [HideInInspector] public bool join;
    [HideInInspector] public bool separate;

    [Header(" ----- GRASS -")]
    public bool grassResetDefaultShaderValues;
    public bool grassClean;
    public GrassSettings grass;

    [Header(" ----- FOG -")]
    public bool setFog;
    bool Fog() { return setFog; }

    public bool fog;
    public FogMode forgMode = FogMode.Linear;
    public float fogStart = 0;
    public float fogEnd = 200;
    public float fogDensity = 1;
    public Color fogColor = Constants.solidBlack;

    [Header(" - PHYSICS -")]
    public float visibilityThresholdShift = 0;
    public float plantCreationSunPosition = 1.3f;

    public float temp = 20f;
    public float tempTransmissionFactor = 0.1f;
    public float gravity = 10f;
    [Range(0.5f,3)] public float indestructibleHardness = 1;
    public float atmosphereThickness = 10f;
    public bool useTopLimit = false;
    public float topWorldLimitExtraHeight = 0f; // si no uso top limit como techo aun esto sirve para definir cosas como donde empieza a fallar el jet pack
    public float extraSideMargin = 50f;
    public float extraBottomMargin = 0;
    public bool plantsGrowOnIndestructible = false;
    public bool simpleWalls = true;
    public int smallRegionThresholdAfterExplosion = 4;
    [Header(" - MISC -")]
    [ReadOnly2Attribute] public Transform topLimit;
    [HideInInspector] public  Transform bottomLimit;
    [HideInInspector] public  Transform leftLimit;
    [HideInInspector] public  Transform rightLimit;
    [HideInInspector] public  bool setLightObstacleColors = false;
    [HideInInspector] public  Color shadowMakerSpritesColor = new Color(0, 0, 0, 0.9f);
    [HideInInspector] public  Color addShadowColor = new Color(0, 0, 0, 0);
    [HideInInspector] public  Color multShadowColor = new Color(0, 0, 0, 0.1647f);
    [Header(" - OBJETS -")]
    byte[] objectsData;
    Texture2D srcObjects; // ahora solo la uso temporalmente para cargar de folder level editor, procesar y eliminar, asi no ocupara espacio en disco en el juego
    string objectsFilePath = "Objects.dat";
    [HideInInspector] public Color32 blackBorder; // la hago publica si necesito cambiar el color con el color picker , pero no interesa que mnoleste en inspector
    [HideInInspector] public Color32 greyBorder; // la hago publica si necesito cambiar el color con el color picker , pero no interesa que mnoleste en inspector
    List<ObjectNode> objectArray = new List<ObjectNode>(); // usada temporalmente ya que se puede redimensionar , luego creo la tabla de bytes objectsData que es la que pongo en el folder level editor para que el juego lea

    public static Vector2 worldCenter;
    [Header(" - BIG DIMENSIONS -")]
    [Range(1, 32)] public int wMultiplier = 1;
    public static int width;
    public int _width;
    [Range(1, 32)] public int hMultiplier = 1;
    public static int height;
    public int _height;
    static public Vector2 size; // lo mismo que width y heigth solo que en vector2 , por comodidad
    static public Vector2 sizeWithTopLimit; // igual que size pero le sumo el topLimit a la coordernada Y
    static public Vector2 sizeWithExtraMargin; // el margen es una seguridad para checkear objetos fuera del mapa, para que no se los elimine si se pasan muy pocos pixeles
    static public Rect sizeRect = new Rect();
    static public Rect sizeWithTopLimitRect = new Rect();
    static public Rect sizeWithExtraMarginRect = new Rect();
    int mapPixelsCount;  // width * height

    [Header(" - Small Dimensions -")]
    [Range(0.01f, 1f)] public float smallToRealRatio;
    public bool scaleIsBilinear = true;
    public static int smallWidth;
    public static int smallHeight;
    [ReadOnly2Attribute] public Vector2 smallSize;
    int smallMapPixelsCount;
    int preResizeSmallWidth;
    int preResizeSmallHeight;

    static public byte[,] terrainFlags;

    [Header(" Debug")]
    Color[] sample;
    Color[] otherPix; // pixels de colores extraños encontrados, no deberia haber ninguno pongo 60 a modo de sample
    DecoManager[] decoManager;


    Transform stickers;// el go padre donde poner los stickers, solo para que esten ordenados

    int objectFileNodeLength = 16; // en principio 15, osea 4 bytes cada int32 y hay 3 int32 + 3 bytes y un byte de relleno
    Color[] srcObjectPix;
    string folderPath = "";

    private Point up = new Point(0, 1);
    private Point down = new Point(0, -1);
    private Point left = new Point(-1, 0);
    private Point right = new Point(1, 0);
    private Point upLeft = new Point(-1, 1);
    private Point upRight = new Point(1, 1);
    private Point downLeft = new Point(-1, -1);
    private Point downRight = new Point(1, -1);

    [HideInInspector] public byte[] tempAlphaData; // map se procesa aqui para no modificar los valores de map que afectarian al resultado , SI NO ME EQEUIVOCO SIEMPRE ES del tamaño de SMALLALPHADATA

    void Awake()
    { //  print (this + " AWAKE ");
        instance = this;
        
        #if UNITY_EDITOR
        if (BuildPipeline.isBuildingPlayer || KuchoBuildData.isBuildingNow)
        {
            return;
        }
        #endif

        if (Application.isPlaying && KuchoHelper.IsUserLevel())
        {
            foreach (Terrain2D t in _terrains2D)
                t.RestoreTileMaterialFromTerrainDataBase();
            WorldLightWall[] walls = FindObjectsOfType<WorldLightWall>();
            foreach (WorldLightWall w in walls)
                w.RestoreLightObstacleMaterial();
        }
        
        AssignStaticTerrainVariables();
        PublicOnValidate();
        
        if (Application.isPlaying && KuchoHelper.IsUserLevel())
        {
            foreach (Terrain2D t in _terrains2D)
                t.RestoreLightObstacleGenerator();
        }

        //SubscribeToOnWorldLightMapCreatedEvent();
        if(Application.isPlaying)
            BringParallaxSunAndSky();
        oldParallaxOffset = parallaxOffset;
        if (!enabled) // a veces, en editor pasa que se apaga, creo que esta relacionado con el asset de guardar los cambios en editor
        {
            enabled = true;
            AdjustWorldToNewSize();
        }
        InitialiseAllTerrains(true);
    }

    #if UNITY_EDITOR
    public void InitialiseInEditor()
    {
        foreach (Terrain2D t in _terrains2D)
            t.RestoreFromDataBaseOrGetIndex();
        AssignStaticTerrainVariables();
        PublicOnValidate();
        InitialiseLimits();
        parallaxColorManager = GetComponentInChildren<ParallaxColorManager>();
        AdjustWorldToNewSize();
        var c = KuchoHelper.FindFirstChildStartingWithName("DecoManagers", transform);
        if (c)
        {
            DestroyImmediate(c.gameObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
    }
    #endif
    

    [HideInInspector] public static bool onLevelWasLoadedHappened = false;

    
    bool subscribedToOnWorldLightMapCreatedEvent = false;

    public void SetFolderPath()
    {
        folderPath = Application.dataPath + "/LevelEditor/";
    }
    public void InitialiseAllTerrains(bool forceFillSpriteTexture)
    {
        foreach (Terrain2D terr in terrains2D)
            terr.Initialise(forceFillSpriteTexture);
    }
    
    public void ZeroAllTerrainTransforms()
    {
        if (_destructible)
            _destructible.ZeroMyPosition();
        if (_indestructible)
            _indestructible.ZeroMyPosition();
        if (_background)
            _background.ZeroMyPosition();
    }

        public void LookForDestructibles()
    {
        // Comprueba que tengamos los componentes Terrain2D necesarios, crea y / o destruye si es necesario
        NullTerrainsIfEqual();
        
        if (!_destructible || !_indestructible || !_background
            //            || _destructible.gameObject != gameObject || _indestructible.gameObject != gameObject || !_background.gameObject != gameObject
            || !_destructible.d2dSprite || !_indestructible.d2dSprite || !_destructible.spriteGO || !_indestructible.spriteGO)
        {
            _terrains2D = GetComponentsInChildren<Terrain2D>();
            if (_terrains2D.Length < 3)
            {
                Debug.LogError(" ME FALTA UN TERRAIN2D, DEBE HABER 3");
            }
            else if (_terrains2D.Length > 3)
            {
                Debug.LogError(" HAY MAS DE 3 TERRAIN2D, DEBE HABER SOLO 3");
            }
            else
            {
                if (_terrains2D[0].groundType == _terrains2D[1].groundType ||
                    _terrains2D[0].groundType == _terrains2D[2].groundType ||
                    _terrains2D[1].groundType == _terrains2D[2].groundType)
                    Debug.Log(" HAY TERRAINS2D CON EL MISMO TIPO DE GROUND TYPE, REASIGNA EN INSPECTOR PONIENDOLO EN MODO DEBUG");
            }

            if (_terrains2D.Length == 3)
            {
                foreach (Terrain2D t in _terrains2D)
                {
                    switch (t.groundType)
                    {
                        case (GroundType.Destructible):
                            if (!_destructible)
                                _destructible = t; // no teniamos un destructible, asignalo
                            else if (t != _destructible)
                                Destroy(t); // si que tenismoa un destructible y ademas es diferente del que hemos encontrado, destruiyel
                            break;
                        case (GroundType.Indestructible):
                            if (!_indestructible)
                                _indestructible = t; // no teniamos un destructible, asignalo
                            else if (t != _indestructible)
                                Destroy(t); // si que tenismoa un destructible y ademas es diferente del que hemos encontrado, destruiyel
                            break;
                        case (GroundType.Background):
                            if (!_background)
                                _background = t; // no teniamos un destructible, asignalo
                            else if (t != _background)
                                Destroy(t); // si que tenismoa un destructible y ademas es diferente del que hemos encontrado, destruiyel
                            _background = t;
                            break;
                    }
                }

                NullTerrainsIfEqual();
            }

            if (!_destructible && _terrains2D.Length < 3)
            {
                _destructible = gameObject.AddComponent<Terrain2D>();
                _destructible.groundType = GroundType.Destructible;
                _terrains2D = GetComponentsInChildren<Terrain2D>();
            }
            if (!indestructible && _terrains2D.Length < 3)
            {
                _indestructible = gameObject.AddComponent<Terrain2D>();
                _indestructible.groundType = GroundType.Indestructible;
                _terrains2D = GetComponentsInChildren<Terrain2D>();
            }
            if (!_background && _terrains2D.Length < 3)
            {
                _background = gameObject.AddComponent<Terrain2D>();
                _background.groundType = GroundType.Background;
                _terrains2D = GetComponentsInChildren<Terrain2D>();
            }
            CalculateMapSizeBasedOnTilesAndMultipliers();
            foreach (Terrain2D t in _terrains2D)
            {
                t.InitialiseInEditor();
            }
        }

        AssignStaticTerrainVariables(); 
        
        if (KuchoHelper.IsUserLevel())
        {
            destructible.d2dPoly.rebuildCollidersAtStart = true;
            indestructible.d2dPoly.rebuildCollidersAtStart = true;
        }
    }

    void OnValidate()
    {
        if (isActiveAndEnabled)
        {
            if (parallaxColorAdd == Constants.solidWhite)
                parallaxColorAdd = Constants.solidBlack;

            //esto evita que se llame dos veces cuando le damos a play desde editor
#if UNITY_EDITOR
            if (!Application.isPlaying && background && background.spriteRenderer.sharedMaterial)
            {
                wallGain = background.spriteRenderer.sharedMaterial.GetFloat("_PreGain");
                wallShift = background.spriteRenderer.sharedMaterial.GetFloat("_PreShift");
            }
#endif
        }
    }

    private Vector2 oldParallaxOffset;
    public void PublicOnValidate()
    {
        if (topLimit == null)
            InitialiseLimits();
        
#if UNITY_EDITOR
        if (BuildPipeline.isBuildingPlayer || KuchoBuildData.isBuildingNow)
            return;
#endif
        instance = this;

        if (!_spritePlanes)// la serializada
            _spritePlanes = GetComponent<SpritePlanes>();

        spritePlanes = _spritePlanes; // la estatica

        LookForDestructibles();

        if (!useDesignedMap)
            CalculateMapSizeBasedOnTilesAndMultipliers();

        if (debug)
        {
            foreach (Terrain2D t in terrains2D)
            {
                if (t.d2dSprite)
                    t.d2dSprite.CheckAlphaData();
                if (t.texture)
                    print(t.groundType + " TEX SIZE=" + t.texture.width + "x" + t.texture.height);
                else
                    print(t.groundType + " TEX NULL");
            }
        }

        InitialiseAllTerrains(false);
        //          LookForDestructibles(true);

        if (useDesignedMap) // MAPA DISEÑADO
        {
            //          if (designedMap == null)
            //          {
            //              designedMap = GetComponent<DesignedMap>();
            //              if (!designedMap) designedMap = gameObject.AddComponent<DesignedMap>();
            //          }

            // copia un src sobre otro al fin y al cabo ambos terrenos se nutren de la misma textura ya que sus pixeles no pueden ocupar el mismo lugar
            if (destructible.srcTerrain != null || indestructible.srcTerrain != null)
            {
                if (destructible.srcTerrain == null && indestructible.srcTerrain != null)
                    indestructible.srcTerrain = destructible.srcTerrain;
                if (indestructible.srcTerrain == null && destructible.srcTerrain != null)
                    destructible.srcTerrain = indestructible.srcTerrain;
            }
            // compruebo si las tiles encajan con las texturas
            if (destructible.srcTerrain != null && destructible.tile)
            {
                if (destructible.srcTerrain.width % destructible.tile.width != 0)
                {
                    print(" ELIMINANDO SRC TERRAIN POR QUE NO ES DIVISIBLE POR ANCHO DE TILE");
                    destructible.srcTerrain = null; // si no es divisible
                }
                if (destructible.srcTerrain.height % destructible.tile.height != 0)
                {
                    print(" ELIMINANDO SRC TERRAIN POR QUE NO ES DIVISIBLE POR ALTO DE TILE");
                    destructible.srcTerrain = null; // si no es divisible 
                }
            }
            if ((destructible.tile != null && indestructible.tile != null) &&
                    (destructible.tile.width != indestructible.tile.width || destructible.tile.height != indestructible.tile.height))
            {
                print(" LAS TILES DE MAPA DISEÑADO SON DE DIFERENTE TAMAÑO, ELIMINANDO AMBAS");
                destructible.tile = null;
                indestructible.tile = null;
            }
            // DEFINE TAMAÑO DEL MUNDO
            if (destructible.srcTerrain != null) // existe terreno fuente
            {
                width = destructible.srcTerrain.width; // asigna dimensiones de mundo segun sea de grande la textura de terreno fuente
                height = destructible.srcTerrain.height;
                _width = width;
                _height = height;
            }
            else// por lo que sea se ha borrado el Source, pero seguramente permanezca el d2dsprite , intento tirar de sus medidas
            {
                width = destructible.d2dSprite.spriteRenderer.sprite.texture.width;
                height = destructible.d2dSprite.spriteRenderer.sprite.texture.height;
                _width = width;
                _height = height;
            }
            RedefineTerrainFlags();

            if (destructible.tile)
            {
                wMultiplier = width / destructible.tile.width; // asigna los multipliers segun el tamaño de laa textura mundo y la tile, es solo por coherencia, pero en terreno diseñado no se usa
                hMultiplier = height / destructible.tile.height;
            }
        }
        else // MAPA ALEATORIO
        {
            //          var designedMaps = GetComponents<DesignedMap>();
            //          if (designedMaps.Length >= 0){
            //              foreach(DesignedMap m in designedMaps)
            //              {
            //                  UnityEditor.EditorApplication.delayCall+=()=>{DestroyImmediate(m);}; 
            //              }
            //          }
            switch (workingOnMap)
            {
                case (GroundType.Destructible):
                    if (selectedTerrain != destructible)
                        selectedTerrain = destructible;
                    break;
                case (GroundType.Indestructible):
                    if (selectedTerrain != indestructible)
                        selectedTerrain = indestructible;
                    break;
                case (GroundType.Background):
                    if (selectedTerrain != background)
                        selectedTerrain = background;
                    break;
            }
            destructible.UpdateSmoothAndPolishThresholds();
            indestructible.UpdateSmoothAndPolishThresholds();
            background.UpdateSmoothAndPolishThresholds();
        }
        mapPixelsCount = width * height;
        size = new Vector2((int)width, (int)height);
        sizeWithTopLimit = size;
        sizeWithTopLimit.y += topWorldLimitExtraHeight;
        
        sizeWithExtraMargin = sizeWithTopLimit + new Vector2(extraSideMargin, extraSideMargin) * 2; // el side margin tambien se sume a la Y ademas del topworld limit
        sizeWithExtraMargin.y += extraBottomMargin;
        
        sizeRect.width = size.x;
        sizeRect.height = size.y;
        sizeWithTopLimitRect.width = sizeWithTopLimit.x;
        sizeWithTopLimitRect.height = sizeWithTopLimit.y;
        sizeWithExtraMarginRect.width = sizeWithExtraMargin.x;
        sizeWithExtraMarginRect.height = sizeWithExtraMargin.y;
        sizeWithExtraMarginRect.center = worldCenter;
        CalculateSmallSizeBasedOnBigSize();
    }

    public void InitialiseLimits()
    {
        var limits = KuchoHelper.FindChildrenWichContainsNameRecursive("WorldLimit", transform);
        //var rightWorldLimit = KuchoHelper.FindChildWithNameRecursive("WorldLimitRight", transform);
        foreach (Transform t in limits)
        {
            t.gameObject.layer = Layers.groundForGoodGuysOnly;
            if (t.name.Contains("Right"))
            {
                rightLimit = t;
            }
            else if (t.name.Contains("Left"))
            {
                leftLimit = t;
            }
            else if (t.name.Contains("Top"))
            {
                topLimit = t;
            }            
            else if (t.name.Contains("Bottom"))
            {
                bottomLimit = t;
            }
        }

        rightLimit.position = new Vector3(width, rightLimit.transform.position.y, rightLimit.transform.position.z);
    }

    private DayNightCycleColorProcessor parallaxColorProcessor;
    public void AssignIlluminationValues()
    {
        if (join)
        {
            zAddFactor -= joinSeparateValue;
            zMulFactor += joinSeparateValue;
            join = false;
        }
        if (separate)
        {
            zAddFactor += joinSeparateValue;
            zMulFactor -= joinSeparateValue;
            separate = false;
        }

        if (ParallaxFather.instance)
        {
            if (ParallaxFather.instance.useDayNightProcessor)
            {
                var p = ParallaxFather.instance.dayNightProcessor[0];
                p.fixedColorAdd = parallaxColorAdd;
                p.fixedColorMult = parallaxColorMult;
                p.zAddFactor = zAddFactor;
                p.zMulFactor = zMulFactor;
                p.fixedColormode = fixedColorParallax;
                p.MyUpdate();
            }
        }
        
        if (SkyManager.instance)
        {
            SkyManager.instance.EnableCopySunColor();
            SkyManager.instance.dayLightReductionFactor = dayLightReduction;
            SkyManager.instance.skyTextureScroll = skyTextureScroll;
            SkyManager.instance.scroller.SetOffset(skyOffset); // lo mueve a este offset pero no cambis su variable
            SkyManager.instance.scroller.offset = skyOffset; // pa que esté igual
            SkyManager.instance.copySunColor[0].mainTint.colorMix = 1 - sunColorToSky;
        }

        if (SkyLights.instance)
        {
            SkyLights.instance.intensity = skyLightsIntensity;
            SkyLights.instance.saturation = skyLightsSaturation;
            SkyLights.instance.sunColorFactor = sunToSkylightsColorRatio;
        }

        if (Sun.instance)
        {
            Sun.instance.skyManagerColorRatio = skyToSunColorRatio;
            Sun.instance.desaturationFactor = sunSaturation;
            Sun.instance.extraIntensity = sunIntensity;
            Sun.instance.forceColor = forceSunColor;
            if (forceSunColor)
                Sun.instance.actualColor = sunColor;
        }

        if (background)
        {
            var m = background.spriteRenderer.sharedMaterial;
            if (contrastDec)
            {
                wallGain -= 0.05f;
                wallShift += 0.05f;
                contrastDec = false;
            }
            if (contrastInc)
            {
                wallGain += 0.05f;
                wallShift -= 0.05f;
                contrastInc = false;
            }

            m.SetFloat("_PreGain", wallGain);
            m.SetFloat("_PreShift", wallShift);
        
        }
    }


    public void AssignGrassShaderValues()
    {
        grass.SetValuesOnMaterials();
    }

    public void AssignStaticTerrainVariables()
    {
        if (!_destructible || !_indestructible)
        {
            LookForDestructibles();
        }
        instance = this;
        terrains2D = _terrains2D;
        destructible = _destructible;
        indestructible = _indestructible;
        if (!destructible || !indestructible)
        {
            Debug.LogError(this + " ME FALTA ALGUN TERRENO : Destructible:" + destructible + " Indestructible:" + indestructible);
        }
        else
        {
            if (Application.isPlaying) // me daba un null reference al hacer build, no se cual era null destructible, d2dpoly ... para mi que todas tendrian que estar asignadas pero bueno
            {
                PhysicsHelper.parentOfDestructibleColliders = destructible.d2dPoly.parentOfAllColliders.transform;
                PhysicsHelper.parentOfIndestructibleColliders = indestructible.d2dPoly.parentOfAllColliders.transform;
            }

            background = _background;

#if UNITY_EDITOR
            SceneVisibilityManager.instance.DisablePicking(destructible.gameObject, true);
            SceneVisibilityManager.instance.DisablePicking(indestructible.gameObject, true);
            SceneVisibilityManager.instance.DisablePicking(background.gameObject, false);
#endif
        }
    }
    void NullTerrainsIfEqual()
    {
        if (destructible == null && indestructible == null && background == null)
            return;
        if (destructible == indestructible || destructible == background || indestructible == background) // a veces pasa que se asignan igual
        {
            destructible = null;
            indestructible = null;
            background = null;
            Debug.Log("ALGUN TERRAIN2D ERA IGUAL AL OTRO? PONIENDOLOS A NULL");
        }
    }
    public void CalculateMapSizeBasedOnTilesAndMultipliers()
    {
        if (destructible.tile)
        {
            width = wMultiplier * destructible.tile.width;
            height = hMultiplier * destructible.tile.height;
        }
        else
        {
            Debug.LogError("NO PUEDO CALCULAR TAMAÑO DEL MUNDO POR QUE NO TENGO DESTRUCTIBLE TILE!");
            width = 0;
            height = 0;
        }
        _width = width;
        _height = height;
        worldCenter = new Vector2(width / 2, height / 2);
        RedefineTerrainFlags();
        // por si lo tocamos a mano pero no se deberian tocar
        //  if (width % destructible.tile.width != 0) width = Mathf.RoundToInt(width/destructible.tile.width) * destructible.tile.width;
        //  if (height % destructible.tile.height != 0) height = Mathf.RoundToInt(height/destructible.tile.height) * destructible.tile.height;
    }
    void CalculateSmallSizeBasedOnBigSize()
    {
        smallSize = size * smallToRealRatio;
        smallWidth = (int)((float)width * smallToRealRatio);
        smallHeight = (int)((float)height * smallToRealRatio);
    }
    static void RedefineTerrainFlags()
    {
        if (terrainFlags == null || terrainFlags.GetLength(0) != width || terrainFlags.GetLength(1) != height) terrainFlags = new byte[width, height];
    }
    public static void CleanTerrainFlags(int sx, int sy, int ex, int ey)
    {
        RedefineTerrainFlags();
        for (int x = sx; x < ex; x++)
        {
            for (int y = sy; y < ey; y++)
            {
                terrainFlags[x, y] = 0;
            }
        }
    }
    public static int GetTerrainFlagCount()
    {
        int flagCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainFlags[x, y] == 1)
                {
                    flagCount++;
                }
            }
        }
        return flagCount;
    }
   
    public void AdjustWorldToNewSize()
    {
        if (width == 0 || height == 0)
        {
            print(this + " ERROR, DIMENSIONES DE MUNDO INCORRECTAS" + width + " x " + height); 
            Debug.Break(); Debug.Log(this + "!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
        else
        {
            worldCenter = new Vector2(width * 0.5f, height * 0.5f);
            SetTopLimit();
            //      CenterSunLight();
            RedefineWorldLightMap();

            UpdateWorldLightWalls();
        }
    }
    public void SetTopLimit()
    {
        //topLimit = KuchoHelper.FindChildWithNameRecursive("WorldLimitTop", transform);
        topLimit.position = new Vector3(topLimit.position.x, height + topWorldLimitExtraHeight, topLimit.position.z);
    }
    public void UpdateWorldLightWalls()
    {
        WorldLightWall[] walls = FindObjectsOfType<WorldLightWall>();
        foreach (WorldLightWall w in walls)
        {
            if (debug)
                print(this + " LLAMANDO a CREAR WORLD LIGHT WALL " + w.name);
            w.Build(destructible.d2dSprite.AlphaTex, indestructible.d2dSprite.AlphaTex, simpleWalls);
        }
    }
    public void RedefineWorldLightMap()
    {
        /*
        LightingSystemExtras lse = FindObjectOfType<LightingSystemExtras>();
        if (lse && Application.isPlaying)
        {
            lse.worldSize.x = width; // si no estamos playing estos valores se fijan pero luego, no se por que al dar a play se recuperan los que tenian antes ...WTF?
            lse.worldSize.y = height;
            lse.CreateWorldLightMap();
        }
        */
    }
    public void InstantiateStickerModelsPrefab()
    {
        StickerModels _stickerModels = GetComponentInChildren<StickerModels>(true);

        if (_stickerModels == null)
        {
            GameObject go = Instantiate(GameData.instance.stickerModels.gameObject, Constants.zero3, Constants.zeroQ) as GameObject;
            go.transform.parent = transform;
            go.transform.localPosition = Constants.zero3;
        }
    }
    public void InstantiateGroundEdit()
    {
        bool found = false;
        var rootsInScene = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject go in rootsInScene)
        {
            if (go.name == "GroundEdit" || go.name == GameData.instance.groundEdit.name)
            {
                var tc = go.GetComponent<ExplosionStampExtras>();
                if (tc)
                {
                    Debug.Log(" GROUND EDIT ENCONTRADO EN ESCENA DESTRUYENDO");
                    found = true;
                    break;
                }
            }
        }
        if (!found)
        {
            GameObject go = Instantiate(GameData.instance.groundEdit, Constants.zero3, Constants.zeroQ) as GameObject;
            go.transform.localPosition = size * 0.5f;
        }
    }
    
    //----------------------------------------------RANDOM MAP GENERATOR--------------------------------
    public void GenerateAllMaps()
    {
        Terrain2D[] terrains = GetComponents<Terrain2D>(); // los pillo de neuvo para procesarlos por orden ya que se pueden reordenar en inspector y el orden de generado influye
        foreach (Terrain2D terr in terrains)
        {
            GenerateMap(terr);
            ApplyMapChangesOnSmallAlphaData(terr);
        }
    }

    public void GenerateMap(Terrain2D terr)
    { // sin segundo parametro aplica smooth
        GenerateMap(terr, true);
    }
    public void GenerateMap(Terrain2D terr, bool smooth)
    {

        if (terr.tile)
        {
            print("GENERANDO MAPA: FillMin=" + terr.fillMin + " FillMax=" + terr.fillMax);
            CheckSizeConsistency(terr); // esto regenera alphadatas si no cuadran con el tamaño de pixeles del mundo

            TS_SpriteCollider sprCollider = terr.d2dSprite.GetComponent<TS_SpriteCollider>();
            if (sprCollider)
                sprCollider.rebuildAllCollidersAllowed = false;
            terr.FillSpriteTextureWithTile(width, height); // con esto ademas se redefine el tamaño de la texura del sprite

            if (smallToRealRatio <= 0)
            {
                Debug.LogError(" ERROR SMALL SIZE RATIO ES ZERO");
            }
            else
            {
                RandomFillMap(terr);

                CopyAlphaData(terr.smallAlphaData, tempAlphaData);

                if (smooth && terr.smoothPasses > 0)
                {
                    int originalPixelsAroundToSmooth = terr.smoothRange;
                    for (int i = 0; i < terr.smoothPasses; i++)
                    {
                        SmoothMap(terr, 1,false);
                        terr.smoothRange = (int)((float)terr.smoothRange * terr.smoothRangePassMultipler);
                    }
                    terr.smoothRange = originalPixelsAroundToSmooth;
                }
            }
        }
        else
        {
            Debug.Log(this + " INTENTO DE GENERAR TERRENO " + terr.groundType.ToString() + " PERO NO TENGO TILE");
        }
    }
    public void RandomFillMap(Terrain2D terr)
    {
        if (terr.tile)
        {
            var smallAlphaData = terr.smallAlphaData;
            byte[] otherSmallAlphaData = null;
            byte[] otherSmallAlphaData2 = null;
            bool checkOtherTerrain = false;
            if (terr.otherTerrainFactor > 0 && terr.otherTerrainReference != GroundReference.None)
                checkOtherTerrain = true;
            int otherAlphaDataValue = 0;
            if (checkOtherTerrain)
            {
                switch (terr.otherTerrainReference)
                {
                    case (GroundReference.Destructible):
                        CheckSizeConsistency(destructible);
                        otherSmallAlphaData = destructible.smallAlphaData;
                        otherSmallAlphaData2 = destructible.smallAlphaData;
                        break;
                    case (GroundReference.Indestructible):
                        CheckSizeConsistency(indestructible);
                        otherSmallAlphaData = indestructible.smallAlphaData;
                        otherSmallAlphaData2 = indestructible.smallAlphaData;
                        break;
                    case (GroundReference.Background):
                        CheckSizeConsistency(background);
                        otherSmallAlphaData = background.smallAlphaData;
                        otherSmallAlphaData2 = background.smallAlphaData;
                        break;
                    case (GroundReference.DestructiblePlusIndestructible):
                        CheckSizeConsistency(destructible);
                        CheckSizeConsistency(indestructible);
                        otherSmallAlphaData = destructible.smallAlphaData;
                        otherSmallAlphaData2 = indestructible.smallAlphaData;
                        break;
                }
            }

            float smallHeighFloat = (float)smallHeight; // para que pueda multiplicarse con otros floats
            float sum = 0;
            if (smallToRealRatio <= 0)
            {
                print(" ERROR SMALL SIZE RATIO ES ZERO");
            }
            else
            {
                for (float y = 0; y < smallHeighFloat; y++) // la hago float porque ha de multiplicarse con otros float
                {
                    //      var invertedY = height - y;
                    float yFactor = (y * 1.0f) / (smallHeighFloat * 1.0f);
                    float heightSub = terr.heightCleanFactor * yFactor * 255f;
                    float max = terr.fillMax - heightSub;

                    for (int x = 0; x < smallWidth; x++)
                    {
                        //if (x == 0 || x == width-1 || y == 0 || y == height -1) //los bordes
                        //if(y == 0) // la linea de abajo
                        int index = SmallAlphaDataGetIndex(x, (int)y);
                        float toFill = UnityEngine.Random.Range(terr.fillMin, max);
                        if (checkOtherTerrain)
                        {
                            int otherAlphaY = (int)Mathf.Clamp(y + terr.otherTerrainOffsetY, 0, smallSize.y - 2);
                            int otherIndex = SmallAlphaDataGetIndex(x, otherAlphaY);
                            if (otherIndex < otherSmallAlphaData.Length)
                            {
                                otherAlphaDataValue = otherSmallAlphaData[otherIndex] + otherSmallAlphaData2[otherIndex];
                                if (otherAlphaDataValue > 255)
                                    otherAlphaDataValue = 255;
                            }
                            else
                            {
                                otherAlphaDataValue = 0;
                            }
                            toFill += (otherAlphaDataValue * terr.otherTerrainFactor);
                        }
                        if (toFill > 255)
                            toFill = 255;
                        else if (toFill < 0)
                            toFill = 0;

                        smallAlphaData[index] = (byte)toFill;
                        sum += toFill;
                    }
                }
                print(this + " GENERADO " + terr.name + " SUMA DE ALPHAS=" + (int)sum);
            }//cada vez que se genera mapa se guarda el tamaño small para poder redimensionarlo cuando se pulsa COMMIT RESIZE
            preResizeSmallWidth = smallWidth;
            preResizeSmallHeight = smallHeight;
        }
    }
    public void ApplyMapChangesOnSmallAlphaData(Terrain2D terr)
    {
        terr.ScaleAlphaDataWithSmallAlphaData(smallWidth, smallHeight, width, height);
        AdjustWorldToNewSize();
    }
    public void ApplyMapChangesOnAlphaData(Terrain2D terr)
    {
        terr.d2dSprite.ReplaceOrUpdateAlphaWith(terr.d2dSprite.AlphaData, width, height);
        AdjustWorldToNewSize();
    }
    public void PolishAllMaps()
    {
        SmoothMap(destructible, 1,true);
        ApplyMapChangesOnSmallAlphaData(destructible);
        SmoothMap(indestructible, 1,true);
        ApplyMapChangesOnSmallAlphaData(indestructible);
        SmoothMap(background, 1,true);
        ApplyMapChangesOnSmallAlphaData(background);
    }
    byte[] bigTempAlphaData;
    byte[] GetBigTempAlphaData()
    {
        if (bigTempAlphaData == null || bigTempAlphaData.Length != mapPixelsCount)
            bigTempAlphaData = new byte[mapPixelsCount];
        return bigTempAlphaData;
    }


    public void SmoothMap(Terrain2D terr, float mult, bool polish)
    { // segun la media del cuadrado en mapa 1 , aplica cambios en mapa 2    
        byte[] aData; // alphaData o smallAlphadata
        byte[] tData; //tempAlphaData o una nueva temporal igual de grande que AlphaData que solo uso para pulir
        int h;
        int w;
        float pixelsAround;
        float threshold;


        if (polish)
        {
            aData = terr.d2dSprite.AlphaData;
            if (aData == null)
            {
                Debug.Log(" IMPOSIBLE POLISH, BIG ALPHA DATA NULL");
                return;
            }
            if (aData.Length != height * width)
            {
                Debug.Log(" IMPOSIBLE POLISH, BIG ALPHA DATA MIDE " + aData.Length + " DEBERIA SER " + height * width);
                return;
            }
            tData = GetBigTempAlphaData();
            h = height;
            w = width;
            terr.UpdatePolishThreshold();
            pixelsAround = terr.polishRange * mult;
            threshold = terr.polishThreshold;
            print(this + " PULIENDO MAPA " + terr.d2dSprite.gameObject.name);
        }
        else
        {
            if (terr.smallAlphaData == null)
            {
                Debug.Log(" RECONSTRUYENDO SMALL ALPHA DATA A PARTIR DE BIG ALPHA DATA, PORQUE ERA NULL");
                terr.GenerateSmallAlphaDataWithAlphaData(width, height, smallWidth, smallHeight);
            }
            else if (terr.smallAlphaData.Length != smallWidth * smallHeight)
            {
                Debug.Log(" RECONSTRUYENDO SMALL ALPHA DATA A PARTIR DE BIG ALPHA DATA, PORQUE SMALL ALPHA DATA MIDE " + terr.smallAlphaData.Length + " DEBERIA SER " + (smallWidth * smallHeight));
                terr.GenerateSmallAlphaDataWithAlphaData(width, height, smallWidth, smallHeight);
            }
            aData = terr.smallAlphaData;
            tData = tempAlphaData;
            h = smallHeight;
            w = smallWidth;
            pixelsAround = 0f;
            threshold = 0f;
            print(this + " SUAVIZANDO MAPA " + terr.d2dSprite.gameObject.name + " SMOOTH RANGE =" + terr.smoothRange * mult);
        }

        int solidCount = 0;
        int transpCount = 0;

        for (int y = 0; y < h; y++)
        {
            if (!polish)
            {
                //          var heightFactor = (((h - y) * 1f) / (h * 1f)) * terr.smoothHeightFactor;
                pixelsAround = ((y * terr.smoothHeightFactor) / h) + terr.smoothRange * mult;
                terr.UpdateSmoothThreshold(pixelsAround);
                threshold = terr.smoothThreshold;
            }
            for (int x = 0; x < w; x++)
            {
                var neighbourWallTiles = GetSurroundingSum(terr, aData, w, x, y, pixelsAround);
                var index = GetIndex(x, y, w);
                //              var alpha = aData[index]; // <--- no la uso ?// TODO borrame
                //              if (tData == null || tData.Length == 0)// TODO saca esto de aqui, ponlo arriba y reconstruye textura si no existe
                //                  CopyAlphaData(aData, tData); // TODO saca esto de aqui
                if (neighbourWallTiles > threshold)
                {
                    //tempAlphaData[index] = ApplyInc(alpha, terr.smoothPassInc);
                    tData[index] = 255;
                    solidCount++;
                }
                else
                {
                    //tempAlphaData[index] = ApplyInc(alpha, - terr.smoothPassInc);
                    tData[index] = 0;
                    transpCount++;
                }
            }
        }
        print("SMOOTH TERMINADO SOLID-COUNT=" + solidCount + " TRANSP-COUNT=" + transpCount);
        CopyAlphaData(tData, aData);
    }

    static int[] indexesToFix1 = new int[100]; // indices a pixeles a alterar no se podran arreglar mas de este numero de esquinas
    static int indexesCount1 = 0;
    static int[] indexesToFix2 = new int[100]; // indices a pixeles a alterar no se podran arreglar mas de este numero de esquinas
    static int indexesCount2 = 0;

    static public void PolishRegion(int w, int h, byte[] bytes)
    { // segun la media del cuadrado en mapa 1 , aplica cambios en mapa 2   
        float threshold = 1147.5f;
        indexesCount1 = 0; // anotaré aqui los pixeles que necesitan cambiar a solido
        indexesCount2 = 0; // anotaré aqui los pixeles que necesitan cambiar a transparente
        int endH = h - 1;
        int endW = w - 1;

        for (int y = 1; y < endH; y++)
        {
            for (int x = 1; x < endW; x++)
            {
                var neighbourSum = GetSurroundingSum(bytes, w, x, y);
                var index = x + y * w;

                if (neighbourSum > threshold) // hay que hacerlo solido
                {
                    if (bytes[index] <= 127) // era transparente 
                    {
                        indexesToFix1[indexesCount1] = index; // anotalo para hacerlo solido
                        indexesCount1++;
                    }
                }
                else // hay que hacerlo transparente
                {
                    if (bytes[index] > 127) // era solido 
                    {
                        indexesToFix2[indexesCount2] = index; // anotalo para hacerlo transparente
                        indexesCount2++;
                    }
                }
            }
        }

        for (int i = 0; i < indexesCount1; i++)
        {
            bytes[indexesToFix1[i]] = 255;
        }
        for (int i = 0; i < indexesCount2; i++)
        {
            bytes[indexesToFix2[i]] = 0;
        }
    }
    static public int GetSurroundingSum(byte[] bytes, int regionWidth, int x, int y)
    { // saca la suma del cuadrado en mapa 1
        float sum = 0;
        int index = x + y * regionWidth;
        int cubeSide = 3;
        int length = bytes.Length; // optimization
        int pixelsAroundInt = 1;
        index = index - pixelsAroundInt - (pixelsAroundInt * regionWidth); 

        for (float iy = 0; iy < cubeSide; iy++) // lo hago float para que s emultiplique
        {
            for (int ix = 0; ix < cubeSide; ix++)
            {
                //if (index < 0) // furta del mapa por abajo || index >= length)
                //{
                //    sum += (255f + iy) * terr.bottomBorderSmoothRatio; // pone + pero esta restndo por que iy es negativo
                //}
                //else if (index >= length) // fuera del mapa por arriba
                //{
                //    sum += (255f - iy) * terr.topBorderSmoothRatio;
                //}
                //else // dentro del mapa
                {
                    sum += bytes[index];
                }
                index++;
            }
            index += regionWidth - cubeSide;
        }
        return (int)sum;
    }
    static Vector2Int[] squareCheck1 = { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(1, -1) };// ^|
    static Vector2Int[] squareCheck2 = { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1) }; // |^
    static Vector2Int[] squareCheck3 = { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) };// |_
    static Vector2Int[] squareCheck4 = { new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1) };// _|

    public void UglyCornerCheckPublic(Terrain2D terr)
    {
        int h = height - 1;
        int w = width - 1;
        byte[] aData = terr.d2dSprite.AlphaData;
        byte[] tData = GetBigTempAlphaData();
        for (int y = 1; y < h; y++)
        {
            for (int x = 1; x < w; x++)
            {
                int index = GetIndex(x, y, width);
                byte a = aData[index];
                tData[index] = a;
                if (a > 0)
                {
                    if (UglyCornerCheck(x, y, squareCheck1, aData) || UglyCornerCheck(x, y, squareCheck2, aData) || UglyCornerCheck(x, y, squareCheck3, aData) || UglyCornerCheck(x, y, squareCheck4, aData))
                        tData[index] = 0; // vacia el pixel de temp alpha data
                }
            }
        }
        CopyAlphaData(tData, aData);
    }
    bool UglyCornerCheck(int x, int y, Vector2Int[] vecs, byte[] a_data)
    {
        foreach (Vector2Int v in vecs)
        {
            int a = a_data[GetIndex(x + v.x, y + v.y, width)];
            if (a > 0)
            {
                return false; // hay un pixel solido ya no es esquina fea
            }
        }
        return true;// todo limpio, es esquina fea
    }
    // copia de las mismas funciones pero estaticas y funcionan sin tData para in game, directamente escriben en lo que leen
    static public void UglyCornerCheckStatic(int width, int height, byte[] bytes)
    {
        indexesCount1 = 0;
        int w = width - 1;
        int h = height - 1;
        for (int y = 1; y < h; y++)
        {
            for (int x = 1; x < w; x++)
            {
                int index = x + y * width;
                if (bytes[index] > 127)
                {
                    if (CheckCorner(x, y, width, squareCheck1, bytes) || CheckCorner(x, y, width, squareCheck2, bytes) || CheckCorner(x, y, width, squareCheck3, bytes) || CheckCorner(x, y, width, squareCheck4, bytes))
                    {
                        indexesToFix1[indexesCount1] = index;
                        indexesCount1++;
                    }
                }
            }
        }
        for (int i = 0; i < indexesCount1; i++)
        {
            bytes[indexesToFix1[i]] = 0;
        }
    }
    static bool CheckCorner(int x, int y, int regionWidth, Vector2Int[] vecs, byte[] bytes)
    {
        foreach (Vector2Int v in vecs)
        {
            int i = (x + v.x) + (y + v.y) * regionWidth;
            if (bytes[i] > 0)
            {
                return false; // hay un pixel solido ya no es esquina fea
            }
        }
        return true;// tod0 limpio, es esquina fea
    }
    public void DetailHorizontalTearsMap(Terrain2D terr)
    { // segun la media del cuadrado en mapa 1 , aplica cambios en mapa 2    
        byte[] aData = terr.d2dSprite.AlphaData;
        byte[] tData = GetBigTempAlphaData();
        int h = height;
        int w = width;
        int howManyPix = terr.horizontalTearLumaDeep;
        int w1 = w - howManyPix;

        int leftDeletionsCount = 0;
        int rightDeletionsCount = 0;

        CopyAlphaData(aData, tData); // clona la textura

        for (int y = 0; y < h; y++)
        {
            float threshold = terr.horizontalTearLumaThreshold;
            for (int x = howManyPix; x < w; x++) // empiezo en 1 para no salirme
            {
                int index = GetIndex(x, y, w);
                byte a = aData[index];
                if (a == 0) // transparente, ahora hay que leer el luma del pixel de la textura, no de la alphadata
                {
                    for (int i = 1; i <= howManyPix; i++) // solo 3 pixels hacia adentro en cada pase
                    {
                        int ii = index - i;
                        if (aData[ii] > 0) // es solido el de al lado?
                        {
                            Color p = terr.GetPixel(x - i, y);
                            float luma = ColorHelper.Luma(p);
                            if (luma < threshold) // hay que borrarlo?
                            {
                                tData[ii] = 0; // borra el pixel en la temp Data
                                leftDeletionsCount++;
                                threshold -= terr.verticalTearLumaThresholdDec;
                            }
                            else // hemos encontrado uno brillante que NO hay que borrar
                            {
                                break; // no se sigue intentando escarbar
                            }
                        }
                    }
                }
            }
            for (int x = 0; x < w1; x++) // w1 = w -1 para no salirme
            {
                int index = GetIndex(x, y, w);
                byte a = aData[index];
                if (a == 0) // transparente, ahora hay que leer el luma del pixel de la textura, no de la alphadata
                {
                    threshold = terr.horizontalTearLumaThreshold;
                    for (int i = 1; i <= howManyPix; i++)
                    {
                        int ii = index + i;
                        if (aData[ii] > 0) // es solido el de al lado?
                        {
                            Color p = terr.GetPixel(x + i, y);
                            float luma = ColorHelper.Luma(p);
                            if (luma < threshold) // hay que borrarlo?
                            {
                                tData[ii] = 0; // borra el pixel en la temp data
                                rightDeletionsCount++;
                                threshold -= terr.verticalTearLumaThresholdDec;
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
        print("HORIZONTAL TEAR DETAIL TERMINADO LEFT-COUNT=" + leftDeletionsCount + " RIGHT-COUNT=" + rightDeletionsCount);
        CopyAlphaData(tData, aData);
    }
    public void DetailVerticalTearMap(Terrain2D terr)
    { // segun la media del cuadrado en mapa 1 , aplica cambios en mapa 2    
        byte[] aData = terr.d2dSprite.AlphaData;
        byte[] tData = GetBigTempAlphaData();
        int h = height;
        int w = width;
        int howManyPix = terr.verticalTearLumaDeep;
        int h1 = h - howManyPix;

        int leftDeletionsCount = 0;
        int rightDeletionsCount = 0;

        CopyAlphaData(aData, tData); // clona la textura

        for (int x = 0; x < w; x++)
        {
            for (int y = howManyPix; y < h; y++) // empiezo en 1 para no salirme
            {
                int index = GetIndex(x, y, w);
                byte a = aData[index];
                if (a == 0) // transparente, ahora hay que leer el luma del pixel de la textura, no de la alphadata
                {
                    float threshold = terr.verticalTearLumaThreshold;
                    for (int i = 1; i <= howManyPix; i++)
                    {
                        int ii = index - w * i; // pixel de abajo
                        if (aData[ii] > 0) // es solido el de al abajo?
                        {
                            Color p = terr.GetPixel(x, y - w);
                            float luma = ColorHelper.Luma(p);
                            if (luma < threshold) // hay que borrarlo?
                            {
                                tData[ii] = 0; // borra el pixel en la temp Data
                                leftDeletionsCount++;
                                threshold -= terr.verticalTearLumaThresholdDec;
                            }
                            else // hemos encontrado uno brillante que NO hay que borrar
                            {
                                break; // no se sigue intentando escarbar
                            }
                        }
                    }
                }
            }
            for (int y = 0; y < h1; y++) // w1 = w -1 para no salirme
            {
                int index = GetIndex(x, y, w);
                byte a = aData[index];
                if (a == 0) // transparente, ahora hay que leer el luma del pixel de la textura, no de la alphadata
                {
                    float threshold = terr.verticalTearLumaThreshold;
                    for (int i = 1; i <= howManyPix; i++) // solo 3 pixels hacia adentro en cada pase
                    {
                        int ii = index + w * i; // pixel de arriba
                        if (aData[ii] > 0) // es solido el de al lado?
                        {
                            Color p = terr.GetPixel(x, y + w);
                            float luma = ColorHelper.Luma(p);
                            if (luma < threshold)
                            {
                                tData[ii] = 0; // borra el pixel en la temp data
                                rightDeletionsCount++;
                                threshold -= terr.verticalTearLumaThresholdDec;
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
        print("VERTICAL TEAR DETAIL TERMINADO LEFT-COUNT=" + leftDeletionsCount + " RIGHT-COUNT=" + rightDeletionsCount);
        CopyAlphaData(tData, aData);
    }
    public void DetailDownwardsTearMap(Terrain2D terr) // todo codigo extraido y por lo tanto duplicado de DetailVerticalTearMap
    { // segun la media del cuadrado en mapa 1 , aplica cambios en mapa 2    
        byte[] aData = terr.d2dSprite.AlphaData;
        byte[] tData = GetBigTempAlphaData();
        int h = height;
        int w = width;
        int howManyPix = terr.verticalTearLumaDeep;
        int h1 = h - howManyPix;

        int leftDeletionsCount = 0;
        int rightDeletionsCount = 0;

        CopyAlphaData(aData, tData); // clona la textura

        for (int x = 0; x < w; x++)
        {
            for (int y = howManyPix; y < h; y++) // empiezo en 1 para no salirme
            {
                int index = GetIndex(x, y, w);
                byte a = aData[index];
                if (a == 0) // transparente, ahora hay que leer el luma del pixel de la textura, no de la alphadata
                {
                    float threshold = terr.verticalTearLumaThreshold;
                    for (int i = 1; i <= howManyPix; i++)
                    {
                        int ii = index - w * i; // pixel de abajo
                        if (aData[ii] > 0) // es solido el de al abajo?
                        {
                            Color p = terr.GetPixel(x, y - w);
                            float luma = ColorHelper.Luma(p);
                            if (luma < threshold) // hay que borrarlo?
                            {
                                tData[ii] = 0; // borra el pixel en la temp Data
                                leftDeletionsCount++;
                                threshold -= terr.verticalTearLumaThresholdDec;
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
        print("DOWNWARDS TEAR DETAIL TERMINADO LEFT-COUNT=" + leftDeletionsCount + " RIGHT-COUNT=" + rightDeletionsCount);
        CopyAlphaData(tData, aData);
    }
    public void DetailUpwardsTearMap(Terrain2D terr)// todo codigo extraido y por lo tanto duplicado de DetailVerticalTearMap
    { // segun la media del cuadrado en mapa 1 , aplica cambios en mapa 2    
        byte[] aData = terr.d2dSprite.AlphaData;
        byte[] tData = GetBigTempAlphaData();
        int h = height;
        int w = width;
        int howManyPix = terr.upwardsTearLumaDeep;
        int h1 = h - howManyPix;

        int leftDeletionsCount = 0;
        int rightDeletionsCount = 0;

        CopyAlphaData(aData, tData); // clona la textura

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h1; y++) // w1 = w -1 para no salirme
            {
                int index = GetIndex(x, y, w);
                byte a = aData[index];
                if (a == 0) // transparente, ahora hay que leer el luma del pixel de la textura, no de la alphadata
                {
                    float threshold = terr.upwardsTearLumaThreshold;
                    for (int i = 1; i <= howManyPix; i++) // solo 3 pixels hacia adentro en cada pase
                    {
                        int ii = index + w * i; // pixel de arriba
                        if (aData[ii] > 0) // es solido el de al lado?
                        {
                            Color p = terr.GetPixel(x, y + w);
                            float luma = ColorHelper.Luma(p);
                            if (luma < threshold)
                            {
                                tData[ii] = 0; // borra el pixel en la temp data
                                rightDeletionsCount++;
                                threshold -= terr.upwardsTearLumaThresholdDec;
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
        print("DOWNWARDS TEAR DETAIL TERMINADO LEFT-COUNT=" + leftDeletionsCount + " RIGHT-COUNT=" + rightDeletionsCount);
        CopyAlphaData(tData, aData);
    }
    // esta esta optimizada, es mucho mas rapida, pero tiene el "defecto" de que no sabe si esta en el borde de la pantalla por lo que si se sale por la
    // derecha pilla el valor del otro extremo en la izquierda, para moons of darsalon va bien , pero los bordes tienden a tener cierta simetria
    public int GetSurroundingSum(Terrain2D terr, byte[] aData, int aDataWidth, int x, int y, float pixelsAround)
    { // saca la suma del cuadrado en mapa 1
        float sum = 0;
        int index = GetIndex(x, y, aDataWidth);
        int cubeSide = (int)(pixelsAround * 2f) + 1;
        int length = aData.Length; // optimization
        int pixelsAroundInt = Mathf.RoundToInt(pixelsAround);
        index = index - pixelsAroundInt - (pixelsAroundInt * aDataWidth);
        for (float iy = 0; iy < cubeSide; iy++) // lo hago float para que s emultiplique
        {
            for (int ix = 0; ix < cubeSide; ix++)
            {
                if (index < 0) // furta del mapa por abajo || index >= length)
                {
                    sum += (255f + iy) * terr.bottomBorderSmoothRatio; // pone + pero esta restndo por que iy es negativo
                }
                else if (index >= length) // fuera del mapa por arriba
                {
                    sum += (255f - iy) * terr.topBorderSmoothRatio;
                }
                else // dentro del mapa
                {
                    sum += aData[index];
                }
                index++;
            }
            index += aDataWidth - cubeSide;
        }
        return (int)sum;
    }
    public void BinarizeTempAlphaData()
    {
        for (int i = 0; i < tempAlphaData.Length; i++)
        {
            if (tempAlphaData[i] >= 127)
                tempAlphaData[i] = 255;
            else
                tempAlphaData[i] = 0;
        }
    }
    #region METODOS ORIGINALES PARA ENCONTRAR ISLAS Y HUECOS (REGIONS) no conviene usarlos porque va muy lento y trabaja sobre ALPHA DATA (BIG)

    //----------------------------------- BUSCA REGIONES OPERA CON SMALL ALPHA DATA POR QUE TARDA MUCHISIMO 
    // el problema esta en que luego los cambios a hacer solo los puede hacer en small alpha data, asi que todo esto solo tiene sentido hacerlo antes de convertir smallalphadata en alphadata y aplicar smoothers y polish etc...
    // aun asi no se si funciona bienm , encuentra demasiadas regiones...
    [System.Serializable]
    public struct Coord
    {
        public int x; // esto podria ser un short para ahorrar
        public int y; // esto podria ser un short para ahorrar
        //Constructor
        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }
    //CAMBIAR EN MOD MADRE
    private List<List<Coord>> regions;

    public void GetIsles(Terrain2D terr)
    {
        regions = GetRegions(terr, true); // List<List<Coord>> 
        Debug.Log("REGIONES LLENAS ENCONTRADAS " + regions.Count);
    }

    public void RemoveSmallIsles(Terrain2D terr)
    { // este es el metodo original pero tarda un huevo
        //regions = GetRegions(terr, true); // List<List<Coord>> 
        //Debug.Log("REGIONES LLENAS ENCONTRADAS " + regions.Count);
        int modCount = 0;
        foreach (List<Coord> wallRegion in regions)
        {
            if (wallRegion.Count < terr.wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    int index = tile.x + tile.y * getRegionsWidth;
                    getRegionsAlphaData[index] = 0;
                    //terr.smallAlphaData[SmallAlphaDataGetIndex(tile.x, tile.y)] = 0;
                }
                modCount++;
            }
        }
        Debug.Log("MODIFICADAS " + modCount);
    }
    public void RemoveSmallCaves(Terrain2D terr)
    { // este es el metodo original pero tarda un huevo 
        var caves = GetRegions(terr, false); //List<List<Coord>> 
        Debug.Log("REGIONES VACIAS ENCONTRADAS " + caves.Count);
        int modCount = 0;
        foreach (List<Coord> roomRegion in caves)
        {
            if (roomRegion.Count < terr.roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    terr.smallAlphaData[SmallAlphaDataGetIndex(tile.x, tile.y)] = 255;
                }
                modCount++;
            }
        }
        Debug.Log("MODIFICADAS " + modCount);
    }
    
    // variables que clonaran tamaño y datos dependiendo de si usamos bigAlphaData (que tarda mucho pero seria mas fino) o SmallAlphaData
    private int getRegionsWidth;
    private int getRegionsHeigth;
    private byte[] getRegionsAlphaData;
    public List<List<Coord>> GetRegions(Terrain2D terr, bool solid)
    { // devuelve una lista de regiones solidas o huecas
        
        getRegionsWidth = width; // esto podria ser selecionable por menu, de momento lo escribo a fuego y purbo cambiando a mano
        getRegionsHeigth = height; // esto podria ser selecionable por menu, de momento lo escribo a fuego y purbo cambiando a mano
        getRegionsAlphaData = terr.d2dSprite.alphaData;// esto podria ser selecionable por menu, de momento lo escribo a fuego y purbo cambiando a mano
        
        if (solid)
            Debug.Log("OBTENIENDO REGIONES ISLAS");
        else
            Debug.Log("OBTENIENDO REGIONES CUEVAS");
        var sa = new System.Diagnostics.Stopwatch();
        sa.Start();
        int min = 0;
        int max = 0;
        if (!solid)
        {
            min = 0;
            max = terr.binarizeThreshold;
        }
        else
        {
            min = terr.binarizeThreshold;
            max = 255;
        }
        List<List<Coord>> regions = new List<List<Coord>>();
        byte[,] terrainFlags = new byte[getRegionsWidth, getRegionsHeigth];

        if (getRegionsAlphaData != null)
        {

            for (int x = 0; x < getRegionsWidth; x++)
            {
                for (int y = 0; y < getRegionsHeigth; y++)
                {
                    var index = x + y * getRegionsWidth;
                    if (sa.Elapsed.Minutes > 0)
                    {
                        Debug.LogError("TIME OUT index=" + index + " x=" + x + " y=" + y);
                        x = width; // romper el bucle
                        y = width; // romper el bucle
                        Debug.Break();
                        break;
                    }
                    if (index < getRegionsAlphaData.Length)
                    {
                        byte coordAlpha = getRegionsAlphaData[index];
                        if (terrainFlags[x, y] == 0 && min <= coordAlpha && coordAlpha <= max)
                        {
                            var newRegion = GetRegion(terr, x, y); // List<Coord>
                            regions.Add(newRegion);

                            foreach (Coord coord in newRegion)
                            {
                                terrainFlags[coord.x, coord.y] = 1;
                            }
                        }
                    }
                    else
                    {
                        Debug.Break();
                    }
                }
            }
        }
        else
        {
            Debug.LogError("NO TENGO REGIONS ALPHA DATA");
        }

        return regions;//List<List<Coord>> 
    }
    public List<Coord> GetRegion(Terrain2D terr, int startX, int startY)
    { // Devuelve una Lista que es una region de pixeles solidos o no solidos dependiendo de como es la posicion inicial
        byte[,] terrainFlags = new byte[getRegionsWidth, getRegionsHeigth]; // ojo esto usaba width y height del mundo, osea de big alpha data , mal!
        //byte tileAlpha = getRegionsAlphaData[SmallAlphaDataGetIndex(startX, startY)]; // PROBLEMA ! small o big alpha data?
        byte tileAlpha = getRegionsAlphaData[startX + startY * getRegionsWidth]; // esto lo soluciona ademas es mas rapido
        int min = 0;
        int max = 0;
        if (tileAlpha < terr.binarizeThreshold)
        { //<---- esto esta bien? creo que esta al reves, comprueba si tocando los threshold en inspector corresponde con lo que hacen
            min = 0;
            max = terr.binarizeThreshold;
        }
        else
        {
            min = terr.binarizeThreshold;
            max = 255;
        }
        List<Coord> coords = new List<Coord>();
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        terrainFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            var coord = queue.Dequeue(); //Coord
            coords.Add(coord);

            for (int x = coord.x - 1; x <= coord.x + 1; x++)
            {
                for (int y = coord.y - 1; y <= coord.y + 1; y++)
                {
                    //if (IsInSmallTerrainRange(x, y) && (y == coord.y || x == coord.x)) // PROBLEMA ESTO NO ES COMPATIBLE CON SELECCIONAR BIG O SMALL ALPHA DATA
                    if (IsInsideWorldMap(x, y) && (y == coord.y || x == coord.x)) // PROBLEMA ESTO NO ES COMPATIBLE CON SELECCIONAR BIG O SMALL ALPHA DATA
                    {
                        tileAlpha = getRegionsAlphaData[x + y * getRegionsWidth];
                        if (terrainFlags[x, y] == 0 && min <= tileAlpha && tileAlpha <= max)
                        {
                            terrainFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return coords;// List<Coord> 
    }
    
    #endregion
    public void DeleteAllSmallRegionsInSmallAlphaData(Terrain2D terr, bool solid)
    { // mi propio metodo para borrar regiones pequeñas, va super rapido y genera poca basura 
        int min;
        int max;
        int threshold;
        byte deleteFill;

        if (solid)
        {
            min = terr.binarizeThreshold;
            max = 255;
            threshold = terr.wallThresholdSize;
            deleteFill = 0;
        }
        else
        {
            min = 0;
            max = terr.binarizeThreshold;
            threshold = terr.roomThresholdSize;
            deleteFill = 255;
        }

        if (threshold > 0)
        {
            Coord[] checkPoint = new Coord[4];
            checkPoint[0] = new Coord(-1, 0);
            checkPoint[1] = new Coord(1, 0);
            checkPoint[2] = new Coord(0, -1);
            checkPoint[3] = new Coord(0, 1);

            CleanTerrainFlags(0, 0, smallWidth, smallHeight);

            int i;

            for (int startX = 0; startX < smallWidth; startX++)
            {
                for (int startY = 0; startY < smallHeight; startY++)
                {
                    if (terrainFlags[startX, startY] == 0) // pos no marcada? si esta marcada es por que era de otra region ( ya borrada )
                    {
                        byte tileAlpha = tempAlphaData[SmallAlphaDataGetIndex(startX, startY)]; // pillo alpha del mapa
                        if (min <= tileAlpha && tileAlpha <= max) // coicide con lo que buscamos? solido/hueco?
                        {
                            List<Coord> tiles = new List<Coord>(); // crea lista de tiles que definiran la region 
                            Queue<Coord> queue = new Queue<Coord>(); // crea lista queue 
                            queue.Enqueue(new Coord(startX, startY)); // mete la posicion inicial en la Queue
                            terrainFlags[startX, startY] = 1; // marca la pos del mapa como usada

                            while (queue.Count > 0)
                            {
                                var tile = queue.Dequeue(); // Saca la coordenada de la queue y metela en tile
                                tiles.Add(tile);  // añade la tile a la tabla que tiene la region 

                                int x;
                                int y;

                                for (i = 0; i < checkPoint.Length; i++)
                                {
                                    x = tile.x + checkPoint[i].x;
                                    y = tile.y + checkPoint[i].y;
                                    if (IsInSmallTerrainRange(x, y) && terrainFlags[x, y] == 0)
                                    {
                                        tileAlpha = tempAlphaData[SmallAlphaDataGetIndex(x, y)]; // lee del mapa el valor de transparencia
                                        if (min <= tileAlpha && tileAlpha <= max) // es lo que buscamos? solido/hueco?
                                        {
                                            terrainFlags[x, y] = 1; // marca casilla como ya comprobada ( para no repetir )
                                            queue.Enqueue(new Coord(x, y)); // metela en la queue ( para sacarla justo despues al comienzo del loop
                                        }
                                    }
                                }
                            }
                            if (tiles.Count < threshold) // BORRA SI ES PEQUEÑO
                            {
                                for (i = 0; i < tiles.Count; i++)
                                {
                                    tempAlphaData[SmallAlphaDataGetIndex(tiles[i].x, tiles[i].y)] = deleteFill;
                                }
                            }
                        }
                    }
                }
            }
            CopyTempAlphaDataIntoSmallAlphaData(terr);
            UpdateWorldLightWalls();
        }
        return;
    }
    public void FillTerrain1BasedOnTerrain2(bool useSmallAlphaData, Terrain2D terr1, Terrain2D terr2, byte dataToFill)
    {
        byte[] terrain1AlphaData = null;
        byte[] terrain2AlphaData = null;

        if (useSmallAlphaData)
        {
            terrain1AlphaData = terr1.smallAlphaData;
            terrain2AlphaData = terr2.smallAlphaData;
        }
        else
        {
            terrain1AlphaData = terr1.d2dSprite.AlphaData;
            terrain2AlphaData = terr2.d2dSprite.AlphaData;
        }
        int threshold = terr2.binarizeThreshold;
        for (int i = 0; i < terrain1AlphaData.Length; i++)
        {
            if (terrain2AlphaData[i] > threshold) terrain1AlphaData[i] = dataToFill;
        }
    }
    // ----------------------------------
    public void CopyRandomSettingsTo(Terrain2D from, GroundType gr)
    {
        Terrain2D to = null;
        switch (gr)
        {
            case (GroundType.Destructible):
                to = destructible;
                break;
            case (GroundType.Indestructible):
                to = indestructible;
                break;
            case (GroundType.Background):
                to = background;
                break;
                break;
        }
        from.CopyRandomSettingsTo(to);
    }
    public void ScaleAllMaps()
    { // por si acaso hay discrepancias en los tamaños de las texuras, hago cada mapa independientement
        //Resize Alpha Data
        AlphaTexture.Load(tempAlphaData, preResizeSmallWidth, preResizeSmallHeight);
        AlphaTexture.Resize(smallWidth, smallHeight);
        tempAlphaData = new byte[smallWidth * smallHeight];
        AlphaTexture.CopyTo(tempAlphaData); // copia uno a uno los datos, no conviene referenciar los datos de la clase estatica
        BinarizeTempAlphaData();

        ScaleMap(destructible);
        ScaleMap(indestructible);
        ScaleMap(background);
        AdjustWorldToNewSize();
        preResizeSmallWidth = smallWidth;
        preResizeSmallHeight = smallHeight;
    }
    public void ScaleMap(Terrain2D terr)
    {// no llamar nunca aqui , solo puede llamar ResizeAllMaps
        RepairMap(terr);
        float scaleWidthFactor = (width * 1f) / (terr.spriteRenderer.sprite.texture.width * 1f);
        float scaleHeightFactor = (height * 1f) / (terr.spriteRenderer.sprite.texture.height * 1f);
        print(this + " REDIMENSIONANDO MAPA " + terr.d2dSprite.gameObject.name + " A " + width + " x " + height + " ESCALANDO A " + scaleWidthFactor + " x " + scaleHeightFactor);
        ScaleMap(terr, scaleWidthFactor, scaleHeightFactor);
        RepairAlphaData(terr);
    }
    public void ResizeAllMaps()
    {
        ResizeMap(destructible);
        ResizeMap(indestructible);
        ResizeMap(background);
        AdjustWorldToNewSize();
    }
    public void ResizeMap(Terrain2D terr)
    {// no llamar nunca aqui , solo puede llamar ResizeAllMaps
        terr.ResizeAlphaTexAndAlphaData(width, height);
    }
    //  public void ScaleAllMaps(float xMult, float yMult){
    //      D2D_AlphaTex.Load(tempAlphaData, smallWidth, smallHeight);
    //      width = (int)(width * xMult);
    //      height = (int)(height * yMult);
    //      _width = width;
    //      _height = height;
    //      CalculateSmallSizeBasedOnBigSize();
    //      D2D_AlphaTex.Resize(smallWidth, smallHeight);
    //      D2D_AlphaTex.CopyTo(tempAlphaData);
    //
    //      RedefineTerrainFlags();
    //  
    //      ScaleMap(destructible, xMult, yMult);
    //      ScaleMap(indestructible, xMult, yMult);
    //      ScaleMap(background, xMult, yMult);
    //  
    //      AdjustWorldToNewSize();
    //  }
    public void ScaleMap(Terrain2D terr, float xMult, float yMult)
    { // no llamar nunca aqui , solo puede llamar ScaleAllMaps
        RepairMap(terr);
        terr.FillSpriteTexture(width, height, true); // esto redefine el tamaño tambien
        //TextureScaler.scale(terr.dedSprite.AlphaTex, width, height, FilterMode.Trilinear);
        if (terr.tile)
        {
            terr.d2dSprite.ScaleAlphaTex(xMult, yMult);
            terr.ScaleSmallAlphaData(preResizeSmallWidth, preResizeSmallHeight, smallWidth, smallHeight);
            CheckSizeConsistency(terr);
        }
        //CopyAlphaTexToTempAlphaData(terr); <--- para que lo hacia? de cualquier modo esto no sirve, si escalo los 3 mapas a la vez , la tempAlphaData solo puede tene la info de uno de ellos
    }
    public void RepairMap(Terrain2D terr)
    {
        if (terr.spriteRenderer.sprite.texture.width == 0 || terr.spriteRenderer.sprite.texture.height == 0)
        {
            print(this + " ERROR, DIMENSIONES DE TEXTURA DE MAPA " + terr.d2dSprite.gameObject.name + " INCORRECTAS " + terr.spriteRenderer.sprite.texture.width + " x " + terr.spriteRenderer.sprite.texture.height);
            print(this + " REPARANDO Y REGENERANDO MAPA " + terr.d2dSprite.gameObject.name);
            terr.FillSpriteTextureWithTile(width, height);
            GenerateMap(terr);
            ApplyMapChangesOnSmallAlphaData(terr);
        }
    }
    public void RepairAlphaData(Terrain2D terr)
    {
        bool corrupt = false;
        var d2dspr = terr.d2dSprite;
        if (d2dspr.AlphaData.Length != d2dspr.AlphaWidth * d2dspr.AlphaHeight)
        {
            print(this + " DIMENSIONES DE ALPHADATA ERRONEAS EN " + d2dspr + " WIDTH=" + d2dspr.AlphaWidth + " HEIGHT=" + d2dspr.AlphaHeight);
            corrupt = true;
        }
        if (d2dspr.AlphaData.Length != width * height)
        {
            print(this + " TAMAÑO DE ALPHADATA ERRONEO EN " + d2dspr + " ES=" + d2dspr.AlphaData.Length + " DEBERIA SER=" + (width + height));
            corrupt = true;
        }
        if (corrupt)
        {
            d2dspr.ReplaceAlphaWith(new byte[width * height], width, height);
            print(this + " REGENERANDO ALPHA DATA a " + width + " x " + height);
        }

    }
    public void CheckSizeConsistency(Terrain2D terr)
    {
        mapPixelsCount = width * height;
        smallMapPixelsCount = smallWidth * smallHeight;
        if (tempAlphaData == null || tempAlphaData.Length != smallMapPixelsCount)
        {
            print(this + " TAMAÑO DE TEMP ALPHA DATA INCONSISTENTE - REGENERANDO");
            tempAlphaData = new byte[smallMapPixelsCount];
        }
        if (terr.d2dSprite.AlphaData == null || terr.d2dSprite.AlphaData.Length != mapPixelsCount)
        {
            print(this + " TAMAÑO DE " + terr.groundType + " ALPHA DATA INCONSISTENTE - REGENERANDO");
            terr.d2dSprite.ReplaceAlphaWith(new byte[mapPixelsCount], width, height);
        }
        if (terr.smallAlphaData == null || terr.smallAlphaData.Length != smallMapPixelsCount)
        {
            print(this + " TAMAÑO DE " + terr.groundType + " SMALL ALPHA DATA INCONSISTENTE - REGENERANDO");
            terr.GenerateSmallAlphaDataWithAlphaData(width, height, smallWidth, smallHeight);
        }
    }
    public void CopyAlphaData(byte[] source, byte[] destination)
    {
        if (destination == null)
        {
            Debug.Log(" NO PUEDO COPIAR ALPHA DATA , DESTINATION NULL");
            return;
        }
        if (source.Length != destination.Length)
        {
            Debug.Log(" NO PUEDO COPIAR ALPHA DATA , DESTINATION SIZE=" + destination.Length + " DEBERIA SER=" + source.Length);
            return;
        }
        for (int i = 0; i < source.Length; i++)
            destination[i] = source[i];
    }
    public void CopyTempAlphaDataIntoSmallAlphaData(Terrain2D terr)
    { // TODO esta funcion sobra, unifica con CopyAlphaData()
        if (terr.smallAlphaData == null)// añade a proyecto madre
            terr.GenerateSmallAlphaDataWithAlphaData(width, height, smallWidth, smallHeight);
        var smallAlphaData = terr.smallAlphaData;
        if (smallAlphaData.Length != tempAlphaData.Length)
            smallAlphaData = new byte[tempAlphaData.Length];
        for (int i = 0; i < tempAlphaData.Length; i++)
            smallAlphaData[i] = tempAlphaData[i];
    }
    public int ApplyInc(int alpha, int inc)
    { // lee map1 y aplica cambio en tempAlphaData (ya no uso esta funcion)
        alpha += inc;
        if (alpha > 255) alpha = 255;
        else if (alpha < 0) alpha = 0;
        return alpha;
    }
    public void RebuildAllCollidersOfAllMaps()
    {
        RebuildAllColliders(destructible);
        RebuildAllColliders(indestructible);
    }
    public void RebuildAllColliders(Terrain2D terr)
    {
        //terr.d2dSprite.notifyChangesAllowed = true;
        TS_SpriteCollider sprCollider = terr.d2dSprite.GetComponent<TS_SpriteCollider>();
        if (sprCollider) sprCollider.rebuildAllCollidersAllowed = true;
        terr.d2dSprite.NotifyChanges();
    }
    int GetIndex(int x, int y, int _width)
    {
        return x + y * _width;
    }
    int AlphaDataGetIndex(int x, int y)
    {
        return x + y * width;
    }
    int SmallAlphaDataGetIndex(int x, int y)
    {
        return x + y * smallWidth;
    }
    //  void AlphaDataIncX(ref int x, ref int y){
    //      x ++;
    //      if (x >= width)
    //      {
    //          x = 0;
    //          y = y + width;
    //      }
    //  }
    //  void SmallAlphaDataIncX(ref int x, ref int y){
    //      x ++;
    //      if (x >= smallWidth)
    //      {
    //          x = 0;
    //          y = y + smallWidth;
    //      }
    //  }
    /// <summary>
    /// Hay que enviarle el int para recoger el alphaData, por que si no me genera 20 bytes de basura
    /// </summary>
    public void AlphaDataGetPixel(ref int alphaPix, int i, bool indestructibleAsWell)
    {
        alphaPix = destructible.d2dSprite.alphaData[i];
        if (indestructibleAsWell)
            alphaPix += indestructible.d2dSprite.alphaData[i];
    }
    public void AlphaDataGetPixelFromIndestructible(ref int alphaPix, int i)
    {
        alphaPix = indestructible.d2dSprite.alphaData[i];
    }
    /// <summary>
    /// devuelve coordenadas enteras dandole un alpha Index, no se necesita en gameplay , la hice para debuguear las plantas qbugueadas que se quedaban en el aire al pasar de nivel
    /// </summary>
    public Point AlphaDataGetXY(int alphaIndex)
    {
        Point p;
        p.y = alphaIndex / width; // lo quiero truncado
        p.x = alphaIndex - (width * p.y);
        return p;
    }
    public void DoNothing() { }
    public void StartPerformanceTest()
    {
        t = Time.realtimeSinceStartup;
    }
    public void PrintElapsedTime(string str)
    {
        print(str + " " + (Time.realtimeSinceStartup - t));
    }
    /*
    
    public void SwapShader()
    {
        Shader newForegroundMat = null;
        Shader newBackgroundMat = null;
        if (destructible.d2dSprite.SourceMaterial.shader == foregroundShader2)
        {
            newForegroundMat = foregroundShader1;
            newBackgroundMat = backgroundShader1;
        }
        else
        {
            newForegroundMat = foregroundShader2;
            newBackgroundMat = backgroundShader2;
        }
        if (newForegroundMat != null)
        {
            destructible.d2dSprite.SourceMaterial.shader = newForegroundMat;
            indestructible.d2dSprite.SourceMaterial.shader = newForegroundMat;
        }

        if (newBackgroundMat != null)
            background.d2dSprite.SourceMaterial.shader = newBackgroundMat;
    }
    */
    public bool IsInSmallTerrainRange(int x, int y)
    {
        return x >= 0 && x < smallWidth && y >= 0 && y < smallHeight;
    }
    //  public bool IsInTerrainRange(int x, int y) {
    //      return x >= 0 && x < width && y >= 0 && y < height;
    //  }
    public static bool IsInsideSmallMap(int x, int y)
    {
        if (x < 0 ||
            x > smallWidth ||
            y < 0 ||
            y > smallHeight) return false;
        else return true;
    }
    public bool IsInsideWorldMap(float x, float y)
    {
        if (x < 0) return false;
        if (y < 0) return false;
        if (x >= size.x) return false;
        if (y >= size.y) return false;
        return true;
    }
    public bool IsInsideWorldMap(Vector2 pos)
    {
        if (pos.x < 0) return false;
        if (pos.y < 0) return false;
        if (pos.x >= size.x) return false;
        if (pos.y >= size.y) return false;
        return true;
    }
    public bool IsInsideWorldMap(Vector3 pos)
    {
        if (pos.x < 0) return false;
        if (pos.y < 0) return false;
        if (pos.x >= size.x) return false;
        if (pos.y >= size.y) return false;
        return true;
    }
    public bool IsInsideWorldMapPlusTopExtraHeight(Vector3 pos)
    {
        if (topLimit)
        {
            if (pos.x < 0) return false;
            if (pos.y < 0) return false;
            if (pos.x >= size.x) return false;
            if (useTopLimit && pos.y >= topLimit.position.y) return false;
            return true;
        }
        else
        {
            Debug.Log(("NO HAY TOP LIMIT"));
            return false;
        }
    }

    public bool IsInsideWorldMapPlusExtraMargin(Vector3 pos)
    {
        if (pos.x < sizeWithExtraMarginRect.min.x) return false;
        if (pos.y < sizeWithExtraMarginRect.min.y) return false;
        if (pos.x > sizeWithExtraMarginRect.max.x) return false;
        if (pos.y > sizeWithExtraMarginRect.max.y) return false;
        return true;
    }

    bool IsInsideRect(int x, int y, int sx, int sy, int ex, int ey)
    {
        if (x < sx) return false;
        if (y < sy) return false;
        if (x >= ex) return false;
        if (y >= ey) return false;
        return true;
    }
    bool IsInArrayRange(ref byte[,] a, ref int x, ref int y)
    {
        if (x < 0) return false;
        if (y < 0) return false;
        if (x >= a.GetLength(0)) return false;
        if (y >= a.GetLength(1)) return false;
        return true;
    }
    public void BringParallaxSunAndSky()
    {


        if (Game.parallax) // ya hay un farbackground
        {
            if (Game.parallax.name != parallaxName) // pero es otro !
            {
                DestroyImmediate(Game.parallax);// TODO ,mejor hacer que sea SUN quien diga a game que es el nuevo
                Game.parallax = null;  // tengo que ponerlo anull por que detroy no funciona inmediatamente
            }
        }

        if (Game.parallax == null)
        {
            Game.parallax = Instantiate(FarBackgroundData.instance.parallax[parallaxIndex], Constants.zero3, Constants.zeroQ);
            Game.parallax.name = FarBackgroundData.instance.parallax[parallaxIndex].name; // asi le quito el (Clone)
        }

        if (ReferenceEquals(SkyManager.instance, null)) // si no existe 
        {
            if (SkyManager.instance != null) // Como coño va a no existir y ser iguala null?
            {
                DestroyImmediate(SkyManager.instance);
            }

            //GameObject skyPrefab = Resources.Load("SkyAndParallaxPrefabs/Sky") as GameObject;

            var newSky = Instantiate(FarBackgroundData.instance.sky, Constants.zero3, Constants.zeroQ);
            newSky.name = FarBackgroundData.instance.sky.name;// asi le quito el (Clone)
        }

        if (Sun.instance == null)
        {
            var newSun = Instantiate(Game.G.sunPrefab, Constants.zero3, Constants.zeroQ);
            newSun.name = Game.G.sunPrefab.name;
        }

    }

    public Color ModColor(Color input)
    {
        HSLColor c = HSLColor.FromRGBA(input);
        c.h -= cosmosHueShift; // por alguna razon ffunciona al reves que el shader, restar es sumar y viceversa
        if (c.h > 360)
            c.h -= 360;
        else if (c.h < 0)
            c.h += 360;
        c.s *= cosmosSaturation;
        c.l *= cosmosBright;
        Color output;
        output = c.ToRGBA();
        float con = cosmosContrast; 
        output.r = (output.r - 0.5f) * (con) + 0.5f; // contrast
        output.g = (output.g - 0.5f) * (con) + 0.5f; // contrast
        output.b = (output.b - 0.5f) * (con) + 0.5f; // contrast
        return output;
    }

    void SizeInc(int inc){
        Point sizeMultipliers = GetNewSizeMultipliers(inc);
        if ( sizeMultipliers.x > 0 && sizeMultipliers.x <= 32 && sizeMultipliers.y > 0 && sizeMultipliers.y <= 32)
        {
            wMultiplier = sizeMultipliers.x;
            hMultiplier = sizeMultipliers.y;
            CalculateMapSizeBasedOnTilesAndMultipliers();
        }
        else Debug.Log(" ERROR, TAMAÑO MUY GRENDE O MUY PEQUEÑO");
        PublicOnValidate();
    }
    Point GetNewSizeMultipliers(int inc){

        string smaller = "";
        float a;
        float newWidthMult;
        float newHeightMult;

        if (wMultiplier < hMultiplier){
            smaller = "width";
        }
        if (smaller == "width"){
            newWidthMult = wMultiplier + inc;
            a = newWidthMult / wMultiplier; // factor por el que hay que multiplicar al otro
            newHeightMult = Mathf.Round(hMultiplier * a); 
        }
        else{
            newHeightMult = hMultiplier + inc;
            a = newHeightMult / hMultiplier; // factor por el que hay que multiplicar al otro
            newWidthMult = Mathf.Round(wMultiplier * a); 
        }
        return new Point((int)newWidthMult, (int)newHeightMult);
    }
    
    public void SCALE_DOWN()
    {
        SizeInc(-1);
    }
    
    public void SCALE_UP()
    {
        SizeInc(1);
    }
    
    public void COMMIT_SCALE()
    {
        ScaleAllMaps();
    }
    
    public void COMMIT_RESIZE()
    {
        ResizeAllMaps();
    }
    
    public void GENERATE_ALL()
    {
        GenerateAllMaps();
    }
    
    public void POLISH()
    {
        StartPerformanceTest();
        PolishAllMaps();
        ApplyMapChangesOnSmallAlphaData(WorldMap.selectedTerrain);
        PrintElapsedTime(" POLISH TERRAIN HA TARDADO");
    }
    
    public void CLEAR_DES_ON_IND()
    {
        FillTerrain1BasedOnTerrain2(false, WorldMap.destructible, WorldMap.indestructible, 0);
        ApplyMapChangesOnAlphaData(WorldMap.destructible);
    }

    
    public void FILL_DES_WITH_IND()
    {
        FillTerrain1BasedOnTerrain2(false, destructible, indestructible, 255);
        ApplyMapChangesOnAlphaData(WorldMap.destructible);
    }

    
    public void REBUILD_ALL_COLLIDERS()
    {
        RebuildAllCollidersOfAllMaps();
    }

    //function GetSurroundingSum_NO(x:int, y:int) { // saca la suma del cuadrado en mapa 1 (variacion del metodo original, ya no la uso tengo otra mucho mas rapida
    //  bool insideX = false;
    //  bool insideY = false;
    //  int sum = 0;
    //  var ratio = 0f;
    //  var alphaData = selectedTerrain.d2dSprite.AlphaData; // para optimizar
    //  for (int nX = x - selectedTerrain.smoothRange; nX <= x + selectedTerrain.smoothRange; nX ++)
    //  {
    //      for (int nY = y - selectedTerrain.smoothRange; nY <= y + selectedTerrain.smoothRange; nY ++)
    //      {
    //          insideX = false;
    //          insideY = false;
    //
    //          if (nX < 0) // sale del mapa por la izq
    //          {
    //              sum += (255 + nX) * selectedTerrain.sideBorderSmoothRatio; // pone + pero esta restndo por que nx es negativo
    //          }
    //          else if(nX >= width) // sale del mapa por la derecha
    //          {
    //              sum += (255 - (nX - width)) * selectedTerrain.sideBorderSmoothRatio;
    //          }
    //          else insideX = true;
    //
    //          if (nY < 0) // sale del mapa por abajo
    //          {
    //              sum += (255 + nY) * selectedTerrain.bottomBorderSmoothRatio; // pone + pero esta restndo por que ny es negativo
    //          }
    //          else if(nY >= height) // sale del mapa por arriba
    //          {
    //              ratio = selectedTerrain.topBorderSmoothRatio;
    //              if (!insideX) ratio *= 0.10f; // esto hace que las esquinas no tengan facilidad para rellenarse mas que el resto
    //               
    //              sum += (255 - (nY - height)) * ratio;
    //          }
    //          else insideY = true;
    //
    //          if (insideX && insideY)
    //          {
    //              sum += alphaData[AlphaDataGetIndex(nX,nY)];
    //          }
    //      }
    //  }
    //  return sum;
    //}
    public void OnDrawGizmos()
    {
        //  if (map != null)
        //  {
        //      for (int x = 0; x < width; x ++)
        //      {
        //          for (int y = 0; y < height; y ++)
        //          {
        //              Gizmos.color = (map[x,y] == 1)?Color.black:Color.white;
        //              Vector3 pos = new Vector3(-width/2 + x + .5f,0, -height/2 + y+.5f);
        //              Gizmos.DrawCube(pos,Vector3.one);
        //          }
        //      }
        //  }
    }
}
