using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(Decorative))]
[System.Serializable]
[CanEditMultipleObjects]
public class Decorative_Editor : Editor
{
    public override void OnInspectorGUI()
    {
	    var sc = (Decorative)target;
    }
}
