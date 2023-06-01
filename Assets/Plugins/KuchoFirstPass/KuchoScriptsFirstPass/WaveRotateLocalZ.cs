using System;
using UnityEngine;
using System.Collections;
using System.ComponentModel;

	
//using System.Net.Configuration;

public class WaveRotateLocalZ : MonoBehaviour
{
	public bool debug;

	[Range(0,300)] public float rotationSpeed = 0.1f;
	[Range(0,1)]public float acceleration = -1;
	public float maxAngle = 20;
	public Transform useMyScaleXasMult;
	public bool invertMult;
	[ReadOnly2Attribute] public float _rotSpeedGoal;
	[ReadOnly2Attribute] public float _currRotSpeed;
	[ReadOnly2Attribute] public float _currRotSpeedSign;
	[ReadOnly2Attribute] public float _rotSpeedSign;
	[ReadOnly2Attribute] public float scaleMult;

	private void Awake()
	{
		CalculateThings();
	}

	private void OnValidate()
	{
		CalculateThings();
	}
	private void CalculateThings()
	{
		if (!useMyScaleXasMult)
			useMyScaleXasMult = transform;
		scaleMult = Mathf.Sign(useMyScaleXasMult.localScale.x);
		if (invertMult)
			scaleMult *= -1;
		rotationSpeed = Math.Abs(rotationSpeed); // no puede ser negativa 
		_rotSpeedGoal = rotationSpeed * scaleMult;
		_rotSpeedSign = Mathf.Sign(_rotSpeedGoal);
	}
	public void Update ()
	{
		var eulers = transform.localRotation.eulerAngles;
		var rot = eulers.x;
		if (rot > 180)
			rot = rot - 360;
		if (_rotSpeedSign == scaleMult && rot > maxAngle)
		{
			_rotSpeedGoal = rotationSpeed * scaleMult;
			_currRotSpeedSign = Mathf.Sign(_rotSpeedGoal);
			if (debug)
				Debug.Log("ROT=" + rot + " ROT SPD=" + _rotSpeedGoal);
		}
		else if (_rotSpeedSign == scaleMult && rot < -maxAngle)
		{
			_rotSpeedGoal = -rotationSpeed * scaleMult;
			_currRotSpeedSign = Mathf.Sign(_rotSpeedGoal);
			if (debug)
				Debug.Log("ROT=" + rot + " ROT SPD=" + _rotSpeedGoal);
		}

		if (debug)
			Debug.Log("ROT=" + rot);
		
		if (acceleration > 0)// valores negativos de aleceracion significa cambio de velocidad instantaneo
		{
			if (_rotSpeedGoal > 0)
			{
				if (_currRotSpeed < _rotSpeedGoal)
				{
					_currRotSpeed += acceleration * _currRotSpeedSign;
				}
				else
				{
					_currRotSpeed = _rotSpeedGoal;
				}
			}
			else
			{
				if (_currRotSpeed > _rotSpeedGoal)
				{
					_currRotSpeed += acceleration * _currRotSpeedSign;
				}
				else
				{
					_currRotSpeed = _rotSpeedGoal;
				}
			}
		}
		else
		{
			_currRotSpeed = _rotSpeedGoal;
		}
		
		Vector3 axis;
		axis.x = 0;
		axis.y = 0;
		axis.z = 1;
		transform.Rotate(axis, _currRotSpeed * KuchoTime.unityDeltaTime, Space.Self);
	}
}
