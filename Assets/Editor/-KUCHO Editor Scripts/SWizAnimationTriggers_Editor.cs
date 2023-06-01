using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(SWizAnimationTriggers)), CanEditMultipleObjects]
[System.Serializable]
public class SWizAnimationTriggers_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        SWizAnimationTriggers sc = (SWizAnimationTriggers)target;
    }
}