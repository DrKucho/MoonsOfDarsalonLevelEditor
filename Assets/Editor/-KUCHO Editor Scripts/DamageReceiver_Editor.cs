using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(DamageReceiver)), CanEditMultipleObjects]
[System.Serializable]
public class DamageReceiver_Editor : Editor
{
	public override void OnInspectorGUI()
	{
		//DrawDefaultInspector();
		DamageReceiver sc = (DamageReceiver)target;
	}
}

