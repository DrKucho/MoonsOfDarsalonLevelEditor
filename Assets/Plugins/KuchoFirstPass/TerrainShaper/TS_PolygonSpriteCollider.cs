using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class TS_PolygonSpriteCollider : TS_SpriteCollider
{
	[System.Serializable]
	public class Path
	{
		public Vector2[] points;
		public Vector2 center; 

		public Path(Vector2[] _points)
		{
			points = _points;
		}

		public void CalculateCenter()
		{
			center = Constants.zero2;
			foreach (Vector2 p in points)
			{
				center.x += p.x;
				center.y += p.y;
			}

			center = center / points.Length;
		}
	}

	[System.Serializable]
	public class Cell
	{
		public PolygonCollider2D Collider;

		public List<LPFixturePoly> Fixture = new List<LPFixturePoly>(); 
		public List<Path> Path = new List<Path>(); 
		public ExpensiveTaskManager globalExplosionManager; 
		public LPBody lpBody; 
		public GameObject child; 
		public int index;
		public float detail;
		public float left;
		public float right; 
		public float top; 
		public float bottom; 

	}

	public int CellSize = 64;

	[Range(0.5f, 1.0f)] public float Detail = 0.69f;

	public float SnapDist = 1f;

	public int SkipPoints = 0; 

	public Vector2 TrimLimit = new Vector2(3f, 3f); 

	public bool Binary = true;

	public bool roundPoints = false; 

	public bool useUnityPhysics = true; 

	public bool useLiquidPhysics = false; 

	public bool independentLPChild = true; 

	public bool independentLPBody = false; 



	[SerializeField] public List<Cell> cells = new List<Cell>();

	[SerializeField] private int cellsX;

	[SerializeField] private int cellsY;

	[SerializeField] private int cellsXY;

	[SerializeField] private int width;

	[SerializeField] private int height;
}