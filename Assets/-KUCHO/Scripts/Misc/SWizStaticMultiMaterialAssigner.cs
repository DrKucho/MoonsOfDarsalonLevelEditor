using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//[ExecuteInEditMode]
#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
#endif
public class SWizStaticMultiMaterialAssigner : MonoBehaviour
{

	public bool debug = false;
	public Material material;
	[ReadOnly2Attribute] public Material matToAssign; // si hay que crear instancia de material se hace na vez y luego se asigna este mat
	public bool setSpriteAtStart = false;
	public bool asignMaterialAtStart = true;
	public bool useSharedMaterial = true;
	public string collectionNameIncludes = "Buildings";

	public enum MustHaveCollider { DontCare, MustHave, MustNotHave };
    public MustHaveCollider mustHaveCollider;
    public SWizSprite[] sprites;
    public List<SWizSpriteMaterialAssigner> shouldRemove;
	Texture mainTex; 

	public void Awake(){

		if (material)
		{
			if (!matToAssign)
				SetMatToAssign();
		}
		else
		{
			Debug.LogError(this + " NO TENGO MATERIAL");
		}
	}
	public void InitialiseInEditor(){
        var allSprites = GetComponentsInChildren<SWizSprite>();
        List<SWizSprite> list = new List<SWizSprite>();
        shouldRemove.Clear();
        foreach (SWizSprite sp in allSprites)
        {
            if (sp.Collection.name.Contains(collectionNameIncludes))
            {
	            Collider2D col = null;
	            if (mustHaveCollider != MustHaveCollider.DontCare)
		            col = sp.GetComponent<Collider2D>();
	            if ((mustHaveCollider == MustHaveCollider.DontCare) || (mustHaveCollider == MustHaveCollider.MustHave && col) || (mustHaveCollider == MustHaveCollider.MustNotHave && !col))
	            {
		            list.Add(sp);
	            }
	            var sr = sp.GetComponent<SWizSpriteMaterialAssigner>();
                if (sr)
                    shouldRemove.Add(sr);
                }
        }
        sprites = list.ToArray();
		SetMatToAssign();
	}
//	#if UNITY_EDITOR
//	void Update(){
//		if (!Application.isPlaying && useSharedMaterial) // si no usamos shared mat da un error de leak materials into the scene
//			AssignMaterial(); 
//	}
//	#endif
	public void SetNewMaterial(Material newMat){
		material = newMat;
		SetMatToAssign();
	}
    public void SetNewMaterialAndAssignIt(Material newMat){
        material = newMat;
        SetMatToAssign();
        AssignMaterials(null);
    }
	
	void SetMatToAssign(){
		if (useSharedMaterial)
			matToAssign = material;
		else
			matToAssign = Instantiate(material);
	}
	public void Start(){
		if (asignMaterialAtStart){
			AssignMaterials(null);
		}
		if (setSpriteAtStart){
            SetSprites();
		}
	}
	
	void AssignMaterials(){
		AssignMaterials(null);
	}
    
    void RemoveSingleAssisners(){
        foreach (SWizSpriteMaterialAssigner rem in shouldRemove)
            DestroyImmediate(rem);
        shouldRemove.Clear();
    }
	// No se por que , a pesar de usar SharedMaterial me crea una instancia, lo dejo asi por que me conviene
	// SWizOnSpriteChanged es igual y no crea instancias, usa el material original (shared)
	public void AssignMaterials (SWizBaseSprite _baseSprite) { // el parametro es por que el system Event sprite.SpriteChanged es asi pero yo no lo uso , llamo con null
        if (matToAssign)
        {
            foreach (SWizSprite sp in sprites)
            {
                mainTex = sp.CachedRenderer.material.mainTexture;
                if (useSharedMaterial)
                {
                    sp.CachedRenderer.sharedMaterial = matToAssign; // cambiamos el material que tiene la main tex vacia
                    sp.CachedRenderer.sharedMaterial.mainTexture = mainTex;// fijamos la main tex que teniamos guardada del sprite original con el material definido por SWiz
                }
                else if (Application.isPlaying) // hacer esto en modo editor con el juego parado da error de leak materials on the scene
                {
                    sp.CachedRenderer.material = matToAssign; // cambiamos el material que tiene la main tex vacia
                    sp.CachedRenderer.material.mainTexture = mainTex;// fijamos la main tex que teniamos guardada del sprite original con el material definido por SWiz
                }
                if (debug)
                {
                    print(this + " MAT ORIGINAL = " + matToAssign.name + " MAT ASIGNADO= " + sp.CachedRenderer.sharedMaterial.name);
                }
            }
        }
	}
    public void SetSprites(){
        foreach (SWizSprite sp in sprites)
        {
            sp.SetSprite(sp.spriteId);  
        }
    }
}
