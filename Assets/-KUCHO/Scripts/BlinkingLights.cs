using UnityEngine;
using System.Collections.Generic;
using Light2D;


public class BlinkingLights : MonoBehaviour
{
	public bool runOnEnable = false;
    public bool tryToSyncWLM;
    public float startDelay = 0;
	public float onDelay; // es la que se usa en la corrutina para generar el delay/rate pero privada para que nadie la pueda cambiar
    public float offDelay;
    public float vanishSpeed;
    public bool blinkThisGo = false;
    public bool resetSpritesLocalRot;
    public bool resetSpritesGlobalRot = true;
    [SerializeField] SWizSprite[] sprites;
	[SerializeField] Light2DManager[] lightManagers;
	float[] alpha;
	int index;
	bool runing;
    WaitForSeconds WaitForOffDelay;
    WaitForSeconds WaitForOnDelay;

	//void OnValidate(){
 //       if (isActiveAndEnabled)
 //       {
 //           DefineWaitForSeconds();
 //       }
	//}
	public void InitialiseInEditor()
	{
		lightManagers = GetComponentsInChildren<Light2DManager>();
		if (blinkThisGo)
		{
			sprites = GetComponentsInChildren<SWizSprite>();
		}
		else
		{
			var allSprites = GetComponentsInChildren<SWizSprite>();
			int i = 0;
			foreach (SWizSprite s in allSprites)
			{
				if (s.gameObject != gameObject)
					i++;
			}

			sprites = new SWizSprite[i];
			i = 0;
			foreach (SWizSprite s in allSprites)
			{
				if (s.gameObject != gameObject)
				{
					sprites[i] = s;
					i++;
				}
			}
		}
	}

}
