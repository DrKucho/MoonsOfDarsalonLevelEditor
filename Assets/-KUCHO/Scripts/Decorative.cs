using UnityEngine;
using System.Collections;


public class Decorative : MonoBehaviour {

	public bool debug = false;
	[HideInInspector] public DecoManager decoManager; // asignado por Filler en cada llamada a WriteInDecoManager

	public int indexInDecoMap = -1; // -1 significa que no esta en decomap , se resetea en cada get back to store o destroy
	public ExplosionInvoker explosions;
	public RandomizeMySWizSprite randomizeMe;
	public Item item;

	public void InitialiseInEditor () {
		explosions = GetComponent<ExplosionInvoker>();
		randomizeMe = GetComponent<RandomizeMySWizSprite>();
		item = GetComponent<Item>();
	}


}
