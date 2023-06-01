using UnityEngine;
using UnityEngine.Profiling;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Represents a b2Body object in the liquidfun simulation world</summary>
public class LPBody : LPThing 
{	

	[Tooltip("What kind of body is this. Static = cant move. Dynamic = can move. Kinematic = not affected by gravity or collisons")]
	public LPBodyTypes BodyType = LPBodyTypes.Static;
	[Tooltip("how much linear velocity this body will lose over time")]
	public float LinearDamping = 0.1f;
	[Tooltip("how much angular velocity this body will lose over time")]
	public float AngularDamping = 0.1f;
	[Tooltip("Can this body sleep? Recommended, improves performance")]
	public bool AllowSleep = true;
	[Tooltip("Can this body not rotate?")]
	public bool FixedRotation = false;
    float _angleInDegrees;
	public float angleInDegrees{
        get{ return _angleInDegrees;}
        set{ 
            _angleInDegrees = value;
            _angleInRadians = value * Mathf.Deg2Rad;
            angle_old = value;
        }
    }
    float _angleInRadians;
     public float angleInRadians{
        get{ return _angleInRadians;}
        set{ 
            _angleInRadians = value;
            _angleInDegrees = value * Mathf.Rad2Deg;
        }
    } // se actualiza a mano ojo!
    [SerializeField][HideInInspector] float angle_old;
	[Tooltip("Is this body a bullet? Bullets are simulated more accurately(and will not pass thorugh other objects when they are going fast), but more expensively")]
	public bool IsBullet  = false;
	[Tooltip("Scale of gravity in relation to this object. Eg minus value will make this fall 'upwards'")]
	public float GravityScale = 1f;
	[Tooltip("Should this body be created when the game plays")]	
	public bool SpawnOnPlay = true;

    public float fakeParticleCollisionFixSqArea = 60; // parece que hay un bug no se si mio o de liquidfun, cuando se mezcla agua y acido a veces me llegan colisiones de acido de particulas que estan en la otra punta del mapa, este float define un area cuadrada que se comprueba antes de cada colision para eliminar este bug

    /// <summary>key of this body in the LPManager dictionary allBodies
    /// and its userdata value in the simulation</summary>	
    public int myIndex {get; private set;}
	public bool InitialisedActiveAndNotZeroPtr {
		get
		{
			return false;
		}
		private set 
		{
			initialized = value;
		}
	}
    public bool InitialisedAndNotZeroPtr
    {
        get
        {
	        return false;
        }
    }
    public event System.Action onBodyInitiasedChange;
	public bool lpManUpdatePosAndAngle = true;

	LPManager lpman;

    [Header("--ALL FIXTURES--")] 
    public float allFixDensity = 1;
    [HideInInspector][SerializeField] float allFixDensity_old;
    public float kinematicMass = 0;
    [Header("--info--")]
	[ReadOnly2Attribute] public bool isAwake; // se actualiza a mano ojo!

    [ReadOnly2Attribute] public float mass; // se calcula automaticamente teniendo en cuenta las densidades de las fixtures, algo incomodo , no veo forma de saltarme esto
	[ReadOnly2Attribute] public Vector2 pos; // se actualiza a mano ojo!

    [ReadOnly2Attribute] public Vector2 velocity; // se actualiza a mano o por dualbodymanager ojo!
    [ReadOnly2Attribute] public float angularVelocity; // se actualiza a mano o por dualbodymanager ojo!

	bool sumContactsWaitingToBeProcessed = false;

	public delegate void OnLPParticleFixtureContact(LPParticleSystem partSys, LPSystemFixPartContact contact);
	public event OnLPParticleFixtureContact onParticleContactDelegate;
	[HideInInspector] public bool onParticleContactDelegateIsNotNull;

    [ReadOnly2Attribute] public IntPtr reusableShape; // si alguna fix es reusable se almacenará aqui para poder acceder en el algunos casos de colision (balas) no hay control en el caso de que varias sean reusables asi que se guardará la ultima en hacer reusable
	public List<LPFixture> myFixtures = new List<LPFixture>();
	public IntPtr[] fixPtrs; // extraido del motor de fisicas !

}