using UnityEngine;
using System.Collections;

public class DifficultyZone : MonoBehaviour {

	
	private LevelDifficulty levelDifficulty;
	public Difficulty zone;
	
	public void Start(){ //  print(this + "START ");

		levelDifficulty = FindObjectOfType<LevelDifficulty>();
	}
	
	public void Update(){ //  print (this + " UPDATE ");
	
	}
	public void OnTriggerEnter2D(Collider2D collider){
		if (collider == Game.playerCol){
			levelDifficulty.zone.maxEnemyWeightOnScene *= zone.maxEnemyWeightOnScene;
			levelDifficulty.zone.pickUpChances *= zone.pickUpChances;
			levelDifficulty.zone.generatorDelay *= zone.generatorDelay;
			levelDifficulty.zone.enemyEnergy *= zone.enemyEnergy;
			levelDifficulty.zone.enemyFlyingForce *= zone.enemyFlyingForce;
			levelDifficulty.zone.enemyRunSpeed *= zone.enemyRunSpeed;
			levelDifficulty.zone.enemyInitialNoAttackTime *= zone.enemyInitialNoAttackTime;
			levelDifficulty.zone.enemyFireTimer *= zone.enemyFireTimer;
			levelDifficulty.zone.enemyPunchTimer *= zone.enemyPunchTimer;
			levelDifficulty.zone.playerBulletSpeed *= zone.playerBulletSpeed;
			levelDifficulty.CalculateRealDifficulty();		
		}
	}
	public void OnTriggerExit2D(Collider2D collider){
		if (collider == Game.playerCol){
			levelDifficulty.zone.maxEnemyWeightOnScene /= zone.maxEnemyWeightOnScene;
			levelDifficulty.zone.pickUpChances /= zone.pickUpChances;
			levelDifficulty.zone.generatorDelay /= zone.generatorDelay;
			levelDifficulty.zone.enemyEnergy /= zone.enemyEnergy;
			levelDifficulty.zone.enemyFlyingForce /= zone.enemyFlyingForce;
			levelDifficulty.zone.enemyRunSpeed /= zone.enemyRunSpeed;
			levelDifficulty.zone.enemyInitialNoAttackTime /= zone.enemyInitialNoAttackTime;
			levelDifficulty.zone.enemyFireTimer /= zone.enemyFireTimer;
			levelDifficulty.zone.enemyPunchTimer /= zone.enemyPunchTimer;
			levelDifficulty.zone.playerBulletSpeed /= zone.playerBulletSpeed;
			levelDifficulty.CalculateRealDifficulty();		
		}
	
	}
}
