using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParallaxMaterilasManager : MonoBehaviour {

    [System.Serializable]
    public class ParallaxMaterialSettings
    {
        public Renderer rend;
        [Range (-180f,180f)] public float hue;
        [Range (-1f,2)]public float val;
        [Range (-1f,2)]public float sat;
        [Range (-1f,2)]public float cont;

        public ParallaxMaterialSettings(Renderer rend, float hue, float val, float sat, float cont)
        {
            this.rend = rend;
            this.hue = hue;
            this.val = val;
            this.sat = sat;
            this.cont = cont;
        }
        public ParallaxMaterialSettings(Renderer rend)
        {
            this.rend = rend;
        }
    
    }
    [Range (-180f,180f)]public float hue;
    [Range (-0.5f,0.5f)]public float val;
    [Range (-0.5f,0.5f)]public float sat;
    [Range (-0.5f,0.5f)]public float cont;

    public List<ParallaxMaterialSettings> mats;

	
    
    public void InitialiseInEditor () {
        if (isActiveAndEnabled)
        {
            if (gameObject)
            {
                Renderer[] allRends = gameObject.GetComponentsInChildren<Renderer>();
                if (mats == null)
                    mats = new List<ParallaxMaterialSettings>();
                mats.Clear();
                foreach (Renderer rend in allRends)
                {
                    bool found = IsInList(rend.sharedMaterial);
                    if (!found)
                    {
                        ParallaxMaterialSettings newPms = new ParallaxMaterialSettings(rend); // le meto el material
                        GetOriginalValues(newPms); // relleno los valores 
                        mats.Add(newPms);
                    }
                }
            }
            else
            {
                print(" NO GAMEOBJECT?");
            }
        }
	}
    public void GetOriginalValues (ParallaxMaterialSettings pms) {
        Material mat = pms.rend.sharedMaterial;
        if (mat)
        {
            if (mat.HasProperty(ShaderProp._Hue))
            {
                pms.hue = mat.GetFloat(ShaderProp._Hue);
            }
            if (mat.HasProperty(ShaderProp._Sat))
            {
                pms.sat = mat.GetFloat(ShaderProp._Sat);
            }
            if (mat.HasProperty(ShaderProp._Cont))
            {
                pms.cont = mat.GetFloat(ShaderProp._Cont);
            }
            if (mat.HasProperty(ShaderProp._Val))
            {
                pms.val = mat.GetFloat(ShaderProp._Val);
            }
        }
    }
	
    void Start(){
        OnValidate();
    }
    
	void OnValidate () {
        if (isActiveAndEnabled)
        {
            if (mats != null)
            {
                foreach (ParallaxMaterialSettings pms in mats)
                {
                    Material mat = pms.rend.sharedMaterial;
                    float curr;
                    if (mat.HasProperty(ShaderProp._Hue))
                    {
                        mat.SetFloat(ShaderProp._Hue, pms.hue + hue);
                    }
                    if (mat.HasProperty(ShaderProp._Sat))
                    {
                        mat.SetFloat(ShaderProp._Sat, pms.sat + sat);
                    }
                    if (mat.HasProperty(ShaderProp._Cont))
                    {
                        mat.SetFloat(ShaderProp._Cont, pms.cont + cont);
                    }
                    if (mat.HasProperty(ShaderProp._Val))
                    {
                        mat.SetFloat(ShaderProp._Val, pms.val + val);
                    }
                }
            }
        }
	}
    public bool IsInList(Material matToFind){
        foreach (ParallaxMaterialSettings pms in mats)
        {
            if (pms.rend.sharedMaterial == matToFind)
                return true;
        }
        return false;
    }
}
