using UnityEngine;
using System.Collections;

public class PivotToObject : MonoBehaviour {

	public Transform pivotPoint;
	public Transform _object;
	public float distance;
	
	public void Start(){ //  print(this + "START ");

		if (!pivotPoint) pivotPoint = transform.parent;
		if (!_object) _object = Game.playerGO.transform;
	}
	public void Update(){ //  print (this + " UPDATE ");
		Vector3 fakePos = pivotPoint.position;
		fakePos.z = _object.transform.position.z; // si le pongo el mismo z que al target, el vector me es mas propicio
		Vector3 dir = (_object.transform.position - fakePos).normalized;
		transform.position = pivotPoint.position + (dir * distance);
	}
}
