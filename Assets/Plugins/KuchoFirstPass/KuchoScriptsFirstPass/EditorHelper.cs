
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorHelper {

	public static GameObject GetRandomGameObjectFromArray(GameObject[] array){
		return array[Random.Range(0, array.Length - 1)];
	}
	public static void Collapse(GameObject go, bool collapse)
	{
		// bail out immediately if the go doesn't have children
		if (go.transform.childCount == 0) return;
		// get a reference to the hierarchy window
		var hierarchy = GetFocusedWindow("Hierarchy");
		// select our go
		SelectObject(go);
		// create a new key event (RightArrow for collapsing, LeftArrow for folding)
		var key = new Event { keyCode = collapse ? KeyCode.RightArrow : KeyCode.LeftArrow, type = EventType.KeyDown };
		// finally, send the window the event
		hierarchy.SendEvent(key);
		SelectObject(go); // por alguna razon la seleccion se va a algunos Gos mas arriba)
	}
	public static void SelectObject(Object obj){
		Selection.activeObject = obj;
	}
	public static EditorWindow GetFocusedWindow(string window){
		FocusOnWindow(window);
		return EditorWindow.focusedWindow;
	}
	public static void FocusOnWindow(string window){
		EditorApplication.ExecuteMenuItem("Window/General/" + window);
	}
	public static Vector3 GetMousePositionOnSceneView()
	{
		var sceneView = SceneView.lastActiveSceneView;
		var camera = sceneView.camera;
		if (Event.current != null)
		{
			Vector3 pos = Event.current.mousePosition;

			pos.y = camera.pixelHeight - pos.y;
			pos.y += Mathf.Log10(camera.orthographicSize) * 12f; // chapu pero sin ella habia un error que se incrementaba a mayor ivle de zoom
			pos.z = camera.nearClipPlane;
			pos = camera.ScreenToWorldPoint(pos);

			return pos;
		}
		return Vector3.zero;
	}
}
#endif
