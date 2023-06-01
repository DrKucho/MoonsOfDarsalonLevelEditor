using UnityEngine;
using System.Collections;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(LPPolyDrawShapeButton))]
public class LPPolyDrawShapeButtonEditor : LPShapeEditor
{
	public override void OnInspectorGUI()
	{
		var sc = (LPPolyDrawShapeButton)target;
		GameObject go = sc.gameObject;
		LPCorporeal corp = go.GetComponent<LPCorporeal>();
		if (!corp)
			corp = go.AddComponent<LPFixturePoly>();
		DrawPointsUI(corp);
	}
}