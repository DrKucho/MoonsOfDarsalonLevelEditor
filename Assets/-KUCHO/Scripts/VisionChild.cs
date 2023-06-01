using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionChild : MonoBehaviour
{
    public Vision vision;
    public void InitialiseInEditor()
    {
        vision = GetComponentInParent<Vision>();
    }
    
}
