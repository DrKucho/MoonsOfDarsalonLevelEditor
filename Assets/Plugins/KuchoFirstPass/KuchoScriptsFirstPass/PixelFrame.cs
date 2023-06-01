using UnityEngine;
using System.Collections;

public class PixelFrame : MonoBehaviour {

	public Transform left;
	public Transform right;
	public Transform top;
	public Transform bottom;

	Camera cam;
	Vector2 origin;

	void Awake()
	{
		cam = GetComponentInParent<Camera>();
	}
	public void Initialise(Vector2 realResolution, float zoom){
		if (gameObject.activeInHierarchy)
		{

			var size = realResolution / zoom;

			origin = cam.ScreenToWorldPoint(Constants.zero3);

			TransformHelper.SetPosX(left, origin.x);
			left.localScale = new Vector3(0.5f, size.y, 0.5f);

			TransformHelper.SetPosX(right, origin.x + size.x);
			right.localScale = new Vector3(0.5f, size.y, 0.5f);

			TransformHelper.SetPosY(top, origin.y + size.y);
			bottom.localScale = new Vector3(size.x, 0.5f, 0.5f);

			TransformHelper.SetPosY(bottom, origin.y);
			top.localScale = new Vector3(size.x, 0.5f, 0.5f);
		}

	}
}
