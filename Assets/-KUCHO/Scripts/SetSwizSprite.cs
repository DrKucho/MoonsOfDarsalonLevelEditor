using UnityEngine;
using System.Collections;

public class SetSwizSprite : MonoBehaviour {

	[TextArea ()]
	public string notes = "";
	public DoItAt doIt = DoItAt.Start;
	public SWizSprite sprite;
	public int spriteID;
	int otherSpriteID = 0; // es necesario fijar otro sprite que no sea el mismo para que SetSprite haga algo

	void OnValidate(){
		sprite = GetComponent<SWizSprite>();
	}
	void Awake (){
		spriteID = sprite.spriteId;

		if (spriteID > 0)
			otherSpriteID = 0;
		else
			otherSpriteID = 1;
		
		if (doIt == DoItAt.Awake)
			SetSprite();
	}
	void OnEnable(){
		if (doIt == DoItAt.Enable)
			SetSprite();
	}
	void Start(){
		if (doIt ==	 DoItAt.Start) 
			SetSprite();
	}
	void OnLevelWasLoaded_NO(int level){
		if (doIt ==	 DoItAt.SceneLoaded)
			SetSprite();
	}
	public void SetSprite(){
		if (sprite)
		{
			sprite.SetSprite(otherSpriteID);
			sprite.SetSprite(spriteID);
		}
	}
}
