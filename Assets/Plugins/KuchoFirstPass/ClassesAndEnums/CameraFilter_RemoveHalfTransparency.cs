

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("CameraFilter_RemoveHalfTransparency")]
public class CameraFilter_RemoveHalfTransparency : MonoBehaviour {
	#region Variables
	[Range(0, 1)]
	public float cutOff = 0.5f;
	public Material fillMaterial;
	public Material simpleAlphaTestMat;
//	public RenderTexture blank; // necesito esta textura solo para ejecutar un graphics.blit y rellenar otra textura con un shader, es un pco estupido por que el resultado no importa lo que tenga esta textura ya que el sharer rellenara , haya lo que haya, un color fijo
	#endregion

	void Start () 
	{
		if(!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
//		Camera cam = GetComponent<Camera>();
//		RenderTexture camTex = cam.targetTexture;
//		blank = new RenderTexture(camTex.width , camTex.height, 24, RenderTextureFormat.ARGB32);

	}
	
	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		// la destination texture es la pantalla , que tiene restos del frame anterior, hay que vaciarla
		Graphics.Blit(destTexture, destTexture, fillMaterial); // usando un shader especial que fija un color solido, relleno tempText con pixeles transparentes 
//		simpleAlphaTestMat.SetFloat("_Cutoff", cutOff); // no funciona en el shader , no se hacer que pille variable 
		Graphics.Blit(sourceTexture, destTexture, simpleAlphaTestMat); // usando simpleAlphaTest, solo machaco los pixeles con alpha alto
	}
	
	
}