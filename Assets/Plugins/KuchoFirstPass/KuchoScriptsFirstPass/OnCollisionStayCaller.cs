using UnityEngine;
using System.Collections;

public class OnCollisionStayCaller : MonoBehaviour {

	
	public delegate void MyDelegate(Collision2D collision);
	public MyDelegate myDelegate;
	
	public void OnCollisionStay2D (Collision2D collision) {
		if (myDelegate != null) myDelegate(collision);
	}
}
