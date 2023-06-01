using UnityEngine;
using System.Collections;

public class DeactivateThisGameObjectOnPlay : MonoBehaviour {

	void Awake(){
		if (Application.isPlaying) gameObject.SetActive(false);
	}
}
