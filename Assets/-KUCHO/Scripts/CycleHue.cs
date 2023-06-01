using UnityEngine;
using System.Collections;

public class CycleHue : MonoBehaviour
{

	public bool useSkyRenderer;
	Renderer rend;
	private SpriteRenderer sprRend;
	public Material[] materials;
	int _Hue;
	public float min;
	public float max;
	public float inc;

	void Start(){ //  print(this + "START ");

		if (useSkyRenderer)
			rend = SkyManager.instance.skyRenderer;
		else
			rend = GetComponent<Renderer>();
		sprRend = GetComponent<SpriteRenderer>();

		_Hue = Shader.PropertyToID("_HueShift");
	}

	private float currentHue;
	void Update(){ //  print (this + " UPDATE ");

		if (rend.sharedMaterial.HasProperty(_Hue))
		{
			float newHue = GetNewHue(rend.sharedMaterial.GetFloat(_Hue));
			rend.sharedMaterial.SetFloat(_Hue, newHue);
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i].SetFloat(_Hue, newHue);
			}
		}
		if(sprRend)
		{
			currentHue = GetNewHue(currentHue);
			Color c = sprRend.color;
			HSLColor chsl = HSLColor.FromRGBA(c);
			chsl.h = currentHue;
			sprRend.color = chsl.ToRGBA();
		}
	}

	float GetNewHue(float hue)
	{
		float newHue = hue + inc * KuchoTime.kuchoDeltaTime;
		if (newHue > max)
			newHue = min;
		else if (newHue < min)
			newHue = max;
		return newHue;
	}
}
