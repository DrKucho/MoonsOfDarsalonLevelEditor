using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;



public enum FollowSide { Front, Back }
//public enum TargetType {Enemy, Friend, Home}
public enum BehaviourOnView { Follow, RunAway, Freeze, Switch, PlaceHomeMark, CleanTargetsImmediatelly, CleanTargetsOnShortDistance };
public enum MarkBehaviour { Follow, DontFollow }

public class Target
{

}
[System.Serializable]
public class TargetSettings
{

    public VisibleObjectType type = VisibleObjectType.Enemy;
    public Vector2 offset; // offset principal con respecto de lo que se este siguiendo, transform o collider, a aplicar a todos los calculos antes que nada
    public bool behabiourOnDetection = true;
    public BehaviourOnView behabiour = BehaviourOnView.Follow;
    public float markAutoFollowTime;
    public bool markAutoFollowX;
    public bool markAutoFollowY;
    public bool markFollowEndsOnStepGround;
    bool IsMark() { return type == VisibleObjectType.HomeMark || type == VisibleObjectType.FriendMark || type == VisibleObjectType.EnemyMark || type == VisibleObjectType.PickUpMark; }
    bool IsFollower() { return behabiour == BehaviourOnView.Follow; }
    bool IsFollowerOrRunerWay() { return behabiour == BehaviourOnView.Follow || behabiour == BehaviourOnView.RunAway; }
    bool IsCleanTargetsOnShortDistance() { return behabiour == BehaviourOnView.CleanTargetsOnShortDistance; }
     public float cleanTargetsDistThreshold = 30;
    [Header("Offset")]
     public FollowSide followSide = FollowSide.Back;
    public Vector2 followOffset;
    /// <summary> Añade el vision collider para definir cuanto nos acercamos junto con colliderFactorX e Y </summary>
    public Collider2D collider;
    bool GotCollider() { return collider; }
     [Range(0f, 1f)] public float colliderFactorX = 0.4f;
    [Range(0f, 1f)] public float colliderFactorY = 0.2f;
     [Range(0f, 1f)] public float colliderFactorY_Down = 0.2f;
     public bool yDistOffsetHasSign = false; // no se puede inicializar a true 
    public Vector2 aimOffset;
    bool YDistOffsetHasSign() { return yDistOffsetHasSign; }
    bool YDistOffsetHasSignAndGotCollider() { return yDistOffsetHasSign & GotCollider(); }
    [Header("Margin")]
     public Vector2 followMargin;
     [Range(0f, 2f)] public float yDistIncreasesXMarginFactor = 0f;
    bool showMaxFollowMargin() { return yDistIncreasesXMarginFactor != 0 && IsFollower();  }
     public Vector2 maxFollowMargin;
     public bool yDistMarginHasSign = false;// no se puede inicializar a true
    [Header("Timing")]
    public RandomFloat considerTime;
    public float patienceMult = 0f;
    public float allowNewTargetTime = 0.5f;
    [Tooltip("Incremento de la velocidad standard, esto se sumara a lo que se calcule con distSpeedFactor y se vuelca en Legs.AiFpsInc")]
     [Range(-10, 10)] public float speedInc = 1f;
    [Tooltip("cuanto mas lejos mas velocidad, a ser sumado a speedInc y se vuelca en Legs.AiFpsInc")]
     [Range(-10f, 10f)] public float distSpeedFactor = 0.5f;
    [Tooltip("el stress se multiplica al final del calculo de fps (en Legs) pero nunca se supera las maxFps (de Legs)")]
     [Range(1, 2)] public float stressSpeedMult = 1f;


    public bool checkNearByOfMyKind = true;
    public bool lookAtItOnFire = true;
    public bool saySomething = true;
    public VisibleObjectList detectedList;

    public override string ToString()
    {
        return type.ToString();
    }
    public TargetSettings(VisibleObjectType _type)
    {
        type = _type;
    }
    public static implicit operator bool(TargetSettings me) // para poder hacer if(class) en lugar de if(class != null) NULLABLE nullable
    {
        return me != null;
    }
}

public class AI_Target : MonoBehaviour
{
}