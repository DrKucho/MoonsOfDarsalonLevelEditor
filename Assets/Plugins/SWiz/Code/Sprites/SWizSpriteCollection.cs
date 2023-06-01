using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

[System.Serializable]
public class SWizSpriteColliderIsland
{
	public bool connected = true;
	public Vector2[] points;
	
}

[System.Serializable]
public class SWizLinkedSpriteCollection
{
	public string name = ""; 
	public SWizSpriteCollection spriteCollection = null;
}


[System.Serializable]
public class SWizSpriteCollectionDefinition
{
    public enum Anchor
    {
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		LowerLeft,
		LowerCenter,
		LowerRight,
		Custom
    }
	
	public enum Pad
	{
		Default,
		BlackZeroAlpha,
		Extend,
		TileXY,
		TileX,
		TileY,
	}
	
	public enum ColliderType
	{
		UserDefined,		
		ForceNone,			
		BoxTrimmed, 		
		BoxCustom, 			
		Polygon, 			
		Advanced,			
	}
	
	public enum PolygonColliderCap
	{
		None,
		FrontAndBack,
		Front,
		Back,
	}
	
	public enum ColliderColor
	{
		Default, 
		Red,
		White,
		Black
	}
	
	public enum Source
	{
		Sprite,
		SpriteSheet,
		Font
	}

	public enum DiceFilter
	{
		Complete,
		SolidOnly,
		TransparentOnly,
	}

	[System.Serializable]
	public class ColliderData {
		public enum Type {
			Box,
			Circle,
		}

		public string name = "";
		public Type type = Type.Box;
		public Vector2 origin = Vector3.zero;
		public Vector2 size = Vector3.zero;
		public float angle = 0;

		public void CopyFrom(ColliderData src) {
			name = src.name;
			type = src.type;
			origin = src.origin;
			size = src.size;
			angle = src.angle;
		}

		public bool CompareTo(ColliderData src) {
			return (name == src.name && 
				type == src.type &&
				origin == src.origin &&
				size == src.size &&
				angle == src.angle);
		}
	}

	public string name = "";
	
	public bool disableTrimming = false;
    public bool additive = false;
    public Vector3 scale = new Vector3(1,1,1);

    public Texture2D texture = null;
	
	[System.NonSerialized]
	public Texture2D thumbnailTexture;
	
	public int materialId = 0;
	
	public Anchor anchor = Anchor.MiddleCenter;
	public float anchorX, anchorY;
    public Object overrideMesh;

    public bool doubleSidedSprite = false;
	public bool customSpriteGeometry = false;
	public SWizSpriteColliderIsland[] geometryIslands = new SWizSpriteColliderIsland[0];
	
	public bool dice = false;
	public int diceUnitX = 64;
	public int diceUnitY = 64;
	public DiceFilter diceFilter = DiceFilter.Complete;

	public Pad pad = Pad.Default;
	public int extraPadding = 0; // default
	
	public Source source = Source.Sprite;
	public bool fromSpriteSheet = false;
	public bool hasSpriteSheetId = false;
	public int spriteSheetId = 0;
	public int spriteSheetX = 0, spriteSheetY = 0;
	public bool extractRegion = false;
	public int regionX, regionY, regionW, regionH;
	public int regionId;
	
	public ColliderType colliderType = ColliderType.UserDefined;
	public List<ColliderData> colliderData = new List<ColliderData>();
	public Vector2 boxColliderMin, boxColliderMax;
	public SWizSpriteColliderIsland[] polyColliderIslands;
	public PolygonColliderCap polyColliderCap = PolygonColliderCap.FrontAndBack;
	public bool colliderConvex = false;
	public bool colliderSmoothSphereCollisions = false;
	public ColliderColor colliderColor = ColliderColor.Default;

	public List<SWizSpriteDefinition.AttachPoint> attachPoints = new List<SWizSpriteDefinition.AttachPoint>();
	
}

[System.Serializable]
public class SWizSpriteCollectionDefault
{
    public bool additive = false;
    public Vector3 scale = new Vector3(1,1,1);
	public SWizSpriteCollectionDefinition.Anchor anchor = SWizSpriteCollectionDefinition.Anchor.MiddleCenter;
	public SWizSpriteCollectionDefinition.Pad pad = SWizSpriteCollectionDefinition.Pad.Default;	
	
	public SWizSpriteCollectionDefinition.ColliderType colliderType = SWizSpriteCollectionDefinition.ColliderType.UserDefined;
}

[System.Serializable]
public class SWizSpriteSheetSource
{
    public enum Anchor
    {
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		LowerLeft,
		LowerCenter,
		LowerRight,
    }
	
	public enum SplitMethod
	{
		UniformDivision,
	}
	
	public Texture2D texture;
	public int tilesX, tilesY;
	public int numTiles = 0;
	public Anchor anchor = Anchor.MiddleCenter;
	public SWizSpriteCollectionDefinition.Pad pad = SWizSpriteCollectionDefinition.Pad.Default;
	public Vector3 scale = new Vector3(1,1,1);
	public bool additive = false;
	
	// version 1
	public bool active = false;
	public int tileWidth, tileHeight;
	public int tileMarginX, tileMarginY;
	public int tileSpacingX, tileSpacingY;
	public SplitMethod splitMethod = SplitMethod.UniformDivision;
	
	public int version = 0;
	public const int CURRENT_VERSION = 1;

	public SWizSpriteCollectionDefinition.ColliderType colliderType = SWizSpriteCollectionDefinition.ColliderType.UserDefined;
	
	public string Name { get { return texture != null?texture.name:"New Sprite Sheet"; } }
}

[System.Serializable]
public class SWizSpriteCollectionFont
{
	public bool active = false;
	public TextAsset bmFont;
	public Texture2D texture;
    public bool dupeCaps = false; 
	public bool flipTextureY = false;
	public int charPadX = 0;
	public SWizFontData data;
	public SWizFont editorData;
	public int materialId;

	public bool useGradient = false;
	public Texture2D gradientTexture = null;
	public int gradientCount = 1;
	
	
	public string Name
	{
		get
		{
			if (bmFont == null || texture == null)
				return "Empty";
			else
			{
				if (data == null)
					return bmFont.name + " (Inactive)";
				else
					return bmFont.name;
			}
		}
	}
	
	public bool InUse
	{
		get { return active && bmFont != null && texture != null && data != null && editorData != null; }
	}
}

[System.Serializable]
public class SWizSpriteCollectionPlatform
{
	public string name = "";
	public SWizSpriteCollection spriteCollection = null;
	public bool Valid { get { return name.Length > 0 && spriteCollection != null; } }
	public void CopyFrom(SWizSpriteCollectionPlatform source)
	{
		name = source.name;
		spriteCollection = source.spriteCollection;
	}
}

public class SWizSpriteCollection : MonoBehaviour 
{
	public const int CURRENT_VERSION = 4;

	public enum NormalGenerationMode
	{
		None,
		NormalsOnly,
		NormalsAndTangents,
	};
	
    [SerializeField] private SWizSpriteCollectionDefinition[] textures; 
    [SerializeField] private Texture2D[] textureRefs;

    public Texture2D[] DoNotUse__TextureRefs { get { return textureRefs; } set { textureRefs = value; } }

	public SWizSpriteSheetSource[] spriteSheets;

	public SWizSpriteCollectionFont[] fonts;
	public SWizSpriteCollectionDefault defaults;

	public List<SWizSpriteCollectionPlatform> platforms = new List<SWizSpriteCollectionPlatform>();
	public bool managedSpriteCollection = false; 
	public SWizSpriteCollection linkParent = null; 
	public bool HasPlatformData { get { return platforms.Count > 1; } }
	public bool loadable = false;
	public AtlasFormat atlasFormat = AtlasFormat.UnityTexture;
	
	public int maxTextureSize = 2048;
	
	public bool forceTextureSize = false;
	public int forcedTextureWidth = 2048;
	public int forcedTextureHeight = 2048;
	
	public enum TextureCompression
	{
		Uncompressed,
		Reduced16Bit,
		Compressed,
		Dithered16Bit_Alpha,
		Dithered16Bit_NoAlpha,
	}

	public enum AtlasFormat {
		UnityTexture, // Normal Unity texture
		Png,
	}

	public TextureCompression textureCompression = TextureCompression.Uncompressed;
	
	public int atlasWidth, atlasHeight;
	public bool forceSquareAtlas = false;
	public float atlasWastage;
	public bool allowMultipleAtlases = false;
	public bool removeDuplicates = true;

    public bool allowSpannedDicing = false;

    public SWizSpriteCollectionDefinition[] textureParams;
    
	public SWizSpriteCollectionData spriteCollection;
    public bool premultipliedAlpha = false;
	
	public Material[] altMaterials;
	public Material[] atlasMaterials;
	public Texture2D[] atlasTextures;
	public TextAsset[] atlasTextureFiles = new TextAsset[0];

	[SerializeField] private bool useSWizCamera = false;
	[SerializeField] private int targetHeight = 640;
	[SerializeField] private float targetOrthoSize = 10.0f;
	
	// New method of storing sprite size
	public SWizSpriteCollectionSize sizeDef = SWizSpriteCollectionSize.Default();

	public float globalScale = 1.0f;
	public float globalTextureRescale = 1.0f;

	// Remember test data for attach points
	[System.Serializable]
	public class AttachPointTestSprite {
		public string attachPointName = "";
		public SWizSpriteCollectionData spriteCollection = null;
		public int spriteId = -1;
		public bool CompareTo(AttachPointTestSprite src) {
			return src.attachPointName == attachPointName && src.spriteCollection == spriteCollection && src.spriteId == spriteId;
		}
		public void CopyFrom(AttachPointTestSprite src) {
			attachPointName = src.attachPointName;
			spriteCollection = src.spriteCollection;
			spriteId = src.spriteId;
		}
	}
	public List<AttachPointTestSprite> attachPointTestSprites = new List<AttachPointTestSprite>();
	
	// Texture settings
	[SerializeField]
	private bool pixelPerfectPointSampled = false; // obsolete
	public FilterMode filterMode = FilterMode.Bilinear;
	public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
	public bool userDefinedTextureSettings = false;
	public bool mipmapEnabled = false;
	public int anisoLevel = 1;

	// Starts off with Unity Physics 3D for current platforms
	public SWizSpriteDefinition.PhysicsEngine physicsEngine = SWizSpriteDefinition.PhysicsEngine.Physics3D;
	public float physicsDepth = 0.1f;
	
	public bool disableTrimming = false;
	public bool disableRotation = false;
	
	public NormalGenerationMode normalGenerationMode = NormalGenerationMode.None;
	
	public int padAmount = -1; // default
	
	public bool autoUpdate = true;
	
	public float editorDisplayScale = 1.0f;

	public int version = 0;

	public string assetName = "";

	// Linked sprite collections
	public List<SWizLinkedSpriteCollection> linkedSpriteCollections = new List<SWizLinkedSpriteCollection>();

}
