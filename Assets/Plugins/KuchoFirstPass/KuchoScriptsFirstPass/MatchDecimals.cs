using UnityEngine;
using System.Collections;

public class MatchDecimals : MonoBehaviour {

	
	public Transform match;
	public Vector2 matching; // asi puedo verlo en inspector y saber si lo esta haciendo bien
	public Vector2 offset;
	public bool each2 = false;
	
	private Transform t;
	private Point point; // para truncar
	private Vector2 dec; // los decimales
	
	public void Start(){
		t = transform;
	}
	
	public void Update () {
		matching = match.localPosition;
		point.x = (int)matching.x;
		point.y = (int)matching.y;
		dec.x = match.localPosition.x - point.x;
		dec.y = match.localPosition.y - point.y;
		
		if(each2)
		{
			// si no es divisible por 2
			if ( point.x % 2 != 0 ) point.x = 1;
			else point.x = 0;
			// si no es divisible por 2
			if ( point.y % 2 != 0 ) point.y = 1;
			else point.y = 0;
		}
		else
		{
			point.x = 0;
			point.y = 0;
		}
		
		t.localPosition = new Vector3(point.x + dec.x + offset.x, t.localPosition.y, t.localPosition.z);
		t.localPosition = new Vector3(t.localPosition.x, point.y + dec.y + offset.y, t.localPosition.z);
	}
}
