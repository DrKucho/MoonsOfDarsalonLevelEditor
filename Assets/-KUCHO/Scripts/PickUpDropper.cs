using UnityEngine;
using System.Collections;


public class StoreDropper{
	ItemStoreFinder store;
	int itemDrops = 1;
	int chances = 100;
	float forceMult = 1;
}
public class PickUpDropper : MonoBehaviour {

	public bool debug = false;
	[Header("PickUps")]
	public int howManyDrops = 1;
	[Header("Chances")]
	public bool dropOnlyIfPlayerGotGadget;
	public bool allwaysDrop = false;
	public int dropChances = 20;
    public bool isGlobalPickup;
    bool IsGlobalPickUp() { return isGlobalPickup; }
    public int forceAndResetOnNoLuckyTries = 4;
    [Header("Position & Impulse")]
    public bool lookForGround;
    public Vector3 offset;
	public RandomPos randomPos;
	public RandomVector2 randomStartImpulse;
	public RandomVector2 randomStartImpulse2;
	public Transform useThisScaleX;
	float scaleDir;
	public RandomFloat randomSpin;
	[Header("Life Time")]
	public bool lifeTimeOverride;
	public RandomFloat lifeTime;
    public bool useGlobalPickUpManager = false;
    bool UseGlobalPickUpManager() { return useGlobalPickUpManager; }
    bool UsingOwnStoreRandomizer() { return !useGlobalPickUpManager; }
    [Header("Stores")]
	[Tooltip("No se lanzaran objetos de este store, otros scripts lo fijan")]
	[HideInInspector] public ItemStore blockedStore;
    public ItemStoreFinder[] stores = new ItemStoreFinder[1];
     public bool randomStores;
    int storeIndex;
	[HideInInspector][SerializeField] ExplosionManager explosionManager;
	CC cC;
	bool dropped;

    public AudioClipArray sequentialAudioToCopy;
    public AudioClipArray sequencialBaseVoices;
    
    RaycastHit2D[] lookForGroundHits = new RaycastHit2D[1];
    [System.NonSerialized] public bool dontDropThingsThisTime; // otros scripts pueden poner esto a true para evitar que dejemos pickup
    
    public static int globalDropTries;
    public static int globalDropCount;

}
