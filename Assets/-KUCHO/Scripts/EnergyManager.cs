using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;



public enum OnDeadAction { ReturnToStoreOrDestroy, DeactivateGameObject, Nothing, RestartLevel };

[System.Serializable]
public class FakeCollision2D
{
    public GameObject gameObject;
    public Collider2D collider;
    public Collider2D myCollider;
    public LPParticle lpParticle;
    public LPFixture lpFixture;
    public Vector2 relativeVelocity;
    public float veloDiffMagnitude;
    public float normalImpulse; // el mas grande
    public int contactCount;
    public Vector2 point; // el punto con el normalImpulse Mas grande
    public Vector2 normal; // la normal del punto con normalImpulse mas grande
    public Rigidbody2D rigidbody;
    public Rigidbody2D myRigidbody;
    public Transform transform;
    public MonoBehaviour monoBehabiour;

}
[System.Serializable]
public class FakeContactPoint2D
{

}
public class LastHurtingThings
{
    public GameObject gameObject;
    public Collider2D collider; 
    public float power;
    public bool grabbed;
    public float time;
}


public class EnergyManager : MonoBehaviour
{

    [HideInInspector] public bool debug = false;
    public string findThisMeter;
    [Header("----DEFENSE")]
    bool alive = true; 
    public float defaultMaxEnergy = 20f;
    float _energy = 20f;
    public float energy
    {
        get { return _energy; }

        set
        {
            {// implementar en MOD asi nunca tiene valores locos
                if (value > 0)
                {
                    if (value > maxEnergy)
                        _energy = maxEnergy;
                    else
                        _energy = value;
                }
                else
                    _energy = 0;
            }
            energyFeel = 1f - ((maxEnergy - _energy) / maxEnergy);
        }
    }
    public float maxEnergy = 20f;
    [ReadOnly2] public float energyFeel;
    public bool fullEnergyOnEnable = true;
    [Range(0.1f, 10)] public float increaseEnergyDelay = 10f;
    [Range(0, 10)] public float increaseEnergyAdd = 0f;
    public float sameColliderSequentialHitDelay = 0.1f; 
    public bool invincibleOnEnable = false;
    public bool invincibleOnHit;
    [Range(0, 6)] public float invincibleTime;
    [System.NonSerialized] [ReadOnly2] public bool invincible = false;
    public bool LP_invincible;
    [Range(0, 6)] public float LP_invincibleTime;


    public enum ColliderType { Trigger, Solid }

    bool ShowPower() { return exploManager == null; }
    [Header("----ATTACK")]
    [Range(0, 1)] public float distToCenterPowerFactor = 1f;
    //[Inspect("ShowPower")]
    [Range(0, 100)] public float power = 1f; 
    public float pushPower = 1f; 
    public enum VanishMode { Alpha_Decrease, Instant_DisableRenderers, Instant_DisableRendererParent }
    [Header("----DEATH")]
    public VanishMode vanishMode;
    bool AlphaDecrease() { return vanishMode == VanishMode.Alpha_Decrease; }
    bool DisableParent() { return vanishMode == VanishMode.Instant_DisableRendererParent; }
    public enum OnDyingOrOnDead { Dying, Dead, Destroy };
    public OnDyingOrOnDead dropThingsOn = OnDyingOrOnDead.Dying;
    //[Inspect("GotCC")]
    public OnDyingOrOnDead vanishStarts = OnDyingOrOnDead.Dying;
    public float vanishDelay = 0;
    //[Inspect("AlphaDecrease")] 
    public float alphaDecreaseRate = 0.1f;
    //[Inspect("DisableParent")]
    public GameObject rendererParent;
    //[Inspect("GotCC")] 
    public OnDyingOrOnDead explosionStarts = OnDyingOrOnDead.Dying;

    public OnDeadAction whatToDoOnDead = OnDeadAction.ReturnToStoreOrDestroy;
    public enum DestructionDelay { Immediate, AfterExplosion, AfterHurtSound, AfterHurtSoundAndExplosion }
    //[Inspect("NoCC")] 
    public DestructionDelay destructionDelay = DestructionDelay.AfterHurtSoundAndExplosion;
    public GameObject disableOnDeath;
    public GameObject enableOnDeath;
    public AudioClipArray audioOnDead;
    [Header("----BLEED CONSTANT")]
    [Range(0, 1)] public float constantBleedThreshold;
    [Range(0, 300)] public float constantBleedMaxAmount;
    public ParticleSystem constantBleed;
    ParticleSystem.MainModule constantBleedMainModule;
    [ReadOnly2] public float bleedFactor;

    bool GotCC() { return cC; }
    bool NoCC() { return !cC; }
    [HideInInspector] public float lastDamagePower;

    [Header("----ON HURT")]
    public AudioClipArray audioOnHurt;
    public bool showDamage;
    public Color damageColor;
    public float timeDamageFloating = 1;
    ItemStore onHurtTextStore;
    [SerializeField] public ExplosionManager exploManager;
    [SerializeField] public Bullet bullet;
    [SerializeField] Renderer[] allRenderers;
    [SerializeField] AudioManager aM;
    [SerializeField] Rigidbody2D rb2D;
    [SerializeField] Player playerSC;
    [SerializeField] public ArcadeText energyMeter;
    [SerializeField] public CC cC;
    [SerializeField] AnimationManager anim;
    [SerializeField] SWizBaseSprite spr;
    [SerializeField] Item item;
    [SerializeField] Collider2D[] allColliders;
    public bool grabExploInvokersManually = false;
    public ExplosionInvoker[] exploInvoker;
    [SerializeField] PickUpDropper[] pickupDroppers;
    [SerializeField] LPBody lpBody;
    [SerializeField] public Vector2 velocityBackup; 
    [SerializeField] public bool takingDamageThisFrame;
    List<LastHurtingThings> lastHurtingCollisions;
    public delegate void OnHurtDelegate(FakeCollision2D fakeC, float hitSide);
    public OnHurtDelegate onHurtDelegate;
    public delegate void GetMeBackToStoreDelegate();
    public GetMeBackToStoreDelegate getMeBackToStoreDelegate;
    public delegate void OnDeath();
    public OnDeath onDeath;

    public float forceHitSide;
    float _hitSide;
}
