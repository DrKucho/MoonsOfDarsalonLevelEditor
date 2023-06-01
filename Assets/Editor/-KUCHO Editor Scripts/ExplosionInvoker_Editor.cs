using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(ExplosionInvoker))]
[System.Serializable]
[CanEditMultipleObjects]
public class ExplosionInvoker_Editor : Editor
{
    public override void OnInspectorGUI()
    {
	    var sc = (ExplosionInvoker)target;
    }
}
