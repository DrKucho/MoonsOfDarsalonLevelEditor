using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;

public class PruebasBitMaskthreadState : MonoBehaviour {

    public int running;
    public int stopRequested;
    public int suspendRequested;
    public int background;
    public int unstarted;
    public int stopped;
    public int waitSleepJoin;
    public int suspended;
    public int abortRequested;
    public int aborted;
    	
    void Do () {
        aborted = (int)ThreadState.Aborted;
        abortRequested = (int)ThreadState.AbortRequested;
        background = (int)ThreadState.Background;
        running = (int)ThreadState.Running;
        stopped = (int)ThreadState.Stopped;
        stopRequested = (int)ThreadState.StopRequested;
        suspended = (int)ThreadState.Suspended;
        suspendRequested = (int)ThreadState.SuspendRequested;
        unstarted = (int)ThreadState.Unstarted;
        waitSleepJoin = (int)ThreadState.WaitSleepJoin;
    }

}
