using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CargoContainerManager : MonoBehaviour
{
    [System.Serializable]
    public class CargoSide
    {
        public Transform parent;
        public Transform doorPosition;// para definir la posicion que sera usada por logicas de cercania etc.
        public HingeManager3D[] hinges;
        public ForceAI[] forceAis;
        public Collider2D[] doorColliders;
        public ColliderSettings floor;
        public float side; // el lado bueno, se necesita que el collider suelo tenga este lado y se comprobará si las puertas estan abiertas
        [ReadOnly2Attribute] public bool hasDoors;

    }
    public Vector2 doorDetectionDistance = new Vector2(100, 130);
    public CargoSide side1;
    public CargoSide side2;

    [ReadOnly2Attribute] public CargoSide sideInFront;
    [ReadOnly2Attribute] public CargoSide sideOnBack;

    [ReadOnly2Attribute] public float colSettingsGoodSide = 0;
    public ColliderSettings floorColliderSettings;
    public Collider2D firstFloorCollider;
    public Transform flippingTransform;
    public float massOnAttach = 100;
    public bool copyPosAndRotFromVehicle = true;


    public float copyPosSpeed = 20;
    public float copyPosSnapThreshold = 20;

    [ReadOnly2Attribute] public bool tutosEnabled;
    [ReadOnly2Attribute] public Vector2 localAttachPosition;
    [ReadOnly2Attribute] public Rigidbody2D rb2D;
    public bool attachToVehicleOnEnable = true;
    [ReadOnly2Attribute] public VehicleInput myVehicle;
    [ReadOnly2Attribute] public List<Item> carriedItems;
    [ReadOnly2Attribute] public List<Collider2D> notTriggercolliders;
    [HideInInspector] public AudioManager aM;
    public AudioClipArray audioOnAttach;
    public AudioClipArray audioOnDetach;
    public GameObject[] enableOnAttach;
    public GameObject[] disableOnAttach;

    private float mass;

}

