using UnityEngine;
using System.Collections;

public class SWizMultiSpriteAlphaSetter : MonoBehaviour {

	
	// bool setAlphaAtStart = false;
	public float Alpha= 1;
	SWizSprite[] sprites;
	Renderer[] _renderers;
	public bool hookedToSpriteChangedEvent = false;
	
	public void Awake () {
		sprites = transform.parent.GetComponentsInChildren<SWizSprite>();
		_renderers = new Renderer[sprites.Length];
		for ( int n = 0; n < sprites.Length; n++){
			_renderers[n] = sprites[n].gameObject.GetComponent<Renderer>();
		}
		Hook();
	//	if (setAlphaAtStart){
	//		SetAlpha();
	//	}
	}
	public void Hook(){
		sprites[0].SpriteChanged += SetAlpha; // system event de SWiz
		hookedToSpriteChangedEvent = true;
	//	print (" ALGUIEN HA LLAMADO A HOOK");
	//	Debug.Break(); Debug.Log(this + "!!!!!!!!!!!!!!!!!!!!!!!!!");
	}
	public void UnHook(){
		sprites[0].SpriteChanged -= SetAlpha; // system event de SWiz ( no funciona? no consigo que funcione)
		hookedToSpriteChangedEvent = false;
	//	print (" ALGUIEN HA LLAMADO A UN-HOOK");
	//	Debug.Break(); Debug.Log(this + "!!!!!!!!!!!!!!!!!!!!!!!!!");
	}
	// ha de llamarse cada vez que SWiz hace un SetSprite porque SetSprite asigna el mateial original con el valor de alpha a 1
	// tambien ha de llamarse a mano la primera vez para evitar que los sprites que han sido desactivados (cabeza y cuerpo al recibir impacto) no se actualicen correctamente
	public void SetAlpha (SWizBaseSprite _baseSprite){ 
		print (this + " CAMBIANDO ALPHA A VALOR = " + Alpha);
		for ( int n = 0; n < sprites.Length; n++){
			SetAlpha(_renderers[n]);
		}
	}
	public void SetAlpha (Renderer _renderer){ // asi puede ser llamada independientemente para cada renderer
			if (_renderer.material.HasProperty("_Alpha")) _renderer.material.SetFloat("_Alpha", Alpha);
			if (_renderer.material.HasProperty("_Tint"))
			{
				var c = _renderer.material.GetColor("_Color");
				c.a = Alpha;
				_renderer.material.SetColor("_Color", c);
			}
	}
}
