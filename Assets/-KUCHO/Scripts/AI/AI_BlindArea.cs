using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AI_BlindArea : MonoBehaviour {

    public string[] tags;
    [ReadOnly2Attribute] public List<Collider2D> detected;
    [ReadOnly2Attribute] public Vision vision;


}
