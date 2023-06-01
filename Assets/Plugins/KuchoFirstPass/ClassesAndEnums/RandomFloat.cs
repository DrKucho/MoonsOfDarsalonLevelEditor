using UnityEngine;
using System.Collections;
using System.Threading;

[System.Serializable]
public class RandomFloat{
    public float value;
    public float randomRange;

    static System.Random rand = new System.Random();

	public RandomFloat (){
		value = 0f;
		randomRange = 0f;
	}
	public float GetValuePlusRandom(){
        if (Thread.CurrentThread == LPManager.mainThread)
            return value + (Random.Range(randomRange, -randomRange));
        else
        {
            int randomRange_int = (int)(randomRange * 100000f);
            int random_int = rand.Next(-randomRange_int, randomRange_int);
            float random_float = (float)random_int/100000f;
            return value + random_float;
        }
	}
	public static float GetValuePlusRandom(RandomFloat r){
        if (Thread.CurrentThread == LPManager.mainThread)
            return r.value + (Random.Range(r.randomRange, -r.randomRange));
        else
        {
            int randomRange_int = (int)(r.randomRange * 100000f);
            int random_int = rand.Next(-randomRange_int, randomRange_int);
            float random_float = (float)random_int/100000f;
            return r.value + random_float;
        }
    }
}
[System.Serializable]
public class RandomClampedFloat{
	public float value;
	public float randomRange;
	public float min;
	public float max;

    static System.Random rand = new System.Random();

	RandomClampedFloat (){
		value = 10f;
		randomRange = 5f;
		min = 8f;
		max = 12f;
	}
	public float GetClampedValuePlusRandom(){
        if (Thread.CurrentThread == LPManager.mainThread)
            return Mathf.Clamp(value + (Random.Range(randomRange, -randomRange)), min, max);
        else
        {
            int min_int = (int)(min * 100000f);
            int max_int = (int)(max * 100000f);
            int random_int = rand.Next(min_int, max_int);
            float random_float = (float)random_int/100000f;
            return Mathf.Clamp(value + random_float, min, max);
        }
    }
}
[System.Serializable]
public class RandomClampedInt{
	public int value;
	public int randomRange;
	public int min;
	public int max;

    static System.Random rand = new System.Random();

	RandomClampedInt (){
		value = 16;
		randomRange = 16;
		min = 0;
		max = 31;
	}
	public int GetClampedValuePlusRandom(){
//        var result = Mathf.Clamp(value + (Random.Range(randomRange, -randomRange)), min, max);
        return Mathf.Clamp(value + (rand.Next(-randomRange, randomRange)), min, max); // al usar System puedo llamarla desde background threads (LPparticleGroup) 
	}
}

