using UnityEngine;
using System.Collections;

[System.Serializable]
public struct PixelRaycastHit{
	public bool found;
	public Color pixel;
	public byte alpha;
	public int distance;
	public Vector2 point;
	public bool insideOfTexture;
	public bool destructible;
}
