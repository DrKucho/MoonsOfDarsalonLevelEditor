using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor (typeof(ExplosionStampExtras))]
[CanEditMultipleObjects]
[System.Serializable]
public class ExplosionStampExtras_Editor : Editor
{
    public override void OnInspectorGUI()
    {        
		ExplosionStampExtras sc = (ExplosionStampExtras)target;

        /*
		EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("INIT")) sc.InitialiseInEditor();
		if(GUILayout.Button("INIT SPRS")) sc.InitializeStampSprites();
		if(GUILayout.Button("SORT SPRS")) sc.SortStampSprites();
		if(GUILayout.Button("EXPLODE"))  sc.Explode();
        if (GUILayout.Button("CYCLE STAMP TEX")) sc.CycleStampTex();
        if (GUILayout.Button("INSP PIX")) sc.InspectPixels();
        EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("GET POLY"))
		{
			if (!sc.fixPoly)
			{
				CreateLPFixAndStartDrawing(sc);
			}
			else
			{
				StopDrawingAndDestroy(sc);
			}
		}
		EditorGUILayout.EndHorizontal();
		*/
//		EditorGUILayout.BeginHorizontal();
//		if (GUILayout.Button("SCALE TO SIZE TEST"))
//		{
//			sc.SetTextByScale();
//		}
//		EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();
    }
	bool button0;
	bool button1;
	bool button2;
	int counter = 0;
    double lastExecutionTime;
    Event lastEvent;
    double lastGroundCreationTime;
    GroundModifierMode lastGroundModifiedMode;
    Vector2Int lastPos;
	public void OnSceneGUI(){

        Event e = Event.current;
        ExplosionStampExtras sc = (ExplosionStampExtras)target;
        double currentTime = EditorApplication.timeSinceStartup;

        if (sc.spriteRenderer)
        {
            switch (sc.groundModifierMode)
            {
                case (GroundModifierMode.Create):
                    sc.spriteRenderer.color = Color.green;
                    break;
                case (GroundModifierMode.Destroy):
                    sc.spriteRenderer.color = Color.red;
                    break;	
                case (GroundModifierMode.None):
                    sc.spriteRenderer.color = Color.white;
                    break;		
            }
            ColorHelper.SetAlpha(sc.spriteRenderer, 0.5f);
        }
        if (sc.useModifierKeysInEditor && e.type == EventType.Layout)
        {

            switch (e.modifiers)
            {
                case (EventModifiers.Control):
                    sc.groundModifierMode = GroundModifierMode.Create;
                    sc.MakeSureWeHaveD2dSpritesWeNeed();
                    sc.Explode();
                    break;
                case (EventModifiers.Shift):
                    sc.groundModifierMode = GroundModifierMode.Destroy;
                    sc.MakeSureWeHaveD2dSpritesWeNeed();
                    sc.Explode();
                    break;
                case (EventModifiers.Alt):
                    break;
                case (EventModifiers.Command):

                    break;
            }
        }
        if (e.type == EventType.KeyDown)
        {
            // ojo me llegan dos eventos decla pulsada seguidos !!!
            double allowNewKeyTime = lastExecutionTime + 0.1;
            if (currentTime > allowNewKeyTime)
            {
                if (e.keyCode == KeyCode.Z)
                    sc.CycleStampTex(-1);
                if (e.keyCode == KeyCode.X)
                    sc.CycleStampTex(1);
                if (e.keyCode == KeyCode.C)
                    sc.RotateCurrentSprite90();
                if (e.keyCode == KeyCode.V)
                    sc.MirrorCurrentSpriteLeftToRight();
                if (e.keyCode == KeyCode.B)
                    sc.MirrorCurrentSpriteUpToDown();
                if (e.keyCode == KeyCode.Tab)
                {
                    if (sc.groundModifierMode == GroundModifierMode.Destroy)
                        sc.groundModifierMode = GroundModifierMode.Create;
                    else
                        sc.groundModifierMode = GroundModifierMode.Destroy;
                }
            }
            lastExecutionTime = currentTime;
            lastEvent = e;
        }
        if (e.isMouse)
        {
            if (e.type == EventType.MouseDown)
            {
                switch (e.button)
                {
                    case (0): // left
                        button0 = true;
                        break;
                    case (2): // middle
                        button2 = true;
                        break;
                    case (1): // right
                        button1 = true;
                        if (sc.fixPoly && sc.fixPoly.pointsList != null && sc.fixPoly.pointsList.Count > 2)// si existe es que esta tambien en modo drawing por qe yo no le he dejado otra
                        {
                            StopDrawingAndDestroy(sc);
                        }
                        else if (e.clickCount > 1)
                        {
                            CreateLPFixAndStartDrawing(sc);
                        }
                        break;
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                switch (e.button)
                {
                    case (0):
                        button0 = false;
                        break;
                    case (1):
                        button1 = false;
                        sc.NotifyChangesOnEditor();
                        break;
                    case (2):
                        button2 = false;
                        break;
                }
            }
        }
        if (!sc.fixPoly || !sc.fixPoly.Drawing) // no estamos creando poligono
        {
            if (button0 && button1)// estamos arrastrando el objeto con el boton principal
            {
                if (lastGroundCreationTime < currentTime )
                {
                    if (lastPos.x != Mathf.RoundToInt(sc.transform.position.x) || lastPos.y != Mathf.RoundToInt(sc.transform.position.y))
                    {
                        lastPos.x = Mathf.RoundToInt(sc.transform.position.x);
                        lastPos.y = Mathf.RoundToInt(sc.transform.position.y);
                        lastGroundCreationTime = currentTime;
                        lastGroundModifiedMode = sc.groundModifierMode;
                        sc.Explode();
                    }
                }
            }
        }
        if (sc.inspectPixOnMove && sc.transform.position != pos_old)
        {
            sc.InspectPixels();
            pos_old = sc.transform.position;
        }

    }
    Vector3 pos_old;
	public void CreateLPFixAndStartDrawing(ExplosionStampExtras sc){
		Debug.Log("CREATE LPF AND START DRAWING");
		sc.fixPoly = sc.gameObject.GetComponent<LPFixturePoly>();
        if (!sc.fixPoly)
        {
            sc.fixPoly = sc.gameObject.AddComponent<LPFixturePoly>();
        }
        //UnityEditorInternal.ComponentUtility.MoveComponentUp(sc.fixPoly);
		sc.fixPoly.NumberOfSides = 0;
		sc.fixPoly.Drawing = true;
		sc.fixPoly.DontDrawLoop = true;
		sc.fixPoly.drawingfirstpoint = true;
	}
	public void StopDrawingAndDestroy(ExplosionStampExtras sc){
		Debug.Log("STOP DRAWING AND DESTROY");
		sc.fixPoly.Drawing = false;
		sc.fixPoly.DontDrawLoop = false;
		sc.GetPolyArea();
		DestroyImmediate(sc.fixPoly);
	}
}
