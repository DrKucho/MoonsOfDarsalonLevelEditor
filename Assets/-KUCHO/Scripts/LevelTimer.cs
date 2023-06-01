using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class LevelTimer : MonoBehaviour
{


    [HideInInspector] public ArcadeText timer;
    [HideInInspector] public Color timerColor = Color.white;
    [HideInInspector] public Color timerBlinkColor = Color.red;
    public float timeStretchFactor = 1; // para hacer que los segundos duren mas o menos y asi poder ajustar las misiones de tiempo a .67
    [HideInInspector] public SWizTextMesh timerCopy;
    [HideInInspector] [Range(0, 2)] public float timeToScoreRatio = 1;
    [System.NonSerialized] [ReadOnly2Attribute] static public int minutes = 0;
    [System.NonSerialized] [ReadOnly2Attribute] static public int seconds = 0;

    public static LevelTimer instance;

}
