using UnityEngine;
using System.Collections;

[System.Serializable]
public class SmartCollider{
	public Collider2D col;
	public bool willGetHit = false;
	public Hit hit;
	public Rect rect;

	public void DefineRect(){
		DefineRect(Vector4.zero);
	}
	public void DefineRect(Vector4 offset){
		rect = new Rect (col.bounds.min.x + offset.x, col.bounds.min.y + offset.y, col.bounds.size.x + offset.z, col.bounds.size.y + offset.w);	
	}
	public Rect GetRectPlusOffset(Vector4 offset){
		return new Rect (col.bounds.min.x + offset.x, col.bounds.min.y + offset.y, col.bounds.size.x + offset.z, col.bounds.size.y + offset.w);
	}
	public void SetHitPoint(Vector2 worldHitPoint){
		hit.point = worldHitPoint - (Vector2)col.bounds.center;
		if (hit.point.x >= 0){
			hit.left = false; 
			hit.right = true;
		}
		else {
			hit.left = true; 
			hit.right = false;		
		}
		if (hit.point.y >= 0){
			hit.up = true;
			hit.down = false;
		}
		else {
			hit.up = false;
			hit.down = true;	
		}
	}
}
[System.Serializable] // estas dos no deberian ir dentro de la _GreyColorartCollider??
public class Hit{
	public Vector2 point;
	public bool left;
	public bool right;
	public bool up;
	public bool down;
}
[System.Serializable]
public class walkerSmartCollider{
	public SmartCollider normal;
	public SmartCollider duck;
}

