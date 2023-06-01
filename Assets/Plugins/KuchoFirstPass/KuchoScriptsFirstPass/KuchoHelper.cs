using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.UI;
using System.Reflection;
using UnityEngine.SceneManagement;

using UnityEngine.Profiling;
using System.Threading;
using System;
using System.IO;
using UnityEditor;


public enum SidesAlgorythm { Random, Alternate, Right_Up_Pos, Left_Down_Neg }; // weapon
public enum LookAt { Target, Left, Right, Random, Alternate, ParentLookingAt }; // enemy generators
public enum CharacterType { Walker, Flyer, Static, Truck };
public enum St { NO, DEPRECATED_borning, IDLE, MOVINGFORWARD, FALL, DUCK, FLIP, FIRE, FIREDUCK, FIREUP, FIREDOWN, DEPRECATED_JUMP, MOVINGUP, MOVINGDOWN, HURT, DYING, DEAD, LADDER, LADDERASS, DEPRECATED_LEVELJUMPUP, DEPRECATED_LEVELJUMPDOWN, KO, STANDUP, DUCKDOWN, PUNCH, PUNCHDUCK, PUNCHUP, PUNCHDOWN, HANG, ROLL, FLYING, WARNING, PUNCHFLYER, PUNCHUPFLYER, FREEZED, SWIMTRANSITION, CLIMB };
public enum weapAmmo { Bullets, Enemies };
public enum BasePos { Player, Generator, ScreenBorders }; // enemy generators v1 solo
public enum FindGroundFrom { Dont, TopScreenBorder, ProjectilePosition, PlayerY, TopWorldBorder }; //weapon
public enum OnOff { Enable, Disable };
public enum ItemKind { Bullet, Enemy, PickUp, PickUpText, ExplosionManager, Decorative, GroundFiller };
public enum WeaponEnum { Tape, CDRom, Pendrive, Vinyl, None }
public enum MovementType { Rigidbody, Transform }
public enum EnemyColliderType { Hurt, NoHurt }
public enum AttackCollider { Vision, Punchable, None }
public enum GroundMask { GroundOnly, Ground_Levels_Obstacle_Roof_EnemObstacle, Generators, Ground_EnemyObstacle_EnemyOnly, Ground_EnemyOnly_EnemyBirth } // enemy generators
public enum DoItAt { Awake, Enable, Start, EveryFrame, GameAndFarBackgroundLoaded, SceneLoaded, None }
public enum FunctionKey { F1 = KeyCode.F1, F2 = KeyCode.F2, F3 = KeyCode.F3, F4 = KeyCode.F4, F5 = KeyCode.F5, F6 = KeyCode.F6, F7 = KeyCode.F7, F8 = KeyCode.F8, F9 = KeyCode.F9, F10 = KeyCode.F10, F11 = KeyCode.F11, F12 = KeyCode.F12 }
public enum Operation { None, Mult, Add, Set }
public enum EnableOrDisable { Enable_Open, Disable_Close, Nothing }
public enum ThreeWaySwitch { SwitchOn, SwitchOff, DoNothing }

public class Delegates
{
    public delegate void Simple();
    public delegate void String(string str);
    public delegate void Bool(bool onOff);
    public delegate bool IntAndReturnBool(int value);
}
[System.Serializable]
public struct UpDownLeftRight
{
    public bool up;
    public bool down;
    public bool left;
    public bool right;
}
[System.Serializable]
public struct TimeStampBool
{
    bool _boolValue;
    public bool boolValue
    {
        get
        {
            return _boolValue;
        }
        set
        {
            _boolValue = value;
            setTime = KuchoTime.time;
        }
    }
    public float setTime;
}


public enum UpdateScoreMode { Add, Reset, Sub }
public enum Direction { Up, Down, Left, Right }


public class ArrayOfIntArrays
{
    public int index;
    public int[] ints;

    public bool Add(int intToAdd)
    {
        if (index < ints.Length)
        {
            ints[index] = intToAdd;
            ints[0] = index;
            index++;
            return true;
        }
        return false;
    }
}
[System.Serializable]
public struct AbsFloat
{
    public float signed;
    public float absolute;

    public AbsFloat(float signedValue)
    {
        signed = signedValue;
        absolute = Mathf.Abs(signed);
    }
}

[System.Serializable]
public class EnableDisableGameObjects
{
    public enum Action { Enable, Disable }
    public Action action;
    public GameObject[] gos;

    public void DoIt()
    {
        bool onOff = false;
        if (action == Action.Enable)
            onOff = true;

        if (gos != null)
        {
            foreach (GameObject go in gos)
            {
                go.SetActive(onOff);
            }
        }
    }
}
[System.Serializable]
public class ShaderScanlineData
{
    public float offset;
    public float strength;
    public float thinness;
    public float lumaMatters;

    public void CopyFrom(ShaderScanlineData o)
    {
        offset = o.offset;
        strength = o.strength;
        thinness = o.thinness;
        lumaMatters = o.lumaMatters;
    }
}

public static class PlayerPrefNames
{
    #if UNITY_EDITOR
    static public string resoWidth = "Resolution Width Editor";
    static public string resoHeight = "Resolution Height Editor";
    static public string resoZoom = "Resolution Zoom Editor"; // ojo! ha de ser igual que el gameobject.name del antiguo PixelZoom que ahora renombro a Resolution Zoom
    static public string resoRate = "Resolution RefreshRate Editor";
    static public string resoBlur = "Resolution Blur Editor"; //TODO no tiene sentido almacenar el blur cuando se inicializa sola dependiendo del zoom
    static public string resoAberration = "Resolution Aberration Editor"; //TODO no tiene sentido almacenar la aberracion cuando se inicializa sola dependiendo del zoom
    #else
    static public string resoWidth = "Resolution Width";
    static public string resoHeight = "Resolution Height";
    static public string resoZoom = "Resolution Zoom"; // ojo! ha de ser igual que el gameobject.name del antiguo PixelZoom que ahora renombro a Resolution Zoom
    static public string resoRate = "Resolution RefreshRate";
    static public string resoBlur = "Resolution Blur"; //TODO no tiene sentido almacenar el blur cuando se inicializa sola dependiendo del zoom
    static public string resoAberration = "Resolution Aberration"; //TODO no tiene sentido almacenar la aberracion cuando se inicializa sola dependiendo del zoom
    #endif

    public static bool HasAllResolutionRelatedKeys()
    {
        if (PlayerPrefs.HasKey(resoWidth) && PlayerPrefs.HasKey(resoHeight) && PlayerPrefs.HasKey(resoZoom) && PlayerPrefs.HasKey(resoRate) && PlayerPrefs.HasKey(resoBlur) && PlayerPrefs.HasKey(resoAberration))
            return true;
        return false;
    }
}

[System.Serializable]
public class WorldLightPixel
{
    public Color pixel;
    [Range(0, 1)] public float visibility;
    public int posX;
    public int posY;
    public int updateFrame;
    //Constructor
    public WorldLightPixel(Color _pixel, float _visibility, int _posX, int _posY)
    {
        pixel = _pixel;
        visibility = _visibility;
        posX = _posX;
        posY = _posY;
    }
    //Constructor
    public WorldLightPixel()
    {
        pixel = new Color(0, 0, 0, 0);
        visibility = 0f;
        posX = 0;
        posY = 0;
    }
    public void SetWorldLightMapPosition(Vector2 worldPosition, Vector2 worldSize, Vector2 lightMapSize)
    {
        posX = Mathf.RoundToInt((worldPosition.x * lightMapSize.x) / worldSize.x);
        posY = Mathf.RoundToInt((worldPosition.y * lightMapSize.y) / worldSize.y);
    }
    public void CopyFrom(WorldLightPixel o)
    {
        pixel = o.pixel;
        visibility = o.visibility;
        posX = o.posX;
        posY = o.posY;
    }
}
[System.Serializable]
public struct Point
{
    public int x;
    public int y;
    //Constructor
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public static Point Cast(Vector2 vec)
    {
        Point p = new Point();
        p.x = (int)vec.x;
        p.y = (int)vec.y;
        return p;
    }
    public Point Sum(Point p)
    {
        this.x += p.x;
        this.y += p.y;
        return this;
    }
    public Point Sub(Point p)
    {
        this.x -= p.x;
        this.y -= p.y;
        return this;
    }
    public static Point Sum(Point p1, Point p2)
    {
        p1.x += p2.x;
        p1.y += p2.y;
        return p1;
    }
    public static Point Sub(Point p1, Point p2)
    {
        p1.x -= p2.x;
        p1.y -= p2.y;
        return p1;
    }
}

[System.Serializable]
public class ParticleSystemWrap
{
    public ParticleSystem PS;
    [HideInInspector] public ParticleSystemRenderer _renderer;
    [SerializeField] // en unity 2018.3 con el nuevo sistema de prefabs parece que las particulas no se serializan, y estoy definiendo esto en initialiseineditor, intento de forzar a la serializacion con serializefield
    public ParticleSystem.Particle[] particles; // buffer de particulas que se definira con el numero  maximo de particulas
    public bool limitedByGlobalManager = true;
    public float minParticlesFactor = 0.5f;
    public int maxParticlesLimit = 300;
    public bool darkenParticlesBasedOnZ = false;
    public float particleRateMult = 1f;
    public bool scaleSpeed;
    public bool scaleSize;
    public bool scaleLifeTime;
    public bool basedOnPixelsDestroyed = false;

    public ParticleCollisionEvent[] collisions;
    [Header("collisions")]
    [Range(0, 1)] public float minVolume;
    [Range(0, 1)] public float maxVolume;
    [Range(0, 0.01f)] public float collisionSpeedFactor = 0.2f;
    public AudioClipArray audioOnParticleCollision;
    public bool inheritLightspritecolor = true;
    [Range(0, 1)] public float colorMult = 1;
    [Range(0, 1)] public float alpha = 1;

    [ReadOnly2Attribute] public float originalStartSpeed; // los MainModule.multiplyers si pero parece que no funcionan como deberian , si yo fijo el multiplier me machaca el valor maximo del rango aleatorio, por eso tengo que guardar el original y calcular un nuevo mazimo en cada explosion
    [ReadOnly2Attribute] public float originalLifeTime; // los MainModule.multiplyers si pero parece que no funcionan como deberian , si yo fijo el multiplier me machaca el valor maximo del rango aleatorio, por eso tengo que guardar el original y calcular un nuevo mazimo en cada explosion
    [ReadOnly2Attribute] public float originalSize; // los MainModule.multiplyers si pero parece que no funcionan como deberian , si yo fijo el multiplier me machaca el valor maximo del rango aleatorio, por eso tengo que guardar el original y calcular un nuevo mazimo en cada explosion
    [ReadOnly2Attribute] public float originalRate; // era privada pero al hacer una class e intentar acceder a ella me da error por que no puede acceder a ella (protection level)
    public ParticleSystem.MinMaxGradient originalColor; //  no parece que la use realmente, la copio del particle system y luego no la uso
    public Color32 startColor; // no parece que la use realmente, la copio del particle system y luego no la uso
    [ReadOnly2Attribute] public int maxParticles;// calculado justo antes de disparar particulas si se limitan mi propio calculo basado en la velocidad de emision y la duracion del sistema de particulas para que sa leido pr el global manager

    public void CalculateMaxParticles()
    {
        maxParticles = (int)(PS.emission.rate.constantMax * PS.duration); // el global particles limiter lee estos valores para calcular cuanto debe limitar
        if (maxParticles > maxParticlesLimit)
            maxParticles = maxParticlesLimit;
    }

    public int minParticles; // calculado justo antes de disparar particulas si se limitan, para que sea leido por el global manager
    //Constructor
    public ParticleSystemWrap()
    { // constructor, aunque solo defino 3 variables, lo que importa es que se crea el elemento entero, las demas pillan el valor por defecto 0 y null etc...
        this.limitedByGlobalManager = true;
        this.minParticlesFactor = 0.5f;
        this.maxParticlesLimit = 300;
        this.darkenParticlesBasedOnZ = false;
        this.particleRateMult = 1f;
        this.basedOnPixelsDestroyed = false;
        this.collisions = new ParticleCollisionEvent[12];
    }
    public override string ToString()
    {
        return PS.ToString();
    }
    public void SetValuesInParticleSystem(float _pixelsDestroyedFactor, float emisionRateMult, float lifeTimeMult, float speedMult)
    {
        KuchoHelper.SetParticleSystemEmmisionRate(PS, originalRate * particleRateMult * emisionRateMult * _pixelsDestroyedFactor);
        ParticleSystem.MainModule main = PS.main; // crea una referencia? o esta copiando? deberia copiar pero puede que haga una referencia tal y como pasa con el EmissionModule

        float scaleX = PS.transform.localScale.x;
        float _scaleTime;
        if (scaleLifeTime)
            _scaleTime = scaleX;
        else
            _scaleTime = 1;

        main.startLifetimeMultiplier = originalLifeTime * lifeTimeMult * _scaleTime; // bastará con esto para que se modifique el valor tal y como pasa con emission.rate? o tengo que volver a copiar el mainmodule al particlesystem?

        float _scaleSpeed;
        if (scaleSpeed)
            _scaleSpeed = scaleX;
        else
            _scaleSpeed = 1;

        main.startSpeedMultiplier = originalStartSpeed * speedMult * _scaleSpeed; // no lo entiendo pero el startSpeedMultiplier funciona como valor absoluto , no como un multiplicador que multiplica al maximo y minimo de la curva, no se puede tener unos valores definidos y despues usar este como ganancia

        float _scaleSize;
        if (scaleSize)
            _scaleSize = scaleX;
        else
            _scaleSize = 1;

        main.startSizeMultiplier = originalSize * _scaleSize;
    }
}

public static class MyLogs
{
    public static string docFolderPath;
    public static string resolutionsLogName = "Resolutions.log";
    public static string resolutionsLogPath = "";
    public static string executionOrderName = "ExecutionOrder.log";
    public static string executionOrderPath = "";
    public static bool initialised;

    public static void Initialise(bool forceInitialise)
    {
        if (!initialised || forceInitialise)
        {
            docFolderPath = "";
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                Debug.Log("INICIALIZANDO MyLogs");

                if (Application.platform != RuntimePlatform.WindowsPlayer) // evito hacerlo en windows por que me dispara una aletra de antivirus solo pedir el directorio mydocuments
                {
                    docFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                    Debug.Log("DOC FOLDER OBTENIDO =" + docFolderPath);

                    var specialFolderPath = "";
                    specialFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    Debug.Log("SPECIAL FOLDER OBTENIDO =" + specialFolderPath);
                    if (docFolderPath == "" && specialFolderPath != "")
                    {
                        docFolderPath = specialFolderPath;
                        Debug.Log("ME QUEDO CON SPECIAL FOLDER COMO DOC FOLDER");
                    }
                    //if (docFolderPath == "")
                    //    docFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
                    //if (docFolderPath == "")
                    //    docFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic);
                    //if (docFolderPath == "")
                    //    docFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
                    //if (docFolderPath == "")
                    //docFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
                }
                else
                {
                    Debug.Log("ESTOY EN WINDOWS NO PIDO PATH MYDOCUMENTS NI PERSONAL POR QUE ME SALTA EL ANTIVIRUS CON ALERTAS DE MIERDA");
                }
                if (docFolderPath == "")
                {
                    Debug.Log("CONSTRUYENDO DOC FOLDER A PARTIR DE PERSISTENT DATA PATH");
                    docFolderPath = Application.persistentDataPath;
                    if (docFolderPath.Contains("AppData"))
                    {
                        var p = docFolderPath.ToCharArray();
                        var appdata = "/AppData/".ToCharArray();
                        int ai = 0;
                        for (int i = 0; i < p.Length; i++)
                        {
                            int ii = i;
                            ai = 0;
                            bool matches = false;
                            for (ii = i; ii < p.Length; ii++)
                            {
                                if (ai < appdata.Length)
                                {
                                    if (p[ii] == appdata[ai])
                                    {
                                        ai++;
                                        matches = true;
                                        if (ai >= appdata.Length)
                                            break;
                                    }
                                    else
                                    {
                                        matches = false;
                                        break;
                                    }
                                }
                            }
                            if (matches)
                            {
                                docFolderPath = docFolderPath.Substring(0, i);
                                if (1 == 2) // esto funciona pero luego me da error al intentar crear el directorio Moons Of Darsalon, no tiene sentido ninguno, pero al menos funciona creando el dir directamente en la caprta de usuario, puede que lo resuleva en un futuro por eso dejo esto aqui
                                {
                                    if (System.IO.Directory.Exists(docFolderPath + "/MyDocuments"))
                                        docFolderPath += "/MyDocuments";
                                    else if (System.IO.Directory.Exists(docFolderPath + "/My Documents"))
                                        docFolderPath += "/My Documents";
                                    else if (System.IO.Directory.Exists(docFolderPath + "/Documentos"))
                                        docFolderPath += "/Documentos";
                                    else if (System.IO.Directory.Exists(docFolderPath + "/Documents"))
                                        docFolderPath += "/Documents";
                                    else if (System.IO.Directory.Exists(docFolderPath + "/Mis Documentos"))
                                        docFolderPath += "/Mis Documentos";
                                    else if (System.IO.Directory.Exists(docFolderPath + "/MisDocumentos"))
                                        docFolderPath += "/MisDocumentos";
                                }
                                break;
                            }
                        }
                        Debug.Log("DESPUES DE LA RECONSTRUCCION EL RESULTADO ES " + docFolderPath);

                    }
                }
                //docFolderPath = Application.persistentDataPath;
                if (docFolderPath != "") // a veces me viene vacio el nombre !!!
                {
                    //if f(docFolderPath.EndsWith("_Data"))
                    //docFolderPath = System.IO.Path.Combine(docFolderPath, "Moons Of Darsalon");
                    if (docFolderPath == Application.persistentDataPath)
                    {
                        docFolderPath += "/Output";
                        Debug.Log("ERA PERSISTENT DATA PATH ASI QUE AÑADO /Output Y QUEDA: " + docFolderPath);
                    }
                    else
                    {
                        docFolderPath += "/Moons Of Darsalon";
                        Debug.Log("NO SE DE DONDE HE OBTENIDO EL PATH PERO AÑADO '/Moons Of Darsalon' Y QUEDA: " + docFolderPath);
                    }
                    Debug.Log("DOC FOLDER PATH DEFINIDO EN " + docFolderPath);
                    if (!System.IO.Directory.Exists(docFolderPath))
                    {
                        Debug.Log("NO EXISTIA CARPETA MOD EN EL DOC FOLDER, CREANDO");
                        System.IO.Directory.CreateDirectory(docFolderPath);
                    }
                    //                  docFolderPath = System.IO.Path.Combine(docFolderPath, docFolderName); // esto me crea la union con el Slash inverso ...WTF? "\"

                    Debug.Log("COMBINEDFOLDERPATH=" + docFolderPath);

                    if (!System.IO.Directory.Exists(docFolderPath))
                    {
                        Debug.Log("EL DIRECTORIO " + docFolderPath + " NO EXISTE, CREANDOLO"); // ojo ! POSIBLE ISOLATED DATA EXCEPTION QUE NO ME DEJA CREAR DIRECTORIO !!!
                        System.IO.Directory.CreateDirectory(docFolderPath);
                    }
                }
                else
                {
                    Debug.Log("DOCFOLDERPATH ME HA VENIDO NULO? WTF?");
                }
            }
            resolutionsLogPath = GetResolutionsLogPath();
            if (resolutionsLogPath != "")
            {
                if (System.IO.File.Exists(resolutionsLogPath))
                    System.IO.File.Delete(resolutionsLogPath);
            }
            executionOrderPath = GetExecutionOrderLogPath();
            if (executionOrderPath != "")
            {
                if (System.IO.File.Exists(executionOrderPath))
                    System.IO.File.Delete(executionOrderPath);
            }
            initialised = true;
            Resolutions(SystemInfo.deviceUniqueIdentifier + "\n");
        }
    }
    static void CombinePath(ref string path1, ref string path2)
    {
        if (path2 != "")
            path1 = path1 + Path.DirectorySeparatorChar + path2;
    }

    public static string GetResolutionsLogPath()
    {
        return PathThisToDocFolder(resolutionsLogName);
    }
    public static string GetExecutionOrderLogPath()
    {
        return PathThisToDocFolder(executionOrderName);
    }
    public static string PathThisToDocFolder(string fileName)
    {
        if (docFolderPath != "")
        {
            //          return System.IO.Path.Combine(docFolderPath, fileName);  // esto me crea la union con el Slash inverso ...WTF? "\"
            string newPath = docFolderPath;
            CombinePath(ref newPath, ref fileName);
            return newPath;
        }
        else
            return "";
    }
    static string execBuffer = "";
    private static int lastExecutionOrderBufferFrame;
    public static void AddToExecutionOrderBuffer(string line) { // ojo , si no se imprime con PrintExecutionOrderBuffer, se va generando barsura de modo acumulativo !!!
        float ms = (float)KuchoTime.frame_SW.ElapsedTicks / 10000f;
        int currentFrame = KuchoTime.frameCountAtFixedUpdate;
        if (currentFrame != lastExecutionOrderBufferFrame)
            execBuffer += "\n";
        execBuffer += "FRM:" + currentFrame + " Ms:" + ms.ToString("0.000") + " " +line + "\n";
        lastExecutionOrderBufferFrame = currentFrame;
    }
    public static void PrintExecutionOrderBuffer()
    {
        if (executionOrderPath != "")
        {
            System.IO.File.AppendAllText(executionOrderPath, execBuffer + "\n");
        }
        execBuffer = "";
    }
    public static void Resolutions(string line)
    {
        Resolutions(line, false);
    }
    public static void Resolutions(string line, bool time)
    {
        if (resolutionsLogPath != "")
        {
            string now = "";
            if (time)
                now = GetTime() + ":";
            System.IO.File.AppendAllText(resolutionsLogPath, now + line + "\n");
        }
    }
    /// <summary>
    /// Imprime en ambos logs, el mio y el de unity
    /// </summary>
    /// <param name="line">Line.</param>
    public static void ResolutionsAndUnityConsole(string line)
    {
        Debug.Log(line);
        Resolutions(line);
    }
    public static string GetTime()
    {
        return System.DateTime.Now.ToString();
    }

    public static void PrintMaterialDataToResolutions(string header, Material mat)
    {
        Resolutions(header + " MAT:" + mat + " SHADER:" + mat.shader + " MAIN TEX:" + mat.mainTexture);
    }
}
public static class StringHelper
{
    public enum CharType {Number, Letter, Symbol, Unknown}; 
    
     /// <summary>
     /// CREO QUE LA CREÉ PARA NO GENERAR BASURA PERO CREA 4K DE BASURA
     /// </summary>
     public static bool Compare(ref string s1, ref string s2)
    {
        Profiler.BeginSample("COMPARE STRINGS");
        if (s1.Length != s2.Length)
            return false;
        Profiler.EndSample();
        int l = s1.Length;
        for (int i = 0; i < l; i++)
        {
            Profiler.BeginSample("COMPARE STRINGS 2");
            if (s1[i] != s2[i])
                return false;
            Profiler.EndSample();
        }
        return true;
    }
     public static bool Compare(char[] s1, char[] s2)
     {
         Profiler.BeginSample("COMPARE CHARS");
         if (s1.Length != s2.Length)
             return false;
         Profiler.EndSample();
         int l = s1.Length;
         for (int i = 0; i < l; i++)
         {
             Profiler.BeginSample("COMPARE CHARS 2");
             if (s1[i] != s2[i])
                 return false;
             Profiler.EndSample();
         }
         return true;
     }

    public static Char spaceChar = (Char)32;
    public static Char slashChar = (Char)47;
    public static Char deleteChar = (Char)127;

    public static string FromInitialCapsToAllCapsWithSpaces(string s) // todo implementar en moons of darsalon MOD
    {
        if (s == "NONE") // chapu pero funciona
            return s;

        char[] c = s.ToCharArray();
        string result = "";
        CharType previousType = CharType.Unknown;
        bool previousWasSpace = false;
        for (int i = 0; i < c.Length; i++)
        {

            if (Char.IsNumber(c[i]))
            {
                if (previousType == CharType.Letter && i > 0)
                {
                    if (!previousWasSpace && previousType != CharType.Symbol)
                         result += " ";
                }
                previousType = CharType.Number;
            }
            else if (Char.IsLetter(c[i]))
            {
                if (Char.IsUpper(c[i]) && i > 0)
                {
                    if (!previousWasSpace && previousType != CharType.Symbol)
                        result += " ";
                }
                previousType = CharType.Number;
            }
            else if (Char.IsSymbol(c[i]) || (c[i] >= 33 && c[i] <= 47) || (c[i] >= 58 && c[i] <= 64) || (c[i] >= 91 && c[i] <= 96) || (c[i] >= 123))
            {
                previousType = CharType.Symbol;
            }
            else
            {
                previousType = CharType.Unknown;
            }

            if (c[i] == spaceChar)
                previousWasSpace = true;
            else
                previousWasSpace = false;
                
            
            result += c[i].ToString().ToUpper();
        }
        return result;
    }

    public static string RemoveStartAndEndSpacesAndFixOsxTerminalPath(string s)
    {
        if (s == null)
            return "";
        while (s.EndsWith(" "))
            s = s.Substring(0, s.Length - 1);
        while (s.StartsWith(" "))
            s = s.Substring(1, s.Length - 1);
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            if (s.Contains("\\"))
            {
                var chars = s.ToCharArray();
                List<char> newChars = new List<char>();
                for (int i = 0; i < chars.Length; i++)
                {
                    if (chars[i] == 92) // && i < chars.Length && chars[i + 1] == 32)
                    {
                    }
                    else
                    {
                        newChars.Add(chars[i]);
                    }
                }

                var a = newChars.ToArray();
                string result = new string(a);
                return result;
            }
        }

        return s;
    }
    
    public static Char minusThanChar = (char) 60;
    public static Char biggerThanChar = (char) 62;
/// <summary>
/// NO PROBADA
/// </summary>
/// <param name="s"></param>
/// <returns></returns>
    public static string ExtractStringBetweenLessThanBiggerThan(string s)
    {
        var sa = s.ToCharArray();
        int start = 0;
        int length = 0;
        bool startFound = false;
        bool endFound = false;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == minusThanChar)
            {
                start = i + 1;
                startFound = true;
                break;
            }
        }

        if (startFound)
        {
            for (int i = start; i < s.Length; i++)
            {
                if (s[i] == biggerThanChar)
                {
                    endFound = true;
                    break;
                }
                else
                {
                    length++;
                }
            }
        }

        if (startFound && endFound)
        {
            return s.Substring(start, length);
        }
        else
        {
            return "";
        }

    }

    public static int ExtractNumber(string str)
    {
        string b = "";
        for (int i=0; i< str.Length; i++)
        {
            if (Char.IsDigit(str[i]))
                b += str[i];
        }

        if (b.Length>0)
            return int.Parse(b);

        return 0;
    }

    public static bool HasSpaces(string str)
    {
        var a = str.ToCharArray();
        foreach(char c in a)
            if (c == spaceChar)
                return true;
        return false;
    }
    
    public static string GetS_IfPlural(int n)
    {
        if (n > 1)
            return "S";
        return "";
    }
    public static string GetTextAfterLastSlash(string text)
    {
        var t = text.ToCharArray();
        int slashPos = GetPosOfLastSlash(t);
        slashPos += 1;
        if (slashPos >= 0)
        {
            string afterSlashText = text.Substring(slashPos, text.Length - slashPos);

            return afterSlashText;
        }
        else
        {
            return "ERROR";
        }
    }
        
    public static int GetPosOfLastSlash(char[] t)
    {
        for (int i = t.Length-1; i >= 0; i--)
        {
            if (t[i] == StringHelper.slashChar)
                return i;
        }
        return 0;
    }
}
public static class ColorHelper
{
    public static void SetAlpha(SpriteRenderer spr, float alpha)
    {
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, alpha);
    }
    public static void SetAlpha(Renderer rend, float alpha)
    {
        var mat = rend.material;
        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
    }
    public static void SetAlpha(SWizBaseSprite spr, float alpha)
    {
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, alpha);
    }
    public static void SetAlpha(SWizSprite spr, float alpha)
    {
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, alpha);
    }
    public static void SetAlpha(SWizTextMesh tm, float alpha)
    {
        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, alpha);
    }
    public static void SetAlphaOnGradient(SWizTextMesh tm, float alpha)
    {
        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, alpha);
        tm.color2 = new Color(tm.color2.r, tm.color2.g, tm.color2.b, alpha);
    }
    public static void SetAlpha(Material mat, float alpha)
    {
        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
    }
    /*
    public static void SetAlpha(Image img, float alpha)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
    }
    */
    public static void SetAlpha(Light2D.CustomSprite spr, float alpha) 
    {
        spr.Color = new Color(spr.Color.r, spr.Color.g, spr.Color.b, alpha);
    }
    public static float Luma(Color c)
    {
        return 0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b;
    }
    public static bool CompareColor(Color32 c1, Color32 c2)
    {
        if (c1.r != c2.r || c1.g != c2.g || c1.b != c2.b || c1.a != c2.a)
            return false;
        return true;
    }
    public static bool CompareColorNoAlpha(Color32 c1, Color32 c2)
    {
        if (c1.r != c2.r || c1.g != c2.g || c1.b != c2.b)
            return false;
        return true;
    }
    public static bool CompareColor(Color c1, Color c2)
    {
        if (c1.r != c2.r || c1.g != c2.g || c1.b != c2.b || c1.a != c2.a)
            return false;
        return true;
    }
    public static bool CompareColorNoAlpha(Color c1, Color c2)
    {
        if (c1.r != c2.r || c1.g != c2.g || c1.b != c2.b)
            return false;
        return true;
    }
    public static Color32 MultNoAlpha(Color32 c, float mult)
    {
        float f = (float)c.r / 255f;
        f *= mult;
        f *= 255;
        c.r *= (byte)f;

        f = (float)c.g / 255f;
        f *= mult;
        f *= 255;
        c.g *= (byte)f;

        f = (float)c.b / 255f;
        f *= mult;
        f *= 255;
        c.b *= (byte)f;

        return c;
    }

    public static Color ShiftHSV_Contrast_Fast(Color c, float _HueShift, float _Sat, float _Val, float _Cont)
    {
        Color result;

        float cr = c.r;
        float cg = c.g;
        float cb = c.b;

        float rr = cr;
        float rg = cg;
        float rb = cb;


        float vsu = _Val * _Sat * Mathf.Cos(_HueShift * 0.0174532925f); //3.14159265/180);
        float vsw = _Val * _Sat * Mathf.Sin(_HueShift * 0.0174532925f); //3.14159265/180);
        float mult_1 = 0.299f * _Val;
        float mult_2 = 0.587f * _Val;
        float mult_3 = 0.114f * _Val;
        float mult_4 = 0.114f * vsu;
        float mult_5 = 0.588f * vsu;

        rr = (mult_1 + .701f * vsu + .168f * vsw) * cr
                 + (mult_2 - mult_5 + .330f * vsw) * cg
                 + (mult_3 - mult_4 - .497f * vsw) * cb;

        rg = (mult_1 - .299f * vsu - .328f * vsw) * cr
                 + (mult_2 + .413f * vsu + .035f * vsw) * cg
                 + (mult_3 - mult_4 + .292f * vsw) * cb;

        rb = (mult_1 - .3f * vsu + 1.25f * vsw) * cr
                 + (mult_2 - mult_5 - 1.05f * vsw) * c.g
                 + (mult_3 + .886f * vsu - .203f * vsw) * cb;

        result.r = (rr - 0.5f) * (_Cont) + 0.5f;
        result.g = (rg - 0.5f) * (_Cont) + 0.5f;
        result.b = (rb - 0.5f) * (_Cont) + 0.5f;
        result.a = c.a;

        return result;
    }
    public static Color GetAverageColor(ref Color[] colors, float threshold)
    {
        int total = 1;
        float r = 0;
        float g = 0;
        float b = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].grayscale > threshold)
            {
                r += colors[i].r;
                g += colors[i].g;
                b += colors[i].b;
                total++;
            }
        }
        return new Color(r / total, g / total, b / total, 1);
    }
    public static Color GetAverageColor(ref Color[,] colors, float threshold)
    {
        int total = 1;
        int xLength = colors.GetLength(0);
        int yLength = colors.GetLength(1);
        float r = 0;
        float g = 0;
        float b = 0;
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                if (colors[x, y].grayscale >= threshold)
                {
                    r += colors[x, y].r;
                    g += colors[x, y].g;
                    b += colors[x, y].b;
                    total++;
                }
            }
        }
        return new Color(r / total, g / total, b / total, 1);
    }
    public static Color GetAverageColor(ref Color[,] colors, float threshold, float centerMatters)
    {
        float total = 1;
        int xLength = colors.GetLength(0);
        int yLength = colors.GetLength(1);
        int xCenter = xLength / 2;
        int yCenter = yLength / 2;
        float xPower = 0;
        float yPower = 0;
        float caracol = 1 - centerMatters;
        float r = 0;
        float g = 0;
        float b = 0;
        for (int x = 0; x < xLength; x++)
        {
            xPower = xCenter - Mathf.Abs(x - xCenter);
            xPower = Mathf.InverseLerp(0, xCenter, xPower);
            for (int y = 0; y < yLength; y++)
            {
                Color c = colors[x, y];
                if (c.grayscale >= threshold)
                {
                    yPower = yCenter - Mathf.Abs(y - yCenter);
                    yPower = Mathf.InverseLerp(0, yCenter, yPower);

                    float power = xPower * yPower;
                    power += caracol;
                    //power = Mathf.Clamp01(power);
                    r += c.r * power;
                    g += c.g * power;
                    b += c.b * power;
                    total += power;
                }
            }
        }
        return new Color(r / total, g / total, b / total, 1);
    }
}

//public static class SceneHelper{
//    public static void FindObjectOfType(){
//        var scene = SceneManager.GetActiveScene();
//        GameObject[] roots = scene.GetRootGameObjects();
//        foreach (GameObject go in roots)
//        {
//            
//        }
//    }
//}
public static class KuchoHelper
{
    private static char slash = (char)47;
    private static char backslash = (char)92;
    public static string FixDirectorySeparators(string path)
    {
        var chars = path.ToCharArray();
        for(int i = 0; i < chars.Length; i ++)
        {
            if (chars[i] == slash || chars[i] == backslash)
                chars[i] = Path.DirectorySeparatorChar;
        }
        //return chars.ToString(); // funcionaba hasta que cambié a unity 2022
        return string.Join("", chars);
    }
    public static bool IsUserLevel()
    {
        if (SceneManager.GetActiveScene().name.ToUpper().Contains("LEVEL USER"))
            return true;
        return false;
    }

    public static GameObject LoadGameCoreFromResourcesFolder()
    {
        //return Resources.Load("GameParts/GameCore") as GameObject;
        #if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<GameObject>("Assets/-KUCHO/InBuildStuff/Resources/GameParts/GameCore.prefab");
        #else
        return null;
        #endif
    }
    public static string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static void SendEmail(string address, string subject, string body)
    {
        Application.OpenURL("mailto:" + address + "?subject=" + MyEscapeURL(subject) + "&body=" + MyEscapeURL(body));
    }
    static string MyEscapeURL(string URL)
    {
        return WWW.EscapeURL(URL).Replace("+", "%20");
    }
    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }
    static public string GetCombinedDataPathForReadWriteFiles(string fileName)
    {
        if (Application.isEditor)
        {
            return System.IO.Path.Combine(Application.dataPath, fileName);
        }
        else
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return System.IO.Path.Combine(Application.persistentDataPath, fileName);
            }
            if (Application.platform == RuntimePlatform.OSXPlayer) // osx me deja hjacer cambios en este directiorio asi que mejor guardo mis cosas aqui
            {
                return System.IO.Path.Combine(Application.dataPath + "/Resources/Data", fileName);
            }
        }
        return "";
    }
    static public string GetCombinedDataPathForReadOnlyFiles(string fileName)
    {
        if (Application.isEditor)
        {
            return System.IO.Path.Combine(Application.dataPath, fileName);
        }
        else
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return System.IO.Path.Combine(Application.dataPath, fileName);
            }
            if (Application.platform == RuntimePlatform.OSXPlayer) // osx me deja hjacer cambios en este directiorio asi que mejor guardo mis cosas aqui
            {
                return System.IO.Path.Combine(Application.dataPath + "/Resources/Data", fileName);
            }
        }
        return "";
    }
    public static bool ThreadIsAlive(Thread t)
    {
        if (t == null)
            return false;
        if ((t.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted)) == 0)
        {
            return true;
        }
        return false;
    }
    public static bool ThreadIsRunning(Thread t)
    {
        if (t == null)
            return false;
        if ((t.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted | ThreadState.WaitSleepJoin | ThreadState.Aborted | ThreadState.Suspended)) == 0)
        {
            return true;
        }
        return false;
    }
    public static void DeletePlayerPrefs() {
        Debug.Log(" BORRANDO PLAYER PREFS ");
        PlayerPrefs.DeleteAll();
        KuchoHelper.SavePlayerPrefsNowIncludingTime();
    }
    public static void SavePlayerPrefsNowIncludingTime()
    {
        PlayerPrefs.SetString("_Last Edit Time", System.DateTime.Now.ToString());
        PlayerPrefs.Save();
    }
    public static float AvoidBadPixelOffset = 0.25f;
   
    /// <summary>
    /// hace una aproximacion a la magnitud de un vextor2, tiene un error que se mantiene bajo hasta que la diferencia entre x e y empieza a ser muy alta (a partir de 200 el error es 10 e incrementando)
    /// </summary>
    /// <returns>The velocity magnitude.</returns>
    /// <param name="v">V.</param>
    public static float CheapMagnitude(Vector2 v)
    {
        Vector2 absV = new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        float big = Mathf.Max(absV.x, absV.y);
        float vDiff = Mathf.Abs(absV.x - absV.y);
        if (big != 0)// prevencion de division por cero que da Nan
        {
            vDiff = vDiff / big;
            float finalMult = 0.70710675f - (vDiff * 0.24f);
            return (absV.x + absV.y) * finalMult;
        }
        return 0;
    }


    public static int lookAtAlternateFlag = 1;
    public static float GetSignFromSidesAlgorythm(SidesAlgorythm s, int chances, float sign)
    {
        switch (s)
        {
            case (SidesAlgorythm.Random):
                if (chances > UnityEngine.Random.Range(0, 100)) sign = 1f; else sign = -1f;
                break;
            case (SidesAlgorythm.Alternate):
                sign = sign * -1f;
                break;
            case (SidesAlgorythm.Right_Up_Pos):
                sign = 1f;
                break;
            case (SidesAlgorythm.Left_Down_Neg):
                sign = -1f;
                break;
        }
        return sign;
    }

    public static int GetRightFromLookAt(Vector3 position, LookAt lookAt, Transform target)
    {
        int r = 0;
        switch (lookAt)
        {
            case (LookAt.ParentLookingAt):
                r = 1;
                Debug.LogError("------ERROR! NO DEBERIA LLAMAR A GETRIGHTFROMLOOKINGAT CON PARENTLOOKINGAT PONGO 1 PARA QUE NO PETE, PERO MAL");
                break;
            case (LookAt.Target):
                if (target)
                {
                    if (position.x > target.position.x) r = -1;
                    else r = 1;
                }
                else r = 1;
                break;
            case (LookAt.Left):
                r = -1;
                break;
            case (LookAt.Right):
                r = 1;
                break;
            case (LookAt.Random):
                if (UnityEngine.Random.Range(-100, 100) > 0) r = 1;
                else r = -1;
                break;
            case (LookAt.Alternate):
                lookAtAlternateFlag *= -1;
                r = lookAtAlternateFlag;
                break;
        }
        if (r == 0)
        {
            Debug.LogError("------ERROR! RIGHT FROM LOOK AT VALE CERO . POS= " + position + " LOOKAT= " + lookAt);
            Debug.Break(); Debug.Log("GET RIGHT FROM LOOK AT !!!!!!!!!!!!!!!!!!!!!!!!!");
        }
        return r;
    }
    public static Color DarkenColor(Color color, float dark)
    {
        color.r -= dark;
        color.g -= dark;
        color.b -= dark;
        return color;
    }
    
    public static Transform FindChildWithTagRecursive(string tag, Transform t)
    {
        Transform[] allChildren = t.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.tag == tag) return child;
        }
        return null;
    }
    // calcula la diferencia entre los dos valores independientemente de si uno es mayor que otro, siempre resta el menor del mayor
    public static float GetDifference(float a, float b)
    {
        if (a > b) return a - b;
        else return b - a;
    }
    public static float GetDecimals(float f)
    {
        int i = (int)f; // trunco
        return f - i;
    }
    public static int GetHowManyDecimals(float a)
    {
        int count;
        for (count = 0; count < 50; count++)
        {
            if (a == Mathf.Round(a)) return count;
            a *= 10;
        }
        return count;
    }
    public static bool Intersect(Rect r1, Rect r2)
    {
        return !(r2.xMin > r1.xMax ||
            r2.xMax < r1.xMin ||
            r2.yMin > r1.yMax ||
            r2.yMax < r1.yMin);
    }
    public static bool Intersect(Rect r, Bounds b)
    {
        return !(b.min.x > r.xMax ||
            b.max.x < r.xMin ||
            b.min.y > r.yMax ||
            b.max.y < r.yMin);
    }
    /// <summary>
    /// No Incluye Los Limites.
    /// </summary>
    /// <param name=_GreyColor>P.</param>
    /// <param name="r">The red component.</param>
    public static bool Intersect(Vector2 p, Rect r)
    {
        if (p.x < r.xMin ||
            p.x > r.xMax ||
            p.y < r.yMin ||
            p.y > r.yMax) return false;
        else return true;
    }
    public static bool IntersectDisregardingHeight(Vector2 p, Rect r)
    {
        if (p.x < r.xMin ||
            p.x > r.xMax ||
            p.y < r.yMin) return false;
        else return true;
    }
    /// <summary>
    /// Incluye los limites de Rect.
    /// </summary>
    /// <param name=_GreyColor>P.</param>
    /// <param name="r">The red component.</param>
    public static bool IntersectIncludingLimits(Vector2 p, Rect r)
    {
        if (p.x <= r.xMin ||
            p.x >= r.xMax ||
            p.y <= r.yMin ||
            p.y >= r.yMax) return false;
        else return true;
    }
    public static bool IntersectIncludingLimits(Vector2 p, Bounds r)
    {
        if (p.x <= r.min.x ||
            p.x >= r.max.x ||
            p.y <= r.min.y ||
            p.y >= r.max.y) return false;
        else return true;
    }
    public static Transform FindChildWithLayer(int layer, Transform t)
    {
        foreach (Transform child in t)
        {
            if (child.gameObject.layer == layer) return child;
        }
        return null;
    }
    public static Transform FindChildWithLayerRecursive(int layer, Transform t)
    {
        Transform[] allChildren = t.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.layer == layer) return child;
        }
        return null;
    }
    public static Transform FindChildWithTag(string tag, Transform t)
    {
        foreach (Transform child in t)
        {
            if (child.gameObject.tag == tag) return child;
        }
        return null;
    }    
    public static Transform[] FindChildrenWichContainsNameRecursive(string name, Transform t)
    {
        Transform[] allChildren = t.GetComponentsInChildren<Transform>(true);
        List<Transform> result = new List<Transform>(); 
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.name.Contains(name))
                result.Add(child);
        }
        return result.ToArray();
    }
    public static Transform FindFirstChildContainingNameRecursive(string[] name, Transform t, bool ignoreCase)//TODO deberia ser recursive
    {
        Transform[] allChildren = t.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            foreach (string s in name) 
            {
                string mustInclude;
                string childName;
                if (ignoreCase)
                {
                    mustInclude = s.ToUpper(); 
                    childName = child.gameObject.name.ToUpper();
                }
                else
                {
                    mustInclude = s;
                    childName = child.gameObject.name;
                }

                if (childName.Contains(mustInclude))
                {
                    return child;
                }
            }
        }
        return null;
    }   
    public static Transform FindFirstChildContainingName(string[] name, Transform t, bool ignoreCase)//TODO deberia ser recursive
    {
        if (t)
        {
            foreach (Transform child in t)
            {
                foreach (string s in name)
                {
                    string mustInclude;
                    string childName;
                    if (ignoreCase)
                    {
                        mustInclude = s.ToUpper();
                        childName = child.gameObject.name.ToUpper();
                    }
                    else
                    {
                        mustInclude = s;
                        childName = child.gameObject.name;
                    }

                    if (childName.Contains(mustInclude))
                    {
                        return child;
                    }
                }
            }
        }

        return null;
    }   
    /// <summary>
    /// Incluye Gos Desactivados
    /// </summary>
    public static Transform FindChildWithNameRecursive(string name, Transform t)
    {
        Transform[] allChildren = t.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.name == name) return child;
        }
        return null;
    }
    public static Transform FindFirstChildWithName(string name, Transform t)
    {
        foreach (Transform child in t)
        {
            if (child.gameObject.name == name) return child;
        }
        return null;
    }
    public static Transform FindFirstChildStartingWithName(string name, Transform t)
    {
        foreach (Transform child in t)
        {
            if (child.gameObject.name.StartsWith(name))
                return child;
        }
        return null;
    }

    public static Transform FindFirstChildWith3DRenderer(Transform t)
    {
        var allRenderers = t.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in allRenderers)
        {
            var meshfilter = r.GetComponent<MeshFilter>();

            if (meshfilter)
            {
                if (meshfilter.sharedMesh)
                {
                    if (meshfilter.sharedMesh.vertices.Length > 20) // menos de 20 vertices deberia ser un sprite o lightsprite o a saber
                    {
                        var spr = r.GetComponent<SWizSprite>();
                        if (!spr)
                        {
                            return r.transform;
                        }
                    }
                }
            }
        }
        return null;
    }
    public static Renderer FindBiggest3DRenderer(Transform t, float zScaleThreshold)
    {
        var allRenderers = t.GetComponentsInChildren<Renderer>();
        float bigestSize = float.MinValue;
        int winnerIndex = 0;
        if (allRenderers.Length > 0)
        {
            for (int i = 0; i < allRenderers.Length; i++)
            {
                var r = allRenderers[i];
                if (r.enabled && r.transform.localScale.z > zScaleThreshold) //evito modelos 2D
                {
                    var size = r.bounds.size.x * r.bounds.size.y * r.bounds.size.z;
                    if (size > bigestSize)
                    {
                        bigestSize = size;
                        winnerIndex = i;
                    }
                }
            }
        }

        if (bigestSize > 0)
            return allRenderers[winnerIndex];
        else
            return null;
    }

    public static bool IsInTagList(Collider2D col, string[] tags)
    {
        if (tags == null || col == null)
            return false;
        for (int i = 0; i < tags.Length; i++)
        {
            if (col.CompareTag(tags[i])) return true;
        }
        return false;
    }

    public static bool IsInArray(Collider2D col, Collider2D[] colArray)
    {
        foreach (Collider2D c in colArray)
        {
            if (c == col)
                return true;
        }
        return false;
    }
    public static bool IsInList(Collider2D col, List<Collider2D> colList)
    {
        foreach (Collider2D c in colList)
        {
            if (c == col)
                return true;
        }
        return false;
    }

    public static GameObject[] FindGameObjectsWithLayer(int layer)
    {
        GameObject[] goArray = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> goList = new List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }
    public static void FindGameObjectsWithLayerAndMeshRederer(ref List<GameObject> goList, int layer)
    {
        GameObject[] goArray = GameObject.FindObjectsOfType<GameObject>();
        goList = new List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer && goArray[i].GetComponent<MeshRenderer>())
            {
                goList.Add(goArray[i]);
            }
        }
    }
    public static Texture2D[] FindTexturesWithLayer(int layer)
    {
        GameObject[] goArray = GameObject.FindObjectsOfType<GameObject>();
        List<Texture2D> goList = new List<Texture2D>();
        Texture2D texture = null;
        SpriteRenderer sprR = null;
        for (int i = 0; i < goArray.Length; i++)
        {
            Renderer _renderer = goArray[i].GetComponent<Renderer>();
            if (_renderer && _renderer.material.mainTexture)
            {
                texture = _renderer.material.mainTexture as Texture2D;
            }
            else
            {
                sprR = goArray[i].GetComponent<SpriteRenderer>();
            }

            if (sprR && sprR.sprite) texture = sprR.sprite.texture;

            if (goArray[i].layer == layer && texture)
            {
                goList.Add(texture);
            }
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }
    public static void AssignNewMaterialKeepTexture(Renderer rend, Material mat) // TODO OJO! genera 40 bytes de basuda por acceder a material, seguro que no quiero acceder a sharedMaterial?
    {
        if (!rend || !rend.sharedMaterial)
        {
            return;
        }
        Texture tex = rend.sharedMaterial.mainTexture;
        if (tex == null)
            Debug.LogError("INTENTANDO CAMBIAR MATERIAL Y CONSERVAR TEXTURE PERO LA TEXTURE ES NULL");
        rend.sharedMaterial = mat;
        rend.sharedMaterial.mainTexture = tex;
    }
    public static float GetUsefullRotation(float rot)
    { // dada una rotacion desde -infinito a +infinito (que incluye numero de vueltas completas dadas) devuelve una rotacion equivalente de -180 a 180 sin vueltas
        // eliminamos el numero de vueltas completas que ha dado
        float a = rot / 360f;
        int turns; // para truncar
        turns = (int)a; // tuerns tiene el valor entero ( las veces que tengo que quitarle 360 )
        float usefullRot = rot - (360 * turns); // esta seria una forma de hacerlo

        //esta es otra forma de hacerlo
        //var decimals = a - b; // a ahora tiene los decimales ( si los multiplico por 360 tengo los grados sin vueltas)
        //myRotation = decimals * 360;

        // aun queda esto
        if (usefullRot > 180)
            usefullRot -= 360;
        else if (usefullRot < -180)
            usefullRot += 360;
        return usefullRot;
    }
    public static bool IsLayerInLayerMask(int layer, int layerMask)
    {
        /// <summary>
        /// Checks if a GameObject is in a LayerMask
        /// </summary>
        /// <param name="obj">GameObject to test</param>
        /// <param name="layerMask">LayerMask with all the layers to test against</param>
        /// <returns>True if in any of the layers in the LayerMask</returns>
        // Convert the layer to a bitfield for comparison
        int objLayerMask = (1 << layer);
        int result = (layerMask & objLayerMask);
        if (result == 0)
            return false;
        else
            return true;
    }
    public static bool AreLayersInLayerMask(int[] layers, int layerCount, int layerMask)
    {
        for (int i = 0; i < layerCount; i++)
        {
            int layer = layers[i];
            int objLayerMask = (1 << layer);
            int result = (layerMask & objLayerMask);
            if (result != 0)
                return true;
        }
        return false;
    }
    public static void SetHSV(Renderer rend, Vector3 hsv)
    {
        Material mat = rend.material;
        if (mat.HasProperty("_HueShift")) mat.SetFloat("_HueShift", hsv.x);
        if (mat.HasProperty("_Sat")) mat.SetFloat("_Sat", hsv.y);
        if (mat.HasProperty("_Val")) mat.SetFloat("_Val", hsv.z);
    }
    public static int AbsInt(int input)
    {
        return (input + (input >> 31)) ^ (input >> 31);
    }
    public static float StepMinus1And1(float value)
    {
        if (value > 0) return 1f;
        else return -1;
    }
    // esto solo funciona para incrementos de +1  y -1
    public static void Wrap(ref int value, int inc, int min, int max)
    {
        value += inc;
        if (value > max)
            value = min;
        else if (value < min)
            value = max;
    }
    public static float Wrap(ref float value, float inc, float min, float max)
    {
        float target = value + inc;
        float result = target;
        float diff;
        //un poco chapu usar un while , no es eficiente en casos de wraps grandes
        while (result > max) // nos hemos pasado por arriba
        {
            diff = max - result;
            result = min - diff;
        }
        while (result < min) // nos hemos pasado por abajo
        {
            diff = result - min;
            result = max + diff;
        }
        value = result;
        return result;
    }
    public static int Clamp(int value, int min, int max)
    {
        if (value >= max) return max;
        else if (value <= min) return min;
        return value;
    }
    public static float ShaderModf(float x, ref float ip)
    {
        ip = (int)x;
        float dec = x - ip;
        return dec;
    }

    public static float ShaderStep(float a, float x)
    {
        if (x < a)
            return 0;
        else // x >= a
            return 1;
    }
    /// <summary>
    /// Clamps value inside Min Max.
    /// </summary>
    /// <returns><c>true</c>, if value was clamped, <c>false</c> otherwise.</returns>
    /// <param name="value">Value.</param>
    /// <param name="arrayLength">Array length.</param>
    public static bool Clamp(ref int value, int min, int max)
    {
        if (value >= max)
        {
            value = max;
            return true;
        }
        else if (value <= min)
        {
            value = min;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Clamps value inside Min Max.
    /// </summary>
    /// <returns><c>true</c>, if value was clamped, <c>false</c> otherwise.</returns>
    /// <param name="value">Value.</param>
    /// <param name="arrayLength">Array length.</param>
    public static bool Clamp(ref float value, float min, float max)
    {
        if (value > max)
        {
            value = max;
            return true;
        }
        else if (value < min)
        {
            value = min;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Clamps value inside ArrayLength.
    /// </summary>
    /// <returns><c>true</c>, if value was clamped, <c>false</c> otherwise.</returns>
    /// <param name="value">Value.</param>
    /// <param name="arrayLength">Array length.</param>
    public static bool ClampInsideArrayLength(ref int index, int arrayLength)
    {
        if (index >= arrayLength)
        {
            index = arrayLength - 1;
            return true;
        }
        if (index < 0)
        {
            index = 0;
            return true;
        }
        return false;
    }
    public static int ClampInsideArrayLength(int index, int arrayLength)
    {
        if (index >= arrayLength)
        {
            index = arrayLength - 1;
            return index;
        }
        if (index < 0)
        {
            index = 0;
            return index;
        }
        return index;
    }
    public static void ZeroIndexIfOutOfRange(ref int index, int length)
    {
        if (index >= length)
            index = 0;
    }
    /// <summary>
    /// Clamps value inside ArrayLength.
    /// </summary>
    /// <returns><c>true</c>, if index was wrapped, <c>false</c> otherwise.</returns>
    /// <param name="index"> index of the array.</param>
    /// <param name="inc">increment for index.</param>
    /// <param name="length">Array length.</param>
    public static bool IncAndWrapInsideArrayLength(ref int index, int inc, int length)
    {
        if (length > 0)
        {
            index += inc;
            if (inc > 0 && index >= length)
            {
                index = 0;
                return true;
            }
            if (inc < 0 && index < 0)
            {
                index = length - 1;
                return true;
            }
            return false; // no ha habido wrap
        }
        else
        {
            index = 0;
            return true; // ha habido wrap? en realidad no , pero es que la tabla es de logitud 0
        }
    }
    public static bool WrapInsideArrayLengthIfOutOfrange(ref int index, int length)
    {
        if (length > 0)
        {
            if (index >= length)
            {
                index = 0;
                return true;
            }
            else if (index < 0)
            {
                index = length - 1;
                return true;
            }
            return false; // no ha habido wrap
        }
        else
        {
            index = 0;
            return true; // ha habido wrap? en realidad no , pero es que la tabla es de logitud 0
        }
    }
    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
    public static float AngleZ(this Vector2 angle) /// robado de light2D
    {
        if (angle == Constants.zero2) return 0;
        return Vector2.Angle(Vector2.up, angle)*Mathf.Sign(-angle.x);
    }
    public static AbsFloat GetAngleToTarget(Vector2 self, Vector2 target, Vector2 reference)
    {
        Vector2 delta = target - self;
        float absAngle = Vector2.Angle(reference, delta);
        Vector3 cross = Vector3.Cross(reference, delta);
        float signedAngle = absAngle * Mathf.Sign(cross.z);// es un truco para veriguar si el angulo entre los dos es negativo en realidad , si el valor de Z es negativo ...
        AbsFloat result = new AbsFloat(signedAngle);
        return result;
    }
    /// <summary>
    /// No busa en el mismo GameObject ! Empieza en el Padre.
    /// </summary>
    /// <returns>The component in parent including inactive objects.</returns>
    /// <param name="go">Go.</param>
    public static Component GetComponentInParentIncludingInactiveObjects(GameObject go, System.Type type)
    {
        Component comp = null;
        Transform parent;
        parent = go.transform.parent;
        while (comp == null && parent != null)
        {
            comp = parent.GetComponent(type);
            parent = parent.parent;
        }
        return comp;
    }


    public static bool HasOneOfTheColliderSettingsTags(Collider2D col)
    {
        if (!col)
            return false;
        if (col.gameObject.CompareTag("ColliderSettings"))
            return true;
        if (col.gameObject.CompareTag("ColliderSettingsInParent"))
            return true;
        return false;
    }

    public static int GetIndexOf(object[] array, object obj)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == obj)
                return i;
        }
        return -1;
    }
    public static bool IsInArray(object[] array, string name)
    {
        for (int i = 0; i < array.Length; i++)
        {
            GameObject go = (GameObject)array[i];
            if (go.name == name)
                return true;
        }
        return false;
    }
    public static int GetIntFromBool(bool b)
    {
        if (b)
            return 1;
        return 0;
    }
    public static bool GetBoolFromInt(int i)
    {
        if (i > 0)
            return true;
        return false;
    }
    public static GameObject GetRandomGameObjectFromArray(GameObject[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length - 1)];
    }
    public static void SetParticleSystemEmmisionRateOverTimeMultiplier(ParticleSystem ps, float rateOverTimeMult)
    { //TODO Esto no seria mas sencillo pasarle un multiplier y asignarlo a rate.curveMultiplier ?
        ParticleSystem.EmissionModule emission = ps.emission; // yo diria que esta copiando , pero segun parece hace una referencia ya que segn gente en los foros, al asignar mas tarde valores a esta referencia , el modulo se modifica
        emission.rateOverTimeMultiplier = rateOverTimeMult; 
    }
    public static void SetParticleSystemEmmisionRate(ParticleSystem ps, float emissionRate)
    { //TODO Esto no seria mas sencillo pasarle un multiplier y asignarlo a rate.curveMultiplier ?
        ParticleSystem.EmissionModule emission = ps.emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.constantMax = emissionRate;
        emission.rate = rate;
    }
    public static bool IsInRange(RangeFloat range, float value)
    {
        if (value >= range.min && value <= range.max)
            return true;
        else
            return false;
    }
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        System.Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }
    public static bool SetTexture(GameObject go, Texture2D tex)
    {
        Renderer rend = go.GetComponent<Renderer>();
        if (rend)
        {
            rend.sharedMaterial.SetTexture("_MainTex", tex);
            return true;
        }
        return false;
    }
    public static bool SetTexture(GameObject go, RenderTexture tex)
    {
        Renderer rend = go.GetComponent<Renderer>();
        if (rend)
        {
            rend.material.SetTexture("_MainTex", tex);
            return true;
        }
        return false;
    }
    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }
    public static void SwitchMonoBehaviours(MonoBehaviour[] monos, bool onOff)
    {
        for (int i = 0; i < monos.Length; i++)
        {
            monos[i].enabled = onOff;
        }
    }
    public static void Destroy(UnityEngine.Object o)
    {
        if (Application.isPlaying)
            Destroy(o);
        else
            UnityEngine.Object.DestroyImmediate(o);
    }

    public static void InitSmartAnimationArray(SWizSpriteAnimator anim, SmartAnimation[] smartAnimation)
    {
        for (int n = 0; n < smartAnimation.Length; n++)
        {
            smartAnimation[n].GetClip(anim);
            if (smartAnimation[n].clip == null)
                Debug.LogError(" CLIP NO ENCONTRADO PARA " + smartAnimation[n].clipName);
        }
    }
    public static float AddOnlyIfSameSign(float src, float _add)
    {
        if (Mathf.Sign(src) == Mathf.Sign(_add)) // si el shift es positivo solo se aplica a input positivos e igual con negativos, eso evita que no se pueda agachar hacia abajo del todo al nadar
            return src + _add;
        else
            return src;
    }

    public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDelta)
    {
        current.x = Mathf.MoveTowards(current.x, target.x, maxDelta);
        current.y = Mathf.MoveTowards(current.y, target.y, maxDelta);
        return current;
    }
    public static bool IsBoxCollider2D(Collider2D col)
    {
        BoxCollider2D box = null;
        try
        {
            box = (BoxCollider2D)col;
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static bool IsCircleCollider2D(Collider2D col)
    {
        CircleCollider2D circle = null;
        try
        {
            circle = (CircleCollider2D)col;
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static BoxCollider2D CastToBoxCollider2D(Collider2D col)
    {
        try
        {
            BoxCollider2D box = (BoxCollider2D)col;
            return box;
        }
        catch
        {
            return null;
        }
    }
    public static CircleCollider2D CastToCircleCollider2D(Collider2D col)
    {
        try
        {
            CircleCollider2D circle = (CircleCollider2D)col;
            return circle;
        }
        catch
        {
            return null;
        }
    }

    static byte[] floatByte = new byte[4];
    public static void EncodeFloatToBytes(byte[] data, ref int index, float f)
    {
        floatByte = System.BitConverter.GetBytes(f);
        data[index] = floatByte[0];
        index++;
        data[index] = floatByte[1];
        index++;
        data[index] = floatByte[2];
        index++;
        data[index] = floatByte[3];
        index++;
    }

    #if UNITY_EDITOR
    public static Material FindAndLoadMaterial(string name)
    {
        var guids = AssetDatabase.FindAssets(name);
        
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<Material>(path);
        }
        return null;
    }
    #endif

}
