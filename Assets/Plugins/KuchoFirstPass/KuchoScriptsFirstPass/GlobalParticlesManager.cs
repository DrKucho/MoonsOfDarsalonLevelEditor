using UnityEngine;
using System.Collections;

public class GlobalParticlesManager : MonoBehaviour {

	public static bool debug = false;
	public bool _debug = false;
	public bool allowParticles = true;
	public bool allowExplosions = true;
	public static int aliveParticlesRightNow;
	public static int maxParticles = 1000;
	public int _maxParticles = 1000;
	public Vector2 guiPos = new Vector2(-300,40);
	public static GlobalParticlesManager instance;
	
	public void Awake(){
		instance = this;
	}
	public void Update () {
		maxParticles = _maxParticles;
		debug = _debug;
		aliveParticlesRightNow = 0;
//		if (Input.GetKeyDown(_GreyColor)) allowParticles = !allowParticles;
//		if (Input.GetKeyDown("e")) allowExplosions = !allowExplosions;
	}
	public static void LimitParticles(ParticleSystemWrap psw){
		int goal = psw.maxParticles;
		float totalWish = goal + aliveParticlesRightNow;
		float excess = totalWish - maxParticles;
		int particlesICanCreate = 0;
		if (excess > 0){
		 	particlesICanCreate = instance.ReducedAmountOfParticles(excess, goal);
		 	if (particlesICanCreate < psw.minParticles) particlesICanCreate = psw.minParticles;
			if (debug) print (psw.PS.gameObject.name + " IMITING: GOAL=" + goal + " TTL WISH=" + totalWish + " EXCSS=" + excess + " PART I CAN CREATE=" + particlesICanCreate);
		}
		else {
			particlesICanCreate = goal;
			if (debug) print (psw.PS.gameObject.name + "ALL GOOD: GOAL=" + goal + " PARTS I CAN CREATE=" + particlesICanCreate);
		}
		psw.PS.maxParticles = particlesICanCreate;
	}
	public int ReducedAmountOfParticles(float excess, float goal){ // son floats por que estos valores se multiplican con otros floats antes de llegar aqui y supuestamente es mejor no mezclar
		float _maxParticles = (float)maxParticles;
		if (excess > _maxParticles * 2)     return (int)(goal * 0.2f);
		if (excess > _maxParticles * 1.75f) return (int)(goal * 0.4f);
		if (excess > _maxParticles * 1.5f)  return (int)(goal * 0.55f);
		if (excess > _maxParticles * 1.25f) return (int)(goal * 0.7f);
		if (excess > _maxParticles * 1.13f) return (int)(goal * 0.83f);
		if (excess > _maxParticles * 1)     return (int)(goal * 0.9f);
		return 0;
	}
	//	if (excess > 0){
	//		particlesICanCreate = excess * ((totalWish - maxParticles) / (goal * decreaseFactor));
	//		if (particlesICanCreate < expManager.minParticles) particlesICanCreate = expManager.minParticles;
	//		if (debug) print (expManager.gameObject.name + " IMITING: GOAL=" + goal + " TTL WISH=" + totalWish + " EXCSS=" + excess + " PART I CAN CREATE=" + particlesICanCreate);
	//	}
	
	public void OnGUI_NO(){
		if (debug){
			int y = 0;
			GUI.Label(new Rect(guiPos.x, guiPos.y + y, 1000, 20), System.String.Format("AlivParticles : {0}", aliveParticlesRightNow)); y += 20;
		}
	}
}
