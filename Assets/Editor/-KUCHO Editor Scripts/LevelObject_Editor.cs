using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(LevelObject))]
[System.Serializable]
[CanEditMultipleObjects]
public class LevelObject_Editor : Editor
{
    private LevelObject sc;
    public override void OnInspectorGUI()
    {
        sc = (LevelObject)target;
    }
}