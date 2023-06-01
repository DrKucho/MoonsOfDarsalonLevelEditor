using UnityEngine;
using System.Collections;

// TENGO OTRA CLASS CASI IGUAL AUDIO CLIP LIST WTF?!?!?!

[System.Serializable]
public class AudioClipWrap
{
    public AudioClip clip;
    public float gain;
    public int polyGroup;
    public Vector2 randomStart;
    public Vector2 randomPitch = new Vector2(1,1);
	public UnityEngine.Audio.AudioMixerGroup mixerGroup;
    [HideInInspector] public bool flag; //por ahora solo para diferenciar nombres de no nombres en bases.voicelist

	public AudioClipWrap (){
		gain = 0.7f;
	}
}
