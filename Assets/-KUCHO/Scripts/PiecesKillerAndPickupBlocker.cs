using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesKillerAndPickupBlocker : MonoBehaviour
{
    public void InitialiseInEditor()
    {
        gameObject.layer = Layers.teleport;
        var col = GetComponent<Collider2D>();
        if (col)
            col.isTrigger = true;
    }


}
