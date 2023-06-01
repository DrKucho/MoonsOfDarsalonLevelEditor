using UnityEngine;
using System.Collections;

public static class TransformHelper{
	// SET EULER ANGLES PASANDO Y DEVOLVIENDO UN QUATERNION ROTATION--------------------------------------------------
	public static Quaternion SetEulerAngleX(Quaternion rotation, float angle){
		Quaternion tempQuat = rotation;
		tempQuat.eulerAngles = new Vector3 (angle, rotation.eulerAngles.y, rotation.eulerAngles.z);
		return tempQuat;
	}
	public static Quaternion SetEulerAngleY(Quaternion rotation, float angle){
		Quaternion tempQuat = rotation;
		tempQuat.eulerAngles = new Vector3 (rotation.eulerAngles.x, angle, rotation.eulerAngles.z);
		return tempQuat;
	}
	public static Quaternion SetEulerAngleZ(Quaternion rotation, float angle){
		Quaternion tempQuat = rotation;
		tempQuat.eulerAngles = new Vector3 (rotation.eulerAngles.x, rotation.eulerAngles.y, angle);
		return tempQuat;
	}
	// SET EULER ANGLES CON TRANSFORM---------------------------------------------------------------------------------
	public static void SetEulerAngleX(Transform transform, float angle){
		Quaternion tempQuat = transform.rotation;
		tempQuat.eulerAngles = new Vector3 (angle, tempQuat.eulerAngles.y, tempQuat.eulerAngles.z);
		transform.rotation = tempQuat;
	}
	public static void SetEulerAngleY(Transform transform, float angle){
		Quaternion tempQuat = transform.rotation;
		tempQuat.eulerAngles = new Vector3 (tempQuat.eulerAngles.x, angle, tempQuat.eulerAngles.z);
		transform.rotation = tempQuat;
	}
	public static void SetEulerAngleZ(Transform transform, float angle){
		Quaternion tempQuat = transform.rotation;
		tempQuat.eulerAngles = new Vector3 (tempQuat.eulerAngles.x, tempQuat.eulerAngles.y, angle);
		transform.rotation = tempQuat;
	}
	// SET LOCAL EULER ANGLES CON TRANSFORM----------------------------------------------------------------------------------
	public static void SetLocalEulerAngleX(Transform transform, float angle){
        var temp = transform.localEulerAngles;
        temp.x = angle;
        transform.localEulerAngles = temp;
	}
	public static void SetLocalEulerAngleY(Transform transform, float angle){
        var temp = transform.localEulerAngles;
        temp.y = angle;
        transform.localEulerAngles = temp;
    }
	public static void SetLocalEulerAngleZ(Transform transform, float angle){
        var temp = transform.localEulerAngles;
        temp.z = angle;
        transform.localEulerAngles = temp;
    }




	// SET LOCAL SCALE CON TRANSFORM------------------------------------------------------------------------------------------
	public static void FlipLocalScaleX(Transform transform){
		transform.localScale = new Vector3 (-transform.localScale.y, transform.localScale.y, transform.localScale.z);
	}
	public static void SetLocalScaleX(Transform transform, float x){
		transform.localScale = new Vector3 (x, transform.localScale.y, transform.localScale.z);
	}
	public static void SetLocalScaleY(Transform transform, float y){
		transform.localScale = new Vector3 (transform.localScale.x, y, transform.localScale.z);
	}
	public static void SetLocalScaleZ(Transform transform, float z){
		transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y, z);
	}




	// SET POSITION CON TRANSFORM--------------------------------------------------------------------------------------
	public static void SetPosX(Transform transform, float x){
		transform.position = new Vector3 (x, transform.position.y, transform.position.z);
	}
	public static void SetPosY(Transform transform, float y){
		transform.position = new Vector3 (transform.position.x, y, transform.position.z);
	}
	public static void SetPosZ(Transform transform, float z){
		transform.position = new Vector3 (transform.position.x, transform.position.y, z);
	}
	public static void SeyXYPositionswhileKeepZ(Transform transform, Vector2 pos)
	{
		transform.position = new Vector3(pos.x, pos.y, transform.position.z);
	}

	// SET LOCALPOSITION CON TRANSFORM---------------------------------------------------------------------------------
	public static void SetLocalPosX(Transform transform, float x){
		transform.localPosition = new Vector3 (x, transform.localPosition.y, transform.localPosition.z);
	}
	public static void SetLocalPosY(Transform transform, float y){
		transform.localPosition = new Vector3 (transform.localPosition.x, y, transform.localPosition.z);
	}
	public static void SetLocalPosZ(Transform transform, float z){
		transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, z);
	}
	public static void SeyXYLocalPositionswhileKeepZ(Transform transform, Vector2 pos)
	{
		transform.localPosition = new Vector3(pos.x, pos.y, transform.localPosition.z);
	}
}