using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum LayerType
{
    GoodGuys,
    GoodGuysAttack,
    BadGuys,
    BadGuysAttack,
    Ground,
    GroundForBadGuysOnly,
    GroundForBadGuysAtBirthOnly,
    GroundForGoodGuysOnly,
    Obstacle,
    Level,
    Ladder,
    NoJumpDown,
    LimitForCharacters,
    ObstacleWontCollideWithCharacters,
    SelfIllum,
    Water,
    Ui,
    Default,
    GroundForPlayerOnly,
    Teleport
}
public class Layers
{

	public static bool initialised;
	public static int goodGuys;
	public static int groundForBadGuysAtBirthOnly;
	public static int ground;
	public static int groundForGoodGuysOnly;
	public static int groundForBadGuysOnly;
	public static int level;
	public static int obstacle;
	public static int obstacleWontCollideWithCharacters;
	public static int defaultLayer;
	public static int badGuys;
	public static int ignore;
	public static int vision;
	public static int transparent;
	public static int badGuysAttack;
	public static int goodGuysAttack;
	public static int ladder;
	public static int noJumpDown;
	public static int limitForCharacters;
	public static int selfIllum;
    public static int water;
    public static int ui;
    public static int groundForPlayerOnly;
    public static int teleport;

    void OnValidate_NO(){
        Layers.Initialise();
    }
    void Awake_NO(){
        Layers.Initialise();
    }

    public static void Initialise(){
		goodGuys = LayerMask.NameToLayer("GoodGuys");
		ground = LayerMask.NameToLayer("Ground");
		groundForGoodGuysOnly = LayerMask.NameToLayer("GroundForGoodGuysOnly");
		groundForBadGuysOnly = LayerMask.NameToLayer("GroundForBadGuysOnly");
		groundForBadGuysAtBirthOnly = LayerMask.NameToLayer("GroundForBadGuysAtBirthOnly");
		goodGuysAttack = LayerMask.NameToLayer("GoodGuysAttack");
		badGuysAttack = LayerMask.NameToLayer("BadGuysAttack");
		level = LayerMask.NameToLayer("Level");
		obstacle = LayerMask.NameToLayer("Obstacle");
		obstacleWontCollideWithCharacters = LayerMask.NameToLayer("ObstacleWontCollideWithCharacters");
		badGuys = LayerMask.NameToLayer("BadGuys");

		ladder = LayerMask.NameToLayer("Ladder");
		noJumpDown = LayerMask.NameToLayer("NoJumpDown");
		limitForCharacters = LayerMask.NameToLayer("LimitForCharacters");
		selfIllum = LayerMask.NameToLayer("SelfIllum");

		defaultLayer = LayerMask.NameToLayer("Default");
		ignore = LayerMask.NameToLayer("Ignore Raycast");
		vision = LayerMask.NameToLayer("Vision");	
		transparent = LayerMask.NameToLayer("TransparentFX");
        water = LayerMask.NameToLayer("Water");
        ui = LayerMask.NameToLayer("UI");
        groundForPlayerOnly = LayerMask.NameToLayer("GroundForPlayerOnly");
        teleport = LayerMask.NameToLayer("Teleport");

        initialised = true;
    }

    public static int GetLayerFromEnum(LayerType enumLayer){
		switch (enumLayer)
		{
			case (LayerType.GoodGuys):
				return goodGuys;
			case (LayerType.GoodGuysAttack):
				return goodGuysAttack;
			case (LayerType.BadGuys):
				return badGuys;
			case (LayerType.BadGuysAttack):
				return goodGuysAttack;
			case (LayerType.Ground):
				return ground;
			case (LayerType.GroundForBadGuysOnly):
				return groundForBadGuysOnly;
			case (LayerType.GroundForBadGuysAtBirthOnly):
				return groundForBadGuysAtBirthOnly;
			case (LayerType.GroundForGoodGuysOnly):
				return groundForGoodGuysOnly;
			case (LayerType.Obstacle):
				return obstacle;
			case (LayerType.Level):
				return level;
			case (LayerType.Ladder):
				return ladder;
			case (LayerType.NoJumpDown):
				return noJumpDown;
			case (LayerType.LimitForCharacters):
				return limitForCharacters;
			case (LayerType.ObstacleWontCollideWithCharacters):
				return obstacleWontCollideWithCharacters;
            case (LayerType.SelfIllum):
                return selfIllum;
            case (LayerType.Water):
                return water;
            case (LayerType.Ui):
                return ui;
            case (LayerType.Default):
                return defaultLayer;
            case (LayerType.GroundForPlayerOnly):
                return groundForPlayerOnly;
			case (LayerType.Teleport):
				return teleport;
			default:
				return -1;
		}
    }
    public static string GetTagFromLayer(int layer)
    {
	    if (layer == goodGuys)
	    {
		    return "GoodGuys";
	    }
	    else if (layer == badGuys)
	    {
		    return "BadGuys";
	    }
	    else if (layer == goodGuysAttack)
	    {
		    return "GoodGuys";
	    }
	    else if (layer == badGuysAttack)
	    {
		    return "BadGuys";
	    }

	    return "Untagged";
    }
}
