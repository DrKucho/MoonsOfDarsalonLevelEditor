using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Light2D;


[ExecuteInEditMode]
public class TileDoorLightObstacle : MonoBehaviour {

	public Door door;
	[ReadOnly2Attribute] public KuchoTile tile;
	[ReadOnly2Attribute] public KuchoTileGrid tileGrid;
	[ReadOnly2Attribute] public SpriteRenderer spr;
	public float switchSpeed = 0.5f;
	[Range (0f,1f)] public RangeFloat alpha = new RangeFloat(0f,1f);
	[ReadOnly2Attribute] public float alphaGoal = 0;

	
	public void InitialiseInEditor(){
			tile = GetComponentInParent<KuchoTile>();
			tileGrid = GetComponentInParent<KuchoTileGrid>();
			spr = GetComponentInChildren<SpriteRenderer>();
	}
	void OnEnable(){
		MyUpdate();
	}
	public void MyUpdate () {
		if (tile && tileGrid)
		{
			if (tileGrid.groupPlayerIsIn != tile.tileGroup)// esta fuera de mi grupo
			{
				switch (door.status)
				{
					case (Door.Status.Opened):
						ColorHelper.SetAlpha(spr, alpha.min);
						break;
					case (Door.Status.Closed):
						ColorHelper.SetAlpha(spr, alpha.min);
						break;
					case (Door.Status.Opening):
						ColorHelper.SetAlpha(spr, alpha.min);
						break;
					case (Door.Status.Closing):
						ColorHelper.SetAlpha(spr, alpha.min);
						break;
				}
			}
			else // esta dentro de mi grupo
			{
				switch (door.status)
				{
					case (Door.Status.Opened):
						ColorHelper.SetAlpha(spr, alpha.min);
						break;
					case (Door.Status.Closed):
						ColorHelper.SetAlpha(spr, alpha.max);
						break;
					case (Door.Status.Opening):
						Fade(alpha.min);
						break;
					case (Door.Status.Closing):
						Fade(alpha.max);
						break;
				}
			}
		}
	}
	void Fade(float goal){
		alphaGoal = goal;
	}
	
}
