using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor (typeof(GroundEdit))]
[System.Serializable]
public class GroundEdit_Editor : Editor
{
    public override void OnInspectorGUI()
    {        
		GroundEdit sc = (GroundEdit)target;

		GUILayout.BeginHorizontal();
        if(GUILayout.Button("COPY"))
        {
			sc.CopyPixels();
        }
		if(GUILayout.Button("CUT"))
		{
			sc.CutPoly();
		}
		if(GUILayout.Button("PASTE"))
		{
			sc.PastePixels();
		}
		GUILayout.EndHorizontal();
		
        DrawDefaultInspector();
    }
}
