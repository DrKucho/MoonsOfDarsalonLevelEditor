using UnityEngine;
using System.Collections;


public class GetSetRootSortingOrder : MonoBehaviour {

	public enum PickParent {Parent, GrandParent, Root}
	public PickParent pickparent = PickParent.Root;

	public void Start(){
		Renderer parentRenderer = null;

		switch (pickparent)
		{
			case (PickParent.Parent):
				parentRenderer = transform.parent.gameObject.GetComponent<Renderer>();
				break;
			case (PickParent.GrandParent):
				parentRenderer = transform.parent.parent.gameObject.GetComponent<Renderer>();
				break;
			case (PickParent.Root):
				parentRenderer = transform.root.gameObject.GetComponent<Renderer>();
				break;
		}
		Renderer _renderer = GetComponent<Renderer>();
		if (parentRenderer)
		{
			_renderer.sortingLayerName = parentRenderer.sortingLayerName;
			_renderer.sortingOrder = parentRenderer.sortingOrder;
		}
	}
}
