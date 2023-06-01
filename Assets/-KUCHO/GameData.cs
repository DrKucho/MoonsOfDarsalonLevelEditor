using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using System;
using System.Reflection;
using System.IO;
using System.Linq;
using UnityEditor;

public enum GameCoreAssetMode { ResorceFolder, Addressable }

[CreateAssetMenu(fileName = "New GameData", menuName = "Game Data", order = 51)]
public class GameData : ScriptableObject
{
    public const string MOTHER_AND_CHILD_PROJECTS_SHARED_FOLDER = "Assets/-KUCHO/LEVEL-MAKER";
    [System.Serializable]
    public class LevelData
    {
        public string fullName;// tal cual es el nombre del fichero
        public string title;//Upper Case & Clean , extraido de full Name y eliminando la palabra Level del principio
        public int starsneeded;
        public Texture2D preview;
        public LevelDifficulty.MainGoal mainGoal;
        public LevelDifficulty.SpecialGoal specialGoal1;
        public LevelDifficulty.SpecialGoal specialGoal2;
        public LevelDifficulty.SpecialGoal specialGoal3;
        public bool includedInBuild;

        public override string ToString()
        {
            if (Application.isPlaying)
                return title;
            else
            {
                if (preview == null)
                    return title + " " + starsneeded + "  -----  MISSING PREVIEW";
                else
                    return title + " " + starsneeded;
            }
        }
        

        public LevelData()
        {
            mainGoal = new LevelDifficulty.MainGoal();
            specialGoal1 = new LevelDifficulty.SpecialGoal();
            specialGoal2 = new LevelDifficulty.SpecialGoal();
            specialGoal3 = new LevelDifficulty.SpecialGoal();
        }
    }
    [System.Serializable]
    public class ExpireData
    {
        public string _elapsedTimeFileName = "Levels";
        [HideInInspector] static public string elapsedTimeFileName = "Levels";
        static string elapsedTimePath;
        
        [Header("Game Expiry RunTime")]
        public float runTimeFullDays = 3f;
        public float runTimeHours = 200f;

        [ReadOnly2] public float runTimeFullSeconds = 500f;

        static public string encryptedTotalElapsedTime;
        [Header("Game Expiry Date")]
        [Tooltip("To be added to the current date and calculate the actual expiry date")]
        public float lifeDays; // los dias que se sumaran a la fecha actual para calcular la fecha de caducidad
        [SerializeField] public System.DateTime expiryDate;
        [HideInInspector] public static int year;
        [HideInInspector] public static int month;
        [HideInInspector] public static int day;
        
        [HideInInspector] public static float startTime;
        [HideInInspector] public static float elapsedTime;
        [HideInInspector] public static float totalElapsedTime;
        
        [HideInInspector] public static bool _expired = false;

        public static bool expired
        {
            get { return _expired; }
            set
            {
                if (value != _expired)
                {
                    if (value)
                    {
                    }
                    else
                    {
                    }
                }
                _expired = value;
            }
        }

    }
    //public Game gameScript;

    private static GameData _instance;
    public static GameData instance
    {
        get
        {
            #if UNITY_EDITOR
            if (_instance == null)
            {
                _instance = AssetDatabase.LoadAssetAtPath<GameData>("Assets/-KUCHO/InBuildStuff/GameData.asset");
                if (!_instance)
                {
                    //Debug.Log("NO ENCUENTRO GAME DATA");
                }
            }
            #endif
            
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public enum BuildType { Steam, GRound, Antidote }

    public BuildType buildType;
    public static GameCoreAssetMode gameCoreAssetMode = GameCoreAssetMode.Addressable;
    public bool isMother;
    public CCStaticData ccStaticData;
    [FormerlySerializedAs("notPoolableObjects")] public LevelObjectDataBase levelObjects;
    public bool masterTapeLoadFxAndC64Switch;
    public string lastOSXVersion;
    public string lastWINVersion;
    public bool isDemo;
    public bool isLevelEditor;
    public bool userIsDeveloper;
    public uint steamFullGameAppId;
    public uint steamDemoAppId;
    public uint steamLevelEditorAppId;
    public bool forceDoSteamProtection = false;
    public bool targetGamePathIsDemo;
    public ExpireData expireData;
    [Tooltip("Se asigna al pulsar play leyendo el file Version")]
    [ReadOnly2] public string version;
    public int levelCountInLastBuild = 0;
    public List<string> playerTips = new List<string>();
    public int starsNeededIncPerLevel = 6;
    public List<LevelData> levelData = new List<LevelData>();
    public GameObject sun;
    public StickerModels stickerModels;
    public GameObject groundEdit;
    public FarBackgroundData farBackgroundData;
    public MaterialDataBase materialDataBase;
    public ShaderDataBase shaderDataBase;
    public MeshDataBase meshDataBase;
    public SoundDataBase soundDataBase;
    public AnimationDataBase animationDataBase;

    public AnimationCurve walkerSpeedToDamage;

    public PalleteColorAdjust palleteAdjustOSX;
    public PalleteColorAdjust palleteAdjustWIN;
    public static PalleteColorAdjust currentPalleteAdjust;

    public float testFloat  = 0.01666667f;
    public float testFloat2  = 1;
    public float testFloat3  = 1;
    [Range(-1,1)] public float testFloat4  = 1;
    [Range(0,2)] public float testFloat5  = 2;
    public int testInt  = 1;
    public bool testBool1 = true;
    public bool testBool2 = true;

    public TestValues testValues;


    public uint GetSteamAppId()
    {
        /* esto no funciona, la demo no puede identificarse como tal al parecer, y steamworks.init falla
        // mientrasno funcione el tema de la nueva id para el level editor, pruebo a mandar el id de demo o full game segun el tipo este inyectando en una u otra
        if (!targetGamePathIsDemo)
            return steamFullGameAppId;// le mando la contraria el full game para que steam no se lie
        else// tiene full?
            return steamDemoAppId;// le mando la contraria, la demo game para que steam no se lie
        */
        
        //if (isLevelEditor) // no funciona, despues de intentar subir el articulo me devuelve error (Busy), he preguntado a steam -> //segun la tipa de steam , la app puede tener su propio ID y operar con el , pero al crear objeto le decimos para que APP es
            return steamLevelEditorAppId;
        /*
        else if (isDemo && false) // no funciona, creo que las appId de demo no sirven para esto, steamworks.net no conecta
            return steamDemoAppId;
        else
            return steamFullGameAppId;
        */
    }

    #if UNITY_EDITOR
    public static KuchoBuildData GetKuchoBuildData()
    {
        var guids = AssetDatabase.FindAssets("t:KuchoBuildData");
        string path = "";
        if (guids.Length > 0)
        {
            path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<KuchoBuildData>(path);
        }

        return null;
    }
    #endif
    static public int GetIndexOfLevel(string fullLevelName)
    {
        if (instance.levelData == null)
            instance.levelData = new List<LevelData>();
        
        for (int i = 0; i < instance.levelData.Count;i++)
        {
            if (instance.levelData[i].fullName == fullLevelName)
                return i;
        }
        return -1;
    }
    public static string GetCleanTitleFromSceneFileName(string t)
    {
        t = t.ToUpper();
        if (t.StartsWith("LEVEL "))
        {
            t = t.Remove(0, 6);
        }
        else if (t.StartsWith("LEVEL"))
        {
            t = t.Remove(0, 5);
        }
        if (t.StartsWith("UGCLEVEL "))
        {
            t = t.Remove(0, 9);
        }
        else if (t.StartsWith("UGC LEVEL"))
        {
            t = t.Remove(0, 9);
        }

        int counter = 0;
        while (t.StartsWith(" ") && counter < 200)
        {
            t = t.Remove(0, 1);
            counter++;
        }
        return t;
    }
    



    private int playerTipIndex;



}
