using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public struct Line
{
	public Vector3 P1;
	public Vector3 P2;
}

public static class LPShapeTools
{		
	public static IntPtr Initialise(Vector2[] plist)
	{
		FillStaticFloatMarshallArray(plist);
		return LPAPIShape.GetPolygonShapeDef(marshalFloats);

		return LPAPIShape.GetPolygonShapeDef(GetfloatArray(plist));
	}
	
	//Check if shape is complex by checking if each non-consecutive line intersects
	public static bool CheckComplex(Vector2[] plist)
	{
		if (plist.Length > 3)
		{
			for (int a = 0; a < plist.Length; a++)
			{
				int index2 = a+1;
				if (index2 == plist.Length) index2 = 0;
				
				Line lineA = new Line()
				{
				P1 = plist[a]
				,P2 = plist[index2]
				};
				
				for (int b = 0; b < plist.Length; b++)
				{			
                 	if (b == a || b == a+1 || b == a-1 ) continue;
					if (a==0 && b == plist.Length -1 ) continue;
					if (a==plist.Length -1 && b == 0 ) continue;
					
					int indexb2 = b+1;		
					if (indexb2 == plist.Length) indexb2 = 0;
					
					Line lineB = new Line()
					{
                        P1 = plist[b]
                        ,P2 = plist[indexb2]
					};
					if (DoLinesIntersect(lineA,lineB)) return true;   
                }
			}
		}
		return false;
	}
	
	//Check if two lines intersect using cross product
	public static bool DoLinesIntersect(Line line1, Line line2)
	{
		if (IsCrossProductPos(line1.P1,line1.P2,line2.P1) != IsCrossProductPos(line1.P1,line1.P2,line2.P2) && 
		    IsCrossProductPos(line2.P1,line2.P2,line1.P1) != IsCrossProductPos(line2.P1,line2.P2,line1.P2) ) 
		    return true;
				
		return false;
    }
	
	//Return whether cross product of 3 points is positive or negative
	public static bool IsCrossProductPos(Vector3 p1, Vector3 p2, Vector3 test)
	{
		if( ((p2.x - p1.x)*(test.y - p2.y)) - ((p2.y - p1.y)*(test.x - p2.x)) >0f )return true;
		return false;
	}
	
	//Check if shape is convex using the sum over edges
    public static bool CheckAntiClockwise(Vector2[] plist)
	{
		float cw = 0;
		for (int i = 0; i < plist.Length; i++)
		{
			Vector3 pointA = plist[i];	
			Vector3 pointB;	
			if (i == plist.Length-1)
			{
				pointB = plist[0];
			}
			else
			{
				pointB = plist[i+1];
			}
			
			cw -= ((pointA.x - pointB.x)*(pointA.y + pointB.y));
		}
		if (cw < 0f) return true;
		
		return false;
	}
	
	//Check if shape is convex using the cross product
	public static bool CheckConvex(Vector2[] plist)
	{
		if(plist.Length < 4)return true;
		
		bool crosslessthanzero = false;
		
		for (int i = 0; i < plist.Length ; i++)
		{
			Vector3 midpoint = plist[i];
			
			Vector3 nextpoint;
			if (i+1 >plist.Length -1 ) 
			{
				nextpoint = plist[0];
			}
			else
			{
				nextpoint = plist[i+1];
			} 
			Vector3 lastpoint;
			if (i-1 <0 ) 
			{
				lastpoint = plist[plist.Length -1];
			}
			else
			{
				lastpoint = plist[i-1];
			} 
			
			Vector3 NexttoMid = midpoint - nextpoint;
			Vector3 LasttoMid = midpoint - lastpoint;
			
			float zcrossproduct = (NexttoMid.x * LasttoMid.y)-(NexttoMid.y * LasttoMid.x);	
			if (i == 0)
			{
				if (zcrossproduct < 0f)crosslessthanzero = true;
				else crosslessthanzero = false;
			}
			else
			{
				if ((crosslessthanzero && zcrossproduct > 0f)||(!crosslessthanzero && zcrossproduct < 0f))
				{
					return false;
				}
			}				
		}		
		return true;
	}
	
	public static List<Vector2> MakeEdgePoints()
	{
		List<Vector2> PointsList = new List<Vector2>();
			
		PointsList.Add(new Vector3(-1,0));
		PointsList.Add(new Vector3(1,0));

		return PointsList;
	}
	public static Vector2[] zeroSquare = new Vector2[] { Constants.zero2, Constants.zero2, Constants.zero2, Constants.zero2};
	public static List<Vector2> Force4Points(List<Vector2> PointsList){
		if (PointsList.Count != 4)
		{
			PointsList = new List<Vector2>();
			PointsList.AddRange(zeroSquare); 
		}
		return PointsList;
	}
	public static void MakeBoxPoints(List<Vector2> PointsList, Vector2 Size){
		for (int i = 0; i < 4; i++)
			PointsList[i] = new Vector2 (Size.x / 2 * SquareOne[i,0], Size.y / 2 * SquareOne[i,1]);
	}
	public static float[,] SquareOne = new float[,]{ {-1,1} ,{-1,-1} ,{1,-1} ,{1,1} };		
	public static List<Vector2> MakeBoxPoints(Vector2 Size)
	{
		List<Vector2> PointsList = new List<Vector2>();
		for (int i = 0; i < 4; i++) 
		{
			float xcoord = Size.x / 2 * SquareOne[i,0];
			float ycoord = Size.y / 2 * SquareOne[i,1];		
			PointsList.Add(new Vector2(xcoord,ycoord));
        }
        return PointsList;
	}
	
	public static List<Vector2> makePolyPoints(int sides,float r)
	{		
		List<Vector2> PointsList = new List<Vector2>();
		if (sides < 3) 
		{
			Debug.LogError("You tried to make a polygon shape with less than 3 sides, making one with 3 instead");
			sides = 3;
		}
		if (sides > 8) 
		{
			Debug.LogError("You tried to make a polygon shape with more than 8 sides, making one with 8 instead.");
			sides = 8;
		}
		for (int i = 0; i < sides; i++)
		{
			PointsList.Add (new Vector3(r * Mathf.Cos(2 * Mathf.PI * (float)i / (float)sides), r * Mathf.Sin(2 * Mathf.PI * (float)i / (float)sides)));
    	}
    	return PointsList;
    }
    
    public static Vector3 RotatePoint(Vector3 point,float Angle,Vector2 tran)
    {
    	List<Vector2> onelist = new List<Vector2>{point};
		RotatePoints(onelist,Angle,tran);
		return onelist[0];
    }
    
	public static void RotatePoints(List<Vector2> PointsList,float Angle,Vector2 offset)
    {
		float radMult = Angle * Mathf.Deg2Rad;
		int PointsListCount = PointsList.Count; //Optimizacion: leer .Count es el doble de lento
		for (int i = 0; i < PointsListCount; i++)
		{
			var point = PointsList[i];
			point -= offset;
			point = new Vector2
				(
					(point.x * Mathf.Cos(radMult)) - (point.y * Mathf.Sin(radMult))
					,(point.x * Mathf.Sin(radMult)) + (point.y * Mathf.Cos(radMult))
				);	
			point += offset;
			PointsList[i] = point;
		}
    }
    
	public static List<Vector2> ChangeRadius(float diff,List<Vector2> PointsList,Vector2 tran)
	{
		int PointsListCount = PointsList.Count; //Optimizacion: leer .Count es el doble de lento
		for (int i = 0; i < PointsListCount; i++)
		{
			PointsList[i] -= tran;
			PointsList[i] += PointsList[i] * diff;
			PointsList[i] += tran;
		}
		return PointsList;
    }
	public static void OffsetPoints(Vector2 offset,List<Vector2> PointsList){
		int PointsListCount = PointsList.Count; //Optimizacion: leer .Count es el doble de lento
		for (int i = 0; i < PointsListCount; i++)
			PointsList[i] += offset;
	}
//    public static List<Vector2> OffsetPoints(Vector2 offset,List<Vector2> PointsList)
//    {
//		for (int i = 0; i < PointsList.Count; i++)
//		{
//			PointsList[i] = new Vector2(PointsList[i].x + offset.x, PointsList[i].y + offset.y);
//		}
//		return PointsList;
//    }
    
	public static Vector2[] TransformPoints(Transform transform,Vector3 pos,List<Vector2> PointsList)
	{
		if (PointsList == null)
		{
			Debug.Log(" ERROR POINTSLIST ES NULL");
			return null;
		}
		else
		{
			Vector2[] Points = new Vector2[PointsList.Count];  	
			for (int i = 0; i < Points.Length; i++)
			{
				Vector3 point3 = (Vector3)PointsList[i];
                Points[i] = transform.TransformPoint(point3);// - pos; // por que carajo puse un -pos aqui? me anula la transformacion WTF?
			}
			return Points;
		}
    }
	public static void TransformPoints(Transform transform, List<Vector2> PointsList, Vector2[] pointsCopy){
		for (int i = 0; i < pointsCopy.Length; i++)
			pointsCopy[i] = transform.TransformPoint((Vector3)PointsList[i]);// - transform.position;
	}
	public static void TransformPoints(Transform transform, float rotation, List<Vector2> PointsList, Vector2[] pointsCopy){
		Transform newTrans = transform;
		newTrans.eulerAngles = new Vector3(newTrans.eulerAngles.x, newTrans.eulerAngles.y, rotation);
		for (int i = 0; i < pointsCopy.Length; i++)
		{
			pointsCopy[i] = newTrans.TransformPoint((Vector3)PointsList[i]);
		}
	}
	static float[] marshalFloats = new float[17]; // los poligonos de box2D tienen de mazimo 8 vertices, 8*2 = 16 + la primera posicion que indica longitud
	public static void FillStaticFloatMarshallArray(Vector2[] points){
		if (points.Length > 17)
		{
			Debug.LogError("INTENTANDO INICIALIZAR UN POLIGONO BOX2D CON MAS DE 8 PUNTOS?!");
			return;
		}
		marshalFloats[0] = points.Length;
		int marshalIndex = 0;
		for (int i = 0; i < points.Length; i++)
		{
			marshalIndex++; // asi la primera vez valdra 1, ya que el indice 0 es lara la longitud
			marshalFloats[marshalIndex] = points[i].x;
			marshalIndex++;
			marshalFloats[marshalIndex] = points[i].y;
		}
			
	}
	public static float[] GetfloatArray(Vector2[] PointsList)
	{
		float[] pointfloats = new float[(PointsList.Length * 2) + 1];
		pointfloats[0] = PointsList.Length;
		for (int i = 0; i < PointsList.Length; i++)
		{
			pointfloats[(i * 2) + 1] = PointsList[i].x;
			pointfloats[(i * 2) + 2] = PointsList[i].y;
		}
		return pointfloats;
	}
	
	public static Vector3[] Vec2listToVec3Array(List<Vector2> PointsList)
	{
		Vector3[] returnarray = new Vector3[PointsList.Count];
		for (int i = 0; i < returnarray.Length; i++)
		{
			returnarray[i] = new Vector3(PointsList[i].x,PointsList[i].y); 
		}
		return returnarray;
	}

    public static Vector3[]Vec2ArrayToVec3Array(Vector2[] vec2array)
    {
        Vector3[] points = new Vector3[vec2array.Length];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = vec2array[i];
            }
            return points;
    }
	   
    public static void DrawGizmos (Color colour,Vector2[] Points,bool loop)
	{								
		Gizmos.color = colour;	
		
		if (Points != null)
		{
			for (int i = 0; i < Points.Length; i++)
			{
				if (i == Points.Length-1) 
				{
					if (loop) 
					{
						Gizmos.DrawLine(Points[i] ,Points[0]);
					}
					else
					{
						break;
					}
				}
				else
				{
					Gizmos.DrawLine(Points[i],Points[i+1]);
				}
			}				
		}
		
	}
	
	public static void DrawGizmosPlaying (Vector3 pos,float diff, Color colour,Vector2[] Points,bool loop)
	{	
//		Vector3 pos;
//		if (tran) 
//			pos = transform.position;
//		else
//			pos = Constants.zero3;
        Quaternion ang;
		if (diff != 0f)
			ang = Quaternion.AngleAxis(diff,Vector3.forward);
		else
			ang = Constants.zeroQ;
		
		Gizmos.matrix = Matrix4x4.TRS(pos,ang,Vector3.one); 
		
		DrawGizmos(colour,Points,loop);
    }
}
