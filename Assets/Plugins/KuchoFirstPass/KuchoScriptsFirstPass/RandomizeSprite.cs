using UnityEngine;
using System.Collections;

public class RandomizeSprite : MonoBehaviour {

	public float snapOffsetX = 0f;
	public Sprite[] sprites;
	private SpriteRenderer sprRenderer;
	
	public void Start(){
		sprRenderer = GetComponent<SpriteRenderer>();
		int l = sprites.Length;
		int i = Random.Range(0,l);
		sprRenderer.sprite = sprites[i];
		//SnapTo.Pixel(transform.position); // ya viene snapeado por groundfiller
		TransformHelper.SetPosX(transform, transform.position.x + snapOffsetX);
		
	}
}
