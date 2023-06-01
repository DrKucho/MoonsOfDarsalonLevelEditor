using UnityEngine;
using System.Collections;

public class CycleColor : MonoBehaviour {

	private float[] c = {0f , 0f, 0f}; //color del texto que sube?
	private	 int index = 0;
	private float cinc = 0.5f;
	public float increment = 0.5f;
	private SWizSprite tkSprite;
	private SpriteRenderer unitySprite;
	
	public void Start(){
		cinc = increment;
		tkSprite = GetComponent<SWizSprite>();
		unitySprite = GetComponent<SpriteRenderer>();
	}
	
	public void Update () {
		c[index] += cinc;
		if (tkSprite) tkSprite.color = new Vector4(c[0], c[1], c[2], 0.8f);
		if (unitySprite) unitySprite.color = new Vector4(c[0], c[1], c[2], 0.8f);
		//print (c[0] +" "+ c[1] +" "+ c[2]);
		if (cinc > 0 && c[index] >= 1) index++;
		if (cinc < 0 && c[index] <= 0) index++;
		if (c[0] + c[1]+ c[2] >= 3) cinc *= -1; //{c[0] = 0;c[1] = 0;c[2] = 0;} 
		if (c[0] + c[1]+ c[2] <= 0) cinc *= -1;
		if (index == 3) index = 0;
	}
}
