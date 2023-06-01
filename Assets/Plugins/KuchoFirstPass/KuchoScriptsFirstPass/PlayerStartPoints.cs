using UnityEngine;
using System.Collections;

public class PlayerStartPoints : MonoBehaviour {

	
	public Transform[] trans;
	public int i = 0;
	
	public Transform NextPoint(){
		i++;
		if (i >= trans.Length) i = 0;
		return trans[i];
	}
}
