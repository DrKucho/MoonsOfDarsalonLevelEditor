using UnityEngine;
using System.Collections.Generic;

[System.Serializable]

public class SWizFontChar
{

    public Vector3 p0, p1;
    public Vector3 uv0, uv1;
    public bool flipped = false;
	public Vector2[] gradientUv;
	public float advance;

    public int channel = 0; // channel data for multi channel fonts. Lookup into an array (R=0, G=1, B=2, A=3)
}

[System.Serializable]

public class SWizFontKerning
{
	public int c0;
	public int c1;
	public float amount;
}

public class SWizFontData : MonoBehaviour
{
	public const int CURRENT_VERSION = 2;
	
	[HideInInspector]
	public int version = 0;

    public float lineHeight;

	public SWizFontChar[] chars;
	
	[SerializeField]
	List<int> charDictKeys;
	[SerializeField]
	List<SWizFontChar> charDictValues;

    public string[] fontPlatforms = null;

    public string[] fontPlatformGUIDs = null;	

	SWizFontData platformSpecificData = null;
	public bool hasPlatformData = false;
    public bool managedFont = false;
    public bool needMaterialInstance = false;
    public bool isPacked = false;
    public bool premultipliedAlpha = false;

    public SWizSpriteCollectionData spriteCollection = null;

	// Returns the active instance
	public SWizFontData inst
	{
		get 
		{
			if (platformSpecificData == null || platformSpecificData.materialInst == null)
			{
				if (hasPlatformData)
				{
					string systemPlatform = SWizSystem.CurrentPlatform;
					string guid = "";

					for (int i = 0; i < fontPlatforms.Length; ++i)
					{
						if (fontPlatforms[i] == systemPlatform)
						{
							guid = fontPlatformGUIDs[i];
							break;							
						}
					}
					if (guid.Length == 0)
						guid = fontPlatformGUIDs[0]; // failed to find platform, pick the first one

					platformSpecificData = SWizSystem.LoadResourceByGUID<SWizFontData>(guid);
				}
				else
				{
					platformSpecificData = this;
				}
				platformSpecificData.Init(); // awake is never called, so we initialize explicitly
			}
			return platformSpecificData;
		}
	}

	void Init()
	{

	}
	
	public Dictionary<int, SWizFontChar> charDict;
	public bool useDictionary = false;
	public SWizFontKerning[] kerning;
	public float largestWidth;
	public Material material;
	[System.NonSerialized]
	public Material materialInst;
	public Texture2D gradientTexture;
	public bool textureGradients;
	public int gradientCount = 1;
	public Vector2 texelSize;
	[HideInInspector]
	public float invOrthoSize = 1.0f;
	[HideInInspector]
	public float halfTargetHeight = 1.0f;	
	

	public void InitDictionary()
	{

	}

}
