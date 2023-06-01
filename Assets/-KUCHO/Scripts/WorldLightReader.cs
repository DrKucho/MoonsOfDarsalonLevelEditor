using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using UnityEngine.Profiling;

public class WorldLightReader : MonoBehaviour {

    // monitoriza la cantidad de luz que hay en transform.position, lo tengo como script aparte por que el personaje puede tener varias main light que necesitan leer esta inormacion
    // y no onviene llamar a la rutina que calcula la luz media de la zona muchas veces por que usa get pixels y ordena una tabla de valores por lo que puede consumir mucho

    // se actualiza siempre al encender el gameObject y ...
    // si updateEachSeconds es mayor de cero se actualizara cada x segundos

    public string notes;
	public bool debug = false;
	
    public enum Method {JustOneLightPixel, AverageOfBiggerValues, AverageOfLowerValues, AllValuesAverage}
    public Method method = Method.AverageOfBiggerValues;
    bool IsJustOnePix(){ return method == Method.JustOneLightPixel; }
    bool IsNotJustOnePix() { return !IsJustOnePix(); }
    bool ShowWinnerVisibilities()
    {
        if (winnerVisibilities == null || winnerVisibilities.Length == 0)
            return false;
        return true;
    }
    [Range(2, 20)] public int squareSide = 0;
    public int howManyPixToMind = 8;
	public float updateEachSeconds = 0.2f;
	[Range (0,1)]
	public float threshold;
	public RangeFloat thresholdCounter;
	public WorldLightPixel lightPixel;
    public float[] visibilities;
    public float[] winnerVisibilities;
	//Color[] pixels; // intento de ahorrar memory allocation , pero no funciona, por dos motivos 1, GetPixels siempre reserva memoria, no puede usar una tabla que le pases tu, 2.- aunque le pase la tabla en la llamada, parece que no envia una referencia sino que envia una copia de la tabla entera por que alli los pixeles son modificados pero los pixeles de aqui no sufren cambios
	
	[System.Serializable]
	public class RangeFloat{
		public float min;
		public float max;
	//	@new Range(min, max)
		public float _value;
		public bool IsMax(){
			if (_value == max) return true;
			return false;
		}
		public bool IsMin(){
			if (_value == min) return true;
			return false;
		}
		// GETTERS AND SETTERS el mimo nombre que la variable que modifican pero con la primera mayuscula
		public float Value{
			get{
				return _value;
			}
			set{
				if (value > max) _value = max;
				else if (value < min) _value = min;
				else _value = value;
			}
		}
		// asi era en javascript , conservo por que s confuso puede que lo haya traducido mal
//	    public float get Value(){ return _value;} 
//	    public void set Value(float value){
//	 	   if (value > max) _value = max;
//	 	   else if (value < min) _value = min;
//	 	   else _value = value;
//	 	}
	}

	public void Awake (){
		Initialize();
	}
	public void OnValidate(){
		Initialize();

	}
    
	public void Initialize(){
		visibilities = new float[squareSide * squareSide];
        if (method != Method.AllValuesAverage)
        {
            if (squareSide <= 1)
            {
                method = Method.JustOneLightPixel;
            }
            winnerVisibilities = new float[howManyPixToMind];
        }
        else
        {
            winnerVisibilities = null;
        }
        //	pixels = new Color[visibilities.length];

        //pixels = new Color[visibilities.length]; // no sirve de nada, por alguna razon si envio la tabla de pixeles para que la usen en world light system extras , la usa, se rellena de valores, pero luego me vuelve vacia ....???
    }


	
	public void Switch(){
		if (this.enabled) this.enabled = false;
		else this.enabled = true;
	}
	public void Switch(bool onOff){
		this.enabled = onOff;
	}
}
