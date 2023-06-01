using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;



public enum PhraseType
{
    None = -1,

    General = 100,

    //problems
    GrabbedByEnemy = 150, 
    NoVisibility = 200, 
    VisibilityNow = 210, 
    CantJumpDown = 300, 
    CantJumpUp = 400, 
    CantGoAhead = 500, 
    TooMuchWater = 600, 
    CantBreath = 700, 
    DoorWontOpenOutside = 800,
    DoorWontOpenInside = 900,
    VehicleBedNotEnabled = 910,
    VehicleDoorsClosed = 920,

    //target related
    OnTargetOutside = 1000,
    OnTargetInside = 1100, 
    ComonderJustGotVisible = 1200, 
    CommanderIsNotVisible = 1300,
    TargetJustTeleported = 1400,
    FollowFriendInsteadOfCommander = 1500,
    OnHome = 1600,
    EnemyDetected = 1700,
    OnPickup = 1710,

    //commands
    RoundCommandsDissabled = 1800,
    RoundCommandsAllowed = 1900,
    LeftOrRight = 2000,
    FollowMe = 2100,
    StayHere = 2200,
    Wait = 2300,
    UnFollow = 2400,
    Dissobey = 2500,
    PatienceOver = 2600,


    //misc
    Laugh = 2700,
    AfterCrash = 2800,
    AfterCrashBecauseGrabbedAndDropped = 2850,
    AfterPlayerHit = 2900,
    AfterEnemyHit = 3000,

    // ground Vehicle
    StillNotInTruck = 3100,
    InTruck = 3200,
    InTruckTooInclined = 3300,
    InTruckTooFast = 3400,

    // air vehicle
    StillNotInShip = 3425,
    InShip = 3450,

    //energy related
    FeelingGood = 3500,
    FeelingOk = 3600,
    FeelingNotSoGood = 3650,
    FeelingBad = 3700,
    FeelingAboutToDie = 3800,
}


[System.Serializable]
public class PhraseArray{
    public enum OtherBallonsBehabiour {Ignore, CancelIfOtherBallons, CancelIfOtherBallonWithSamePhraseType };
    [HideInInspector] public string name; // para mostrar en inspector
	public PhraseType type = PhraseType.General;
    public bool randomizeIndexAtStart = true;
    public bool timerIsGolbal;
    public float timer = 6f;
    float dynamicTimer; // se calcula cuando se dice algo , usa timer como base y luego le mete algo de aleatoriedad, es el que luego se usa para saber si podemos volver a decir otra frase
    float lastTimeUsed;
    public static float globalDynamicTimer;
    public static float globalLastTimeUsed;
    public OtherBallonsBehabiour otherBallons; // tener o no tener en cuenta si hay otros hablando
    public bool ignoreTimer = false; // para ignorar el takTimer
    public bool resetBallon = false; // para cortar lo que estabamos diciendo y forzar a decir esto
    [Range(0,1)] public float chances = 0.5f;
    public string[] phrase;
    public SWizSpriteAnimationClip emojisClip;
    public int i;

    public static implicit operator bool(PhraseArray me) // para poder hacer if(class) en lugar de if(class != null) NULLABLE nullable
    {
        return me != null;
    }
    public override string ToString()
    {
        return name;
    }
}
public class ActiveTextBallon{
    public PhraseType phraseType;
    public RemoteReceiver rR;
}
public static class ActiveTextBallons {
    public static List<ActiveTextBallon> activeBallons;
    public static bool atLeastOneTextBallonIsActive;
    
}
[System.Serializable]
public class LastPhraseInfo{
    public PhraseArray array;
    public int index;
    public float time;
    public int frame;

    public LastPhraseInfo() {
        array = new PhraseArray();
    }
}
[System.Serializable]
public class PhraseLineCutSettings {
    public int minLength;
    public int maxLength;
    public int lineBreak;
}
public class Phrases : MonoBehaviour {

    public string wordToFind;
	public static LastPhraseInfo staticLastPhrase;
    public LastPhraseInfo lastPhrase;
    public Color ballonColor = Color.white;

	public bool silence = false; // PARA QUE SIRVE ESTO? lo fijo en varios sitios pero nunca lo leo para hacer algo al respecto 
    public bool sayAllOnEnable;
	public RenderedVisibleFlag rendererVisibleFlag;
    public PhraseLineCutSettings[] cutSettings;
    public PhraseArray[] phrase;
    public float otherCharactersNearCheckRadius = 60;

    [Range (0,1)]public float phraseVsEmoji = 0.5f;
    public bool cancelTimerOnCantJump = true;
//	bool talkTimerPassed = true;
//	Coroutine talkTimerCoroutine;
	[HideInInspector] public RemoteReceiver rR;
    AI_Target target;
    [ReadOnly2Attribute] public PhraseArray forcedPhrases;
    [ReadOnly2Attribute] public PhraseArray replacementPhrases; // esta no se fuerza , solo se dice si queremos decir una tipo de frase y esta coincide con el tipo

    void OnValidate(){
		foreach (PhraseArray pa in phrase)
		{
			pa.name = pa.type.ToString();
		}
	}
    
    public static Phrases levelGoodGuysPhrases;
    public static Phrases levelBadGuysPhrases;

    Collider2D[] nearGuys = new Collider2D[20]; 
    
    public static System.Char space = ' ';
    
   
}

