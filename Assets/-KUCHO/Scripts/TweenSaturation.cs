using UnityEngine;
using System.Collections;

public class TweenSaturation : MonoBehaviour {

	Renderer rend;
	Material mat;
	int _Sat;

	public Material[] materials;
	public float min;
	public float max;
	public float inc;

	void Start(){ //  print(this + "START ");

		rend = GetComponent<Renderer>();
		if (rend)
            mat = rend.material;
		_Sat = Shader.PropertyToID("_Sat");
	}
	
	void Update(){ //  print (this + " UPDATE ");
        float sat= mat.GetFloat(_Sat);
        float newSat = sat + inc;
		if (newSat > max)
            inc *= -1;
		else if (newSat < min)
            inc *= -1;
		mat.SetFloat(_Sat, newSat);
		for (int i = 0; i < materials.Length; i++)
        {
			materials[i].SetFloat(_Sat, newSat);
		}
	}
}
