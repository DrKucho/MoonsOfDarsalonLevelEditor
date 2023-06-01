using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SWizCollider2DData {
	public Vector2[] points = new Vector2[0];	
}

[System.Serializable]

public class SWizSpriteColliderDefinition {
	public enum Type {
		Box,
		Circle,
	}

	public Type type = Type.Box;

	public Vector3 origin;

	public float angle;

	public string name = "";

	public Vector3[] vectors = new Vector3[0];
	public float[] floats = new float[0];

	public SWizSpriteColliderDefinition( Type type, Vector3 origin, float angle ) {
		this.type = type;
		this.origin = origin;
		this.angle = angle;
	}

	public float Radius {
		get {
			return type == Type.Circle ? floats[0] : 0;
		}
	}

	public Vector3 Size {
		get {
			return type == Type.Box ? vectors[0] : Vector3.zero;
		}
	}
}


[System.Serializable]

public 
	class SWizSpriteDefinition
{

	public enum ColliderType
	{

		Unset,

		None,

		Box,

		Mesh,

		Custom,
	}

	public enum PhysicsEngine
	{
		Physics3D,
		Physics2D
	}

	public string name;
	
	public Vector3[] boundsData;
	public Vector3[] untrimmedBoundsData;
	
	public Vector2 texelSize;

    public Vector3[] positions;

	public Vector3[] normals;

	public Vector4[] tangents;

    public Vector2[] uvs;

    public Vector2[] normalizedUvs = new Vector2[0];

    public int[] indices = new int[] { 0, 3, 1, 2, 3, 0 };

	public Material material;

	[System.NonSerialized]
	public Material materialInst;

	public int materialId;

    public int[] materialIndexData;

	public string sourceTextureGUID;

	public bool extractRegion;
	public int regionX, regionY, regionW, regionH;
	
	public enum FlipMode {
		None,
		SWiz,
		TPackerCW,
	}

	public FlipMode flipped;

	public bool complexGeometry = false;

	public PhysicsEngine physicsEngine = PhysicsEngine.Physics3D;

	public ColliderType colliderType = ColliderType.Unset;

	public SWizSpriteColliderDefinition[] customColliders = new SWizSpriteColliderDefinition[0];

	public Vector3[] colliderVertices; 
	public int[] colliderIndicesFwd;
	public int[] colliderIndicesBack;
	public bool colliderConvex;
	public bool colliderSmoothSphereCollisions;
	public SWizCollider2DData[] polygonCollider2D = new SWizCollider2DData[0];
	public SWizCollider2DData[] edgeCollider2D = new SWizCollider2DData[0];

	[System.Serializable]
	public class AttachPoint
	{
		public string name = "";
		public Vector3 position = Vector3.zero;
        public string relativeTo;
		public float angle = 0;

		public void CopyFrom( AttachPoint src ) {
			name = src.name;
			position = src.position;
			angle = src.angle;
		}

		public bool CompareTo( AttachPoint src ) {
			return (name == src.name && src.position == position && src.angle == angle);
		}
	}

	public AttachPoint[] attachPoints = new AttachPoint[0];
	
	public bool Valid { get { return name.Length != 0; } }

	public Bounds GetBounds()
	{
		return new Bounds(new Vector3(boundsData[0].x, boundsData[0].y, boundsData[0].z),
		                  new Vector3(boundsData[1].x, boundsData[1].y, boundsData[1].z));
	}

	public Bounds GetUntrimmedBounds()
	{
		return new Bounds(new Vector3(untrimmedBoundsData[0].x, untrimmedBoundsData[0].y, untrimmedBoundsData[0].z),
		                  new Vector3(untrimmedBoundsData[1].x, untrimmedBoundsData[1].y, untrimmedBoundsData[1].z));
	}
}

public class SWizSpriteCollectionData : MonoBehaviour 
{
	public const int CURRENT_VERSION = 3;
	
	public int version;
	public bool materialIdsValid = false;
	public bool needMaterialInstance = false;
	public bool Transient { get; set; } // this should not get serialized

    public SWizSpriteDefinition[] spriteDefinitions;

	Dictionary<string, int> spriteNameLookupDict = null;

    public bool premultipliedAlpha;

	public Material material;	

	public Material[] materials;

	[System.NonSerialized]
	public Material[] materialInsts;

	[System.NonSerialized]
	public Texture2D[] textureInsts = new Texture2D[0];

	public Texture[] textures;

	public TextAsset[] pngTextures = new TextAsset[0];
	public int[] materialPngTextureId = new int[0];

	// Used only for PNG textures
	public FilterMode textureFilterMode = FilterMode.Bilinear;
	public bool textureMipMaps = false;

	public bool allowMultipleAtlases;

	public string spriteCollectionGUID;

	public string spriteCollectionName;

	public string assetName = "";	

	public bool loadable = false;

	public float invOrthoSize = 1.0f;

	public float halfTargetHeight = 1.0f;
	
	public int buildKey = 0;

	public string dataGuid = "";

    public int Count { get { return inst.spriteDefinitions.Length; } }

    public bool managedSpriteCollection = false;

	public bool hasPlatformData = false;

    public string[] spriteCollectionPlatforms = null;

    public string[] spriteCollectionPlatformGUIDs = null;

	public int GetSpriteIdByName(string name)
	{
		return GetSpriteIdByName(name, 0);
	}

	public int GetSpriteIdByName(string name, int defaultValue)
	{
		inst.InitDictionary();
		int returnValue = defaultValue;
		if (!inst.spriteNameLookupDict.TryGetValue(name, out returnValue)) return defaultValue;
		return returnValue; // default to first sprite
	}

	public void ClearDictionary()
	{
		spriteNameLookupDict = null;
	}

	public SWizSpriteDefinition GetSpriteDefinition(string name) {
		int id = GetSpriteIdByName(name, -1);
		if (id == -1) {
			return null;
		}
		else {
			return spriteDefinitions[id];
		}
	}

	public void InitDictionary()
	{
		if (spriteNameLookupDict == null)
		{
			spriteNameLookupDict = new Dictionary<string, int>(spriteDefinitions.Length);
			for (int i = 0; i < spriteDefinitions.Length; ++i)
			{
				spriteNameLookupDict[spriteDefinitions[i].name] = i;
			}
		}
	}

	public SWizSpriteDefinition FirstValidDefinition
	{
		get 
		{
			foreach (var v in inst.spriteDefinitions)
			{
				if (v.Valid)
					return v;
			}
			return null;
		}
	}

	public bool IsValidSpriteId(int id) {
		if (id < 0 || id >= inst.spriteDefinitions.Length) {
			return false;
		}
		return inst.spriteDefinitions[id].Valid;
	}

	public int FirstValidDefinitionIndex
	{
		get 
		{
			SWizSpriteCollectionData data = inst;

			for (int i = 0; i < data.spriteDefinitions.Length; ++i)
				if (data.spriteDefinitions[i].Valid)
					return i;
			return -1;
		}
	}

	public void InitMaterialIds()
	{
		if (inst.materialIdsValid)
			return;
		
		int firstValidIndex = -1;
		Dictionary<Material, int> materialLookupDict = new Dictionary<Material, int>();
		for (int i = 0; i < inst.materials.Length; ++i)
		{
			if (firstValidIndex == -1 && inst.materials[i] != null)
				firstValidIndex = i;
			materialLookupDict[materials[i]] = i;
		}
		if (firstValidIndex == -1)
		{
			Debug.LogError("Init material ids failed.");
		}
		else
		{
			foreach (var v in inst.spriteDefinitions)			
			{
				if (!materialLookupDict.TryGetValue(v.material, out v.materialId))
					v.materialId = firstValidIndex;
			}
			inst.materialIdsValid = true;
		}
	}

	SWizSpriteCollectionData platformSpecificData = null;

	public SWizSpriteCollectionData inst
	{
		get 
		{
			if (platformSpecificData == null)
			{
				if (hasPlatformData)
				{
					string systemPlatform = SWizSystem.CurrentPlatform;
					string guid = "";

					for (int i = 0; i < spriteCollectionPlatforms.Length; ++i)
					{
						if (spriteCollectionPlatforms[i] == systemPlatform)
						{
							guid = spriteCollectionPlatformGUIDs[i];
							break;							
						}
					}
					if (guid.Length == 0)
						guid = spriteCollectionPlatformGUIDs[0]; // failed to find platform, pick the first one

					platformSpecificData = SWizSystem.LoadResourceByGUID<SWizSpriteCollectionData>(guid);
				}
				else
				{
					platformSpecificData = this;
				}
			}
			platformSpecificData.Init(); // awake is never called, so we initialize explicitly
			return platformSpecificData;
		}
	}

	public static readonly string internalResourcePrefix = "SWizInternal$.";
	
	void Init()
	{
		// check if already initialized
		if (materialInsts != null)
			return;

		if (spriteDefinitions == null) spriteDefinitions = new SWizSpriteDefinition[0];
		if (materials == null) materials = new Material[0];

		materialInsts = new Material[materials.Length];
		if (needMaterialInstance)
		{
			if (SWizSystem.OverrideBuildMaterial) {
				// This is a hack to work around a bug in Unity 4.x
				// Scene serialization will serialize the actively bound texture
				// but not the material during the build, only when [ExecuteInEditMode]
				// is on, eg. on sprites.
				for (int i = 0; i < materials.Length; ++i)
				{
					materialInsts[i] = new Material(Shader.Find("SWiz/BlendVertexColor"));
	#if UNITY_EDITOR
					materialInsts[i].hideFlags = HideFlags.DontSave;
	#endif
				}
			}
			else {
				bool assignTextureInst = false;
				if (pngTextures.Length > 0) {
					assignTextureInst = true;
					textureInsts = new Texture2D[pngTextures.Length];
					for (int i = 0; i < pngTextures.Length; ++i) {
						Texture2D tex = new Texture2D(4, 4, TextureFormat.ARGB32, textureMipMaps);
	#if UNITY_EDITOR
						tex.name = string.Format("{0}PNG_{1}_{2}", internalResourcePrefix, name, i);
						tex.hideFlags = HideFlags.DontSave;
	#endif
						tex.LoadImage(pngTextures[i].bytes);
						textureInsts[i] = tex;
						tex.filterMode = textureFilterMode;
						tex.Apply(textureMipMaps, true);	
					}
				}
 
				for (int i = 0; i < materials.Length; ++i) 
				{
					materialInsts[i] = Instantiate(materials[i]) as Material;
	#if UNITY_EDITOR
					materialInsts[i].name = string.Format("{0}Material_{1}_{2}", internalResourcePrefix, name, materials[i].name);
					materialInsts[i].hideFlags = HideFlags.DontSave; 
	#endif
					if (assignTextureInst) {
						int textureId = (materialPngTextureId.Length == 0) ? 0 : materialPngTextureId[i];
						materialInsts[i].mainTexture = textureInsts[ textureId ];
					}
				}
			}
			for (int i = 0; i < spriteDefinitions.Length; ++i)
			{
				SWizSpriteDefinition def = spriteDefinitions[i];
				def.materialInst = materialInsts[def.materialId];
			}
		}
		else
		{
			for (int i = 0; i < materials.Length; ++i) {
				materialInsts[i] = materials[i];
			}
			for (int i = 0; i < spriteDefinitions.Length; ++i)
			{
				SWizSpriteDefinition def = spriteDefinitions[i];
				def.materialInst = def.material;
			}
		}


#if (UNITY_EDITOR && !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2))
		// Unity 4.3 when in 2D mode overrides imported textures with alphaIsTransparency set
		// which naturally breaks our old demo scenes. This happens even when meta files
		// are present :(
		if (materialInsts != null 
				&& materialInsts.Length > 0 
				&& materialInsts[0] != null
				&& materialInsts[0].mainTexture != null
				&& materialInsts[0].shader != null
				&& materialInsts[0].shader.name.Contains("Premul")) { // Detect premultiplied textures
			string path = UnityEditor.AssetDatabase.GetAssetPath( materialInsts[0].mainTexture );
			if (path.Length > 0) {
				UnityEditor.TextureImporter importer = UnityEditor.TextureImporter.GetAtPath(path) as UnityEditor.TextureImporter;
				if (importer != null && (importer.alphaIsTransparency 
#if UNITY_5_5_OR_NEWER
					|| importer.alphaSource == UnityEditor.TextureImporterAlphaSource.FromGrayScale
#else
					|| importer.grayscaleToAlpha
#endif
				)) {
					if (UnityEditor.EditorUtility.DisplayDialog(
							"Atlas texture incompatibility", 
							string.Format("Atlas texture '{0}' for sprite collection '{1}' must be reimported to display correctly in Unity 4.3 when in 2D mode.", materialInsts[0].mainTexture.name, name), 
							"Reimport")) {
						List<Texture> textures = new List<Texture>();
						for (int i = 0; i < materialInsts.Length; ++i) {
							if (materialInsts[i] != null 
									&& materialInsts[i].mainTexture != null 
									&& !textures.Contains(materialInsts[i].mainTexture) // only do this once
									&& materialInsts[i].shader != null) {
								path = UnityEditor.AssetDatabase.GetAssetPath( materialInsts[i].mainTexture );
								if (path.Length > 0) {
									importer = UnityEditor.TextureImporter.GetAtPath(path) as UnityEditor.TextureImporter;
									if (importer != null && (importer.alphaIsTransparency 
#if UNITY_5_5_OR_NEWER
																|| importer.alphaSource == UnityEditor.TextureImporterAlphaSource.FromGrayScale
#else
																|| importer.grayscaleToAlpha
#endif
									)) {
#if UNITY_5_5_OR_NEWER
										importer.alphaSource = UnityEditor.TextureImporterAlphaSource.FromInput;
#else
										importer.grayscaleToAlpha = false;
#endif
										importer.alphaIsTransparency = false;
										SWizUtil.SetDirty(importer);
										UnityEditor.AssetDatabase.ImportAsset(path);
									}
								}
								textures.Add( materialInsts[i].mainTexture );
							}
						}
					}
				}
			}
		}
#endif

		SWizEditorSpriteDataUnloader.Register(this);
	}

	public static SWizSpriteCollectionData CreateFromTexture(Texture texture, SWizSpriteCollectionSize size, string[] names, Rect[] regions, Vector2[] anchors)
	{
		return SWizRuntime.SpriteCollectionGenerator.CreateFromTexture(texture, size, names, regions, anchors);
	}

	public static SWizSpriteCollectionData CreateFromTexturePacker(SWizSpriteCollectionSize size, string texturePackerData, Texture texture)
	{
		return SWizRuntime.SpriteCollectionGenerator.CreateFromTexturePacker(size, texturePackerData, texture);
	}

	public void ResetPlatformData()
	{
		SWizEditorSpriteDataUnloader.Unregister(this);

		if (platformSpecificData != null) {
			platformSpecificData.DestroyTextureInsts();
		}
		DestroyTextureInsts();

		if (platformSpecificData)
		{
			platformSpecificData = null;
		}
		
		materialInsts = null;
	}

	void DestroyTextureInsts() {
		foreach (Texture2D texture in textureInsts) {
			Object.DestroyImmediate(texture);
		}
		textureInsts = new Texture2D[0];
	}


	void DestroyMaterialInsts()
	{
		if (needMaterialInstance)
		{
			foreach (Material material in materialInsts) {
				DestroyImmediate(material);
			}
		}
		materialInsts = null;
	}

	void OnDestroy()
	{
		if (Transient)
		{
			foreach (Material material in materials)
			{
				DestroyImmediate(material);
			}
		}
		else if (needMaterialInstance) // exclusive
		{
			foreach (Material material in materialInsts) {
				DestroyImmediate(material);
			}
			materialInsts = new Material[0];

			foreach (Texture2D texture in textureInsts) {
				Object.DestroyImmediate(texture);
			}
			textureInsts = new Texture2D[0];
		}

		ResetPlatformData();
	}
}
