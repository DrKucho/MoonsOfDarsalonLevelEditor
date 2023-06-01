using UnityEngine;
using System.Collections;

public class PingPong : MonoBehaviour {

	public Vector2 inc;
	public Vector2 maxPosition;
	public Vector2 minPosition;
	private Transform t;
	public Vector2 dir;
	
	public void Start(){
		t = transform;
        previousFrameTime = KuchoTime.time;
    }
    float previousFrameTime;
	public void Update () {
        float elapsed = KuchoTime.time - previousFrameTime;
        if (elapsed < 0.014f) // ha tardado menos de lo que tarda si va a 60 Hz?
        {

        }
        else // tod0 bien
        {
            TransformHelper.SetPosX(t, t.position.x + inc.x * dir.x);

            if (t.position.x > maxPosition.x)
                dir.x = -1;
            else if (t.position.x < minPosition.x)
                dir.x = 1;
            previousFrameTime = KuchoTime.time;
        }
    }
}
