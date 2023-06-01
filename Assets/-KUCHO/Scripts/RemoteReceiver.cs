using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public enum BallonInteligence
{
	IaGoodGuys,
	IaBadGuys,
	HumanGoodGuys,
	HumanBadGuys,
	Other
}

public class RemoteReceiver : MonoBehaviour {
    public enum RemoteReceiverTiming { GameTime, RealTime, Frames }
	public bool debug = false;
    public GameObject ballonsParent;
	[Header ("SPEECH BALLON")]
    public bool thinking;
    [HideInInspector] public GameObject textBallonParent;
    [HideInInspector] public GameObject textGO;
    [HideInInspector] public SWizTextMesh textTm;
    [HideInInspector] [SerializeField] GameObject ballonGO;
    [HideInInspector] [SerializeField] SWizTextMesh ballonTm;
    [HideInInspector] [SerializeField] SWizTextMesh arrowTm;
    [HideInInspector] [SerializeField] GameObject arrowGO;
    [HideInInspector] [SerializeField] bool updateBallonPos = true;
	public MinMax ballonTimer; // evita que salgan ballones demasiado seguidos de este personaje
	public Transform head;
	public Vector3 ballonOffset;
	public float fontHeight = 6;
    public RemoteReceiverTiming timing = RemoteReceiverTiming.GameTime;
    bool TimingIsFrames(){return timing == RemoteReceiverTiming.Frames;}
    bool TimingIsNotFrames(){return timing != RemoteReceiverTiming.Frames;}
    public float _textDelay = 0.1f;
     public int textDelayFrames = 1;
     public int howManyCharactersEachFrame = 1;
	public string cursor = "";
	public float cursorBlinkDelay = 0.5f;
	public float ballonSwitchOffDelayRatio = 0.5f;
	public float ballonSwitchOffMin = 0.6f;
	public float ballonFreezeCheckTime = 2f; // a veces los bocadillos se quedan perennes, creo que es por que alguien para la corrutina, pero no se como puede ocurrir asi que hice una chapu en update , si vale 0 no se comprueba
	public Vector2 ballonSpeed = new Vector2(0.6f, 0.4f);
//	public Vector3 smoothedPos;
//	Vector2 previousBallonPos;


	[ReadOnly2Attribute] public bool ballonWaitingToBeActive= false;
	[ReadOnly2Attribute] public bool ballonWaitingToBeDisabled = false;
	string command;
	string text2;

	public bool _audioAllowed = true;
	public AudioClipArray letterSound;
	public AudioClipArray newLineSound;
	public AudioClipArray spaceSound;


    [Header ("EMOJI BALLON")]
    public GameObject emojiBallonParent;
    [HideInInspector] public SWizSpriteAnimator emojiAnim;
    public float emojiBallonSwitchOffDelay = 1f;
    public Vector3 emojiBallonOffset;

    [Header ("BOTH BALLONS")]
    [ReadOnly2Attribute] public bool sayingSomethingOnBallon = false;
	public static int activeBallonCount = 0;
	private bool pendingDecrementOnActiveBallonCount;
	public BallonInteligence defaultBallonIntelligence;
	public static int iaBallonCount;
	public static int iaGoodGuysBallonCount;
	public static int iaBadGuysBallonCount;
	
	public static int humanBallonCount;
	public static int humanGoodGuysBallonCount;
	public static int humanBadGuysBallonCount;

	public static float iaMultiplierBasedOnHumanBallonCount;
	
    public bool beingControlled = false;

    [HideInInspector] public AudioManager aM;
    [HideInInspector] public CC cC;

	int i = 0;
	int lineI=0;
	int lineStart=0;//posision desde donde va a empezar la nueva linea
	int commandLines = 1; //numero de lineas que va teniendo el texto
	int longerLineLength; //almacena la longitud de la linea mas larrrga
	int ballonW; // almacena el ancho del globo dependiendo de la linea mas larga
	string[] line = new string[20];
	string ballonTopLine;
	string ballonLine;
	string ballonBottomLine;
	int xToGo;
	
	bool rCWalking= false;
	SWizSpriteAnimator anim;
	
	Animation _animation;

	float lastLetterTime;
    float lastLetterRealTime ;

//    WaitForSecondsRealtime waitForEmojiBallonSwitchOffRealTime;
//    WaitForSeconds waitForEmojiBallonSwitchOff;
//    WaitForSecondsRealtime waitForTextDelayRealTime;
//    WaitForSeconds waitForTextDelay;

//	Coroutine switchTextBallonOffCoroutine = null;
//	Coroutine doCommandCoroutine = null;
//    Coroutine sayCorroutine = null;
//    Coroutine ballonTimerCoroutine = null;



	bool cursorAllowed = true;

	float oneFrame = 0.01666666666667f;
	float halfFrame = 0.00833333333334f;
	float oneThirdFrame = 0.00555555555556f;
	float oneQuarterFrame = 0.00416666666667f;
	float oneFifthFrame = 0.00333333333333f;
	float oneSixthFrame = 0.00277777777778f;
	

    [System.NonSerialized] public int segment = 1;

}
