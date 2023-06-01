using UnityEngine;
using System.Collections;


public class Orb : MonoBehaviour {


	public Light2DManager l;
	public RandomFloat dischargeRate;
	public RandomFloat spinAnimSpeed;

	public float oscAlphaFactor;
	[ReadOnly2Attribute] public Item item;
	[ReadOnly2Attribute] public SWizSprite sprite;
	[ReadOnly2Attribute] public SWizSpriteAnimator anim;
	[ReadOnly2Attribute] public Color originalColor;
	
	public void InitialiseInEditor () {
		item = GetComponent<Item>();
		sprite = GetComponentInChildren<SWizSprite>();
		anim = GetComponentInChildren<SWizSpriteAnimator>();
		originalColor = sprite.color;
	}
}
