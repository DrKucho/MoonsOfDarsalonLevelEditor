using UnityEngine;
using System.Collections;

/// <summary>
/// establece posicion aleatoria del transform bien local o world
/// Creo que ya no lo uso , pero lo dejo por que puede ser util
/// </summary>
public class RandomPosition : MonoBehaviour {

	public Vector2 min;
	public Vector2 max;
	public DoItAt doItAt = DoItAt.EveryFrame; // enum DoItAt {Awake, Enable, Start, EveryFrame}
	public bool localPos = true;
	public Snap snapX = Snap.RealPixel;
	public Snap snapY = Snap.RealPixel;

	void Awake () {
		if (doItAt == DoItAt.Awake) DoIt();
	}
	void Start(){
		if (doItAt == DoItAt.Start) DoIt();
	}
	void OnEnable(){
		if (doItAt == DoItAt.Enable) DoIt();
	}
	void OnLevelWasLoaded_NO(int level){
		if (doItAt == DoItAt.SceneLoaded)  DoIt();
	}
	void DoIt () {
		Vector3 newPos;
		newPos = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
		newPos = SnapTo.Pixel(newPos, snapX, snapY);
		newPos.z = transform.position.z;
		if (localPos)
			transform.localPosition = newPos;
		else
			transform.position = newPos;
	}
}
