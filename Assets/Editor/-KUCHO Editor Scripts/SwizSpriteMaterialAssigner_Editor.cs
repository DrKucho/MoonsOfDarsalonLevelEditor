using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(SWizSpriteMaterialAssigner))]
[System.Serializable]
[CanEditMultipleObjects]
public class SWizSpriteMaterialAssigner_Editor : Editor
{
    public override void OnInspectorGUI()
    {
	    var sc = (SWizSpriteMaterialAssigner)target;
    }
}
