using UnityEngine;
using System.Collections;


public class ForegroundTerrainInnerShadowInSprite : MonoBehaviour {

    [ReadOnly2Attribute] public Texture2D alphaTex;

    [ReadOnly2Attribute] public Renderer rend;
    [ReadOnly2Attribute] public Material mat;
    [ReadOnly2Attribute] public Terrain2D myTerrain2D;
    Vector2 texSize;


    public void InitialiseInEditor(){
        rend = GetComponent<Renderer>();
        if (rend)
            mat = rend.sharedMaterial; // si no hay mat lo pilla del renderer y se crea un clon , que se resetea al
        myTerrain2D = GetComponentInParent<Terrain2D>();
        
    }
    
    public void CopyMyMatToSprite(){
        rend = GetComponent<Renderer>();
        rend.sharedMaterial = mat;
    }
    public void SetShaderProperties(){
        if(mat != null)
        {
            mat.SetTexture("_AlphaTex", alphaTex);
            mat.SetVector(ShaderProp._TexSize, new Vector4(WorldMap.size.x, WorldMap.size.y,0 ,0));//, texSize.x/2 , texSize.y/2 ));
        }
    }

    public void OnEnable(){
        // esto no puede estar en initialize in editor por que las texturas se borran al guardar nivel
        if (myTerrain2D && myTerrain2D.d2dSprite.alphaTex)
            alphaTex = myTerrain2D.d2dSprite.alphaTex;
        SetShaderProperties();
    }
        
}
