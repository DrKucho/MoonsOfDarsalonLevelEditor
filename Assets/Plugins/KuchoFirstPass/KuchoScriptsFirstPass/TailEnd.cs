using UnityEngine;
using System.Collections;

public class TailEnd : MonoBehaviour {

	public Transform head;
	public int areaW;
	public float forceMult = 2f;
	public float tresspassing;
	public Vector3 force;
	public Rigidbody2D rb2D;
	
	public void Awake () {
		rb2D = GetComponent<Rigidbody2D>();
	}
	
	public void FixedUpdate () {
		if (transform.position.y > head.position.y){
			if (rb2D.velocity.x < 0 && transform.position.x < head.position.x + areaW && transform.position.x >= head.position.x){ // entra en la zona de expulsion de derecha a izq
				tresspassing = transform.position.x - head.position.x + areaW;
				force = new Vector3((-rb2D.velocity.x - tresspassing) * forceMult ,0,0);
				//rb2D.AddForce(force);
				
			}
			else if (rb2D.velocity.x > 0 && transform.position.x > head.position.x - areaW && transform.position.x <= head.position.x){ // entra en la zona de expulsion de izq a dcha
				tresspassing = transform.position.x - head.position.x - areaW;
				force = new Vector3((-rb2D.velocity.x - tresspassing) * forceMult ,0,0);
				//rb2D.AddForce(force);
			}
			else tresspassing = 0f;
		}
	 }
}
