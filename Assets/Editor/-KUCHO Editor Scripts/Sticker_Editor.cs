using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(Sticker))]
[System.Serializable]
[CanEditMultipleObjects]
public class Sticker_Editor : Editor
{
    public override void OnInspectorGUI()
    {
	    var sc = (Sticker)target;
    }
}
