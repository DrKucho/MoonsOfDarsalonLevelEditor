using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class SWizFont : MonoBehaviour 
{
	public TextAsset bmFont;
	public Material material;
	public Texture texture;
	public Texture2D gradientTexture;
    public bool dupeCaps = false; // duplicate lowercase into uc, or vice-versa, depending on which exists
	public bool flipTextureY = false;
	
	[HideInInspector]
	public bool proxyFont = false;

	[HideInInspector][SerializeField]
	private bool useSWizCamera = false;
	[HideInInspector][SerializeField]
	private int targetHeight = 640;
	[HideInInspector][SerializeField]
	private float targetOrthoSize = 1.0f;

	public SWizSpriteCollectionSize sizeDef = SWizSpriteCollectionSize.Default();
	
	public int gradientCount = 1;
	
	public bool manageMaterial = false;
	
	[HideInInspector]
	public bool loadable = false;
	
	public int charPadX = 0;
	
	public SWizFontData data;

	public static int CURRENT_VERSION = 1;
	public int version = 0;
	
}
