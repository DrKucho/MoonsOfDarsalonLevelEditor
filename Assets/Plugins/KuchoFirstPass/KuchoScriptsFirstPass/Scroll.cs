using UnityEngine;
using System.Collections;

public enum ScrollMode {MovePosition, OffsetTexture}
public class Scroll : MonoBehaviour {

	
	public Vector2 inc;
	public Scroll copyInc;
	public Renderer rend; // para pillar el tamaño ... no lo uso?
	public ScrollMode scrollMode = ScrollMode.MovePosition;
	public Vector2 size; // tamaño del sprite o limites de cuando quieres que haga salto de loop
	public bool loopX = true;
	public bool loopY = false;
	public bool snapToRealPixel = true; // no puedo usar lo habitual dearcade pixel y real pixel, no se por que SM.snapTpRealPixel vale 1, pero yo necesito snapear teniendo en cuenta el zoom factor
	Transform t;
	public Vector2 pos;
	Vector2 jumpDiff; // calculado en el momento de hacer loop
//	Vector2 prevPos; // no lo uso
	Vector2 initPos;
	Vector2 traveled;

	public void Start(){
		t = transform;
		pos = t.position;
//		prevPos = pos;// no lo uso
		initPos = pos;
		traveled = Constants.zero2;
	}
	
	public void Update () {
		if (copyInc) inc = copyInc.inc;
		var timeScaledInc = inc * Time.timeScale;
//		prevPos = pos;// no lo uso
		pos += timeScaledInc;
		traveled += timeScaledInc;
		// ---------------------------------- X
		if (traveled.x < -size.x) // limite avanzando hacia la izquierda
		{
			if (loopX)
			{
				jumpDiff.x = traveled.x + size.x; // en realidad es una resta por que va hacia la izquierta, traveled es negativo
				ResetX();
			}
			else inc.x *= -1;
		}
		if (traveled.x > size.x) // limite avanzando hacia la derecha
		{
			if (loopX)
			{
				jumpDiff.x = traveled.x - size.x;
				ResetX();
			}
			else inc.x *= -1;
		}
		// ---------------------------------- y
		if (traveled.y < -size.y) // limite avanzando hacia abajo
		{
			if (loopY)
			{
				jumpDiff.y = traveled.y + size.y; // en realidad es una resta por que va hacia la izquierta, traveled es negativo
				ResetY();
			}
			else inc.y *= -1;
		}
		if (traveled.y > size.y) // limite avanzando hacia arriba
		{
			if (loopY)
			{
				jumpDiff.y = traveled.y - size.y;
				ResetY();
			}
			else inc.y *= -1;
		}
		Vector2 finalPos = Constants.zero2; 
		if (snapToRealPixel)
		{
			var snapped = pos;
			snapped.x = SnapTo.ZoomFactor(pos.x);
			snapped.y = SnapTo.ZoomFactor(pos.y);
			finalPos = new Vector2(snapped.x, snapped.y);
		}
		else
		{
			finalPos = new Vector3(pos.x, pos.y);
		}

		if (scrollMode == ScrollMode.MovePosition)
		{
			t.position = new Vector3(finalPos.x, finalPos.y, t.position.z);
		}
		else
		{
			rend.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(finalPos.x / size.x, finalPos.y / size.y));
		}
	}
	public void ResetX(){
		pos.x = initPos.x + jumpDiff.x;
		if (copyInc)
		{
			int i= (int)pos.x; // trunco
			pos.x = i + KuchoHelper.GetDecimals(copyInc.pos.x); // le pongo los decimales de a quien seguimos
		}
		traveled.x = jumpDiff.x;
	}
	public void ResetY(){
		pos.y = initPos.y + jumpDiff.y;
		if (copyInc)
		{
			int i= (int)pos.y; // trunco
			pos.y = i + KuchoHelper.GetDecimals(copyInc.pos.y); // le pongo los decimales de a quien seguimos
		}
		traveled.y = jumpDiff.y;
	}
}
