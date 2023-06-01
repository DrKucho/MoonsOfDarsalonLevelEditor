using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(Light2DManager)), CanEditMultipleObjects]
[System.Serializable]
public class Light2DManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        Light2DManager sc = (Light2DManager)target;
    }
}