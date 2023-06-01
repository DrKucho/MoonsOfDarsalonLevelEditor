using UnityEngine;
using System.Collections;

public class Weight : MonoBehaviour {

	private GameObject child;
//	private EnemyController eC;
	
	public void Start(){ //  print(this + "START ");

		child = GetComponentInChildren<Transform>().gameObject;
//		eC = GetComponentInParent<EnemyController>();
//		child.GetComponent<SWizTextMesh>().text = eC.general.weight.ToString();
	}
	
	public void Update(){ //  print (this + " UPDATE ");
		if (ScenesAndDifficultyManager.levelDiff.debug){
			child.SetActive(true);
			print ( this + " WEIGHT ACTIVADO");
		}
		else {
			child.SetActive(false);
			print ( this + " WEIGHT APAGADO");
		}
	}
}
