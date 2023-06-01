using UnityEngine;
using System.Collections;

public class DisableMeOnPlayerTriggerEnter : MonoBehaviour {

	public GameObject target;

	void OnTriggerEnter2D (Collider2D col) {
		TriggerColliders trigCol = col.GetComponent<TriggerColliders>();
		if (trigCol && trigCol.cC == Game.playerCC)
		{
            if (target)
                target.SetActive(false);
            else
                gameObject.SetActive(false);
		}
	}
}
