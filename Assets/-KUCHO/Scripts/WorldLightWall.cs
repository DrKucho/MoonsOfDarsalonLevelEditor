using UnityEngine;
using System.Collections;
using Light2D;

public class WorldLightWall : MonoBehaviour {

	
	public enum WorldLightWallType {Left, Right, Floor}
	public Transform mainWall;
	public Transform corner;
	public WorldLightWallType side;
	public bool simpleWall = true;
	public Sprite spriteForSimpleWall;
	public int complexWallResolutionRatio = 4;
	public FilterMode filterMode = FilterMode.Point;
	public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
	private Color[] pixels;
	private Color[] pixels2;
	private byte[] alpha;
	private SpriteRenderer rend;
	private Rect rect;
	public Texture2D destructibleAlphaTex;
	public Texture2D indestructibleAlphaTex;
	public int width;
	public int height;

	public void OnValidate()
	{
		//SetMyLayer(); // DA WARNING
		if (!Application.isPlaying)
		{
			if (simpleWall == false)
			{
				if (WorldMap.instance)
				{
					if (!WorldMap.destructible.d2dSprite || !WorldMap.indestructible.d2dSprite)
						WorldMap.instance.LookForDestructibles();
					Build(WorldMap.destructible.d2dSprite.AlphaTex, WorldMap.indestructible.d2dSprite.AlphaTex, WorldMap.instance.simpleWalls);
				}
			}
		}
	}

	void Awake()
	{
		SetMyLayer();
	}

	void SetMyLayer()
	{
		mainWall.gameObject.layer = Layers.ground; // para que no salga la linea transparente por la que se ve el cielo
	}

	public void Build(Texture2D _destructibleAlphaTex, Texture2D _indestructibleAlphaTex, bool _simpleWalls){
		//Game.MakeSureGameInstanceIsNotNull();
		destructibleAlphaTex = _destructibleAlphaTex;
		indestructibleAlphaTex = _indestructibleAlphaTex;
		width = indestructibleAlphaTex.width;
		height = indestructibleAlphaTex.height;
		simpleWall = _simpleWalls;
		switch (side)
		{
			case (WorldLightWallType.Left):
				rect = new Rect(0,0,1,height);
	//			pixels = srcTerrain.GetPixels(rect.x, rect.y, rect.width, rect.height);
				if (simpleWall) BuildSimple();
				else BuildComplex();
				break;
			case (WorldLightWallType.Right):
				transform.position = new Vector3(width, transform.position.y, transform.position.z);
				rect = new Rect(width - 1,0,1,height);
	//			pixels = srcTerrain.GetPixels(rect.x, rect.y, rect.width, rect.height);
				if (simpleWall) BuildSimple();
				else BuildComplex();
				break;
			case (WorldLightWallType.Floor):
				mainWall.localScale = new Vector3(width, mainWall.localScale.y, mainWall.localScale.z);
				mainWall.localPosition = new Vector3(width/2, mainWall.localPosition.y, mainWall.localPosition.z);
				rend = mainWall.GetComponent<SpriteRenderer>();
				rend.sharedMaterial = MaterialDataBase.instance.defaultSpritesMat;
				break;
		}
	}
	void BuildSimple(){ // TODO PARECE QUE NO FUNCIONA????
		rend = mainWall.GetComponent<SpriteRenderer>();
		rend.sprite = spriteForSimpleWall;
		rend.sharedMaterial = MaterialDataBase.instance.defaultSpritesMat;
		int height = -1;
		pixels = destructibleAlphaTex.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
		pixels2 = indestructibleAlphaTex.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
		alpha = new byte[pixels.Length];
		// mezcla las alphas de las dos texturas de pixels en una 
		for (int i = 0; i < rect.width * rect.height; i++)
		{
			var _alpha = pixels[i].a + pixels2[i].a;
			if (_alpha > 1) _alpha = 1;
			alpha[i] = (byte)(_alpha * 255); // esto antes no multiplicaba por 255 , MAL
		}
		for (int n = alpha.Length -1; n >= 0; n--)
		{
			if (alpha[n] > 127)// solido
            { 
				height = n;
				n = -1; // rompe el loop
			}
		}
		if (height == -1) // no habia ningun pixel en ese lado
		{
			mainWall.gameObject.SetActive(false);
			corner.gameObject.SetActive(false);
		}
		else
		{
			mainWall.gameObject.SetActive(true);
			corner.gameObject.SetActive(true);
			mainWall.localScale = new Vector3(mainWall.localScale.x, height, mainWall.localScale.z);
		}
		rend = corner.GetComponent<SpriteRenderer>();
		rend.sharedMaterial = MaterialDataBase.instance.defaultSpritesMat;
	}
	public void BuildComplex(){
		if (complexWallResolutionRatio <= 0) complexWallResolutionRatio = 1;
		if (complexWallResolutionRatio > height) complexWallResolutionRatio = height;
		mainWall.localScale = new Vector3(mainWall.localScale.x, complexWallResolutionRatio, mainWall.localScale.z);
		mainWall.gameObject.SetActive(true);
		rend = mainWall.GetComponent<SpriteRenderer>();
		pixels = new Color[height / complexWallResolutionRatio];
		var tex = new Texture2D(1, pixels.Length ,TextureFormat.RGBA32 , false);
		tex.filterMode = filterMode;
		tex.wrapMode = wrapMode;
		int p = 0;
		for (int i = 0; i < height; i += complexWallResolutionRatio)
		{
			if (p < pixels.Length)
			{
				var destructibleTransparency = GetPixelsAverage(destructibleAlphaTex, new Vector2(rect.x, i), complexWallResolutionRatio);
				var indestructibleTransparency = GetPixelsAverage(indestructibleAlphaTex, new Vector2(rect.x, i), complexWallResolutionRatio);
				var alpha = destructibleTransparency + indestructibleTransparency;
				if (alpha > 1)
                    alpha = 1;
				pixels[p] = new Color (1,1,1, alpha);
				p++;
			}
		
		}
		tex.SetPixels(pixels);
		tex.Apply();
		rend.sprite = Sprite.Create(tex, new Rect(0,0,1,tex.height), new Vector2(0.5f, 0), 1, 0, SpriteMeshType.FullRect, Vector4.zero);
		
		if (tex.GetPixel(0, 0).a > 0.5f)
			rend.sharedMaterial = MaterialDataBase.instance.defaultSpritesMat;
		else
			corner.gameObject.SetActive(false);
		
		rend = corner.GetComponent<SpriteRenderer>();
		rend.sharedMaterial = MaterialDataBase.instance.defaultSpritesMat;
	}
	public float GetPixelsAverage(Texture2D tex, Vector2 _pos, int howMany){
		int nothingColorCount = 0;
		int otherColorCount = 0;
		int pixCount = 0;
		Point pos = new Point((int)_pos.x, (int)_pos.y);
		for (int i = 0; i < howMany; i++)
		{
			if (i < tex.height)
			{
				Color pix = tex.GetPixel(pos.x, pos.y + i);
				if (pix.a < 0.5f)
				{
					nothingColorCount ++;
				}
				else
				{
					otherColorCount ++;
				}
				pixCount ++;
			}
		}
		float average = otherColorCount / pixCount;
		return average;
	}

	public void RestoreLightObstacleMaterial()
	{
		var lo = GetComponentInChildren<LightObstacleGenerator>();
		lo.Material = MaterialDataBase.instance.worldWallLightObstacleMaterial;
	}
}
