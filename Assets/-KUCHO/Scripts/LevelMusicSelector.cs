using UnityEngine;
using System.Collections;

public class LevelMusicSelector : MonoBehaviour {

	
	public LevelMusic[] music;
	public bool turnOffInEditor = true;
	public int musicIndex = 0;
	public AudioSource[] audioSources= new AudioSource[4];

}
