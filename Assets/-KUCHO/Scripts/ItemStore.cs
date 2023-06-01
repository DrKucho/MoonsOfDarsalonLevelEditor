using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

using UnityEngine.Serialization;
using UnityEngine.Profiling;



public enum PolyphonyLimitSystem {StopOlderClip, DontAllowNewClips}

[System.Serializable]
public class OutOfScreen{
	public bool enabled;
	public float time = 15; // no destruirá antes de este tiempo aunque se cumplan las condiciones
    public bool checkAllAtOnce;
     public int howManyPerFrame;
	float oldMargin; // BORRAME ya no no necesito
	public Vector2 margin;
    public Vector2 offset;
    [HideInInspector] public Rect rect;
    [HideInInspector] [ReadOnly2Attribute] public int checkIndex;

    [HideInInspector] [ReadOnly2Attribute] public Item[] items = new Item[20];
    [HideInInspector] [ReadOnly2Attribute] public int processIndex; 

    bool ShowHowManyPerFrame(){ return !checkAllAtOnce;}

}
[System.Serializable]
public class SpecialCheck
{
    public bool enabled;
    public bool checkAllAtOnce;
    public int howManyPerFrame;
    public int outOfStoreInc;
    public int checkIndex;

    public Item[] items = new Item[20];
    public int processIndex;

    bool ShowHowManyPerFrame() { return !checkAllAtOnce; }
    
}
public class ItemStore : MonoBehaviour {

	public bool debug = false;
	public bool debugPoly = false;
	public bool debugConsistency = false;
	public bool performanceTest = false;
	[Tooltip ("Lo asigna ItemStore List")]
	[ReadOnly2Attribute] public byte storeNumberInList;
	public bool dontDestroyOnLoad = false;
	public bool reconstructArrayOnStart = true;
	public bool getEverythingBackOnStart = false;
	public int howManyDupicatesOfEachItem = 0;
	public bool renameClonesWithNumbers = true;
	public bool switchOffItemBeforeCopy = false;
    public int lazyPositionsPerFrame = 4;
	public bool sleepBodies = false;
    public bool updateAI = false;
	public bool backToStoreIfOutOfMap = false;// este no necesita una clase tipo OutOfScreen porque es el Mapa, no necesita redefinir un rect cada vez
	public float backToStoreIfOutOfMapTime = 25f; // antes de 25 segundos desde que nacen no se enviaran de vuielta incluso si estan fuiera del mapa
	public OutOfScreen disableRenderersOutOfScreen;
	public OutOfScreen ccEconomyOutOfScreen;
    public OutOfScreen backToStoreOutOfScreen;
    public SpecialCheck destroyOnSpecialCondition;


    public PolyphonyGroup[] polyGroup;

	public Material[] materials;
	public Transform[] originalChilds; // los hijos con item que no son duplicados

	public Item[] item;
	public int nextItemIndex = 0; // este es el numero del siguiente item a entregar
	public Item nextItem;
	public int totalItems;
	public int itemsAvailable;
	//public ArcadeText itemsAvailableArcadeText; // LO COMENTO POR QUE NO NECSITO QUE SE SERIALIZE ITEMSTORE, SOLO QUE SE ACCEDA (si acaso)
	public int itemsOut;
	//public ArcadeText itemsOutArcadeText; // LO COMENTO POR QUE NO NECSITO QUE SE SERIALIZE ITEMSTORE, SOLO QUE SE ACCEDA ( si acaso)
	public float nextItemWeight = 0f;
	public int i = 0;
	public int arrayCreatedOnGameFrame = -1;

	int vi= 0; // indice para recorrer las voces de los plygrupos una vez por frame
	int pgi= 0; // indice para recorrer los grupos una ver pr framw
	int omi = 0; // indice para recorrer los items y checkear si estan fuera de mapa

	public delegate void OnItemReturned(Item item);
	public OnItemReturned onItemReturned;

    public string at_name;
    public static ItemStore storeOnDebug; // asi puedo monitorizar el store y sus items desde cualquier punto de ejecucion, basta con activar su bool debug, pero claro, solo funciona cuando un solo store tiene debuf = true, si hay mas, el ultimo secuestrará la variable estatica

    public ArmyType defaultArmy = ArmyType.WildAnimals;
    private ArmySettings defaultArmySettings;
    [HideInInspector] public char[] chars; // nombre pero en cadena de caracteres para rapida comparacion y sin basura
    
	[System.Serializable]
	public class PolyphonyGroup{
		public PolyphonyLimitSystem limitSystem = PolyphonyLimitSystem.StopOlderClip;
		public int maxPoly = 10;
        [ReadOnly2Attribute] public int voicesCount = 0;
        [ReadOnly2Attribute] public int voiceIndex = -1; // antes de añadir un nuevo campo a la tabla la rutina suma 1 , asi que arrancando aqui con -1 luego la primera vez sera cero
        [ReadOnly2Attribute] public int ID_Index = 0; // numero secuencial que va incrementando para cada uno de los clips que se reproducen , asi puedo saber el mas antiguo
        [ReadOnly2Attribute] public PolyGroupVoice[] voice;
    }
    [System.Serializable]
	public class PolyGroupVoice{
		public AudioClip clip;
		public AudioManager aM;
		public int aS_Index;
		public int ID;
	}

    bool checkItemsOutOfRangeOrSpecialCondition = false;
    void UpdateBoolCheckItemsOutOfRange(){
        checkItemsOutOfRangeOrSpecialCondition = backToStoreIfOutOfMap | disableRenderersOutOfScreen.enabled | backToStoreOutOfScreen.enabled | ccEconomyOutOfScreen.enabled | destroyOnSpecialCondition.enabled;
    }



}	