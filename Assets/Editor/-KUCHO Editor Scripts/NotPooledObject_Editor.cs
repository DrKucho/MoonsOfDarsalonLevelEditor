using UnityEngine;
using System.Collections;
using UnityEditor;

/*
[CustomEditor (typeof(NotPooledObject))]
[System.Serializable]
[CanEditMultipleObjects]
public class NotPooledObject_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
	void OnEnable(){
		NotPooledObject sc = (NotPooledObject)target;
		;
	}

	private NotPooledObject sc;
	void OnSceneGUI()
	{  
		NotPooledObject sc = (NotPooledObject)target;

		//NotPooledObjectsDataBase.NotPooledObjectsList.ConsolidateLastInstance();
                
		HandleUtility.AddDefaultControl(0);

		//Get the transform of the component with the selection base attribute
		Transform selectionBaseTransform = sc.transform;

		//Detect mouse events
		if (Event.current.type == EventType.MouseDown)
		{
			//get picked object at mouse position
			GameObject pickedObject = HandleUtility.PickGameObject(Event.current.mousePosition, true);

			//If selected null or a non child of the component gameobject
			if (pickedObject == null || !pickedObject.transform.IsChildOf(selectionBaseTransform))
			{
				//Set selection to the picked object
				Selection.activeObject = pickedObject;
			}
		}
	}
}
*/	