using UnityEngine;
using System.Collections;

public class SetSpriteBasedOnBackGround : MonoBehaviour {

	public Vector2 offset;
	public Vector2 boxSize;
	public int minPercentage = 50;
	public int lowRatioSprite = 0;
	public int highRatioSprite = 0;
	public SWizSprite sprite;
	
	public void Awake(){ //  print (this + " AWAKE ");
		sprite = GetComponent<SWizSprite>();
	}
	public void OnEnable(){ //  print(this + " ONENABLE ");
		Vector2 start = (Vector2)transform.position + offset;
		start.x -= boxSize.x / 2;
		var end = start + boxSize;
		var pos = start;
		var totalPixelsToCheck = boxSize.x * boxSize.y;
		int solidPixelCount = 0;
		for (int i = 0; i < totalPixelsToCheck; i++){
			if (PixelTools.IsInsideTexture(pos, WorldMap.background.texture)){
				if (PixelTools.AlphaDataGetBackGroundPixel(pos) > 127) solidPixelCount ++;
			}
			pos.x ++;
			if (pos.x >= end.x)
			{
				pos.x = start.x;
				pos.y ++;
			}
		}
		float ratio = (100 * solidPixelCount) / totalPixelsToCheck;
		int win;
		if (ratio >= minPercentage) win = highRatioSprite;
		else win = lowRatioSprite;
		sprite.SetSprite(win);
	}
	
	public void Update(){ //  print (this + " UPDATE ");
	
	}
}
