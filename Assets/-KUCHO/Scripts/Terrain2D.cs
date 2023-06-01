using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Light2D;
using UnityEngine.Profiling;
using UnityEngine.Tilemaps;

using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public enum GroundType {Destructible, Indestructible, Background}
public enum BackgroundType {SingleTile, TileMap, _3DModels}
public enum GroundReference {Destructible, Indestructible, Background, DestructiblePlusIndestructible, None}

public class Terrain2D : MonoBehaviour
{
    public string name;
    public BackgroundType backgroundType = BackgroundType.TileMap;
    public bool groundMakerNeedCatalyst = false;
    public TS_DynamicSprite d2dSprite;
    public TS_PolygonSpriteCollider d2dPoly;
    public GameObject spriteGO;
    public SpriteRenderer spriteRenderer;
    public LightObstacleGenerator lightObstacleGenerator;
    public Texture2D texture;
    [System.NonSerialized] public byte[] smallAlphaData;
    public byte[] undoAlphaData;
    public Rect rect;
    public Tilemap tileMap;
    public TilemapRenderer tileMapRenderer;
    public Material tileMapMaterial;
    public Texture2D tile;
    public Camera cam3DtoSprite;
#if UNITY_EDITOR
    [HideInInspector] public Texture2D oldTile;
#endif
    Material _tileMaterial;

    public Material tileMaterial
    {
        get { return _tileMaterial; }
        set
        {
            if (value != _tileMaterial)
            {
                _tileMaterial = value;
                if (d2dSprite)
                {
                    var t = _tileMaterial.GetTexture("_DefaultTex");
                    _tileMaterial.SetTexture("_MainTex", t);
                    d2dSprite.SetNewMaterial(_tileMaterial);
                    tile = (Texture2D) t;
                }
            }
        }
    }

    Material _cam3DtoSpriteMaterial;

    public Material cam3DtoSpriteMaterial
    {
        get { return _cam3DtoSpriteMaterial; }
        set
        {
            if (value != _cam3DtoSpriteMaterial)
            {
                _cam3DtoSpriteMaterial = value;
                if (spriteRenderer)
                {
                    spriteRenderer.sharedMaterial = _cam3DtoSpriteMaterial;
                }
            }
        }
    }

    public string background3DModelsPath;
    public Background3DModelParent background3DModelsParent;
    private GameObject background3DModelsPrefabInstance;
    private GameObject background3DModelsPrefab;
    public GroundType groundType;
    public Vector2 at_transformPosition;
    public Vector2 at_transformLocalScale;

    [Header("PreDesigned")] public Texture2D srcTerrain;
    public Color[] color;

    [Header("Random Generation")] public GroundType copyRandomSettingsTo;
    public GroundReference otherTerrainReference;
    [Range(0, 1)] public float otherTerrainFactor = 0f;
    [Range(-40, 40)] public int otherTerrainOffsetY = 0;
    public int fillMid; // para el custom inspector, con este y randomRange se calculan fillMin y fillMax que son los que se usan para rellenar el mapa
    public int randomRange; // para el custom inspector, con este y fillMid se calculan fillMin y fillMax que son los que se usan para rellenar el mapa
    [HideInInspector] public float fillMin = 0;
    [HideInInspector] public float fillMax = 255;
    [Range(0, 0.1f)] public float heightCleanFactor = 0f;
    [Range(0, 10)] public int smoothRange = 2; // toma valores negativos para poder jugar con smoothHeightFactor
    [Range(0, 10)] public int smoothPasses = 2;
    [Range(0, 1)] public float smoothRangePassMultipler = 1f;
    [Range(-30, 30)] public float smoothHeightFactor = 0f;
    [Range(0, 1)] public float sideBorderSmoothRatio = 1f;
    [Range(0, 1)] public float bottomBorderSmoothRatio = 1f;
    [Range(0, 1)] public float topBorderSmoothRatio = 1f;
    [HideInInspector] public float smoothSquareRatio = 0.5f; // podria ser ajustable, pero por ahora teniendola a 0.5 da busnos resultados en todos los casos y no parece necesitar ajustes
    public float smoothThreshold = 0f;
    [Range(-30, 30)] public int polishRange = 2;

    public float polishThreshold = 0f;

    //	[Range (0,1000)]
    //  v_ar smoothPassInc = 25f;
    [Range(0, 255)] public int binarizeThreshold = 127;
    [Range(0, 500)] public int wallThresholdSize = 200;
    [Range(0, 500)] public int roomThresholdSize = 200;


    public float horizontalTearLumaThreshold = 0.3f;
    public float horizontalTearLumaThresholdDec = 0.01f;
    public int horizontalTearLumaDeep = 3;
    public int inGameHorizontalLumaDeep = 2;
    public float verticalTearLumaThreshold = 0.3f;
    public float verticalTearLumaThresholdDec = 0.01f;
    public int verticalTearLumaDeep = 3;

    public float upwardsTearLumaThreshold = 0.3f;
    public float upwardsTearLumaThresholdDec = 0.01f;

    public int upwardsTearLumaDeep = 3;
    public int inGameVerticalLumaDeep = 2;


    [HideInInspector] public int changeClickNeed = 5;
    [HideInInspector] public int changeClickCount = 5;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isEditor && UnityEditor.BuildPipeline.isBuildingPlayer && backgroundType == BackgroundType._3DModels)
        {
            if (cam3dtospriteCompressedTexData.Count == 0)
            {
                Debug.LogError(KuchoHelper.GetSceneName() + " " + this + " COMPRESSED DATA VACIO!");
                //FillSpriteTextureWith3DObjects(); Camara rendering during OnValidate is not allowed :_(
            }
            else
            {
                print(KuchoHelper.GetSceneName() + " " + name + " COMPRESSED DATA OK (" + cam3dtospriteCompressedTexData.Count + ")");
            }
        }
    }
#endif
    void Awake()
    {
        at_transformPosition = spriteGO.transform.position;
        at_transformLocalScale = spriteGO.transform.localScale;
        /*if (tileMaterial)
        {
            if (!Constants.appIsEditor || Constants.appIsEditor) // TODO elimina la segunda, esto es asi para probar y para que hno se me olvide
                tileMaterial.shader = ShaderDataBase.instance.GetShaderFromList(tileMaterial.shader);
        }
        */
        RestoreFromDataBaseOrGetIndex();
    }

    public void Initialise(bool forceFillSpriteTexture)
    {
        FillSpriteTexture(forceFillSpriteTexture);
        if (d2dSprite)
        {
            tileMaterial = d2dSprite.SourceMaterial;

            if (tile)
            {
                if (d2dSprite.spriteCollider && d2dSprite.spriteCollider.parentOfAllColliders)
                    d2dSprite.spriteCollider.parentOfAllColliders.transform.localPosition = Constants.zero3; // por alguna razon al salvar scena y destruir el spriterenderer el gameobect child se desplaza un pixel
                if (d2dSprite.dirty || d2dSprite.alphaData == null || d2dSprite.alphaData.Length == 0)
                    d2dSprite.UncompressAlphaDataShort();
            }
        }

        switch (groundType)
        {
            case (GroundType.Destructible):
                spriteGO.tag = "Destructible"; 
                break;
            case (GroundType.Indestructible):
                spriteGO.tag = "Indestructible";
                d2dSprite.Indestructible = true; // si no el global explosions manager flipa
                break;
            case (GroundType.Background):
                spriteGO.tag = "Background";
                if (backgroundType == BackgroundType._3DModels)
                {
                    if (spriteRenderer)
                    {
                        if (_cam3DtoSpriteMaterial)
                        {
                            spriteRenderer.sharedMaterial = _cam3DtoSpriteMaterial;
                        }
                        else
                        {
                            cam3DtoSpriteMaterial = spriteRenderer.sharedMaterial;
                        }
                    }
                }

                break;
        }

        spriteGO.layer = Layers.ground;
    }

    public void InitialiseInEditor()
    {
        name = SceneManager.GetActiveScene().name + " " + gameObject.name;
        d2dSprite = GetComponent<TS_DynamicSprite>();
        d2dPoly = GetComponent<TS_PolygonSpriteCollider>();
        if (d2dSprite)
            spriteGO = d2dSprite.gameObject; // esto antes tenia sentido , el go no siempre era el de este mismo script, los terrain estaban en el go del mapa y los d2d estaban en los go de sprites
        else
            spriteGO = gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        lightObstacleGenerator = GetComponent<LightObstacleGenerator>();
        if (lightObstacleGenerator && WorldMap.instance.setLightObstacleColors) 
        {
            lightObstacleGenerator.MultiplicativeColor = WorldMap.instance.multShadowColor;
            lightObstacleGenerator.AdditiveColor = WorldMap.instance.addShadowColor;
        }
        RestoreFromDataBaseOrGetIndex();
        FillSpriteTexture(true);
        rect = new Rect(spriteRenderer.bounds.min.x, spriteRenderer.bounds.min.y, spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y);
        if (backgroundType == BackgroundType._3DModels)
        {
            FillSpriteTextureWithCompressedData();
            //GetBackground3DModelsFromDiskOrCreateIt();
            //_3dModelsToSpriteList.FindAll(); // todo esta lista creo que no la necesito mas
        }

        if (tile && !tileMap && !cam3DtoSprite)
            backgroundType = BackgroundType.SingleTile;
        else if (!tile && tileMap && !cam3DtoSprite)
            backgroundType = BackgroundType.TileMap;
        else if (!tile && !tileMap && cam3DtoSprite)
            backgroundType = BackgroundType._3DModels;
        else
            Debug.LogError("LEVEL " + KuchoHelper.GetSceneName() + " " + this + " NO SE EN QUE MODO BACKGROUND ESTOY");
        if (!tile && !tileMap && cam3DtoSprite && spriteRenderer)
            spriteRenderer.sharedMaterial = cam3DtoSpriteMaterial;
    }

    public void UpdateSmoothAndPolishThresholds()
    {
        UpdateSmoothThreshold(0f);
        UpdatePolishThreshold();
    }

    public void UpdateSmoothThreshold(float pixelsAround)
    {
        // pixelsAround es calculado segun la altura, no vale un valor para toda la tabla, si smoothHeightFactor esta a , heightFactor vendra siempre a cero
        float squareSide = (pixelsAround * 2f) + 1f;
        int maxValue = (int) (squareSide * squareSide * 255);
        smoothThreshold = maxValue * smoothSquareRatio;
    }

    public void UpdatePolishThreshold()
    {
        int squareSide = (polishRange * 2) + 1;
        int maxValue = (int) (squareSide * squareSide * 255);
        polishThreshold = maxValue * smoothSquareRatio;
    }

    public void FillSpriteTexture(bool force)
    {
        // usar los estaticos puede dar errroe en editor por que no estan actualizados 
        int w = WorldMap.instance._width;
        int h = WorldMap.instance._height;
        // al parecer no puedo hacer esta comprobacion de seguridad porque la textura aun es null?
        /*
        if (w != texture.width || h != texture.height)
        {
            Debug.LogError(this + " ALGO HA IDO MAL TAMAÑO DE MUNDO (" + w + "," + h + ") DISTINTO A TAMAÑO DE TEXTURA (" + texture.width + "," + texture.height + ")");
            Debug.Break();
            return;
        }
        */
        FillSpriteTexture(w, h, force);
    }

    public void FillSpriteTexture(int width, int height, bool force)
    {
        // esto evita que se ejecute cuando paro, cuando updateo el script, solo se ejecuta cuando esoty en PLAY REAL o he dado a SAVE (ya que save lo destruye antes de salvar para que en dico ocupe poco)
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode && !Application.isPlaying) // editor esta realmente parado
        {
            if (!force) // solo se activa cuando hemos dado a save
                return; // asi gano agilidad en editor cuando cargo nivel paro un juego, sino esto se vuelve a ejecutar
        }
#endif

        if (tile && tileMap)
        {
            Debug.LogError("LEVEL " + KuchoHelper.GetSceneName() + " " + this + groundType.ToString() + " TENGO TILE Y TILE MAP?");
        }
        else
        {
            if (tile)
                FillSpriteTextureWithTile(width, height);
            else if (tileMap)
                FillSpriteTextureWithTileMap(width, height);
            else if (cam3DtoSprite)
            {
                if (cam3dtospriteCompressedTexData == null || cam3dtospriteCompressedTexData.Count == 0)
                {
                    if (cam3dtospriteCompressedTexData == null)
                        print("LEVEL " + KuchoHelper.GetSceneName() + " " + this + "  COMPRESSED DATA ES NULL");
                    else
                        print("LEVEL " + KuchoHelper.GetSceneName() + " " + this + "  COMPRESSED DATA ESTA VACIO");

                    //FillSpriteTextureWith3DObjects(); 
                }
                else
                    FillSpriteTextureWithCompressedData();
            }
        }
    }

    #region SINGLE TILE MODE (DESTRUCTIBLE & UNDESTRCTIBLE background antiguo) ---------------------------------------------------------------------------

    public void SetSingleTileMode()
    {
        changeClickCount--;
        if (changeClickCount > 0)
        {
            print("FALTAN " + changeClickCount + " PARA EFECTUAR EL CAMBIO");
            return;
        }

        DestroyTileMap();
        DestroyCamera();
        backgroundType = BackgroundType.SingleTile;
    }

    public void FillSpriteTextureWithTile()
    {
        FillSpriteTextureWithTile(WorldMap.width, WorldMap.height);
    }

    public void FillSpriteTextureWithTile(int width, int height)
    {
        if (width > 0 && height > 0)
        {
            if (spriteRenderer)
            {
                spriteRenderer.sharedMaterial = tileMaterial;

                Texture2D spriteTex = TextureHelper.ReuseOrCreateNewSpriteText(spriteRenderer, width, height, TextureFormat.ARGB32);

                for (int y = 0; y < spriteTex.height; y += tile.height)
                {
                    for (int x = 0; x < spriteTex.width; x += tile.width)
                    {
                        Point range = new Point(tile.width, tile.height);
                        if (x + tile.width > spriteTex.width) // el mapa no es multiplo de la textura, contemplo unicamente el caso de que sea el doble
                            range.x = spriteTex.width - x;

                        if (y + tile.height > spriteTex.height)
                            range.y = spriteTex.height - y;

                        var tilePixels = tile.GetPixels(0, 0, range.x, range.y); //TODO estopodria estar fuera del bucle y solo volverlo a hacer si ha cambiado range
                        spriteTex.SetPixels(x, y, range.x, range.y, tilePixels, 0); // SetPixels32 es mas rapido pero no funciona, en el manual de unity dice que si , sera que es algo nuevo de unity5?
                    }
                }

                spriteTex.filterMode = FilterMode.Point;
                spriteTex.Apply();
                spriteTex.name = SceneManager.GetActiveScene().name;
                var newSpr = Sprite.Create(spriteTex, new Rect(0, 0, spriteTex.width, spriteTex.height), Constants.zero2, 1);
                newSpr.name = "SingleTileSprite";
                spriteRenderer.sprite = newSpr;
                texture = spriteRenderer.sprite.texture;
            }
            else
            {
                Debug.LogError("LEVEL " + KuchoHelper.GetSceneName() + " " + this + "TERRAIN2D - EL GO " + gameObject.name + " NO TIENE SPRITE RENDERER?");
            }
        }
        else
        {
            Debug.LogError("LEVEL " + KuchoHelper.GetSceneName() + " " + this + " FILL TEX WITH TILE ERROR , WIDTH(" + width + ") o HEIGHT(" + height + ") SON CERO");
        }
    }

    #endregion

    #region TILE MAP MODE -------------------------------------------------------------------------------------

    public void SetTileMapMode()
    {
        if (tileMap)
            return;
        changeClickCount--;
        if (changeClickCount > 0)
        {
            print("FALTAN " + changeClickCount + " PARA EFECTUAR EL CAMBIO");
            return;
        }

        DestroyCamera();
        changeClickCount = changeClickNeed;

        tile = null;
        //        d2dSprite.CleanAlphaData();
        //        DestroyImmediate(d2dSprite.alphaTex);
        //        d2dSprite.alphaData = null;
        //        d2dSprite.enabled = false;
        DestroyImmediate(d2dSprite);
        if (!tileMapMaterial && tileMaterial)
            tileMapMaterial = tileMaterial;
        if (spriteRenderer)
            spriteRenderer.sharedMaterial = tileMapMaterial;
        if (!tileMap)
        {
            Grid grid = spriteGO.GetComponent<Grid>();
            if (!grid)
                grid = spriteGO.AddComponent<Grid>();
            grid.cellSize = new Vector3(128, 128, 0);
            var tileMapGO = Instantiate(new GameObject(), transform);
            tileMapGO.transform.localPosition = new Vector3(0, 0, -0.1f);
            tileMapGO.name = groundType.ToString() + "TileMap";
            tileMap = tileMapGO.AddComponent<Tilemap>();
            tileMapRenderer = tileMapGO.AddComponent<TilemapRenderer>();
        }

        backgroundType = BackgroundType.TileMap;
    }

    public void FillSpriteTextureWithTileMap()
    {
        FillSpriteTextureWithTileMap(WorldMap.width, WorldMap.height);
    }

    public void FillSpriteTextureWithTileMap(int width, int height)
    {
        if (width > 0 && height > 0)
        {
            spriteRenderer.sharedMaterial = tileMapMaterial;

            var spriteTex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            if (tileMap.layoutGrid)
            {
                var cellSize = tileMap.layoutGrid.cellSize;
                int tileHeight = (int) cellSize.x;
                int tileWidth = (int) cellSize.y;
                for (int y = 0; y < spriteTex.height; y += tileHeight)
                {
                    for (int x = 0; x < spriteTex.width; x += tileWidth)
                    {
                        Vector2Int range = new Vector2Int(tileWidth, tileHeight);
                        if (x + tileWidth > spriteTex.width) // el mapa no es multiplo de la textura, contemplo unicamente el caso de que sea el doble
                            range.x = spriteTex.width - x;

                        if (y + tileHeight > spriteTex.height)
                            range.y = spriteTex.height - y;
                        Vector3Int cellPos = new Vector3Int(x / tileWidth, y / tileHeight, 0);
                        Sprite tileSprite = tileMap.GetSprite(cellPos);
                        Color[] tilePixels = null;
                        Texture2D tileSprTex = null;

                        if (tileSprite)
                        {
                            tileSprTex = tileSprite.texture;
                            if (tileSprTex)
                            {
                                if (TextureHelper.IsReadable(tileSprTex))
                                {
                                    Vector2Int sprTexPos = new Vector2Int((int) (tileSprite.textureRect.xMin - tileSprite.textureRectOffset.x), (int) (tileSprite.textureRect.yMin - tileSprite.textureRectOffset.y));
                                    Vector2Int sprPosMax = sprTexPos + range;

                                    if (sprPosMax.x > tileSprTex.width)
                                    {
                                        Debug.Log("ME SALGO DE ANCHO EN " + tileSprTex.name + "(" + tileSprTex.width + ") AL LEER PIXELS EN " + sprTexPos.x + " RANGO=" + range.x + " HASTA=" + sprPosMax.x);
                                    }
                                    else if (sprPosMax.y > tileSprTex.height)
                                    {
                                        Debug.Log("ME SALGO DE ALTO EN " + tileSprTex.name + "(" + tileSprTex.height + ") AL LEER PIXELS EN " + sprTexPos.y + " RANGO=" + range.y + " HASTA=" + sprPosMax.y);
                                    }
                                    else
                                        tilePixels = tileSprTex.GetPixels(sprTexPos.x, sprTexPos.y, range.x, range.y);
                                }
                                else
                                {
                                    Debug.Log("TEXTURA DE TILES " + tileSprTex.name + " NO ES LEGIBLE, ACTIVALA EN IMPORT SETTINGS");
                                    break;
                                }
                            }
                            else // hay tileSprite pero no textura ...?
                            {
                                Debug.Log("ALGUN SPRITE DEL TILEMAP " + tileMap.transform.name + " NO TIENE TEXTURA");
                                tilePixels = null;
                            }
                        }

                        if (!tileSprite || !tileSprTex) // no hay tileSprite o no hay tileSpriteTex
                        {
                            tilePixels = new Color[range.x * range.y];
                            Color emptyPix = Constants.transparentBlack;
                            for (int i = 0; i < tilePixels.Length; i++)
                                tilePixels[i] = emptyPix;
                        }

                        spriteTex.SetPixels(x, y, range.x, range.y, tilePixels, 0); // SetPixels32 es mas rapido pero no funciona, en el manual de unity dice que si , sera que es algo nuevo de unity5?

                    }
                }

                spriteTex.filterMode = FilterMode.Point;
                spriteTex.Apply();
                spriteTex.name = SceneManager.GetActiveScene().name;
                var newSpr = Sprite.Create(spriteTex, new Rect(0, 0, spriteTex.width, spriteTex.height), Constants.zero2, 1);
                newSpr.name = "TileMapSprite";
                spriteRenderer.sprite = newSpr;
                texture = spriteRenderer.sprite.texture;
                if (Application.isPlaying)
                    tileMap.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("LEVEL " + KuchoHelper.GetSceneName() + " " + this + " EL TILEMAP " + tileMap.name + " DE " + groundType + " NO TIENE GRID");
            }
        }
        else
        {
            Debug.LogError("LEVEL " + KuchoHelper.GetSceneName() + " " + this + " FILL TEX WITH TILE MAP ERROR , WIDTH(" + width + ") o HEIGHT(" + height + ") SON CERO");
        }

    }

    #endregion

    #region 3D MODELS TO SPRITE ------------------------------------------------------------
    public List<KuchoGrayScaleTextureCompressionData> cam3dtospriteCompressedTexData;
    public void FillSpriteTextureWithCompressedData()
    {
        //_3dModelsToSpriteList.SwitchAll(false); 
        if (cam3dtospriteCompressedTexData != null && cam3dtospriteCompressedTexData.Count > 0)
        {
            texture = TextureHelper.ReuseOrCreateNewSpriteText(spriteRenderer, WorldMap.width, WorldMap.height, TextureFormat.Alpha8);
            byte[] texData = texture.GetRawTextureData();
            int i = 0;
            foreach (KuchoGrayScaleTextureCompressionData cd in cam3dtospriteCompressedTexData)
            {
                cd.FillTex(ref i, texData);
            }

            texture.LoadRawTextureData(texData);
            texture.Apply();
            if (spriteRenderer.sprite == null || spriteRenderer.sprite.texture == null)
            {
                texture.filterMode = FilterMode.Point;
                var newSpr = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Constants.zero2, 1);
                //newSpr.name = "UncompressedFromGrayscaleKuchoFormat";
                spriteRenderer.sprite = newSpr;
                texture = spriteRenderer.sprite.texture; // innecesario creo
            }
            else
            {
                spriteRenderer.sprite.name = "UncompressedFromGrayscaleKuchoFormat";
            }
        }
    }
#if UNITY_EDITOR
    public void Set3dObjectsToSpriteMode()
    {
        float worldMapUnit = 128;
        float worldMapHlfUnit = worldMapUnit / 2;
        Vector2 worldMapUnit2 = new Vector2(worldMapUnit, worldMapUnit);
        DestroyTileMap();
        DestroyCamera();
        if (!cam3DtoSprite)
        {
            // crea camara
            GameObject cam3dGo = Instantiate(new GameObject(), transform);
            cam3dGo.transform.localPosition = new Vector3(0, 0, -0.1f);
            cam3dGo.name = groundType.ToString() + "Cam3dToSprite";

            cam3DtoSprite = cam3dGo.AddComponent<Camera>();
            cam3DtoSprite.orthographic = true;

            cam3DtoSprite.cullingMask = Masks.GetLayerMaskFromEnum(MaskType.Default);
            cam3DtoSprite.transform.position = new Vector3(worldMapHlfUnit, worldMapHlfUnit, WorldMap.spritePlanes.lightCoverPlane);

            cam3DtoSprite.enabled = false; // la voy a llamar manualmente, es mejor asi
            // crea Luces
            CreateDirectionalLight(cam3DtoSprite.transform, new Vector3(55, 0, 0), 1, LightShadows.Soft, 0.5f);
            CreateDirectionalLight(cam3DtoSprite.transform, new Vector3(0, 5, 0), 0.5f, LightShadows.Soft, 0.5f);
            CreateDirectionalLight(cam3DtoSprite.transform, new Vector3(0, -5, 0), 0.5f, LightShadows.Soft, 0.5f);

            if (spriteRenderer)
                spriteRenderer.sharedMaterial = cam3DtoSpriteMaterial;
        }

        backgroundType = BackgroundType._3DModels;
    }

    void CreateDirectionalLight(Transform parent, Vector3 rotation, float intensity, LightShadows shadows, float shadowStrength)
    {
        GameObject lightGO = Instantiate(new GameObject(), parent);
        lightGO.name = groundType.ToString() + "3D Light";
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.lightmapBakeType = LightmapBakeType.Realtime;
        light.intensity = intensity;
        light.shadows = shadows;
        light.shadowStrength = shadowStrength;
        light.shadowResolution = UnityEngine.Rendering.LightShadowResolution.Low;
        light.cullingMask = cam3DtoSprite.cullingMask;
        light.transform.eulerAngles = rotation;
    }

    //public Background3DModelList _3dModelsToSpriteList = new Background3DModelList();
    public void FillSpriteTextureWith3DObjects()
    {
        StartCoroutine(FillSpriteTextureWith3DObjectsCoroutine()); // no puedo usar MEC por que al usarlo desde editor me crear un gameobject timing controller que luego si me despisto podria grabarse en la escena y darme errores al ejecutar
    }

    Texture2D tempTex = null;

    RenderTexture renderTexture;

    public static bool saving3DModelsAsset;

//    public TextureFormat compressionFormat = TextureFormat.DXT5;
//    public TextureCompressionQuality compressionQuality = TextureCompressionQuality.Best;
    IEnumerator FillSpriteTextureWith3DObjectsCoroutine()
    {
        if (cam3DtoSpriteMaterial == null)
            Initialise(false);
        if (cam3DtoSpriteMaterial != null)
        {
            spriteRenderer.sharedMaterial = cam3DtoSpriteMaterial;
            Vector3 camPos = Vector3.zero;

            if (!Application.isPlaying) // comprobaciones por que podria haber cosas no inicializadas
            {
                if (!WorldMap.instance)
                {
                    Debug.Log("WOOPS ! WORLD MAP NO INICIALIZADO");
                    WorldMap.instance = FindObjectOfType<WorldMap>();
                }

                if (!WorldMap.instance._spritePlanes || WorldMap.width == 0 || WorldMap.height == 0)
                    WorldMap.instance.PublicOnValidate();

            }

            if (WorldMap.background == this) // soy background
                camPos.z = WorldMap.instance._spritePlanes.foreground.max;
            else
                camPos.z = WorldMap.instance._spritePlanes.lightCoverPlane;

            int width = WorldMap.width;
            int height = WorldMap.height;

            // crea sprite y asigna textura aunque aun no este rellena para que no de error otros programas
            tempTex = TextureHelper.ReuseOrCreateNewSpriteText(spriteRenderer, width, height, TextureFormat.ARGB32);
            tempTex.filterMode = FilterMode.Point;
            tempTex.name = SceneManager.GetActiveScene().name;
            var newSpr = Sprite.Create(tempTex, new Rect(0, 0, tempTex.width, tempTex.height), Constants.zero2, 1);
            newSpr.name = "3DCamToSprites";
            spriteRenderer.sprite = newSpr;
            texture = spriteRenderer.sprite.texture;
            if (texture == null)
                Debug.LogError("LEVEL " + KuchoHelper.GetSceneName() + " " + this + "ACABO DE ASIGNARME UNA TEXTURA NULL");
            bool game3DLightsChanged = false;
            bool game3DLightsEnabled = false;
            if (Game.G && Game.G._3D_LightsParent)
            {
                game3DLightsChanged = true;
                game3DLightsEnabled = Game.G._3D_LightsParent.activeSelf;
                Game.G._3D_LightsParent.SetActive(false);
            }

            DisableSpriteAndEnable3DModels();
            if (Application.isPlaying)
                yield return null; // si no espero un frame a veces sale rojo todo WTF?

            cam3DtoSprite.enabled = false;
            cam3DtoSprite.gameObject.SetActive(true);
            RenderTexture activeT = RenderTexture.active; // backup
            Rect readRect;
            //transforma 3d obj3cts to sprite

            if (1 == 1)
            {
                #region trozo a trozo

                float worldMapUnit = 128; // TODO ojo podria no encajar, normalmente uso 128 en destructible.tile.width/height, pero podria cambiar !!
                cam3DtoSprite.orthographicSize = worldMapUnit / 2;
                cam3DtoSprite.pixelRect = new Rect(0, 0, (int) worldMapUnit, (int) worldMapUnit);
                cam3DtoSprite.rect = new Rect(0, 0, 1, 1);
                if (renderTexture == null || renderTexture.width != worldMapUnit || renderTexture.height != worldMapUnit || renderTexture.format != RenderTextureFormat.ARGB32)
                    renderTexture = new RenderTexture((int) worldMapUnit, (int) worldMapUnit, 1, RenderTextureFormat.ARGB32);
                cam3DtoSprite.targetTexture = renderTexture;
                RenderTexture.active = cam3DtoSprite.targetTexture; // fijo mi renderTex

                float worldMapHalfUnit = worldMapUnit / 2;
                Vector2 worldMapUnit2 = new Vector2(worldMapUnit, worldMapUnit);
                int xEnd = WorldMap.instance.wMultiplier;
                int yEnd = WorldMap.instance.hMultiplier;

                Vector3 pos = Vector3.zero;
                camPos.z = 200; // los Background3DModel empiezan en z 300 (el centro de la roca/mdelo ojo)
                readRect = new Rect(Vector2.zero, worldMapUnit2);
                for (int xu = 0; xu < xEnd; xu++)
                {
                    pos.x = xu * worldMapUnit;
                    for (int yu = 0; yu < yEnd; yu++)
                    {
                        pos.y = yu * worldMapUnit;
                        camPos.x = pos.x + worldMapHalfUnit;
                        camPos.y = pos.y + worldMapHalfUnit;
                        cam3DtoSprite.transform.position = camPos;
                        cam3DtoSprite.Render();
                        tempTex.ReadPixels(readRect, (int) pos.x, (int) pos.y); // leo pixeles a la spreite tex
                    }
                }

                #endregion
            }

            if (1 == 2)
            {
                #region todo el mapa

                cam3DtoSprite.orthographicSize = WorldMap.height / 2;
                cam3DtoSprite.pixelRect = new Rect(0, 0, WorldMap.width, WorldMap.height);
                cam3DtoSprite.rect = new Rect(0, 0, 1, 1);
                if (renderTexture == null || renderTexture.width != WorldMap.width || renderTexture.height != WorldMap.height)
                    renderTexture = new RenderTexture(WorldMap.width, WorldMap.height, 1, RenderTextureFormat.ARGB32); // todo el mapa !
                cam3DtoSprite.targetTexture = renderTexture;
                RenderTexture.active = cam3DtoSprite.targetTexture; // fijo mi renderTex
                camPos.x = width / 2;
                camPos.y = height / 2;
                cam3DtoSprite.transform.position = camPos;
                cam3DtoSprite.Render();
                readRect = new Rect(Vector2.zero, WorldMap.size);
                tempTex.ReadPixels(readRect, 0, 0); // leo pixeles a la spreite tex

                #endregion
            }

            RenderTexture.active = activeT; // restauro


            //TODO si funcioona hacer que polish lo haga con pix, y especial para este tipo de textura, asi sera mas rapido que traerse los pixeles de nuevo de la textura
            Texture2D spriteTexAlpha8 = TextureHelper.ReuseOrCreateNewSpriteText(spriteRenderer, tempTex.width, tempTex.height, TextureFormat.Alpha8);
            spriteTexAlpha8.name = "BackgroundSpriteAlpha7+1";
            spriteTexAlpha8.filterMode = FilterMode.Point;
            var alpha8data = spriteTexAlpha8.GetRawTextureData(); // en unity 2018 creo que esto no genera allocation pero en 2017 aun si
            var colorData = tempTex.GetRawTextureData(); // en unity 2018 creo que esto no genera allocation pero en 2017 aun si
            int end = tempTex.width * tempTex.height + 0;
            // codifica en 7+1
            for (int i = 0; i < end; i++)
            {
                int ii = i * 4;
                byte v = colorData[ii]; // leo alpha de la textura de color
                if (v < 255) // el pixel no es totalmente solido?
                {
                    alpha8data[i] = 0;
                }
                else
                {
                    int red = (int) colorData[ii + 1];
                    if (red < 255)
                    {
                        red++;
                    }

                    alpha8data[i] = (byte) red;
                }
            }

            TextureHelper.PolishAlpha8Data(alpha8data, spriteTexAlpha8.width, spriteTexAlpha8.height, polishRange, smoothSquareRatio);
            spriteTexAlpha8.LoadRawTextureData(alpha8data); // TODO en unity 2018 parece que esto y ano es necesario por que GetRawData devuelve puntero a los datos originales de la textura !
//            Color32[] pix = tempTex.GetPixels32(); // no puedo trarme los datos a Byte, no existe posibilidad creo, asi se desperdicia mucha memoria
//            Color32 transparent = new Color32(0, 0, 0, 0);
//            for (int i = 0; i < pix.Length; i++)
//            {
//                byte v = pix[i].a;
//                if (v == 0)
//                {
//                    pix[i] = transparent;
//                }
//                else
//                {
//                    v = 1;
//                    int red = (int)pix[i].r;
//                    if (red < 255)
//                    {
//                        pix[i].a = (byte)(red + (int)v);
//                    }
//                }
//            }
//            TextureHelper.Polish(pix, tempTex.width, tempTex.height, polishRange , smoothSquareRatio);

//            spriteTexAlpha8.SetPixels32(pix);

//            spriteTex.SetPixels32(pix);
            // NO FUNCIONA PARA ALPHA8 ??? me devuelve todo con alpha 0,804 WTF?
//            EditorUtility.CompressTexture(spriteTexAlpha8, compressionFormat , compressionQuality);
            spriteTexAlpha8.Apply();
            Color[] pixf = spriteTexAlpha8.GetPixels();
            newSpr = Sprite.Create(spriteTexAlpha8, new Rect(0, 0, spriteTexAlpha8.width, spriteTexAlpha8.height), Constants.zero2, 1);
            newSpr.name = "3DCamToSprites";
            spriteRenderer.sprite = newSpr;
            texture = spriteRenderer.sprite.texture;

            //_3dModelsToSpriteList.SwitchAll(false);
            GetBackground3DModelsFromDiskOrCreateIt();

            var mcs = background3DModelsParent.GetComponentsInChildren<MeshCollider>();
            foreach (MeshCollider mc in mcs)
            {
                DestroyImmediate(mc);
            }

            saving3DModelsAsset = true;// para que no salte mi OnSceneSave
            PrefabUtility.SaveAsPrefabAsset(background3DModelsParent.gameObject, background3DModelsPath);
            saving3DModelsAsset = false;
            Debug.Log("GUARDADO PREFAB DE 3D MODELS " + GetCleanLevelName());
            EditorUtility.SetDirty(gameObject);

            DestroyImmediate(background3DModelsParent.gameObject);

            spriteRenderer.enabled = true;
            cam3DtoSprite.gameObject.SetActive(false); // apago camara y luces que tuviera como hijas

            if (game3DLightsChanged)
            {
                Game.G._3D_LightsParent.SetActive(game3DLightsEnabled);
            }

            //CompressGrayscaleKuchoFormatTexture(); // no es necesario hacerlo ahora, solo cuando se guarde a disco el nivel 
        }
        else
            Debug.LogError("LEVEL " + KuchoHelper.GetSceneName() + " " + this + " NO TENGO CAM 3D TO SPRITES MATERIAL");
        
    }
    public void CompressGrayscaleKuchoFormatTexture()
    {
        if (texture != null)
        {
            cam3dtospriteCompressedTexData = TextureHelper.CompressKuchoGrayScaleTexture(texture);
        }
        else
        {
            Debug.Log(KuchoHelper.GetSceneName() + this + " NO PUEDO COMPRIMIR TEXTURA, ES NULL");
        }

        if (!Application.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        else
        {
            Debug.Log(" COMPRIMIENDO TEXTURA A FORMATO KUCHO EN MODO PLAY, SE HA PERDIDO LA INFO COMPRIMIDA?, RECOMPRIME EN EDITOR Y VUELVE A GUARDAR EL NIVEL");
        }
    }

    public void DisableSpriteAndEnable3DModels()
    {
        spriteRenderer.enabled = false;
        if (!cam3DtoSprite)
            cam3DtoSprite = gameObject.GetComponentInChildren<Camera>(true);
        if (!cam3DtoSprite)
            Set3dObjectsToSpriteMode();
        cam3DtoSprite.gameObject.SetActive(true); // enciendo camara mas que nada para las luces
        /*
        if (_3dModelsToSpriteList == null) // puede venir null
        {
            _3dModelsToSpriteList = new Background3DModelList();
        }
        _3dModelsToSpriteList.SwitchAll(true);// enciendo todos los que tenia apagados en la lista
        _3dModelsToSpriteList.FindAll(); // reconstruye la lista , asi si he añadiddo algunos  en inspector se actualiza con todos los activos, asi evito tene ruqe incuirse en la lista cada on enable y evito que se cuelgue unity 
        */

        GetBackground3DModelsFromDiskOrCreateIt();
    }
    void GetBackground3DModelsFromDiskOrCreateIt()
    {
        if (background3DModelsParent)
            return;
        background3DModelsParent = GetComponentInChildren<Background3DModelParent>();
        if (background3DModelsParent)
            return;
        string levelName = GetCleanLevelName();

        {// MAKE PATH
            background3DModelsPath = "Assets" + Path.DirectorySeparatorChar + "-KUCHO";

            if (!Directory.Exists(background3DModelsPath))
            {
                Directory.CreateDirectory(background3DModelsPath);
            }

            background3DModelsPath += Path.DirectorySeparatorChar + "Background3DModelPrefabs";
            if (!Directory.Exists(background3DModelsPath))
            {
                Directory.CreateDirectory(background3DModelsPath);
            }
            
            if (levelName.Contains("USER"))
            {
                background3DModelsPath += "(UserLevels)";
            }
            if (!Directory.Exists(background3DModelsPath))
            {
                Directory.CreateDirectory(background3DModelsPath);
            }

            background3DModelsPath += Path.DirectorySeparatorChar + levelName + " - BACKGROUND 3D MODELS" + ".Prefab";
        }
        
        // lo busco en disco
        background3DModelsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(background3DModelsPath);
        if (background3DModelsPrefab) // existe 
        {
            background3DModelsParent = background3DModelsPrefab.GetComponent<Background3DModelParent>();
            if (background3DModelsParent == null)
                Debug.LogError("ALGO HA IDO MAL HE ENCONTRADO EL PREFAB DE 3DMODELS PERO NO TIENE SCRIPT");
            else
            {
                background3DModelsPrefabInstance = PrefabUtility.InstantiatePrefab(background3DModelsPrefab) as GameObject;
                background3DModelsPrefabInstance.name = "BACKGROUND 3D MODELS";
                background3DModelsPrefabInstance.transform.parent = transform;
                background3DModelsPrefabInstance.transform.localPosition = Constants.zero3;
                background3DModelsParent = background3DModelsPrefabInstance.GetComponent<Background3DModelParent>();
                Debug.Log("CARGADO PREFAB DE 3D MODELS " + GetCleanLevelName());
            }
        }
        else
        {
            // no existe, lo creo
            background3DModelsPrefab = new GameObject("BACKGROUND 3D MODELS");
            background3DModelsPrefab.transform.parent = transform;
            background3DModelsPrefab.transform.localPosition = Constants.zero3;
            background3DModelsParent = background3DModelsPrefab.AddComponent<Background3DModelParent>();
            var all = GetComponentsInChildren<Background3DModel>();
            foreach (Background3DModel m in all)
            {
                m.transform.parent = background3DModelsParent.transform; // para que se me hagan hijos del background3dmodelparent
            }

            background3DModelsPrefabInstance = PrefabUtility.SaveAsPrefabAssetAndConnect(background3DModelsPrefab, background3DModelsPath, InteractionMode.AutomatedAction);
            Debug.Log("CREADO PREFAB DE 3D MODELS " + GetCleanLevelName());
        }

        PrefabUtility.UnpackPrefabInstance(background3DModelsPrefabInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction); // para evitar que unity quiera seleccionar el padre cuando selecciono una roca
        EditorUtility.SetDirty(gameObject);
    }
#endif
#endregion

    string GetCleanLevelName()
    {
        string levelName = SceneManager.GetActiveScene().name.ToUpper();
        if (levelName.StartsWith("LEVEL "))
            return levelName.Substring(6, levelName.Length - 6);
        if (levelName.StartsWith("LEVEL"))
            return levelName.Substring(5, levelName.Length - 5);
        return levelName;
    }

    static bool _3dModelstoSpriteListChangeAllowed = true;

    void DestroyTileMap()
    {
        if (tileMap)
        {
            spriteRenderer.sprite = null;
            DestroyImmediate(tileMap.layoutGrid);
            DestroyImmediate(tileMap.gameObject);
        }
    }

    void DestroyCamera()
    {
        cam3DtoSprite = GetComponentInChildren<Camera>();
        if (cam3DtoSprite)
            DestroyImmediate(cam3DtoSprite.gameObject);
    }

    public void ResizeAlphaTexAndAlphaData(int width, int height)
    {
        if (d2dSprite)
        {
            Texture2D newAlphaText = new Texture2D(width, height, d2dSprite.AlphaTex.format, false); // creo newva alpha text con nuevo tamaño

            var fillArray = new Color[width * height]; // relleno la texture nueva con blank
            Color c = new Color(0, 0, 0, 0);
            for (int i = 1; i < fillArray.Length; i++)
                fillArray[i] = c;
            newAlphaText.SetPixels(fillArray);

            int blockWidth = d2dSprite.AlphaTex.width; // como de grande va a ser el bloque a copiar
            if (width < d2dSprite.AlphaTex.width)
                blockWidth = width;
            int blockHeight = d2dSprite.AlphaTex.height;
            if (height < d2dSprite.AlphaTex.height)
                blockHeight = height;

            Color[] pixels = d2dSprite.AlphaTex.GetPixels(0, 0, blockWidth, blockHeight); // pillo los pixels de la tex antigua
            newAlphaText.SetPixels(0, 0, blockWidth, blockHeight, pixels); // relleno la nueva con los pix que he sacado de la antigua

            d2dSprite.ReplaceAlphaWith(newAlphaText); // intercambio
            FillSpriteTextureWithTile(width, height);
        }
    }
    // ESTAS TRES TUTINAS PUEDEN SER UNIFICADAS EN UNA QUE SEA
    // CopyAndResizeAlphaData(byte[] src, int srcWidth, int src height, byte[] dest, int destWidth, int destWidth)

    public void ScaleSmallAlphaData(int currentWidth, int currentHeight, int newWidth, int newHeight)
    {
        AlphaTexture.Load(smallAlphaData, currentWidth, currentHeight);
        AlphaTexture.Resize(newWidth, newHeight);
        smallAlphaData = new byte[newWidth * newHeight];
        AlphaTexture.CopyTo(smallAlphaData); // copia uno a uno los datos, no conviene referenciar los datos de la clase estatica
        Binarize(smallAlphaData);
    }

    public void ScaleAlphaDataWithSmallAlphaData(int smallWidth, int smallHeight, int newWidth, int newHeight) // mult siempre ha de ser el WorldMap.smallToRealRatio
    {
        AlphaTexture.Load(smallAlphaData, smallWidth, smallHeight);
        if (WorldMap.instance.scaleIsBilinear)
            AlphaTexture.Resize(newWidth, newHeight);
        else
            AlphaTexture.ResizeNoFilter(newWidth, newHeight);
        Binarize(AlphaTexture.Data);
        d2dSprite.UpdateAlphaWith(AlphaTexture.Data, AlphaTexture.Width, AlphaTexture.Height);
    }

    public void GenerateSmallAlphaDataWithAlphaData(int width, int height, int smallWidth, int smallHeight)
    {
        AlphaTexture.Load(d2dSprite.AlphaData, width, height);
        AlphaTexture.Resize(smallWidth, smallHeight);
        Binarize(AlphaTexture.Data);
        smallAlphaData = new byte[smallWidth * smallHeight];
        AlphaTexture.CopyTo(smallAlphaData); // copia uno a uno los datos, no conviene referenciar los datos de la clase estatica

    }
    public void BackupAlphaData()
    {
        if (undoAlphaData == null || undoAlphaData.Length != d2dSprite.alphaData.Length)
            undoAlphaData = new byte[d2dSprite.alphaData.Length];

        WorldMap.instance.CopyAlphaData(d2dSprite.alphaData, undoAlphaData);
    }
    void CleanTexture(Texture2D t)
    {
        Color[] pixels = new Color[t.width * t.height];
        int length = pixels.Length;
        for (int i = 0; i < length; i++)
            pixels[i] = Constants.transparentBlack;
        t.SetPixels(pixels);
    }

    public void Binarize(byte[] data)
    {
        int solidPixelCount = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] >= binarizeThreshold)
            {
                data[i] = 255;
                solidPixelCount++;
            }
            else
                data[i] = 0;
        }

        print(this + " ALPHA DATA BINARIZADO CON THRESHOLD (" + binarizeThreshold + ") TOTAL PIXELES SOLIDOS=" + solidPixelCount);
    }

    public Color GetPixel(int x, int y)
    {
        return texture.GetPixel(x, y);
    }

    public Color GetPixelWithShader(int x, int y)
    {
        Color c = texture.GetPixel(x, y);
        if (tileMaterial)
        {
            var hue = tileMaterial.GetFloat(ShaderProp._Hue);
            var sat = tileMaterial.GetFloat(ShaderProp._Sat);
            var val = tileMaterial.GetFloat(ShaderProp._Val);
            var cnt = tileMaterial.GetFloat(ShaderProp._Cont);
            var tint = tileMaterial.GetColor(ShaderProp._Color);
            c = ColorHelper.ShiftHSV_Contrast_Fast(c, hue, sat, val, cnt);
            c *= tint;
        }

        return c;
    }

    byte[] aDataRegion = new byte[1];

    public void ResetADataRegion()
    {
        aDataRegion = new byte[0];
    }

    public byte[] GetAlphaDataRegion(int x, int y, int w, int h)
    {
        int size = w * h;
        if (aDataRegion.Length < size)
            aDataRegion = new byte[size];
        int writeIndex = 0;
        int end = y + h;
        byte[] aData = d2dSprite.alphaData; // alphaTex.GetRawTextureData();
        for (int yy = y; yy < end; yy++)
        {
            int readIndex = x + yy * texture.width;
            int endWrite = writeIndex + w;
            for (writeIndex = writeIndex; writeIndex < endWrite; writeIndex++)
            {
                aDataRegion[writeIndex] = aData[readIndex];
                readIndex++;
            }
        }

        return aDataRegion;
    }

    public void SetAlphaDataAndRawDataRegion(int x, int y, int w, int h, byte[] bytes)
    {
        // bytes puede ser mayor, sobrarle datos, se usara solo lo que de w * h
        var rData = d2dSprite.alphaTex.GetRawTextureData<byte>(); // TODO NECESITO UNITY 2018 para esta funcion, que es la que me da la referencia, no hace copia
        byte[] aData = d2dSprite.alphaData;

        int size = w * h;
        int mapWidth = texture.width;
        int index = (y * mapWidth) + x;
        int j = 0;
        for (int i = 0; i < size; i++)
        {
            aData[index] = bytes[i];
            rData[index] = bytes[i];
            index++;
            j++;
            if (j >= w) // salto
            {
                j = 0;
                index += mapWidth - w; // subo una coordenada y y retraso x al principio
            }
        }
    }

    public void CopyRandomSettingsTo(Terrain2D to)
    {
        to.otherTerrainFactor = otherTerrainFactor;
        to.fillMid = fillMid;
        to.randomRange = randomRange;
        to.fillMin = fillMin;
        to.fillMax = fillMax;
        to.heightCleanFactor = heightCleanFactor;
        to.smoothHeightFactor = heightCleanFactor;
        to.smoothPasses = smoothPasses;
        to.smoothRangePassMultipler = smoothRangePassMultipler;
        to.smoothHeightFactor = smoothHeightFactor;
        to.smoothRange = smoothRange;
        to.polishRange = polishRange;
        to.sideBorderSmoothRatio = sideBorderSmoothRatio;
        to.bottomBorderSmoothRatio = bottomBorderSmoothRatio;
        to.topBorderSmoothRatio = topBorderSmoothRatio;

        to.binarizeThreshold = binarizeThreshold;
        to.wallThresholdSize = wallThresholdSize;
        to.roomThresholdSize = roomThresholdSize;
    }

    public void AddMaterialToTerrainDataBase()
    {
        //Game.MakeSureGameInstanceIsNotNull();
        MaterialDataBase.instance.AddMaterialIfNotInTheList(this);
    }
    public void AddToTerrainGenerationDataToDatabase()
    {
        MaterialDataBase.instance.AddTerrainGenerationPresetToList(this);
    }

    public bool IsEqual(Terrain2D t)
    {
        if (!t)
        {
            Debug.Log("INTENTO DE COMPARAR TERRENOS PERO ME LLEGA NULL");
            return false;
        }

        if (groundType != t.groundType) return false;
        if (groundType == GroundType.Background)
        {
            if (backgroundType == BackgroundType.TileMap)
            {
                if (tileMapMaterial != t.tileMapMaterial) return false;
            }
            else if (backgroundType == BackgroundType._3DModels)
            {
                if (cam3DtoSpriteMaterial != t.cam3DtoSpriteMaterial) return false;
            }
        }
        else
        {
            if (backgroundType != t.backgroundType) return false;
            if (texture != t.texture) return false;
            if (tile != t.tile) return false;
            if (tileMaterial != t.tileMaterial) return false;
            if (otherTerrainReference != t.otherTerrainReference) return false;
            if (otherTerrainFactor != t.otherTerrainFactor) return false;
            if (otherTerrainOffsetY != t.otherTerrainOffsetY) return false;
            if (fillMid != t.fillMid) return false;
            if (randomRange != t.randomRange) return false;
            if (heightCleanFactor != t.heightCleanFactor) return false;
            if (smoothRange != t.smoothRange) return false;
            if (smoothPasses != t.smoothPasses) return false;
            if (smoothRangePassMultipler != t.smoothRangePassMultipler) return false;
            if (smoothHeightFactor != t.smoothHeightFactor) return false;
            if (sideBorderSmoothRatio != t.sideBorderSmoothRatio) return false;
            if (bottomBorderSmoothRatio != t.bottomBorderSmoothRatio) return false;
            if (topBorderSmoothRatio != t.topBorderSmoothRatio) return false;
            if (smoothSquareRatio != t.smoothSquareRatio) return false;
            if (polishRange != t.polishRange) return false;
            if (horizontalTearLumaThreshold != t.horizontalTearLumaThreshold) return false;
            if (horizontalTearLumaThresholdDec != t.horizontalTearLumaThresholdDec) return false;
            if (horizontalTearLumaDeep != t.horizontalTearLumaDeep) return false;
            if (verticalTearLumaThreshold != t.verticalTearLumaThreshold) return false;
            if (verticalTearLumaThresholdDec != t.verticalTearLumaThresholdDec) return false;
            if (verticalTearLumaDeep != t.verticalTearLumaDeep) return false;
            if (upwardsTearLumaThreshold != t.upwardsTearLumaThreshold) return false;
            if (upwardsTearLumaThresholdDec != t.upwardsTearLumaThresholdDec) return false;
            if (upwardsTearLumaDeep != t.upwardsTearLumaDeep) return false;

            if (inGameHorizontalLumaDeep != t.inGameHorizontalLumaDeep) return false;
            if (inGameVerticalLumaDeep != t.inGameVerticalLumaDeep) return false;
        }

        return true;
    }
    public string generationPresetName = "";
    public int indexInTerrainDataBase;
    public void CopyFrom(Terrain2D t)
    {
        if (!t)
        {
            Debug.Log("INTENTO DE COPIAR TERRENOS PERO ME LLEGA NULL");
            return;
        }

        groundType = t.groundType;
        backgroundType = t.backgroundType;
        tileMapMaterial = t.tileMapMaterial;
        cam3DtoSpriteMaterial = t.cam3DtoSpriteMaterial;
        tile = t.tile;
        tileMaterial = t.tileMaterial;
        otherTerrainReference = t.otherTerrainReference;
        otherTerrainFactor = t.otherTerrainFactor;
        otherTerrainOffsetY = t.otherTerrainOffsetY;
        fillMid = t.fillMid;
        randomRange = t.randomRange;
        fillMin = t.fillMin;
        fillMax = t.fillMax;
        heightCleanFactor = t.heightCleanFactor;
        smoothRange = t.smoothRange;
        smoothPasses = t.smoothPasses;
        smoothRangePassMultipler = t.smoothRangePassMultipler;
        smoothHeightFactor = t.smoothHeightFactor;
        sideBorderSmoothRatio = t.sideBorderSmoothRatio;
        bottomBorderSmoothRatio = t.bottomBorderSmoothRatio;
        topBorderSmoothRatio = t.topBorderSmoothRatio;
        smoothSquareRatio = t.smoothSquareRatio;
        polishRange = t.polishRange;
        horizontalTearLumaThreshold = t.horizontalTearLumaThreshold;
        horizontalTearLumaThresholdDec = t.horizontalTearLumaThresholdDec;
        horizontalTearLumaDeep = t.horizontalTearLumaDeep;
        verticalTearLumaThreshold = t.verticalTearLumaThreshold;
        verticalTearLumaThresholdDec = t.verticalTearLumaThresholdDec;
        verticalTearLumaDeep = t.verticalTearLumaDeep;
        upwardsTearLumaThreshold = t.upwardsTearLumaThreshold;
        upwardsTearLumaThresholdDec = t.upwardsTearLumaThresholdDec;
        upwardsTearLumaDeep = t.upwardsTearLumaDeep;

        inGameHorizontalLumaDeep = t.inGameHorizontalLumaDeep;
        inGameVerticalLumaDeep = t.inGameVerticalLumaDeep;
        InitialiseInEditor();
        if (Application.isPlaying && groundType == GroundType.Background)
        {
            var terrainShadowInSprite = GetComponent<TerrainShadowInSprite>();
            if (terrainShadowInSprite)
            {
                terrainShadowInSprite.enabled = false;
                terrainShadowInSprite.enabled = true;
            }
        }
    }
    public void CopyFrom(MaterialDataBase.TerrainMatPreset tmi, int indexInTerrainDataBase)
    {
        if (tmi == null)
        {
            Debug.Log("INTENTO DE COPIAR TERRENOS PERO ME LLEGA NULL");
            return;
        }

        this.indexInTerrainDataBase = indexInTerrainDataBase;

        if (tmi != null)
        {
            groundType = tmi.groundType;
            backgroundType = tmi.backgroundType;
            tileMapMaterial = tmi.tileMapMaterial;
            cam3DtoSpriteMaterial = tmi.cam3DtoSpriteMaterial;
            tile = tmi.tile;
            tileMaterial = tmi.tileMaterial;
        }
        
        InitialiseInEditor();
        if (Application.isPlaying && groundType == GroundType.Background)
        {
            var terrainShadowInSprite = GetComponent<TerrainShadowInSprite>();
            if (terrainShadowInSprite)
            {
                terrainShadowInSprite.enabled = false;
                terrainShadowInSprite.enabled = true;
            }
        }
    }
    public void CopyFrom( MaterialDataBase.TerrainGenerationPreset t)
    {
        if (t == null )
        {
            Debug.Log("INTENTO DE COPIAR TERRENOS PERO ME LLEGA NULL");
            return;
        }
        
        if (t != null)
        {
            generationPresetName = t.name;
            otherTerrainReference = t.otherTerrainReference;
            otherTerrainFactor = t.otherTerrainFactor;
            otherTerrainOffsetY = t.otherTerrainOffsetY;
            fillMid = t.fillMid;
            randomRange = t.randomRange;
            fillMin = t.fillMin;
            fillMax = t.fillMax;
            heightCleanFactor = t.heightCleanFactor;
            smoothRange = t.smoothRange;
            smoothPasses = t.smoothPasses;
            smoothRangePassMultipler = t.smoothRangePassMultipler;
            smoothHeightFactor = t.smoothHeightFactor;
            sideBorderSmoothRatio = t.sideBorderSmoothRatio;
            bottomBorderSmoothRatio = t.bottomBorderSmoothRatio;
            topBorderSmoothRatio = t.topBorderSmoothRatio;
            smoothSquareRatio = t.smoothSquareRatio;
            polishRange = t.polishRange;
            horizontalTearLumaThreshold = t.horizontalTearLumaThreshold;
            horizontalTearLumaThresholdDec = t.horizontalTearLumaThresholdDec;
            horizontalTearLumaDeep = t.horizontalTearLumaDeep;
            verticalTearLumaThreshold = t.verticalTearLumaThreshold;
            verticalTearLumaThresholdDec = t.verticalTearLumaThresholdDec;
            verticalTearLumaDeep = t.verticalTearLumaDeep;
            upwardsTearLumaThreshold = t.upwardsTearLumaThreshold;
            upwardsTearLumaThresholdDec = t.upwardsTearLumaThresholdDec;
            upwardsTearLumaDeep = t.upwardsTearLumaDeep;

            inGameHorizontalLumaDeep = t.inGameHorizontalLumaDeep;
            inGameVerticalLumaDeep = t.inGameVerticalLumaDeep;
        }
    }

    public void RestoreFromDataBaseOrGetIndex()
    {
        if ((groundType == GroundType.Background && cam3DtoSpriteMaterial) || (groundType != GroundType.Background && tileMaterial))
        {
            //Game.MakeSureGameInstanceIsNotNull();
            indexInTerrainDataBase = MaterialDataBase.instance.GetIndexOf(this, groundType);
        }
        else
            RestoreTileMaterialFromTerrainDataBase();
    }

    public void RestoreTileMaterialFromTerrainDataBase()
    {
        //Game.MakeSureGameInstanceIsNotNull();
        var terrainInfo = MaterialDataBase.instance.GetTerrainMaterialInfo(groundType, indexInTerrainDataBase);
        tileMaterial = terrainInfo.tileMaterial;
        tile = terrainInfo.tile;
        cam3DtoSpriteMaterial = terrainInfo.cam3DtoSpriteMaterial;
    }

    public void RestoreLightObstacleGenerator()
    {
        if (lightObstacleGenerator) // background no tiene
        {
            lightObstacleGenerator.Material = Instantiate(MaterialDataBase.instance.terrainLightObstacleMaterial); // lo instancio para que cada uno pueda tener su alpha tex
            lightObstacleGenerator.Material.SetTexture(ShaderProp._Alpha, d2dSprite.AlphaTex);
        }
    }
    public bool HaveLostMySpriteRendererOrSprite()
    {
        if (backgroundType == BackgroundType.SingleTile)
        {
            if (!spriteRenderer)
                return true;
            if (!spriteRenderer.sprite)
                return true;
        }
        return false;
    }
    public void ZeroMyPosition()
    {
        var pos = transform.position;
        pos.x = 0;
        pos.y = 0;
        transform.position = pos;
    }

    public override string ToString()
    {
        string texName;
        if (texture)
            texName = texture.name;
        else
            texName = " NULL TEXTURE";

        string matName;
        if (tileMaterial)
            matName = tileMaterial.name;
        else
            matName = " NULL TILE MATERIAL";

        return gameObject.name + " " + texName + "/" + matName;
    }
}

