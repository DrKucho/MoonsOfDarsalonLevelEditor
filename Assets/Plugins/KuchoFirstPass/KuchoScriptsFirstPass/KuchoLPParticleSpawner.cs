using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Profiling;


[System.Serializable]
public class LiquidExplosion{
	public KuchoLPParticleSpawner spawner;
	public bool basedOnPixelsDestroyed = false;
}

public class KuchoLPParticleSpawner : MonoBehaviour {

	public bool debug;
	[Tooltip("Spawner: should this begin spawning when the game starts? Acclerator: should this accelerate operate?")]
	public bool Active = true;
    public float disableIfGroundRadius;
	public LPTarget target;
//	 public AimSlave aimSlave;
	public float velocity;
	[HideInInspector] public Vector2 linearVelocityVector = Constants.zero2;
	[HideInInspector] public Color _Color = Color.white;

	public float startDelay = 0f;
	[Range(0.2f,60)] public float spawnsPerSecond = 10f; // es calculado en onvalidate solo a titulo informativo
	[ReadOnly2Attribute] public float delayBetweenSpawns = 0.1f; // el que realmente uso para la frecuencia de spwaneado
	public float randomRangeAdd = 0f; // se calculara un valor entre el negativo y el positivo de este valor para añadir a delayBetweenSpawns

	public bool nonStop = true; // KUCHO HACK

	bool ShowDuration() { return !nonStop; }

	 public float duration = 0.5f; // KUCHO HACK lo uso en ExplosionManager
	[ReadOnly2Attribute] public float durationMultiplier = 1f; // KUCHO HACK
	[ReadOnly2Attribute] public float realDuration = 0.5f; // KUCHO HACK
	public bool useShake = true;
	bool UsingShake(){return useShake;}
	public Vector2 shake;
	public Vector2 offset;
	public float aimSlaveVectorToOffset;
    public float particleCountProtectionMult = 0.5f;
    public float particleCountProtectionThreshold = 5800;
    public float contacCountProtectionThreshold = 2000;

	[Tooltip("If this is set only one particle will be spawned at a time, rather than a shape filled with particles")]
	public bool SpawnOnlyOneParticle;
	[Tooltip("Reference to a LPParticleGroup component. Must be set in order for this to work")]
	[ReadOnly2Attribute] public LPParticleGroup pg;
	[SerializeField] [HideInInspector] LPParticleGroupCircle pgCircle; // sdi es un circulo he de saberlo por que el offser se aplica diferente, sin tocar localPosition y si necesidad de padre
    [HideInInspector] public LPParticleGroupBox pgBox; // sdi es un circulo he de saberlo por que el offser se aplica diferente, sin tocar localPosition y si necesidad de padre
	public bool JoinGroups;

	bool first = true;
	IntPtr lastgroup;
	LPManager lpman;
	//bool spawning;
	bool IsPlaying(){
		return Application.isPlaying;
	}
	bool NoTarget(){
		return !target;
	}



    float newSpawnTime;
    private float stopSpawningTime;
    static Collider2D[] groundFound = new Collider2D[20];
    
    public Vector2 inputAim;

}
