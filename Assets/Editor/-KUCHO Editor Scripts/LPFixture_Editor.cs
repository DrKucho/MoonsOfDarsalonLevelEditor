using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(LPFixture))]
[System.Serializable]
[CanEditMultipleObjects]
public class LPFixture_Editor : Editor
{
    public override void OnInspectorGUI()
    {
	    var sc = (LPFixture)target;
    }
}
