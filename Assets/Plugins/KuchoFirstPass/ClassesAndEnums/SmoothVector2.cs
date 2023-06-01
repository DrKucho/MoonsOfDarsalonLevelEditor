using UnityEngine;
using System.Collections;

// instrucciones hechas a posteriori asi es como creo que funciona
// 1.- pones el nuevo valor en vec
// 2.- llamas a GetSmoothed o GetSmoothedNormalized
// 3.- Ambas funciones usan vec para machacar la posicion mas vieja de la tabla y calcula el resultado suavizado, te devuelve un valor y lo pone en vec tambien

[System.Serializable]
public class SmoothVector2{
	public Vector2 vec;
	public Vector2[] array= new Vector2[2];
	public int ind;
	public int updateCount;
	int arrayLength; // para usar temporalmente en los calculos, podria causar problemas si se accede desde varios threads ... ? 

	public  SmoothVector2(int length){
		vec = Constants.zero2;
		array = new Vector2[length];
	}

	public void Reset()
	{
		vec = Constants.zero2;
		for (int i = 0; i < array.Length; i++){
			array[i] = vec;
		}
		updateCount = 0;
	}

	public void Fill(Vector2 _vec){
		vec = _vec;
		for (int i = 0; i < array.Length; i++){
			array[i] = vec;
		}
	}
    public void Add(Vector2 v) {
        if (ind >= arrayLength)
            ind = 0;
        array[ind] = v;
        ind++;
        updateCount++;
    }
    public Vector2 GetSmoothed()
    {
        arrayLength = array.Length;
        vec = Constants.zero2;
        for (int n = 0; n < arrayLength; n++)
        {
            vec.x += array[n].x;
            vec.y += array[n].y;
        }
        vec.x = vec.x / arrayLength;
        vec.y = vec.y / arrayLength;
        return vec;
    }
    public Vector2 GetSmoothedNormalized()
    {
        arrayLength = array.Length;
        vec = Constants.zero2;
        for (int n = 0; n < arrayLength; n++)
        {
            vec.x += array[n].x;
            vec.y += array[n].y;
        }
        vec.x = vec.x / arrayLength;
        vec.y = vec.y / arrayLength;
        vec = vec.normalized;
        return vec;
    }
    public Vector2 AddVecAndGetSmoothed(){
		arrayLength = array.Length;
		if (ind >= arrayLength)
			ind = 0;
		array[ind] = vec;
		ind ++;
		updateCount++;
		vec = Constants.zero2;
		for ( int n = 0; n < arrayLength; n++)
		{
			vec.x += array[n].x;
			vec.y += array[n].y;
		}
		vec.x = vec.x / arrayLength;
		vec.y = vec.y / arrayLength;
		return vec;
	}
	public Vector2 AddVecAndGetSmoothedNormalized(){
		arrayLength = array.Length;
		if (ind >= arrayLength) ind = 0;
		array[ind] = vec;
		ind ++;
		updateCount++;
		vec = Constants.zero2;
		for ( int n = 0; n < arrayLength; n++)
		{
			vec.x += array[n].x;
			vec.y += array[n].y;
		}
		vec.x = vec.x / arrayLength;
		vec.y = vec.y / arrayLength;
		vec = vec.normalized;
		return vec;
	}
}
[System.Serializable]
public class SmoothFloat{
	public float value;
	public float[] array= new float[2];
	private int ind;
	public int updateCount;
	public float timeOfLastAdd;
	int arrayLength; // para usar temporalmente en los calculos, podria causar problemas si se accede desde varios threads ... ? 

	public  SmoothFloat(int length){
		value = 0f;
		array = new float[length];
		timeOfLastAdd = 0;
	}
	public void Reset(){
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 0;
		}
		updateCount = 0;
		timeOfLastAdd = 0;
	}
	public void Fill(float _value){
		value = _value;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = value;
		}
		timeOfLastAdd = KuchoTime.time;
	}
    public void Add(float v)
    {
        if (ind >= arrayLength)
            ind = 0;
        array[ind] = v;
        ind++;
        updateCount++;
        timeOfLastAdd = KuchoTime.time;
    }
    public float GetSmoothed(){
		arrayLength = array.Length;
        if (arrayLength > 0)
        {
            if (ind >= arrayLength)
                ind = 0;
            array[ind] = value;
            ind++;
            updateCount++;
            value = 0f;
            for (int n = 0; n < arrayLength; n++)
            {
                value += array[n];
            }
            value = value / arrayLength;
        }
        return value;
	}
}
/// <summary>
/// Nueva no solo para int sino que ademas funciona con getter y setter, mas elegante y seguramente mas practico
/// solo asigna valor a intValue y este actualiza la tabla correctamente
/// luego lee valor de intvalu y obtienes el valor de la tabla suavizado
/// </summary>
[System.Serializable]
public class SmoothInt{
    public int intValue{
        get {
            int result = 0;
            for ( int n = 0; n < array.Length; n++)
            {
                result += array[n];
            }
            result = result / array.Length;
            return result;
        }
        set {
            if (ind >= array.Length)
                ind = 0;
            array[ind] = value;
            ind ++;
            updateCount++;
        }
    }
    public int[] array= new int[2];
    private int ind;
    public int updateCount;

    public  SmoothInt(int length){
        intValue = 0;
        array = new int[length];
    }
    public void Reset(){
	    for (int i = 0; i < array.Length; i++)
	    {
		    array[i] = 0;
	    }
	    updateCount = 0;
    }
    public void Fill(int _value){
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = _value;
        }
    }
}

