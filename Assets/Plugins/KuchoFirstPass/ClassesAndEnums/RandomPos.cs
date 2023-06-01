using UnityEngine;
using System.Collections;



[System.Serializable]
public class RandomPos
{
	public bool enabled = true;
    public MinMax x;
    public MinMax y;
    public SidesAlgorythm x_SignAlgorythm;
	public int chancesRight = 50;
    public SidesAlgorythm y_SignAlgorythm;
	public int chancesUp = 50;
    public Vector3 offset;
	[HideInInspector] public Vector2 sign = new Vector2(1,1);
    [ReadOnly2Attribute] public Vector3 finalPos;

	bool XisRandom(){ return x_SignAlgorythm == SidesAlgorythm.Random;}
	bool YisRandom(){ return y_SignAlgorythm == SidesAlgorythm.Random;}

//	public static Vector2 GetPos(RandomPos randomPos){
//		Vector2 pos;
//		randomPos.sign.x = Helper.GetSignFromSidesAlgorythm(randomPos.x_SignAlgorythm, randomPos.chancesRight, (int)randomPos.sign.x); // fija el signo del offset aleatorio para coordenada x
//		pos.x = ((Random.Range(randomPos.x.min, randomPos.x.max))* randomPos.sign.x);
//		randomPos.sign.y = Helper.GetSignFromSidesAlgorythm(randomPos.y_SignAlgorythm, randomPos.chancesUp, (int)randomPos.sign.y); // fija el signo del offset aleatorio para coordenada y
//		pos.y = ((Random.Range(randomPos.y.min, randomPos.y.max))* randomPos.sign.y);
//		pos += (Vector2)randomPos.offset;
//		return pos;
//	}

	public Vector3 GetFinalPos(float mult){ // fija finalPos pero tambien la devvuelve
        if (enabled)
        {
            sign.x = KuchoHelper.GetSignFromSidesAlgorythm(x_SignAlgorythm, chancesRight, (int)sign.x); // fija el signo del offset aleatorio para coordenada x
            finalPos.x = ((Random.Range(x.min, x.max)) * sign.x);
            sign.y = KuchoHelper.GetSignFromSidesAlgorythm(y_SignAlgorythm, chancesUp, (int)sign.y); // fija el signo del offset aleatorio para coordenada y
            finalPos.y = ((Random.Range(y.min, y.max)) * sign.y);
            finalPos.z = 0;
            finalPos += offset;
            finalPos *= mult;
        }
        else
            finalPos = offset;// Constants.zero3;
		return finalPos;
	}
	public Vector3 GetFinalPos(float mult, float signForX){ // fija finalPos pero tambien la devvuelve
		if (enabled)
		{
			sign.x = signForX;
			finalPos.x = ((Random.Range(x.min, x.max)) * sign.x);
			sign.y = KuchoHelper.GetSignFromSidesAlgorythm(y_SignAlgorythm, chancesUp, (int)sign.y); // fija el signo del offset aleatorio para coordenada y
			finalPos.y = ((Random.Range(y.min, y.max)) * sign.y);
			finalPos.z = 0;
			finalPos += offset;
			finalPos *= mult;
		}
		else
			finalPos = offset;// Constants.zero3;
		return finalPos;
	}
	public Vector3 GetOnlyYFinalPos(float mult){ // fija finalPos pero tambien la devvuelve
		if (enabled)
		{
			finalPos.x = 0;
			sign.y = KuchoHelper.GetSignFromSidesAlgorythm(y_SignAlgorythm, chancesUp, (int)sign.y); // fija el signo del offset aleatorio para coordenada y
			finalPos.y = ((Random.Range(y.min, y.max)) * sign.y);
			finalPos.z = 0;
			finalPos += offset;
			finalPos *= mult;
		}
		else
			finalPos = offset;// Constants.zero3;
		return finalPos;
	}
	public float GetX(float mult){
		sign.x = KuchoHelper.GetSignFromSidesAlgorythm(x_SignAlgorythm, chancesRight, (int)sign.x); // fija el signo del offset aleatorio para coordenada x
		finalPos.x = ((Random.Range(x.min, x.max)) * sign.x);
		finalPos.x += offset.x;
		finalPos *= mult;
		return finalPos.x;
	}
	public float GetY(float mult){
		sign.y = KuchoHelper.GetSignFromSidesAlgorythm(y_SignAlgorythm, chancesUp, (int)sign.y); // fija el signo del offset aleatorio para coordenada y
		finalPos.y = ((Random.Range(y.min, y.max)) * sign.y);
		finalPos.y += offset.y;
		finalPos *= mult;
		return finalPos.y;
	}
}
