using UnityEngine;
using System.Collections;

public class RenameGameObjectsOfSWizSprites : MonoBehaviour {

	
	public string from ="";
	public string spriteName ="";
	public string to = "";
	
	public void Start(){
		GameObject[] gos = FindObjectsOfType<GameObject>();
		foreach (GameObject go in gos){
			print ("FOUND:" + go);
			if (go.name == from){
				print ("FOUND A NAME MATCH");
				SWizSprite sprite = go.GetComponent<SWizSprite>();
				if (sprite.CurrentSprite.name == spriteName){
					print ("FOUND A SPRITE MATCH");
					go.name = to;
				}
				else print ("SPRITE NAME DOES NOT MATCH = " + sprite.name);
	
			}
		}
	}
}
