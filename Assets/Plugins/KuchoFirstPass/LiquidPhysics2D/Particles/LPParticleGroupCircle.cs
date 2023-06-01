using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Represents a circle shaped particle group in the liquidfun simulation</summary>
public class LPParticleGroupCircle : LPParticleGroup
{	
	[Tooltip("Radius of this circle")]
	public MinMax radius = new MinMax(0.25f, 0.25f); // la antigua property float radius de LP inicializaba a 0.25f
    [HideInInspector] public float randomRadius;
    [HideInInspector] public float multiplier = 1; // otros scripts pueden fijar este valor para aumentar o disminuir la produccion de particulas aun conservando el componente aleatorio


}

