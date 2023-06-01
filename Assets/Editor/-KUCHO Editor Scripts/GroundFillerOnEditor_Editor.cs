using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


[CustomEditor (typeof(GroundFillerOnEditor))]
[System.Serializable]
public class GroundFillerOnEditor_Editor : Editor
{
	int clearClicks = 0;
	float clearClickTime = 0;
	int createRateCounter = 0;

    public override void OnInspectorGUI()
    {   
		float elapsedTime = Time.realtimeSinceStartup - clearClickTime;
		if (elapsedTime > 3)
		{
			clearClicks = 0;
		}
		GroundFillerOnEditor sc = (GroundFillerOnEditor)target;

        if(GUILayout.Button("DO IT")) sc.DoIt();
      
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("CREATE")) sc.SetCreateMode();
		if(GUILayout.Button("DELETE")) sc.SetDeleteMode();
		if(GUILayout.Button("MODIFY")) sc.SetModifyMode();
		if(GUILayout.Button("NONE")) sc.SetNoneMode();
		if(GUILayout.Button("Clear", EditorStyles.miniButton))
		{
			clearClickTime = Time.realtimeSinceStartup; 
			clearClicks ++;
			if (clearClicks > 3)
			{
				sc.ClearAllThings();
			}
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("PREVIOUS MATERIAL")) sc.PreviousMaterial();
		if(GUILayout.Button("NEXT MATERIAL")) sc.NextMaterial();
		EditorGUILayout.EndHorizontal();

		DrawDefaultInspector();
    }
    public void OnSceneGUI(){
	    GroundFillerOnEditor sc = (GroundFillerOnEditor)target;
	    Event e = Event.current;

	    if (e.type == EventType.Layout)
	    {

		    switch (e.modifiers){
			    case (EventModifiers.Shift):
				    sc.fillMode = GroundFillerOnEditor.FillMode.Delete;
				    sc.DoIt();
				    break;
			    case (EventModifiers.Control):
				    sc.fillMode = GroundFillerOnEditor.FillMode.Create;
				    createRateCounter ++;
				    if (createRateCounter >= sc.createEach)
				    {
					    createRateCounter = 0;
					    sc.DoIt();
				    }
				    break;
			    case (EventModifiers.Alt):
				    sc.fillMode = GroundFillerOnEditor.FillMode.Modify;
				    sc.DoIt();
				    break;
			    case (EventModifiers.Command):// solo en mac?
				    sc.fillMode = GroundFillerOnEditor.FillMode.CreateAndModify;
				    sc.DoIt();
				    break;
		    }
	    }
    }

}
