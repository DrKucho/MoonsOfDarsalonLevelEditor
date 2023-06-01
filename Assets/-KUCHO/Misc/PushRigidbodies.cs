using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushRigidbodies : MonoBehaviour
{
    public Vector2 force;

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.attachedRigidbody)
        {
            col.attachedRigidbody.AddForce(force);
        }
    }
}
