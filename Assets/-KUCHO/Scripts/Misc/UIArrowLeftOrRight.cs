using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIArrowLeftOrRight : MonoBehaviour
{
    public Renderer otherRend;
    public bool left;
    public Vector2 offset;

    public void MyUpdate(bool activate)
    {
        gameObject.SetActive(activate);

        if (otherRend)
        {
            Vector3 newPos;
            if (left)
            {
                newPos.x = otherRend.bounds.min.x - offset.x;
            }
            else
            {
                newPos.x = otherRend.bounds.max.x + offset.x;
            }
            newPos.y = otherRend.bounds.center.y + offset.y;
            newPos.z = transform.position.z;

            transform.position = newPos;
        }
    }
}
