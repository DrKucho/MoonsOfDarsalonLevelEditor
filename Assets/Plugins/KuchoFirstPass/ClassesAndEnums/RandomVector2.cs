using UnityEngine;
using System.Collections;



[System.Serializable]
public class RandomVector2{
    public Vector2 value;
    public Vector2 randomRange;

	public static Vector2 GetValuePlusRandom(RandomVector2 r){
		return new Vector2 (r.value.x + (Random.Range(r.randomRange.x, -r.randomRange.x)), r.value.y + (Random.Range(r.randomRange.y, -r.randomRange.y)));
	}

	public Vector2 GetValuePlusRandom(){
		return new Vector2 (value.x + (Random.Range(randomRange.x, -randomRange.x)), value.y + (Random.Range(randomRange.y, -randomRange.y)));
	}

	public Vector2 GetValuePlusRandom2(){ // esta usa el value como un minimo de impulso y luego le suma el componente aleatorio
		Vector2 sign;
		sign.x = Random.Range(100, -100);
		if (sign.x > 0 ) sign.x = 1; else sign.x = -1;
		sign.y = Random.Range(100, -100);
		if (sign.y > 0 ) sign.y = 1; else sign.y = -1;
		return new Vector2 ((value.x + (Random.Range(randomRange.x, 0))) * sign.x, (value.y + (Random.Range(randomRange.y, 0))) * sign.y);
	}
}
