using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// This is the base class for bodies, fixtures, particle groups and joints</summary>
public abstract class LPThing : MonoBehaviour
{
    public bool debug;

    protected bool initialized = false; 

    [HideInInspector] public string at_name;
    [HideInInspector] public bool at_enabled = false;
    [HideInInspector] public bool at_isActiveAndEnabled = false;
    [HideInInspector] public Transform at_myTransform;
    [HideInInspector] public Vector3 at_transformPosition; // para poder leerla desde el audio thread
    [HideInInspector] public float at_transformRotation;
    [HideInInspector] public Vector3 at_lossyScale;
    [HideInInspector] public bool destroyUnityComponent = false; 

    public bool initTaskPending;
    public uint mtInitID; // para debuguear y saber si esta tarea se ha añadido antes o despues que las de sus fixtures;
    public bool deleteTaksPenging;
    public uint mtDeleteID;// para debuguear y saber si esta tarea se ha añadido antes o despues que las de sus fixtures  

    public virtual void OnValidate(){
	}
	public virtual void Awake(){
        at_myTransform = transform;
        at_transformPosition = at_myTransform.position;
        at_name = gameObject.name;
	}
    /// <summary>
    /// llama aqui para que las variables que no se puden actualizar en audio thread se actualice
    /// </summary>
    public virtual void MainThreadUpdate(){
        at_transformPosition = at_myTransform.position;
        at_transformRotation = at_myTransform.eulerAngles.z;
        at_lossyScale = at_myTransform.lossyScale;
        at_enabled = enabled;
        at_isActiveAndEnabled = isActiveAndEnabled;
        //if (debug)
            //Debug.Log(name + " MT Upd - Enable = " + at_isActiveAndEnabled + " FRM=" + KuchoTime.frameCount);
    }
	/// <summary>
	/// This IntPtr structure stores the pointer to the relevant C++ object in the liquidfun library.</summary> 
	 public IntPtr ThingPtr;
	
	/// <summary>
	/// Get the pointer to the C++ object represented by this object</summary>
	public IntPtr GetPtr()
	{
		return ThingPtr;
	}
	/// <summary>
	//Remove the thing from the simulation and delete the associated unity component.</summary>
	//public abstract void Delete();

    public bool InSimulation(){
        return initialized && ThingPtr != IntPtr.Zero;
    }
}
