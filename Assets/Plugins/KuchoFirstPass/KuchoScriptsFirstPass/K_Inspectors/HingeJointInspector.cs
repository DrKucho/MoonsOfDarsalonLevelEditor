using UnityEngine;
using System.Collections;

public class HingeJointInspector : MonoBehaviour {

	public Vector2 guiPos;
	private HingeJoint2D h;
	
	public void Start(){
		h = GetComponent<HingeJoint2D>();
	}
	
	public void OnGUI_NO(){
		
			var y = guiPos.y;
			var x = guiPos.x;
			GUI.Label(new Rect(x, y, 200, 20), System.String.Format("jointAngle: {0}", h.jointAngle)); y += 20;
			GUI.Label(new Rect(x, y, 200, 20), System.String.Format("referenceAngle: {0}", h.referenceAngle)); y += 20;
			GUI.Label(new Rect(x, y, 200, 20), System.String.Format("maxLimit: {0}", h.limits.max)); y += 20;
			GUI.Label(new Rect(x, y, 200, 20), System.String.Format("minLimit: {0}", h.limits.min)); y += 20;
		}
}
