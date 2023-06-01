using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxWeakener : MonoBehaviour
{
    private float energyToSet = 1;
    public void InitialiseInEditor()
    {
        gameObject.layer = Layers.teleport;
        var col = GetComponent<Collider2D>();
        if (col)
            col.isTrigger = true;
    }
}
