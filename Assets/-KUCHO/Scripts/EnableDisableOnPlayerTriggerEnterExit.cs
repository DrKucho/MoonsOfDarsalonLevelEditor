using UnityEngine;
using System.Collections;

public class EnableDisableOnPlayerTriggerEnterExit : MonoBehaviour
{

	public bool vehicleInsteadPlayer = false;
	public GameObject activeOnTrigger;
	public bool enterMeansEnable = true;
    public GameObject reverseActivation;


	void Start(){ //  print(this + "START ");

		activeOnTrigger.SetActive(!enterMeansEnable);
	}

	void OnTriggerEnter2D (Collider2D col) {
		TriggerColliders trigCol = col.GetComponent<TriggerColliders>();
		if (trigCol)
		{
			if ((!vehicleInsteadPlayer && trigCol.cC == Game.playerCC) || (vehicleInsteadPlayer && trigCol.vehicle))
			{
				activeOnTrigger.SetActive(enterMeansEnable);
				if (reverseActivation)
					reverseActivation.SetActive(!enterMeansEnable);
			}
		}
	}
	void OnTriggerExit2D (Collider2D col) {
		TriggerColliders trigCol = col.GetComponent<TriggerColliders>();
		if (trigCol)
		{
			if ((!vehicleInsteadPlayer && trigCol.cC == Game.playerCC) || (vehicleInsteadPlayer && trigCol.vehicle))
			{
				activeOnTrigger.SetActive(!enterMeansEnable);
				if (reverseActivation)
					reverseActivation.SetActive(enterMeansEnable);
			}
		}
	}
}
