using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class SimpleDamageReceiver : MonoBehaviour
{
    public float testSpeed = 1000;


    // RECIBE LOS EVENTOS COLISION Y TRIGGER ENTER Y APLICA DAÑO 
    public bool debug = false;

    [SerializeField] CC cC;
    [SerializeField] AudioManager aM;
    public EnergyManager eM;
    public Rigidbody2D myBody; // se asigna en la primera colision
    public Collider2D myCol; // se asigna en la primera colision
    public float myMass; // se asigna en la primera colision
    public Collider2D[] _collider;
    public Collider2D centerCollider;
    public int weGetHurtBy = 99999999; // dependinedo de la variable destroys we Hurt se rellena con la layer del bando contrario y se usa para comparar con la layer del collider encontrado, le pongo 9999999 para que se inicialize con una layer que no existe

    public bool takeDamageFromAnyObject = false;
    public bool deflectBullets;
    bool WeDontDeflectBullets() { return !deflectBullets; }
    public ArmyType takeDamageFrom = ArmyType.All;

    float hitVolume = 0.1f;
    Player playerSC;
    int colliderCharacterLayer;
    Collider2D lastCol;
    int  currentColLayer;
    bool currentColIsGroundOrVision = false;

    public ExplosionInvoker selfCollisionExploInvoker;
    public float bloodSpeedMult = 1;
    public float bloodAmountMult = 1;


    public string noDeflectiveTag;

    float myContactCount;
    float hurtFactor = 0;
    Vector2 contactsNormals; // para ir sumando todos los contactsimpulse hasta que se procesan en update
    float contacsNormalImpulse;
    Vector2 contactPoints;
    Vector2 contactPointAverage;
    Collider2D lastGroundOrObstacleCollider;
    Collider2D lastOwnCollider;


}

