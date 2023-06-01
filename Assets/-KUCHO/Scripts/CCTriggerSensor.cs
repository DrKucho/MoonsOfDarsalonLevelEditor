using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CCTriggerSensor : MonoBehaviour
{
    public CapsuleCollider2D capsule;
    public CircleCollider2D circle;
    public BoxCollider2D box;

    [Header("(cambia a modo debug para editar)")]
    public ContactFilter2D contactFilter;
    [ReadOnly2Attribute] public int resultCount;
    [ReadOnly2Attribute] public Collider2D[] results;
    [ReadOnly2Attribute] public bool found;
    public void InitialiseInEditor(){
        // solo permito un tipo de collider, tiene mas prioridad capsula,  luego circulo
        capsule = null;
        circle = null;
        box = null;
        capsule = GetComponent<CapsuleCollider2D>();
        if (!capsule)
        {
            circle = GetComponent<CircleCollider2D>();
            if (!circle)
                box = GetComponent<BoxCollider2D>();
        }
        if (results.Length == 0)
            results = new Collider2D[5];
        for (int i = 0; i < results.Length; i++)
            results[i] = null;
    }
    private void OnEnable()
    {
        if (capsule)
            capsule.enabled = false;
        if (circle)
            circle.enabled = false;
        if (box)
            box.enabled = false;
    }
    
    public bool MyUpdate(){
        if (capsule)
        {
            Vector2 point;
            point.x = transform.position.x + capsule.offset.x;
            point.y = transform.position.y + capsule.offset.y;

            resultCount = Physics2D.OverlapCapsule(point, capsule.size, capsule.direction, transform.eulerAngles.z, contactFilter, results);
        }
        else if (circle)
        {
            Vector2 point;
            point.x = transform.position.x + circle.offset.x;
            point.y = transform.position.y + circle.offset.y;

            resultCount = Physics2D.OverlapCircle(point, circle.radius, contactFilter, results);
        }
        else if (box)
        {
            resultCount = Physics2D.OverlapArea(new Vector2(box.bounds.min.x, box.bounds.min.y), new Vector2(box.bounds.max.x, box.bounds.max.y), contactFilter, results);
        }
        if (resultCount > 0)
        {
            found = true;
        }
        else
        {
            found = false;
        }
        return found;
    }
}
