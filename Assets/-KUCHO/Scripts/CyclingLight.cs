using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Light2D;


public class CyclingLight : MonoBehaviour
{

	public int inc;
	float runDelay; // 

	public float m_runFPS; // 

	public float runFPS
	{
		get { return m_runFPS; }
		set
		{
			m_runFPS = value;
			runDelay = 1 / value;
		}
	}

	public float vanishSpeed;
	float vanishDelay;

	public float m_vanishFPS; // 

	public float vanishFPS
	{
		get { return m_vanishFPS; }
		set
		{
			m_vanishFPS = value;
			vanishDelay = 1 / value;
		}
	}

	bool vanishing;

	float[] alpha;
	int index;
	bool runing;
	[ReadOnly2Attribute] public SWizSprite[] sprites;
	[ReadOnly2Attribute] public Light2DManager lightManager;

	public void InitialiseInEditor()
	{
		lightManager = GetComponentInChildren<Light2DManager>();
		sprites = GetComponentsInChildren<SWizSprite>();
	}

	private void OnValidate()
	{
		runFPS = m_runFPS;
	}

}
