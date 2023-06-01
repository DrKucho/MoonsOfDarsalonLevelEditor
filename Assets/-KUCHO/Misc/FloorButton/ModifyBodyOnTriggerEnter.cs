using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyBodyOnTriggerEnter : MonoBehaviour
{
    public float dragMult = 1;
    public float massMult = 1;
    public float gravityScaleMult = 1;

    void OnTriggerEnter2D(Collider2D col)
    {
        var body = col.attachedRigidbody;
        if (body)
        {
            if (dragMult != 1)
                body.drag *= dragMult;
            if (massMult != 1)
                body.mass *= massMult;
            if (gravityScaleMult != 1)
                body.gravityScale *= gravityScaleMult;
        }
    }

    // Update is called once per frame
    void OnTriggerExit2D(Collider2D col)
    {
        var body = col.attachedRigidbody;
        if (body)
        {
            if (dragMult != 1)
                body.drag /= dragMult;
            if (massMult != 1)
                body.mass /= massMult;
            if (gravityScaleMult != 1)
                body.gravityScale /= gravityScaleMult;
        }
    }
}
