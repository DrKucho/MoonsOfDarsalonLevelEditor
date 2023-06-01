using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    public AudioClip audio;
    public int sampleRate = 44100;
    public int channels = 2;
    public static List<AudioRecorder> instances;
    public string name;

    public void OnEnable()
    {
        instances.Add(this);
    }

    public void OnDisable()
    {
        instances.Remove(this);
    }

    public void Initialise(float seconds)
    {
        audio = AudioClip.Create(name , (int) (sampleRate * seconds), channels, sampleRate, false);
        pos = 0;
    }

    private int pos = 0;
    public void OnAudioFilterRead(float[] data, int channels)
    {
        pos += data.Length;
        int diff =  pos - audio.samples;
        if (diff > 0) // nos pasamos?
            pos = diff;

        audio.SetData(data, pos);
    }
}
