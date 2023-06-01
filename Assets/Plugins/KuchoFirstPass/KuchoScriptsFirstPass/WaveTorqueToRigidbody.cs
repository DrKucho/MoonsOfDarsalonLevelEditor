using System;
using UnityEngine;
using System.Collections;
using System.ComponentModel;


public class WaveTorqueToRigidbody : MonoBehaviour
{
    public bool debug;

    public float torque = 100f;
    public float maxAngle = 20;
    public Transform useMyScaleXasMult;
    public bool invertMult;
    [ReadOnly2Attribute] public float _torque;
    [ReadOnly2Attribute] public float _torqueSign;
    [ReadOnly2Attribute] public float scaleMult;
    [ReadOnly2Attribute] public float rotZ;
    [ReadOnly2Attribute] public Rigidbody2D rb;

    private void Awake()
    {
        CalculateThings();
    }

    public void InitialiseInEditor()
    {
        rb = GetComponent<Rigidbody2D>();
        var myself = useMyScaleXasMult.GetComponentInChildren<WaveTorqueToRigidbody>();
        if (myself == this)
        {// soy hijo del transform useMyuScaleXasMult , todo bien
        }
        else // el transform no es padre mio? no permitido, reseteo
        {
            useMyScaleXasMult = transform;
        }

        if (!useMyScaleXasMult)
        {
            useMyScaleXasMult = transform;
        }
    }

    private void OnValidate()
    {
        CalculateThings();
    }
    private void CalculateThings()
    {
        scaleMult = Mathf.Sign(useMyScaleXasMult.localScale.x);
        if (invertMult)
            scaleMult *= -1;
        torque = Math.Abs(torque); // no puede ser negativa 
        _torque = torque * scaleMult;
        _torqueSign = Mathf.Sign(_torque);
    }
    public void Update ()
    {
        var rot = transform.localRotation.eulerAngles;
        rotZ = rot.z;
        if (rotZ > 180)
            rotZ = rotZ - 360;
        if (_torqueSign == scaleMult && rotZ > maxAngle)
        {
            _torque = torque * scaleMult;
            if (debug)
                Debug.Log("ROTX=" + rotZ + " TORQUE=" + _torque);
        }
        else if (_torqueSign == scaleMult && rotZ < -maxAngle)
        {
            _torque = -torque * scaleMult;
            if (debug)
                Debug.Log("ROTX=" + rotZ + " TORQUE=" + _torque);
        }

		
        rb.AddTorque(_torque);
    }
}
