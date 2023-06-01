using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WaitForSecondsLUT {
    public float maxSeconds;
    public int steps;
    [HideInInspector] public WaitForSeconds[] array;
    [HideInInspector] public float[] floats;

    float stepInc;

    public void Initialise(){
        array = new WaitForSeconds[steps];
//        #if UNITY_EDITOR // por que carajo puse esto aqui?
        floats = new float[steps];
 //       #endif // por que carajo puse esto aqui?
        stepInc = maxSeconds / steps;
        for (int i = 0; i < steps; i++)
        {
            float value = stepInc * (i + 1);
            array[i] = new WaitForSeconds(stepInc * (i + 1)); // cada elemento se define con una fraccion del tiempo proporcional
//            #if UNITY_EDITOR// por que carajo puse esto aqui?
            floats[i] = value;
//            #endif// por que carajo puse esto aqui?
        }
    }
    public WaitForSeconds Get(float time){
        int index = Mathf.RoundToInt(time / stepInc);
        if (index > array.Length)
            index = array.Length - 1;
        else if (index < 0)
            index = 0;
        else if (index >= array.Length)
            index = array.Length - 1;
        return array[index];
    }
    public int GetIndex(float time){
        int index = Mathf.RoundToInt(time / stepInc);
        if (index > array.Length)
            index = array.Length - 1;
        else if (index < 0)
            index = 0;
        else if (index >= array.Length)
            index = array.Length - 1;
        return index;
    }
}

public class KuchoWaitForSeconds : MonoBehaviour {
    public static WaitForSeconds tweintyFiveCents = new WaitForSeconds(0.25f);
    public static WaitForSeconds halfSecond = new WaitForSeconds(0.5f);
    public static WaitForSeconds seventyFiveCents = new WaitForSeconds(0.75f);
	public static WaitForSeconds fullSecond = new WaitForSeconds(1f);
	public static WaitForSeconds secondAndHalf = new WaitForSeconds(1.5f);
	public static WaitForSeconds twoSeconds = new WaitForSeconds(2f);

    public WaitForSecondsLUT oneSecLUT;
    public WaitForSecondsLUT fiveSecsLUT;
    public WaitForSecondsLUT sixtySecsLUT;
    public WaitForSecondsLUT threeMinsLUT;

    public static KuchoWaitForSeconds instance;

    public void Awake(){
        instance = this;
        oneSecLUT.Initialise();
        fiveSecsLUT.Initialise();
        sixtySecsLUT.Initialise();
        threeMinsLUT.Initialise();

    }
    public WaitForSeconds GetFromSixtySecs(float time){
        return sixtySecsLUT.Get(time);
    }
}
