using UnityEngine;
using System.Collections;

public class AnimationClipIDs : MonoBehaviour {
	public static bool countMode;
	public static int animationCount;
	public static void Reset(){
		countMode = true;
		animationCount = 0;
	}

	public bool debug = false;
	private int i= 0;
	public bool clipFound = false; // se actualiza para cada intento de reproducir animacion
	public int[] ID; // OJO!!! ha de haber el mismo numero que de animaciones !
	public SWizSpriteAnimator anim;
	public AnimationManager animManager;


}
