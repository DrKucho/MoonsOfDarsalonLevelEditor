using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundZ: MonoBehaviour {

    [Range(0,500)]public float speed;
    float rot ;
	void Update () {
        TransformHelper.SetLocalEulerAngleZ(transform, transform.localEulerAngles.z + speed * Time.deltaTime);
	}
}
