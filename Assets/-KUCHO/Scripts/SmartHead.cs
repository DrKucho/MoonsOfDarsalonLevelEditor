using UnityEngine;
using System.Collections;


public class SmartHead : MonoBehaviour {

	public bool debug = false;
	public Vector3 posOnBigAngle;
	public float updateEvery = 0.3f;
	[Range (0.5f, 4f)] public float normal = 0.5f;
	[Range (0.5f, 4f)] public float fast = 1f;
	 Vector2 previousPos;
	 Vector2 diff;
	float absDiffX;
	public CC cC;
	public SWizSpriteAnimator anim ;
	public Renderer rend;
	public Collider2D col;

	SWizSpriteAnimationClip forward;
	SWizSpriteAnimationClip forwardFast;
	SWizSpriteAnimationClip backward;
	SWizSpriteAnimationClip backwardFast;
	SWizSpriteAnimationClip up;
	SWizSpriteAnimationClip upFast;
	SWizSpriteAnimationClip down;
	SWizSpriteAnimationClip downFast;
	SWizSpriteAnimationClip stable;

	SWizSpriteAnimationClip clipToPlay;
	SWizSpriteAnimationClip currentClip;
	int frame = 0;

	bool _backwards = false;
	float newLocalScaleX = 1;

	public float diameter;

	[ReadOnly2Attribute] public float realLookAtDir = 1f; // direccion a la que miramos, no es local, no depende de los padres como localScale.X si vale -1 la cabeza mira a la izqueirda, si vale 1 mira a la derecha.
	[ReadOnly2Attribute] public float aimXsign;

//	float previousRotation;
	float attachPointScale = 1;


}
