using UnityEngine;
using System.Collections;

public class BlinkFlash : MonoBehaviour {

	private Vision activate;
	private Vision deactivate;
	private Light lit;
	public float maxIntensity = 8f;
	public float inc = 0.25f;
	private float inc2;
	private float originalIntensity;
	private bool blinking = false;
	
	public void Awake () {
		activate = transform.Find("Activate").gameObject.GetComponent<Vision>();
		if (transform.Find("Deactivate")) deactivate = transform.Find("Deactivate").gameObject.GetComponent<Vision>();
		lit = GetComponentInChildren<Light>();
		VisibleObjectList list;
		if (activate)
		{
			list = activate.GetVisibleObjectListByType(VisibleObjectType.Switch);
			if (list)
			{
				list.onDetected = ActivateDelegate;
				list.onSomethingIsGone = ExitOfActivateDelegate;
			}
		}
		if (deactivate)
		{
			list = deactivate.GetVisibleObjectListByType(VisibleObjectType.Switch);
			if (list)
			{
				list.onDetected = DeactivateDelegate;
			}
		}
	}
	public void Start(){ //  print(this + "START ");

		originalIntensity = lit.intensity;
		inc2 = inc;
	}
	
	public void Update(){ //  print (this + " UPDATE ");
		if (blinking){
			if (inc2 > 0){ //going up
				if (lit.intensity >= maxIntensity) inc2 = -inc;
			}
			else if (inc2 < 0){
				if (lit.intensity <= originalIntensity) inc2 = inc;
			}
			lit.intensity += inc2;
		}
		else{ // no esta blinking
			if ( lit.intensity > originalIntensity){
				lit.intensity -=inc;
				if (lit.intensity < originalIntensity) lit.intensity = originalIntensity;
			}
		}
	}
	public void ActivateDelegate(VisibleObjectList list, Collider2D col){ // acaba de entrar en el rango de vision grande
		blinking = true;
	}
	public void ExitOfActivateDelegate(VisibleObjectList list, Collider2D col){// acaba de salir del rango de vision grande
		blinking = false;
	}
	public void DeactivateDelegate(VisibleObjectList list, Collider2D col){ // acaba de entrar en el rango de vision pequeño
		blinking = false;
	}
}
