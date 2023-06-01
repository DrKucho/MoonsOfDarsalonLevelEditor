using UnityEngine;
using System.Collections;


public class DistanceJointInspector : MonoBehaviour {

	public Vector2 guiPos;
	private DistanceJoint2D joint;
	
	public void Start(){
		joint = GetComponent<DistanceJoint2D>();
	}


    #if UNITY_EDITOR
	public void OnGUI_disabled(){
		var y = guiPos.y;
		var x = guiPos.x;
		GUI.Label(new Rect(x, y, 200, 20), System.String.Format("Reac Force: {0}", joint.reactionForce)); y += 20;
        GUI.Label(new Rect(x, y, 200, 20), System.String.Format("Reac Torqe: {0}", joint.reactionTorque)); y += 20;
        GUI.Label(new Rect(x, y, 200, 20), System.String.Format("distance: {0}", joint.distance)); y += 20;


    }
#endif
}
