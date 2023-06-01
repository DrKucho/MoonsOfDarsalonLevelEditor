using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(AudioManager))]
[System.Serializable]
[CanEditMultipleObjects]
public class AudioManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
	    var sc = (AudioManager)target;
    }
}
