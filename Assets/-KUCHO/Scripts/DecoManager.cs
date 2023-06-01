using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Hosting;


[System.Serializable]
public class DecoCell{
	public ushort item;
	public byte store;
    public int index;
	//Constructor
	public DecoCell(){
		
	}
	public DecoCell(ushort _item, byte _store){
		item = _item;
		store = _store;
        index = -1;
	} 
    public void Clear(){
        index = -1;
        store = 255; // 255 significa que no hay store
        item = 0;
    }
}

public class DecoManager : MonoBehaviour {
    
	//static var pix = new Color32(0,0,0,0);
	public bool debug = false;
	public bool debugWrite = false;
	public enum Type { Undefined, Grass, SkyLights};
	public DecoManager.Type  type;
	[HideInInspector] public ushort[] itemMap;
	[HideInInspector] public byte[] storeMap;
	public Vector2 ratio;
	[Header ("---Activation Rect---")]
	public OutOfScreen outOfScreen;
	int activationIndex = 0;

	[Header ("-----Calculadas -----")]
    [ReadOnly2Attribute] public Point _mapSize;
	Point mapSize;
    [ReadOnly2Attribute] public Point cellSize;

	[Header ("---Solo Para Debugear---")]
    [ReadOnly2Attribute] public int writeCount = 0;
    public Texture2D decoTex;// la textura del mundo destruible  es de 4096*2048 , pero la resolucion deco es menor dependiendo del ratio
    public string fileName = "/Sprites/Background/Parallax/Mountains0(HiRes).png";


    public static DecoManager grassDecoManager;
    public static DecoManager skyLightsDecoManager;
	
}