using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;


using UnityEditor;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Debug = UnityEngine.Debug;

[System.Serializable]
public class LevelTitleAndPreview
{
    public string name;
    public Texture2D preview;

    public override string ToString()
    {
        return name;
    }
}

public class ScenesAndDifficultyManager : MonoBehaviour
{

    public bool debug = false;
    public float loadSceneMinDelay = 4f;
    public float sceneStartDelay = 0.1f;
    public float sceneStartFadeSpeedMult = 1.5f;
    public List<string> alternativeScenes;
    //public List<string> gameScenes;
    //public LevelsData gameData;
    public Texture2D levelPreviewNotFound;
    public int _sceneIndex = -1;
    public int sceneIndex
    {
        get { return _sceneIndex; }
        set
        {
            _sceneIndex = value;
        }
    }
    public Scene previousScene;
    public Scene currentScene;
    public static GameObject[] currentSceneRootGos;
    public string currentSceneNameUpper;
    public string actualDifficultyLevel;
    public static int difficultyLevelIndex = 0;
    public static Difficulty difficulty;
    public static LevelMusicLoader sceneMusicLoader;
    public AudioClipArray audioOnLoadStandBy;
    public static bool gameTitleScreenHasBeenLoaded = false;

    //  public List<Gadget> gadgetsPickedUpThisLevel;
    public static LevelDifficulty levelDiff;
    public static ScenesAndDifficultyManager instance;
    
    public static int maxPlayerStartPointIndex; // lo fijan los stickers player
    //public static bool cargoWasPickedUp;
    static List<string> deadStickers = new List<string>();
    public static int deathPerLevelCount = 0;

    [Header("FOG (Standard settings)")]
    public bool setFog;
    bool Fog() { return setFog; }
    public bool fog = true;
    public FogMode forgMode = FogMode.Linear;
    public float fogStart = 0;
    public float fogEnd = 200;
    public float fogDensity = 1;
    public Color fogColor = Constants.solidBlack;



    int scoreOnNewScene;
    private bool showLowFpsScreenBackup;
    private bool showLowFpsScreenInEditorBackup;

    static AsyncOperationHandle<SceneInstance> loadSceneOperation;

    Stopwatch sw = new Stopwatch();


    public static bool levelWasFullyLoadedAndScriptsStarted;
    
    float internalFXVolumeBackup = -1;



}
