using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

[System.Serializable]
public class DualMinMax
{
    public MinMax dist;
    public MinMax force;

    public bool IsBig(float distMag, float forceMag)
    {
        float t = Mathf.InverseLerp(dist.min, dist.max, distMag);
        float threshold = Mathf.Lerp(force.min, force.max, t);
        if (forceMag > threshold)
            return true;
        return false;
    }
}

public class AI_Flyer : AI
{

  
}
