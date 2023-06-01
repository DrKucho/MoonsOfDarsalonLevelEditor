using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Animation DataBase", order = 51)] 

public class AnimationDataBase : ScriptableObject
{

    [FormerlySerializedAs("alienSoldierRay")] public SWizSpriteAnimation alienSoldierSpawnerRay;
    static AnimationDataBase _instance;
    public static AnimationDataBase instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameData.instance.animationDataBase;
                if (!_instance)
                {
                    Debug.Log("NO ENCUENTRO ANIMATION DATA");
                }
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
}