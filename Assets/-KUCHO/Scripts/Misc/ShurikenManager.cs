using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShurikenManager : MonoBehaviour
{

    public ParticleSystem ps;
	public bool looping = true;
    public Vector3 inputToPosMax;
    Vector3 originalPos;
    Vector3 _inputToPosMax;
    [Range (0,1200)] public RangeFloat speed;
    [Range (0,300)] public RangeFloat amount;
    [ReadOnly2Attribute] public float input;
    ParticleSystem.MainModule main;
    [ReadOnly2Attribute] public Transform myTransform;

    public void InitialiseInEditor()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Awake()
    {
        originalPos = myTransform.localPosition;
        _inputToPosMax = originalPos + inputToPosMax;
    }

    public void Initialise()
    {
        myTransform = transform;
        main = ps.main;
        Stop();
    }

    public void MyUpdate(float _input)
    {
        // input valor de 0 a 1

        input = _input;

        var pos = Vector3.Lerp(originalPos, _inputToPosMax, input);
        myTransform.localPosition = pos;
        ParticleSystem.EmissionModule emission = ps.emission;
        float currentEmissionRatio = Mathf.Lerp(amount.min, amount.max, input);
        currentEmissionRatio /= amount.max;
        ParticleSystem.MinMaxCurve emissionRateCurve = emission.rateOverTime;
        emissionRateCurve.constantMin = amount.min * currentEmissionRatio;
        emissionRateCurve.constantMax = amount.max * currentEmissionRatio;
        emission.rateOverTime = emissionRateCurve;

        float currentSpeedRatio = Mathf.Lerp(speed.min, speed.max, input);
        currentSpeedRatio /= speed.max;
        ParticleSystem.MinMaxCurve startSpeedCurve = main.startSpeed;
        startSpeedCurve.constantMin = speed.min * currentSpeedRatio;
        startSpeedCurve.constantMax = speed.max * currentSpeedRatio;
        main.startSpeed = startSpeedCurve;

    }

    public void Stop(){
//        var main = ps.main;
//        main.startSpeedMultiplier = 0;
		if (looping)
		{
			main.loop = false;
		}
		ps.Stop();
    }

    public void Play()
    {
        ps.Play();
        main.loop = looping;
    }
}
