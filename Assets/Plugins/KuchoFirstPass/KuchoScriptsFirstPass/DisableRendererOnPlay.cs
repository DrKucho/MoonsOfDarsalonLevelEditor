using UnityEngine;
using System.Collections;

public class DisableRendererOnPlay : MonoBehaviour {

	public bool standalone;
	public bool editor;
	void Start(){
		Renderer renderer = GetComponent<Renderer>();
		if (renderer)
		{
			if (Application.isEditor)
			{
				if (editor)
					renderer.enabled = false;
			}
			else if(standalone)
				renderer.enabled = false;
		}
	}

}
