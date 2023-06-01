using UnityEngine;
using System.Collections;

public class GetAlphaData : MonoBehaviour {

	public int index = 0;
	public int alpha = 0;

	void Update(){ //  print (this + " UPDATE ");
		index = PixelTools.AlphaDataGetIndex((Vector2)transform.position);
		alpha = (int)PixelTools.AlphaDataGetPixel(index, true);
	}
}
