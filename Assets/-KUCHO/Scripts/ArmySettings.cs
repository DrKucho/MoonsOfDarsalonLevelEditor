using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using UnityEngine.Serialization;

public class ArmySettings : MonoBehaviour
{

    
    public ArmyType armyType;
    [Header("BulletLimits")] public bool bulletsAllowed = true;
    public int maxBulletsAtOnce = 7;
    public float delayBetweenShots = 0.3f;
    [ReadOnly2Attribute] [System.NonSerialized] public float activeBulletCount;
    [ReadOnly2Attribute] [System.NonSerialized] public float lastBulletTime;
    [Header("EenemyLimits")] public int maxCharactersAtOnce = 7;
    public float delayBetweenNewCharacter = 0.3f;
    [ReadOnly2Attribute] [System.NonSerialized] public float activeCharacterCount;
    [ReadOnly2Attribute] [System.NonSerialized] public float lastCharacterTime = float.MinValue;
    public ColliderArray ignore;
    public int myLayer;

    public Phrases phrases;

    [System.Serializable]
    public class ColliderArray
    {
        public Collider2D[] cols;
        static List<Collider2D> tempList = new List<Collider2D>(); 
        
    }



}
