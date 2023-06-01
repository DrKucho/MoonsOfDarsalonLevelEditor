using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(LPThing))]
[System.Serializable]
[CanEditMultipleObjects]
public class LPThing_Editor : Editor
{
    public override void OnInspectorGUI()
    {
	    var sc = (LPThing)target;
    }
}
