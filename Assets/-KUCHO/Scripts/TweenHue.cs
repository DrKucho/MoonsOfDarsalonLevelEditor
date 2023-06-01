using UnityEngine;
using System.Collections;

public class TweenHue : MonoBehaviour {

	Renderer rend;
	Material mat;
	public Material[] materials;
	int _Hue;
	public float min;
	public float max;
	public float inc;

	void Start(){ //  print(this + "START ");

		rend = GetComponent<Renderer>();
		if (rend) mat = rend.material;
		_Hue = Shader.PropertyToID("_HueShift");
	}
	
	void Update(){ //  print (this + " UPDATE ");
		float hue = mat.GetFloat(_Hue);
		float newHue = hue + inc;
		if (newHue > max)
            inc *= -1;
		else if (newHue < min)
            inc *= -1;
		mat.SetFloat(_Hue, newHue);
		for (int i = 0; i < materials.Length; i++){
			materials[i].SetFloat(_Hue, newHue);
		}
	}
}
