using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// The base class for any classes that use polygon shapes (fixtures and particle groups)
/// Contains code for dealing with polygon editing in the unity editor</summary>
public abstract class LPCorporeal : LPThing
{
	[HideInInspector] public bool Drawing;
	[HideInInspector] public bool drawingfirstpoint;	
	[HideInInspector] public bool DontDrawLoop;

	[HideInInspector] public float ClosestDist;
	[HideInInspector] public bool EditMe = true;
    
	[HideInInspector] public Vector2[] pointsListOriginal;
	[HideInInspector] public List<Vector2> pointsList;
	[HideInInspector] public Vector2[] pointsCopy;
	[HideInInspector] public bool isComplex = false; // se pone a true si no se ha podido crear el poligono por intersecciones feas y otros problemas

    [HideInInspector] public IntPtr shape; // KUCHO HACK
	[HideInInspector] public List<IntPtr> SubPtrs; // KUCHO HACK la puse public para que pueda ser borrada por LpManager
    [HideInInspector] public List<IntPtr> subShapes; // KUCHO HACK me la inventé y la puse public para que pueda ser borrada por LpManager

	[HideInInspector] public List<List<Vector2>> paths; // KUCHO HACK para el modo pintar terreno con explosion stamp extras, no se que hice pero use las fixtures para dibujar y extraer un poligono
	List<List<Vector2>> subFixs; //fixtures de liquidFun con 8 vertices o menos:
	[HideInInspector] public Rect bounds = new Rect(); // KUCHO HACK no habia bounds


	/// <summary>
	/// Get the list of points representing this polygonal shape</summary>
	public virtual List<Vector2> GetPoints(){
		return pointsList;
	}
	/// <summary>
	/// KUCHO HACK --- esta funcion es totalmente nueva</summary>
	public virtual Vector2[] GetPointsCopy(){
		return pointsCopy;
	}
	public void CopyPointsListToPointsCopy(){
		int pointsListCount = pointsList.Count; //optimizacion leer .Count es el doble de lento!
		if (pointsCopy == null || pointsCopy.Length != pointsListCount)
			pointsCopy = new Vector2[pointsListCount];
		for (int i = 0; i < pointsListCount; i++)
			pointsCopy[i] = pointsList[i];
	}
	protected void CopyPointsListToOriginalPointsList(){
		int pointsListCount = pointsList.Count; //optimizacion leer .Count es el doble de lento!
		if (pointsListOriginal == null || pointsListOriginal.Length != pointsListCount)
			pointsListOriginal = new Vector2[pointsListCount];
		for (int i = 0; i < pointsListCount; i++)
			pointsListOriginal[i] = pointsList[i];
	}
	/// <summary>
	/// Define the points of a polygonal particle group or fixture, or a chainshape programmatically</summary>
	public virtual void DefinePoints(Vector2[] points){
		var type = this.GetType();
		if (type == typeof(LPFixturePoly) || type == typeof(LPParticleGroupPoly) || type == typeof(LPFixtureChainShape))
		{
			pointsList.Clear(); // asi no genera tanta basura, antes creaba lista nueva siempre
			for (int i = 0; i < points.Length; i++)
				pointsList.Add(points[i]);
		}
	}
	
	/// <summary>
	/// Change the position of a particular point</summary>
	public virtual void EditPoint(int index,Vector3 pos){
		pointsList[index] = transform.InverseTransformPoint(pos + transform.position);
	}
	
	/// <summary>
	/// Clear all the points from this polygonal shape</summary>
	public virtual void EmptyPoints(){
		if (pointsList == null)
			pointsList = new List<Vector2>();
		pointsList.Clear();
	}
	
	/// <summary>
	/// Remove a particular point from this polygonal shape</summary>
	public virtual void RemovePoint(int index){
		pointsList.RemoveAt(index);
	}
	
	/// <summary>
	/// Add a point to this polygonal shape, at the end of the list</summary>
	public virtual void AddPoint(Vector3 pos)
	{
		pointsList.Add(transform.InverseTransformPoint(pos + transform.position));
	}
	
	/// <summary>
	/// Insert a new point to this polygonal shape, at a particular index</summary>
	public virtual void InsertPoint(int index,Vector3 pos)
	{
		pointsList.Insert(index,transform.InverseTransformPoint(pos + transform.position));
	}		
	
	/// <summary>
	/// Initialse a subshape of this fixture or particle group (for convex shapes)</summary>
	//protected abstract IntPtr AT_InitialiseWithShape(IntPtr shape);
	
	/// <summary>
	/// Logs an error if there is an attempt to create a complex (self intersecting) shape
	/// A default diamond shape is created instead </summary>
	protected virtual void LogComplex()
	{
	}
/// <summary>
/// fija la lista paths, pero no es la misma que al inicializar el poligono en la simulation , esa es bits(ahora subFixs), por que? lo hice asi por no enredar en codigo que no es mio , pero creo que facilmente puedo eliminar bits y usar siempre paths, asi se actualiza si lo pido en esta funcion y si simplemente se inicializa el poligono en la simulacion
/// </summary>
	public virtual void DecomposeAndSetPaths(){
		pointsCopy = LPShapeTools.TransformPoints(transform,Constants.zero3,pointsList);

		//check anticlockwise			
		if (!LPShapeTools.CheckAntiClockwise(pointsCopy))
		{
			Array.Reverse(pointsCopy); 
		}

		if (pointsCopy.Length > 8 || !LPShapeTools.CheckConvex(pointsCopy))
		{					
			List<Vector2> verts = new List<Vector2>();

			foreach (Vector3 point in pointsCopy)
			{
				verts.Add(new Vector2(point.x, point.y));
			}
			paths = PolygonDecomposerAPI.DecomposePolygon(verts);
		}
		else
		{
			paths = new List<List<Vector2>>();
			paths.Add(new List<Vector2>());
			var path = paths[0];
			for (int i = 0; i < pointsCopy.Length; i++)
			{
				path.Add((Vector2)pointsCopy[i]);
			}
		}
	}
	/// <summary>
	// KUCHO HACK --- esta funcion es totalmente nueva
	public virtual Vector2[] GetPath(int pathIndex)
	{
		List<Vector2> path = paths[pathIndex];
		Vector2[] v2 = new Vector2[path.Count];
		for (int i = 0; i < v2.Length; i++)
			v2[i] = path[i];
		return v2;
	}

    public void CalculatePointsCopy(){
        pointsCopy = LPShapeTools.TransformPoints(at_myTransform, at_transformPosition, pointsList);
    }
	public void SetBounds(){
		Vector2 min;
		Vector2 max;
		max.x = float.MinValue;
		max.y = float.MinValue;
		min.x = float.MaxValue;
		min.y = float.MaxValue;

		// saca los maximos y minimos
		int pointListCount = pointsList.Count; // optimizacion , leer .Count es el doble de lento!
		for (int i = 0; i < pointListCount; i++)
		{
			Vector3 p = pointsList[i];
			if (p.x > max.x)
				max.x = p.x;
			if (p.y > max.y)
				max.y = p.y;
			if (p.x < min.x)
				min.x = p.x;
			if (p.y < min.y)
				min.y = p.y;
		}
		bounds = new Rect(min, max - min);
		bounds.center += (Vector2)transform.position;
	}

}
