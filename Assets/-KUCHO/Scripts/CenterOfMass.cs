using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public Rigidbody2D rb;
    public void InitialiseInEditor()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void OnEnable()
    {
        rb.centerOfMass = rb.position - (Vector2) transform.position;
    }
}
