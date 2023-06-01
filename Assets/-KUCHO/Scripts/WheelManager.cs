using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WheelType {None, Front, Rear};

public class WheelManager : MonoBehaviour {

    public WheelType type = WheelType.None;
    [Range (0,1)] public float drivePower = 1;
    [Range (0,1)] public float brakePower = 1;
//    [Range (0,1)] public float brakeToBodyRotationRatio = 1;
//    [Range (0,1)] public float brakeToBodyRotationInverter = 0;
    [Range (0,15)] public float suspStiffnes = 1;
    [Range (0,1)] public float suspDamping = 1;
    public AudioManager springAudioManager;
    public AudioManager rubAudioManager;
    [ReadOnly2Attribute] public float translation;
    public WheelJoint2D joint;

    public void InitialiseInEditor(){
        var ams = GetComponentsInChildren<AudioManager>();
        foreach (AudioManager am in ams)
        {
            string amName = am.gameObject.name.ToUpper();
            if (amName.Contains("SPRING") || amName.Contains("SUSPENSION"))
                springAudioManager = am;
            else
                rubAudioManager = am;
        }
    }
    public void Accelerate(float maxGas, float gasAdd, float motorTorque, float translationRatio){
        JointMotor2D motor = joint.motor;
        motor.motorSpeed = joint.jointSpeed + gasAdd;
        motor.motorSpeed = Mathf.Clamp(motor.motorSpeed, -maxGas, maxGas);
        motor.maxMotorTorque = motorTorque * drivePower;
        joint.motor = motor;
        translation = joint.jointTranslation;  
        UpdateSpringSound(translationRatio);
    }
    public void Brake(float brakeIntensity, float motorTorque, float translationRatio){
        JointMotor2D motor = joint.motor;
        float currentBrakePower = brakePower * brakeIntensity;

		motor.motorSpeed = 0; // el target! no significa que lo vaya a conseguir

		motor.maxMotorTorque = motorTorque * currentBrakePower;
        joint.motor = motor;
        translation = joint.jointTranslation;
        UpdateSpringSound(translationRatio);
    }
    public void UpdateSpringSound(float translationRatio){
        var diff = Mathf.Abs(translation - joint.jointTranslation);
        if(springAudioManager)
            springAudioManager.noise.input = diff * translationRatio;
    }
//    public float GetBreakPower(float bodyRotationFactor){
//        float bp = (brakePower - (brakeToBodyRotationRatio * bodyRotationFactor)) - brakeToBodyRotationInverter;
//        return bp;
//    }
}
