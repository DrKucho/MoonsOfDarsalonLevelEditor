using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UnTriggerizeMeOnHurt : MonoBehaviour {
	
	public bool debug = false;
	public float triggerBackDelay = 0.2f;
	
	Collider2D colliderBackup;
				
	DamageReceiver damageReceiver;
	Collider2D myCol;
	

}
