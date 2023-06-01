using UnityEngine;
using System.Collections;

public class SetSkyColorToShader : MonoBehaviour {

	public Color max;
	public Color min;
	public Material mat;
	public float saturationBalance = 0.5f;

    public void InitialiseInEditor(){
		var matAssigner = GetComponent<SWizSpriteMaterialAssigner>();
        if (matAssigner)
        {
            mat = matAssigner.material;
        }
        else
        {
            var multiAssigner = GetComponentInParent<SWizStaticMultiMaterialAssigner>();
            if (multiAssigner)
                mat = multiAssigner.material;
            else
            {
                mat = null;
                Debug.LogError(" NO ENCUENTRO MATERIAL ASSIGNER PARA PILLAR MATERIAL, ESTO VA A DAR ERROR");
            }
        }

	}

	void Update () {
		if (mat.HasProperty(ShaderProp._SkyColor))
		{
			Color sky = Color.Lerp(Game.skyManager.realSaturatedSkyColor, Game.skyManager.skyColor, saturationBalance);
			sky.r = Mathf.Clamp(sky.r, min.r, max.r);
			sky.g = Mathf.Clamp(sky.g, min.g, max.g);
			sky.b = Mathf.Clamp(sky.b, min.b, max.b);
			mat.SetColor(ShaderProp._SkyColor, sky);

		}
	}
}
