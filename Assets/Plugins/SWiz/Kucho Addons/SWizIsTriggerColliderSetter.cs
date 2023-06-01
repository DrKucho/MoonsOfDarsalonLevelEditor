using UnityEngine;
using System.Collections;

public class SWizIsTriggerColliderSetter : MonoBehaviour {

	
	public bool setIsTriggerAtStart = false;
	private SWizSprite sprite;
	private Collider2D col;
	public void Start(){
		sprite = GetComponent<SWizSprite>();
		sprite.SpriteChanged += SetIsTrigger; // system event de SWiz
		if (setIsTriggerAtStart){
			SetIsTrigger(null);
		}
	}
	public void SetIsTrigger (SWizBaseSprite _basesprite) {
		col = GetComponent<Collider2D>(); // hay que pillarlo cada vez porque puede ser borrado
		if (col) col.isTrigger = true; 
	}
}
