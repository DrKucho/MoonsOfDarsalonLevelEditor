//#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using System;
using System.IO;
using Debug = UnityEngine.Debug;

public static class Terminal 
{
    public static void StartScript(string path, AudioClip finishSound, string sayAtEnd, string logHeader, string errorDetection, Delegates.String onFinished, string description, Delegates.String logReceiver)
    {
        if (scriptRuninng)
            Debug.LogError("CAN'T START SHELL SCRIPT BECAUSE THERE IS OTHER SHELL SCRIPT RUNNING?");
        else
            EditorCoroutines.Execute(StartScriptCoroutine(path, finishSound, sayAtEnd, logHeader, errorDetection, onFinished, description, logReceiver));
    }

    public static bool scriptRuninng = false;
    public static IEnumerator StartScriptCoroutine(string path, AudioClip finishSound, string sayAtEnd, string logHeader, string errorDetection, Delegates.String onFinished, string description, Delegates.String logReceiver)
    {
        scriptRuninng = true;
        Process proc = new Process
        {
            StartInfo = new ProcessStartInfo {FileName = path, Arguments = "", UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true}
        };
        proc.Start();
        bool errorDetected = false;
        while (!proc.StandardOutput.EndOfStream)
        {
            string line = logHeader + proc.StandardOutput.ReadLine();
            if (line != null)
            {
                string upper = line.ToUpper();
                Debug.Log(upper);
                if (upper.Contains(errorDetection))
                {
                    errorDetected = true;
                }
            }

            if (logReceiver != null)
                logReceiver(line);
            yield return null;
        }
        Debug.Log("SHELL SCRIPT FINISHED");
        if (sayAtEnd != "" && !errorDetected)
        {
            Say(sayAtEnd, 5);
        }
        if (errorDetected)
        {
            Say("SOME ERROR HAPPENED!", 5);
        }

        scriptRuninng = false;
        EditorExtras.PlayClip(finishSound, 0, false);
        if (onFinished != null)
            onFinished(logHeader);
    }

    public static void ExecuteCommand(string command)
    {
        ShellHelper.ShellRequest req = ShellHelper.ProcessCommand(command, "");
        //req.onLog += delegate(int logType, string log) { Debug.Log(arg2); };
    }
    public static void ExecuteCommandInCurrentDirectory(string command)
    {
        ShellHelper.ShellRequest req = ShellHelper.ProcessCommand(command, Directory.GetCurrentDirectory());
        //req.onLog += delegate(int logType, string log) { Debug.Log(arg2); };
    }
    public static void ExecuteCommandInCurrentDirectory(string command, string workDirectory)
    {
        ShellHelper.ShellRequest req = ShellHelper.ProcessCommand(command, workDirectory);
        //req.onLog += delegate(int logType, string log) { Debug.Log(arg2); };
    }

    static float letterToSeconds = 0.085f;
    public static void Say(string phrase)
    {
        Say(phrase, 0);
    }

    public static DateTime lastSayTime;

    public static DateTime Say(string phrase, float safeTime)
    {
        if (safeTime > 0 && (System.DateTime.Now - lastSayTime).Seconds < safeTime)
        {
            EditorCoroutines.Execute(SayCoroutine(phrase, safeTime));
            return GetEndingTime(phrase);
        }
        else
            return SayNow(phrase);
    }

    public static DateTime SayNow(string phrase)
    {
        lastSayTime = System.DateTime.Now;
        var s = phrase.ToCharArray();
        List<char> l = new List<char>();
        foreach (char c in s)
        {
            if (c == 32)
            {   
                l.Add((char)92);// \
            }
            l.Add(c);
        }
        string command = "say " + new string(l.ToArray());
        ShellHelper.ShellRequest req = ShellHelper.ProcessCommand(command, "");
        return GetEndingTime(phrase);
    }

    public static IEnumerator SayCoroutine(string phrase, float safeTime)
    {
        int count = 0;// contador de seguridad por si mi logica del tiempo fallase
        while ((DateTime.Now - lastSayTime).Seconds < safeTime && count < 400) // 300 = 5 segundos en frames de 60 Hz
        {
            count++;
            yield return null;
        }
        SayNow(phrase);
    }
    public static DateTime GetEndingTime(string phrase)
    {
        float dur = phrase.Length * letterToSeconds;
        var now = DateTime.Now; 
        DateTime endTime = now;
        endTime = endTime.AddSeconds(dur);
        return endTime;
    }
}
//#endif
