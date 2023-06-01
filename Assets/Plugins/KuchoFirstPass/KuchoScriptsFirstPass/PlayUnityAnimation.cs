using UnityEngine;
using System.Collections;

public class PlayUnityAnimation : MonoBehaviour {

	public float delay = 1f;
	private Animation unityAnim;
	
	public void Start(){
		unityAnim = GetComponent<Animation>();
		StartCoroutine(Play(delay));
	}
	
	public IEnumerator Play(float delay) {
		yield return new WaitForSeconds(delay);
		unityAnim.Play();
	}
}
