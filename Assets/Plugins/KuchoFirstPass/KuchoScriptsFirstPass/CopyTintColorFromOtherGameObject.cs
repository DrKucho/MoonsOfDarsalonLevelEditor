using UnityEngine;
using System.Collections;

public class CopyTintColorFromOtherGameObject : MonoBehaviour {

	
	public GameObject otherGO;
	public string findThisGO="";
	
	public bool copyAlpha = false;
	
	private Color color;
	
	public void Start(){
        if (!otherGO)
            otherGO = GameObject.Find(findThisGO);
		
		if (otherGO)
		{
			Renderer otherRend = otherGO.GetComponent<Renderer>();
			SpriteRenderer otherSpr = otherGO.GetComponent<SpriteRenderer>();
			SWizSprite otherSWiz = otherGO.GetComponent<SWizSprite>();
		
			Renderer rend = GetComponent<Renderer>();
			SpriteRenderer spr = GetComponent<SpriteRenderer>();
			SWizSprite sWiz = GetComponent<SWizSprite>();
			
			if (otherSWiz) color = otherSWiz.color;
			else if (otherSpr) color = otherSpr.color;
			else if (otherRend) color = otherRend.material.color;
			
			float myAlpha = 0f;
			
			if (sWiz) myAlpha = sWiz.color.a;	
			if (spr) myAlpha = spr.color.a;
			if (rend && rend.material.HasProperty("_Color")) myAlpha = rend.material.color.a;
			
			if (copyAlpha == false) color.a = myAlpha; 
	
			if (sWiz) sWiz.color = color;
			if (spr) spr.color = color;
			if (rend && rend.material.HasProperty("_Color")) rend.material.color = color;
		}
	}
}
