using UnityEngine;
using System.Collections;

public class SnakeMonsterTorso : MonoBehaviour {

	
	public Rigidbody2D rb2D;
	
	public void Awake () {
		rb2D = GetComponent<Rigidbody2D>();
	}
	
	public void FixedUpdate () {
		TransformHelper.SetEulerAngleZ(transform, -rb2D.velocity.x);
	}
}
