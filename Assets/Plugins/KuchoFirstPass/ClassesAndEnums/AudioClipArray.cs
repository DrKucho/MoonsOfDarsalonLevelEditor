using UnityEngine;
using System.Collections;

[System.Serializable]

public class AudioClipArray
{

    [Range(0f, 1.2f)] public float gain;
    public int polyGroup;
    public Vector2 randomStart;
    public Vector2 randompPitch = new Vector2(1,1);
    public UnityEngine.Audio.AudioMixerGroup mixerGroup;
    public AudioClip[] clip;
    [HideInInspector] public int index; //para seguir orden secuencial de clips // todo implementar en MOD

    public AudioClipArray()
    {
        gain = 0.7f;
        randomStart = Constants.zero2;
        polyGroup = 0;
        clip = new AudioClip[0]; // antes era 1 puede que me salte algun error por que mi codigo asuma que se crea un clip como minimo
    }
    public void Resize(int l)
    {
        clip = new AudioClip[l];
    }
    public void GetFromRandomAudioClipList(AudioClipWrap[] src)
    {
        if (src.Length > 0)
        {
            gain = src[0].gain;
            randomStart = src[0].randomStart;
            polyGroup = src[0].polyGroup;

            Resize(src.Length);
            for (int i = 0; i < clip.Length; i++)
            {
                clip[i] = src[i].clip;
            }
        }
    }
    static void CopyRandomAudioClipListToAudioClipArray(AudioClipWrap[] src, AudioClipArray dst)
    {
        if (src.Length > 0)
        {
            dst.gain = src[0].gain;
            dst.randomStart = src[0].randomStart;
            dst.polyGroup = src[0].polyGroup;

            dst.Resize(src.Length);
            for (int i = 0; i < dst.clip.Length; i++)
            {

                dst.clip[i] = src[i].clip;
            }
        }
    }
    public void CopyTo(AudioClipArray o)
    {
        o.gain = gain;
        o.randomStart = randomStart;
        o.polyGroup = polyGroup;
        o.randompPitch = randompPitch;
        o.mixerGroup = mixerGroup;
        o.clip = new AudioClip[clip.Length];
        for (int i = 0; i < clip.Length; i++)
        {
            o.clip[i] = clip[i];
        }
    }
    
    public AudioClipArray (AudioClipArray o)
    {
        gain = o.gain;
        randomStart = o.randomStart;
        polyGroup = o.polyGroup;
        randompPitch = o.randompPitch;
        mixerGroup = o.mixerGroup;
        clip = new AudioClip[o.clip.Length];
        for (int i = 0; i < o.clip.Length; i++)
        {
            clip[i] = o.clip[i];
        }
    }

    public bool CompareWith(AudioClipArray o)
    {
        if (gain != o.gain) return false;
        if (randomStart != o.randomStart) return false;
        if (randompPitch != o.randompPitch) return false;
        if (mixerGroup != o.mixerGroup) return false;
        if (clip.Length != o.clip.Length) return false;
        for (int i = 0; i < clip.Length; i++)
        {
            if (clip[i] != o.clip[i]) return false;
        }
        return true;
    }
}

