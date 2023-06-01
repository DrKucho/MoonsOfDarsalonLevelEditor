using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;


public enum ArmyType {GoodGuys, BadGuys, All, WildAnimals, NoArmy}
public enum LevelCollision {Never ,Always, OnBiggerHorizontalSpeedOnly}

[System.Serializable]
public class BulletMask
{
	public ArmyType armyType = ArmyType.BadGuys;
  	public bool ground;
    public bool obstacle;
	public bool obstacleWontCollideWithCharacters;
    public LevelCollision level;
   	
   	[HideInInspector] public int mask;
	public int MakeMask(){
		return MakeMask(Constants.zero2);
	} 
	public int MakeMask(Vector2 speed){
		mask = 0;
		switch (armyType){
			case (ArmyType.BadGuys):
				AddBadGuys();
				break;
			case (ArmyType.GoodGuys):
				AddGoodGuys();
				break;
			case (ArmyType.All):
				AddBadGuys();
				AddGoodGuys();
				break;
		}
		if (ground) mask = mask + (1 << Layers.ground); // LayerMask.NameToLayer("Ground"));}
		if (obstacle)
			mask = mask + (1 << Layers.obstacle); // LayerMask.NameToLayer("Obstacle"));

		if (obstacleWontCollideWithCharacters)
			mask = mask + (1 << Layers.obstacleWontCollideWithCharacters); // LayerMask.NameToLayer("ObstacleWontCollideWithCharacters"));

		switch (level){
			case (LevelCollision.Never):
				break;
			case (LevelCollision.Always):
				mask = mask + (1 << Layers.level); //LayerMask.NameToLayer("Level"));
				break;
			case (LevelCollision.OnBiggerHorizontalSpeedOnly):
				if (Mathf.Abs(speed.x) > Mathf.Abs(speed.y)) mask = mask + (1 << Layers.level); // LayerMask.NameToLayer("Level")); 
				break;	
		}
		return mask;
	}
	public void AddBadGuys(){
		mask = mask + (1 << Layers.badGuys); // LayerMask.NameToLayer("BadGuys"));
		mask = mask + (1 << Layers.badGuysAttack); // LayerMask.NameToLayer("BadGuysAttack"));
		if (ground) mask = mask + (1 << Layers.groundForGoodGuysOnly); //LayerMask.NameToLayer("GroundForPlayerOnly"));}
	}
	public void AddGoodGuys(){
		mask += (1 << Layers.goodGuys); //LayerMask.NameToLayer("GoodGuys");
		mask += (1 << Layers.goodGuysAttack); //LayerMask.NameToLayer("GoodGuysAttack");
		if (ground){
			mask = mask + (1 << Layers.groundForBadGuysOnly); //LayerMask.NameToLayer("GroundForBadGuysOnly"));
			mask = mask + (1 << Layers.groundForBadGuysAtBirthOnly); //LayerMask.NameToLayer("GroundForBadGuysAtBirthOnly"));
		}
	}
}

//function MakeMask(speed:Vector2){
//	int mask = 0;
//	print(" DENTRO DE MAKE MASK");
//	switch (army){
//		case (Army.BadGuys): // es una bala de player/goodguy y destruye a los malos
//			print(" LA BALA DESTRUYE ENEMIGOS");		
//			mask = mask + (1 << LayerMask.NameToLayer("BadGuys")); print (" SUMO BadGuys" + LayerMask.NameToLayer("BadGuys") + " MASK = " +mask);
//			mask = mask + (1 << LayerMask.NameToLayer("BadGuysAttack")); print (" SUMO BadGuysAttack" + LayerMask.NameToLayer("BadGuysAttack") + " MASK = " +mask);
//			if (ground){ mask = mask + (1 << LayerMask.NameToLayer("GroundForPlayerOnly")); print (" SUMO GndForPlayerOnly" + LayerMask.NameToLayer("GroundForPlayerOnly") + " MASK = " +mask);}
//			break;
//		case (Army.GoodGuys): // eres una bala de enemigo/badguy y destuye a los buenos
//			print(" LA BALA DESTRUYE PLAYER");		
//			mask += 1 << LayerMask.NameToLayer("GoodGuys"); print (" SUMO GoodGuys" + LayerMask.NameToLayer("GoodGuys") + " MASK = " +mask);
//			mask += 1 << LayerMask.NameToLayer("GoodGuysAttack"); print (" SUMO GoodGuys Attack" + LayerMask.NameToLayer("GoodGuysAttack") + " MASK = " +mask);
//			if (ground){
//				mask = mask + (1 << LayerMask.NameToLayer("GroundForBadGuysOnly")); print (" SUMO GroundForBadGuysOnly" + LayerMask.NameToLayer("GroundForBadGuysOnly") + " MASK = " +mask);
//				mask = mask + (1 << LayerMask.NameToLayer("GroundForBadGuysAtBirthOnly")); print (" SUMO GroundForBadGuysAtBirthOnly" + LayerMask.NameToLayer("GroundForBadGuysAtBirthOnly") + " MASK = " +mask);
//			}
//	}
//	if (ground){ mask = mask + (1 << LayerMask.NameToLayer("Ground")); print (" SUMO Ground" + LayerMask.NameToLayer("Ground") + " MASK = " +mask);}
//	if (roof){ mask = mask + (1 << LayerMask.NameToLayer("Roof")); print (" SUMO Roof" + LayerMask.NameToLayer("Roof") + " MASK = " +mask);}
//	if (obstacle){ mask = mask + (1 << LayerMask.NameToLayer("Obstacle")); print (" SUMO Obstacle" + LayerMask.NameToLayer("Obstacle") + " MASK = " +mask);}
//	switch (level){
//		case (LevelCollision.Never):
//			break;
//		case (LevelCollision.Always):
//			mask = mask + (1 << LayerMask.NameToLayer("Level"));
//			break;
//		case (LevelCollision.OnBiggerHorizontalSpeedOnly):
//			if (Mathf.Abs(speed.x) > Mathf.Abs(speed.y)) mask = mask + (1 << LayerMask.NameToLayer("Level")); 
//			break;	
//	}
//	return mask;
//}
