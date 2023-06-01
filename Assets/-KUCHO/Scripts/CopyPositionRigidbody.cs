using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CopyPositionRigidbody : MonoBehaviour {


    public Transform transformToCopy;
    public float speed = 80;
    public float snapThreshold = 2;
    
    [ReadOnly2Attribute] public Rigidbody2D rb;
    
    [ReadOnly2Attribute] public Vector2 pos;
    [ReadOnly2Attribute] public Vector2 previousPos;
    [ReadOnly2Attribute] public Vector2 posDiff;
    [ReadOnly2Attribute] public Vector2 absPosDiff;
    [ReadOnly2Attribute] public Vector2 movement;
    [ReadOnly2Attribute] public Vector2 absMovement;
    [ReadOnly2Attribute] public Vector2 newVelocity;
    [ReadOnly2Attribute] public Vector2 previousVelocity;
    [ReadOnly2Attribute] public Vector2 lerpedVelocity;

    public void InitialiseInEditor()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!transformToCopy)
            transformToCopy = transform.parent;
        MoveTransform();
    }
    void OnEnable()
    {
        pos.x = transformToCopy.position.x;
        pos.y = transformToCopy.position.y;
        previousPos = pos;
	}

    
    public void MoveTransform()
    {
        transform.position = transformToCopy.position;
    }

    void FixedUpdate () {
        previousPos = pos;
        previousVelocity = newVelocity;

        pos.x = transformToCopy.position.x;
        pos.y = transformToCopy.position.y;
        posDiff.x = pos.x - rb.position.x;
        posDiff.y = pos.y - rb.position.y;
        absPosDiff.x = Mathf.Abs(posDiff.x);
        absPosDiff.y = Mathf.Abs(posDiff.y);

        movement.x = pos.x - previousPos.x;
        movement.y = pos.y - previousPos.y;
        absMovement.x = Mathf.Abs(movement.x);
        absMovement.y = Mathf.Abs(movement.y);
        if (absPosDiff.x > snapThreshold || absPosDiff.y > snapThreshold)
        {
            rb.MovePosition(pos);
            rb.velocity = Constants.zero2;

        }
        else
        {
            
            newVelocity.x = posDiff.x * speed;
            newVelocity.y = posDiff.y * speed;

            lerpedVelocity.x = Mathf.Lerp(newVelocity.x, previousVelocity.x, 0.5f);
            lerpedVelocity.y = Mathf.Lerp(newVelocity.y, previousVelocity.y, 0.5f);


            rb.velocity = lerpedVelocity;
        }
    }
}
