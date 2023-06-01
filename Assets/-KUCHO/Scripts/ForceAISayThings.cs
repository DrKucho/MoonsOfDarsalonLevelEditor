using UnityEngine;
using System.Collections;

public class ForceAISayThings : MonoBehaviour {

	public string[] targetTags;
    public PhraseArray pa;
    public bool dontForceJustReplacePhrases = false;

	Collider2D myCollider;

	void OnValidate(){
		Initialize();
	}
	void Initialize(){
		if (!myCollider)
			myCollider = GetComponentInChildren<Collider2D>();
	}

}
