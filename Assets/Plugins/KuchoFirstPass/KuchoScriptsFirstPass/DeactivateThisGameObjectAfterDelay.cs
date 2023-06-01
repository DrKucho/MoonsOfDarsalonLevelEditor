using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DeactivateThisGameObjectAfterDelay : MonoBehaviour {

	public string notes = "";
	public bool debug = false;
	public DoItAt doIt = DoItAt.Start;
    public enum FramesOrSeconds {Frames, Seconds}
    public FramesOrSeconds framesOrSeconds = FramesOrSeconds.Seconds;
    bool Frames(){return framesOrSeconds == FramesOrSeconds.Frames;}
    bool Seconds(){return framesOrSeconds == FramesOrSeconds.Seconds;}
    public int segment;
    public float seconds = 0;
    public int frames = 0;
    
}
