using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class TerrainShadowInSprite : MonoBehaviour {
    
    [ReadOnly2Attribute] public Renderer rend;
    Material mat;
    Vector2 texSize;


    public void InitialiseInEditor(){
        rend = GetComponent<Renderer>();
    }
    
    public void CopyMyMatToSprite(){
        rend = GetComponent<Renderer>();
        rend.sharedMaterial = mat;
    }
    public void SetOneTimeShaderProperties(){
        mat = rend.sharedMaterial; // si no hay mat lo pilla del renderer y se crea un clon , que se resetea al
        if(mat != null && WorldMap.destructible)
        {
            float separation = 0.01f;
            float far = 300f;
            if (WorldMap.spritePlanes)
            {
                separation = WorldMap.spritePlanes._minSeparation * 2; // en solo una minSeparation esta destructibleGround!
                far = WorldMap.spritePlanes.farBackground.max;
            }

            mat.SetTexture(ShaderProp._AlphaTex, WorldMap.destructible.d2dSprite.AlphaTex);
            mat.SetTexture(ShaderProp._AlphaTex2, WorldMap.indestructible.d2dSprite.AlphaTex);

            if (ShaderProp._TexSize != 0) // para evitar un mensaje de error que solo sale al cargar el proyecto
                mat.SetVector(ShaderProp._TexSize, new Vector4(WorldMap.size.x, WorldMap.size.y,0 ,0));//, texSize.x/2 , texSize.y/2 )); 
        }
    }

    public void OnEnable(){
        mat = rend.sharedMaterial; // si no hay mat lo pilla del renderer y se crea un clon , que se resetea al
        SetOneTimeShaderProperties();
    }
    
    void LateUpdate(){
        if (Application.isEditor && !Application.isPlaying)
        {
            SetOneTimeShaderProperties();
        }
        else
        {
            mat = rend.sharedMaterial; // si no hay mat lo pilla del renderer y se crea un clon , que se resetea al
            mat.SetVector(ShaderProp._CameraPos_MapSize, new Vector4(transform.position.x, transform.position.y, WorldMap.destructible.d2dSprite.AlphaTex.width, WorldMap.indestructible.d2dSprite.AlphaTex.height));
            mat.SetFloat(ShaderProp._Angle, Game.sun.elipse.reverseAngleToCam);
            float dissapearFactor = Mathf.Clamp(Game.sun.finalIntensity * Game.sun.terrainShadowDissapearRateAtNight, 0, 1);

            mat.SetFloat(ShaderProp._Projected_Directional_Dissapear_At_Night, dissapearFactor);
        }
    }
}
