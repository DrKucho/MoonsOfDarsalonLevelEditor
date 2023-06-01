using UnityEngine;
using System.Collections;

public class Elbow : MonoBehaviour {

	public Transform head;
	public Transform torso;
	public float forceMult = 1f;
	public float distToHead;
	public float distToTorso;
	public Vector2 force;
	private Rigidbody2D rb2D;
	
	public void Awake () {
		rb2D = GetComponent<Rigidbody2D>();
	}
	
	public void Update () {
		if(head) distToHead = (head.position - transform.position).sqrMagnitude;
		if(torso) distToTorso = (torso.position - transform.position).sqrMagnitude;
		if (distToTorso < distToHead) force=((torso.position - transform.position) * forceMult); else force = Constants.zero2;
		rb2D.AddForce(force);
	}
}
