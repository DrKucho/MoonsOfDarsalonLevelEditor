using UnityEngine;
using System.Collections;

public class WaterGroup : MonoBehaviour {

	public GameObject waterParticlePrefab;
	
	public void Start(){
		Collider2D col = GetComponent<Collider2D>();
		for (float x = col.bounds.min.x; x < col.bounds.max.x ; x++){
			for (float y = col.bounds.min.y; y < col.bounds.max.y ; y++){
				GameObject part = Instantiate(waterParticlePrefab, new Vector3(x, y, transform.position.z), Constants.zeroQ) as GameObject;
				part.transform.parent = transform;
			}
		}
	}
}
