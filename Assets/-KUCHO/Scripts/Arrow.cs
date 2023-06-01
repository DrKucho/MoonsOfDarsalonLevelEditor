using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public int playAnimHowManyTimes = 3;
	private int animCounter = 0;
	private Vision activate;
	private Vision deactivate;
	public SWizSpriteAnimator anim;
	public SWizSpriteAnimationClip arrowAnim;

}
