using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public static class LevelCounters
{
    [System.NonSerialized] public static int playerBlasterShotCount;
    [System.NonSerialized] public static int groundDestructionCount;
    [System.NonSerialized] public static int playerGroundAmmoShotCount;
    [System.NonSerialized] public static int ammoPickupCount;
    [System.NonSerialized] public static int laserGunAmmoPickupCount;
    [System.NonSerialized] public static int groundAmmoPickupCount;
    [System.NonSerialized] public static int playerHits;
    [System.NonSerialized] public static int allCompanionHits;
    [System.NonSerialized] public static int flamableParticleExplosions;
    
    public static void Reset()
    {
        playerGroundAmmoShotCount = 0;
        playerBlasterShotCount = 0;
        ammoPickupCount = 0;
        laserGunAmmoPickupCount = 0;
        groundAmmoPickupCount = 0;
        playerHits = 0;
        allCompanionHits = 0;
        flamableParticleExplosions = 0;
        groundDestructionCount = 0;
    }
}

public enum LevelType { Single, FirstSubLevel, MiddleSublevel, EndSublevel };

public class LevelDifficulty : MonoBehaviour
{
    //public GameData levelInfoSO;
    public enum MainGoalType
    {
        RescueDarsanauts,
        GetYourselfToBase,
        GetVehicleToBase
    };

    [System.Serializable]
    public class MainGoal
    {
        public MainGoalType type;
        [Range(0.15f, 0.8f)] public float sucessRatio = 0.5f;
        [ReadOnly2Attribute] public Vector3 thresholds;
        [ReadOnly2Attribute] public int menCount;
        [HideInInspector] [ReadOnly2Attribute] public int allSubLevelsMenCount;

        public int menNeededForBronze
        {
            get
            {
                int value = Mathf.FloorToInt(menCount * sucessRatio);
                if (value == 0)
                    value = 1;
                return value;
            }
        }

        public int menNeededForSilver
        {
            get
            {
                float half = (1 - sucessRatio) * 0.5f;
                float t = sucessRatio + half;
                int value = Mathf.FloorToInt(menCount * t);
                if (value == 0)
                    value = 1;
                return value;
            }
        }

        public int menNeededForGold
        {
            get
            {
                return menCount;
            }
        }

        public override string ToString()
        {
            return GetDisplayName(true);
        }

        bool IsRescueDarsanauts()
        {
            return type == MainGoalType.RescueDarsanauts;
        }

        public void CountMenOut()
        {
            menCount = 0;
            Sticker[] stickers = FindObjectsOfType<Sticker>();
            foreach (Sticker st in stickers)
                if (st.storeFinder.name == "CivilianStore" && (!st.transform.parent || !st.transform.parent.GetComponent<StickerModels>()))
                    menCount++;
            allSubLevelsMenCount = menCount;
            thresholds.x = menNeededForBronze;
            thresholds.y = menNeededForSilver;
            thresholds.z = menNeededForGold;
        }

        public MainGoal()
        {
            type = MainGoalType.RescueDarsanauts;
        }

        public void CopyFrom(MainGoal o)
        {
            type = o.type;
            menCount = o.menCount;
            allSubLevelsMenCount = o.allSubLevelsMenCount;
            sucessRatio = o.sucessRatio;
            thresholds = o.thresholds;
        }

        public static string GetDisplayName_NOP(MainGoalType type, LevelDifficulty ld)
        {
            string s = "";
            switch (type)
            {
                case MainGoalType.RescueDarsanauts:
                    if (ld.mainGoal.menNeededForBronze == 0)
                        s = "NO DARSANAUTS TO RESCUE?";
                    else if (ld.mainGoal.menNeededForBronze == 1)
                        s = "RESCUE 1 DARSANAUT";
                    else
                        s = "RESCUE DARSANAUTS (MIN " + ld.mainGoal.menNeededForBronze.ToString() + ")";
                    break;
                case MainGoalType.GetVehicleToBase:
                    s = "GET VEHICLE TO BASE";
                    break;
                case MainGoalType.GetYourselfToBase:
                    s = "GET YOURSELF TO BASE";
                    break;
            }

            return s;
        }

        public string GetDisplayName(bool endColon)
        {
            string s = "";
            switch (type)
            {
                case MainGoalType.RescueDarsanauts:
                    if (menNeededForBronze == 0)
                        s = "NO DARSANAUTS TO RESCUE?";
                    else if (menNeededForBronze == 1)
                        s = "RESCUE 1 DARSANAUT"; 
                    else
                        s = "RESCUE DARSANAUTS (MIN " + menNeededForBronze.ToString() + ")";
                    break;
                case MainGoalType.GetVehicleToBase:
                    s = "RETURN VEHICLE (MIN INTEGRITY " + (int) (sucessRatio * 100) + ")";
                    break;
                case MainGoalType.GetYourselfToBase:
                    s = "GET YOURSELF TO BASE (MIN ENERGY " + (int) (sucessRatio * 100) + ")";
                    break;
            }

            if (endColon)
                s += ":";
            return s;
        }

        public int GetStarValue(float value)
        {
            if (value >= sucessRatio * 3)
                return 3;
            if (value >= sucessRatio * 2)
                return 2;
            if (value >= sucessRatio * 1)
                return 1;
            return 0;
        }
    }

    public enum SpecialGoalType
    {
        Time,
        KillAll,
        FindTreasures,
        YouUntouched,
        AllCoins,
        AllLinedCoinsAtOnce,
        NoShooting,
        NoGroundMaker,
        TheyUntouched,
        AllEnterBaseAtOnce,
        VehicleUntouched,
        AllChallengesAtOnce,
        VehicleHalfEnergy,
        NoGroundDestruction
    }

    [System.Serializable]
    public class SpecialGoal
    {
        public SpecialGoalType type;

        bool IsTime()
        {
            return type == SpecialGoalType.Time;
        }

        public int minutes;
        public int seconds;

        public override string ToString()
        {
            return GetDisplayName(true);
        }

        public void CopyFrom(SpecialGoal o)
        {
            type = o.type;
            minutes = o.minutes;
            seconds = o.seconds;
        }

        public SpecialGoal()
        {

        }

        public SpecialGoal(SpecialGoalType t)
        {
            type = t;
        }

        public static string GetDisplayName(SpecialGoalType t, string mins, string secs, bool endColon)
        {
            string s = "";
            switch (t)
            {
                case SpecialGoalType.Time:
                    string minstring;
                    if (mins == "0" || mins == "1")
                        minstring = " MIN ";
                    else
                        minstring = " MINS ";

                    s = "LESS THAN " + mins + minstring + secs + " SECS";
                    break;
                case SpecialGoalType.KillAll:
                    s = "KILL ALL ENEMIES";
                    break;
                case SpecialGoalType.FindTreasures:
                    s = "GET SPECIAL TREASURE";
                    break;
                case SpecialGoalType.YouUntouched:
                    s = "YOU STAY UNTOUCHED";
                    break;
                case SpecialGoalType.VehicleUntouched:
                    s = "VEHICLE UNTOUCHED";
                    break;
                case SpecialGoalType.AllCoins:
                    s = "PICK ALL COINS";
                    break;
                case SpecialGoalType.AllLinedCoinsAtOnce:
                    s = "ALL COIN GROUPS ON EACH FLIGHT";
                    break;
                case SpecialGoalType.NoShooting:
                    s = "NO LASER SHOOTING";
                    break;
                case SpecialGoalType.NoGroundMaker:
                    s = "NO GROUND MAKER";
                    break;
                case SpecialGoalType.NoGroundDestruction:
                    s = "NO GROUND DESTRUCTION";
                    break;
                case SpecialGoalType.TheyUntouched:
                    s = "THEY STAY UNTOUCHED";
                    break;
                case SpecialGoalType.AllEnterBaseAtOnce:
                    s = "THEY ALL ENTER BASE AT ONCE";
                    break;
                case SpecialGoalType.AllChallengesAtOnce:
                    s = "ALL CHALLENGES AT ONCE";
                    break;
                case SpecialGoalType.VehicleHalfEnergy:
                    s = "VEHICLE 50% OF INTEGRITY";
                    break;
                default:
                    s = "NOT FOUND: ADD NAME TO GetDisplayName FUNCTION";
                    break;
            }

            if (endColon)
                s += ":";
            return s;
        }

        public string GetDisplayName(bool endColon)
        {
            return GetDisplayName(type, minutes.ToString(), seconds.ToString(), endColon);
        }


    }

    public enum ChargeAction
    {
        Add,
        Set,
        SetPercentage
    }

    [System.Serializable]
    public class GadgetChargeAtLoadLevel
    {
        public string name;
        public ChargeAction action;
        public float amount;
    }

    [HideInInspector] public bool debug = false;
    [HideInInspector] public string osxBuildVersion; // para debuguear lo de los niveles de usuario y comprobar que efectivamente son de build distintas o no
    [HideInInspector] public string winBuildVersion;

    [HideInInspector] public bool allCommandsAreRound = false;
    //bool testModeQuickStart= false;
    //float noEnemiesAtStartInSeconds;

    //public int startsNeeded = 0;
    public Texture2D preview;
    public enum TagType { ACTION = 1000, EXPLORATION = 2000, PUZZLE = 3000, SHIP = 4000, TRUCK = 5000, EASY = 6000, NORMAL = 7000, DIFFICULT = 8000 };
    public TagType[] tags;
    public MainGoal mainGoal;
    public SpecialGoal specialGoal1 = new SpecialGoal();
    public SpecialGoal specialGoal2 = new SpecialGoal();
    public SpecialGoal specialGoal3 = new SpecialGoal(SpecialGoalType.AllChallengesAtOnce);
    public LevelMusic.Type[] musicTypes;
    [HideInInspector] public string specificMusic;
    [HideInInspector] public int specificMusicStartBar = 0;
    [HideInInspector] public string notes = "";
    [HideInInspector] public int extraBaseTimeSeconds = 0;
    [HideInInspector] public static int extraBaseTimeSecondsStatic;
    [HideInInspector] [Range(0, 1)] public float flyerCharge = 0.5f;
    [Header("OJO! Pon: FlashLight, JetPack, BlueLaser, GroundMaker... NO PONGAS WeaponFlashLight")]
    [HideInInspector] public GadgetChargeAtLoadLevel[] gadGetCharge;// no necesario desde que se han de recoger en cada nivel
    [HideInInspector] public string[] gadgets;// para que sirve esto?
    [HideInInspector] public LevelType levelType = LevelType.Single;
    /// <summary> All SubLevels men count at start
    [HideInInspector] public SubLevelJump subLevelJump;

    /// <summary> This SubLevel or SingleLevel men count at start
    /// <summary> This SingleLevel OR ALL SubLevels men count at start
    //[ReadOnly2Attribute] public int menNeeded;
    [HideInInspector] public int deadMenCount; // lo actualiza bases cada vez que muere alguien
    [HideInInspector] public bool enemySpawnersRunning = false;
    [HideInInspector] public bool decorativeSpawnersRunning = false;
    [HideInInspector] public bool otherSpawnersRunning = false;
    [HideInInspector] public Difficulty level;
    [HideInInspector] public Difficulty zone; // Hide
    [HideInInspector] public Difficulty real; // Hide

    [HideInInspector] public float enemyWeightOnScene;
    [HideInInspector] public bool maxEnemyWeightOnSceneReached = true;

    [HideInInspector] private ObjectSpawner[] allSpawners;
    [HideInInspector] public ObjectSpawner[] enemySpawners;
    [HideInInspector] public ObjectSpawner[] otherspawners;
    [HideInInspector] public ObjectSpawner[] decorativeSpawners;
    [HideInInspector] public Collider2D[] enemySpawnerColliders;

    private int enemySpawnerCount = 0;
    private int decorativeSpawnerCount = 0;
    private int otherSpawnerCount = 0;

    public static LevelDifficulty instance;

    bool IsSubLevel() { return levelType != LevelType.Single; }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying) // si estamos en plan y ademas el nicel es un addressable, el preview se serializa con una textura sacada del addressable y cuando paramos el juego esa textura se rdestruye, por lo que el preview se qeda a null
        {
            Tools.visibleLayers &= ~(1 << LayerMask.NameToLayer("TransparentFX"));
            Tools.lockedLayers |= (1 << LayerMask.NameToLayer("Ground")); //LocK  
            //Tools.lockedLayers |= (1 << LayerMask.NameToLayer("Gound")); //Unlock
            //ShaderDataBase.SerializeCurrentLevelMaterials(); // no tiene sentido serializar los materiales/shaders en proyecto hijo level editor
        }
    }
    #endif 

    private void OnEnable()
    {


    }

    public void InitialiseInEditor()
    {
        osxBuildVersion = GameData.instance.lastOSXVersion;
        winBuildVersion = GameData.instance.lastWINVersion;
        extraBaseTimeSecondsStatic = extraBaseTimeSeconds;
        deadMenCount = 0;
        if (mainGoal == null)
            mainGoal = new MainGoal();
        mainGoal.CountMenOut();
        GetSpawners();

    }

    void GetSpawners()
    {
        ObjectSpawner[] allGenerators = FindObjectsOfType<ObjectSpawner>();// ahora solo pillo los del nivel
        // primero hago una pasada por todos apra saber cuantos de losgeneradores tienen el tag "Enemy"
        enemySpawnerCount = 0;
        decorativeSpawnerCount = 0;
        otherSpawnerCount = 0;
        
        foreach (ObjectSpawner eg in allGenerators)
        {
            if (eg.isActiveAndEnabled)
            {
                if (eg.CompareTag("Enemy"))
                    enemySpawnerCount++;
                else if (eg.CompareTag("Decorative"))
                    decorativeSpawnerCount++;
                else
                    otherSpawnerCount++;
            }
        }
        if (debug) print(this + " GENERADORES ENCONTRADOS = " + allGenerators.Length + " DE LOS CUALES SON ENEMIGOS=" + enemySpawnerCount + " DECORATIVOS=" + decorativeSpawnerCount);
        //asi puedo crear una tabla fija mas rapida y que aparece en inspector
        enemySpawners = new ObjectSpawner[enemySpawnerCount];
        decorativeSpawners = new ObjectSpawner[decorativeSpawnerCount];
        otherspawners = new ObjectSpawner[otherSpawnerCount];
        // y luego vuelvo a repasarlo todo y copio los generadores enemigos a la tabla que acabo de crear
        enemySpawnerCount = 0;
        decorativeSpawnerCount = 0;
        otherSpawnerCount = 0;
        int i;
        for (i = 0; i < allGenerators.Length; i++)
        {
            if (allGenerators[i].isActiveAndEnabled)
            {
                if (allGenerators[i].CompareTag("Enemy"))
                {
                    enemySpawners[enemySpawnerCount] = allGenerators[i];
                    enemySpawnerCount++;
                }
                else if (allGenerators[i].CompareTag("Decorative"))
                {
                    decorativeSpawners[decorativeSpawnerCount] = allGenerators[i];
                    decorativeSpawnerCount++;
                }
                else //
                {
                    otherspawners[otherSpawnerCount] = allGenerators[i];
                    otherSpawnerCount++;
                }
            }
        }
        enemySpawnerColliders = new Collider2D[enemySpawners.Length];
        for (i = 0; i < enemySpawners.Length; i++)
        {
            enemySpawnerColliders[i] = enemySpawners[i].gameObject.GetComponent<Collider2D>();
        }
    }
    
    public void CalculateRealDifficulty()
    {
        real.maxEnemyWeightOnScene = level.maxEnemyWeightOnScene * zone.maxEnemyWeightOnScene * ScenesAndDifficultyManager.difficulty.maxEnemyWeightOnScene;
        real.pickUpChances = level.pickUpChances * zone.pickUpChances * ScenesAndDifficultyManager.difficulty.pickUpChances;
        real.generatorDelay = level.generatorDelay * zone.generatorDelay * ScenesAndDifficultyManager.difficulty.generatorDelay;
        real.generatorEnemies = level.generatorEnemies * zone.generatorEnemies * ScenesAndDifficultyManager.difficulty.generatorEnemies;
        real.enemyEnergy = level.enemyEnergy * zone.enemyEnergy * ScenesAndDifficultyManager.difficulty.enemyEnergy;
        real.enemyFlyingForce = level.enemyFlyingForce * zone.enemyFlyingForce * ScenesAndDifficultyManager.difficulty.enemyFlyingForce;
        real.enemyMaxFlyingForce = level.enemyMaxFlyingForce * zone.enemyMaxFlyingForce * ScenesAndDifficultyManager.difficulty.enemyMaxFlyingForce;
        real.enemyRunSpeed = level.enemyRunSpeed * zone.enemyRunSpeed * ScenesAndDifficultyManager.difficulty.enemyRunSpeed;
        real.enemyInitialNoAttackTime = level.enemyInitialNoAttackTime * zone.enemyInitialNoAttackTime * ScenesAndDifficultyManager.difficulty.enemyInitialNoAttackTime;
        real.enemyFireTimer = level.enemyFireTimer * zone.enemyFireTimer * ScenesAndDifficultyManager.difficulty.enemyFireTimer;
        real.enemyPunchTimer = level.enemyPunchTimer * zone.enemyPunchTimer * ScenesAndDifficultyManager.difficulty.enemyPunchTimer;
        real.playerBulletSpeed = level.playerBulletSpeed * zone.playerBulletSpeed * ScenesAndDifficultyManager.difficulty.playerBulletSpeed;
        real.enemyBulletSpeed = level.enemyBulletSpeed * zone.enemyBulletSpeed * ScenesAndDifficultyManager.difficulty.enemyBulletSpeed;
    }

}
