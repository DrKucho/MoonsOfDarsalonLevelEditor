using UnityEngine;
using System.Collections;

public class EnableDisableOnPlayerTriggerExit : MonoBehaviour {

	public enum Action { Enable, Disable}
	public GameObject target;
	public Action action;

	void OnTriggerExit2D (Collider2D col) {
		if (col == Game.playerCol)
		{
			if (action == Action.Enable) target.SetActive(true);
			else target.SetActive(false);
		}
	}
}
