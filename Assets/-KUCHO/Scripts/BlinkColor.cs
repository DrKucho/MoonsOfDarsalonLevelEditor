using UnityEngine;
using System.Collections;

public class BlinkColor : MonoBehaviour {

	Renderer rend;
	Material mat;
	SWizTextMesh tm;
	int i = 0;

	public Color[] color;
	public float speed = 0.5f;

	void Start(){ //  print(this + "START ");

		tm = GetComponent<SWizTextMesh>();
		if (!tm)
		{
			rend = GetComponent<Renderer>();
			if (rend) mat = rend.material;
		}
	}
	void OnEnable(){ //  print(this + " ONENABLE ");
		CancelInvoke();
		if (mat) InvokeRepeating("BlinkMat", 0, speed);
		if (tm) InvokeRepeating("BlinkTm", 0, speed);
	}
	public void OnDisable(){
		CancelInvoke();
	}
	void BlinkMat(){
		mat.color = color[i];
		i++;
		if (i >= color.Length) i = 0;
	}
	void BlinkTm(){
		tm.color = color[i];
		i++;
		if (i >= color.Length) i = 0;
	}
}
