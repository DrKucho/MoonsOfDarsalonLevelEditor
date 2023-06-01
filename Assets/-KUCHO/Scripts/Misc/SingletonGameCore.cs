using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;


public class SingletonGameCore : MonoBehaviour {

    static public GameObject corePrefab;
    static public GameObject coreInstance;
    
    static public Scene currentScene;
    static public GameObject[] rootGos;
    static public SingletonGameCore instance;

    public delegate void OnGameLoaded();
    public static event OnGameLoaded onGameLoaded;
    
}
