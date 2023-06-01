using UnityEngine;
using System.Collections;
using System;

public static class LPShapeCircle 
{	
	public static  IntPtr Initialise(Vector2 pos,float Radius)
	{
		return LPAPIShape.GetCircleShapeDef(Radius,pos.x,pos.y);
	}
	
    public static void DrawGizmosKucho(Transform transform, Vector2 point, Color colour,float Radius)
    {
        Gizmos.color = colour;
//        Quaternion ang;
//        if (angle !=0f)
//            ang = Quaternion.AngleAxis(angle,Vector3.forward);
//        else
//            ang = Constants.zeroQ;
//        Gizmos.matrix = Matrix4x4.TRS(pos,ang,Vector3.one); 
        Vector3 pos =  transform.TransformPoint(new Vector3(point.x,point.y));

        Gizmos.DrawWireSphere(pos,Radius);
    }
	public static void DrawGizmos (Transform transform,bool tran,float diff, Color colour,float Radius,Vector2 Offset)
	{
		Gizmos.color = colour;
		Vector3 pos =  transform.TransformPoint(new Vector3(Offset.x,Offset.y));
        Gizmos.DrawWireSphere(pos,Radius);
	}
	/// <summary>
    /// esto esta cogido con pinzas, no entiendo como hay un pos y un point, pero como no entiendo lo que hace y pasandole a ambos las mismas coordenadas parece funcionar lo dejo asi
    /// </summary>

	public static void DrawGizmosPlaying (float radius,Vector3 pos,bool tran,float angle, Color colour,Vector3 Point)
	{	
//		Vector3 pos;
		Quaternion ang;
		if (tran)
		{
		}
//			pos = transform.position;
		else
			pos = Constants.zero3;

		if (angle !=0f)
			ang = Quaternion.AngleAxis(angle,Vector3.forward);
		else
			ang = Constants.zeroQ;
		
		Gizmos.matrix = Matrix4x4.TRS(pos,ang,Vector3.one); 
		Gizmos.color = colour;
		Gizmos.DrawWireSphere(Point,radius);
	}
}
