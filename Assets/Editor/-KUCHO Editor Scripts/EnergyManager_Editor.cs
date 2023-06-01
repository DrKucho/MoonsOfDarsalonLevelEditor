using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(EnergyManager)), CanEditMultipleObjects]
[System.Serializable]
public class EnergyManager_Editor : Editor
{
	public override void OnInspectorGUI()
	{
		//DrawDefaultInspector();
		EnergyManager sc = (EnergyManager)target;
	}
}

