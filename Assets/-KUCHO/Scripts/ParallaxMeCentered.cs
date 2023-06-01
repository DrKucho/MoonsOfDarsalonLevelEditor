using UnityEngine;
using System.Collections;

public class ParallaxMeCentered : MonoBehaviour {

	public bool debug = false;
	Vector3 originalPos;
//	public Transform follow;
	Vector3 myPos;
	Vector2 movement;// copia de camGND.movement
	public Vector2 speed;
	public Snap pixelSnapX = Snap.ArcadePixel;
	public Snap pixelSnapY = Snap.ArcadePixel;
//	Vector2 distanceToWorldCenter; // no lo uso
	public Material mat;
	Vector2 shift;
	Transform _t;


    //	void OnDisable(){
    //        Game.G.parallax.Remove(this);
    //		SM.instance.camMovement.onReady -= EnableMeWhenCameraIsReady;
    //	}
    //	void Start(){
    //        Game.G.parallax.Add(this);
    //		SM.instance.camMovement.onReady += EnableMeWhenCameraIsReady;
    //		this.enabled = false; // luego lo activa cuando la camara este ready
    //	}
    //	public void EnableMeWhenCameraIsReady(){
    //		StartCoroutine(EnableMeWhenCameraIsReadyCoroutine() );
    //	}
    //	IEnumerator EnableMeWhenCameraIsReadyCoroutine(){
    //		if (debug) print (this + " DENTRO DE ENABLE ME WHEN CAM IS READY");
    //		int counter = 0;
    //		while (!SM.instance.camMovement.firstStep || counter == 10)
    //		{
    //			yield return null;
    //			if (debug) print (this + " DENTRO DE SPERANDO A CAMARA FIRST STEP, COUNTER=" + counter);
    //			counter ++;
    //		}
    //		if (counter >= 10) {
    //			if (debug) print (this + " DENTRO DE ENABLE ME CUANDO CAMARA ES FIRST STEP PERO HAN PASADO 10 FRAMES Y NADA, ASI QUE APAGO ESTE GO");
    //			this.enabled = false;
    //		}
    //		else {
    //			if (debug) print (this + " DENTRO DE ENABLE ME CUANDO CAMARA ES FIRST STEP, CAM HA DADO EL PRIMER PASO, HAN PASADO FRAMES=" + counter);
    //			this.enabled = true;
    //		}
    //	}
    public void InitialiseInEditor()
    {
	    var allRends = GetComponentsInChildren<Renderer>();
	    if (allRends.Length > 1)
	    {
		    for (int i = 1; i < allRends.Length; i++)
		    {
			    if (allRends[i].sharedMaterial != allRends[0].sharedMaterial)
			    {
				    Debug.LogError(this + " TENGO MATERIALES DIFERENTES, DEBERIAN SER TODOS IGUALES");
				    return;
			    }
		    }
	    }

	    if (allRends.Length > 0)
		    mat = allRends[0].sharedMaterial;
    }

    public void Awake()
    {
        _t = transform;
        _t.position = SnapTo.Pixel(transform.position, Snap.ArcadePixel, Snap.ArcadePixel);
        originalPos = _t.localPosition;

    }
    public void Initialise(){
        //transform.position = SnapTo.Pixel(transform.position, Snap.ArcadePixel, Snap.ArcadePixel); // hacer esto aqui mal se va incrementando desfase cada vez que mueres!
        //originalPos = _t.localPosition;// hacer esto aqui mal se va incrementando desfase cada vez que mueres!
        myPos = SnapTo.Pixel(originalPos , Snap.ArcadePixel, Snap.ArcadePixel);
        myPos.z = _t.position.z;		


			myPos = originalPos;
//            follow = transformToFollow;
//			movement = -SM.instance.camMovement.transformMovement;
//            MyUpdate(SM.instance.camMovement.transformMovement); // si no la ejecuto ahora, la corrutina en espera camGND.Initialize se ejecutara antes, pondra la camara en ready, y actualizara movement a 0,0, esto causa que LateUpdate se ejecute dos veces pero solo ocurre una vez
//		}
	}
	
    public void MyUpdate(Vector2 _movement){ 
//        movement = -SM.instance.camMovement.transformMovement;
        movement = -_movement;
	
//		distanceToWorldCenter = (Vector2)follow.position - halfWorldSize; // no lo uso
	
		myPos.x += movement.x * -speed.x * ParallaxFather.globalSpeedMult.x;
		myPos.y += movement.y * -speed.y * ParallaxFather.globalSpeedMult.y;
        myPos.z = _t.position.z; // asi puedo cambiarla en inspector

        _t.localPosition = SnapTo.Pixel(myPos, pixelSnapX, pixelSnapY);
	}
}
