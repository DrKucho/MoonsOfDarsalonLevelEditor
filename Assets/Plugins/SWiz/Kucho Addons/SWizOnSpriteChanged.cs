using UnityEngine;
using System.Collections;


public class SWizOnSpriteChanged : MonoBehaviour {

	
	//bool setAlphaAtStart = false;
	public bool debug = false;
	public float alpha= 1;
	public bool changeAlphaOnShader = true;
	public Material material;
	public bool doEverythingAtStart = true;
	public bool setSpriteAtStart = false;
//	public bool findGlobalSettingsOnEnable = false;
//	public bool globalSettingsFindsChildsOnEnable = false;
	
    public SWizSprite sprite;
    public SWizBaseSprite baseSprite;
    public  Renderer _renderer;
	[SerializeField]  Texture2D mainTex;
	public bool hookedToSpriteChangedEvent = false;
    public SWizOnSpriteChangedGlobal globalSettings;
	
	public void InitialiseInEditor(){
		sprite = GetComponent<SWizSprite>();
		baseSprite = GetComponent<SWizBaseSprite>();
		_renderer = sprite.gameObject.GetComponent<Renderer>();
		globalSettings = GetComponentInParent<SWizOnSpriteChangedGlobal>();
		if (Application.isPlaying) // TODO ESTO BORRALO NO? NO PUEDE OCURRIR NUNCA ESTOES INITIALISE IN EDITOR
			mainTex = _renderer.material.mainTexture as Texture2D;
		mainTex = _renderer.sharedMaterial.mainTexture as Texture2D;
	}
	public void Awake () {
		if (doEverythingAtStart){
			OnSpriteChanged();
		}
		if (setSpriteAtStart){
			sprite.SetSprite(sprite.spriteId);	
		}
		Hook();
	}
//	public void OnEnable(){
//		if (findGlobalSettingsOnEnable) globalSettings = GetComponentInParent<SWizOnSpriteChangedGlobal>();
//		if (globalSettingsFindsChildsOnEnable && globalSettings) globalSettings.FindChilds(); 
//	}
	public void Hook(){
//		sprite.SpriteChanged += OnSpriteChanged; // system event de SWiz
        sprite.kuchoOnSpriteChanged.Add(this);
		hookedToSpriteChangedEvent = true;
		if (debug) print (" ALGUIEN HA LLAMADO A HOOK");
	}
	public void UnHook(){
//		sprite.SpriteChanged -= OnSpriteChanged; // system event de SWiz ( no funciona? no consigo que funcione)
        sprite.kuchoOnSpriteChanged.Remove(this);
        hookedToSpriteChangedEvent = false;
		if (debug) print (" ALGUIEN HA LLAMADO A UN-HOOK");
	}
	public void OnSpriteChanged(){
		if (globalSettings)
		{
			alpha = globalSettings.alpha;
			changeAlphaOnShader = globalSettings.changeAlphaOnShader;
			if (globalSettings.material) material = globalSettings.material;
		}
		AssignMaterial();
		SetAlpha();
	}
	public void SetAlpha (float _alpha){
		alpha = _alpha;
		SetAlpha();
	}
	public void SetAlpha (){ 
		if (changeAlphaOnShader)
		{
			if (debug)
				print(this + " CAMBIANDO ALPHA A VALOR = " + alpha);
			if (_renderer.sharedMaterial.HasProperty("_Alpha"))
				_renderer.sharedMaterial.SetFloat("_Alpha", alpha);
			if (_renderer.sharedMaterial.HasProperty("_Tint"))
			{
				var c = _renderer.sharedMaterial.GetColor("_Color");
				c.a = alpha;
				_renderer.sharedMaterial.SetColor("_Color", c);
			}
		}
		else
		{
			baseSprite.color = new Color(baseSprite.color.r, baseSprite.color.g, baseSprite.color.b, alpha);	
		}
	}
	public void AssignMaterial () {
		if (material){
			_renderer.sharedMaterial = material; // cambiamos el material que tiene la main tex vacia
			_renderer.sharedMaterial.mainTexture = mainTex;// fijamos la main tex que teniamos guardada del sprite original con el material definido por SWiz
		}
	}
}
