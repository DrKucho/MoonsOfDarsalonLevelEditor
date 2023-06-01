using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor (typeof(ElipseOrbit))]
[System.Serializable]
public class ElipseOrbit_Editor : Editor
{
    public override void OnInspectorGUI()
    {        
		ElipseOrbit sc = (ElipseOrbit)target;

		if(GUILayout.Button("NEXT SNAP POINT")) sc.NextSnapPoint();
        DrawDefaultInspector();
    }
}
