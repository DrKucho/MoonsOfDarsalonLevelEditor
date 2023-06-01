using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickUpGlobalManager : MonoBehaviour
{
    [System.Serializable]
    public class SequentialRandomStore {
        public string _name;
        public ItemStoreFinder[] store = new ItemStoreFinder[1];
        
    }

    public SequentialRandomStore[] SRStores;
    [HideInInspector] public int index;
    public static PickUpGlobalManager instance;


}
