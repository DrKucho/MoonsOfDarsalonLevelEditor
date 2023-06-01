using System;
using UnityEngine;
using System.Collections;


public class SWizOnSpriteChangedGlobal : MonoBehaviour {

	
	public float alpha= 1;
	public bool changeAlphaOnShader = true;
	public Material material;
	public bool resetMaterialAddOnEnable = true;
    public float add = 0;
	public SWizOnSpriteChanged[] childs;
	public static int _Add;
	public static bool _AddIsSet;
	
	public void InitialiseInEditor(){
		childs = GetComponentsInChildren<SWizOnSpriteChanged>();
	}

	private void Awake()
	{
		if (!_AddIsSet)
		{
			_Add = Shader.PropertyToID("_Add");
			_AddIsSet = true;
		}
	}

	void OnEnable(){
		if (resetMaterialAddOnEnable)
			SetAdd(add);
	}
	public void SetAdd(float _add){
        if (material && material.HasProperty(_Add))
        {
            material.SetFloat(_Add, _add); // no copio el Add porque asi se me quedn ya con el valor que tenia el shader original (compensacion de luz)
        }
    }
    public void RestoreAdd()
    {
        if (material && material.HasProperty(_Add))
            material.SetFloat(_Add, add);
    }
    
	public void ApplyAll(){
		for (int n = 0; n < childs.Length; n++){
			childs[n].OnSpriteChanged();
		}
	}

}
