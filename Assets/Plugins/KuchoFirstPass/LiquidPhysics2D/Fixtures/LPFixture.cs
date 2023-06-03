using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Represents a fixture in the liquidfun simulation</summary>
//[RequireComponent (typeof (LPBody))] // KUCHO HACK , no necesito un body en las fixtures que son hijas de un body que esta en un componente mas arriba
public abstract class LPFixture : LPCorporeal
{
//	[ReadOnly2Attribute] public bool initialised = false; // KUCHO HACK si borro una fixture que no ha sido inicializada,  LP se cuelga
	[ReadOnly2Attribute] public bool inBodyList;
	public string fixName; // KUCHO HACK para poder identificar partes 
	[Tooltip("Density of this fixture. Will determine its mass")]
	public float Density = 1f;
	[Tooltip("Is this fixture a sensor? Sensors dont interact in collisions")]
	public bool IsSensor = false;
	[Tooltip("Offset of this fixture from the body")]
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

	[Tooltip("What material is this made out of? Governs bounciness and friction")]
	public PhysicsMaterial2D PhysMaterial;
	
	public int CollisionGroupIndex = 0;
		
	protected float actualRestitution;
	protected float actualFriction;
	protected float defaultRestitution = 0.2f;
	protected float defaultFriction = 0.1f;
	protected LPShapeTypes Shapetype;
	/// <summary>key of this fixture in the LPbody dictionary myFixtures
	/// and its userdata value in the simulation</summary>	
//	protected int myIndex;	
	public LPBody body; // la puse public
	UInt16 categoryBits = 0x0001;
	UInt16 maskBits = 0xFFFF;

	public bool makeItReusable; // si true usara los shapes de la vez anterior para no tener que recalcular los puntos y pedirle a la engine que nos entregue una shape
//    public bool zombie = false;
	bool reusable; // flag interno para saber si podemos reusar
	int userDataFlags;
    bool applicationIsPlaying ; // no puedo leer Application.IsPLaying desde el audio thread

    public override void OnValidate(){
	    if (isActiveAndEnabled)
	    {
		    if (pointsList == null)
			    pointsList = new List<Vector2>();
		    CopyPointsListToPointsCopy();
	    }
    }
}
