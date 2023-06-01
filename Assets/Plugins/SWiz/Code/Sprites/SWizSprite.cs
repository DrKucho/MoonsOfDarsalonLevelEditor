using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
/// <summary>
/// Sprite implementation which maintains its own Unity Mesh. Leverages dynamic batching.
/// </summary>
public class SWizSprite : SWizBaseSprite
{
	Mesh mesh;
	Vector3[] meshVertices;
	Vector3[] meshNormals = null;
	Vector4[] meshTangents = null;
	Color32[] meshColors;
	
	new void Awake()
	{
		base.Awake();

		// Create mesh, independently to everything else
		mesh = new Mesh();
#if !UNITY_3_5
		mesh.MarkDynamic();
#endif
		mesh.hideFlags = HideFlags.DontSave;
		GetComponent<MeshFilter>().mesh = mesh;
		
		// This will not be set when instantiating in code
		// In that case, Build will need to be called
		if (Collection)
		{
			// reset spriteId if outside bounds
			// this is when the sprite collection data is corrupt
			if (_spriteId < 0 || _spriteId >= Collection.Count)
				_spriteId = 0;
			
			Build();
		}
	}

	protected void OnDestroy()
	{
		if (mesh)
		{
#if UNITY_EDITOR
			DestroyImmediate(mesh);
#else
			Destroy(mesh);
#endif
		}

#if !STRIP_PHYSICS_3D
		if (meshColliderMesh)
		{
#if UNITY_EDITOR
			DestroyImmediate(meshColliderMesh);
#else
			Destroy(meshColliderMesh);
#endif
		}
#endif
	}
	
	public override void Build()
	{
		var sprite = collectionInst.spriteDefinitions[spriteId];

		meshVertices = new Vector3[sprite.positions.Length];
        meshColors = new Color32[sprite.positions.Length];
		
		meshNormals = new Vector3[0];
		meshTangents = new Vector4[0];
		
		if (sprite.normals != null && sprite.normals.Length > 0)
		{
			meshNormals = new Vector3[sprite.normals.Length];
		}
		if (sprite.tangents != null && sprite.tangents.Length > 0)
		{
			meshTangents = new Vector4[sprite.tangents.Length];
		}
		
		SetPositions(meshVertices, meshNormals, meshTangents);
		SetColors(meshColors);
		
		if (mesh == null)
		{
			mesh = new Mesh();
#if !UNITY_3_5
			mesh.MarkDynamic();
#endif
			mesh.hideFlags = HideFlags.DontSave;
			GetComponent<MeshFilter>().mesh = mesh;
		}
		
		mesh.Clear();
		mesh.vertices = meshVertices;
		mesh.normals = meshNormals;
		mesh.tangents = meshTangents;
		mesh.colors32 = meshColors;
		mesh.uv = sprite.uvs;
		mesh.triangles = sprite.indices;
		mesh.bounds = AdjustedMeshBounds( GetBounds(), renderLayer );
		
		UpdateMaterial();
	}

	protected override void UpdateGeometry() { UpdateGeometryImpl(); }
	protected override void UpdateColors() { UpdateColorsImpl(); }
	protected override void UpdateVertices() { UpdateVerticesImpl(); }
	
	
	protected void UpdateColorsImpl()
	{
		// This can happen with prefabs in the inspector
		if (mesh == null || meshColors == null || meshColors.Length == 0)
			return;

		SetColors(meshColors);
		mesh.colors32 = meshColors;
	}
	
	protected void UpdateVerticesImpl()
	{
		var sprite = collectionInst.spriteDefinitions[spriteId];
		
		// This can happen with prefabs in the inspector
		if (mesh == null || meshVertices == null || meshVertices.Length == 0)
			return;
		
		// Clear out normals and tangents when switching from a sprite with them to one without
		if (sprite.normals.Length != meshNormals.Length)
		{
			meshNormals = (sprite.normals != null && sprite.normals.Length > 0)?(new Vector3[sprite.normals.Length]):(new Vector3[0]);
		}
		if (sprite.tangents.Length != meshTangents.Length)
		{
			meshTangents = (sprite.tangents != null && sprite.tangents.Length > 0)?(new Vector4[sprite.tangents.Length]):(new Vector4[0]);
		}
		
		SetPositions(meshVertices, meshNormals, meshTangents);
		mesh.vertices = meshVertices;
		mesh.normals = meshNormals;
		mesh.tangents = meshTangents;
		mesh.uv = sprite.uvs;
		mesh.bounds = AdjustedMeshBounds( GetBounds(), renderLayer );
	}

	protected void UpdateGeometryImpl()
	{
		// This can happen with prefabs in the inspector
		if (mesh == null)
			return;
		
		var sprite = collectionInst.spriteDefinitions[spriteId];

		if (meshVertices == null || meshVertices.Length != sprite.positions.Length)
		{
			meshVertices = new Vector3[sprite.positions.Length];
			meshColors = new Color32[sprite.positions.Length];
		}
		
		if (meshNormals == null || (sprite.normals != null && meshNormals.Length != sprite.normals.Length))
		{
			meshNormals = new Vector3[sprite.normals.Length];
		}
		else if (sprite.normals == null)
		{
			meshNormals = new Vector3[0];
		}

		if (meshTangents == null || (sprite.tangents != null && meshTangents.Length != sprite.tangents.Length))
		{
			meshTangents = new Vector4[sprite.tangents.Length];
		}
		else if (sprite.tangents == null)
		{
			meshTangents = new Vector4[0];
		}

		SetPositions(meshVertices, meshNormals, meshTangents);
		SetColors(meshColors);

		mesh.Clear();
		mesh.vertices = meshVertices;
		mesh.normals = meshNormals;
		mesh.tangents = meshTangents;
		mesh.colors32 = meshColors;
		mesh.uv = sprite.uvs;
		mesh.bounds = AdjustedMeshBounds( GetBounds(), renderLayer );
        mesh.triangles = sprite.indices;
	}
	
	protected override void UpdateMaterial()
	{
        if (!updateMaterial) // KUCHO HACK con esto evito que me machaque el material cada vez , lo cual tambien deberia evitar la necesidad de onspritechanged y de SWizsetmaterial ... ?
            return; // KUCHO HACK
            
        Renderer renderer = GetComponent<Renderer>();
		if (renderer.sharedMaterial != collectionInst.spriteDefinitions[spriteId].materialInst)
			renderer.material = collectionInst.spriteDefinitions[spriteId].materialInst;
	}

	public void ForceUpdateMaterial() // KUCHO HACK
	{
		Renderer renderer = GetComponent<Renderer>();
		if (renderer.sharedMaterial != collectionInst.spriteDefinitions[spriteId].materialInst)
			renderer.material = collectionInst.spriteDefinitions[spriteId].materialInst;	}

	protected override int GetCurrentVertexCount()
	{
		if (meshVertices == null)
			return 0;
		// Really nasty bug here found by Andrew Welch.
		return meshVertices.Length;
	}

#if UNITY_EDITOR
	void OnDrawGizmos() {
		if (collectionInst != null && spriteId >= 0 && spriteId < collectionInst.Count) {
			var sprite = collectionInst.spriteDefinitions[spriteId];
			Gizmos.color = Color.clear;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawCube(Vector3.Scale(sprite.untrimmedBoundsData[0], _scale), Vector3.Scale(sprite.untrimmedBoundsData[1], _scale));
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = Color.white;
		}
	}
#endif
	
	public override void ForceBuild()
	{
		base.ForceBuild();
		GetComponent<MeshFilter>().mesh = mesh;
	}

	public override void ReshapeBounds(Vector3 dMin, Vector3 dMax) {
		float minSizeClampTexelScale = 0.1f; // Can't shrink sprite smaller than this many texels
		// Irrespective of transform
		var sprite = CurrentSprite;
		Vector3 oldAbsScale = new Vector3(Mathf.Abs(_scale.x), Mathf.Abs(_scale.y), Mathf.Abs(_scale.z));
		Vector3 oldMin = Vector3.Scale(sprite.untrimmedBoundsData[0], _scale) - 0.5f * Vector3.Scale(sprite.untrimmedBoundsData[1], oldAbsScale);
		Vector3 oldSize = Vector3.Scale(sprite.untrimmedBoundsData[1], oldAbsScale);
		Vector3 newAbsScale = oldSize + dMax - dMin;
		newAbsScale.x /= sprite.untrimmedBoundsData[1].x;
		newAbsScale.y /= sprite.untrimmedBoundsData[1].y;
		// Clamp the minimum size to avoid having the pivot move when we scale from near-zero
		if (sprite.untrimmedBoundsData[1].x * newAbsScale.x < sprite.texelSize.x * minSizeClampTexelScale && newAbsScale.x < oldAbsScale.x) {
			dMin.x = 0;
			newAbsScale.x = oldAbsScale.x;
		}
		if (sprite.untrimmedBoundsData[1].y * newAbsScale.y < sprite.texelSize.y * minSizeClampTexelScale && newAbsScale.y < oldAbsScale.y) {
			dMin.y = 0;
			newAbsScale.y = oldAbsScale.y;
		}
		// Add our wanted local dMin offset, while negating the positional offset caused by scaling
		Vector2 scaleFactor = new Vector3(Mathf.Approximately(oldAbsScale.x, 0) ? 0 : (newAbsScale.x / oldAbsScale.x),
			Mathf.Approximately(oldAbsScale.y, 0) ? 0 : (newAbsScale.y / oldAbsScale.y));
		Vector3 scaledMin = new Vector3(oldMin.x * scaleFactor.x, oldMin.y * scaleFactor.y);
		Vector3 offset = dMin + oldMin - scaledMin;
		offset.z = 0;
		transform.position = transform.TransformPoint(offset);
		scale = new Vector3(_scale.x * scaleFactor.x, _scale.y * scaleFactor.y, _scale.z);
	}
}
