    using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Light2D;
using System;

using System.Threading;
using UnityEngine.Serialization;
    
    using UnityEditor;
    
    public enum GameStatus { StandBy, Playing, Alternative, Welcome, GameOver }

public class Game : MonoBehaviour
{

    public bool debug = false;
    public GameData _data;

    public static GameData data
    {
        get
        {
            if (G)
                return G._data;
            else
            {
                //MakeSureGameInstanceIsNotNull();
                if((!G))
                    Debug.LogError("GAME.G ES NULL INCLUSO DE IR A BUSCARLO A RESOURCES? WTF?");
                return null;
            }
        }
        set
        {
            if (value == null)
                Debug.LogError("INTENTO DE ASIGNAR NULL A GAME.DATA");
            else
            {
                if (G)
                    G._data = value;
                else
                    Debug.LogError("NO PUEDO ASIGNAR GAME.DATA PORQUE GAME.G ES NULL");
            }
        }
    }

    public static int levelFrame = 0;
    public static int frame = 0;
    public static float time = 0;
    public static bool runInBackGround;

    public AudioClip resetSound;
    public AudioMixer mainMixer;
    public static AudioMixerGroup miscAudioMixerGroup;
    public static AudioMixerGroup musicAudioMixerGroup;
    public static AudioMixerGroup musicWetAudioMixerGroup;
    public static AudioMixerGroup voicesAudioMixerGroup;
    public static AudioMixerGroup playerAudioMixerGroup;
    public AudioManager priorityVoice;
    public AudioMixerSnapshot allaudioMixerSnapshot;
    public AudioMixerSnapshot noAudioMixerSnapshot;
    public AudioMixerSnapshot musicOnlyMixerSnapshot;
    public float maxAudioDistance = 90000;
    public float maxAudioPanDistance = 180;
    // VARIABLES DEL SISTEMA ANTIGUO CUANDONO HABIA AUDIO MIXER
    public static float FXVolume = 1f;
    public static float internalFXVolume = 0.75f;
    public static float musicVolume = 1f;
    public static float internalMusicVolume = 0.33f * 1.15f;
    private float musicVolume_ = 1f;
    public static float realMusicVolume;
    public static float fadeToBlackVolume = 1f;
    private float volumeDecToZeroAtVinylFX = 0.02f; // decremento e incremento usado en las rutinas de efecto vinilo para bajar a tope o volver a subir el volumen y evitar rumble cuando el pitch es cero
    public static float noBackgroundMusicVolume = 0.4f;

    public static GameStatus status = GameStatus.Welcome;
    public static bool pauseOnJustNow = false;
    public static bool pauseOffJustNow = false;
    public static float timeScaleAndPitch = 1f;
    public float fixedDeltaTime = 0.01f;
    public float maximumDeltaTime = 0.01f;
    public int lightMapReadyTries = 60;
    public static float slowFactor;
    public float normalSpeed = 1f;
    public float slowMotionSpeed = 0.2f;//
    public static bool slowMotion = false;
    public float pauseSpeed = 0f; // normalmente debe ser 0 asi se pausa totalmente, pero si pongo valores mas altos tengo slow motion (aunque no responderia a los controles
    public int pauseStrength = 4; // constante , la fuerza del freno y arranque del efecto vinylFX en modo pausa, siempre fue 4
    public float lostOneLifeStrength = 1.2f; // constante , la fuerza del freno y arranque del efecto vinylFX cuando te matan una vida
    public static float vinylFXStrength = 4f;
    public static LevelMusic levelMusic;
    public static LevelTimer levelTimer;
    public static Transform cameraTarget; //es lo que seguira la camara, se encuentra por que lleva el tag CameraTarget
    public PhysicsMaterial2D alwaysTriggerMaterial;
    //public InputActionAsset inputActions;
    //public MODInputActions myControls;
    public GameObject playerPrefab;
    public GameObject defaultSkyPrefab;
    public GameObject sunPrefab;
    //  public static PlayerStartPoints playerStartPoints;
    public static GameObject playerGO;
    public static Collider2D playerCol;
    public static AudioManager playerAM;
    public static Player playerSC;
    public static CC playerCC;
    public static EnergyManager playerEM;
    public bool infiniteLives;
    public int startingPlayerLives = 3;
    int _playerLives; // variable privada 
    public int playerLives
    { // GETTER AND SETTER ha de actuar con la variable privada, si actuo con el mismo getter y setter entra en loop infinito
        get
        {
            return _playerLives;
        }
        set
        {
            if (!infiniteLives)
                _playerLives = value;
        }
    }

    public static GameObject parallax; // lo pone CreateGameIfNotPresent and instanciar el background, sirve para al instanciar uno nuevo diferente, poder destruir el antiguo
    public static Sun sun;
    public static Sun skyLight;
    public static SkyManager skyManager; // TODO todas las variables de este tipo deberian ser sustituidas por Skymanager.instance Sun.instance... 

    public static Game _G;

    public static Game G
    {
        get
        {
#if UNITY_EDITOR
            if (_G == null)
            {
                var go = KuchoHelper.LoadGameCoreFromResourcesFolder();
                if (go)
                    _G = go.GetComponent<Game>();
                //if (!_G)
                //{
                    //Debug.Log("NO ENCUENTRO GAME CORE");
                //}
            }
#endif
            
            return _G;
        }
        set
        {
            _G = value;
        }
    }
    public static int zoomFactor = 2;

    public static float previousTimeScale;
    public float speedUpTimeScale = 4f;
    public GameObject _3D_LightsParent;
    public Color ambientLightColor;
    //-------------------------------------------------------------------------------------------------------------------------


    public ExplosionStampExtras thinAreaKiller;
    public MaterialDataBase materialDataBase;


    public enum PauseStatus { ApplyingPause, ReleasingPause, TotallyPaused, PlayingAtNormalSpeed };
    public static PauseStatus pauseStatus = PauseStatus.PlayingAtNormalSpeed;
    public static bool paused = false;
    public GameObject weightPrefab;
    Transform debugThings;
    [ReadOnly2Attribute] public ItemStoreList allStoresList; // TODO borrame y cambiame por ItemStoreList.instance.
    //public static LightingSystemExtras worldLightSystemExtras; // LO COMENTO PORQUE NO NECESITO GAME SERIALIZADO SOLO ACCEDER A SUS MIERDAS
    public LightingSystem lightingSystem; // el mapa de luz del mundo en baja resolucion;

    public static DecoManager[] decoManagers; // todos ellos
    public static KuchoTileGrid[] tileGrids;

    public static Texture2D plantsTexture;

    public static ExpensiveTaskManager expensiveTaskManager;
    public static GlobalParticlesManager globalParticlesManager;

    public static ArmySettings goodGuys;
    public static ArmySettings badGuys;
    public static ArmySettings wildAnimals;
    


    public delegate void OnFindLevelSpecificStuff();
    public static event OnFindLevelSpecificStuff onFindLevelSpecificStuff;
    
    public GameObject mark;
    public GameObject markBottomLeft;

    public GameObject debugPosToFlighTo;
    public GameObject debugTargetPos;
    


    public static bool helpDeveloper = false;
    public bool quitIfNoInternet = true;
    public static LPManager LiquidPhysicsManager;
    public static Thread mainThread;

    public static bool imposible = false;

    public static bool debugAI;
    public static bool debugFollowers;
    public static bool debugPatience;

    public static int groundMakerBulletStamped = 0; // se suma 1 desde bullet en cada stamp existoso
    public static int groundMakerBulletCatalized = 0; // se suma 1 desde explosionstampextras en cada stamp que ha necesitado de catalizador
    public static int groundMakerBulletNotCatalized = 0; // se suma 1 desde explosionstampextras en cada stamp fallido y se pone a 0 cada vez que decimos una frase en PlayerThinkZone
    //public SteamManager steamManager;
    public ColliderSettings defaultGroundSettings;

   
}
