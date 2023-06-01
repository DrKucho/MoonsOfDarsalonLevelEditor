using UnityEngine;
using System.Collections;

public class AudioManagerSaver : MonoBehaviour {

	[TextArea]
	public string _ = " ESTE SCRIPT TIENE QUE ESTAR EN EL MISMO GO QUE EL RENDERER PARA QUE SALTEN LAS LLAMADAS A ONBECAMEVISIBLE...";
	public bool debug = false;
	Renderer _renderer;
	public float switchOffDelay = 1.5f;
	public bool isVisible = true;
	
	public AudioManager aM;

	
}
