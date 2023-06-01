using UnityEngine;
using System.Collections;

public class RotatioPIDController : MonoBehaviour {

	public float targetAngle = 0; // the desired angle
	public float curAngle; // current angle
	public float accel; // applied accel
	public float angSpeed= 0; // current ang speed
	public float maxAccel = 180; // max accel in degrees/second2
	public float maxASpeed = 90; // max angular speed in degrees/second
	public float pGain = 20; // the proportional gain
	public float dGain = 10; // differential gain
	public Rigidbody2D rb;
	float lastError; 

	void Start(){
		targetAngle = transform.eulerAngles.z; // get the current angle just for start
		curAngle = targetAngle;
	}

	void FixedUpdate(){
		float error = targetAngle - curAngle; // generate the error signal
		float diff = (error - lastError)/ Time.deltaTime; // calculate differential error
		lastError = error;
		// calculate the acceleration:
		accel = error * pGain + diff * dGain;
		// limit it to the max acceleration
		accel = Mathf.Clamp(accel, -maxAccel, maxAccel);
		// apply accel to angular speed:
		angSpeed += accel * Time.deltaTime; 
		// limit max angular speed
		angSpeed = Mathf.Clamp(angSpeed, -maxASpeed, maxASpeed);
		curAngle += angSpeed * Time.deltaTime; // apply the rotation to the angle...
		// and make the object follow the angle (must be modulo 360)
		rb.MoveRotation(curAngle%360); 
	}
}
