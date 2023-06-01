using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(SWizSprite))]
[System.Serializable]
[CanEditMultipleObjects]
public class SWizSprite_Editor : Editor
{
    public override void OnInspectorGUI()
    {
	    var sc = (SWizSprite)target;
    }
}
