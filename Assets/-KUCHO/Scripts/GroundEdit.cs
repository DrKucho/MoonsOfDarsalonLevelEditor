using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundEdit : MonoBehaviour {

//	LPCorporeal corp;
//	List<Vector3> points;

	PolygonCollider2D poly;
	public GroundType groundType; 
	Terrain2D terrain2D;
	Texture2D alphaTex;
	Color[] pixels;
	Color[] pixels2;
	Point size;
	Point minPos;

	// Use this for initialization
	void OnValidate () {
//		corp = GetComponent<LPCorporeal>();
//		points = corp.GetPoints();
		poly = GetComponent<PolygonCollider2D>();
	}
	
	// Update is called once per frame
	public void CopyPixels () {
		switch (groundType)
		{
			case (GroundType.Destructible):
				terrain2D = WorldMap.destructible;
				break;
			case (GroundType.Indestructible):
				terrain2D = WorldMap.indestructible;
				break;
			case (GroundType.Background):
				terrain2D = WorldMap.background;
				break;
		}

		alphaTex = terrain2D.d2dSprite.AlphaTex;

		GetMinPosAndSize();

		pixels = alphaTex.GetPixels(minPos.x, minPos.y, size.x, size.y, 0);

//		pixels2 = alphaTex.GetPixels((int)bounds.min.x, (int)bounds.min.y, (int)bounds.size.x, (int)bounds.size.y, 0);

		TrimPixels();
	}
	/// <summary>
	/// Clean the outside of the polygon in the copied area.
	/// </summary>
	public void TrimPixels(){
		for (float y = 0; y < size.y; y++)
		{
			for (float x = 0; x < size.x; x++)
			{
//				if (!poly.OverlapPoint(new Vector2(x,y))); // esto estaba sin comentar y daba un warning, en principio no hacia nada ... por eso lo comenté
				int i = GetIndex(new Vector2(x, y));
				pixels[i].a = 0;
			}
		}
	}
	public void CutPoly(){
		GetMinPosAndSize();
		for (int y = minPos.y; y < minPos.y + size.y; y++)
		{
			for (int x = minPos.x; x < minPos.x + size.x; x++) 
			{
				Vector2 pos = new Vector2((float)x, (float)y);
				if (poly.OverlapPoint(pos)) // dentro del poligono ?
					terrain2D.d2dSprite.AlphaData[GetIndex(pos)] = 0; // borralo
			}
		}
		terrain2D.d2dSprite.AlphaTex.Apply(false, false);
		terrain2D.d2dSprite.NotifyChangesAndRebuildColliders();
//		terrain2D.d2dSprite.ReplaceOrUpdateAlphaWith(terrain2D.d2dSprite.AlphaData, alphaTex.width, alphaTex.height);
//		alphaTex.SetPixels((int)bounds.min.x, (int)bounds.min.y, (int)bounds.size.x, (int)bounds.size.y, pixels2, 0);
	}
	public void PastePixels(){
		alphaTex.SetPixels(pixels);
	}
	Vector2 GetCoords(int i){
		float width = size.x;
		float ifloat = i; // tiene que ser todo floats o si no los calculos se van a la mierda, si metes un int en una division el resultado pierde los decimales !!!???
		float yf = ifloat / width;
		float y = Mathf.Floor(yf);
		float x = (yf - y) * width;
		return new Vector2(x, y);
	}
	int GetIndex(Vector2 pos){
		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		return (int)(x + y * size.x); 
	}
	void GetMinPosAndSize(){
		minPos.x = Mathf.RoundToInt(poly.bounds.min.x);
		minPos.y = Mathf.RoundToInt(poly.bounds.min.y);
		size.x = Mathf.RoundToInt(poly.bounds.size.x);
		size.y = Mathf.RoundToInt(poly.bounds.size.y);
	}

	//	Rect GetBounds(){
	//		Rect bounds = new Rect();
	//		bounds.max.y = float.MaxValue;
	//		bounds.min.y = float.MinValue;
	//		bounds.max.x = float.MaxValue;
	//		bounds.min.x = float.MinValue;
	//
	//		// saca los maximos y minimos
	//		for (int i = 0; i < points.Count; i++)
	//		{
	//			Vector3 p = points[i];
	//			if (p.y > bounds.max.y)
	//				bounds.max = p.y;
	//			if (p.y < bounds.min.y)
	//				bounds.min = p.y;
	//			if (p.x > bounds.max.x)
	//				bounds.max.x = p.x;
	//			if (p.x < bounds.min.x)
	//				bounds.min.x = p.x;
	//		}
	//	}
	//		for (int y = 0; y < bounds.size.y; y++)
	//		{
	//			for (int x = 0; x < bounds.size.x; x++)
	//			{
	//				
	//			}
	//		}
}
