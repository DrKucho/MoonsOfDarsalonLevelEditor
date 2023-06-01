using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Serialization;


[ExecuteInEditMode]
public class ForceAI : MonoBehaviour {
    [System.Serializable]
    public class StretachbleWidthBoxCollider2D {
        public bool enabled;
        public bool compensateOffset;
        public BoxCollider2D col;
        public CapsuleCollider2D capsCol;
        public Transform flippingTransform;
        public float disableRot;
        public float maxRot;
        public float minRot;
        public float maxWidth;
        public float minWidth;
        float previousRot;
        
    }
    public enum EnterExitStay {Enter, Exit, Stay};
    public enum InputRightModdifier {GoLeft, GoRight, Stop, DontChange, GetOut}

    public enum VisibleOnScreenType { DontCare, VisibleOnScreen, NotVisibleOnScreen }

    [Header("CONDITIONS-------------------")]
    public VisibleOnScreenType visibleOnScreenType = VisibleOnScreenType.DontCare;
    public bool isShipZModifier;
	public string[] targetTags;
	[Tooltip("Solo Forzará Accion Si AI Esta Haciendo Uno De Estos Comandos")]
	public AI_Task[] task;

    public bool mindGrounded;
    bool MindGrounded() { return mindGrounded; }
    public bool groundedMustBe;
    [Tooltip("Solo Forzará Accion Si cC.ground Layer es")]
    public bool mindGroundLayerMustBe;
    bool MindGroundLayerMustBe() { return mindGroundLayerMustBe; }
    public LayerType groundLayerMustBe;
    [Tooltip("Solo Forzará Accion Si cC.ground Layer NO es")]
    public bool mindGroundLayerMustNotBe;
    bool MindGroundLayerMustNotBe() { return mindGroundLayerMustNotBe; }
    public LayerType groundLayerMustNotBe;
    [Tooltip("Solo Forzará Accion Si cC.ground es este collider")]
    public Collider2D groundMustBe;
    [Tooltip("Solo Forzará Accion Si cC.ground NO es este collider")]
    public Collider2D groundMustNotBe;
    
    public bool mindBed;
    bool MindBed() { return mindBed; }
    public bool bedMustBe;
    
    public bool mindTarget;

    public enum MindTargetMode { Undefined = 0, DontCare = 10, MustBe  = 20, MustNotBe = 30};

    public MindTargetMode mindTargetMode = MindTargetMode.DontCare;
    public VisibleObjectType targetMustBe;
     public VisibleObjectType targetMustNotBe;
    bool MindTargetMustBe() { return mindTargetMode == MindTargetMode.MustBe; }
    bool MindTargetMustNotBe() { return mindTargetMode == MindTargetMode.MustNotBe; }

    public bool mindTakingDamage;
    bool MindTakingDamage() { return mindTakingDamage; }
    public bool takingDamageMustBe;
    public bool mindRight;
    bool MindRight() { return mindRight; }
    public float rightMustBe;

    [Header("IF CONDITIONS DO-------------------")]
    public AI_Task taskToDo = AI_Task.None;
	[FormerlySerializedAs("inputRight")] public InputRightModdifier horizontalInput = InputRightModdifier.DontChange;
    bool ShowJumpThings() { return !lockJump & jump > 0; }
    bool ShowJump() { return !lockJump; }
    public bool modFollowMargin;
    public Vector2 followMarginAdd;
    public Vector2 maxFollowMarginAdd;
    public bool lockJump = false;
    public bool forceAllowJump;
     public float jump = 0f;
    [ReadOnly2Attribute] public bool _jump;
    public Vector2 jumpFactor = Vector2.one;
	public float ildeTime = 1f;
    public bool ignoreColliderForIldeTime;
    public bool clearAllTargets = false;
    public bool removeAI_Task = true;
    public bool noAmmo;
    public float flyerForceMultiplier = 1;
    bool ClearAllTargetsIsFalse(){return !clearAllTargets;}
    public ColliderSettings.MediumPointZ mediumPointZ; 
    //public float zMult = 0.5f;
    //public float zOffset;
    public Rigidbody2D groundAvoidsSetZ;

    public StretachbleWidthBoxCollider2D resizableCollider;

    public Collider2D myCollider;

	void OnValidate(){
		_jump = System.Convert.ToBoolean(jump);
        if (lockJump)
            _jump = false;
        
		if (_jump)
			ildeTime = 0; // si tiene que hacer saltar a alguien no puede esperar y hacerlo saltar despues... 

        FixMindTarget();

    }

    void FixMindTarget() // se pùede borrar una vez me asegure que todo esta updateado al nuevo enum
    {
        if (mindTargetMode == MindTargetMode.Undefined)
        {
            if (mindTarget)
                mindTargetMode = MindTargetMode.MustBe;
            else
                mindTargetMode = MindTargetMode.DontCare;
        }
    }
    public void InitialiseInEditor(){
        if (!myCollider)
            myCollider = GetComponentInChildren<Collider2D>();
        if (myCollider)
            myCollider.isTrigger = true;
    }

}
