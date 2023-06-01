using UnityEngine;
using System;
using UnityEditor;


public class Constants : MonoBehaviour{
	public static Vector3 zero3;
    public static Vector2 zero2;
    public static Vector3 one3 = new Vector3(1,1,1);
    public static Vector2 one2 = new Vector2(1,1);
    public static Vector2 minValue2 = new Vector2(float.MinValue, float.MinValue);
	public static Quaternion zeroQ = new Quaternion(0,0,0,1);
    public static Vector2 down2 = new Vector2(0,-1);
    public static Vector2 up2 = new Vector2(0,1);
    public static Vector2 left2 = new Vector2(-1,0);
    public static Vector2 right2 = new Vector2(1,0);
	public static Color solidBlack = new Color (0,0,0,1);
    public static Color transparentBlack = new Color (0,0,0,0);
    public static Color solidWhite = new Color (1,1,1,1);
    public static Color halfWhite = new Color (1,1,1,0.5f);
    public static Color quarterWhite = new Color (1,1,1,0.25f);
    public static bool isDebugBuild;
    public static bool appIsEditor;
    public const bool appIsLevelEditor = true;

    public static bool drawGadgetsAfterDriving = true;
    
    public static Char spaceChar = (Char)32; 
    public static Char slashChar = (Char)47;

	void Awake(){
        isDebugBuild = Debug.isDebugBuild;
        appIsEditor = Application.isEditor;
	} 
}