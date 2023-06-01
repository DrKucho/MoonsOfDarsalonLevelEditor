using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(SnapMeAtStart))]
[System.Serializable]
[CanEditMultipleObjects]
public class SnapMeAtStart_Editor : Editor
{
    public override void OnInspectorGUI()
    {
	    var sc = (SnapMeAtStart)target;
    }
}
