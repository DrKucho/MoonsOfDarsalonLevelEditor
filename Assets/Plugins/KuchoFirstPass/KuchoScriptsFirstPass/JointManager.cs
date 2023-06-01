using UnityEngine;
using System.Collections;

public class JointManager : MonoBehaviour {

	public bool debug = false;
	public HingeJoint2D joint;
	public float speedToGetInsideLimits = 2f;
	public float overLimit;
	public void Start(){
		joint = GetComponent<HingeJoint2D>();
	}
	
	public void FixedUpdate () {
		if (joint.useLimits){
			//if (joint.jointAngle < joint.limits.max || joint.jointAngle > joint.limits.min ) print ("BAM !!");
			if (joint.jointAngle < joint.limits.max){
				overLimit = -joint.limits.min - joint.jointAngle;
				joint.useMotor = true;
				SetMotorSpeed(joint, speedToGetInsideLimits * overLimit * overLimit);
			}
			else if (joint.jointAngle > joint.limits.min){
				overLimit = joint.jointAngle - joint.limits.min;
				joint.useMotor = true;
				SetMotorSpeed(joint, speedToGetInsideLimits * overLimit * overLimit);
	
			}
			else {
				SetMotorSpeed(joint, 0f);
				joint.useMotor = false;
				overLimit = 0f;
			}
		}
	//	sprite.color.r = originalColor.r + overLimit * colorMult;
	//	sprite.color.g = originalColor.g + overLimit * colorMult;
	//	sprite.color.b = originalColor.b + overLimit * colorMult;
	
	}
	void SetMotorSpeed(HingeJoint2D _joint, float _speed){
		// no puedo modificar motorSpeed directamente por que es un struct
		JointMotor2D motor = _joint.motor;
		motor.motorSpeed = _speed;
		_joint.motor = motor;
	}
	
	public void OnGUI_NO(){
		if(debug){
			int y = 40;
			GUI.Label(new Rect(20, y, 200, 20), System.String.Format("angle:{0}", joint.jointAngle)); y += 20;
			GUI.Label(new Rect(20, y, 200, 20), System.String.Format("OverLimit:{0}", overLimit)); y += 20;
			GUI.Label(new Rect(20, y, 200, 20), System.String.Format("refAngle:{0}", joint.referenceAngle)); y += 20;
			GUI.Label(new Rect(20, y, 200, 20), System.String.Format("motorSpeed:{0}", joint.motor.motorSpeed)); y += 20;
			GUI.Label(new Rect(20, y, 200, 20), System.String.Format("LimitState:{0}", joint.limitState)); y += 20;
	
		}
	}
}
