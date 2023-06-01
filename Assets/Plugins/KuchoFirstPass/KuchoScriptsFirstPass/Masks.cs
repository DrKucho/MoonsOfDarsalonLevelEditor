using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaskType
{
	GroundObstacleAndLevel,
	GroundOnly,
	ObstacleOnly,
	GroundAndObstacle,
	GroundObstacleAndDefault,
	GroundObstacleLevelAndVision,
	GroundForPlayer, 
	GroundIncGoodGuysGround,	
	GroundIncBadGuysGround,
	GroundIncBadGuysAndBadGuysBirthGround,
	GroundObstacleAndGoodGuys,
	GroundObstacleAndBadGuys,
    LevelGroundAndObstacle,
	GoodGuys,
	GoodGuysAndGoodGuysAttack,
	BadGuys,
	BadGuysAndHBadGuysAttack,
	GoodAndBadGuys,
	GoodGuysAttack,
	BadGuysAttack,
	GoodAndBadGuysAttack,
	Vision,
	Generators,
	Ladder,
	Ignore,
	Transparent,
	DecoSpecial,
    Teleport,
	Default, 
    LevelOnly,
    groundAndObstacleAndLimitForCharacters,
    GroundObstacleGroundForBadGuysAndDefuault,
    GroundObstacleGoundForGoodGuysAndDefuault,
    NoJumpDown,
    GroundAndLevel,
    LevelAndObstacle

}

public class Masks : MonoBehaviour
{
	public static bool initialised;

	public static int _default;
	public static int vision;
	public static int groundOnly; // solo tiene Ground , puede venir bien en el caso de generadores y en otras ocasiones
	public static int groundAndLevel;// usado para la detecciond e nivel alto y bajo de la escalera
	public static int groundObstacleAndLevel; //define todo lo que se puede pisar y no se puede atravesar, usado en deteccion de suelo y paredes
	public static int groundAndObstacle; //usada para los lineCast de vision, asi los enemigos pueden ver a traves de levels pero no de groun y obstaculo
	public static int groundAndObstacleAndSelfIllum; //usada para el flashlight flare reducer
    public static int groundAndObstacleAndLimitForCharacters; //usada en los raycaster sensores como el de torso 
    public static int groundObstacleAndDefuault; // NUEVO usada para vision Ahora la Capa Default son cosas que los ai pueden detectar, nuevo sistema, sustituira a groundAndObstacleMask
    public static int groundObstacleGroundForBadGuysAndDefault; // NUEVO usada para vision Ahora la Capa Default son cosas que los ai pueden detectar, nuevo sistema, sustituira a groundAndObstacleMask
    public static int groundObstacleGoundForGoodGuysDefault;// antes tenia "AndLadder;" // NUEVO usada para vision Ahora la Capa Default son cosas que los ai pueden detectar, nuevo sistema, sustituira a groundAndObstacleMask
	public static int groundObstacleLevelDefaultAndVision; // todo lo que se puede pisar y no atravesar + la capa vision , usada para discrimiar colisiones que no producen daño
	public static int groundForPlayer; // el suelo  para player , incluye capa adicional GroundForPlayer
	public static int groundForGoodGuys; // el suelo normal 
	public static int groundForBadGuys; // el suelo que pisan los enemigos, tiene todas las capas normales de suelo + una solo para enemigos
	public static int groundObstacleAndGoodGuys;
	public static int groundObstacleAndBadGuys;
	public static int obstacleOnly; // solo tiene Obstacle
	public static int levelAndObstacle; // solo tiene Obstacle
	public static int generators; // capa especial para colliders de generadores, para uso en enemy generator y tirar un rayo que encuentre el suelo valido mas facilmente
	public static int groundIncBadGuysAndBadGuysBirthGround; // tine todas las capas normales de suelo + la de solo para enemigos + una especial solo para los enemigos que estan naciendo , una vez que pisan otra capa, su mascara de suelo pasara a ser groundIncBadGuysGroundMask
    public static int groundStandard; // usado en deteccion de niveles inferiores al iniciar salto a nivel inferior
    public static int levelOnly;
    public static int ladder; // usada al detectar escaleras
	public static int badGuys; // para comprobar si enemigos colisionan con enemigos , normalmente no lo hago , solo a veces
	public static int badGuysAndBadGuysAttack; // para que el arma ignore sus propios colliders cercanos
	public static int goodGuys;
	public static int goodGuysAndGoodGuysAttack; // para que el arma ignore sus propios colliders cercanos
	public static int goodAndBadGuys;
	public static int goodAndBadGuysAttack;
	public static int ignore;
	public static int transparent;
	public static int badGuysAttack;
	public static int goodGuysAttack;
    public static int decoSpecial;
    public static int teleport;
    public static int noJumpDown;

    void OnValidate(){ 
        Masks.Initialise();
    }
    void Awake(){
        Masks.Initialise();
    }
	public static void Initialise(){
		_default = LayerMask.GetMask("Default");

		vision = LayerMask.GetMask("Vision");

		groundOnly = LayerMask.GetMask("Ground");
		groundAndLevel = LayerMask.GetMask("Ground", "Level");
		obstacleOnly = LayerMask.GetMask("Obstacle");	
		levelAndObstacle = LayerMask.GetMask("Obstacle", "Level");
		groundObstacleAndLevel = LayerMask.GetMask("Ground", "Obstacle", "Level"); // el normal	
		groundAndObstacle = LayerMask.GetMask("Ground", "Obstacle");
		groundAndObstacleAndSelfIllum = LayerMask.GetMask("Ground", "Obstacle", "SelfIllum");
        groundAndObstacleAndLimitForCharacters = LayerMask.GetMask("Ground", "Obstacle", "LimitForCharacters");
        groundObstacleAndDefuault = LayerMask.GetMask("Ground", "Obstacle", "Default");
        groundObstacleGroundForBadGuysAndDefault = LayerMask.GetMask("Ground", "Obstacle", "GroundForBadGuysOnly","Default");
        groundObstacleGoundForGoodGuysDefault = LayerMask.GetMask("Ground", "Obstacle", "GroundForGoodGuysOnly","Default");
		groundObstacleLevelDefaultAndVision = LayerMask.GetMask("Ground", "Obstacle", "Level", "Default", "Vision");
		groundForPlayer = LayerMask.GetMask("Ground", "Obstacle", "Level", "GroundForGoodGuysOnly" , "GroundForPlayerOnly", "NoJumpDown"); // para player	
		groundForGoodGuys = LayerMask.GetMask("Ground", "Obstacle", "Level", "GroundForGoodGuysOnly", "NoJumpDown"); // el normal	
		groundForBadGuys = LayerMask.GetMask("Ground", "Obstacle", "Level", "GroundForBadGuysOnly", "NoJumpDown"); // mascara especial para enemigos que incluye una capa que solo pueden pisar enmigos
		groundStandard = LayerMask.GetMask("Ground", "Obstacle", "Level", "NoJumpDown"); // usado en deteccion de niveles inferiores al iniciar salto a nivel inferior
		noJumpDown = LayerMask.GetMask("NoJumpDown");

		groundObstacleAndGoodGuys = LayerMask.GetMask("Ground", "Obstacle", "GoodGuys");
		groundObstacleAndBadGuys = LayerMask.GetMask("Ground", "Obstacle", "BadGuys");

		goodGuys = LayerMask.GetMask("GoodGuys");
		badGuys = LayerMask.GetMask("BadGuys") ;
		goodAndBadGuys = LayerMask.GetMask("GoodGuys", "BadGuys");
		goodGuysAttack = LayerMask.GetMask("GoodGuysAttack");
		goodGuysAndGoodGuysAttack = LayerMask.GetMask("GoodGuys", "GoodGuysAttack");
		badGuysAttack = LayerMask.GetMask("BadGuysAttack") ;
		badGuysAndBadGuysAttack = LayerMask.GetMask("BadGuys", "BadGuysAttack");
		goodAndBadGuysAttack = LayerMask.GetMask("GoodGuysAttack", "BadGuysAttack");

		generators = LayerMask.GetMask("GroundForBadGuysAtBirthOnly");
		groundIncBadGuysAndBadGuysBirthGround = LayerMask.GetMask("Ground", "Obstacle", "Level", "GroundForBadGuysOnly", "GroundForBadGuysAtBirthOnly"); // mascara especial para enemigos que incluye una capa que solo pueden pisar enmigos
        levelOnly = LayerMask.GetMask("Level");
        ladder = LayerMask.GetMask("Ladder");
		ignore = LayerMask.GetMask("Ignore Raycast");
		transparent = LayerMask.GetMask("TransparentFX");
		decoSpecial = LayerMask.GetMask("Ground", "Obstacle", "Level", "GroundForBadGuysOnly", "GroundForBadGuysAtBirthOnly"); // mascara especial grounFiller tipo farolas

        teleport = LayerMask.GetMask("Teleport");

        initialised = true;
	}
	public static int GetLayerMaskFromEnum(MaskType enumMask){
		switch (enumMask){
			case (MaskType.GroundObstacleAndLevel):
				return groundObstacleAndLevel;
			case (MaskType.GroundOnly):
				return groundOnly;
			case (MaskType.ObstacleOnly):
				return obstacleOnly;
			case (MaskType.LevelAndObstacle):
				return levelAndObstacle;
			case (MaskType.GroundAndObstacle):
				return groundAndObstacle;
			case (MaskType.GroundObstacleAndDefault):
				return groundObstacleAndDefuault;
			case (MaskType.GroundObstacleGoundForGoodGuysAndDefuault):
				return groundObstacleGoundForGoodGuysDefault;
			case (MaskType.GroundObstacleGroundForBadGuysAndDefuault):
				return groundObstacleGroundForBadGuysAndDefault;
			case (MaskType.GroundObstacleLevelAndVision):
				return groundObstacleLevelDefaultAndVision;
			case (MaskType.GroundForPlayer):
				return groundForPlayer;
			case (MaskType.GroundIncGoodGuysGround):
				return groundForGoodGuys;
			case (MaskType.GroundIncBadGuysGround):
				return groundForBadGuys;
			case (MaskType.GroundIncBadGuysAndBadGuysBirthGround):
				return groundIncBadGuysAndBadGuysBirthGround;
			case (MaskType.GroundObstacleAndGoodGuys):
				return groundObstacleAndGoodGuys;
			case (MaskType.GroundObstacleAndBadGuys):
				return groundObstacleAndBadGuys;
			case (MaskType.LevelGroundAndObstacle):
				return groundStandard;
            case (MaskType.LevelOnly):
                return levelOnly;
            case (MaskType.GoodGuys):
				return goodGuys;
			case (MaskType.GoodGuysAndGoodGuysAttack):
				return goodGuysAndGoodGuysAttack;
			case (MaskType.BadGuys):
				return badGuys;
			case (MaskType.BadGuysAndHBadGuysAttack):
				return badGuysAndBadGuysAttack;
			case (MaskType.GoodAndBadGuys):
				return goodAndBadGuys;
			case (MaskType.GoodGuysAttack):
				return goodGuysAttack;
			case (MaskType.BadGuysAttack):
				return badGuysAttack;
			case (MaskType.GoodAndBadGuysAttack):
				return goodAndBadGuysAttack;

			case (MaskType.Vision):
				return vision;
			case (MaskType.Default):
				return _default;

			case (MaskType.Generators):
				return generators;

			case (MaskType.Ladder):
				return ladder;

			case (MaskType.Ignore):
				return ignore;	
			case (MaskType.Transparent):
				return transparent;	
			case (MaskType.DecoSpecial):
				return decoSpecial;
            case (MaskType.Teleport):
                return teleport;
            case (MaskType.groundAndObstacleAndLimitForCharacters):
                return groundAndObstacleAndLimitForCharacters;
			case (MaskType.NoJumpDown):
				return noJumpDown;
			case (MaskType.GroundAndLevel):
				return groundAndLevel;
            default:
				print("------LA LAYER MASK " + enumMask + " NO EXISTE");
				Debug.Break(); Debug.Log("GetLayerMaskFromEnum !!!!!!!!!!!!!!!!!!!!!!!!!");
				return -1;
		}
	}
}
