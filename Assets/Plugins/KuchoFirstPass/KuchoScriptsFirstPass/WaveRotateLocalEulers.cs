using System;
using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;

using RangeInt = UnityEngine.RangeInt;

//using System.Net.Configuration;

public class WaveRotateLocalEulers : MonoBehaviour
{
	[Serializable]
	public class RotationAxis
	{
		public bool enabled;
		[Tooltip("solo uno debe tener un 1, si no funciona bien es porque hay alguna inversion en la escala PON -1")]public Vector3 axis;
		[ReadOnly2Attribute] public float rot;

		[Range(0, 40)] public float rotationSpeed = 20f;
		[Range(0, 1)] public float acceleration = -1;
		public int startDir = 1;
		public float maxAngle = 20;
		[ReadOnly2Attribute] public float _rotSpeedGoal;
		[ReadOnly2Attribute] public float _currRotSpeed;

		public void Initialise(Transform transform)
		{
			rotationSpeed = Math.Abs(rotationSpeed); // no puede ser negativa 
			if (startDir < 0)
				startDir = -1;
			else
				startDir = 1;
			_rotSpeedGoal = rotationSpeed * startDir;

			GetRot(transform.localRotation.eulerAngles);

			if (Mathf.Sign(rot) != Mathf.Sign(_rotSpeedGoal))
				_rotSpeedGoal *= -1;
		}

		void GetRot(Vector3 eulers)
		{
			if (axis.x != 0)
			{
				rot = eulers.x;
			}
			else if (axis.y != 0)
			{
				rot = eulers.y;
			}
			else if (axis.z != 0)
			{
				rot = eulers.z;
			}
			else
				rot = 0;

			if (rot > 180)
				rot = rot - 360;

		}
	}

	public bool debug;
	public Transform referenceParent;
	public bool referenceParentInversion;
	public RotationAxis[] rotations;

	private Transform myTrans;


	private void OnValidate()
	{
		myTrans = transform;

		if (rotations != null)
		{
			foreach(RotationAxis r in rotations)
				r.Initialise(myTrans);
		}
	}

}
