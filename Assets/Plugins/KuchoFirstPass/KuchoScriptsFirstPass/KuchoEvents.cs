using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KuchoEvents {

    public static event System.Action onSceneCompleted;
    public static void OnLevelCompleted(){
        if (onSceneCompleted != null)
            onSceneCompleted();
    }
    public static event System.Action onSceneStartLoading;
    public static void OnSceneStartLoading(){
        if (onSceneStartLoading != null)
            onSceneStartLoading();
    }
    public static event System.Action afterSceneWasLoaded;
    public static void AfterLevelWasLoaded(){
        if (afterSceneWasLoaded != null)
            afterSceneWasLoaded();
    }
    public static event System.Action onSceneWasLoaded1FramesAfter;
    public static void OnLevelWasLoaded1FramesAfter(){
        if (onSceneWasLoaded1FramesAfter != null)
            onSceneWasLoaded1FramesAfter();
    }
    public static event System.Action onSceneWasLoaded3FramesAfter;
    public static void OnLevelWasLoaded3FramesAfter(){
        if (onSceneWasLoaded3FramesAfter != null)
            onSceneWasLoaded3FramesAfter();
    }
    public static event System.Action onSceneWasLoaded10FramesAfter;
    public static void OnLevelWasLoaded10FramesAfter(){
        if (onSceneWasLoaded10FramesAfter != null)
            onSceneWasLoaded10FramesAfter();
    }
    public static event System.Action onPlayerLoosesOneLife;
    public static void OnPlayerLoosesOneLife(){
        if (onPlayerLoosesOneLife != null)
            onPlayerLoosesOneLife();
    }

}
