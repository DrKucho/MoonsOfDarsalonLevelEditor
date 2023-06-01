using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor (typeof(SwitchGameObjects))]
[System.Serializable]
public class SwitchGameObjects_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
		SwitchGameObjects myScript = (SwitchGameObjects)target;
        if(GUILayout.Button("SWITCH"))
        {
            myScript.Switch();
        }
     }
}
