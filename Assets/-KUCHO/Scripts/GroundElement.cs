using UnityEngine;
using System.Collections;

public class GroundElement : MonoBehaviour {

//    	private GameObject para;
//    	private Parallax paraSC;
//    	public SWizSprite sprite;
//    	private float r;
//    	private float g;
//    	private float b;
//    	private float y;
//    	
//    	public void Start(){ //  print(this + "START ");
//
//    		para = GameObject.Find("P-A-R-A-L-A-X");
//    		paraSC = para.GetComponent<Parallax>();
//    		sprite = GetComponent<SWizSprite>();
//    		// int max= paraSC.parallaxElements.length;
//    		y = transform.position.y - para.transform.position.y;
//    		r = sprite.color.r;
//    		g = sprite.color.g;
//    		b = sprite.color.b;
//    		ApplyGradient();
//    	}
//    	public void Update(){ //  print (this + " UPDATE ");
//    		ApplyGradient();	// por que esto esta en update? no basta con que esté en start? seguramente sea para ir probando desde editor y ya lo bueno borrar BORRAME DEBERIA FUNCIONAR IGUAL
//    	}
//    	public void ApplyGradient(){
//    		float multiplier = paraSC.colorDarken/ 100f;
//    		Color newColor = sprite.color;
//    		newColor.r = (r - (y * multiplier));
//    		newColor.g = (g - (y * multiplier));
//    		newColor.b = (b - (y * multiplier));
//    		sprite.color = newColor;
//    	}
}
