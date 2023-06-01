using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlinkCollider : MonoBehaviour
{
    public float delay = 1;
    public float randomOffset = 0;
    public Collider2D col;
    //apague y encienda el collider cada x segundos para colocar en objetos como force AI y conseguir que los que eten parados reciban una orden
    public void InitialiseInEditor() {
        col = GetComponent<Collider2D>();
    }

}
