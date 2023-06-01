
using UnityEngine;
using System.Collections;

using UnityEngine.Profiling;

public class CorruTimer : MonoBehaviour {

	public string notes;
	public float time;
	public float randomRange;
	[ReadOnly2Attribute] public bool running = false;
	[Header("Last Run")]
	[ReadOnly2Attribute] public float start;
	[ReadOnly2Attribute] public float end;
	[ReadOnly2Attribute] public float duration;
	public Coroutine coroutine;
    int LastFrameStartCall = -1;
    private int callsOnSameFrameCount = 0;
    
}
