using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class Background3DModelParent : MonoBehaviour
{
    public int setIndex = 0;
    [ReadOnly2Attribute] public string elements;
    [ReadOnly2Attribute] public LevelObjectDataBase.LevelObjectObjectList list;
#if UNITY_EDITOR

    void Awake()
    {
        Init();
        gameObject.layer = Layers.ground; // no puede estar en onvalidate o da warning
    }

    public void OnValidate()
    {
        Init();
    }

    void Init()
    {

        var rocks = GameData.instance.levelObjects.backgroundrocks;
        if (setIndex >= rocks.Length || setIndex < 0)
            setIndex = 0;
        list = GameData.instance.levelObjects.backgroundrocks[setIndex];
        elements = "";
        foreach (GameObject go in list.gos)
        {
            if (go)
                elements += go.name + "/";
        }

        SceneVisibilityManager.instance.DisablePicking(gameObject, false);
        if (Layers.ground == 0)
            Layers.Initialise();
    }
#endif
}
