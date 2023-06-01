using UnityEngine;
using System.Collections;
//using UnityEngine.UI;

public class ToggleAlpha : MonoBehaviour {

	public bool validate;
	public Renderer rend;
	public Material mat;
	//public Image img;
	float originalMatAlpha;
	float originalImgAlpha;

	void OnValidate(){
		rend = GetComponent<Renderer>();
		if (rend)
		{
			mat = rend.material;
			originalMatAlpha = mat.color.a;
		}
		/*
		img = GetComponent<Image>();
		if (rend)
		{
			originalImgAlpha = img.color.a;
		}
		*/

	}
	void Update () {
		if (Constants.isDebugBuild && Input.GetKeyDown(KeyCode.PageDown)) //Keyboard.current.pageDownKey.wasPressedThisFrame)// Input.GetKeyDown(KeyCode.PageDown))
		{
			if (mat)
			{
				if (mat.color.a == originalMatAlpha)
					ColorHelper.SetAlpha(mat, 1);
				else
					ColorHelper.SetAlpha(mat, originalMatAlpha);
			}
			/*
			if (img)
			{
				if (img.color.a == originalImgAlpha)
					ColorHelper.SetAlpha(img, 1);
				else
					ColorHelper.SetAlpha(img, originalImgAlpha);
			}
			*/
		}	
	}
}
