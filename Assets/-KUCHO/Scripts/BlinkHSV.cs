using UnityEngine;
using System.Collections;

public class BlinkHSV : MonoBehaviour {

	public Material mat;
	bool allGood = false;
	int blink = 1;

    [Range(0, 1)]public float speed = 0.5f;
	[Range(-180, 180)]public float hue1 = 0;
	[Range(0, 2)]public float sat1 = 1;
	[Range(0, 2)]public float val1 = 1;

	[Range(-180, 180)]public float hue2 = 0;
	[Range(0, 2)]public float sat2 = 1;
	[Range(0, 2)]public float val2 = 1;

	void Start(){

        if (!mat)
        {
            Renderer rend;
            rend = GetComponent<Renderer>();
            if (rend)
                mat = rend.material;
        }

		if (mat.HasProperty(ShaderProp._Hue) && mat.HasProperty(ShaderProp._Val) && mat.HasProperty(ShaderProp._Sat))
            allGood = true;
	}
	void OnEnable(){ 
        if (allGood)
            StartCoroutine(Blink());
	}
    IEnumerator Blink(){
        while (enabled)
        {
            if (blink == 1)
            {
                mat.SetFloat(ShaderProp._Hue, hue1);
                mat.SetFloat(ShaderProp._Val, val1);
                mat.SetFloat(ShaderProp._Sat, sat1);
                blink = 2;
            }
            else
            {
                mat.SetFloat(ShaderProp._Hue, hue2);
                mat.SetFloat(ShaderProp._Val, val2);
                mat.SetFloat(ShaderProp._Sat, sat2);
                blink = 1;
            }
            yield return KuchoWaitForSeconds.instance.oneSecLUT.Get(speed);
        }
	}
}
