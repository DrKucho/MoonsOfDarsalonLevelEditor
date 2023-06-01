using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using UnityEngine.Profiling;


public class SkyLights : MonoBehaviour {
	
	public string _ = "i)";
	[Header ("Light Creation")]
	public bool debug; 
    public ItemStoreFinder storeFinder;
	public string findThisDecoManager;
	public int cleanRegionSize = 8;
	public int randomTries = 20;
	/// <summary>
	/// 0 significa todas las luces iguales, al incrementarlo se reduce el tamaño de la luz en funcion a como de libre estaba su espacio de alrededor
	/// </summary>
	[Range (0,1)] public float holeSizeFactor = 1f;
    [ReadOnly2Attribute] public int activeCount = 0;
    [ReadOnly2Attribute] public int totalLightCount = 0;
    [ReadOnly2Attribute] public List<SkyLight> skyLight;
    [Header ("Light Adjust")]
	[Range (0,1)] public float intensity = 0.5f;
    [Range(0, 1)] public float averageSkyColorFactor = 0.5f;
    public DayNightCycleColorProcessor.ColorReader sunColorReader;
    [Range(0, 1)] public float sunColorFactor = 0.5f;
    [Range (0,1)] public float saturation = 1f;
	[Range (0,0.2f)] public float activationSpeed = 0.007f;
	public int switchsPerFrame;
	public float activeRectAdd;
	public Rect activeRect;
	Point[] randomPos; 
	int i;
	static public SkyLights instance;

}