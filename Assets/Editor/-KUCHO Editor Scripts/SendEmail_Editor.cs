/*using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor (typeof(Internet))]
[System.Serializable]
public class SendEmail_Editor : Editor
{
	public override void OnInspectorGUI()
    {        
		Internet sc = (Internet)target;
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("SEND")) sc.SendMail();
		EditorGUILayout.EndHorizontal();
		DrawDefaultInspector();
    }
}
*/