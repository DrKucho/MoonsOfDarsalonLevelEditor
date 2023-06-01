using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum ThingType { Ground_Level_Obstacle, Ladder, Pipe, BaseDoors, Area }

[ExecuteInEditMode]
public class ColliderSettings : MonoBehaviour
{


    [System.Serializable]
    public class Actions
    {
        public bool enabled;

        bool Show2()
        {
            return enabled;
        }

        bool ShowHingeAction()
        {
            return enabled & hinge;
        }

         public GameObject activate;
         public MonoBehaviour enable;
         public bool disableCC_Renderers;
         public VehicleInput vehicleInput;
         public HingeManager3D hinge;
         public MovingPlatform movingPlatform;
         public Door.Action hingeAction;
         [ReadOnly2Attribute] public bool actionsPerformed;

        public void Init()
        {
            if (activate)
                activate.SetActive(false);
            if (enable)
                enable.enabled = false;
        }


    }

    [System.Serializable]
    public class MediumPointZ
    {
        public Transform side1;
        public Transform side2;
        public bool dinamicLerp = false;

        bool NotDinamicLerp()
        {
            return !dinamicLerp;
        }

        [Range(0, 1)]
        public float lerp = 0.5f;

        public float add;
        [ReadOnly2Attribute] public float lastZ;

        
        public float Calculate() // TODO en lugar de calcular el punto medio puedo darle un valor de entrada, la posicion del CC que esta entre los dos puntos, y que me haga un lerp para sacarme un a z distinta dependiendo de donde este el CC , ojo, esto podria joder la good guys ship
        {
            // saco punto mas cercano a la pantalla, al plano de accion
            if (!side2 && side1) // solo hay 1
                return side1.position.z;
            else if (!side1 && side2)
                return side2.position.z;
            else if (!side1 && !side2)
                return 0;

            float z1 = side1.position.z;
            float z2 = side2.position.z;
            float minZ = Mathf.Min(z1, z2);

            float medium = (z1 + z2) * 0.5f;

            // creo que esto tiene sentido cuando piensas en la nave que rota hacia y lo que esa en frente puede ser lo que esta luego detras, , siempre interesa estar en un punto intermedio entre el medio y lo que esta delante MinZ
            // por eso lerp 0 sera el medio y lerp 1 el total
            lastZ = Mathf.Lerp(medium, minZ, lerp); // asi era antes y no se por que , un punto medio entre el centro y el mas cercano al plano 0 ...? por que?

            lastZ += add;

            return lastZ;
        }

        public float GetLerpedZ(Collider2D[] cols, Vector2 pos)
        {
            if (side1 && side2)
            {
                float maxX = float.MinValue;
                Collider2D winnerCol = null;
                if (cols.Length > 0)
                {
                    foreach (Collider2D c in cols)
                    {
                        if (c.bounds.size.x > maxX)
                        {
                            winnerCol = c;
                            maxX = c.bounds.size.x;
                        }
                    }

                    float delta = Mathf.InverseLerp(winnerCol.bounds.min.x, winnerCol.bounds.max.x, pos.x); // delta = 0-1

                    float z1 = side1.position.z;
                    float z2 = side2.position.z;

                    float x1 = side1.position.x;
                    float x2 = side2.position.x;

                    float result;
                    if (x1 < x2)
                        result = Mathf.Lerp(z1, z2, delta);
                    else
                        result = Mathf.Lerp(z2, z1, delta);

                    return result;
                }
                else
                {
                    return add;
                }
            }
            else
            {
                if (cols.Length > 0)
                {
                    return cols[0].transform.position.z + add;
                }
                else
                {
                    return add;
                }
            }
        }

        public bool Enabled()
        {
            if (!side1 && !side2)
                return false;
            return true;
        }
    }

    public bool debug;

    public enum GetRightFromLocalScaleX
    {
        Direct,
        Invert,
        Dont
    }

    public GetRightFromLocalScaleX getRightFromLocalScaleX = GetRightFromLocalScaleX.Direct;
    public float right = 0;
    public ThingType type = ThingType.Ground_Level_Obstacle;
    public bool disableByRotation = false;
     public MinMax validRotation;
    [ReadOnly2Attribute] public float myRotation;

    bool IsLadder()
    {
        return type == ThingType.Ladder;
    }

    bool IsGround()
    {
        return type == ThingType.Ground_Level_Obstacle;
    }

    bool IsGroundOrLadderOrPipe()
    {
        return type == ThingType.Ground_Level_Obstacle || type == ThingType.Ladder || type == ThingType.Pipe;
    }

    bool IsLadderOrPipe()
    {
        return type == ThingType.Ladder || type == ThingType.Pipe;
    }

    public float rungSeparation = 5;

    [Header(" for pickups ")]
    public float pushX = 0f;

    //public HingeManager3D hingeManager3D;
    //bool GotHinge3D() { return hingeManager3D;}
    [Range(0, 1)] public float updateDelay = 0.3f;
    public KuchoTile tile;

    [Range(-1, 1)] public float lookTo = 0;
    public Transform snappedTransForm; // si hay un sprite o padre de sprites que se snapea en este suelo o escalera, referencioalo aqui para que cC.ladderSnapX e Y funcionen bien
    [Range(0, 0.1f)] public float rotationPushFactor = 0.016f;
    public Vector2 force = Constants.zero2;

    public CC cC;

    public enum LightModifierMode
    {
        Dont,
        LeftToRight,
        RightToLeft
    }

    [Header(" for CCs ")] public LightModifierMode lightModifierMode = LightModifierMode.Dont;
    [Range(0, 1)] public float lightIntensityMult = 1;

    bool ModifyLight()
    {
        return lightModifierMode != LightModifierMode.Dont;
    }

     public bool playerCanStep = true;
     public bool goodGuysCanStep = true;
     public bool badGuysCanStep = true;
     public float forceToCenterOnStepOut = 100;
     public float shouldWalk = -1f;
     public bool forceStandUpFire = false;
     public Actions onStep;
     public Actions onDuck;
     public HingeManager3D vehicleDoor;
     public CargoContainerManager cargoContainer;
     public bool isVehicleBed;
     public bool avoidFalling;
     public bool useGroundJoint;
     public bool groundJointIsFakeTransformFor3Drotations;
     public Renderer biggest3DRenderer;

    [Header(" for CCs On Jump (!grounded)")]
    public bool ignoreCC_RenderZ_Offset = false;

    public float cC_RenderZ_offsetMult = 1;

    
    public float armsAndJumpDownOffsetZ = 0;

     public bool setStuffOnJump = false;
     public float onJumpOffsetZ = 0;
     public float onJumpZLockTime = 0;
     public float onJumpOffsetZdelay = 0;

     [Tooltip("Se Restaura OnJump y/o OnLeaveGround")]
    public bool setShaderAdd;

    bool SetShaderAdd()
    {
        return setShaderAdd;
    }

     [Tooltip("Se Restaura OnJump y/o OnLeaveGround")]
    public float shaderAdd = 0;

     public Transform ildeMagnet;
     public bool fakeMagnet = false;
     public bool walkLeftPossible = true;
     public bool walkRightPossible = true;

    bool ShowOnJumpThings()
    {
        return IsGround() & setStuffOnJump;
    }

    [Header("-----------")] public EnableOrDisable renderers = EnableOrDisable.Nothing;

    public enum GetZMethod
    {
        Collider,
        MediumPoint,
        Transform
    }

    public GetZMethod getZ_Method = GetZMethod.Collider;
    

     public Transform getMyZ;
     public float offsetZ = -0.00001f;
     public MediumPointZ mediumPointZ;
    public bool billBoardMode;
     public Transform billBoardAxis;
     public Transform billBoardCopyRot;
    public Vector3 billBoarRotOffset;

    public ForceAI jumpOnlyFromThisZ;

     public bool drawGadgetsAllowed = true;
     public bool attackAllowed = true;

    
    public bool jumpStraightAllowed = true;

     public bool jumpLeftAllowed = true;
     public bool jumpRightAllowed = true;

    
    public bool jumpDownAllowed; // algunos suelos obstaculo/ground pueden permitir que se salte abajo con esto

     public bool switchToFlyerAllowed = true;
     public bool pushAwayOtherCcAllowed = false;



    [Header(" for AIs ")] [ReadOnly2Attribute] public CC myCc;
     public bool modFollowMargin;
     public Vector2 followMarginAdd;
     public Vector2 maxFollowMarginAdd;
     public bool talkAllowed = true;
     public bool AiAvoidsJumpHere = false;
     public bool AiAvoidsJumpFromHereToHere = false;
     public bool AiAvoidsJumpOutIfFollowingMe = false;
     public bool AiAvoidsJumpOutIfDifferentCcRight = false;
     public float AiIgnoresCcRightLimitationOnSmallFlipRight = 0.5f;
     public ForceAI neededZ;
     public bool sideMatters = false;
     public float sideThatMatters = 1;
     public bool jumpLevelDownAllowed = true;
     public AudioClipArray soundOnStep;
     public DamageMaker damageMakerToIgnore;

     public Collider2D ladderUpLevel;
     public Collider2D[] ladderUpLeftLevels;
     public Collider2D[] ladderUpRightLevels;
     public Collider2D ladderDownlevel;
     public Collider2D[] ladderDownLeftlevels;
     public Collider2D[] ladderDownRightlevels;
     public ColliderSettings connectingLadderUp;
     public ColliderSettings connectingLadderDown;

    
     public SWizSprite mainLadderSprite;

     public LadderRung3D rung0;
     public DragMeHandle stretchHandle;



    public AudioManager aM;
    public float nearbyOfMyKindIldeTimeMult = 1;

    public enum NearByOfMyKindDirectionChange
    {
        Change,
        DoNotChange,
        TargetBased
    };

    public NearByOfMyKindDirectionChange nearbyOfMyKinfDirectionChange = NearByOfMyKindDirectionChange.TargetBased;
    public bool doOnStepTask;
    public AI_Task onStepTask = AI_Task.None;
    public bool doOnLeaveTask;
    public AI_Task onLeaveTask = AI_Task.None;

    public Collider2D[] cols;
    [HideInInspector] public Collider2D col; // el codigo anterior a los colliderSettings Multicollider se refiere mucho a este unico colloider, asi que para mantener compatibilidad lo asigno al primero de la array, ademas esto puede prevenir el cuelga masivo de noviembre de 2020
    public DualBodyManager dualBodyManager;
    public Rigidbody2D rb2D;

    public delegate void On_StepGround(CC cC);

    public event On_StepGround onStepGround;

    public delegate void On_StepOnTile(ColliderSettings groundOrLadder, CC cC, KuchoTile tile);

    public event On_StepOnTile onStepOnTile;

    public delegate void On_LeaveGround(CC cC);

    public event On_LeaveGround onLeaveGround;

    public delegate void On_StepOnTileEnd(ColliderSettings groundOrLadder, CC cC, KuchoTile tile);

    public event On_StepOnTileEnd onStepOnTileEnd;

    private static Collider2D lastColliderWithSettings;
    static ColliderSettings lastColliderSettings;




#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
        {
            if (billBoardMode)
                DoBillBoard();
            if (IsLadder() && stretchHandle && stretchHandle.transformHasChanged)
            {
                float ladderLowStarrtPos;
                int minRungs;

                if (mainLadderSprite)
                {
                    ladderLowStarrtPos = mainLadderSprite.transform.localPosition.y;
                    minRungs = 6;
                }
                else
                {
                    ladderLowStarrtPos = rung0.transform.localPosition.y;
                    minRungs = 0;
                }

                float height = stretchHandle.transform.localPosition.y - ladderLowStarrtPos;
                int minHeight = (int) (minRungs * rungSeparation);
                if (height < minHeight)
                    TransformHelper.SetPosY(stretchHandle.transform, ladderLowStarrtPos + minHeight);
                float heightToFill = height - minHeight;
                int missingRungs = (int) (heightToFill / rungSeparation);
                int rungCount = minRungs;
                Vector3 localPosToBuild = new Vector3(transform.localPosition.x, ladderLowStarrtPos + minHeight, transform.localPosition.z);
                int count = 0;

                if (mainLadderSprite)
                {
                    var ladderSprites = GetComponentsInChildren<SWizSprite>();
                    foreach (SWizSprite s in ladderSprites)
                    {
                        if (s != mainLadderSprite)
                            DestroyImmediate(s.gameObject);
                    }

                    while (missingRungs > 0 && count < 30)
                    {
                        Vector3 worldPosToBuild = transform.position + localPosToBuild;
                        if (missingRungs >= minRungs)
                        {
                            var newGO = Instantiate(mainLadderSprite.gameObject, worldPosToBuild, transform.rotation, transform);
                            newGO.transform.localPosition = localPosToBuild;
                            newGO.name = "Ladder 6 rungs";
                            localPosToBuild.y += minHeight;
                            missingRungs -= (int) minRungs;
                            rungCount += minRungs;
                        }
                        else if (missingRungs >= 3)
                        {
                            var newGO = Instantiate(mainLadderSprite.gameObject, worldPosToBuild, transform.rotation, transform);
                            newGO.transform.localPosition = localPosToBuild;
                            newGO.name = "Ladder 3 rungs";
                            var newSpr = newGO.GetComponent<SWizSprite>();
                            var newSpriteID = newSpr.GetSpriteIdByName("Ladder 3");
                            if (newSpriteID >= 0)
                            {
                                newSpr.SetSprite(newSpriteID);
                                localPosToBuild.y += rungSeparation * 3;
                                missingRungs -= 3;
                                rungCount += 3;
                            }
                        }
                        else if (missingRungs >= 1)
                        {
                            var newGO = Instantiate(mainLadderSprite.gameObject, worldPosToBuild, transform.rotation, transform);
                            newGO.transform.localPosition = localPosToBuild;
                            newGO.name = "Ladder 1 rung";
                            var newSpr = newGO.GetComponent<SWizSprite>();
                            var newSpriteID = newSpr.GetSpriteIdByName("Ladder 1");
                            if (newSpriteID >= 0)
                            {
                                newSpr.SetSprite(newSpriteID);
                                localPosToBuild.y += rungSeparation;
                                missingRungs -= 1;
                                rungCount += 1;
                            }
                        }
                    }
                }
                else // escalera 3D
                {
                    var rungs = GetComponentsInChildren<LadderRung3D>();

                    foreach (LadderRung3D rung in rungs)
                    {
                        if (rung != rung0)
                            DestroyImmediate(rung.gameObject);
                    }

                    GameObject newRung = null;
                    for (int i = 1; i <= missingRungs; i++)
                    {
                        Vector3 worldPosToBuild = transform.position + localPosToBuild;

                        newRung = Instantiate(rung0.gameObject, worldPosToBuild, transform.rotation, transform);
                        newRung.transform.localPosition = localPosToBuild;
                        newRung.name = "Rung " + i;
                        localPosToBuild.y += rungSeparation;
                        rungCount += 1;
                        #if UNITY_EDITOR
                        UnityEditor.SceneVisibilityManager.instance.DisablePicking(newRung, true);
                        #endif
                    }

                    getZ_Method = GetZMethod.MediumPoint;
                    mediumPointZ.side1 = rung0.transform;
                    if (newRung)
                        mediumPointZ.side2 = newRung.transform;
                }

                count++;
                
                BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
                if (!boxCol)
                    boxCol = gameObject.AddComponent<BoxCollider2D>();
                boxCol.isTrigger = true;
                gameObject.layer = Layers.ladder;
                float sizey = rungCount * rungSeparation;
                boxCol.size = new Vector2(12, sizey);
                boxCol.offset = new Vector2(0, (sizey * 0.5f) - 1);
            }
            /*
            if (IsLadder() && stretchHandle && mainLadderSprite && stretchHandle.transformHasChanged)
            {
                float height = stretchHandle.transform.position.y - mainLadderSprite.transform.position.y;
                int minRungs = 6;
                int minHeight = (int) (minRungs * rungSeparation);
                if (height < minHeight)
                    TransformHelper.SetPosY(stretchHandle.transform, mainLadderSprite.transform.position.y + minHeight);
                float heightToFill = height - minHeight;
                int missingRungs = (int) (heightToFill / rungSeparation);
                int rungCount = minRungs;
                Vector3 posToBuild = new Vector3(transform.position.x, mainLadderSprite.transform.position.y + minHeight, transform.position.z);
                int count = 0;
                var ladderSprites = GetComponentsInChildren<SWizSprite>();
                foreach (SWizSprite s in ladderSprites)
                {
                    if (s != mainLadderSprite)
                        DestroyImmediate(s.gameObject);
                }

                while (missingRungs > 0 && count < 30)
                {
                    if (missingRungs >= minRungs)
                    {
                        var newGO = Instantiate(mainLadderSprite.gameObject, posToBuild, transform.rotation, transform);
                        newGO.name = "Ladder 6 rungs";
                        posToBuild.y += minHeight;
                        missingRungs -= (int) minRungs;
                        rungCount += minRungs;
                    }
                    else if (missingRungs >= 3)
                    {
                        var newGO = Instantiate(mainLadderSprite.gameObject, posToBuild, transform.rotation, transform);
                        newGO.name = "Ladder 3 rungs";
                        var newSpr = newGO.GetComponent<SWizSprite>();
                        var newSpriteID = newSpr.GetSpriteIdByName("Ladder 3");
                        if (newSpriteID >= 0)
                        {
                            newSpr.SetSprite(newSpriteID);
                            posToBuild.y += rungSeparation * 3;
                            missingRungs -= 3;
                            rungCount += 3;
                        }
                    }
                    else if (missingRungs >= 1)
                    {
                        var newGO = Instantiate(mainLadderSprite.gameObject, posToBuild, transform.rotation, transform);
                        newGO.name = "Ladder 1 rung";
                        var newSpr = newGO.GetComponent<SWizSprite>();
                        var newSpriteID = newSpr.GetSpriteIdByName("Ladder 1");
                        if (newSpriteID >= 0)
                        {
                            newSpr.SetSprite(newSpriteID);
                            posToBuild.y += rungSeparation;
                            missingRungs -= 1;
                            rungCount += 1;
                        }
                    }

                    count++;
                }

                BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
                if (!boxCol)
                    boxCol = gameObject.AddComponent<BoxCollider2D>();
                boxCol.isTrigger = true;
                gameObject.layer = Layers.ladder;
                float sizey = rungCount * rungSeparation;
                boxCol.size = new Vector2(12, sizey);
                boxCol.offset = new Vector2(0, (sizey * 0.5f) - 1);
            }
            */
        }
    }
#endif
    void DoBillBoard()
    {
        Vector3 rot = Constants.zero3;
        rot.y = billBoardCopyRot.rotation.eulerAngles.y;
        rot.x += billBoarRotOffset.x;
        rot.y += billBoarRotOffset.y;
        rot.z += billBoarRotOffset.z;
        billBoardAxis.localRotation = Quaternion.Euler(rot);
    }

    public static List<Collider2D> levelObstacleColliders;


    [ReadOnly2Attribute] public Vector2 speed;


    [HideInInspector] public CC lastCC;

    List<CC> attachedWithFakeJoints = new List<CC>();

}