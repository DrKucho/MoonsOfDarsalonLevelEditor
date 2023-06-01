using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
//using UnityEngine.XR.WSA.Sharing; // WTF??
using Object = UnityEngine.Object;

#if UNITY_EDITOR
public enum FileAction { MoveToFolder, Delete, Nothing }

[CreateAssetMenu(fileName = "New Kucho Build Data", menuName = "Kucho Build Data", order = 51)]
[System.Serializable]
public class KuchoBuildData : ScriptableObject
{
    public GameObject gamePrefab;

    public bool buildOsxFull;
    public bool buildWinFull;

    
    public static bool isBuildingNow;


    
    public string projectPath;
    public string motherProjectPath;
    public string osxPath;
    public string winPath;

    public bool logDirs;

    public string scenesPath;

    public AudioClip buildStartSound;
    public AudioClip finishedSound;
    public AudioClip errorsFoundSound;

    public string targetStr;

    public string gamePath;

}
#endif