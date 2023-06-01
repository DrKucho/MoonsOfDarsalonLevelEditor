using UnityEngine;
using System.Collections;


public class TileComponent : MonoBehaviour {

//	public enum RendererType {None, SwitchableNormal, SwitchableInverse, NotSwitchable}
//	public RendererType rendererType = RendererType.None;
	public bool switchable = true;
	public bool inverseSwitch = false;
//	public int tileGridMaterial = -1;
	[HideInInspector] public Renderer rend;
	[HideInInspector] public SWizSpriteMaterialAssigner matAssigner;
	[HideInInspector] public Light2D.LightSprite lightSprite;
	[HideInInspector] public Light2D.LightObstacleSprite lightObstacle;
	[HideInInspector] public SpritePlane spritePlane;
	public float originalAlpha = 1f;
	[ReadOnly2Attribute] public KuchoTile tile;

//	bool ShowMat(){
//		if (tileGridMaterial >= 0)
//			return true;
//		return false;
//	}
	public void InitialiseInEditor(){
		rend = GetComponent<Renderer>();
		lightSprite = GetComponent<Light2D.LightSprite>();
		lightObstacle = GetComponent<Light2D.LightObstacleSprite>();
		matAssigner = GetComponent<SWizSpriteMaterialAssigner>();
		spritePlane = GetComponent<SpritePlane>();
	}

	public void SetMaterial(Material backMat, Material coverMat){
		if (lightSprite || lightObstacle)
			return;
		Material matToAssign = null;

		if (transform.position.z < WorldMap.spritePlanes.lightCoverPlane)
			matToAssign = coverMat;
		else
			matToAssign = backMat;

		if (matAssigner)
			matAssigner.SetNewMaterial(matToAssign);
		rend.sharedMaterial = matToAssign;
	}
	public void SetAlpha(float newAlpha){
		if (switchable)
		{
			if(inverseSwitch)
				newAlpha = 1 - newAlpha;
			
			newAlpha *= originalAlpha;

            if (newAlpha > 0)
            {
                rend.enabled = true;

                if (Application.isPlaying) // no funciona en parado por que hay que usar material y da error, leak materials
                {
                    if (rend.sharedMaterial.HasProperty(ShaderProp._Color))
                    {
                        Color c = rend.material.color;
                        c.a = newAlpha;
                        rend.material.SetColor(ShaderProp._Color, c);
                    }
                    else
                    {
                        rend.material.SetFloat(ShaderProp._Alpha, newAlpha);
                    }
                }

                // restaura material
                //				if (Application.isEditor && tileGridMaterial > 0 && newAlpha >= 1)
                //				{
                //					Texture mainTex = rend.sharedMaterial.mainTexture;
                //					rend.sharedMaterial = tile.tileGrid.materials[tileGridMaterial];
                //					rend.sharedMaterial.mainTexture = mainTex;
                //				}

            }
            else
            {
#if UNITY_EDITOR
                if (!rend)
                    InitialiseInEditor();
#endif
                rend.enabled = false;
            }
        }
	}
}
