using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor (typeof(SpritePlanes))]
[System.Serializable]
public class SpritePlanes_Editor : Editor
{
    public override void OnInspectorGUI()
    {        
        SpritePlanes sc = (SpritePlanes)target;

    }
}
