using UnityEngine;
using System.Collections;

public class RearSuepension : MonoBehaviour {

	public enum RefAngle {Up, Down, Left, Right}
	public Transform point1;
	public Transform point2;
	public RefAngle _reference = RefAngle.Up; 
	Vector2 reference;

	public void OnValidate () {
		if (isActiveAndEnabled)
			SetReference();
	}

	void Awake(){
		SetReference();
	}

	void SetReference(){
		switch(_reference)
		{
			case (RefAngle.Up):
				reference = Vector2.up;
				break;
			case (RefAngle.Down):
				reference = Vector2.down;
				break;
			case (RefAngle.Left):
				reference = Vector2.left;
				break;
			case (RefAngle.Right):
				reference = Vector2.right;
				break;
		}
	}
	public void Update () {
		Vector3 mediumPoint = (point1.position + point2.position)/2;
		transform.position = new Vector3(mediumPoint.x, mediumPoint.y, transform.position.z);
//		float angle = Vector2.Angle(tire1.position, tire2.position);
		AbsFloat angle = KuchoHelper.GetAngleToTarget(mediumPoint, (Vector2)point2.position, reference);
		TransformHelper.SetEulerAngleZ(transform,  angle.signed);
	}
}
