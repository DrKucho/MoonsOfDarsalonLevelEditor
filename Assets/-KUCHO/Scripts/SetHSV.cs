using UnityEngine;
using System.Collections;

public class SetHSV : MonoBehaviour {

	Renderer rend;
	Material mat;
	[Range(-180, 180)]public float hue = 0;
	[Range(0, 2)]public float sat = 1;
	[Range(0, 2)]public float val = 1;

	void Start(){ //  print(this + "START ");

		rend = GetComponent<Renderer>();
		if (rend) mat = rend.material;
		if(mat)
		{
			if (mat.HasProperty(ShaderProp._Hue)) mat.SetFloat(ShaderProp._Hue, hue);
			if (mat.HasProperty(ShaderProp._Val)) mat.SetFloat(ShaderProp._Val, val);
			if (mat.HasProperty(ShaderProp._Sat)) mat.SetFloat(ShaderProp._Sat, sat);
		}
	}
}
