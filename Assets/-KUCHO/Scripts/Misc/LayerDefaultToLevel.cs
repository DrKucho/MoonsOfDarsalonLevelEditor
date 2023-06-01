using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerDefaultToLevel : MonoBehaviour
{
    // Start is called before the first frame update
    public void InitialiseInEditor()
    {
        var all = GetComponentsInChildren<Renderer>();
        foreach(Renderer r in all)
            if (r.gameObject.layer == Layers.defaultLayer)
                r.gameObject.layer = Layers.level;
    }
}
