using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;

/// <summary>
/// Sirvo para dibujar particulas Liquidas, soy mejor que el Visualizer con Elipsoid particle emiter etc por que uso shuriken y
/// no genero basura ya que uso shurikeny con shuriken se pueden leer las particulas sin memory allocation reutilizando siempre la misma tabla  
/// </summary>

public class KuchoLPMultiDrawer: MonoBehaviour
{
	public bool debug = false;
	new public LPParticleSystem particleSystem; // KUCHO HACK solo puse el new para qu eno de warning
	public List<KuchoLPDrawer> drawer;
	[ReadOnly2Attribute] public int drawerCount; // optimizacion , leer .Count es el doble de lento
	public int allDrawersPartCount;

	void Awake(){
		drawer.Clear();// ha de estar vacia para que sea rellenada por los drawers correctamente !
	}
	void OnValidate(){
		if (isActiveAndEnabled)
			particleSystem = GetComponent<LPParticleSystem>(); // para que los drawers sepan cual es y puedan mostrar/ocultar opciones en inspector, luego se va a sobreescribir en la inicializacion
	}
	
    int previousFrame = -1;
    
}
