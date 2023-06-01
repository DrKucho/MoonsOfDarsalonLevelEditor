using UnityEngine;
using System.Collections;

public class ForceAIDissobey : MonoBehaviour {

	public string[] targetTags;
	Collider2D myCollider;

	void OnValidate(){
		Initialize();
	}

	void Initialize(){
		if (!myCollider)
			myCollider = GetComponentInChildren<Collider2D>();
	}

}
