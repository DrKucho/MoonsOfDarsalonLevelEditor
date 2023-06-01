using UnityEngine;
using System.Collections;

public class ForceToTag : MonoBehaviour {

	public bool debug = false;
	public string tagToMagnet = "GoodGuys";
	private Collider2D goal;
	public float forceMult=0.1f;
	public Vector2 force;
	private Vector2 dist;
	private Vector2 dir;
	private Rigidbody2D rb;

	void Awake () {
		rb = GetComponentInParent<Rigidbody2D>();
	}
	void OnEnable(){ // se llama desde pickup()
		if(debug) print (this + "INICIALIZANDO");

	}
	
	
	void OnTriggerEnter2D(Collider2D col){
		if (col.CompareTag(tagToMagnet)){
			if (debug) print (this + " PLAYER ENTER ");
			AddForceTo(col);
		}
	}
	void OnTriggerStay2D(Collider2D col){
		if (col.CompareTag(tagToMagnet)) AddForceTo(col);
	}
	void AddForceTo(Collider2D col){
		dist = (col.bounds.center - transform.position);
		dir = dist.normalized;
		force = dir * forceMult;
		rb.AddForce(force);
	}

}
