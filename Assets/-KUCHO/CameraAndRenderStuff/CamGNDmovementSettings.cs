using UnityEngine;
using System.Collections;


[System.Serializable]
public class CamGNDmovementSettings{
	public bool	following = true;
	public float	stopFollowingAfterSeconds;
	public Snap	pixelSnapX;
	public Snap	pixelSnapY;
	public Vector2	offset;
	public bool	waitForGroundedToFollowY;
	public float	speedToOffsetZone; // la velocidad a la movera hasta alcanzar el offset
	[HideInInspector] public float speedToOffsetZone2; // de uso interno para ir calculando la velocidad a la que debe moverse en cada frame
	public int	yLimit;
	public int yLimitUp;
	public int xLimitLeft;
	public int xLimitRight;
	public Vector2 speedDivider;
	public Vector2 speedFactor;
	public Vector2 minSpeed;
	public Vector2 stopSpeed;
	public Vector2 distanceBasedSpeed;
	public Vector2 speedFactorOnTargetZeroMovement;
	public Vector2 speedFactorOnWalker;
	public Vector2 centeredRange;
	public Vector2 minTargetPpsToMove;
	public Vector2 matchAtThisTargetSpeed;
	public bool limitInc = false;
	public Vector2 incLimit;
	public float aimDistance;
	public float aimDistanceTrim;
	public float aimSpeedFactor;
	public float aimMaxSpeed;
	public float aimMinSpeed;
	public float aimZeroSpeed;

	public void CopyTo(CamGNDmovementSettings to){
		to.following = following;
		to.stopFollowingAfterSeconds = stopFollowingAfterSeconds;
		to.pixelSnapX = pixelSnapX;
		to.pixelSnapY = pixelSnapY;
		to.offset = offset;
		to.waitForGroundedToFollowY = waitForGroundedToFollowY;
		to.speedToOffsetZone = speedToOffsetZone;
		to.speedToOffsetZone2 = speedToOffsetZone2;
		to.yLimit = yLimit;
		to.yLimitUp = yLimitUp;
		to.xLimitLeft = xLimitLeft;
		to.xLimitRight = xLimitRight;
		if (speedDivider.x == 0) speedDivider.x = 1.1f; // si es cero data infinito/error al calcular la posicion
		if (speedDivider.y == 0) speedDivider.y = 1.1f; // si es cero data infinito/error al calcular la posicion
		to.speedDivider = speedDivider;
		to.speedFactor = speedFactor;
		to.minSpeed = minSpeed;
		to.stopSpeed = stopSpeed;
		to.distanceBasedSpeed = distanceBasedSpeed;
		to.speedFactorOnTargetZeroMovement = speedFactorOnTargetZeroMovement;
		to.centeredRange = centeredRange;
		to.minTargetPpsToMove = minTargetPpsToMove;
		to.matchAtThisTargetSpeed = matchAtThisTargetSpeed;
		to.limitInc = limitInc;
		to.incLimit = incLimit;
		to.aimDistance = aimDistance;
		to.aimDistanceTrim = aimDistanceTrim;
		to.aimSpeedFactor = aimSpeedFactor;
		to.aimMaxSpeed = aimMaxSpeed;
		to.aimMinSpeed = aimMinSpeed;
		to.aimZeroSpeed = aimZeroSpeed;
	}
}
	
