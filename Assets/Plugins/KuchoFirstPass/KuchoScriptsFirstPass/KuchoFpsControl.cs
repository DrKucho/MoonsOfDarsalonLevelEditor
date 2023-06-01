using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class KuchoFpsControl : MonoBehaviour
{


	public static float fpsUpdateInterval = 0.5f;
	static float previousAccurateFps;
	public static float accurateFps;
	public static SmoothFloat smoothAccurateFps;
	public static float myFps;
	public static float oneDivDeltaTime;
	public static float accurateFpsAccum = 0.0f; // FPS fpsAccumulated over the interval
	public static int fpsFrames = 0; // fpsFrames drawn over the interval
	public static float fpsTimeLeft; // Left time for current interval

	public static float myDeltaTime;
	public static float previousFrameTime;
	public static float thisFrameTime;
	public static float lastBadMyDeltaTime;
	public static float lastBadDeltaTime;
	public static int lastFrameWithHighDelta;
	public static int framesOK;
	public static int framesLostInLastInterval;
	public static int intervalFrame = 0;
	public static float lastIntervalRealTime;

	public static bool applicationFocus = true;

	//---MyFpsCounter
	public static int myFpsCount = 0;
	public static int myPreviousFpsCount = -1;

	public static KuchoFpsControl instance;
	

	public static float lastDeltaTime;
	public static float lastDeltaDiff;
	public static float deltaDiffPeak;


	public static float vsyncCount;
	public static int diff;


}

