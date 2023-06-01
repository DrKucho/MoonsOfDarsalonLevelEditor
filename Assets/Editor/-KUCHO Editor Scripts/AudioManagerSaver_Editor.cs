using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(AudioManagerSaver))]
[System.Serializable]
[CanEditMultipleObjects]
public class AudioManagerSaver_Editor : Editor
{
    public override void OnInspectorGUI()
    {
	    var sc = (AudioManagerSaver)target;
    }
}
