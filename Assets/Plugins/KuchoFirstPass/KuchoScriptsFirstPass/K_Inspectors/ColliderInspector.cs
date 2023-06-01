using UnityEngine;
using System.Collections;

public class ColliderInspector : MonoBehaviour {
	
	public Vector2 guiPos;
	
	private Collider2D myCol;
	private Rigidbody2D rb;
	
	void Start(){
		myCol = GetComponent<Collider2D>();
		if (myCol) rb = myCol.attachedRigidbody;
	}
	
	void OnGUI_disabled(){
		
		float y = guiPos.y;
		float x = guiPos.x;
		GUI.Label(new Rect(x, y, 600, 20), System.String.Format("myCol: {0}", myCol)); y += 20;
        GUI.Label(new Rect(x, y, 600, 20), System.String.Format("myRB-GO: {0}", rb.gameObject.name)); y += 20;
        Bounds b = myCol.bounds;
        Vector2 vx = new Vector2(b.min.x, b.max.x);
        Vector2 vy = new Vector2(b.min.y, b.max.y);
        GUI.Label(new Rect(x, y, 600, 20), System.String.Format("bounds x: {0}", vx)); y += 20;
        GUI.Label(new Rect(x, y, 600, 20), System.String.Format("bounds y: {0}", vy)); y += 20;
        GUI.Label(new Rect(x, y, 600, 20), System.String.Format("size: {0}", b.size)); y += 20;
    }

}