using UnityEngine;
using System.Collections;
using Light2D;

[System.Serializable]
public class SkyLight : MonoBehaviour {
	LightSprite ls;
	SkyManager sky;

	public Color fromCosmosTexture; // el color del cosmos en nuestra posicion pixel
	public Color afterWorldMapAdjust;
	//public Color afterSunColor;
	public Color lerpCosmosPointColor;
	public Color atmosColor; // copia de SkyManager solo para mostrar inspector , lo necesito para saber el color del cielo real
	public Color plusAtmosColor;
	public Color afterDayLightReduction;
	public Color mixWithAverage;
	public Color sunColor;
	public float sunColorFactor;
	public Color mixWithSunColor;
	public Color desaturatedColor;
//	public Color plusDayWhite; // eliminado , mejor desaturar y ya
	public Color lightColor;

	    float activationAlpha;
	float targetActivationAlpha;

	SkyLights lights;
	Item item;
	
}
