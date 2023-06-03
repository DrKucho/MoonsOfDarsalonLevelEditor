using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(LPFixturePoly))]
[System.Serializable]
[CanEditMultipleObjects]
public class LPFixturePoly_Editor : Editor
{
    public override void OnInspectorGUI()
    {
	    var sc = (LPFixturePoly)target;
        DrawDefaultInspector();
    }
}
