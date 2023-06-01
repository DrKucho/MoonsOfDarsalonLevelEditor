using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(DecoThingsCreatedInEditor))]
[System.Serializable]
[CanEditMultipleObjects]
public class DecoThingsCreatedInEditor_Editor : Editor
{
    private DecoThingsCreatedInEditor sc;
    public override void OnInspectorGUI()
    {
        sc = (DecoThingsCreatedInEditor)target;
    }
}