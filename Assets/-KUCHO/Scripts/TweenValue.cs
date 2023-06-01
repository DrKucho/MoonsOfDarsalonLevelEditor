using UnityEngine;
using System.Collections;

public class TweenValue : MonoBehaviour {

	Renderer rend;
	Material mat;
	public Material[] materials;

	int _Val;
	public float min;
	public float max;
	public float inc;
	public float delay;

	void Awake () {
		rend = GetComponent<Renderer>();
		if (rend) mat = rend.material;
		_Val = Shader.PropertyToID("_Val");
	}
	void OnEnable(){ //  print(this + " ONENABLE ");
		StartCoroutine(MyUpdate());
	}
	void OnDisable(){
		StopCoroutine(MyUpdate());
	}
	
	IEnumerator MyUpdate () {
		float newHue = float.MinValue;
		float _inc = Mathf.Abs(inc);
		while(this.enabled){
			_inc =  Mathf.Abs(inc);
			while (newHue < max)
			{
				newHue = GetIncSet(_inc);
				yield return null;
			}
			_inc *= -1;
			while (newHue > min)
			{
				newHue = GetIncSet(_inc);
				yield return null;
			}
			yield return new WaitForSeconds(delay);
		}
	}
	float GetIncSet(float _inc){
		float hue;
		float newHue;
		hue = mat.GetFloat(_Val);
		newHue = hue + _inc;
		mat.SetFloat(_Val, newHue);
		for (int i = 0; i < materials.Length; i++){
			materials[i].SetFloat(_Val, newHue);
		}
		return newHue;
	}
}
