using UnityEngine;
using System.Collections;

public enum walkerAIType { Normal, Ilde}

public class NotFlyerAI : MonoBehaviour {

	//public class GroundedIA {
	public walkerAIType typeOfAI;
	public AttackCollider keepDistanceToTarget = AttackCollider.None;
	[Range (0,1)] public float keepDistanceFactor = 0.8f;
	[Range (0,3)] public float keepDistanceY_Factor = 0.2f; // la distancia x a mantener se vera reducida proporcionalmente a la distancia de altura con el target
	[HideInInspector] public int xDistanceToTargetToKeep;
	public SwitchToFlyer switchToFlyer;
	//
	//	@Header("ya no las uso")
	//	bool _distToPlayerTurnBack = true;
	//	Vector2 distToPlayerTurnBack;
	//}
	//function CopyTo(to:WalkerAI){
	//	to.typeOfAI = typeOfAI;
	//	to._distToPlayerTurnBack = _distToPlayerTurnBack;
	//	to.distToPlayerTurnBack = distToPlayerTurnBack;
	//	to.keepDistanceToTarget = keepDistanceToTarget;
	//	to.keepDistanceFactor = keepDistanceFactor;
	//	to.distanceToTargetToKeep = distanceToTargetToKeep;
	////	to.walkToPositionMargin = walkToPositionMargin;
	//	to.switchToFlyer = switchToFlyer; // esta poria no copiarse sino crearse referencia porque es una class?
	//}
}
