using UnityEngine;
using System.Collections;

// ahora los decimales snapeados se guardan en una tabla de dos dimensioones, una dimension es para cada zoom , y la otra para los valores decimales snapeados que se repetiran muchas veces igual a partes iguales
// los decimales sin snapear del numero entrante se usan como indice para localizar en la tabla un valor snapeado, esta tecnica ocupa algo de memoria en almacenar la tabla
// pero es mucho mas rapida que la antigua que tenia muchos ifs 

public enum Snap {None, ArcadePixel, RealPixel};


public class SnapTo {

	public float decimalsForZoom2 = 0f;
	public float decimalsForZoom3 = 0f;
	public float decimalsForZoom4 = 0f;
	public float decimalsForZoom5 = -0.1f;
	public float decimalsForZoom6 = -0.1f;
	public static float dec;
	public static Point realPixel; // lo fija SM, si usamos renderTexture = SM.pixelResolution, si no usamos renderTexture = SM.zoomFactor, se usa cuando se hace SnapTo.RealPixel( Vector3, bool, bool);
	public static int zoomFactor; //lo fija SM , alli es float, creo que por los cauculos por dejar abierto a que pueda ser 0.5 algun dia, pero aqui no necesito para nada que sea float
	public static int zooms = 6; // cuantos niveles de zoom?
	public static int steps = 60; // cuanta precision de la tabla de decimales? celdas que tendran repetido el snap
	static int stepsMinusOne; // necesito este numero para calular el indice de la tabla y que no de nunca el maximo que daria out of range
	static float[,] snapSteps;		


	public static void FillSnapSteps(){
		stepsMinusOne = steps-1;
		float decs; // los decimales a los que se ha de snapear , repetidos por todas las celulaas
		float g; // lo que mide el grupo de repeticiones
		snapSteps = new float[zooms, steps] ;
		for (int z = 0; z < zooms; z++)
		{
			float usefullZ = (float)(z + 1); // la tabla 0 corresponde al Zoom 1 
			float decsInc = 1f / usefullZ; 
			float gInc = steps / usefullZ;
			decs = 0f;
			g = gInc;
			for (int s = 0; s < steps; s++)
			{
				if (s >= g)
				{
					decs += decsInc;
					g += gInc;
				}
				snapSteps[z,s] = decs;
			}
		}
	}
	public static float PixelRes(float n, int pixelResolution){
//		int intN = (int)n;
        int intN = Mathf.RoundToInt(n);
		float decimals = n - intN;
		int index = Mathf.RoundToInt(Mathf.Abs(decimals) * stepsMinusOne);
		float snappedDecimals = snapSteps[pixelResolution -1, index];
		float snapped;
		if (n > 0) snapped = intN + snappedDecimals;
		else snapped = intN - snappedDecimals;
		return snapped;
	}
	public static float ZoomFactor(float n){ // Conversion desde JS --> podria ser un int ...?
		int intN = (int)n;
		float decimals = n - intN;
		int index = Mathf.RoundToInt(Mathf.Abs(decimals) * stepsMinusOne);
		float snappedDecimals = snapSteps[zoomFactor -1, index];
		float snapped;
		if (n > 0) snapped = intN + snappedDecimals;
		else snapped = intN - snappedDecimals;
		return snapped;
	}
	public static Vector3 RealPixel(Vector3 p, bool snapX, bool snapY){
		Vector3 snapped = Constants.zero3;
		if (snapX) snapped.x = PixelRes(p.x, realPixel.x);
		if (snapY) snapped.y = PixelRes(p.y, realPixel.y);
		snapped.z = p.z;
		return snapped;
	}
	public static float Pixel(float n, Snap snap){
		switch (snap){
			case (Snap.RealPixel):
				return PixelRes(n, (int)realPixel.x);
			case (Snap.ArcadePixel):
				//return Mathf.RoundToInt (Mathf.RoundToInt(n * 100f)/ 100f); // ojo esto estaba sin las fs al final de los 100 con lo que estaba truncando n a int , perdiendo todos los decimales, de todas formas, por que quiero operar con solo dos decimales???
				return Mathf.RoundToInt(n);
			case (Snap.None):
				return n;
		}
		return 0f;
	}
	public static Vector2 Pixel(Vector2 p, Snap snapX, Snap snapY){
		Vector2 snapped;
		snapped.x = SnapTo.Pixel(p.x, snapX);
		snapped.y = SnapTo.Pixel(p.y, snapY);
		return snapped;
	}
	public static Vector3 Pixel(Vector3 p, Snap snapX, Snap snapY){
		Vector3 snapped;
		snapped.x = SnapTo.Pixel(p.x, snapX);
		snapped.y = SnapTo.Pixel(p.y, snapY);
		snapped.z = p.z;
		return snapped;
	}
	public static Vector3 Pixel(Vector3 p, int zoomX, int zoomY){ // nadie llama a esta
		Vector3 snapped;
		snapped.x = SnapTo.PixelRes(p.x, zoomX);
		snapped.y = SnapTo.PixelRes(p.y, zoomY);
		snapped.z = p.z;
		return snapped;
	}
	public static Vector3 LocalPixel(Vector3 p, bool snapX, bool snapY){ // nadie llama a esta
		Vector3 decimals;
		if (snapX) decimals.x = Mathf.RoundToInt (p.x) - p.x; else decimals.x = 0;
		if (snapY) decimals.y = (Mathf.RoundToInt (p.y) + dec) - p.y; else decimals.y = 0;
		decimals.z = 0;
		return decimals;
	}

//	static float GetSnapDecs_NO(int pixelResolution){
//		float snapDecs = 0f;
//		float absN = Mathf.Abs(pixelResolution);
//		int intAbsN = (int)absN;
//		float dec = absN - intAbsN;
//		float frac = 0f;
//		switch (pixelResolution){
//			case (1):
//				snapDecs = 0;
//				break;
//			case (2):
//				frac = dec/0.5f;
//				if (frac > 1) snapDecs = 0.5f;
//				else snapDecs = 0;
//				break;
//			case (3):
//				frac = dec/0.3333333333333333f;
//				if (frac > 2) snapDecs = 0.6666666666666f;
//				else if (frac > 1) snapDecs = 0.333333333333f;
//				else snapDecs = 0;
//				break;
//			case (4):
//				frac = dec/0.25f;
//				if (frac > 3) snapDecs = 0.75f;
//				else if (frac > 2) snapDecs = 0.5f;
//				else if (frac > 1) snapDecs = 0.25f;
//				else snapDecs = 0;
//				break;
//			case (5):
//				frac = dec/0.2f;
//				if (frac > 4) snapDecs = 0.8f;
//				else if (frac > 3) snapDecs = 0.6f;
//				else if (frac > 2) snapDecs = 0.4f;
//				else if (frac > 1) snapDecs = 0.2f;
//				else snapDecs = 0;
//				break;
//			case (6):
//				frac = dec/0.16666666666666666666f;
//				if (frac > 5) snapDecs = 0.16666666666666666666f * 5;
//				else if (frac > 4) snapDecs = 0.16666666666666666666f * 4;
//				else if (frac > 3) snapDecs = 0.16666666666666666666f * 3;
//				else if (frac > 2) snapDecs = 0.16666666666666666666f * 2;
//				else if (frac > 1) snapDecs = 0.16666666666666666666f;
//				else snapDecs = 0;
//				break;
//		}
//		return snapDecs;
//	}
	//static function Pixel(p:Vector3, snapX:Snap, snapY:Snap ){
	//	Vector3 snapped;
	//	switch (snapX){
	//		case (Snap.RealPixel):
	//			snapped.x = RealPixel(p.x, SM.snapToRealPixel.x);
	//			break;
	//		case (Snap.ArcadePixel):
	//			snapped.x = Mathf.RoundToInt (p.x);
	//			break;
	//		case (Snap.None):
	//			snapped.x = p.x;
	//			break;
	//	} 
	//	switch (snapY){
	//		case (Snap.RealPixel):
	//			snapped.x = RealPixel(p.y, SM.snapToRealPixel.y);
	//			break;
	//		case (Snap.ArcadePixel):
	//			snapped.y = Mathf.RoundToInt (p.y) + dec;
	//			break;
	//		case (Snap.None):
	//			snapped.y = p.y;
	//			break;
	//	}
	//	snapped.z = p.z;
	//	return snapped;
	//	
	//}
	//	public void Awake(){
	//		switch (Game.zoomFactor){
	//			case (2):
	//				dec = decimalsForZoom2;
	//				break;
	//			case (3):
	//				dec = decimalsForZoom3;
	//				break;
	//			case (4):
	//				dec = decimalsForZoom4;
	//				break;
	//			case (5):
	//				dec = decimalsForZoom5;
	//				break;
	//			case (6):
	//				dec = decimalsForZoom5;
	//				break;
	//		}
	//	
	//	}
	//	public void LateUpdate(){ // ESTA DEBE BORRARSE UNA VEZ LOS VALORES SEAN DEFINITIVOS
	//		switch (Game.zoomFactor){
	//			case (2):
	//				dec = decimalsForZoom2;
	//				break;
	//			case (3):
	//				dec = decimalsForZoom3;
	//				break;
	//			case (4):
	//				dec = decimalsForZoom4;
	//				break;
	//			case (5):
	//				dec = decimalsForZoom5;
	//				break;
	//			case (6):
	//				dec = decimalsForZoom5;
	//				break;
	//		}
	//				
	//	}
}
