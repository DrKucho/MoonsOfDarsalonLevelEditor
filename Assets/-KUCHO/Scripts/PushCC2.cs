using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PushCC2 : MonoBehaviour {

    public float boundsThreshold = 5f; // por debajo de este temaño no sera empujado (asi evito cabezas)
	public Vector2 pushForce = new Vector2(1000,1000);
    public bool multiplyForceByMass = false;
    public bool onlyIfIlde;
    public bool onlyIfJumping;
	public bool pushPlayer;
	public string[] tags;
    public Rigidbody2D myBody; // si tiene se usara para copiar la velocity al rb del CC (y que no se caigan del camion)
	public Collider2D myCol;
	public List<ManInfo> found ;
	public float updateDelay = 0.2f;
    public bool disableGroundJoints;

    public void InitialiseInEditor()
    {
	    myCol = GetComponent<Collider2D>();
    }


	Vector2 pos;

}
