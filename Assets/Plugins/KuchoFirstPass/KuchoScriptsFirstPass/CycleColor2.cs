using UnityEngine;
using System.Collections;

public class CycleColor2 : MonoBehaviour {

	
	public Color[] colors;
	public float frequency;
	private int i = 0;
	private SWizSprite tkSprite;
	private SpriteRenderer unitySprite;
	
	public void Awake(){
		tkSprite = GetComponent<SWizSprite>();
		unitySprite = GetComponent<SpriteRenderer>();
	}
	public void Start(){
		InvokeRepeating ( "MyUpdate", 0.1f, frequency);
	}
	public void MyUpdate () {
		if (tkSprite) tkSprite.color = colors[i];
		if (unitySprite) unitySprite.color = colors[i];
		i++;
		if (i >= colors.Length) i = 0;
	}
}
