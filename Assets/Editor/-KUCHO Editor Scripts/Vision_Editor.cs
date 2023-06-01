using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(Vision))]
[System.Serializable]
[CanEditMultipleObjects]
public class Vision_Editor : Editor
{
	private Vision sc;
    public override void OnInspectorGUI()
    {
	    sc = (Vision)target;
    }
}
