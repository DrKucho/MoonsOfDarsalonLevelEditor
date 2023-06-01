using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine.Profiling;

using UnityEngine.Profiling;
using System.Threading;

public enum LProcess {PrimaryThread_0, SecondaryThread_1, MainThreadTasks_2, Shurikens_3 }
[System.Serializable]
public class MainThreadTask
{
     public Behaviour[] behabiourToDisable;
     public Behaviour[] behabiourToEnable;
    /// <summary> Enables Or Disables Monobehabiours
    public MainThreadTask(Behaviour[] beh, bool onOff)
    {
        if (onOff)
            behabiourToEnable = beh;
        else
            behabiourToDisable = beh;
    }
    bool ShowBehToDisable() { return behabiourToDisable != null; }
    bool ShowBehToEnable() { return behabiourToEnable != null; }
}
/// <summary>
/// Represents and manages the liquidfun physics simulation world.
/// You will need an instance of this component on a gameobject in your scene in order for the physics simulation to run.</summary>
public class LPManager : MonoBehaviour
{
    [System.Serializable]
    public class Settings
    {
        public Vector2 gravity = new Vector2(0, -9.78f);
        public float timeStep = 0.2f;
        public int velocityIterations = 1;
        public int positionIterations = 1;
        public bool overridePartIterations = true;
        public int partIterationsOverrride = 2;
        public float baseInterpolationVelocityMultiplayer = 3;
        
    }
    public bool debug = false;
    public bool debugPolyPoints = false;
    public bool debugPolyOnOff = false;
    public bool debugThreadExecs;
    public bool debugSwitchOfMonoAfter1Sec = false;
    public bool debugSkips = false;
    public bool debugExecutionOrder = false;

    public LProcess[] process = { LProcess.PrimaryThread_0, LProcess.SecondaryThread_1, LProcess.MainThreadTasks_2, LProcess.Shurikens_3 };
    int procIdx = 0;
    public int extraframes = 0;

    [Header("Thread1")]
    public float PLT_Ms;
    public float contactMs;
    public float getParticlesDataAndZombies;
    public bool PLT_Running;

    [Header("Thread2")]
    public float SLT_Ms;
    public float drawersMs;
    public float afterDrawerTasksMs;
    public bool SLT_Running;

    [Header("Thread1 & Thread2")]
    public float partDataStart;
    public float allLiquidMs;
    public float allLiquidWithYieldsMs;

    public Settings one;
    public Settings two;
    public Settings three;
    public Settings four;
    [Header("2 = OK")]
    [ReadOnly2Attribute] public int frameDiff = 0;
    [Header("Time Step")]
    [Tooltip("How much time should pass in the simulation every time it steps")]
    public float TimeStep = 1.0f / 60.0f;
    public float timeStepMultCompensator = 0.5f;
    [ReadOnly2Attribute] public float currentTimeStep; // NO FUNCIONA BIEN, es el valor timeStep calculado en cada nuevo ciclo para llamar a LPAPIWorld.WorldStep tiene en cuenta si se ha tardado mas de dos frames en hacer un ciclo y lo compensa añadiendo la parte proporcional , pero me vuelve las particulas locas, un timestep mayor no seolo aumenta su velocidad sino que influye tambien en las forzas que se aplican unas a otras , y los lagos saltan arriba y abajo
    [Header("Gravity")]
    [Tooltip("Gravity Vector in the world")]
    public Vector2 Gravity = new Vector2(0f, -9.81f);
    public float gravityMultCompensator = 0.5f;
    [ReadOnly2Attribute] public Vector2 currentGravity; // NO FUNCIONA BIEN, es el valor gravity calculado en cada nuevo ciclo para llamar a LPAPIWorld.WorldStep tiene en cuenta si se ha tardado mas de dos frames en hacer un ciclo y lo compensa añadiendo la parte proporcional , pero me vuelve las particulas locas, un timestep mayor no seolo aumenta su velocidad sino que influye tambien en las forzas que se aplican unas a otras , y los lagos saltan arriba y abajo
    [Tooltip("How accurate body velocity calculations should be. Note: More accurate = more expensive")]
    public int VelocityIterations = 6;
    [Tooltip("How accurate body position calculations should be. Note: More accurate = more expensive ")]
    public int m_positionIterations = 2;
    [Tooltip("Override the recommended number of particle iterations")]
    public bool OverrideParticleIterations;
    [Tooltip("If overriding use this many particle iterations. Note: less particle iterations can dramatically improve performance. but does so at the cost of a less accurate, more volatile simulation")]
    public int ParticleIterationsOverride = 2;
    public int[] partIterations = { 2, 1 };
    int partIterationsIndex;
    public float baseInterpolationVelocityMultiplayer = 3;

    [ReadOnly2Attribute] public float interpolationVelocityMultiplayer = 6;
    public float unityToLiquidsTime = 0.37f; // para saber el tiempo unity de un tiempo liquido(la vida de las particulas) hay que multiplicar por este numero, ojo, calculado a mano midiendo el tiempo en que tardan en desaparecer las particulas, cambniará si cambiamos los ajustes de lpmanager (timeStep)
    [Tooltip("Use a contact listener. Will reduce performance especially if you have particles with contact listener flags set")]
    public bool UseContactListener = false;
    public bool useKuchoContactListener = false; // KUCHO HACK
    public int maxContacts = 3000;
    public int maxBodiesToUpdate = 300;
    [ReadOnly2Attribute] public int particleCount;
    [ReadOnly2Attribute] public int fixPartContactCount = 0;
    [ReadOnly2Attribute] public double watchMs;
    [ReadOnly2Attribute] public double watchTicks;
    [ReadOnly2Attribute] public double realTimeMs;

    private int m_particleIterations = 2;
    public bool useJoints = false; // asi me ahorro el codigo prehistorico con findobjectsoftype
    static LPManager _instance;// KUCHO HACK

    public static LPManager instance
    {
        get
        {
            if (_instance == null && !Application.isPlaying)
            {
                var core = KuchoHelper.LoadGameCoreFromResourcesFolder();
                _instance = core.GetComponentInChildren<LPManager>();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
    
    public LPParticleSystem[] ParticleSystems;
    [ReadOnly2Attribute] public LPParticleSystem firstParticleSystemWithCatalyst;
    [ReadOnly2Attribute] public LPParticleSystem firstParticleSystemWithFlamableParts;
    [ReadOnly2Attribute] public LPParticleSystem firstParticleSystemWithDamageParts;

    public LPContactListener ContactListener { get; private set; }
    public LPKuchoContactListener KuchoContactListener { get; private set; } // KUCHO HACK


    [HideInInspector] public IntPtr worldPtr; // KUCHO HACK la hice publica

    private LPBody[] bodies; // KUCHO HACK 
    private LPJoint[] joints; // KUCHO HACK
    private LPJointGear[] jointGears; // KUCHO HACK

    public LPParticleMaterial[] lpMaterials;
    [HideInInspector] public static event System.Action onPLT_Update;
    [HideInInspector] public static event System.Action onMT_Update_1;
    [HideInInspector] public static event System.Action onMT_Update_2;
    [HideInInspector] public static bool isDebugBuild; // copia de Constants.isDebugBuild para poder leerla en audio thread
    int worldSteppeddOnFrame;

    public LPTasks lpTasks;
    public static LPTasks statLpTasks;
    public MainThreadTask[] mainThreadTasks = new MainThreadTask[500]; // no deberia nunca superar este numero
    public List<MonoBehaviour> MonobehabiourToDestroyComponent = new List<MonoBehaviour>();// aqui copio los componentes LPFixture a destruir para hacerlo en el main thread

    bool at_isActiveAndEnabled; // lo mismo

    [HideInInspector] public static GameObject gameGO; // lo fija game con su propio gameObejct, mlo uso al cargar nivel y eliminar bodies
    [HideInInspector] public static GameObject playerGO;


        [HideInInspector] public static ThreadLoop primaryLiquidThread;
        [HideInInspector] public static ThreadLoop secondaryLiquidThread;

    public static int primaryThreadExecutionCount = 0;
    public static int secondaryThreadExecutionCount = 0;



    public static Thread mainThread;

    public static float debugUnityTimeCounter = 0;
    public static System.Diagnostics.Stopwatch debugSW = new System.Diagnostics.Stopwatch();

    [HideInInspector] public static LPBody generalStaticBody;


    public IntPtr GetPtr()
    {
        return worldPtr;
    }

    public Dictionary<int, LPBody> allBodies = new Dictionary<int, LPBody>();
    private Dictionary<int, LPBody> bodiesToReset;
    private int bodiesIndex = 0;
    int bodiesToUpdateCount;
    LPBody[] bodiesToUpdate;
    IntPtr[] bodiesToUpdatePtr;
    float[] bodiesToUpdateInfo; // donde se importa los datos de los bodies a updatear directamente desde el engine de liquidos

    WaitForEndOfFrame waitForEndOfFrame;
    
    public static bool _syncResetInProcess;
    public static bool syncResetInProcess // no parece funcionar el metodo normal handle.isRunning
    {
        get { return _syncResetInProcess;}
        set
        {
            if (value == true)
            {
                Debug.LogError("QUIEN ME HA ACTIVADO?");
            }
            _syncResetInProcess = value;
        }
    }

    public static bool asyncResetInProcess; // no parece funcionar el metodo normal handle.isRunning
    
    
    

    public static Delegates.Simple onPaused;


    float unityTimescale = 1;
    public static bool runAllowed = true;
    public static bool stopRequested = false;
    //public static bool newTasksAllowed = true;
    int sltTriesCount = 0;
    int framesToWaitThreads = 60;
    private int goodFrame = 0;

   
    int frameToCheckLiquidsAreRuning = 0;

    static public bool breakRequested; // Debug.Break solo puede ser llamado desde l main thread, cuando queiro parar el editor por algo que ocurre en algun background thread uso esto

    int mtIndex = 0;

    public bool IsPossibleToAddNewMainThreadTask()
    {
        return mtIndex < mainThreadTasks.Length;
    }

    int preStepFrame = 0;
    int postStepFrame = 0;
    int updateCoroutineCycleControlCount = 0; // este se resetea en update, sirve para controlar que estamos avanzando y se pone a cero cada x ciclos
    public static int updateCoroutineCycleCount = 0;

    [HideInInspector] public static string activeScreen;
   
    // HILO PRIMARIO - PROCESAR LAS LIQUID TASKS - WORLD STEP - OTRAS TAREAS ######################################################################################################
    System.Diagnostics.Stopwatch allLiquidWatch = new System.Diagnostics.Stopwatch();
    bool plt_1_LPTasks_Done;
    bool plt_2_WorldStep_Done;
    bool plt_3_ContactListenerUpdate_Done;
    bool slt_4_ProcessContacts_Done;
    bool plt_5_DestroyByWeight_Done;
    bool plt_6_GetPartDataAndZombies_Done;
    bool plt_7_UpdateBodies_Done;
    bool plt_8_DelegateCalls_Done;
    int primaryThreadHappenedOnFrame = -1;

  
    // HILO SECUNDARIO -  ACTUALIZAR LOS PARTICLE DRAWERS Y OTRAS TAREAS ###############################################################################################
    //(se dispara en FixedUpdate, por algo referente a la optimizacion de tiempos, es lo primero que ocurre antes de lo demas)
    bool slt_1_UpdateDrawers_Done;
    bool slt_2_DelegateCalls_Done;
    int secondaryThreadHappenedOnFrame;
    public event Action afterUpdateDrawersBackgroundTask;
    System.Diagnostics.Stopwatch SW1 = new System.Diagnostics.Stopwatch();
    System.Diagnostics.Stopwatch SW2 = new System.Diagnostics.Stopwatch();

    // TAREAS QUE SOLO SE PUEDEN HACER EN EL HILO PRINCIPAL (apagar gameobjects y cosas asi) #####################################################################################
    public int processMainThreadTasksExecutionCount = 0;
    float MTT_ms;
    /// <summary>
    /// revisa los bodies uno a uno y selecciona los que deben ser updateados, despues, si hay bodies, mete una tarea para hacer en el MainThread que es updatear bodies


    int fullupdateCycles;
    int frameLastProcessWasexecuted;
  

    private List<float> frameMultiplierArray;
    private int frameMultiplierArrayIndex;
   
}

