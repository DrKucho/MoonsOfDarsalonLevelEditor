using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PreventCollisionForce { // info recogida de los contactosde nuestros colliders para crear una fuerza repeledora
    public Vector2 dir; // la direccion que ha de tener la fuerza (normalizada)
    public Vector2 point; // el punto en world space
    public float contactSeparation; // la penetracion que ha sufrido el collider a mas separacion mas penetracion y mas fuerza hay que aplicar (se guarda en negativo por que viene asi)
    public float contactCount; // cuantos contactos tenia la colision que gneró este dato, a mas contactos mas fuerza se aplicará
    public int destructibleContactCount; // cuantos contactos tenia la colision que gneró este dato, a mas contactos mas fuerza se aplicará

    public PreventCollisionForce() {
        dir = Constants.zero2;
        point = Constants.zero2;
        contactCount = 0;
        contactSeparation = 0;
    }
    public PreventCollisionForce(Vector2 normal, Vector2 point, float contactSeparation, int forceMultiplier)
    {
        this.dir = normal;
        this.point = point;
        this.contactCount = forceMultiplier;
        this.contactSeparation = contactSeparation;
    }
}
[RequireComponent(typeof(Rigidbody2D))] // necesita uno que ha de estar unido con un fixed joint al rb Principal , esto es asi para que se mueva con el pero que mis colliders me reporten solo a mi
public class PreventCollisioner : MonoBehaviour
{
 
}
