using UnityEngine;
using System.Collections;


[System.Serializable]
public class MinMax
{
    public float min;
    public float max;
	[HideInInspector] public float value;
	//Constructor
	public MinMax(float a, float b){
		min = a;
		max = b;
	}

	public float GetRandom(){
		value = Random.Range(min, max);
		return value;
	}
	public float GetAverage(){
		return (min + max) / 2f;
	}
    public float GetLerp(float t){
        return Mathf.Lerp(min, max, t);
    }
}


[System.Serializable]
public class MinMaxInt
{
	public int min;
	public int max;
	[HideInInspector] public int value;

	//Constructor
	public MinMaxInt(int a, int b){
		min = a;
		max = b;
	}

	public float GetRandom(){
		value = Random.Range(min, max);
		return value;
	}
}