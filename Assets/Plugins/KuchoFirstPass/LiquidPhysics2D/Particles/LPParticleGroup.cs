using UnityEngine;
using UnityEngine.Profiling;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
	

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Represents particle group in the liquidfun simulation</summary>
public class LPParticleGroup : LPCorporeal
{
    public bool Initialised = false;
	public bool singleParticle = false;
	[Tooltip("Should this particle group be created when the game play?")]
	public bool spawnOnEnable = false; // KUCHO HACK
	public string findThisParticleSystem = "";
	bool ShowFindThisParticleSystem(){ return !lpParticleSystem; }
	public LPParticleSystem lpParticleSystem; // KUCHO HACK
	[Tooltip("This particle group will be created in the particle system with this index")] 
	[HideInInspector] public int ParticleSystemIndex = 0; // KUCHO HACK linea comentada
	//LPManager lpMan; //KUCHO HACK

//    [Tooltip("The partcle physics material for this particle group. Drag a LPparticleMaterial scriptable object in here")]
	//    public LPParticleMaterial ParticlesMaterial; // KUCHO HACH (linea comentada) ahora el partmaterial esta en kuchoDrawer, un partmatyerial siempre se dibujara de la misma forma y ademas lo necesito asi para poder sacarlo de usar data
    [Tooltip("The particle group physics material for this particle group. Drag a LPparticleGroupMaterial scriptable object in here")]
	public LPParticleGroupMaterial GroupMaterial; // KUCHO HACK (hide) no se para que vale asi que me lo quito de la vista
	 public string findThisDrawer;
	public KuchoLPDrawer drawer;
	bool ShowFindThisDrawer(){ return !drawer; }
    [Tooltip("Color of all the particles created in this group")]
	public Color _Color = LPColors.DefaultParticleCol;
	int intColorR;
	int intColorG;
	int intColorB;
	int intColorA;

	bool ShowColor(){
		if (rotatingParticles)
			return false;
		else if (lpParticleSystem && lpParticleSystem.GetRotations)
			return false;
		return true;
	}
	[SerializeField][HideInInspector] Vector2 _Offset;
	 public Vector2 Offset
	{
		get{return _Offset;}
		set{
			_Offset = value;
			if (_Offset != lastOffset)
			{
					LPShapeTools.OffsetPoints(_Offset - lastOffset, pointsList); 
				lastOffset = _Offset;
			}
		}
	}
	[SerializeField][HideInInspector] protected Vector2 lastOffset;

	public Transform transformPointsWithThis;// un transform externo para usarlo como base para Transformar los puntos y crear la shape correctamente en casos en los que somos hijos de GOs que estan rotados o tienen escalas negativas
//	public Vector2 Offset; // KUCHO HACK
    [Tooltip("Strenght of forces between the particles. Affected by the particles material flags, eg. elastic")]
    public float Strenght = 1f;
    [Tooltip("Angular velocity this particle group should be created with SIRVE PARA ALGO? EN LOS DOCS DE LIQUID FUN APARECE SOLO AL CREAR GRUPOS, PERO NO HAY MAS EVIDENCIA DE QUE LAS PARTICULAS PUEDAN GIRAR")]
	[HideInInspector] public float AngularVelocity = 0f;
    [Tooltip("Linear velocity this particle group should be created with")]
    public Vector2 LinearVelocity = Constants.zero2;
    
	public bool _inputLifeTimeInSeconds = true;
	public RandomFloat LifeTimeInSeconds;
	[ReadOnly2Attribute] public MinMax lifeTimeInSecondsMinMax;
	[Tooltip("Lifetime particles should start with. They will be deleted when their lifetime runs out. Value of 0 indicates infinite lifetime")]
	[UnityEngine.Serialization.FormerlySerializedAs("LifeTime")]
	[ReadOnly2Attribute] public RandomFloat LifeTimeInLiquidTimeUnits; // KUCHO HACK era float = 0.0f
	[ReadOnly2Attribute] public float lastTimeInSeconds;// KUCHO HACK, traduce el tiempo raro de LPSystem en segundos con un calculo a mano segun los settings de lpmanager
	
	public bool rotatingParticles; 
	public RandomClampedInt RotationSpeed; // KUCHO HACK al final no la estoy usando en multidrawe, la fijo en LPParticle.UserData pero no hago nada con ella por que me dio problemas y lo dejé
	public RandomClampedInt Rotation; // KUCHO HACK al final no la estoy usando en multidrawe, la fijo en LPParticle.UserData pero no hago nada con ella por que me dio problemas y lo dejé
	public RandomClampedInt RenderSize; // KUCHO HACK the display size to be asigned to the particles;

    [Tooltip("What distance to space between the particles when creating the group. Value of 0 means distance will be the same as particle diamater")]
	public RandomFloat stride;// KUCHO HACK

    int renderStartSize;
    float lifeTime;
    float _stride;
    protected Color GetColors()
    {
#if UNITY_EDITOR
	    if (Selection.Contains(gameObject))
	    {
		    return LPColors.Selected;
	    }
#endif
	    return LPColors.ParticleGroup;
    }
}
