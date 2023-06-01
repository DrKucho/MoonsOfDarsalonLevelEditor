using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class DesignedMap : MonoBehaviour {
	public bool debug = false;
	
	public bool useLevelEditor = false;
	public Texture2D srcTerrain;
	public Texture2D srcBackground;

	[FormerlySerializedAs("destructibleTsSprite")] [FormerlySerializedAs("destructibleD2DSprite")] public TS_DynamicSprite dynamicTsSprite;
	public Texture2D destructibleTex;
	public Texture2D destructibleAlphaTex;
	public Texture2D destructibleTile;
	public Color[] destructibleColor = new Color[2] {Color.red, Color.magenta};

	[FormerlySerializedAs("indestructibleD2DSprite")] public TS_DynamicSprite indestructibleTsSprite;
	public Texture2D indestructibleTex;
	public Texture2D indestructibleAlphaTex;
	public Texture2D indestructibleTile;
	public Color[] indestructibleColor = new Color[3] {Color.white, Color.yellow, Color.green};

	public SpriteRenderer backgroundSpriteRenderer;
	public Color[] backgroundColor = new Color[2] {Color.blue, Color.cyan};
	public Texture2D backgroundTex;
	public Texture2D backgroundTile;
	public Color[] nothingColor = new Color[1] {Color.black};


}
