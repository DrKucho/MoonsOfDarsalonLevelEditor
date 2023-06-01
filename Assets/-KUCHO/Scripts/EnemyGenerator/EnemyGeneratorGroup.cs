using UnityEngine;
using System.Collections;

public class EnemyGeneratorGroup : MonoBehaviour {

	public bool debug = false;
	  public Component[] enemyGeneratorsInComponentFormat; // solo me sirve para poder extraer los EnemyGeneratos, no me queda otra que extraerlos primero en formato component
	 public ObjectSpawner[] enemyGenerator; // esta si es la tabla que va a guardar los enemy generators
	 public ObjectSpawner[] enemyGeneratorThatDetectedPlayerAndHasNoEnemies;
	public int enemies = -1;
	public int maxEnemiesOnScene = 2;
	 public int enemiesOnScene = 0;
	 public int i = 0;
	private int e = 0;
}
