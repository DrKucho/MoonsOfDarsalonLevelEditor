using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnableDisableOnNpcTriggerEnter : MonoBehaviour
{
    public ArmyType armyType;
    public GameObject activeOnTrigger;
    public bool enterMeansEnable = true;
    public GameObject reverseActivation;


    TriggerColliders trigCol;
    Collider2D lastCol;

}
