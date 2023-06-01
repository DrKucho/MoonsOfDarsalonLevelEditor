using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Profiling;

using System.Threading;

/// <summary>
/// Class holding information about one particle. Note may be partially filled out depending on LPparticlesystem settings</summary>
public class LPParticle
{
    public Vector2 Position; // era vector 3 probando a que sea Vector2
    /// <summary>Color of the particle, color use is incompatible with rotation </summary>
    public Color _Color;
	/// <summary> rotation codified inside Color.r (KUCHO HACK) </summary>
	public float startRotation;
	/// <summary> TangularSpeed codified inside Color.g (KUCHO HACK) </summary>
	public float angularVelocity;
    /// <summary>Remaining lifetime of the particle, < 0 = infinite, 0 = particle will be destroyed next step</summary>
    public float LifeTime;
    /// <summary>Pressure the particle is experiencing</summary>
    public float Weight;
    /// <summary>Velocity of the particle</summary>
    public Vector2 Velocity;
    /// <summary>Userdata value of the particle. Can be used for gameplay mechanics</summary>
    public uint UserData;


}

/// <summary>
/// Class holding information about one particle/particle contact</summary>
public struct LPSystemPartPartContact
{
    /// <summary>Index of 1st particle involved in the collision</summary>
    public int ParticleAIndex;
    /// <summary>Index of 2nd particle involved in the collision</summary>
    public int ParticleBIndex;
    /// <summary>Userdata value of 1st particle involved in the collision</summary>
    public int ParticleAUserData;
    /// <summary>Userdata value of 2nd particle involved in the collision</summary>
    public int ParticleBUserData;
}

/// <summary>
/// Class holding information about one fixture/particle contact</summary>
public struct LPSystemFixPartContact
{
    /// <summary>index of the body involved in the collision</summary>
    public int BodyIndex;
    /// <summary>index of the fixture in the body involved in the collision</summary>
    public int FixtureIndex;
    /// <summary>index of the particle involved in the collision</summary>
    public int ParticleIndex;
    /// <summary>Userdata value of the particle involved in the collision</summary>
    public int ParticleUserData;
    /// <summary>Normal to the collision vector</summary>
    public Vector3 Normal;
    /// <summary>Weight or pressure of the collision</summary>
    public float Weight;
}


/// <summary>
/// Represents and manages a liquidfun particle system</summary>	
public class LPParticleSystem : MonoBehaviour
{
	public bool debug = false;
    [Tooltip("particles or particle groups with this index will be created in this particle system")]
	[ReadOnly2Attribute] public int partSysIndex;
    [Tooltip("Radius of all the particles in the system")]
    public float ParticleRadius = 0.1f;
    [Tooltip("How much particles loose their velocity")]
    public float Damping = 0.6f;
    [Tooltip("Scale of the world gravity in relation to this particle system. A higher value makes the simultion look smaller. Minus value will make the particles fall 'upwards'")]
    public float GravityScale = 1f;
	public float SurfaceTensionNormalStrenght = 0.2f;
	public float SurfaceTensionPressureStrenght = 0.2f;
	public float ViscousStrenght = 1f;
	[Header ("Parts Number Control")]
    [Tooltip("Can particles be destroyed autmatically according to their age?")]
    public bool DestroyByAgeAllowed = true;
    [Tooltip("Limit for the amount of particles this particle system can contain." +
             "If limit is exceeded and DestroyByAgeAllowed is true oldest particles are destroyed 1st." +
             "If limit is exceeded and DestroyByAgeAllowed is false, new particles will not be added to the system" +
            " Value of 0 or less indicates no limit")]
    public int ParticleAmountLimit = 0;
    public int _particleCount;
    public int particleCount
    {
        get { return _particleCount; }
        set
        {
            _particleCount = value;
            particleCountFloat = (float)value;
        }
    }
    [HideInInspector] public float particleCountFloat = 0; // KUCHO HACK
    [ReadOnly2Attribute] public int deletedCount = 0; // particulas borradas en este frame
	public float destructByWeightThreshold = 20;
	public int maxZombieParts = 20;
	public int howManyZombieChecksPerFrame = 10;
	[ReadOnly2Attribute] public int fixPartContactsCount;

    private bool GetPositions = true;
    [Tooltip("Should all the particles colour info be retrieved on Update")]
	public bool GetColors = true; // SI SE CAMBIA HAY QUE LLAMAR LUEGO A OnlyOneGetColorsOrRotations() !!!!!!!!!!!
	[HideInInspector][SerializeField] bool GetColorsOld;
	public bool GetRotations = false; // SI SE CAMBIA HAY QUE LLAMAR LUEGO A OnlyOneGetColorsOrRotations() !!!!!!!!!!!
	[HideInInspector][SerializeField] bool GetRotationsOld;
	bool GetColorsOrGetRotations = false;
    [Tooltip("Should all the particles lifetime info be retrieved on Update")]
    public bool GetLifeTimes = false;
    [Tooltip("Should all the particles weight info be retrieved on Update")]
    public bool GetWeights = false;
    [Tooltip("Should all the particles velocity info be retrieved on Update")]
    public bool GetVelocities = false;
    [Tooltip("Should all the particles userdatas be retrieved on Update")]
    public bool GetUserData = false;
    [Tooltip("Should we update particle groups. IMPORTANT: Only needed if SplitGroups() is called")]
    public bool GetGroupData = false;

    ///[SerializeField]
    //public LPParticle[] SavedParticles;

	public KuchoLPMultiDrawer multiDrawer; // KUCHO HACK
	public LPParticle[] Particles;
	public LPParticleGroup[] Groups;

     [ReadOnly2Attribute] [System.NonSerialized] public bool initialised = false; // KUCHO HACK
     [ReadOnly2Attribute] [System.NonSerialized] public IntPtr ptr;
     [ReadOnly2Attribute] [System.NonSerialized] public string at_name;

    bool NotGettingColors(){return !GetColors;}

	[HideInInspector] [NonSerialized] public static int[] particlesInShape; //marshalled array para compartir entre todos los chequeos y no generar mucha basura NO LA USO?

	private LPManager lpMan;
    
	int previousCount = 0; //KUCHO HACK para debug
	
    /// <summary>
    /// Get the pointer to the C++ object represented by this object (In this case it is the particle system object)</summary>
    public IntPtr GetPtr()
    {
        return ptr;
    }

    int getPositionsInt;
    int getColorsInt;
    int getRotationsInt;
    int getLifeTimesInt;
    int getWeightsInt;
    int getVelocitiesInt;
    int getUserDataInt;


    IntPtr particlesPointer;
    float[] particlesCountArray;
    float[] particleData;
    int at_particleCount;


    int positionsStartPos = 1;
    int colorsOrRotationsStartPos = 0;
    int lifeTimesStartPos = 0;
    int weightsStartPos = 0;
    int velocitiesStartPos = 0;
    int userDataStartPos = 0;
    int groupDataStartPos = 0;


    [ReadOnly2Attribute] public int zombieCount; // indice para las particulas zombies
    int[] zombieParts;
    int zombieCheckIndex = 0;

    int weightContactZombiCountPlusOne = 1; // se inicializa a 1 y por consiguiente muestra el conteo de particulas MAS UNO, esot me ahorra hacer una suma cada vez que añado
    int[] zombiesByWeightContact;

    [HideInInspector] public LPSystemFixPartContact contact = new LPSystemFixPartContact(); // contacto reusable para cada contacto que se esta procesando en ese momento
    int badCount = 0;


}
