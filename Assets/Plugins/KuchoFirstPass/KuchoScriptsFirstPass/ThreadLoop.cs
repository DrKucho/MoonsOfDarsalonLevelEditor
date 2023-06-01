using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.Profiling;
public class ThreadLoop
{
    public string name;
    public Semaphore semaphore;
    public int slots;
    public bool signal;
    public Thread thread;
    public bool isPaused;
    public bool executing;
    public bool pauseRequest = false; // la activas desde fuera del loop y el loop debe comprobarlo y llamarse a Pause();

    public delegate void PausableThreadDelegate(ThreadLoop threadLoop);

    public ThreadLoop(string name, PausableThreadDelegate threadLoopDelegate) // recibo una delegate (la funcion que correra en otro hilo) que tiene un solo parametro y ese parametro del tipo KuchoThread, osea de este mismo tipo, el thread podra acceder a esta class que lo creó , principalmente por que asi puede tener acceso a su handle
    {
        if (Application.isPlaying)
        {
            thread = new Thread(() => threadLoopDelegate(this)); // al arancarse el thread se le enviara un dato que es este mismo script
            semaphore = new Semaphore(1, 1);
            this.name = name;
            thread.Name = name;
        }
        else
        {
            Debug.Log(this + " QUIEN ME LLAMA CON EL JUEGO PARADO? MAL!");  
        }
    }

    public void Pause()
    {
        pauseRequest = false;
        if (!isPaused)
        {
            isPaused = true;
            signal = semaphore.WaitOne();
        }
    }

    public void Run()
    {
        pauseRequest = false;
        if (!executing)
        {
            thread.Start();
            executing = true;  
        }
        if (isPaused)
        {
            Profiler.BeginSample(" SEMAPHORE RELEASE ");
            slots = semaphore.Release();
            Profiler.EndSample();
            isPaused = false;
        }
    }
    public void Sleep(int miliseconds)
    {
        Thread.Sleep(miliseconds);
    }
    public bool noMoreNaps;
    public void LoopNapTillSignal() {
        noMoreNaps = false;
        while (noMoreNaps == false)
        {
            Thread.Sleep(1);
        }
    }

}
