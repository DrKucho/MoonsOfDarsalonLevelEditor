using UnityEngine;
using System.Collections;


public class AlphaSwitch : MonoBehaviour {

//	public string[] tags;
//	public float onEnter = 0;
//	public float onExit = 1;
//	public float speed;
//	public float currentAlpha = 1;
//	public float goal = 0;
	public Renderer rend;
	[ReadOnly2Attribute] public int tileGroup;
	public delegate void OnPlayerEnterOrExit(int groupIndex, AlphaSwitch alphaSwitch);
	public bool playerIsIn = false;
	public event OnPlayerEnterOrExit onPlayerEnterAlphaSwitch;
	public event OnPlayerEnterOrExit onPlayerExitsAlphaSwitch;

	void Awake () {
		if (!rend) rend = GetComponentInParent<Renderer>();
//		currentAlpha = rend.material.GetFloat(ShaderProp._Alpha);
	}
//	void Update () {
//		currentAlpha = Mathf.MoveTowards(currentAlpha, goal, speed);
//		rend.material.SetFloat(ShaderProp._Alpha, currentAlpha);
//	}
	void OnTriggerEnter2D(Collider2D col){
		if (col == Game.playerCol && onPlayerEnterAlphaSwitch != null)
		{
			playerIsIn = true;
			onPlayerEnterAlphaSwitch(tileGroup, this);
		}
	}
	void OnTriggerExit2D(Collider2D col){
		if (col == Game.playerCol && onPlayerExitsAlphaSwitch != null)
		{
			playerIsIn = false;
			onPlayerExitsAlphaSwitch(tileGroup, this);
		}
	}
}
