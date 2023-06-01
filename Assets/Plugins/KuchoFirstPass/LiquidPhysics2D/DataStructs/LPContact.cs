using UnityEngine;
using System.Collections;

//This class contains information about a fixture / fixture contact
public struct LPContactFixFix
{
	public int BodyAIndex;
	public int FixtureAIndex;
	
	public int BodyBIndex;
	public int FixtureBIndex;
	
	public Vector3 ManifoldPoint1;
	public Vector3 ManifoldPoint2;
	
	public Vector3 Normal;
	
	public bool IsTouching;
}

//This class contains information about a particle / fixture contact
public struct LPContactPartFix
{
	public int ParticleSystemIndex;
	public int ParticleIndex;
	
	public int BodyIndex;
	public int FixtureIndex;
		
	public Vector2 Normal; // KUCHO HACK era Vector3, ineficiente!

	// KUCHO HACK no tenia constructor, se inicializaba con llaves y nombres de las variables , no sabia que se podia hacer asi
	public LPContactPartFix(int _ParticleSystemIndex, int _ParticleIndex, int _BodyIndex, int _FixtureIndex, Vector2 _Normal){
		ParticleSystemIndex = _ParticleSystemIndex;
		ParticleIndex = _ParticleIndex;
		BodyIndex = _BodyIndex;
		FixtureIndex = _FixtureIndex;
		Normal = _Normal;
	}
}

//This class contains information about a particle / particle contact
public struct LPContactPartPart
{
	public int ParticleSystemIndex;
	public int ParticleAIndex;
	public int ParticleBIndex;	
}