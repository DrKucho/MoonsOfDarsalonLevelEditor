using UnityEngine;
using System.Collections;

public class PlayerBulletCollisioner : MonoBehaviour {

	public float activateDelay = 0f;
	public float deactivateDelay = 0f;
	private Vision activate;
	private Vision deactivate;
	private Collider2D col;
	
	public void Awake () {
		activate = transform.Find("Activate").gameObject.GetComponent<Vision>();
		deactivate = transform.Find("Deactivate").gameObject.GetComponent<Vision>();
		col = GetComponent<Collider2D>();

		VisibleObjectList list;
		if (activate)
		{
			list = activate.GetVisibleObjectListByType(VisibleObjectType.Switch);
			if (list)
			{
				list.onDetected = OnSomethingEntersActivateCollider;
				list.onSomethingIsGone = OnSomethingExitsActivateCollider;
			}
		}
		if (deactivate)
		{
			list = deactivate.GetVisibleObjectListByType(VisibleObjectType.Switch);
			if (list)
			{
				list.onDetected = OnSomethingEntersDeactivateCollider;
			}
		}
	}
	
	public IEnumerator Enable(){
		yield return new WaitForSeconds(activateDelay);
		col.enabled = true;
	}
	public IEnumerator Disable(){
		yield return new WaitForSeconds(deactivateDelay);
		col.enabled = false;
	}
	public void OnSomethingEntersActivateCollider(VisibleObjectList list, Collider2D col){ // acaba de entrar en el rango de vision grande
		StartCoroutine(Enable());
	}
	public void OnSomethingExitsActivateCollider(VisibleObjectList list, Collider2D col){  // acaba de salir del rango de vision grande
		StartCoroutine(Disable());
	}
	public void OnSomethingEntersDeactivateCollider(VisibleObjectList list, Collider2D col){ // acaba de entrar en el rango de vision pequeño
		StartCoroutine(Disable());
	}
}
