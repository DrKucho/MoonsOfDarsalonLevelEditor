using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(Game)), CanEditMultipleObjects]
[System.Serializable]
public class Game_Editor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		Game sc = (Game)target;
	}
}

