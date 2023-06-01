using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

using UnityEngine.Profiling;


public enum VisionType {Vision, Punchable, Other}
public enum VisibleObjectType { Friend = 0, Enemy = 1, Home = 2, Bullet = 3, Pickup = 4, Switch = 5, DEPRECATED_LastTimeSeenMark = 6, Commander = 7, VehicleBed = 8, ladder = 50 , HomeMark = 100, FriendMark = 110, EnemyMark = 120, PickUpMark = 130, Null = 100000};

[System.Serializable]
public class VisibleObject {
	public Collider2D col;
	public ColliderSettings colliderSettings; 
	public int priority;
	public bool visible;
	public float dist;

	public override string ToString()
	{
		if (col)
			return col.name;
		else return "missing Collider2D";
	}

	public VisibleObject(){
		col = null;
		priority = 0;
		visible = false;
		dist = 0;
	}
	public void Clean(){
		col = null;
		priority = 0;
		visible = false;
		dist = 0;
	}
	public void CopyFrom(VisibleObject o){
		col = o.col;
		priority = o.priority;
		visible = o.visible;
		dist = o.dist;
	}
	public static implicit operator bool(VisibleObject me) // para poder hacer if(class) en lugar de if(class != null) NULLABLE nullable
	{
		return me != null;
	}
}

[System.Serializable]
public class ColliderVisionMode
{
	public float delay;
	public Vector2 offset;
	public Vector2 size;
	public float radius;
}

public enum NeededLineCast { Simple, NoNeed, OnlyIndestructibleBlocksMe, BlockedByIndestructibleAndThickDestructible};

[System.Serializable]
public class VisibleObjectList{
	public VisibleObjectType type;
	public bool isSecondaryPriority;
	bool ShowPriority() { return isSecondaryPriority & IsPlaying();}
	[ReadOnly2Attribute] public int priority;
    public NeededLineCast detectionLineCastType = NeededLineCast.NoNeed;
    public NeededLineCast fireLineCastType = NeededLineCast.OnlyIndestructibleBlocksMe;
    bool ShowDestructibleThickness() { return detectionLineCastType == NeededLineCast.BlockedByIndestructibleAndThickDestructible | fireLineCastType == NeededLineCast.BlockedByIndestructibleAndThickDestructible; }
    public float maxDestructibleThickness = 25;
    public float lineCastNeedDist = 8;
    [HideInInspector] public float lineCasetSqrMagnitude;
    public bool getColliderSettings;
    public Collider2D uniqueCollider;
    public string[] tags; // tags que identifican los objetos de esta clase
    public float ignoreEnergyFeelThreshold = 1; // si tenemos mas energia que esto lo ignoraremos
    public float considerOthersDistance = 0;
    public float closerUpdateMinTime = 1;
    [HideInInspector][ReadOnly2Attribute] [NonSerialized] public bool forceCloserColliderUpdate;
	[HideInInspector][ReadOnly2Attribute] [NonSerialized] public VisibleObject closer; // indice al mas cercano
	[HideInInspector][ReadOnly2Attribute] [NonSerialized] public VisibleObject closerVisible; // indice al mas cercano
	[HideInInspector][ReadOnly2Attribute] [NonSerialized] public bool hasCloserVisible; // si existe
	[HideInInspector][ReadOnly2Attribute] [NonSerialized] public bool hadCloserVisible; // si habia uno antes del ultimo objeto detectado/perdido
    [HideInInspector][ReadOnly2Attribute] [NonSerialized] public List<VisibleObject> objects = new List<VisibleObject>();
    [HideInInspector][ReadOnly2Attribute] [NonSerialized] public float closerUpdateTime;

	[HideInInspector] public int targetSettingsIndex; //no puedo referenciar la classe por que me da error de composition cycle ya que targetsettings referncia esta clase tambien
	public delegate void VisionDelegate(VisibleObjectList list,  Collider2D col);
	[HideInInspector] public VisionDelegate onDetected;
	[HideInInspector] public VisionDelegate onSomethingIsGone; // TODO no tiene en cuenta si necesita linecast o no! si lo necesita y sigue detectado aunque sin linea visual no llama
	[HideInInspector] public VisionDelegate onEverythingIsGone; // en este caso el collider que se envia es el ultimo collider que se ha perdido de la lista, no tiene aplicacion practica aun pero puede ser util

	bool NeedLineCastOnDetection(){return detectionLineCastType != NeededLineCast.NoNeed;}
    public override string ToString()
    {
        return type.ToString();
    }
	public void ClearObjects(){
		objects.Clear();
		closer.Clean();
		closerVisible.Clean();
	}
	public bool SomethingDetected(){
        if (detectionLineCastType != NeededLineCast.NoNeed)
		{
			if (closerVisible)
				return true;
			else
				return false;
		}
		else
		{
			if (closer)
				return true;
			else
				return false;
		}
	}
	public static implicit operator bool(VisibleObjectList me)
	{
		return me != null;
	}
    bool NoUniqueCollider(){ return uniqueCollider == null; }
    bool NoTags(){ return tags.Length == 0; }
    bool IsPlaying(){return Application.isPlaying;}

    public void CopyFrom(VisibleObjectList o) // ojo no restaura los tags
    {
	    type = o.type;
	    detectionLineCastType = o.detectionLineCastType;
	    fireLineCastType = o.fireLineCastType;
	    lineCastNeedDist = o.lineCastNeedDist;
	    ignoreEnergyFeelThreshold = o.ignoreEnergyFeelThreshold;
	    considerOthersDistance = o.considerOthersDistance;
	    closerUpdateMinTime = o.closerUpdateMinTime;
    }
}

public class Vision : MonoBehaviour {

    //public static KuchoPriorityArray priorityArray = new KuchoPriorityArray(); // AL SER ESTATICO NO SE DEBE SERIALIZAR
	public bool debug=false;
	public bool debugAllEvents = false;
	public VisionType type = VisionType.Vision;
	public float updateRate = 0.3f;
	public bool mindPriorities = false;
//	public string[] followTags; 
//	public string enemyBulletTag = "";
//	public string homeTag = ""; // creado para el movimiento de los fantasmas en the template
//	public bool needLineCast = false; // ahora lo tienen las listas
	public NeededLineCast patrolPointLineCastType = NeededLineCast.OnlyIndestructibleBlocksMe;
	public bool needLight = false;
	[Range (0,0.5f)] public float visibilityThreshold = 0.5f;
	[Range (0,1)] [ReadOnly2Attribute] public float visibility = 0f;
	public WorldLightPixel lightPixel;
	public List<VisibleObjectList> detected = new List<VisibleObjectList>();
	
	public bool useTwoModes;
	public ColliderVisionMode normalMode;
	public ColliderVisionMode attackMode;
	
    public VisibleObjectList friend;
    public VisibleObjectList commander;
	public VisibleObjectList enemy;
	public VisibleObjectList home;
	public VisibleObjectList patrolPoint;
	public VisibleObjectList pickup;
    public VisibleObjectList bullet;
    public VisibleObjectList lastTimeSeen;
    public VisibleObjectList friendMark;
    public VisibleObjectList enemyMark;
    public VisibleObjectList homeMark;
    public VisibleObjectList ladder;


	public GameObject mainGO;
	public Collider2D myCol;
	public float myColRadius;
	public BoxCollider2D myBoxCol;
	private int maskForLinecasts;
	private int maskForBlockingOverlap;

	public CircleCollider2D myCircleCol;
	public Collider2D groundDetectorCol;

    [ReadOnly2Attribute] public Collider2D lastColliderIHurt;
    [ReadOnly2Attribute] public Vector2 lastCollideriHurtHitPos;
    [ReadOnly2Attribute] public Vector2 targetPosOnLastHit;

    public Transform myTrans;
    public CC cC;
    public AI aI;
    public AI_Target target;

    public VisibleObjectList enemyBackup;// enemy se puede modificar para cmabiar el comportamiento en algunas situaciones (no poder encontrar ruta hasta el enemigo y asignar patrol point)
    public bool restoreEnemyPending;// cuando modificas enemy marcas esto para saber que hay que recuperar los settings originales en algun momento



    int frameLastIteration;
    
	[System.NonSerialized]  bool unlockMode; // activado externamente hará que solo veamos como objetivo el terreno destructible
	
	Collider2D lastTargetWeAttack;
	
    public VisibleObjectList GetVisibleObjectListByType(VisibleObjectType type){
	    for (int i = 0; i < detected.Count; i++)
	    {
		    if (detected[i].type == type)
			    return detected[i];
	    }
	    return null;
    }

}
