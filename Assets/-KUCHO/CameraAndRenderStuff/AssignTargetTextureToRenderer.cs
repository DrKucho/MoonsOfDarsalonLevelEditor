using UnityEngine;
using System.Collections;


public class AssignTargetTextureToRenderer : MonoBehaviour {

    public Camera cam;
    public Renderer rend;
    
	public void DOIT(){
        if (cam && rend)
            rend.material.mainTexture = cam.targetTexture;
	}
}
