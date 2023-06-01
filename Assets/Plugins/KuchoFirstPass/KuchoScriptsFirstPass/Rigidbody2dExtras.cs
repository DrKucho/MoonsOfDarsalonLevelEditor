using UnityEngine;
using System.Collections;

public enum DoItAt2 {Awake, Start, OnCall};

public class Rigidbody2dExtras : MonoBehaviour {

	public DoItAt2 doItAt;
	public Vector2 centerOfMass;
	public bool unParent = true;
	public Transform makeChildOf;
	
	private Rigidbody2D rb2d;
	
	void Awake(){
		rb2d = GetComponent<Rigidbody2D>();
		if (doItAt == DoItAt2.Awake) DoIt();
	}
	void Start(){
		if (doItAt == DoItAt2.Start) DoIt();
	}
	public void DoIt () {
		rb2d.centerOfMass = centerOfMass;
		if (makeChildOf) transform.parent = makeChildOf;
		else if (unParent) transform.parent = null;
	}

}
