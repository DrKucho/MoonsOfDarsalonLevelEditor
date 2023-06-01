using UnityEngine;
using System.Collections;

public class SwapTrigger : MonoBehaviour {

	
	public bool init_IsTrigger = false;
	public float delay = 0.3f;
	private Collider2D col;
	
	public void Awake () {
		col = GetComponent<Collider2D>();
	}
	public void OnEnable(){
		col.isTrigger = init_IsTrigger;
		CancelInvoke();
		Invoke("DoIt", delay);
	}
	public void OnDisable(){
		CancelInvoke();
	}
	public void DoIt () {
		if (col) col.isTrigger = !col.isTrigger; 
	}
}
