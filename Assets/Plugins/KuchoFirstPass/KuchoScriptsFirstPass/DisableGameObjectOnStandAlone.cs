using UnityEngine;
using System.Collections;

public class DisableGameObjectOnStandAlone : MonoBehaviour {

	void Start(){
		if (!Application.isEditor) gameObject.SetActive(false);
	}

}
