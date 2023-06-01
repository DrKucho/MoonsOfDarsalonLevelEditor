using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Serialization;


public class PushCC3 : MonoBehaviour {
	public enum GroundCondition { DontDoIt, CCIsGrounded, CcGroundMustBeBed, CcGroundMustNotBeBed, MyBodyAndCCGrounndMustMatch, MyBodyAndCCGrounndMustBeDifferent }
	public float boundsThreshold = 5f; // por debajo de este temaño no sera empujado (asi evito cabezas)
	public Vector2 pushForce = new Vector2(1000,1000);
	public bool proporcioanlforceToColcenter = true;
    public bool multiplyForceByMass = false;
    public bool onlyIfIlde;
	public bool pushPlayer;
	public string[] tags;
	public GroundCondition groundedCondition = GroundCondition.MyBodyAndCCGrounndMustBeDifferent;
	public bool doItNotGrounded;
	public Collider2D myCol;
	[ReadOnly2Attribute] public Rigidbody2D myBody;
	public List<ManInfo> found ;
	public float updateDelay = 0.2f;
    public bool disableGroundJoints;

    public void InitialiseInEditor()
    {
	    myCol = GetComponent<Collider2D>();
	    if (myCol)
	    {
		    gameObject.layer = Layers.vision;
		    myCol.isTrigger = true;
	    }
    }


	Vector2 pos;

	

}
