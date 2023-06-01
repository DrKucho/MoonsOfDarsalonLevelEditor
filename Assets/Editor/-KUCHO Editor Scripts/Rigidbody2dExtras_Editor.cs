using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor (typeof(Rigidbody2dExtras))]
[System.Serializable]
public class Rigidbody2dExtras_Editor : Editor
{
    public override void OnInspectorGUI()
    {        
		Rigidbody2dExtras myScript = (Rigidbody2dExtras)target;
        if(GUILayout.Button("DO IT"))
        {
            myScript.DoIt();
        }
        DrawDefaultInspector();
    }
}
