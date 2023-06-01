using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Transform))]
[CanEditMultipleObjects]
public class Transform_Editor:Editor
{
	Transform _transform;
//	bool snapXY = false;

	public override void OnInspectorGUI()
	{
		//We need this for all OnInspectorGUI sub methods
		if (!_transform)
			_transform = (Transform)target;

		StandardTransformInspector();
	}

	private void StandardTransformInspector()
	{
		bool didWorldPosChange = false;
		bool didPositionChange = false;
		bool didRotationChange = false;
		bool didScaleChange = false;
		// Watch for changes.
		//  1)  Float values are imprecise, so floating point error may cause changes
		//      when you've not actually made a change.
		//  2)  This allows us to also record an undo point properly since we're only
		//      recording when something has changed.

		// Store current values for checking later
//		Vector3 initialWorldPosition = _transform.position; // no la uso, por eso comento, la dejo para mostrar donde hay que hacerlo si lo necesito hacer en un futuro	
		Vector3 initialLocalPosition = _transform.localPosition;
		Vector3 initialLocalEuler = _transform.localEulerAngles;
		Vector3 initialLocalScale = _transform.localScale;

//		snapXY = EditorGUILayout.Toggle("Snap World X Y", snapXY);

		EditorGUI.BeginChangeCheck();
		Vector3 worldPosition = EditorGUILayout.Vector3Field("WorldPos", _transform.position);
		if (EditorGUI.EndChangeCheck())
			didWorldPosChange = true;

		EditorGUI.BeginChangeCheck();
		Vector3 localPosition = EditorGUILayout.Vector3Field("LocalPos", _transform.localPosition);
		if (EditorGUI.EndChangeCheck())
			didPositionChange = true;

		EditorGUI.BeginChangeCheck();
		Vector3 localEulerAngles = EditorGUILayout.Vector3Field("Euler Rotation", _transform.localEulerAngles);
		if (EditorGUI.EndChangeCheck())
			didRotationChange = true;

		EditorGUI.BeginChangeCheck();
		Vector3 localScale = EditorGUILayout.Vector3Field("Scale", _transform.localScale);
		if (EditorGUI.EndChangeCheck())
			didScaleChange = true;


		// CAMBIOS SOLO A ESTE TRANSFORM
		// Apply changes with record undo
		if (didPositionChange || didRotationChange || didScaleChange || didWorldPosChange)
		{
			Undo.RecordObject(_transform, _transform.name);

			if (didWorldPosChange)
				_transform.position = worldPosition;
			
			if (didPositionChange)
				_transform.localPosition = localPosition;

			if (didRotationChange)
				_transform.localEulerAngles = localEulerAngles;

			if (didScaleChange)
				_transform.localScale = localScale;
		}

		// SI HAY SELECCIONADOS MAS DE UNO APLICAR CAMBIOS A TODOS
		// Since BeginChangeCheck only works on the selected object
		// we need to manually apply transform changes to all selected objects.
		Transform[] selectedTransforms = Selection.transforms;
		if (selectedTransforms.Length > 1)
		{
			foreach (var item in selectedTransforms)
			{
				if (didPositionChange || didRotationChange || didScaleChange || didWorldPosChange)
					Undo.RecordObject(item, item.name);

				if (didWorldPosChange)
				{
					item.position = ApplyChangesOnly(item.position, worldPosition, _transform.position);
				}
				if (didPositionChange)
				{
					item.localPosition = ApplyChangesOnly(item.localPosition, initialLocalPosition, _transform.localPosition);
				}

				if (didRotationChange)
				{
					item.localEulerAngles = ApplyChangesOnly(item.localEulerAngles, initialLocalEuler, _transform.localEulerAngles);
				}

				if (didScaleChange)
				{
					item.localScale = ApplyChangesOnly(item.localScale, initialLocalScale, _transform.localScale);
				}

			}
		}
	}

	private Vector3 ApplyChangesOnly(Vector3 toApply, Vector3 initial, Vector3 changed)
	{
		if (!Mathf.Approximately(initial.x, changed.x))
			toApply.x = changed.x;

		if (!Mathf.Approximately(initial.y, changed.y))
			toApply.y = changed.y;

		if (!Mathf.Approximately(initial.z, changed.z))
			toApply.z = changed.z;

		return toApply;
	}

//	Vector3 SnapXY(Vector3 v){
//		if (snapXY)
//		{
//			v.x = Mathf.RoundToInt(v.x);
//			v.y = Mathf.RoundToInt(v.y);
//		}
//		return v;
//	}
//	Vector3 SnapXYAlways(Vector3 v){
//		v.x = Mathf.RoundToInt(v.x);
//		v.y = Mathf.RoundToInt(v.y);
//		return v;
//	}
//	void OnSceneGUI( )
//	{
//		_transform.position = SnapXYAlways(_transform.position);
//	}

}