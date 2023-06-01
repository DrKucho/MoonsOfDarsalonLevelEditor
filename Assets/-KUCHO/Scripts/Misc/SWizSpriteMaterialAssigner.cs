using UnityEngine;
using System.Collections;


//[ExecuteInEditMode]
#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
#endif
public class SWizSpriteMaterialAssigner : MonoBehaviour {

	public bool debug = false;
	public Material material;
	[ReadOnly2Attribute] public Material matToAssign; // si hay que crear instancia de material se hace na vez y luego se asigna este mat
	[ReadOnly2Attribute] public Texture mainTex;
	public bool setSpriteAtStart = false;
	public bool asignMaterialAtStart = true;
	public bool useSharedMaterial = true;
	[HideInInspector] [SerializeField] SWizSprite sprite;
	[HideInInspector] [SerializeField] Renderer rend;

	public void Awake(){
		if (sprite)
			sprite.SpriteChanged += AssignMaterial; // system event de SWiz
        GetMainTex();
		if (!matToAssign)
			SetMatToAssign();
	}
	public void InitialiseInEditor(){
		sprite = GetComponent<SWizSprite>();
		rend = GetComponent<Renderer>();
	}
//	#if UNITY_EDITOR
//	void Update(){
//		if (!Application.isPlaying && useSharedMaterial) // si no usamos shared mat da un error de leak materials into the scene
//			AssignMaterial(); 
//	}
//	#endif
    public void GetMainTex()
    {
	    if (!material)
		    return;
	    
	    if (!mainTex)
		    mainTex = material.mainTexture;
	    
	    if (!mainTex)
	    {
		    if (useSharedMaterial)
			    mainTex = rend.sharedMaterial.mainTexture;
		    else
			    mainTex = rend.material.mainTexture;
	    }

	    if (mainTex)
	    {
		    material.mainTexture = mainTex;
		    SetMatToAssign();
	    }
	    
	    else
		    Debug.LogError(this + " NO TENGO MAIN TEXTURE, ESTO VACIARÁ LA MAIN TEX CUANDO ASIGNE MATERIAL!");
    }
	public void SetNewMaterial(Material newMat){
		material = newMat;
		SetMatToAssign();
	}
    public void SetNewMaterialAndAssignIt(Material newMat){
        material = newMat;
        SetMatToAssign();
        AssignMaterial(null);
    }
	
	void SetMatToAssign(){
		if (material)
		{
			if (useSharedMaterial)
				matToAssign = material;
			else
				matToAssign = Instantiate(material);
		}
		else
		{
			Debug.LogError(this + " NO TENGO MATERIAL");
		}
	}
	public void Start(){
		if (asignMaterialAtStart){
			AssignMaterial(null);
		}
		if (setSpriteAtStart){
			sprite.SetSprite(sprite.spriteId);	
		}
	}
	
	void AssignMaterial(){
		AssignMaterial(null);
	}
	// No se por que , a pesar de usar SharedMaterial me crea una instancia, lo dejo asi por que me conviene
	// SWizOnSpriteChanged es igual y no crea instancias, usa el material original (shared)
	public void AssignMaterial (SWizBaseSprite _baseSprite) { // el parametro es por que el system Event sprite.SpriteChanged es asi pero yo no lo uso , llamo con null
		if (matToAssign)
		{
			/*if (!Constants.appIsEditor || Constants.appIsEditor) // TODO elimina la segunda, esto es asi para probar y para que hno se me olvide
				if (matToAssign)
					matToAssign.shader = ShaderDataBase.instance.GetShaderFromList(matToAssign.shader);
            */
			if (!mainTex)
                GetMainTex();
            if (mainTex)
            {
                if (useSharedMaterial)
                {
                    rend.sharedMaterial = matToAssign; // cambiamos el material que tiene la main tex vacia
                    rend.sharedMaterial.mainTexture = mainTex;// fijamos la main tex que teniamos guardada del sprite original con el material definido por SWiz
                }
                else if (Application.isPlaying) // hacer esto en modo editor con el juego parado da error de leak materials on the scene
                {
                    rend.material = matToAssign; // cambiamos el material que tiene la main tex vacia
                    rend.material.mainTexture = mainTex;// fijamos la main tex que teniamos guardada del sprite original con el material definido por SWiz
                }
                if (debug)
                {
                    print(this + " MAT ORIGINAL = " + matToAssign.name + " MAT ASIGNADO= " + rend.sharedMaterial.name);
                }
            }
            else
            {
                Debug.LogError(this + " INTENTO DE ASIGNAR MATERIAL SIN TENER MAINTEXTURE GUARDADA PARA FIJAR DESPUES, IGNORANDO");
            }
		}
	}
}
