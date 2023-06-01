using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SWizBaseSprite : MonoBehaviour, SWizRuntime.ISpriteCollectionForceBuild
{
	
    public enum Anchor
    {
		LowerLeft,
		LowerCenter,
		LowerRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		UpperLeft,
		UpperCenter,
		UpperRight,
    }
    
	[SerializeField]
    private SWizSpriteCollectionData collection;

	public SWizSpriteCollectionData Collection 
	{ 
		get { return collection; } 
		set { collection = value; collectionInst = collection.inst; } 
	}

    protected SWizSpriteCollectionData collectionInst;
	
	[SerializeField] protected Color _color = Color.white;
	[SerializeField] protected Vector3 _scale = new Vector3(1.0f, 1.0f, 1.0f);
	[SerializeField] protected int _spriteId = 0;

#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
#if !STRIP_PHYSICS_2D
	public BoxCollider2D boxCollider2D = null;
	public List<PolygonCollider2D> polygonCollider2D = new List<PolygonCollider2D>(1);
	public List<EdgeCollider2D> edgeCollider2D = new List<EdgeCollider2D>(1);
#endif
#endif

#if !STRIP_PHYSICS_3D
	public BoxCollider boxCollider = null;
	public MeshCollider meshCollider = null;
	public Vector3[] meshColliderPositions = null;
	public Mesh meshColliderMesh = null;
#endif	

	public event System.Action<SWizBaseSprite> SpriteChanged;

    public List<SWizOnSpriteChanged> kuchoOnSpriteChanged = new List<SWizOnSpriteChanged>(); // KUCHO HACK
    [System.NonSerialized] public SWizSpriteAttachPoint attachPointScript; // KUCHO HACK

    void InitInstance()
	{
		if (collectionInst == null && collection != null)
			collectionInst = collection.inst;
	}
    public Color color 
	{ 
		get { return _color; } 
		set 
		{
			if (value != _color)
			{
				_color = value;
				InitInstance();
				UpdateColors();
			}
		} 
	}
    
	public Vector3 scale 
	{ 
		get { return _scale; } 
		set
		{
			if (value != _scale)
			{
				_scale = value;
				InitInstance();
				UpdateVertices();

				if (SpriteChanged != null) {
					SpriteChanged( this );
				}
			}
		}
	}
	
	Renderer _cachedRenderer = null;
    public Renderer CachedRenderer { // KUCHO HACK la hice publica
        get
        {
			if (_cachedRenderer == null) {
				_cachedRenderer = GetComponent<Renderer>();
			}
			return _cachedRenderer;
		}
	}

	[SerializeField] protected int renderLayer = 0;

	public int SortingOrder {
		get { 

			return CachedRenderer.sortingOrder;
		}
		set {
			if (CachedRenderer.sortingOrder != value) {
				renderLayer = value; // for awake
				CachedRenderer.sortingOrder = value;
#if UNITY_EDITOR
				SWizUtil.SetDirty(CachedRenderer);
#endif
			}
		}
	}
    public bool updateMaterial = true; // KUCHO HACK


	public bool FlipX {
		get { return _scale.x < 0; }
		set { scale = new Vector3( Mathf.Abs(_scale.x) * (value?-1:1), _scale.y, _scale.z ); }
	}
	
	public bool FlipY {
		get { return _scale.y < 0; }
		set { scale = new Vector3( _scale.x, Mathf.Abs(_scale.y) * (value?-1:1), _scale.z ); }
	}

	public int spriteId 
	{ 
		get { return _spriteId; } 
		set 
		{
			if (value != _spriteId)
			{
				InitInstance();
				value = Mathf.Clamp(value, 0, collectionInst.spriteDefinitions.Length - 1);
				if (_spriteId < 0 || _spriteId >= collectionInst.spriteDefinitions.Length ||
					GetCurrentVertexCount() != collectionInst.spriteDefinitions[value].positions.Length ||
					collectionInst.spriteDefinitions[_spriteId].complexGeometry != collectionInst.spriteDefinitions[value].complexGeometry)
				{
					_spriteId = value;
					UpdateGeometry();
				}
				else
				{
					_spriteId = value;
					UpdateVertices();
				}

    			UpdateMaterial();

				if (SpriteChanged != null) {
					SpriteChanged( this );
				}
                // KUCHO NOTA: esto lo cree para evitar usar el sistema de eventos SpriteChanged que consume mucho
                foreach (SWizOnSpriteChanged o in kuchoOnSpriteChanged)// KUCHO HACK
                    o.OnSpriteChanged();// KUCHO HACK
                //if (attachPointScript != null) attachPointScript.HandleSpriteChanged(this);// KUCHO HACK


			}
		} 
	}

	public void SetSprite(int newSpriteId) {
		this.spriteId = newSpriteId;
	}

	public bool SetSprite(string spriteName) {
		int spriteId = collection.GetSpriteIdByName(spriteName, -1);
		if (spriteId != -1) { 
			SetSprite(spriteId);
		}
		else {
			Debug.LogError("SetSprite - Sprite not found in collection: " + spriteName);
		}
		return spriteId != -1;
	}

	public void SetSprite(SWizSpriteCollectionData newCollection, int newSpriteId) {
		bool switchedCollection = false;
		if (Collection != newCollection) {
			collection = newCollection;
			collectionInst = collection.inst;
			_spriteId = -1; // force an update, but only when the collection has changed
			switchedCollection = true;
		}
		
		spriteId = newSpriteId;
		
		if (switchedCollection) {
			UpdateMaterial();
		}
	}

	public bool SetSprite(SWizSpriteCollectionData newCollection, string spriteName) {
		int spriteId = newCollection.GetSpriteIdByName(spriteName, -1);
		if (spriteId != -1) { 
			SetSprite(newCollection, spriteId);
		}
		else {
			Debug.LogError("SetSprite - Sprite not found in collection: " + spriteName);
		}
		return spriteId != -1;
	}

	protected abstract void UpdateMaterial(); 
	protected abstract void UpdateColors(); 
	protected abstract void UpdateVertices(); 
	protected abstract void UpdateGeometry(); 
	protected abstract int  GetCurrentVertexCount(); 

	public abstract void Build();

	public int GetSpriteIdByName(string name)
	{
		InitInstance();
		return collectionInst.GetSpriteIdByName(name);
	}
	
	protected int GetNumVertices()
	{
		InitInstance();
		return collectionInst.spriteDefinitions[spriteId].positions.Length;
	}
	
	protected int GetNumIndices()
	{
		InitInstance();
		return collectionInst.spriteDefinitions[spriteId].indices.Length;
	}
	
	protected void SetPositions(Vector3[] positions, Vector3[] normals, Vector4[] tangents)	
	{
		var sprite = collectionInst.spriteDefinitions[spriteId];
		int numVertices = GetNumVertices();
		for (int i = 0; i < numVertices; ++i)
		{
			positions[i].x = sprite.positions[i].x * _scale.x;
			positions[i].y = sprite.positions[i].y * _scale.y;
			positions[i].z = sprite.positions[i].z * _scale.z;
		}
		
		int numNormals = sprite.normals.Length;
		if (normals.Length == numNormals)
		{
			for (int i = 0; i < numNormals; ++i)
			{
                if (i >= normals.Length || i >= sprite.normals.Length) print(this + " SPRITE NAME = " + sprite.name + " NORMALS.LENGTH=" + normals.Length + " SPRITE NORMALS LENGTH=" + sprite.normals.Length); // KUCHO HACK linea nueva
                normals[i] = sprite.normals[i];
			}
		}

		int numTangents = sprite.tangents.Length;
		if (tangents.Length == numTangents)
		{
			for (int i = 0; i < numTangents; ++i)
			{
				tangents[i] = sprite.tangents[i];
			}
		}
	}
	
	protected void SetColors(Color32[] dest)
	{
		Color c = _color;
        if (collectionInst.premultipliedAlpha) { c.r *= c.a; c.g *= c.a; c.b *= c.a; }
        Color32 c32 = c;

		int numVertices = GetNumVertices();
		for (int i = 0; i < numVertices; ++i)
			dest[i] = c32;
	}

	public Bounds GetBounds()
	{
		InitInstance();
		var sprite = collectionInst.spriteDefinitions[_spriteId];
		return new Bounds(new Vector3(sprite.boundsData[0].x * _scale.x, sprite.boundsData[0].y * _scale.y, sprite.boundsData[0].z * _scale.z),
		                  new Vector3(sprite.boundsData[1].x * Mathf.Abs(_scale.x), sprite.boundsData[1].y * Mathf.Abs(_scale.y), sprite.boundsData[1].z * Mathf.Abs(_scale.z) ));
	}

	public static Bounds AdjustedMeshBounds(Bounds bounds, int renderLayer) {
		Vector3 center = bounds.center;
		center.z = -renderLayer * 0.01f;
		bounds.center = center;
		return bounds;
	}

	public SWizSpriteDefinition CurrentSprite {
		get {
			InitInstance();
			return (collectionInst == null) ? null : collectionInst.spriteDefinitions[_spriteId];
		}
	}
	
	protected void Awake()
	{
		if (collection != null)
		{
			collectionInst = collection.inst;
		}
		
	}

#if UNITY_EDITOR
	private void OnEnable() {
		if (GetComponent<Renderer>() != null && Collection != null && GetComponent<Renderer>().sharedMaterial == null && Collection.inst.needMaterialInstance) {
			ForceBuild();
		}
	}
#endif
	
	public bool UsesSpriteCollection(SWizSpriteCollectionData spriteCollection)
	{
		return Collection == spriteCollection;
	}
	
	public virtual void ForceBuild()
	{
		if (collection == null) {
			return;
		}
		collectionInst = collection.inst;
		if (spriteId < 0 || spriteId >= collectionInst.spriteDefinitions.Length)
    		spriteId = 0;

		Build();
		if (SpriteChanged != null) {
			SpriteChanged(this);
		}
	}
	public virtual void ReshapeBounds(Vector3 dMin, Vector3 dMax) {
		;
	}
}
