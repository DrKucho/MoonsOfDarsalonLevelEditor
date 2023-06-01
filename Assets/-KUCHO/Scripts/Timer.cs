using UnityEngine;
using System.Collections;


public enum TS {STOP, PLAY, PAUSE, END};

[System.Serializable]
public class Timer{
	public MinMax time;
	// GETTERS AND SETTERS esta es la que se cambia y fija la otra variable que es la que se usa
	public bool passed {
		get{
			return m_passed;
		} 
		set{
			m_passed = value;
			m_runing = !value;
		}
	}
	public bool runing {
		get{
			return m_runing;
		} 
		set{
			m_runing = value;
			m_passed = !value;
		}
	}
	[HideInInspector][ReadOnly2Attribute] public TS status = TS.STOP;
	[HideInInspector][ReadOnly2Attribute] public bool m_runing = false;
	[HideInInspector][ReadOnly2Attribute] public float actualTime; // la cantidad de segundos resultantes del ultimo calculo aleatorio con time.min y max
	[HideInInspector][ReadOnly2Attribute] public float start;
	[HideInInspector][ReadOnly2Attribute] public float end;
	[HideInInspector][ReadOnly2Attribute] public float pause;
	[HideInInspector][ReadOnly2Attribute] public float remaining;
	[HideInInspector][ReadOnly2Attribute] public bool m_passed = false;
	public delegate void OnPassed();
	[HideInInspector]public OnPassed onPassed;

	public void Restart(){
		Restart(1);
	}
	public void Restart(float factor){
		passed = false;
		actualTime = time.GetRandom() * factor;
		start = Time.time;
		end = start + actualTime;
		remaining = end - Time.time;
		status = TS.PLAY;
	}
	public void RestartSeconds(float seconds){
		RestartSeconds(seconds, 1);
	}
	public void RestartSeconds(float seconds, float factor){
		passed = false;
		start = Time.time;
		actualTime = seconds * factor;
		end = start + actualTime;
		remaining = end - Time.time;
		status = TS.PLAY;
	}
	public void Update(){
		if (status == TS.PLAY)
		{
			remaining = end - Time.time;
			if (!passed && remaining <= 0)
			{
				remaining = 0f;
				passed = true;
				status = TS.STOP;
				if (onPassed != null) onPassed();
			}
		}
	}
	public void Pause(){
		if (status == TS.PLAY)
		{
			status = TS.PAUSE;
			pause = Time.time;
			remaining = end - Time.time;
		}
	}
	public void Stop(){
		remaining = 0f;
		status = TS.STOP;
		passed = true;
	}
	public void Stop(bool _passed){
		passed = _passed;
		m_runing = false; // si, aqui tiene sentido modificar la m_ por que hemso parado, si o si 
		remaining = 0f;
		status = TS.STOP;
	}
	public void StopAndTriggerDelegate(){
		Stop();
		if (onPassed != null) onPassed();
	}
	public void Continue(){
		if (status == TS.PAUSE)
		{
			end = Time.time + remaining;
			status = TS.PLAY;
			runing = true;
		}
	}
	public void Play(){
		if (status == TS.STOP)
			Restart(1);
		else if (status != TS.PLAY) // podria estar en pause
		{
			end = Time.time + remaining;
			status = TS.PLAY;
			runing = true;
		}
	}
}

