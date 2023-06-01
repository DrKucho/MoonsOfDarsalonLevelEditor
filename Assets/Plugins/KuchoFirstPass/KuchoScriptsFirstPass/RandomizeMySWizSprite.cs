using UnityEngine;
using System.Collections;

public class RandomizeMySWizSprite : MonoBehaviour {

	public int minSprID = 0;
	public int maxSprID = 10;
	public bool flipSpriteRandomly = false;
	public float shiftOnFlipSprite;
	
	public void Start(){
		var spr = GetComponent<SWizSprite>();
		spr.SetSprite(Mathf.RoundToInt(Random.Range(minSprID, maxSprID)));
	}
	public void Flip(){
		if (flipSpriteRandomly){
			float c = Random.Range(100f,-100f);
			c = Mathf.Sign(c);
			if (c == -1){
				transform.localScale = new Vector3(c, transform.localScale.y, transform.localScale.z);
				if (shiftOnFlipSprite != 0) TransformHelper.SetPosX(transform, transform.position.x + shiftOnFlipSprite);
			}
		}
	}
}
