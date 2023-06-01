using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OrdenaDamageLUT : MonoBehaviour
{
    public string notas;
    public List<Vector2> datos;
    public List<Quaternion> datos4;

    
    public void Ordena()
    {
        datos.Sort((a, b) => a.x.CompareTo((b.x)));
        datos4.Sort((a, b) => a.x.CompareTo((b.x)));
    }

}
