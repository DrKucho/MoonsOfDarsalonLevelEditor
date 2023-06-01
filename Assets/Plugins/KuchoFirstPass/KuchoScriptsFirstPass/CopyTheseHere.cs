using UnityEngine;
using System.Collections;

public class CopyTheseHere : MonoBehaviour {

	public GameObject[] GO;

	public void Awake() {
		int i = 0;
		foreach (GameObject g in GO){
			var clone = Instantiate(g, Constants.zero3, Constants.zeroQ, transform); 
			clone.name = clone.name.Substring(0,clone.name.Length - 7); // le quito el finel (Clone)
			clone.name += "(C) " + i.ToString();
			i ++;
		}
	}
}
