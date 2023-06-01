using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ItemStoreFinder {
    public string name;
    public ItemStore store;
    public char[] chars;
    
}

public class ItemStoreList : MonoBehaviour {

	public bool debug = false;
	public GameObject[] prefabs;
	private GameObject[] instantiatedPrefabs;
	public ItemStore[] stores;
	[Header ("OUT OF RANGE AND SLEEP BODIES THREAD")]
	public bool sleepBodies = true;
	public bool checkItemsOutOfRange = true;
	[ReadOnly2Attribute] public float checkItemsOutOfRangeMs = 0;
    public int setLazyPositionsPerFrame = 50;
    static public Delegates.Simple onCheckOutOfRangeThreadedLoop;
    ThreadLoop checkOutOfRangeThread;
	static public ItemStoreList instance;
	public PickUpGlobalManager pickUpGlobalManager;
    [ReadOnly2Attribute] public ItemStore baseStore;
    [ReadOnly2Attribute] public ItemStore gadgetStore;
    [ReadOnly2Attribute] public ItemStore markStore;
    [ReadOnly2Attribute] public ItemStore pickUpTextStore;
    [ReadOnly2Attribute] public ItemStore civilianStore;
    [ReadOnly2Attribute] public ItemStore grassStore;
    [ReadOnly2Attribute] public ItemStore treasureStore;
    [ReadOnly2Attribute] public ItemStore coinStore;
    
	
	public bool everythingWentBackToStoreAsyncAlright;

    bool at_isActiveAndEnabled;



    public static bool cameraMovement = false;
    public void OnCameraMovement(){
        cameraMovement = true;
    }
    int btCycleCount = 0;
    /// <summary> BACKGROUND THREAD

	int storeReadyCount = 0;
	public delegate void OnReady();
	public OnReady onReady;
	public bool ready;


}
