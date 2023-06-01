using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class AnimationManager : MonoBehaviour {

	// Este script, crea un script hijo que generará un diccionario en cada GO con un SWizSpriteAnimator que este en la tabla Anims
	// aqui tengo una lista de variables que contienen un indice, con ese indice accedo a los SWizAnimatorClipID que estaran almacenados en los script hijos  

	//[Inspect, TextArea(3,7)][Descriptor(0,1,0)]
	public string INSTRUCTIONS = "AÑADE A MANO LOS ANIM\nEN ANT/CIVILIAN SOLO LEGS Y TORSO!!!\nEL BUSCA/CREA LOS ANIM-CLIP-ID";

	public bool debug = false;
	public string debugStopOnThisAnimName = "";
	public bool playBlankIfNoAnim = false;
	[SerializeField] SWizOnSpriteChangedGlobal onSpriteChangedGlobal;
	[SerializeField] CC cC;
	public SWizSpriteAnimator main; // el torso o las piernas...
	public AnimationClipIDs mainIDs;
	public Vector2 mainSpriteSize;
	public Renderer mainSpriteRenderer;
	public int ladderAssToLadderDownPixelOffset = 4; // Ha de definirse a mano
	public float ladderRotCompensation  = 0.18f;
	public float ladderAssStartRotCompensation = -0.18f;	public SWizSpriteAnimator[] anims;
	[Space(30)][Header ("AUTO ASSIGN")]


	[SerializeField] [ReadOnly2Attribute] SWizSprite[] sprites;
	[SerializeField] [ReadOnly2Attribute] Renderer[] renderers;
	[SerializeField] [ReadOnly2Attribute] SmartHead smartHead;
	[SerializeField] [ReadOnly2Attribute] float[] originalSpritesAlpha;
	[SerializeField] [ReadOnly2Attribute] float onSpriteChangedGlobalOriginalAlpha;
	[SerializeField] [ReadOnly2Attribute] Collider2D[] colliders; // todos los colliders que encuentre para llamadas tipo disable colliders
	[HideInInspector] public bool completed = true;
	[SerializeField] [ReadOnly2Attribute] AnimationClipIDs[] clipIDs; // los clips a reproducir en cada llamada a Play , seran el mismo numero que los animadores, uno por cada


	[ReadOnly2Attribute] [HideInInspector] public SWizSpriteAttachPoint[] attachpoints;
	[ReadOnly2Attribute] [HideInInspector] public int currentLadderAnimIndex = -1; // ID de la animacion escalera segun la variable thing.right
    [ReadOnly2Attribute] [HideInInspector] public int ladderAnimFrameCount = 0; // conteo de frames de la anim ladder actual, hay que llamar a CountFramesOnLadderAnim() para que se actualice
    [ReadOnly2Attribute] [HideInInspector] public int ladderAnimFrameCountHalf = 0; // la mitad
    [ReadOnly2Attribute] [HideInInspector] public int ladderAnimFrameCountQuarter = 0; // un cuarto
    [ReadOnly2Attribute] [HideInInspector] public int ladderAnimFrameCountThreeQuarters = 0; // tres cuartas partes del total
    [ReadOnly2Attribute] [HideInInspector] public int ladderAnimLastFrame = 0;
    [ReadOnly2Attribute] [HideInInspector] public int currentHurtAnimIndex = -1;
    [ReadOnly2Attribute] [HideInInspector] public int currentKoAnimIndex = -1;
    [ReadOnly2Attribute] [HideInInspector] public int currentDyingAnimIndex = -1;
    [ReadOnly2Attribute] [HideInInspector] public int currentDeadAnimIndex = -1;
    [ReadOnly2Attribute] [HideInInspector] public int currentJumpAnimIndex = -1; // tiene siempre la animacion correcta de salto, sea subiendo, bajando , avanzando, en el sitio ...
    [ReadOnly2Attribute] [HideInInspector] public float jumpTouchDownDuration = 0f; // la duracion de la animacion JumpTouchDown que ha de ser igual a la duracion de JumpFwdTouchDown para saber cuando empezar a dispararla
    [ReadOnly2Attribute] [HideInInspector] public bool standUpExists = false; // se pone a true si existe animacion "StandUp" que se usa cuando se levanta despues de que le tumben
    [ReadOnly2Attribute] [HideInInspector] public bool duckDownExists = false; // se pone a true si existe animacion "DuckDown" que es un paso intemedio entre de pie y agachado
    [ReadOnly2Attribute] [HideInInspector] public bool duckAfterFallExists = false; // se pone a true si existe animacion "DuckAfterFall" que se usa depues de caidas desde altura modereada
    [ReadOnly2Attribute] [HideInInspector] public bool duckAfterFallSmallExists = false; // se pone a true si existe animacion "DuckAfterFall" que se usa depues de caidas desde altura modereada
    [ReadOnly2Attribute] [HideInInspector] public bool flipExists = false; // se pone a true si existe animacion "Flip" para cuando el cambio de izquierda a derecha no es inmediato y tiene transicion
    [ReadOnly2Attribute] [HideInInspector] public bool warningFlyerExists = false;
    [ReadOnly2Attribute] [HideInInspector] public bool flyerHurtExists = false; // se pone a true si existe animacion "FlyerHurt"
    [ReadOnly2Attribute] [HideInInspector] public bool hurtExists = false; // se pone a true si existe animacion "Hurt"
    [ReadOnly2Attribute] [HideInInspector] public bool koAnimationsExist = false;
    [ReadOnly2Attribute] [HideInInspector] public bool flyerDyingExists = false; // se pone a true si existe animacion "Hurt"
    [ReadOnly2Attribute] [HideInInspector] public bool dyingExists = false; // se pone a true si existe animacion "Hurt"
    [ReadOnly2Attribute] [HideInInspector] public bool swimStraightExists = false; // se pone a true si existe animacion "Hurt"
    [ReadOnly2Attribute] [HideInInspector] public bool swimStraightDownExists = false; // se pone a true si existe animacion "Hurt"
    [ReadOnly2Attribute] [HideInInspector] public bool swimTransitionToForwardExists = false; // se pone a true si existe animacion "Hurt"
    [ReadOnly2Attribute] [HideInInspector] public bool swimTransitionToStraightExists = false; // se pone a true si existe animacion "Hurt"
    [ReadOnly2Attribute] [HideInInspector] public bool swimForwardExists = false; // se pone a true si existe animacion "Hurt"
    [ReadOnly2Attribute] [HideInInspector] public bool swimHurtExists = false;
    [ReadOnly2Attribute] [HideInInspector] public bool climbExists = false;

	// estos de abajo son los clipIndex, en estas variables se almacenaran los clip index de las animaciones que se llaman como la variable, si es que existen
	// estos valores son rellenados por el otro script secundario AnimationClipIDs en start, este proceso lo repetira cada uno de los script secundarios hijos
	// pero todos pondran la misma informacion , no reparo esa redundancia por simplicidad, ya que evitarlo seria complicado y es algo que ocurre una sola vez
	//inicializo a -100 que significa que no ha sido inicializado
	[Header (" Indices a Animaciones Existentes ")]
	[ReadOnly2Attribute] public int FlyerBackward= -100; // si no existe apuntara a la misma que FlyerIlde
    [ReadOnly2Attribute] public  int FlyerForward= -100; // si no existe apuntara a la misma que FlyerIlde
    [ReadOnly2Attribute] public  int FlyerForwardUp= -100; // si no existe apuntara a la misma que FlyerIlde
	[ReadOnly2Attribute] public  int FlyerHurt= -100;
    [ReadOnly2Attribute] public  int FlyerIlde= -100;
    [ReadOnly2Attribute] public  int FlyerUp= -100;
    [ReadOnly2Attribute] public  int FlyerDown= -100;
	[ReadOnly2Attribute] public  int FlyerDying= -100;
	[ReadOnly2Attribute] public  int FlyerDead= -100;
	
	[ReadOnly2Attribute] public  int Jump= -100;
	[ReadOnly2Attribute] public  int JumpFwd= -100;
	[ReadOnly2Attribute] public  int JumpFwdDown= -100; // si no existe apuntara a la misma animacion que JumpFwd
	[ReadOnly2Attribute] public  int JumpFwdTouchDown= -100; // si no existe apuntara a la misma que JumpFwdDown
	[ReadOnly2Attribute] public  int JumpDown= -100; // si no existe apuntara a la misma que Jump
	[ReadOnly2Attribute] public  int JumpTouchDown= -100; // si no existe apuntara a la misma que JumpDown
	
	[ReadOnly2Attribute] public  int Hurt= -100;
	[ReadOnly2Attribute] public  int HurtBack= -100;
	[ReadOnly2Attribute] public  int HurtFront= -100;
	[ReadOnly2Attribute] public  int KO= -100;
	[ReadOnly2Attribute] public  int KOBack= -100;
	[ReadOnly2Attribute] public  int KOFront= -100;
	[ReadOnly2Attribute] public  int Dying= -100;
	[ReadOnly2Attribute] public  int DyingFront= -100;
	[ReadOnly2Attribute] public  int DyingBack= -100;
	[ReadOnly2Attribute] public  int Dead= -100;
	[ReadOnly2Attribute] public  int DeadFront= -100;
	[ReadOnly2Attribute] public  int DeadBack= -100;
	
	[ReadOnly2Attribute] public  int Ilde= -100;
	[ReadOnly2Attribute] public  int Run= -100;
	[ReadOnly2Attribute] public  int ReverseRun= -100;
	[ReadOnly2Attribute] public  int Duck= -100;
	[ReadOnly2Attribute] public  int DuckDown= -100;
	[ReadOnly2Attribute] public  int StandUp= -100;
    [ReadOnly2Attribute] public int DuckAfterFall = -100;
    [ReadOnly2Attribute] public int DuckAfterFallSmall = -100;
    [ReadOnly2Attribute] public  int Roll= -100;
	[ReadOnly2Attribute] public  int Fall= -100;
	[ReadOnly2Attribute] public  int Escape= -100;
	[ReadOnly2Attribute] public  int Shock= -100;
	[ReadOnly2Attribute] public  int Flip= -100;
	
	[ReadOnly2Attribute] public  int Ladder= -100;
	[ReadOnly2Attribute] public  int LadderAss= -100;
	[ReadOnly2Attribute] public  int LadderAssSide= -100;
	[ReadOnly2Attribute] public  int LadderUpSide= -100;
	[ReadOnly2Attribute] public  int LadderDownSide= -100;
	[ReadOnly2Attribute] public  int Pipe= -100;
	[ReadOnly2Attribute] public  int PipeUpSide= -100;
	[ReadOnly2Attribute] public  int PipeDownSide= -100;
	
	[ReadOnly2Attribute] public  int Fire= -100;
	[ReadOnly2Attribute] public  int FireDuck= -100;
	[ReadOnly2Attribute] public  int FireUp= -100;
	[ReadOnly2Attribute] public  int FireDown= -100;
	
	[ReadOnly2Attribute] public  int Punch= -100;
	[ReadOnly2Attribute] public  int PunchDuck= -100;
	[ReadOnly2Attribute] public  int PunchUp= -100;
	[ReadOnly2Attribute] public  int PunchDown= -100;
	
	[ReadOnly2Attribute] public  int FireFlyer= -100;
	[ReadOnly2Attribute] public  int FireUpFlyer= -100; // si no existe apuntara a la misma que FireFlyer
	[ReadOnly2Attribute] public  int WarningFlyer= -100;
	[ReadOnly2Attribute] public  int PunchFlyer= -100;
	
	[ReadOnly2Attribute] public  int WalkerBirth= -100;
	[ReadOnly2Attribute] public  int FlyerBirth= -100;
	
	[ReadOnly2Attribute] public  int FlyerCarry= -100;
	[ReadOnly2Attribute] public  int Blank= -100;

	[ReadOnly2Attribute] public  int SwimStraight = -100;
	[ReadOnly2Attribute] public  int SwimStraightDown = -100;
	[ReadOnly2Attribute] public  int SwimTransitionEnd = -100;
	[ReadOnly2Attribute] public  int SwimTransitionToStraight = -100;
	[ReadOnly2Attribute] public  int SwimForward = -100;
	[ReadOnly2Attribute] public  int SwimHurt = -100;
	[ReadOnly2Attribute] public  int SwimHurtFront = -100;
	[ReadOnly2Attribute] public  int SwimHurtBack = -100;
	[ReadOnly2Attribute] public  int SwimKOFront = -100;
	[ReadOnly2Attribute] public  int SwimKOBack = -100;

    [ReadOnly2Attribute] public int Climb = -100;
    [ReadOnly2Attribute] public int OnShip = -100;
    
    [ReadOnly2Attribute] public int Hang = -100;

    int currentClipIndex;
	public  int CurrentClipIndex{get { return currentClipIndex; } }

	Dictionary<int, Delegates.Simple> onCompleted ;

}
