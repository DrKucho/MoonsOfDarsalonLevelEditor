using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using UnityEngine.Serialization;


public class DamageReceiver : MonoBehaviour
{
    public static float angularDamageMult = 0f;// algo chapu tener este valor estatico, podria ser diferente para seguin que objetos pero ademas tengo deslizadores para modificar en inspector que se copian a esta estatica onvalidate..., con cual se esta quedando?
    [System.Serializable]
    public class HurtThreshold {
        public string colName;
        public Collider2D col;
        public float normalImpulse;
        public float veloDiffMagnitude; // aun sin usar pero si termino usando esto para los voladores 2D es aqui donde debe ir 
        [HideInInspector] public DamageReceiver dR;
        public HurtThreshold() {
            normalImpulse = 6000;
            veloDiffMagnitude = 15;
            colName = "";
        }

        public void Initi(DamageReceiver dR)
        {
            this.dR = dR;
        }

        public float GetThreshold(bool useVeloDiff)
        {
            if (useVeloDiff)
                return veloDiffMagnitude;
            return normalImpulse;
        }

        bool ShowCollider()
        {
            if (colName == null || colName == "")
                return false;
            return true;
        }
        /*
        bool ShowImpulse()
        {
            if (!dR)
                return true;
            return !dR.walkerUseVeloDiff;
        }
        bool ShowVeloDiff()
        {
            if (!dR)
                return true;
            return dR.walkerUseVeloDiff;
        }
        */
    }
    
    //public float testSpeed = 1000;


    // RECIBE LOS EVENTOS COLISION Y TRIGGER ENTER Y APLICA DAÑO 
    public bool debug = false; 
    public float debugDamageThreshold; 
     public bool debugOnCollisionEnter = false;
    public bool debugOnCollisionWithGroundOrObstacle = false;

    [SerializeField] CC cC;
    [SerializeField] AudioManager aM;
    public EnergyManager eM;
    private FakeCollision2D fakeColl;
    public Rigidbody2D myBody; // se asigna en la primera colision
    public Collider2D myCol; // se asigna en la primera colision
    public float myMass; // se asigna en la primera colision
    public Collider2D[] _collider;
    public Collider2D centerCollider;
    public int weGetHurtBy = 99999999; // dependinedo de la variable destroys we Hurt se rellena con la layer del bando contrario y se usa para comparar con la layer del collider encontrado, le pongo 9999999 para que se inicialize con una layer que no existe

    public bool unityUpdate = true;
    public bool copyZOnCollision = false;
    bool CopyZOnCollision() { return copyZOnCollision; }
     public float _zOffset = -3; // para que las cajas esten por delante de la hierba
    public bool takeDamageFromAnyObject = false;
    public bool deflectBullets;

    bool WeDontDeflectBullets() { return !deflectBullets; }
     public ArmyType takeDamageFrom = ArmyType.All;
    //    public float speedToHurtRatio = 1;
    [Header("THRESHOLDS")]
    public bool walkerUseVeloDiff;
    //public float collisionSpeedThreshold = 0;
    public HurtThreshold[] walkerHurtThresholds;
    public bool flyerUseVeloDiff;
    public HurtThreshold[] flyerHurtThresholds;
    public HurtThreshold[] notCC_HurtThresholds;
    bool IsCC() { return cC; }
    bool IsNotCC() { return !cC; }
    [HideInInspector] public bool dontUseCollisionSpeedThreshold; // otros scripts pueden activar esto para que siempre se reciba daño de las colisiones, se resetea en onenable
    //[HideInInspector][Range(0, 100f)] public float collisionSpeedMagnifier = 0f; 
    //[Range(1, 9999999)] public float allCollisionsHurtDiv = 1;
    [Header("HURT SLIDERS")]
    [Range(0,0.25f)] public float _angularDamageMult = 0.1f;
    [Range(0, 2)] public float allCollisionsDamageMult = 1;
    [Range(0, 0.02f)] public float incomingBodyMassMult = 0.02f;
    [Range(0, 1)] public float bumperDamageReductionFactor = 0.5f;

    bool AtLeastOneKindOfHurtingIsAllowed() { return hurtByGroundAllowed | hurtByObstacleAllowed | hurtByAnyRbAllowed; }

    [Header("---Ground---")]
    public bool hurtByGroundAllowed = true;
    bool HurtByGroundAllowed() { return hurtByGroundAllowed; }
    [HideInInspector] public bool internalGroundCollisionsAllowed = true;
    [Range(0, 0.6f)] public float hurtByGroundFactor = 0f;
    [Range(0, 0.6f)] public float hurtByIndestructibleGroundFactor = 0f;
    
    [Header("---Obstacle---")]
    public bool hurtByObstacleAllowed = true;
    bool HurtByObstacleAllowed() { return hurtByObstacleAllowed; }
    [Range(0, 0.6f)] public float hurtByObstacleFactor = 0.5f;
    [Range(0, 0.6f)] public float hurtByKinematicBodyFactor = 0.5f;
    [Range(0, 0.6f)] public float hurtByStaticObstacleFactor = 0.5f;

    [Header("---AnyRB---")] 
    public bool hurtByAnyRbAllowed = false;
    bool HurtByAntRbAllowed() { return hurtByAnyRbAllowed; }
    [Range(0, 0.6f)] public float hurtByAnyRbFactor = 0.5f; // para mostrar en inspector
    [Range(0, 0.6f)] public float hurtByAnyKinematicRbBodyFactor = 0.5f;
    [Range(0, 0.6f)] public float hurtByAnyStaticObstacleFactor = 0.5f;
    
    [Header(" ")]

    [Range(0, 1f)] public float groundedSpeedReductionRatio = 1;
    [Header("---Info---")]
    [ReadOnly2Attribute] public float damageToBeApplied;

    [Header("HIT SOUNDS")]
    public bool triggerHitGroundSounds = true;
    bool TriggerHitGroundSounds() { return triggerHitGroundSounds; }
     [Range(0, 100)] public float velocityToTriggerGroundHitSound = 10f;
     public bool useMonoAudioChannel = true;
     [Range(0, 200)] public float maxHitVolumeSpeed = 80f;
     public AudioClipArray audioOnHitGround;

    float hitVolume = 0.1f;
    Player playerSC;
    int colliderCharacterLayer;
    Collider2D lastCol;
    int  currentColLayer;
    bool currentColIsGroundOrVision = false;
    
    [Header("HIT PARTICLES")]
    public ExplosionInvoker selfCollisionExploInvoker;
    bool GotPS() { return selfCollisionExploInvoker; }

     public float bloodSpeedMult = 1;
     public float bloodAmountMult = 1;
     [Range(0, 1f)] public float particlesAmount = 1;
     public float particlesMax = 250;
     [Range(0, 1f)] public float particlesSpeed = 1;
     public float particlesSpeedMax = 250;



    float hurtFactor = 0;


    private Vector2 velocityBeforeCollisions;
    private float angularVelocityBeforCollisions;
    [HideInInspector] public bool collidingWithStaticCollider; // para que flyer pueda saber si debe immpedir gitat al flippingtransform
    Vector2 veloDiff;
    float absAngularVeloDiff;
    
    
}

