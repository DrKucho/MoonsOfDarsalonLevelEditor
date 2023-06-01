
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class KuchoTime : MonoBehaviour
{
    [Tooltip("GOOD FRAME IS EXECUTED EACH 1/60")]
    public bool identifyGoodAndBadFrames = false;

    public float maxFixedDeltaTimeMult = 4;

    public static float DisplayFrameRate;
    public static float DisplayFrameRateVsFpsRatio;
    public static bool fpsAndDisplayFrameRateAreEqual;
    public static float realTimeSinceStartUp;
    public static float realTimeSinceStartUp_old;
    public static float fixedUpdateRealTimeSinceStartUp;
    public static float realDeltaTime;
    public static float updateTime;
    public static float time; // clon , por comodidad
    public static float time_old; // el frame anterior
    public static float goodFrameTime; // el frame anterior
    public static float myDeltaTime;
    public static float expectedDeltaTime; // delta time ideal si el sistema va clavado a los fps del monitor
    public static float deltaTimeDiff; // diferencia entre expectedDeltaTime y deltaTima, sera 0 si son iguales
    public static float realDeltaTimeDiff; // diferencia entre expectedDeltaTime y deltaTima, sera 0 si son iguales

    public static float kuchoDeltaTime; // será 1 si deltaTimeDiff es 0, irá incrementando a medida que deltaTimeDiff crezca , si deltatimediff es el doble de lo esperado sera 2 etc
    public static float kuchoDeltaTimeReScaled;
    public static float kuchoDeltaTime60;
    public static float kuchoDeltaTime60ReScaled;
    public static float compensatorFor60fps;

    //public static float realDeltaTimeDiffMultiplier; // será 1 si deltaTimeDiff es 0, irá incrementando a medida que deltaTimeDiff crezca , si deltatimediff es el doble de lo esperado sera 2 etc
    public static float fixedUpdateTime;

    public static float fixedUpdateRate;

    //public static float speedCompensatorFor60Fps = 1; // valor proporcional que depende de los frames por segundo del monitor, para multiplicar las fuerzas y velocidades, en 60fps vale 1, en 120 vale 0.5f
    public static float displayFrameRateDivBy60 = 1;
    public static float fps = 60;

    public static float timeBetweenFrames; // en el caso de 60 frames es 0,016666

    //public static float SpeedCompensatorFor60FpsAndTimeScale = 1;
    public static float walkingMultiplier = 60;
    public static float levelTime;
    public static float levelStartTime;
    public static float onEnableTime;

    public static float unityDeltaTime;

    //public static float kuchoDeltaTime; // DisplayFrameRate/deltaTimeOne asi si deltaTimeOne es estabele a 1 , 60FPS = 1, 120FPS = 0.5 ...etc
    public static float unitySmoothDeltaTime;
    public static float timeScale = 1;
    public static float timeScale_old;
    public static int frameCount;
    public static int levelFrameCount;
    public static int frameCountAtFixedUpdate;
    public static int fixedUpdateCallCount;


    public static bool appIsPlaying; // tiuene en cuenta Application.isPlaying y EditorApplication.isPaused
    public static bool editorApplicationIsPaused;

    public static bool quitting;

    public static KuchoTime instance;


    static public bool _badFrame = false;

    static public bool badFrame
    {
        get { return _badFrame; }
        private set
        {
            _badFrame = value;
            _goodFrame = !value;
        }
    }

    static public bool _goodFrame = false;

    static public bool goodFrame
    {
        get { return _goodFrame; }
        private set
        {
            _goodFrame = value;
            _badFrame = !value;
        }
    }



    public static int badFrameCount;
    public static int goodFrameCount;
    public static float deltaTimeSumator;
    public static float lastDeltaTimeSumator;





    public static System.Diagnostics.Stopwatch frame_SW = new System.Diagnostics.Stopwatch();

    public static int consecutiveFixedUpdateCount = 0;


    static private int executedInFixedUpdateCall;
    static public bool trishaMode;


#if UNITY_EDITOR
    private void HandleOnPlayModeChanged()
    {
        if (EditorApplication.isPaused)
        {
            appIsPlaying = false;
        }
    }
#endif
}

/// <summary>
    /// para que pueda ser serializada, unity no serializa fechas....
    /// </summary>
    [System.Serializable]
    public class KuchoDate
    {
        public System.DateTime systemDate;
        public int year;
        public int month;
        public int day;
        public int hour;
        public int minute;
        public int second;
        
        public void FillSystemDateWithInts()
        {
            systemDate = new System.DateTime(year, month, day, hour, minute, second);
        }
        public void Set(System.DateTime d)
        {
            systemDate = d;
            this.year = d.Year;
            this.month = d.Month;
            this.day = d.Day;
            this.hour = d.Hour;
            this.minute = d.Minute;
            this.second = d.Second;
        }
        public void Set(int year, int month, int day, int hour, int minute, int second)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            FillSystemDateWithInts();
        }
        public KuchoDate()
        {
            Set(System.DateTime.Now);
        }
        public KuchoDate(int year, int month, int day, int hour, int minute, int second)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            FillSystemDateWithInts();
        }

        public KuchoDate(System.DateTime d)
        {
            systemDate = d;
            this.year = d.Year;
            this.month = d.Month;
            this.day = d.Day;
            this.hour = d.Hour;
            this.minute = d.Minute;
            this.second = d.Second;
        }

        public int CompareTo(KuchoDate otherDate)
        {
            return systemDate.CompareTo(otherDate.systemDate);
        }

    }

