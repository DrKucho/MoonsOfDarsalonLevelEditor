using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPFixtureTimeShifter : MonoBehaviour {

	public bool validateMe;
	public LPFixture fix;
	public Vector2 startOffset;
	public Vector2 endOffset;
	public float speed;

	Vector3 previousPos;
	float previousScaleX;
	Vector2 originalFixOffset;

	void OnValidate () {
		if (!fix)
			fix = GetComponent<LPFixture>();
	}
	void Awake(){
		fix.makeItReusable = false;
		originalFixOffset = fix.Offset;
	}
	void OnEnable(){ // ha de ejecutarse antes de LPFixture para que funcione bien!
        LPManager.onMT_Update_2 += MyUpdate;
		fix.Offset = startOffset;
	}
	void OnDisable(){
        LPManager.onMT_Update_2 -= MyUpdate;
	}
	void MyUpdate(){
		fix.enabled = false;
		Vector2 newOffset;
		newOffset.x = Mathf.MoveTowards(fix.Offset.x, endOffset.x, speed);
		newOffset.y = Mathf.MoveTowards(fix.Offset.y, endOffset.y, speed);
		fix.Offset = newOffset;
		fix.enabled = true;
		if (fix.Offset == endOffset)
            LPManager.onMT_Update_2 -= MyUpdate;
	}
}
