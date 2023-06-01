using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.Audio;



public class AudioManager : MonoBehaviour
{
    static string debugClip = "NO*KFGHOPHDFG";
    public bool debug = false;
    public bool debug2 = false;
    public bool debugPoly = false;
    public Rigidbody2D rb2D;
    public AudioMixerGroup output;
    public bool priorityOnVoices = false;
    public float masterGain = 1f;
    public float masterPitch = 1f;
    public float extraAudibleDistance = 0;
    public bool staticAudio = false;
    public bool pitchFollowsTimeScale = true;
    public LastAudio lastAudio;
    public LastAudio lastAudio2;

    float fadeIncDec = 0.1f;
    Vector3 distToPlayer;
    Vector3 distToCamCenter;

    //int sequentialListIndex = 0;
    //AudioClipWrap[] sequentialList;
    //float sequentialListGain;
    //bool sequentialListBeingPlayed = false;

    int sequentialArrayIndex = 0;
    AudioClipArray sequentialArray;
    float sequentialArrayGain;
    bool sequentialArrayBeingPlayed = false;

    //int index= 0; // todo implmentar en MOD (Eliminar) este indice ahora esta en AudioClipArray
    public bool isTouching = false;
    public OnCollisionStayCaller onCollisionStayCaller;

    [Header("---LOOPED NOISE-----------------")]
    public bool useLoopedNoise = false;
    bool UsingNoise() { return useLoopedNoise; }
    public LoopedNoise noise;

    [Header("---REGULAR POLIPHONY-------------")]
    public int poliphony = 1;
    public bool monoChannelLooped = false;
    public AudioSource[] audioS;
    private int[] screenRecorderAudioSourceIndex;
    public float[] audioSG; // la Ganancia del audio source , que viene dada por el comando PlayAudio
    public float[] audioSPitch; // el pitch , que viene dada por el comando PlayAudio
    public float[] audioSPauseFadeVolume; // el valor de volumen durante el fade out que se multiplicara por los otros volumenes para obtener el real
    int a = 0; // indice que marca el audio source a utilizar, se actualiza cno RotateAudioSource()
    [HideInInspector] public float distanceVolume = 1f;
    [HideInInspector] public float distancePan = 1f;
    [HideInInspector] public Item item;
    public int audioClipsRuning = 0;  // esto no es preciso , puede darse el caso en el que se disparen audio clips muy seguidos, este contador incremente y de mas clips runing que audiosources , lo cual hace imposible el conteo pero es orientativo, se comprueban los audio clips reales en cada invoke de audio clip terminado
    bool polyLimited = false;
    public AudioManagerSaver audioSaver;
    public enum SpeedType { RB_Linear_Spd, RB_Angular_Spd, RB_AngularPlusLinear_Spd, RB_AngularMinusLinear_Spd, RB_AngularMultLinear_Spd, TireSkid, Given, None }
    public enum SignChange { MakeZero, MakePositive, MakeNegative, Unchanged }

    [HideInInspector]
    public int enabledFrame;
    [System.Serializable]
    public class LastAudio
    {
        public AudioClip clip;
        public float time;
        public float elapsed; // vslisble en minusculas, se puede leer libremente, es mas rapida que la property Elapsed por que no tiene que hacer operaciones, pero si no las llamado al getter tendra un valor desactualizado
        // GETTERS AND SETTERS el mismo nombre que la variable que modifican pero con la primera mayuscula
        public float Elapsed
        {
            get
            {
                return Time.time - time;
            }
            set
            {
                elapsed = value;
            }
        }
    }
    [System.Serializable]
    public class LoopedNoise
    {

        //@Header(" usadas solo en noise modo TireSkid")
        float linearSpeed;
        float angularSpeed;
        public bool playOnEnable;
        [ReadOnly2Attribute] public float input; // se fija externamente o la lee de Rb2D
        public AudioSource audioSource;
        public int indexInSoundDataBase; // solo se usa si no hay clip
        public AudioClip clip;
        public SpeedType inputMode = SpeedType.None;

        public bool useItemRBForLinearVelInput;
        public SignChange negativeInput = SignChange.Unchanged;

        bool ShowLinearToAngularRatio() { return inputMode == SpeedType.TireSkid; }
        [Range(0, 20)] public float linearToAngularRatio; // la vel angular es siempre mayor que la lineal este valor se ha de ajustar segun el tamaño de la rueda para que ambas velocidades den un valor igual y la formula que calculan el derrape funcione

        bool ShowLinearSpeedRatio() { return inputMode == SpeedType.RB_Linear_Spd | inputMode == SpeedType.RB_AngularPlusLinear_Spd | inputMode == SpeedType.RB_AngularMultLinear_Spd | inputMode == SpeedType.RB_AngularMinusLinear_Spd | inputMode == SpeedType.TireSkid; }
        [Range(0, 10)] public float linearSpeedRatio;

        bool ShowAngularSpeedRatio() { return inputMode == SpeedType.RB_Angular_Spd | inputMode == SpeedType.RB_AngularMultLinear_Spd | inputMode == SpeedType.RB_AngularPlusLinear_Spd | inputMode == SpeedType.RB_AngularMinusLinear_Spd | inputMode == SpeedType.TireSkid; } 
        [Range(0, 10)] public float angularSpeedRatio;

        [Range(-1, 1)] public float masterRatio = 1f; // se multiplica a los calculos para rebajar o aumentar la magnitud de la velocidad del RB antes de trabajar con ella
        [Range(-1, 1)] public float inputOffset = 0f; // se suma despues de aplucar el master ratio
        [ReadOnly2Attribute] public float processedInput; // despues de aplicar el masterRatio
        [Header("--------")]
        [Range(0, 10)] public float collisionVelocityToSpeedRatio = 1f;
        public bool instantCatchUp = false;
        [Range(0, 20)] public float catchUpSpeed = 1f;
        //  bool muteAtLowSpeed = true;
        [Range(0, 10)] public float muteUnderInput = 2f;
        public bool onlyOnContact = false;
        [Header("--------")]
        [Range(0, 5)] public float pitch = 1f; // es como un master gain?
        [Range(0, 0.6f)] public float pitchRatio = 1f;
        [Range(0, 1)] public float pitchOffset = 0f;
        [Range(0, 30)] public float maxPitch = 20f;
        [Header("--------")]
        [Range(0, 5)] public float volume = 1f; // es como un master gain?
        [Range(0, 0.3f)] public float volumeRatio = 1f;
        [Range(0, 1)] public float volumeOffset = 0f;
        [Range(0, 1)] public float maxVolume = 1f;
        [Header("--------")]
        [ReadOnly2Attribute] public float volumeToBeApplied = 0f;
        [ReadOnly2Attribute] public float pitchToBeApplied = 0f;
        [ReadOnly2Attribute] public float speedReacher = 0f; // intenta llagar a rigidBody2D.velocity.magnitude (osea a speed), pero va un poco por detras
        public int contactsWithStaticColliderCount = 0;
        public int contactsWithDynamicColliderCount = 0;
        private int screenRecorderAudioSourceIndex = -1;

        Collider2D[] contacts = new Collider2D[4];
        ContactFilter2D tireSkidFilter = new ContactFilter2D();
        int playOnFrame = -1;

    }

    void OnValidate()
    {
        if (isActiveAndEnabled)
        {
            if (audioS == null || poliphony != audioS.Length || (useLoopedNoise && !noise.audioSource) || (!useLoopedNoise && noise.audioSource))
                InitialiseInEditor();
        }
    }
    
    public void InitialiseInEditor()
    {
        item = GetComponentInParent<Item>();
        audioSaver = GetComponent<AudioManagerSaver>();
        lastAudio.elapsed = KuchoTime.time; //la inicializo solo para que no tenga null
        lastAudio2.elapsed = KuchoTime.time; //la inicializo solo para que no tenga null
        rb2D = GetComponentInParent<Rigidbody2D>();
        poliphony = Mathf.Abs(poliphony);
        DestroyAudioSources();

        if (!output)
            output = Game.miscAudioMixerGroup; // si no esta asignado en inspector pilla el de por defecto

        // CREA AUDIO SOURCE PARA NOISE ---------------
        if (useLoopedNoise)
        {
            noise.audioSource = gameObject.AddComponent<AudioSource>();
            noise.audioSource.outputAudioMixerGroup = output; // era output
            noise.audioSource.loop = true;
            noise.audioSource.playOnAwake = false;
            noise.audioSource.clip = noise.clip;
            noise.audioSource.volume = 0;
            
        }
        else
            noise.audioSource = null;

        // CREA POLIFONIA NORMAL  ---------------
        audioS = new AudioSource[poliphony];
        audioSPauseFadeVolume = new float[poliphony];
        audioSG = new float[poliphony];
        audioSPitch = new float[poliphony];

        for (int i = 0; i < poliphony; i++)
        {
            audioS[i] = gameObject.AddComponent<AudioSource>();
            audioS[i].playOnAwake = false;
            audioSPauseFadeVolume[i] = 1f;
            audioS[i].outputAudioMixerGroup = output; // era output
        }

        if (audioS.Length > 0)
            audioS[0].loop = monoChannelLooped; //TODO sigo usando esto?

        onCollisionStayCaller = GetComponent<OnCollisionStayCaller>();

        a = audioS.Length - 1;
    }
    void DestroyAudioSources() // se la llama inmediatamente o despues de que todos los inspectors hayan sido actualizado en unity editor
    {
        AudioSource[] audToDestroy = GetComponents<AudioSource>();

        for (int i = 0; i < audToDestroy.Length; i++)
        {
            if (audToDestroy[i] != null)
            {
                if (Application.isPlaying)
                    Destroy(audToDestroy[i]);
                else
                    DestroyImmediate(audToDestroy[i]);
            }
        }
        audToDestroy = null;
    }

    

}
