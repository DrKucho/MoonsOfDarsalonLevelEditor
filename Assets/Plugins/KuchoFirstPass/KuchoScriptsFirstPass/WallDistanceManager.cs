using UnityEngine;
using System.Collections;

public class WallDistanceManager : MonoBehaviour {

	public bool debug = false;
	public float forceOnWallHit = -5f;
	public float forceOnStopped = -10f;
	public bool triggerEventsAsWell = false;
	float _forceMult = -5f;
	Vector2 distToCollider;
	Vector2 force;
	Vector2 pos;
	Vector2 previousPos;
	Vector2 posDif;
	Rigidbody2D rb;
	Collider2D _collider2D;
	Vector2[] distToContactPoints = new Vector2[3];
	int c; // El numero de contactos de la colision
	
	public void Awake () {
		rb = GetComponentInParent<Rigidbody2D>();
		_collider2D = GetComponent<Collider2D>();
	}
	public void OnCollisionEnter2D(Collision2D coll){
		if (debug) print (this + " ENTER");
		ProcessCollision(coll);
	}
	public void OnCollisionStay2D(Collision2D coll){
		if (debug) print (this + " STAY");
		ProcessCollision(coll);
	}
	public void OnCollisionExit2D(Collision2D coll){
		if (debug) print (this + " EXIT");
		c = 0;
		_forceMult = 0;
	}   
	ContactPoint2D[] contacts = new ContactPoint2D[30];
	public void ProcessCollision(Collision2D coll){
		int contactCount = coll.GetContacts(contacts);
		ProcessPositionDiference();
		for (int c = 0; c < contactCount;c++)//for (c = 0; c < coll.contacts.Length; c++)
        {
            if (c < distToContactPoints.Length)
                distToContactPoints[c] = contacts[c].point - (Vector2)_collider2D.bounds.center;
            else
                break;
		}
		for ( int i = 0; i < c; i++)
        {
            if (c < distToContactPoints.Length)
            {
                force = distToContactPoints[i] * _forceMult;
                if (debug)
                    print("FORCE = " + force);
                rb.AddForce(force);
            }
            else
                break;
		}
		previousPos = rb.position;
	}
	public void ProcessPositionDiference(){
		pos = rb.position;
		posDif = pos - previousPos;
		if (posDif.x <= 1 && posDif.y <= 1) {
			if (debug) print (" QUIETOOOOR!");
		 	_forceMult = forceOnStopped;
		}
		else _forceMult = forceOnWallHit;
		previousPos = rb.position;
	}
	public void OnTriggerEnter2D(Collider2D col){
		if (triggerEventsAsWell){
			if (debug) print (this + " ENTER");
		}
	}
	public void OnTriggerStay2D(Collider2D col){
		if (triggerEventsAsWell){
			if (debug) print (this + " STAY");
			ProcessPositionDiference();
			distToCollider = col.bounds.center - _collider2D.bounds.center;
			force = distToCollider * _forceMult;
			rb.AddForce(force);	
			previousPos = rb.position;
		}
	}
	public void OnTriggerExit2D(Collider2D col){
		if (triggerEventsAsWell){
			if (debug) print (this + " EXIT");
			_forceMult = 0;
		}
	}
}
