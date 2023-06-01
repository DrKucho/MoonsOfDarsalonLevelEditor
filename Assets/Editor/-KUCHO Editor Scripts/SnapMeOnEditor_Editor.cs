using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor (typeof(SnapMeOnEditor))]
[System.Serializable]
public class SnapMeOnEditor_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        
		SnapMeOnEditor myScript = (SnapMeOnEditor)target;
        //if(GUILayout.Button("ROTATE"))
        //{
        //    myScript.Rotate();
        //}

    }
}
