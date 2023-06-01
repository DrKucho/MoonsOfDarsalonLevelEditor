using UnityEngine;
using System.Collections;
using System;


public class KuchoLPDrawer : MonoBehaviour {
    public bool debug;
	public KuchoLPMultiDrawer multiDrawer;
	[ReadOnly2Attribute] public bool inMultiDrawerList; // por alguna razon de comodidad que no recuerdo se añade a la lista de multidrawer en cada enable/disable, pero ahroa me da problemas con otros scripts por que ha de estar actualizado en Awake, asi que lo hago en awake tb, pero para no repetir tengo este flag
	[ReadOnly2Attribute] public int index; // el indice en la lista multiDrawer.drawer
	public LPParticleMaterial mat; // ahora el material va asociado al drawer, tiene sentido y lo necesito asi para poder scar el material una vez tengo el drawer que he sacado de user data, si no seria imposible
	[HideInInspector] public Int32 matInt;// lo necesitan los particlegroup al ionicializar particulas, representa el material en si para lel motor
	public ParticleSystem shuriken;
	public ParticleSystem.Particle[] particles;
	[ReadOnly2Attribute] public int shurikenParticleCount;
	[ReadOnly2Attribute] public int spi;
	[HideInInspector]public int justKilledParticleCount;
	[Header("-ECO MODE-")]
	public bool SendFixedLifeTimeToShuriken_ForNoAgeKillParticles = false;
	[Header("-UserData-")]
	public bool rotatingParticles = false;
	public bool sendStartLifeTimeToShuriken = true;
	public bool sendStartSizeToShuriken = true;
	[Header("-Interpolation-")]
	public float VelocityMult_interpolation = 1;
	public float angularVelocityMult_interpolation = 1; 
	[Header("-NormalData-")]
    public float zPos = 0;
	public float linearVelToAngVelFactor = 0.2f;

	public bool sendLifeTimeToShuriken = true;
	public bool sendColorToShuriken = false;
	public bool ageAlphaChange = false;
	public float vanishTime = 1; // cuando le queden 1 segundos de vida comenzara a hacerse transparente reduciendo alpha a cantidad vanishSpeed cada frame;

	int newPartCount = 0; //cada vez que se emite se va sumando , se resetea al final del frame

	bool NotRotatingParticles(){ return !rotatingParticles; }
	bool ShowSendColorOptions(){
		if (!rotatingParticles && multiDrawer && multiDrawer.particleSystem && multiDrawer.particleSystem.GetColors)
			return true;
		else
			return false;
	}

    Vector3 newPartPos = Constants.zero3;

    int emitIndex = 0;
    ParticleSystem.EmitParams[] emitParams = new ParticleSystem.EmitParams[200];
    int[] newParts = new int[200];


    [HideInInspector] public bool updateShuriken = false;

}
	