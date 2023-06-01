using UnityEngine;
using System.Collections;

public class SlidingPlatform : MonoBehaviour {

	public SliderJoint2D sj;
	public float speed;
	public float translation;
	
	public void Start(){
		sj = GetComponentInChildren<SliderJoint2D>();
	}
	
	public void Update () {
		JointMotor2D motor = sj.motor;
		if (sj.jointTranslation >= sj.limits.max) motor.motorSpeed *= -1;
		if (sj.jointTranslation <= sj.limits.min) motor.motorSpeed *= -1;
		sj.motor = motor;
		speed = sj.jointSpeed;
		translation = sj.jointTranslation;
	}
}
