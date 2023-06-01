using UnityEngine;
using System.Collections;

public class LPFixturePolyFromPolygonCollider : MonoBehaviour {

	
	[System.Serializable]
	public class Cell {
		public LPFixturePoly Collider;
	//		public LiquidPhysics2D.LPThing.LPCorporeal.LPFixture.LPFixturePoly Collider;
		
		public void Destroy(){
			if (Collider != null)
			{
				Collider = TS_Helper.Destroy(Collider);
			}
		}
		public void UpdateColliderSettings(bool isTrigger, PhysicsMaterial2D material){
			if (Collider != null)
			{
				Collider.IsSensor      = isTrigger;
				Collider.PhysMaterial = material;
			}
		}
		public void Trim(int pathCount){
	//		if (pathCount > 0)
	//		{
	//			if (Collider.pathCount > pathCount)
	//			{
	//				Collider.pathCount = pathCount;
	//			}
	//		}
	//		else
	//		{
	//			Destroy();
	//		}
		}
	}
}
