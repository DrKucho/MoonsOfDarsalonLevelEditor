using UnityEngine;
using System.Collections;

public class OffsetScroller : MonoBehaviour {
	public Renderer rend;
	public Vector2 size; // el tamaño de la textura 
	public Vector2 inc; //  20 mueve un pixel real por frame , 10 cada dos frames, 5 cada 4 etc
	public Vector2 pixelFactor; // offseteando este valor, equivale a 1 pixel en la textura
	public Vector2 offsetInc; 
	public Vector2 offset; // el offset aplicado a la textura

	public void InitialiseInEditor(){
		Initialize();
	}
	void Awake(){
		Initialize();
	}
	public void Initialize(){
		if (!rend)
            rend = GetComponent<Renderer>();
		if (rend)
		{
			size.x = (float) rend.sharedMaterial.mainTexture.width;
			size.y = (float) rend.sharedMaterial.mainTexture.height;
            transform.localScale = new Vector3(size.x, size.y, 1);
			pixelFactor.x = 1 / size.x;
			pixelFactor.y = 1 / size.y;
			offsetInc.x = inc.x / size.x;
			offsetInc.y = inc.y / size.y;
		}
	}

	public void MyUpdate()
	{ // lo llama skymananger
		offset.x = Mathf.Repeat(KuchoTime.time * offsetInc.x, 1);
		offset.y = Mathf.Repeat(KuchoTime.time * offsetInc.y, 1);
		SetOffset(offset);
	}

	public void SetOffset(Vector2 _offset){
        rend.sharedMaterial.mainTextureOffset = _offset;
    }
}
