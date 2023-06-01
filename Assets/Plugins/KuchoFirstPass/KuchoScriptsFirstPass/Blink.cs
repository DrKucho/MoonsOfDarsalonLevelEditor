using UnityEngine;
using System.Collections;


public class Blink : MonoBehaviour {
    public Material mat;
	[HideInInspector] [SerializeField]  SWizSprite sprite;
	[HideInInspector] [SerializeField]  Rigidbody2D rb;
	[HideInInspector] [SerializeField] Renderer _renderer;
//	[HideInInspector] [SerializeField]  SWizSpriteMaterialAssigner matAssigner;

//	[HideInInspector] Material mat;
	[ReadOnly2Attribute] public float intensity = 0f;
	public float inc = 1f;
	public float max = 1;
	public float min = 0;
	public float delay = 2f;

	WaitForSeconds waitForSecondsDelay;

	public void InitialiseInEditor(){
		sprite = GetComponent<SWizSprite>();
        if (sprite)
    		_renderer = sprite.gameObject.GetComponent<Renderer>();
		rb = GetComponent<Rigidbody2D>();
//		matAssigner = GetComponent<SWizSpriteMaterialAssigner>();
        if (!mat)
            mat = _renderer.sharedMaterial;
	}
	void Awake(){
//		if (matAssigner)
//			mat = matAssigner.matToAssign;
		waitForSecondsDelay = new WaitForSeconds(delay);
	}
	public void OnEnable(){
		StartCoroutine( BlinkMaterialCycle() );
	}
	public void OnDisable(){
		intensity = 0;
		AssignValueToMaterial(intensity);
	}

	public IEnumerator BlinkMaterialCycle(){
		while (isActiveAndEnabled)
		{
			yield return waitForSecondsDelay;

			if ((rb && rb.velocity == Constants.zero2) || !rb)
			{
				while (intensity < max)
				{
					intensity += inc;
					if (intensity > max)
						intensity = max;
					AssignValueToMaterial(intensity);
					yield return null;
				}
				while (intensity > min)
				{
					intensity -= inc;
					if (intensity < min)
						intensity = min;
					AssignValueToMaterial(intensity);
					yield return null;
				}
			}
		}
	}
	public void AssignValueToMaterial(float value){
        if (mat.HasProperty(ShaderProp._Add))
            mat.SetFloat(ShaderProp._Add, value);
        else if (mat.HasProperty(ShaderProp._Val))
            mat.SetFloat(ShaderProp._Val, value);
	}
}
