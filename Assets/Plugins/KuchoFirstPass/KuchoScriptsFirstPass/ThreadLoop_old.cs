/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ThreadLoop {

    public Thread thread;
    public EventWaitHandle handle;
    public bool pauseRequest = false; // la activas desde fuera del loop y el loop debe comprobarlo y llamarse a Pause();
    public delegate void KuchoThreadDelegate(ThreadLoop threadLoop);
    //    public bool debug = false;

    public ThreadLoop(KuchoThreadDelegate threadLoopDelegate) // recibo una delegate (la funcion que correra en otro hilo) que tiene un solo parametro y ese parametro del tipo KuchoThread, osea de este mismo tipo, el thread podra acceder a esta class que lo creó , principalmente por que asi puede tener acceso a su handle
    {
        if (Application.isPlaying)
        {
            thread = new Thread(() => threadLoopDelegate(this)); // al arancarse el thread se le enviara un dato que es este mismo script
            handle = new EventWaitHandle(true, EventResetMode.AutoReset);
        }
        else
        {
            Debug.Log(this + " QUIEN ME LLAMA CON EL JUEGO PARADO? MAL!");
        }
    }
    public bool Run(){
        bool allGood = true;
        switch (thread.ThreadState)
        {
            case (ThreadState.Running): // ya estaba corriendo, no hagas nada
                pauseRequest = false;
                break;
            case (ThreadState.WaitSleepJoin):
                handle.Set();
                pauseRequest = false;
                break;
            case (ThreadState.Unstarted):
                thread.Start();
                pauseRequest = false;
                break;
            case (ThreadState.Stopped): // ojo, un thread no puede volver a ponerse en marcha despues de parado
                pauseRequest = false;
                allGood = false;
                break;
            default:
                Debug.LogError(" NO HE PODIDO ECHAR A ANDAR EL THREAD POR QUE ESTABA " + thread.ThreadState.ToString());
                allGood = false;
                break;
        }
        if (allGood)
        {
            for (int i = 0; i < 40000; i++)
            {
                if (thread.ThreadState == ThreadState.Running)
                {
                    return true;
                }
            }
            Debug.LogError(" NO HE PODIDO ECHAR A ANDAR EL THREAD 40000 INTENTOS Y SIGUE " + thread.ThreadState.ToString());
        }
        return false;

    }
    // SOLO FUNCIONAN LLAMANDOLAS DESDE DENTRO DEL THREAD !?!? POR ESO EXISTE pauseRequest
    public void Pause(){
        switch (thread.ThreadState)
        {
            case (ThreadState.WaitSleepJoin):
                pauseRequest = false;
                break;
            case (ThreadState.Running):
                pauseRequest = false;
                try
                {
                    handle.WaitOne();
                    for (int i = 0; i < 40000; i++)
                    {
                        if (thread.ThreadState != ThreadState.Running)
                        {
                            handle.WaitOne();
                            return;
                        }
                    }
                    Debug.LogError(" NO HE PODIDO PARAR EL THREAD 40000 INTENTOS Y SIGUE " + thread.ThreadState.ToString());
                }
                catch (System.ObjectDisposedException ex)
                {
                    Debug.LogError(thread.Name + "EXCEPTION=" + ex.ObjectName + " NO HE PODIDO PAUSAR EL THREAD");
                }
                break;
        }
    }
    public void Sleep(int miliseconds){
        Thread.Sleep(miliseconds);
    }
}
*/
