using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Profiling;


public class LPContactListener 
{
	private IntPtr ContactListenerPtr;
	
	/// <summary>
	/// Array of contact data for fixture / fixture contacts that took place during the last step. This is updated on FixedUpdate</summary>	
	public LPContactFixFix[] FixtureFixtureContacts;
	/// <summary>
	/// Array of contact data for fixture / particle contacts that took place during the last step. This is updated on FixedUpdate</summary>
	public LPContactPartFix[] ParticleFixtureContacts;
	/// <summary>
	/// Array of contact data for particle / particle contacts that took place during the last step. This is updated on FixedUpdate</summary>
	public LPContactPartPart[] ParticleParticleContacts;
	
	private int ff = 11;
	private int pf = 6;
	private int pp = 3;
	
	/// <summary>
	/// Get the pointer to the C++ object represented by this object (in this case the contact listener)</summary>
	public IntPtr GetPtr()
	{
		return ContactListenerPtr;
	}
	
}

public class LPKuchoContactListener // KUCHO HACK
{
	private IntPtr ContactListenerPtr;

	private int ff = 11; // cada colision fixture fixture ocupa 11 floats
	private int pf = 6; // cada colision particula fixture ocupa 6 bytes
	private int pp = 3; // cada colision particula particula ocupa 3 bytes

	float[] allinfo = new float[(5000 * 6) + 3 + 1000]; // 5000 particulas * 6 floats que ocupa cada colision + 3 floats de cabecera pongo un extra de 1000 por si acaso las otras colisiones siman mas datos

	public IntPtr GetPtr()
	{
		return ContactListenerPtr;
	}

	float[] info = new float[3];

}
